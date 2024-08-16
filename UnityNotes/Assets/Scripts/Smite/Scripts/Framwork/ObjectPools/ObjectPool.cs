using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Framework
{
    public abstract class CObject
    {
        public abstract int GetClassID();
        
        public void Release()
        {

        }

        public virtual void OnUse()
        {

        }

        public virtual void OnRelease()
        {

        }
    }

    public class ObjectPool
    {
        public int ClassID;

        public Type ClassType;

        public List<CObject> unusedObjs = new List<CObject>(128);

        static List<ObjectPool> poolList = new List<ObjectPool>(1024);

        public int total { get; private set; }

        public int frees { get { return unusedObjs.Count; } }

        public static int PoolCount { get { return poolList.Count; } }

        public static ObjectPool GetPool(int index) { return poolList[index]; }

        public CObject Get(int ClassID)
        {
            var pool = poolList[ClassID];

            if(pool.unusedObjs.Count > 0)
            {
                int idx = pool.unusedObjs.Count - 1;
                CObject obj = pool.unusedObjs[idx];
                pool.unusedObjs.RemoveAt(idx);

                obj.OnUse();
                return obj;
            }
            else
            {
                pool.total++;

                return (CObject)Activator.CreateInstance(pool.ClassType);
            }
        }

        public static void Release(CObject obj)
        {
            int objID = obj.GetClassID();
            poolList[objID].unusedObjs.Add(obj);
        }

        static ObjectPool()
        {
            Init();
        }

        public static void Init()
        {
            if (poolList.Count > 0) return;

            Type baseType = typeof(CObject);

            Type[] allType = baseType.Assembly.GetTypes();
            for(int idx = 0; idx < allType.Length;idx++)
            {
                Type type = allType[idx];

                if(!type.IsAbstract && type.IsSubclassOf(baseType))
                {
                    var fi = type.GetField("CLASS_ID",BindingFlags.Static | BindingFlags.Public);

                    int objID = (int)fi.GetValue(null);

                    ObjectPool pool = new ObjectPool();
                    pool.ClassType = type;
                    pool.ClassID = objID;

                    AddPool(pool);
                }
            }
        }

        static void AddPool(ObjectPool pool)
        {
            if(poolList.Count <= pool.ClassID)
            {
                for(int idx = poolList.Count;idx <= pool.ClassID;idx++)
                {
                    poolList.Add(null);
                }
            }

            poolList[pool.ClassID] = pool;
        }

        public static void Clear(int nReserve = 0)
        {
            for(int idx = 0; idx < poolList.Count;idx++)
            {
                var pool = poolList[idx];

                if(nReserve == 0)
                {
                    pool.unusedObjs.Clear();
                }
                else
                {
                    int unusedObjCnt = pool.unusedObjs.Count;
                    int removeCnt = unusedObjCnt - nReserve;

                    if(removeCnt > 0)
                    {
                        pool.unusedObjs.RemoveRange(unusedObjCnt - removeCnt, removeCnt);
                    }
                }
            }
        }
    }
}