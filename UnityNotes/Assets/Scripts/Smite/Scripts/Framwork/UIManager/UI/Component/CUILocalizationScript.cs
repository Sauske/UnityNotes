//==================================================================================
/// UI本地化组件
/// @neoyang
/// @2015.03.20
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Framework
{
    public class CUILocalizationScript : CUIComponent
    {
        [HideInInspector]
        public string m_key;

        //UGUI Text组件
        private UnityEngine.UI.Text m_text;

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            m_text = gameObject.GetComponent<UnityEngine.UI.Text>();

            SetDisplay();
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_text = null;

            base.OnDestroy();
        }

        //--------------------------------------
        /// 设置Key
        //--------------------------------------
        public void SetKey(string key)
        {
            m_key = key;

            SetDisplay();
        }

        //--------------------------------------
        /// 设置文本显示
        //--------------------------------------
        public void SetDisplay()
        {
            if (m_text == null || string.IsNullOrEmpty(m_key) || !CTextManager.GetInstance().IsTextLoaded())
            {
                return;
            }

            m_text.text = CTextManager.GetInstance().GetText(m_key);
        }
    };
};