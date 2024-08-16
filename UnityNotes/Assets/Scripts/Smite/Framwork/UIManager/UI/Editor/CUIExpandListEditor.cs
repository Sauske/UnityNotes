//==================================================================================
/// CUIExpandListScript Inspector
/// @neoyang
/// @2015.03.11
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIExpandListScript))]
    public class CUIExpandListEditor : CUIListEditor
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