//==================================================================================
/// CUISliderEventScript Inspector
/// @lighthuang
/// @2015.05.21
//==================================================================================

using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUISliderEventScript))]
    public class CUISliderEventEditor : Editor
    {
        private string[] m_eventOptions;
        private CUISliderEventScript m_eventScript;

        void OnEnable()
        {
            m_eventScript = target as CUISliderEventScript;
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
