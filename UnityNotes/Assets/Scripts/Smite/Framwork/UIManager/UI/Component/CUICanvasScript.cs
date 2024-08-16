//====================================
/// UICanvas控件 主要用于当form被隐藏的时候同时跟随隐藏
/// @royjin
/// @2015.12.01
//====================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Framework
{
    public class CUICanvasScript : CUIComponent  
	{	
        private Canvas m_Canvas;

        public bool m_isNeedMaskParticle = false;   //是否需要遮挡同UI下的粒子特效

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
		{
            if (m_isInitialized)
            {
                return;
            }

            m_Canvas = this.GetComponent<Canvas>();

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// Hide
        //--------------------------------------------------
        public override void Hide()
        {
            base.Hide();

            CUIUtility.SetGameObjectLayer(this.gameObject, CUIUtility.c_hideLayer);
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void Appear()
        {
            base.Appear();

            CUIUtility.SetGameObjectLayer(this.gameObject, CUIUtility.c_uiLayer);
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void SetSortingOrder(int sortingOrder)
        {
             //通过Form之间order 10的差量来支持UI里某个节点可以挡住UI下的其它元素包括粒子
            if (m_Canvas != null && m_isNeedMaskParticle)
            {
                m_Canvas.overrideSorting = true;
                m_Canvas.sortingOrder = sortingOrder + 1;
            }
        }
	} 
}