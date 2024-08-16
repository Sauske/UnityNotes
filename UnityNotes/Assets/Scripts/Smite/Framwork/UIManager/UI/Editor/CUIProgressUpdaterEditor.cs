//==================================================================================
/// CUIProgressUpdaterScript Inspector
/// @lighthuang
/// @2016.05.20
//==================================================================================

using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIProgressUpdaterScript))]
    public class CUIProgressUpdaterEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIProgressUpdaterScript m_relatedScript;

        protected virtual void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_relatedScript = target as CUIProgressUpdaterScript;
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("Fill End Event", ref m_relatedScript.m_fillEndEventID, m_eventOptions);
            //CUIEditorUtility.EditorEventID("Fill Full Event", ref m_relatedScript.m_fillFullEventID, m_eventOptions);
        }
    };
};