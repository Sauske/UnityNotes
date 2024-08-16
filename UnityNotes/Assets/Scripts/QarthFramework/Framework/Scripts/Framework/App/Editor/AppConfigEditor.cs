//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Qarth.Editor
{
    public class AppConfigEditor
    {
        [MenuItem("Assets/Qarth/Config/Build AppConfig")]
        public static void BuildAppConfig()
        {
            AppConfig data = null;
            string folderPath = PathHelper.GetResourcePath()+"/Config";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string configPath = folderPath + "/AppConfig.asset";
            if (!File.Exists(configPath))
            {
                configPath = PathHelper.GetAssetsRelatedPath(configPath);
                data = ScriptableObject.CreateInstance<AppConfig>();
                AssetDatabase.CreateAsset(data, configPath);
                Log.i("Create Project Config In Folder:" + configPath);
            }
            Log.i("Create App Config In Folder:" + configPath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
