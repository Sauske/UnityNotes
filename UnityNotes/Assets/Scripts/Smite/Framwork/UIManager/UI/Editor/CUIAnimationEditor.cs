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
    [CustomEditor(typeof(CUIAnimationScript))]
    public class CUIAnimationEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIAnimationScript m_animationScript;

        protected virtual void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_animationScript = target as CUIAnimationScript;

            Animation animation = m_animationScript.gameObject.GetComponent<Animation>();
            if (animation == null)
            {
                m_animationScript.gameObject.AddComponent<Animation>();
            }
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Animation Start", ref m_animationScript.m_eventIDs[(int)enAnimationEventType.AnimationStart], m_eventOptions);
            CUIEditorUtility.EditorEventID("On Animation End", ref m_animationScript.m_eventIDs[(int)enAnimationEventType.AnimationEnd], m_eventOptions);
        }
    };
};