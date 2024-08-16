//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System.Collections;

namespace Qarth
{
    [System.Serializable]
    public class AppConfig : ScriptableObject
    {
        
        #region 枚举
        public enum APP_MODE
        {
            DebugMode,
            ReleaseMode,
        }

        public enum eServerMode
        {
            kLocal,
            kRemote
        }
        #endregion
        
        #region DebugSetting
        [System.Serializable]
        public class DebugSetting
        {
            public string m_DumpPath = null;
            public bool m_DumpToScreen = false;
            public bool m_DumpToFile = true;
        }
        #endregion
        
        #region 数据区
        public APP_MODE appMode;
        public bool loadInEditor;
        public DebugSetting debugSetting;
        public LogLevel logLevel = LogLevel.Max;
        public bool isGuideActive = false;
        #endregion

        #region 初始化过程
        private static AppConfig s_Instance;

        private static AppConfig LoadInstance()
        {
            ResLoader loader = ResLoader.Allocate("AppConfig", null);

            UnityEngine.Object obj = loader.LoadSync(ProjectPathConfig.appConfigPath);
            if (obj == null)
            {
                Log.w("Not Find App Config, Will Use Default App Config.");
                loader.ReleaseAllRes();
                obj = loader.LoadSync(ProjectPathConfig.DEFAULT_APP_CONFIG_PATH);
                if (obj == null)
                {
                    Log.e("Not Find Default App Config File!");
                    loader.Recycle2Cache();
                    loader = null;
                    return null;
                }
            }

            //Log.i("Success Load App Config.");
            s_Instance = obj as AppConfig;

            AppConfig newAB = GameObject.Instantiate(s_Instance);

            s_Instance = newAB;

            loader.Recycle2Cache();

            return s_Instance;
        }

        #endregion

        public static AppConfig S
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = LoadInstance();
                }

                return s_Instance;
            }
        }

        public void InitAppConfig()
        {
            Log.i("Init[AppConfig]");
            Log.Level = logLevel;
        }
    }

}

