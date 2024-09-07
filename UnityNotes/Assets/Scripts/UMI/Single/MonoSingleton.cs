using UnityEngine;
namespace UMI
{

    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 单例实例不添加在一起，不好看组件
                    GameObject goRoot = GetOrCreateRootObj();
                    GameObject goInst = new GameObject(typeof(T).Name);
                    goInst.transform.SetParent(goRoot.transform);
                    T t = goInst.AddComponent<T>();

                    SetInstance(t);
                }

                return _instance;
            }
        }

        protected static GameObject GetOrCreateRootObj()
        {
            GameObject obj = GameObject.Find("MonoSingleton");
            if (null == obj)
            {
                obj = new GameObject("MonoSingleton");
                DontDestroyOnLoad(obj);
            }

            return obj;
        }

        protected virtual void Awake()
        {
            SetInstance(this as T);

            DontDestroyOnLoad(this.gameObject);

            if (this is IUpdate)
            {
              //  GameMain.Instance.AddModule(this as IUpdate);
            }

            //这里将IUpdate和IModule分开，为了职责各单一
            if (this is IModule)
            {
               // GameMain.Instance.AddModule(this as IModule);
            }
        }

        /// <summary>
        /// 初始化
        /// 没任何一样，只是让外部有个调用方法的假象
        /// </summary>
        public void Init()
        {
            SetInstance(this as T);
        }

        /// <summary>
        /// 挂载到单例根节点上
        /// 有些节点走预制体创建，而不是直接new 一个GameObject
        /// </summary>
        public void MountToRootNode()
        {
            GameObject goRoot = GetOrCreateRootObj();
            this.transform.SetParent(goRoot.transform);
        }

        /// <summary>
        /// 自定义初始化行为
        /// </summary>
        protected virtual void OnInit()
        {

        }

        /// <summary>
        /// 设置实例
        /// </summary>
        /// <param name="t"></param>
        private static void SetInstance(T t)
        {
            if (_instance != null)
            {
                return;
            }

            _instance = t;
            _instance.OnInit();

            //if (_instance is ILoginManager)
            //{
            //    LoginMgr.Instance.RegisterLoginModule(_instance as ILoginManager);
            //}
        }
    }
}

