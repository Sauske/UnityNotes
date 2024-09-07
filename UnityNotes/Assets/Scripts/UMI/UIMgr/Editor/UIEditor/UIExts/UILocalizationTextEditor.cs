using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UMI;

namespace UnityEditor.UI
{
    /// <summary>
    /// 本地化控件编辑器
    /// </summary>
    [CustomEditor(typeof(UILocalizationText), true)]
    public class UILocalizationTextEditor : Editor
    {
        private UILocalizationText currentSelect;       // 当前选择的本地化控件
        private SerializedProperty textIdProperty;      // 当前的文本id属性

        private int lastTextId;                         // 上一次的文本id
        
        private void OnEnable()
        {
            currentSelect = target as UILocalizationText;
            textIdProperty = serializedObject.FindProperty("TextId");
            lastTextId = 0;

            LoadExcelJsonConfig();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("  ");
            GUILayout.Label("  ");
            GUILayout.Label("非运行时改了文本表，因为已经加载过一次就不会重新加载，需要手动加载刷新。");
            if (GUILayout.Button("手动重新加载表"))
            {
                LoadExcelJsonConfig();
            }

            if (lastTextId == textIdProperty.intValue)
            {
                // id 没有变动，不刷新
                return;
            }

            if (Application.isPlaying)
            {
                // 运行下不进行改动
                return;
            }

            lastTextId = textIdProperty.intValue;
            currentSelect.ChangeText(lastTextId);
            
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// 加载json格式的配置
        /// </summary>
        private void LoadExcelJsonConfig()
        {
            const string ASSET_PATH = "Assets/DataBytes/Config/excel.txt";

            string txt = File.ReadAllText(ASSET_PATH);
            //CsJsonDataLoader.InitJsonData(txt);
        }
    }
}
