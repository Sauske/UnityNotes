//==================================================================================
/// UI ToggleList 元素控件
/// @neoyang
/// @2015.03.18
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework
{
    public class CUIToggleListElementScript : CUIListElementScript
    {
        //UGUI Toggle
        private Toggle m_toggle;

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

            m_toggle = GetComponentInChildren<Toggle>(this.gameObject);

            if (m_toggle != null)
            {
                m_toggle.interactable = false;
            }
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_toggle = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// 改变显示(选中/非选中)
        /// @override
        //--------------------------------------------------
        public override void ChangeDisplay(bool selected)
        {
            base.ChangeDisplay(selected);

            //改变勾选状态            
            if (m_toggle != null)
            {
                m_toggle.isOn = selected;
            }
        }        
    };
};