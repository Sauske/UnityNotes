using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Qarth
{
    public class DataToolEditor
    {
        [MenuItem("SaveData Tools/Clear Prefs")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
        
        [MenuItem("SaveData Tools/Clear Save Data")]
        public static void ClearGameSaveData()
        {
            string path = Application.persistentDataPath + "/cache";
            bool isHaveData = Directory.Exists(path);
            if (isHaveData)
            {
                Directory.Delete(path, true);
            }
        }
    }
}


