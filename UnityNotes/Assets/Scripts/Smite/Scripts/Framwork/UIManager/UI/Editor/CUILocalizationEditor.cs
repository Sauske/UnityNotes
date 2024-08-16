//==================================================================================
/// CUILocalizationScript Inspector
/// @neoyang
/// @2015.03.20
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using ExcelLibrary;
using ExcelLibrary.SpreadSheet;

namespace Framework
{
    //语言信息
    public struct stLanguageInfo
    {
        public string m_name;
        public int m_columnIndex;
    };

    [CustomEditor(typeof(CUILocalizationScript))]
    public class CUILocalizationEditor : Editor
    {
        //CUILocalizationScript组件
        private CUILocalizationScript m_localizationScript;
        private UnityEngine.UI.Text m_text;

        //文本信息(从xls表中读取)
        private static Dictionary<string, string[]> s_textMap;
        private static List<string> s_textKeys;
        private static stLanguageInfo[] s_languageInfos;
        private static int s_keyColumnIndex;

        void OnEnable()
        {
            m_localizationScript = target as CUILocalizationScript;
            m_text = m_localizationScript.gameObject.GetComponent<UnityEngine.UI.Text>();

            LoadTextXls();            
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(6f);
            CUIEditorUtility.SetLabelWidth(80f);

            GUILayout.BeginHorizontal();
            
            //Key not found in the localization file -- draw it as a text field
            SerializedProperty sp = CUIEditorUtility.DrawProperty("Key", serializedObject, "m_key");

            string currentKey = sp.stringValue;
            bool isCurrentKeyExist = (s_textKeys != null) && s_textKeys.Contains(currentKey);
           
            GUI.color = isCurrentKeyExist ? Color.green : Color.red;
            GUILayout.BeginVertical(GUILayout.Width(22f));
            GUILayout.Space(2f);

#if UNITY_3_5
		    GUILayout.Label(isCurrentKeyExist? "ok" : "!!", GUILayout.Height(20f));
#else
            GUILayout.Label(isCurrentKeyExist ? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
#endif

            GUILayout.EndVertical();
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (isCurrentKeyExist)
            {
                if (CUIEditorUtility.DrawHeader("Preview"))
                {
                    CUIEditorUtility.BeginContents();

                    //string[] keys;
                    string[] values;

                    if (s_languageInfos != null && s_languageInfos.Length > 0 && s_textMap != null && s_textMap.TryGetValue(currentKey, out values))
                    {
                        for (int i = 0; i < s_languageInfos.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(s_languageInfos[i].m_name, GUILayout.Width(70f));

                            if (GUILayout.Button(values[i], "AS TextArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 110f)))
                            {
                                if (m_text != null)
                                {
                                    m_text.text = values[i];
                                    EditorUtility.SetDirty(m_text.gameObject);
                                }

                                GUIUtility.hotControl = 0;
                                GUIUtility.keyboardControl = 0;
                            }

                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.Label("No preview available");
                    }

                    CUIEditorUtility.EndContents();
                }
            }
            else if (s_textKeys != null && !string.IsNullOrEmpty(currentKey))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80f);
                GUILayout.BeginVertical();
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.35f);

                int matches = 0;

                for (int i = 0; i < s_textKeys.Count; i++)
                {
                    if (s_textKeys[i].StartsWith(currentKey, System.StringComparison.OrdinalIgnoreCase) || s_textKeys[i].Contains(currentKey))
                    {
#if UNITY_3_5
					    if (GUILayout.Button(mKeys[i] + " \u25B2"))
#else
                        if (GUILayout.Button(s_textKeys[i] + " \u25B2", "CN CountBadge"))
#endif
                        {
                            sp.stringValue = s_textKeys[i];
                            GUIUtility.hotControl = 0;
                            GUIUtility.keyboardControl = 0;
                        }

                        if (++matches == 8)
                        {
                            GUILayout.Label("...and more");
                            break;
                        }
                    }
                }

                GUI.backgroundColor = Color.white;
                GUILayout.EndVertical();
                GUILayout.Space(22f);
                GUILayout.EndHorizontal();
            }

            //重新加载xls
            if (GUILayout.Button("Reload Text Xls"))
            {
                ReloadTextXls();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void LoadTextXls()
        {
            if (s_textMap != null && s_textKeys != null && s_languageInfos != null)
            {
                return;
            }

            s_textMap = new Dictionary<string, string[]>();
            s_textKeys = new List<string>();

            Workbook workbook = Workbook.Load(new System.IO.FileStream(Application.dataPath + "../../../Tools/Tdr/ResConvert/data_xls/99.文本配置表.xls", System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite));
            //Workbook workbook = Workbook.Load(Application.dataPath + "../../../Tools/Tdr/ResConvert/data_xls/99.文本配置表.xls");

            for (int i = 0; i < workbook.Worksheets.Count; i++)
            {
                Worksheet worksheet = workbook.Worksheets[i];

                s_keyColumnIndex = -1;

                for (int j = 0; j < worksheet.Cells.Rows.Count; j++)
                {
                    Row row = worksheet.Cells.Rows[j];

                    int cellCount = row.LastColIndex - row.FirstColIndex + 1;

                    if (cellCount > 0)
                    {
                        //根据表头(第一页第一行)数据设定language
                        if (i == 0 && j == 0)
                        {
                            for (int k = 0; k < cellCount; k++)
                            {
                                if (row.GetCell(row.FirstColIndex + k).StringValue.Equals("Key"))
                                {
                                    s_keyColumnIndex = row.FirstColIndex + k;
                                    break;
                                }
                            }

                            if (s_keyColumnIndex < 0)
                            {
                                Debug.Log("Can not Find Key!!!");
                                return;
                            }

                            s_languageInfos = new stLanguageInfo[row.LastColIndex - s_keyColumnIndex];

                            for (int m = 0; m < s_languageInfos.Length; m++)
                            {
                                s_languageInfos[m].m_name = row.GetCell(s_keyColumnIndex + 1 + m).StringValue;
                                s_languageInfos[m].m_columnIndex = s_keyColumnIndex + 1 + m;
                            }

                            continue;
                        }

                        //解析每一行的数据
                        Cell keyCell = row.GetCell(s_keyColumnIndex);

                        if (keyCell != null)
                        {
                            string key = keyCell.StringValue;

                            if (!string.IsNullOrEmpty(key))
                            {
                                string[] values = new string[s_languageInfos.Length];
                            
                                for (int k = 0; k < s_languageInfos.Length; k++)
                                {
                                    Cell valueCell = row.GetCell(s_languageInfos[k].m_columnIndex);

                                    if (valueCell != null)
                                    {
                                        values[k] = valueCell.StringValue;
                                    }
                                    else
                                    {
                                        values[k] = string.Empty;
                                    }                                
                                }

                                s_textKeys.Add(key);
                                s_textMap.Add(key, values);
                            }
                        }                        
                    }
                }
            }
        }

        private void ReloadTextXls()
        {
            s_textMap.Clear();
            s_textMap = null;

            s_textKeys.Clear();
            s_textKeys = null;

            s_languageInfos = null;

            LoadTextXls();
        }
    };
};