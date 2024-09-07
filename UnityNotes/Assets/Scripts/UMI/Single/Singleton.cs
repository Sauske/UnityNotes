using UnityEngine;

namespace UMI
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
        /// 没有意义的接口，只是让外部以为在调用了初始化，看起来好看点
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 自定义初始化行为
        /// </summary>
        protected virtual void OnInit()
        {

        }

        public Singleton()
        {
            if (_instance != null)
            {
                Debug.LogError("Cannot have two instances in singleton");
                return;
            }
            _instance = (T)(System.Object)this;


            //if (this is IUpdate)
            //{
            //    GameMain.Instance.AddModule(this as IUpdate);
            //}

            //if (this is IModule)
            //{
            //    GameMain.Instance.AddModule(this as IModule);
            //}

            //if (this is ILoginManager)
            //{
            //    LoginMgr.Instance.RegisterLoginModule(this as ILoginManager);
            //}
        }
    }
}