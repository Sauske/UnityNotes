//==================================================================================
/// CUIJoystickScript Inspector
/// @neoyang
/// @2015.03.27
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIJoystickScript))]
    public class CUIJoystickEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIJoystickScript m_joystickScript;

        protected virtual void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_joystickScript = target as CUIJoystickScript;
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Axis Changed", ref m_joystickScript.m_onAxisChangedEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Axis Down", ref m_joystickScript.m_onAxisDownEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Axis Released", ref m_joystickScript.m_onAxisReleasedEventID, m_eventOptions);
        }
    };
};