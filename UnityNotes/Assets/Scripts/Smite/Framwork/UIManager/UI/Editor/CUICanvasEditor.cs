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
    [CustomEditor(typeof(CUICanvasScript))]
    public class CUICanvasEditor : Editor
    {
        protected CUICanvasScript m_canvasScript;

        protected virtual void OnEnable()
        {
            m_canvasScript = target as CUICanvasScript;

            Canvas cs = m_canvasScript.GetComponent<Canvas>();
            if (cs == null)
            {
                m_canvasScript.gameObject.AddComponent<Canvas>();
            }
        }
    };
};