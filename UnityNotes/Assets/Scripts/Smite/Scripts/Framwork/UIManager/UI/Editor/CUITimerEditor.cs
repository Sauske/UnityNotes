//==================================================================================
/// CUITimerScript Inspector
/// @neoyang
/// @2015.03.14
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUITimerScript))]
    public class CUITimerEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUITimerScript m_timerScript;

        protected virtual void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_timerScript = target as CUITimerScript;
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Time Start", ref m_timerScript.m_eventIDs[(int)enTimerEventType.TimeStart], m_eventOptions);
            CUIEditorUtility.EditorEventID("On Time Up", ref m_timerScript.m_eventIDs[(int)enTimerEventType.TimeUp], m_eventOptions);
            CUIEditorUtility.EditorEventID("On Time Changed", ref m_timerScript.m_eventIDs[(int)enTimerEventType.TimeChanged], m_eventOptions);
        }
    };
};