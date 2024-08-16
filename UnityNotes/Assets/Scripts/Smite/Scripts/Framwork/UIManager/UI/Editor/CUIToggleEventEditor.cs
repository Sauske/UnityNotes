//==================================================================================
/// CUISliderEventScript Inspector
/// @xellosscao
/// @2015.05.21
//==================================================================================

using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIToggleEventScript))]
    public class CUIToggleEventEditor : Editor
    {
        private string[] m_eventOptions;
        private CUIToggleEventScript m_eventScript;

        void OnEnable()
        {
            m_eventScript = target as CUIToggleEventScript;
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            CUIEditorUtility.EditorEventID("On Value Changed", ref m_eventScript.m_onValueChangedEventID, m_eventOptions);

            base.OnInspectorGUI();
        }
    };
};
