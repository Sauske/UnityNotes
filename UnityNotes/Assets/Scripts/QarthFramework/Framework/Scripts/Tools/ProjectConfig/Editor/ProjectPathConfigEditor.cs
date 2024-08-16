//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace Qarth.Editor
{
    [CustomEditor(typeof(ProjectPathConfig), false)]
    public class ProjectPathConfigEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Qarth/Config/Build ProjectConfig")]
        public static void BuildProjectConfig()
        {
            ProjectPathConfig data = null;
            string folderPath = PathHelper.GetResourcePath()+"/Config";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string configPath = folderPath + "/ProjectConfig.asset";
            if (!File.Exists(configPath))
            {
                configPath = PathHelper.GetAssetsRelatedPath(configPath);
                data = ScriptableObject.CreateInstance<ProjectPathConfig>();
                AssetDatabase.CreateAsset(data, configPath);
                Log.i("Create Project Config In Folder:" + configPath);
            }
            
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Reset"))
            {
                ProjectPathConfig.Reset();
            }
        }
    }
}
