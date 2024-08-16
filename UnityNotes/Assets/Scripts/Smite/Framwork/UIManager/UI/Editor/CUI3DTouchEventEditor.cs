//==================================================================================
/// CUI3DTouchScript Inspector
/// @neoyang
/// @2015.03.11
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUI3DTouchEventScript))]
    public class CUI3DTouchEventEditor : CUIMiniEventEditor
    {
        private string[] m_eventOptions;
        private CUI3DTouchEventScript m_eventScript;

        protected override void OnEnable()
        {
            m_eventScript = target as CUI3DTouchEventScript;
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));

            base.OnEnable();
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Touched", ref m_eventScript.m_onTouchedEventID, m_eventOptions);            
        }
    };
};
