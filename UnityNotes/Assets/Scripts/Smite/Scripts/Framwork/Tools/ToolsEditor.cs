//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework
{
    
    public class ToolsEditor:Editor
    {
        
        [MenuItem("Tools/DeletaMeta")]
        public static void DeleteMeta()
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            DeleteMeta(path);
        }       
        public static void DeleteMeta(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length;i++)
            {
                DeleteMeta(dirs[i]);
            }

            string[] files = Directory.GetFiles(path);
            for (int i = files.Length -1; i >= 0;i--)
            {
                if (files[i].EndsWith(".meta"))
                {
                    File.Delete(files[i]);
                }
            }
        }
    }
}