//==================================================================================
/// CUIStepListScript Inspector
/// @neoyang
/// @2015.03.11
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIStepListScript))]
    public class CUIStepListEditor : CUIListEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        void OnDisable() 
        {
 
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CUIEditorUtility.EditorEventID("On Start Dragging", ref ((CUIStepListScript)m_listScript).m_onStartDraggingEventID, m_eventOptions);
        }
    };
};
