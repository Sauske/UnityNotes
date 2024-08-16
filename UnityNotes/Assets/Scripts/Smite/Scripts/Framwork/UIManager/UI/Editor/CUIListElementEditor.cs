//==================================================================================
/// CUIListElementScript Inspector
/// @neoyang
/// @2015.03.17
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIListElementScript))]
    public class CUIListElementEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIListElementScript m_listElementScript;

        protected virtual void OnEnable()
        {
            m_listElementScript = target as CUIListElementScript;
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Enable", ref m_listElementScript.m_onEnableEventID, m_eventOptions);
        }
    };
};