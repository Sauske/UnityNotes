using System;
using UnityEngine;

namespace SFramework
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        protected static T _instance;
        private static readonly object _locker = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            _instance.OnInit();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 自定义初始化行为
        /// </summary>
        protected virtual void OnInit()
        {
            if (this is IUpdate)
            {
                ModuleMgr.AddUpdate(this as IUpdate);
            }

            if (this is IModule)
            {
                ModuleMgr.AddModule(this as IModule);
            }

            if (this is ILogin)
            {
                ModuleMgr.AddLogin(this as ILogin);
            }
        }
    }
}