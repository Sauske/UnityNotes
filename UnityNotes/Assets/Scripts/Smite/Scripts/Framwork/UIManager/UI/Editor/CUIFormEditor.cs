//==================================================================================
/// CUIFormScript Inspector
/// @neoyang
/// @2015.03.24
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.IO;

namespace Framework
{    
    [CustomEditor(typeof(CUIFormScript))]    
    public class CUIFormEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIFormScript m_formScript;

        //private static ListView<CUIFormScript> s_formScripts = new ListView<CUIFormScript>();
        //private static bool s_initUpdate = false;
        //private static Vector2 s_screen = Vector2.zero;

        //Editor模式下Screen.width 和 Screen.height会莫名其妙变化为Game窗体的实际大小一桢，然后又变回来，加个计数器来解决这个bug
        //private static uint s_checkCount = 0;

        private string m_strWidgetsDeclare = "";

        void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_formScript = target as CUIFormScript;
            m_formScript.InitializeCanvas();

            //m_formScript.m_onDestroyed -= CloseForm;
            //m_formScript.m_onDestroyed += CloseForm;

            //AddToFormScriptList(m_formScript);

//             if (!s_initUpdate)
//             {
//                 s_initUpdate = true;
// 
//                 EditorApplication.update -= UpdateScreen;
//                 EditorApplication.update += UpdateScreen;
//             }
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Form Opened", ref m_formScript.m_eventIDs[(int)enFormEventType.Open], m_eventOptions);
            CUIEditorUtility.EditorEventID("On Form Closed", ref m_formScript.m_eventIDs[(int)enFormEventType.Close], m_eventOptions);
            CUIEditorUtility.EditorEventID("Revert To Visable", ref m_formScript.m_revertToVisibleEvent, m_eventOptions);
            CUIEditorUtility.EditorEventID("Revert To Hide", ref m_formScript.m_revertToHideEvent, m_eventOptions);

            //auto generate enum widgets
            string widgetsClsName = "Widgets_" + m_formScript.gameObject.name;
            string nowWidgetsDefStr = "namespace Assets.Scripts.GameSystem\n{\n\tpublic enum en" + widgetsClsName + "\n\t{\n";
            for (int i = 0; i < m_formScript.m_formWidgets.Length; i++)
            {
                if (m_formScript.m_formWidgets[i] != null && !m_formScript.m_formWidgets[i].transform.IsChildOf(m_formScript.gameObject.transform))
                {                    
                    Debug.Log("Form widgets can only contain widgets in it self!!!");
                    m_formScript.m_formWidgets[i] = null;
                }
                else if (m_formScript.m_formWidgets[i] != null)
                {
                    nowWidgetsDefStr += "\t\tEN_" + m_formScript.m_formWidgets[i].name + " = " + i + ",\n";                
                }                
            }
            nowWidgetsDefStr += "\t}\n};";

            if (GUILayout.Button("Auto generate widgets enum file"))
            {
                StreamWriter sw = null;

                string fileName = Application.dataPath + "/Scripts/UI/WidgetsDefine/" + widgetsClsName + ".cs";
                if (CFileManager.IsFileExist(fileName))
                {
                    StreamReader sr = new StreamReader(fileName);
                    m_strWidgetsDeclare = sr.ReadToEnd();
                    sr.Close();
                }
                else
                {
                    sw = File.CreateText(fileName);
                }

                if (m_strWidgetsDeclare == null || !m_strWidgetsDeclare.Equals(nowWidgetsDefStr))
                {
                    if (sw == null)
                        sw = new StreamWriter(fileName);
                    m_strWidgetsDeclare = nowWidgetsDefStr;
                    sw.Write(nowWidgetsDefStr);
                    sw.Flush();
                }
                if (sw != null)
                    sw.Close();
            }           
        }

/*
        //--------------------------------------
        /// Close Form
        //--------------------------------------
        public static void CloseForm(CUIFormScript formScript)
        {
            RemoveFromFormScriptList(formScript);
        }

        //--------------------------------------
        /// 屏幕变化
        //--------------------------------------
        private static void UpdateScreen()
        {
            if (s_screen.x != Screen.width || s_screen.y != Screen.height)
            {
                if (s_checkCount++ < 3)
                {
                    return;
                }
                s_checkCount = 0;

                s_screen = new Vector2(Screen.width, Screen.height);

                for (int i = 0; i < s_formScripts.Count; )
                {
                    if (s_formScripts[i] == null)
                    {
                        s_formScripts.RemoveAt(i);
                        continue;
                    }

                    //屏幕适配并强制刷新
                    s_formScripts[i].MatchScreen();
                    UnityEditor.EditorUtility.SetDirty(s_formScripts[i].gameObject.GetComponent<CanvasScaler>());

                    i++;
                }
            }
            else
            {
                s_checkCount = 0;
            }
        }

        //--------------------------------------
        /// 添加到FormScript列表
        /// @formScript
        //--------------------------------------
        public static void AddToFormScriptList(CUIFormScript formScript)
        {
            if (!s_formScripts.Contains(formScript))
            {
                s_formScripts.Add(formScript);
            }
        }

        //--------------------------------------
        /// 从FormScript列表移除
        /// @formScript
        //--------------------------------------
        public static void RemoveFromFormScriptList(CUIFormScript formScript)
        {
            s_formScripts.Remove(formScript);
        }
*/ 
    };
 
    //-------------------------------------------------
    /// 保存Form时的回调
    /// @必须把m_canvasScaler.matchWidthOrHeight存成0
    //-------------------------------------------------
    public class CFormSaveProcessor : UnityEditor.AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Contains("Assets/Resources/UGUI/Form") && CFileManager.GetExtension(paths[i]).ToLower().Equals(".prefab"))
                {
                    GameObject formPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(paths[i], typeof(GameObject));

                    CanvasScaler canvasScaler = formPrefab.GetComponent<CanvasScaler>();
                    if (canvasScaler != null)
                    {
                        canvasScaler.referenceResolution = new Vector2(canvasScaler.referenceResolution.x + 0.001f, canvasScaler.referenceResolution.y + 0.001f);
                        //canvasScaler.matchWidthOrHeight = 0f;

                        UnityEditor.EditorUtility.SetDirty(canvasScaler);
                    }

                    Canvas canvas = formPrefab.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        canvas.worldCamera = null;

                        UnityEditor.EditorUtility.SetDirty(canvas);
                    }

                    //Clear 带有CUILocalizationScript的Text
                    CUILocalizationScript[] localizationScripts = new CUILocalizationScript[100];
                    int count = 0;

                    CUIUtility.GetComponentsInChildren<CUILocalizationScript>(formPrefab, localizationScripts, ref count);

                    for (int j = 0; j < count; j++)
                    {
                        Text textScript = localizationScripts[j].GetComponent<Text>();
                        if (textScript != null)
                        {
                            textScript.text = null;
                            UnityEditor.EditorUtility.SetDirty(textScript);
                        }
                    }
                }
            }
                
            return paths;
        }
    };
};
