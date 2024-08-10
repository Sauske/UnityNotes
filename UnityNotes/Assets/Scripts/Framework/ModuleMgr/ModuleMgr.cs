using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UMI
{

    public class ModuleMgr
    {
        private static Dictionary<string, BaseModule> modules = new Dictionary<string, BaseModule>();

        static bool ResInited = false;

        public static void Initialize()
        {
            InitModule();
            //if (AppConst.DebugMode)
            //{
            //    InitModule();

            //    ResInited = true;
            //}
            //else
            //{
            //    ResourceManager.GetInstance().Init();

            //    ResourceManager.GetInstance().InitResMainfest(AppConst.ResIndexFile, delegate ()
            //    {
            //        DebugHelper.Log("InitModule");
            //        InitModule();

            //        ResInited = true;
            //    });
            //}
        }




        private static void OnResInitOk()
        {
            SceneManager.LoadSceneAsync("Game");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode model)
        {

        }

        private static void InitModule()
        {

        }

        static void DllInitCallback()
        {

        }

        // Update is called once per frame
        public static void OnUpdate()
        {
            foreach (KeyValuePair<string, BaseModule> pair in modules)
            {
                pair.Value.OnUpdate();
            }
        }

        public static void OnDestroy()
        {
            foreach (KeyValuePair<string, BaseModule> pair in modules)
            {
                pair.Value.OnDispose();
            }
            modules.Clear();

            SceneManager.sceneLoaded -= OnSceneLoaded;

            ResInited = false;
        }

        public static void OnApplicationFocus(bool focus)
        {

        }

        static void AddModules()
        {
            //AddModule<BagModule>();
            //AddModule<ChatModule>();
            //AddModule<MailModule>();
            //AddModule<PlayerInfoModule>();
            //AddModule<SettingModule>();

        }

        static T AddModule<T>() where T : BaseModule
        {
            var type = typeof(T);
            var obj = (T)Activator.CreateInstance(type);
            modules.Add(type.Name, obj);
            return obj;
        }

        public static T GetModule<T>() where T : BaseModule
        {
            var type = typeof(T);
            BaseModule module = null;
            if (modules.TryGetValue(type.Name, out module))
            {
                return (T)module;
            }
            else
            {
                return AddModule<T>();
            }
        }

        #region 解析Dll


        public static IEnumerator DownLoadDlls(Action onDownloadComplete)
        {
            yield return null;

            SceneManager.LoadSceneAsync("Game");

            yield return null;

            if (onDownloadComplete != null)
                onDownloadComplete();
        }
        #endregion
    }
}
