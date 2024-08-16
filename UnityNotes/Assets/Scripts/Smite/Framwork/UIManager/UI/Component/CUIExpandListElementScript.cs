//==================================================================================
/// UI Expand List 元素控件
/// @neoyang
/// @2015.06.23
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework
{
    public class CUIExpandListElementScript : CUIListElementScript
    {
        //收起时的尺寸(-1表示维持当前gameObject的宽/高)
        public Vector2 m_retractedSize = new Vector2(-1, -1);

        //展开时的尺寸
        [HideInInspector]
        public Vector2 m_expandedSize;

        //--------------------------------------
        /// 初始化
        /// @formScript
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            m_expandedSize = GetExpandedSize();

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// 获取元素默认尺寸
        /// @返回收起时的尺寸
        //--------------------------------------------------
        protected override Vector2 GetDefaultSize()
        {
            if (m_retractedSize.x <= 0)
            {
                m_retractedSize.x = ((RectTransform)this.gameObject.transform).rect.width;
            }

            if (m_retractedSize.y <= 0)
            {
                m_retractedSize.y = ((RectTransform)this.gameObject.transform).rect.height;
            }

            return m_retractedSize;
        }

        //--------------------------------------------------
        /// 获取元素展开时的尺寸
        //--------------------------------------------------
        protected Vector2 GetExpandedSize()
        {
            return (new Vector2((this.gameObject.transform as RectTransform).rect.width, (this.gameObject.transform as RectTransform).rect.height));
        }

        //--------------------------------------------------
        /// 改变显示(选中/非选中)
        //--------------------------------------------------
        public override void ChangeDisplay(bool selected)
        {
            //here need do nothing, 因为element上面的Image是用来作mask的，与显示无关
        }
    };
};