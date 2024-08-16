//==================================================================================
/// UI Expand List Ԫ�ؿؼ�
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
        //����ʱ�ĳߴ�(-1��ʾά�ֵ�ǰgameObject�Ŀ�/��)
        public Vector2 m_retractedSize = new Vector2(-1, -1);

        //չ��ʱ�ĳߴ�
        [HideInInspector]
        public Vector2 m_expandedSize;

        //--------------------------------------
        /// ��ʼ��
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
        /// ��ȡԪ��Ĭ�ϳߴ�
        /// @��������ʱ�ĳߴ�
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
        /// ��ȡԪ��չ��ʱ�ĳߴ�
        //--------------------------------------------------
        protected Vector2 GetExpandedSize()
        {
            return (new Vector2((this.gameObject.transform as RectTransform).rect.width, (this.gameObject.transform as RectTransform).rect.height));
        }

        //--------------------------------------------------
        /// �ı���ʾ(ѡ��/��ѡ��)
        //--------------------------------------------------
        public override void ChangeDisplay(bool selected)
        {
            //here need do nothing, ��Ϊelement�����Image��������mask�ģ�����ʾ�޹�
        }
    };
};