//==================================================================================
/// CUIExpandListElementScript Inspector
/// @neoyang
/// @2015.06.23
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIExpandListElementScript))]
    public class CUIExpandListElementEditor : CUIListElementEditor
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