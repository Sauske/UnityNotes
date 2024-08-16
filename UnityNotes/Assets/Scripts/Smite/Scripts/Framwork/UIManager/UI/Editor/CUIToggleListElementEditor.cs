//==================================================================================
/// CUIToggleListElementScript Inspector
/// @neoyang
/// @2015.03.18
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIToggleListElementScript))]
    public class CUIToggleListElementEditor : CUIListElementEditor
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
        }
    };
};