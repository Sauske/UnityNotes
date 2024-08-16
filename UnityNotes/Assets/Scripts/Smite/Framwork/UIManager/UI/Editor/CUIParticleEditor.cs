//==================================================================================
/// CUIParticleScript Inspector
/// @neoyang
/// @2015.06.02
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIParticleScript))]
    public class CUIParticleEditor : Editor
    {
        private CUIParticleScript m_uiParticleScript;

        protected virtual void OnEnable()
        {
            m_uiParticleScript = target as CUIParticleScript;
        }

        void OnDisable()
        {
            m_uiParticleScript = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //只在编辑时进行处理
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            //更改Layer
            CUIUtility.SetGameObjectLayer(m_uiParticleScript.gameObject, CUIUtility.c_uiLayer);
        }
    };
};