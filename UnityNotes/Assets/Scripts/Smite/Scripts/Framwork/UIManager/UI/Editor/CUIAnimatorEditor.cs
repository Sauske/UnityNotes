//==================================================================================
/// CUIAnimationScript Inspector
/// @neoyang
/// @2015.04.09
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIAnimatorScript))]
    public class CUIAnimatorEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIAnimatorScript m_animatorScript;

        protected virtual void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_animatorScript = target as CUIAnimatorScript;

            Animator animation = m_animatorScript.gameObject.GetComponent<Animator>();
            if (animation == null)
            {
                m_animatorScript.gameObject.AddComponent<Animator>();
            }
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Animator Start", ref m_animatorScript.m_eventIDs[(int)enAnimatorEventType.AnimatorStart], m_eventOptions);
            CUIEditorUtility.EditorEventID("On Animator End", ref m_animatorScript.m_eventIDs[(int)enAnimatorEventType.AnimatorEnd], m_eventOptions);
        }
    };
};