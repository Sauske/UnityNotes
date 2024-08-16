using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Framework
{
    /// <summary>
    /// 对象池接口，用于隔离泛型
    /// </summary>
    public interface IObjPoolCtrl
    {
        void Release(PooledClassObject obj);

#if UNITY_EDITOR
        void ClearUnused();
        int total { get; }
        int frees { get; }
#endif
    }

    /// <summary>
    /// 可池化的基类
    /// </summary>
    public class PooledClassObject
    {
#if UNITY_EDITOR
        public bool bInPool = false;
#endif
        //对象SeqNum，用于跟踪Handle有效性
        public UInt32 usingSeq = 0;
        //Pool分配器
        public IObjPoolCtrl holder;
        //是否要做字段复位检查
        public bool bChkReset = true;

        //对象从池中被启用
        public virtual void OnUse()
        {
        }
        //对象还回池中善后
        public virtual void OnRelease()
        {
        }

        public void Release()
        {
#if UNITY_EDITOR
            if (bInPool)
            {
                DebugHelper.LogError(string.Format("对象已经在池子里面了.. obj:{0}", GetType().Name));
            }
#endif

            if (holder != null)
            {
                OnRelease();
                holder.Release(this);
            }
        }
    }

    /// <summary>
    /// 弱引用机制的访问器，
    /// 为true可使用，禁止私自保存handle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PoolObjHandle<T> : IEquatable<PoolObjHandle<T>> where T : PooledClassObject
    {
        public UInt32 _handleSeq;
        public T _handleObj;

        public PoolObjHandle(T obj)
        {
            if (obj != null && obj.usingSeq > 0)
            {
                _handleSeq = obj.usingSeq;
                _handleObj = obj;
            }
            else
            {
                _handleSeq = 0;
                _handleObj = null;
            }
        }

        public void Validate()
        {
            _handleSeq = _handleObj != null ? _handleObj.usingSeq : 0;
        }

        //释放引用
        public void Release()
        {
            _handleObj = null;
            _handleSeq = 0;
        }

        //判断引用是否有效
        public static implicit operator bool(PoolObjHandle<T> ptr)
        {
            return ptr._handleObj != null && ptr._handleObj.usingSeq == ptr._handleSeq;
        }

        public static bool operator ==(PoolObjHandle<T> lhs, PoolObjHandle<T> rhs)
        {
            return lhs._handleObj == rhs._handleObj && lhs._handleSeq == rhs._handleSeq;
        }

        public static bool operator !=(PoolObjHandle<T> lhs, PoolObjHandle<T> rhs)
        {
            return lhs._handleObj != rhs._handleObj || lhs._handleSeq != rhs._handleSeq;
        }

        public bool Equals(PoolObjHandle<T> other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                this.GetType() == obj.GetType() &&
                this == (PoolObjHandle<T>)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        //自动转换成原始对象引用
        public static implicit operator T(PoolObjHandle<T> ptr)
        {
            return ptr.handle;
        }

        //获取对象引用
        public T handle
        {
            get
            {
#if UNITY_EDITOR && !SGAME_PROFILE
                DebugHelper.Assert(_handleObj != null && _handleObj.usingSeq == _handleSeq);
#endif
                return _handleObj;
            }
        }
    }

#if UNITY_EDITOR
    public class ClassObjPoolRepository : Singleton<ClassObjPoolRepository>
    {
        public List<KeyValuePair<Type, IObjPoolCtrl>> Repositories = new List<KeyValuePair<Type, IObjPoolCtrl>>();

        public void Add(Type InType, IObjPoolCtrl InCtrl)
        {
            Repositories.Add(new KeyValuePair<Type, IObjPoolCtrl>(InType, InCtrl));
        }

        public void Clear()
        {
            for (int i = 0; i < Repositories.Count; ++i)
            {
                Repositories[i].Value.ClearUnused();
            }
        }
    }
#endif

    public abstract class ClassObjPoolBase : IObjPoolCtrl
    {
        /** Internal pool */
        protected List<object> pool = new List<object>(128);
        protected UInt32 reqSeq;

#if UNITY_EDITOR
        protected UInt32 totals;
#endif

        public abstract void Release(PooledClassObject obj);

        public int capacity
        {
            get
            {
                return pool.Capacity;
            }
            set
            {
                pool.Capacity = value;
            }
        }

#if UNITY_EDITOR
        public int total { get { return (int)totals; } }
        public abstract void ClearUnused();
        public abstract int frees { get; }
#endif
    }

    /// <summary>
    /// Pool对象分配器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassObjPool<T> : ClassObjPoolBase where T : PooledClassObject, new()
    {
        private static ClassObjPool<T> instance = null;

#if UNITY_EDITOR && !SGAME_PROFILE
        static T _default = new T();
#endif

        public static uint NewSeq()
        {
            if (instance == null)
            {
                instance = new ClassObjPool<T>();

#if UNITY_EDITOR
           //     ClassObjPoolRepository.instance.Add(typeof(T), instance);
#endif
            }

            instance.reqSeq++;
            return instance.reqSeq;
        }

        public static T Get()
        {
            if (instance == null)
            {
                instance = new ClassObjPool<T>();

#if UNITY_EDITOR
             //   ClassObjPoolRepository.instance.Add(typeof(T), instance);
#endif
            }

            if (instance.pool.Count > 0)
            {
                T ls = (T)instance.pool[instance.pool.Count - 1];
                instance.pool.RemoveAt(instance.pool.Count - 1);

#if UNITY_EDITOR
                ls.bInPool = false;
#endif

                instance.reqSeq++;
                ls.usingSeq = instance.reqSeq;
                ls.holder = instance;

                ls.OnUse(); 

#if UNITY_EDITOR && !SGAME_PROFILE && !SGAME_PROFILE_GC
                //编辑器下检查OnUse重置是否完整，如果从对象池中获得的对象存在脏数据可能是一个极大的不确定风险
                if (ls.bChkReset)
                {
                    System.Reflection.FieldInfo[] tar = ls.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    System.Reflection.FieldInfo[] ori = _default.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    DebugHelper.Assert(tar.Length == ori.Length, "对象池属性Reset字段不匹配");
                    for (int i = 0; i < ori.Length; i++)
                    {
                        if (tar[i].Name != "usingSeq" && tar[i].Name != "holder")
                        {
                            if (tar[i].Name != ori[i].Name)
                            {
                                DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                            }
                            else
                            {
                                var ftar = tar[i].GetValue(ls);
                                var fori = ori[i].GetValue(_default);
                                if (ftar is ICollection)
                                {
                                    if ((tar[i].FieldType != ori[i].FieldType))
                                    {
                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                    }
                                    else if (tar[i].FieldType.IsArray)
                                    {
                                        var atar = ftar as Array;
                                        var aori = fori as Array;
                                        if (atar.Length != aori.Length)
                                        {
                                            DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                        }
                                        else
                                        {
                                            for (int e = 0; e < atar.Length; e++)
                                            {
                                                var tempVal = atar.GetValue(e);
                                                //为了支持二维数组
                                                if (tempVal != null && tempVal.GetType().IsArray)
                                                {
                                                    var atarItem = atar.GetValue(e) as Array;
                                                    var aoriItem = aori.GetValue(e) as Array;
                                                    if(atarItem.Length != aoriItem.Length)
                                                    {
                                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                                    }
                                                    else
                                                    {
                                                        for(int ii = 0; ii < aoriItem.Length; ++ ii)
                                                        {
                                                            if (!System.Object.Equals(atarItem.GetValue(ii), aoriItem.GetValue(ii)))
                                                            {
                                                                DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (!System.Object.Equals(atar.GetValue(e), aori.GetValue(e)))
                                                {
                                                    DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((ftar as ICollection).Count != 0 || (fori as ICollection).Count != 0)
                                        {
                                            DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                        }
                                    }
                                }
                                else if (ftar is ListViewBase)
                                {
                                    if ((tar[i].FieldType != ori[i].FieldType))
                                    {
                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                    }
                                    else if ((ftar as ListViewBase).Count != 0 || (fori as ListViewBase).Count != 0)
                                    {
                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                    }
                                }
                                else if (ftar is ListValueViewBase )
                                {
                                    if ((tar[i].FieldType != ori[i].FieldType))
                                    {
                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                    }
                                    else if ((ftar as ListValueViewBase).Count != 0 || (fori as ListValueViewBase).Count != 0)
                                    {
                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                    }
                                }
                                else if (tar[i].FieldType.FullName.Contains("CrypticInt32"))
                                {
                                    int x = HackCrypticInt32Int(ftar);
                                    int y = HackCrypticInt32Int(fori);
                                    if (x!=y)
                                    {
                                        DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                    }
                                }
                                else if ((ftar != null && !ftar.Equals(fori)) || !System.Object.Equals(ftar, fori))
                                {
                                    DebugHelper.LogError(string.Format("[{0}]属性[{1}]不一致", ls.GetType().Name, tar[i].Name));
                                }
                            }
                        }
                    }
                }
#endif
                return ls;
            }
            else
            {
                var ls = new T();

                instance.reqSeq++;
                ls.usingSeq = instance.reqSeq;
                ls.holder = instance;

                ls.OnUse();

#if UNITY_EDITOR
                instance.totals++;
#endif

                return ls;
            }
        }

#if UNITY_EDITOR && !SGAME_PROFILE
        static int HackCrypticInt32Int( object InValue )
        {
            var Methods = InValue.GetType().GetMethods();

            for( int i=0; i<Methods.Length; ++i)
            {
                if( Methods[i].Name == "ToInt" )
                {
                    int Value = Convert.ToInt32(Methods[i].Invoke(InValue, null));

                    return Value;
                }
            }

            return 0;
        }
#endif
        
        public override void Release(PooledClassObject obj)
        {
            var tobj = obj as T;
#if (DEBUG) && !SGAME_PROFILE
            DebugHelper.Assert(tobj != null);
            for (int i = 0; i < pool.Count; i++)
                if (pool[i] == obj)
                    throw new System.InvalidOperationException("The object is released even though it is in the pool. Are you releasing it twice?");
#endif
            obj.usingSeq = 0;
            obj.holder = null;
            pool.Add(tobj);
#if UNITY_EDITOR
            obj.bInPool = true;
#endif
        }

#if UNITY_EDITOR
        public override void ClearUnused()
        {
            totals -= (uint)pool.Count;
            pool.Clear();
        }

        public override int frees { get { return instance.pool.Count; } }
#endif
    }


    public abstract class ProtocolObject
    {
        public abstract int GetClassID();

        //#if UNITY_EDITOR
        //        public bool released = false;
        //#endif

        //public virtual TdrError.ErrorType construct() { return TdrError.ErrorType.TDR_NO_ERROR; }
        //public virtual TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer) { return TdrError.ErrorType.TDR_NO_ERROR; }
        //public virtual TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer) { return TdrError.ErrorType.TDR_NO_ERROR; }

        public void Release()
        {
            //#if UNITY_EDITOR
            //            released = true;
            //#endif
            OnRelease();

            ProtocolObjectPool.Release(this);
        }

        public virtual void OnUse()
        {

        }

        public virtual void OnRelease()
        {

        }
    }

    public class ProtocolObjectPool
    {
        public int ClassID;
        public Type ClassType;
        
        public List<ProtocolObject> unusedObjs = new List<ProtocolObject>(128);

        static List<ProtocolObjectPool> poolList = new List<ProtocolObjectPool>(1024);

#if UNITY_EDITOR
        public int total { get; private set; }
        public int frees { get { return unusedObjs.Count; } }
#endif

        public static ProtocolObject Get(int ClassID)
        {
            var pool = poolList[ClassID];

            if (pool.unusedObjs.Count > 0)
            {
                int idx = pool.unusedObjs.Count - 1;
                ProtocolObject obj = pool.unusedObjs[idx];
                pool.unusedObjs.RemoveAt(idx);
//#if UNITY_EDITOR
//                obj.released = false;
//#endif
                obj.OnUse();
                return obj;
            }
            else
            {
#if UNITY_EDITOR
                pool.total++;
#endif
                return (ProtocolObject)Activator.CreateInstance(pool.ClassType);
            }
        }

        public static void Release(ProtocolObject obj)
        {
            int objID = obj.GetClassID();
            poolList[objID].unusedObjs.Add(obj);
        }

        static ProtocolObjectPool()
        {
            Init();
        }

        public static int PoolCount { get { return poolList.Count; } }
        public static ProtocolObjectPool GetPool(int index) { return poolList[index]; }

        public static void Init()
        {
            if (poolList.Count > 0)
                return;

            Type baseType = typeof(ProtocolObject);
            Type[] allTypes = baseType.Assembly.GetTypes();
            for (int i = 0; i < allTypes.Length; ++i)
            {
                Type type = allTypes[i];
                if (!type.IsAbstract && type.IsSubclassOf(baseType))
                {
                    var fi = type.GetField("CLASS_ID", BindingFlags.Static | BindingFlags.Public);
                    int objID = (int)fi.GetValue(null);

                    ProtocolObjectPool pool = new ProtocolObjectPool();
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
                    Debug.LogError(string.Format("ProtocolObjectPool[{0}] is null !", i));
                }
            }
#endif
        }

        public static void Clear(int nReserve = 0)
        {
            for (int i = 0; i < poolList.Count; ++i)
            {
                var pool = poolList[i];

                if (nReserve == 0)
                {
                    pool.unusedObjs.Clear();
                }
                else
                {
                    int unusedObjCnt = pool.unusedObjs.Count;
                    int removeCnt = unusedObjCnt - nReserve;
                    if (removeCnt > 0)
                    {
                        pool.unusedObjs.RemoveRange(unusedObjCnt - removeCnt, removeCnt);
                    }
                }
            }
        }

        static void AddPool(ProtocolObjectPool pool)
        {
            if (poolList.Count <= pool.ClassID)
            {
                for (int i = poolList.Count; i <= pool.ClassID; ++i)
                {
                    poolList.Add(null);
                }
            }

            poolList[pool.ClassID] = pool;
        }
    }
}
