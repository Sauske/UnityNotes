//==================================================================================
/// UI编辑功能库
/// @neoyang
/// @2015.03.12
//==================================================================================

using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Object = UnityEngine.Object;

namespace Framework
{
    public class CUIEditorUtility
    {
        //--------------------------------------------------
        /// 返回枚举选项
        //--------------------------------------------------
        public static string[] GetEnumOptions(System.Type enumType)
        {
            string[] eventIDOptions = System.Enum.GetNames(enumType);
            SortEventIDOptions(eventIDOptions);
            
            for (int i = 0; i < eventIDOptions.Length; i++)
            {
                string str = eventIDOptions[i];
                if (str.IndexOf("_") != -1)
                {
                    string prefix = str.Split('_')[0];
                    eventIDOptions[i] = prefix + @"/" + eventIDOptions[i];
                }
            }

            return eventIDOptions;
        }

        /// <summary>
        /// 排序eventID选项（字典序）
        /// </summary>
        /// <param name="eventIDOptions"></param>
        private static void SortEventIDOptions(string[] eventIDOptions)
        {
            Array.Sort(eventIDOptions, delegate(string s1, string s2)
            {
                if ("None".Equals(s1))
                {
                    return -1;
                }
                if ("None".Equals(s2))
                {
                    return 1;
                }
                return string.Compare(s1, s2, true);
            });
        }

        //--------------------------------------------------
        /// 编辑EventID
        //--------------------------------------------------
        public static void EditorEventID(string label, ref enUIEventID eventID, string[] eventIDOptions)
        {
            int index = CUIEditorUtility.GetOptionIndex(eventID, eventIDOptions);
            int newIndex = EditorGUILayout.Popup(label, index, eventIDOptions);

            if (newIndex != index)
            {
                index = newIndex;
                eventID = CUIEditorUtility.GetEventID(index, eventIDOptions);

                //Debug.Log(label + "Selected Event ID = " + eventID + ", value = " + (int)eventID);
            }
        }

        //--------------------------------------------------
        /// 通过eventID返回选项索引
        //--------------------------------------------------
        public static int GetOptionIndex(enUIEventID eventID, string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                string str = options[i];
                if (str.IndexOf(@"/") != -1)
                {
                    str = str.Split('/')[1];
                }

                if (str.Equals(eventID.ToString()))
                {
                    return i;
                }
            }

            return -1;
        }

        //--------------------------------------------------
        /// 通过选项索引返回eventID
        //--------------------------------------------------
        public static enUIEventID GetEventID(int optionIndex, string[] options)
        {
            if (optionIndex < 0 || optionIndex >= options.Length)
            {
                return enUIEventID.None;
            }

            string str = options[optionIndex];
            if (str.IndexOf(@"/") != -1)
            {
                str = str.Split('/')[1];
            }

            return (enUIEventID)(System.Enum.Parse(typeof(enUIEventID), str));
        }

        //----------------------------------------------
        /// 返回资源在Resources目录下的路径(方便动态加载)
        /// @assetObject
        /// @extension : 扩展名 (例: ".prefab")
        //----------------------------------------------
        public static string GetAssetPathInResources(Object assetObject, string extension)
        {
            if (assetObject == null)
            {
                return null;
            }

            string path = AssetDatabase.GetAssetPath(assetObject);

            int resourcesIndex = path.LastIndexOf("Resources");
            int extensionIndex = path.LastIndexOf(extension);

            if (resourcesIndex < 0 || extensionIndex < 0)
            {
                path = null;
                Debug.LogError("Please put " + assetObject.name + " in [Resources] directory!");
            }
            else
            {
                path = path.Substring(resourcesIndex + 10, extensionIndex - resourcesIndex - 10);
            }

            return path;
        }

        //----------------------------------------------
        /// 从Resources下获取资源
        /// @path
        /// @assetType
        //----------------------------------------------
        public static Object GetAssetInResources(string path, System.Type assetType)
        {
            if (path == null || path == "")
            {
                return null;
            }

            return Resources.Load(path, assetType);
        }

        //----------------------------------------------
        /// 设置Lable宽度
        /// @width
        //----------------------------------------------
        static public void SetLabelWidth(float width)
        {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		    EditorGUIUtility.LookLikeControls(width);
#else
            EditorGUIUtility.labelWidth = width;
#endif
        }

        //----------------------------------------------
        /// Helper function that draws a serialized property.
        //----------------------------------------------
        static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
        {
            return DrawProperty(label, serializedObject, property, false, options);
        }

        //----------------------------------------------
        /// Helper function that draws a serialized property.
        //----------------------------------------------
        static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
        {
            SerializedProperty sp = serializedObject.FindProperty(property);

            if (sp != null)
            {
                if (padding)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                if (label != null)
                {
                    EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
                }
                else
                {
                    EditorGUILayout.PropertyField(sp, options);
                }

                if (padding)
                {
                    GUILayout.Space(18f);
                    EditorGUILayout.EndHorizontal();
                }
            }

            return sp;
        }

        //----------------------------------------------
        /// Draw a distinctly different looking header label
        //----------------------------------------------
        static public bool DrawHeader(string text) 
        { 
            return DrawHeader(text, text, false); 
        }

        //----------------------------------------------
        /// Draw a distinctly different looking header label
        //----------------------------------------------
        static public bool DrawHeader(string text, string key) 
        { 
            return DrawHeader(text, key, false); 
        }

        //----------------------------------------------
        /// Draw a distinctly different looking header label
        //----------------------------------------------
        static public bool DrawHeader(string text, bool forceOn) 
        { 
            return DrawHeader(text, text, forceOn); 
        }

        //----------------------------------------------
        /// Draw a distinctly different looking header label
        //----------------------------------------------
        static public bool DrawHeader(string text, string key, bool forceOn)
        {
            bool state = EditorPrefs.GetBool(key, true);

            GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);

            GUI.changed = false;
#if UNITY_3_5
		    if (state) text = "\u25B2 " + text;
		    else text = "\u25BC " + text;
		    if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
#else
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25B2 " + text;
            else text = "\u25BC " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
#endif
            if (GUI.changed) EditorPrefs.SetBool(key, state);

            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        //----------------------------------------------
        /// Begin drawing the content area.
        //----------------------------------------------
        static public void BeginContents()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        //----------------------------------------------
        /// End drawing the content area.
        //----------------------------------------------
        static public void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }
    };
};
