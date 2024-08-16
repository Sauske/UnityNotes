using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public abstract class RGameProtocolObject
    {
        public abstract int GetClassID();


        public virtual CommError.Type construct() { return CommError.Type.COMM_NO_ERROR; }

        public virtual CommError.Type pack(ref CommWriteBuf destBuf) { return CommError.Type.COMM_NO_ERROR; }

        public virtual CommError.Type unpack(ref CommReadBuf srcBuf) { return CommError.Type.COMM_NO_ERROR; }

        public void Release()
        {
            OnRelease();

            RGameProtocolObjectPool.Release(this);
            
        }

        public virtual void OnUse()
        {

        }

        public virtual void OnRelease()
        {

        }
    }

    public class RGameProtocolObjectPool
    {
        public int ClassID;
        public Type ClassType;

        public List<RGameProtocolObject> unusedObjs = new List<RGameProtocolObject>(128);

        static List<RGameProtocolObjectPool> poolList = new List<RGameProtocolObjectPool>(1024);

        public int total { get; private set; }

        public int frees { get { return unusedObjs.Count; } }

        public static int PoolCount { get { return poolList.Count; } }


        public static RGameProtocolObjectPool GetPool(int index) { return poolList[index]; }

        public static RGameProtocolObject Get(int ClassID)
        {
            var pool = poolList[ClassID];

            if(pool.unusedObjs.Count > 0)
            {
                int idx = pool.unusedObjs.Count - 1;
                RGameProtocolObject obj = pool.unusedObjs[idx];
                pool.unusedObjs.RemoveAt(idx);

                obj.OnUse();
                return obj;
            }else
            {
                pool.total++;

                return (RGameProtocolObject)Activator.CreateInstance(pool.ClassType);
            }
        }

        public static void Release(RGameProtocolObject obj)
        {
            int objID = obj.GetClassID();
            poolList[objID].unusedObjs.Add(obj);
        }

        static RGameProtocolObjectPool()
        {
            Init();
        }


        public static void Init()
        {
            if (poolList.Count > 0) return;


            Type baseType = typeof(RGameProtocolObject);

            Type[] allTypes = baseType.Assembly.GetTypes();
            for(int idx =0;idx < allTypes.Length;idx++)
            {
                Type type = allTypes[idx];

                if(!type.IsAbstract && type.IsSubclassOf(baseType))
                {
                    var fi = type.GetField("CLASS_ID", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                    int objID = (int)fi.GetValue(null);


                    RGameProtocolObjectPool pool = new RGameProtocolObjectPool();
                    pool.ClassType = type;
                    pool.ClassID = objID;

                    AddPool(pool);
                }
            }

#if UNITY_EDITOR
            for (int i = 0; i < poolList.Count; ++i)
            {
                if (poolList[i] == null)
                {
                    Debug.LogError(string.Format("RGameProtocolObjectPool[{0}] is null !", i));
                }
            }
#endif
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        /// <param name="nReserve">保留的个数</param>
        public static void Clear(int nReserve = 0)
        {
            for(int idx =0; idx < poolList.Count;idx++)
            {
                var pool = poolList[idx];

                if(nReserve == 0)
                {
                    pool.unusedObjs.Clear();
                }else
                {
                    int unusedObjCnt = pool.unusedObjs.Count;
                    int removeCnt = unusedObjCnt - nReserve;

                    if (removeCnt > 0)
                        pool.unusedObjs.RemoveRange(unusedObjCnt - removeCnt, removeCnt);
                }
            }
        }


        static void AddPool(RGameProtocolObjectPool pool)
        {
            if(poolList.Count <= pool.ClassID)
            {
                for (int idx = poolList.Count; idx <= pool.ClassID; idx++)
                    poolList.Add(null);
            }

            poolList[pool.ClassID] = pool;
        }
    }
}
