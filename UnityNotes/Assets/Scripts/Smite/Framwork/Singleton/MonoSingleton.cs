using UnityEngine;
using System;

namespace Framework
{
    public class AutoMonoSingletonAttribute : Attribute
    {
        public bool bAutoCreate;

        public AutoMonoSingletonAttribute(bool bCreate)
        {
            bAutoCreate = bCreate;
        }
    }


    [AutoMonoSingleton(true)]
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {

        private static T _instance;

        private static bool _destroyed;


        public static T GetInstance()
        {
            if (_instance == null && !_destroyed)
            {
                Type theType = typeof(T);

                _instance = (T)FindObjectOfType(theType);

                if (_instance == null)
                {
                    object[] Attributes = theType.GetCustomAttributes(typeof(AutoMonoSingletonAttribute), true);
                    if (Attributes.Length > 0)
                    {
                        var bAutoCreate = ((AutoMonoSingletonAttribute)Attributes[0]).bAutoCreate;
                        if (!bAutoCreate)
                            return null;
                    }

                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();

                    GameObject bootObj = GameObject.Find("BootObj");
                    if (bootObj != null)
                    {
                        go.transform.SetParent(bootObj.transform);
                    }
                }
            }

            return _instance;
        }

        public static T Instance
        {
            get
            {
                return GetInstance();
            }
        }

        public static void DestroyInstance()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
            _destroyed = true;
            _instance = null;
        }

        public static void ClearDestroy()
        {
            DestroyInstance();

            _destroyed = false;
        }


        public virtual void Awake()
        {
            if (_instance != null && _instance.gameObject != gameObject)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);  //UNITY_EDITOR
                }
            }
            else if (_instance == null)
            {
                _instance = GetComponent<T>();
            }

            DontDestroyOnLoad(gameObject);

            Init();
        }

        public virtual void OnDestroy()
        {
            if (_instance != null && Instance.gameObject == gameObject)
            {
                _instance = null;
            }
        }



        public virtual void Init()
        {

        }


        public static bool HasInstance()
        {
            return _instance != null;
        }
    }
}
