//==================================================================================
/// CUIEventScript Inspector
/// @neoyang
/// @2015.03.11
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIMiniEventScript))]
    public class CUIMiniEventEditor : Editor
    {
        private string[] m_eventOptions;
        private CUIMiniEventScript m_eventScript;

        protected virtual void OnEnable()
        {
            m_eventScript = target as CUIMiniEventScript;
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            CUIEditorUtility.EditorEventID("On Down", ref m_eventScript.m_onDownEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Up", ref m_eventScript.m_onUpEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Click", ref m_eventScript.m_onClickEventID, m_eventOptions);

            m_eventScript.m_closeFormWhenClicked = EditorGUILayout.Toggle("Close Form When Clicked", m_eventScript.m_closeFormWhenClicked);

            base.OnInspectorGUI();
        }
    };
};
