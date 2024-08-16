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
    [CustomEditor(typeof(CUIEventScript))]
    public class CUIEventEditor : Editor
    {
        private string[] m_eventOptions;
        private CUIEventScript m_eventScript;

        void OnEnable()
        {
            m_eventScript = target as CUIEventScript;
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
            CUIEditorUtility.EditorEventID("On Hold Start", ref m_eventScript.m_onHoldStartEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Hold", ref m_eventScript.m_onHoldEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Hold End", ref m_eventScript.m_onHoldEndEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Drag Start", ref m_eventScript.m_onDragStartEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Drag", ref m_eventScript.m_onDragEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Drag End", ref m_eventScript.m_onDragEndEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Drop", ref m_eventScript.m_onDropEventID, m_eventOptions);

            m_eventScript.m_closeFormWhenClicked = EditorGUILayout.Toggle("Close Form When Clicked", m_eventScript.m_closeFormWhenClicked);
            m_eventScript.m_isDispatchDragEventForBelongList = EditorGUILayout.Toggle("DisPatch Drag Event For Belong List(If In List)", m_eventScript.m_isDispatchDragEventForBelongList);

            base.OnInspectorGUI();
        }
    };
};
