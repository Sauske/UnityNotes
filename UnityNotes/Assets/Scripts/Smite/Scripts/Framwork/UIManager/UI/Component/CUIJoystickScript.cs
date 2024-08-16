//==================================================================================
/// UIҡ�����
/// @neoyang
/// @2015.03.26
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Framework
{
    public class CUIJoystickScript : CUIComponent, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        //�Ƿ���Ը����϶�λ���ƶ�
        public bool m_isAxisMoveable = true;

        //Axis�ƶ���Χ
        public Vector2 m_axisScreenPositionOffsetMin;
        public Vector2 m_axisScreenPositionOffsetMax;

        //Cursor������ʾ�����ƫ�ư뾶
        public float m_cursorDisplayMaxRadius = 128f; 

        //��Ӧҡ�������������С�뾶
        public float m_cursorRespondMinRadius = 15f;

        //Axis�����ı�ʱ�ɷ��¼�ID������
        [HideInInspector]
        public enUIEventID m_onAxisChangedEventID;
        public stUIEventParams m_onAxisChangedEventParams;

        //Axis����ʱ�ɷ��¼�ID������
        [HideInInspector]
        public enUIEventID m_onAxisDownEventID;
        public stUIEventParams m_onAxisDownEventParams;

        //Axis�ɿ�ʱ�ɷ��¼�ID������
        [HideInInspector]
        public enUIEventID m_onAxisReleasedEventID;
        public stUIEventParams m_onAxisReleasedEventParams;

        //������ʾ��Axis��Cursor��RectTransform
        private RectTransform m_axisRectTransform;
        private RectTransform m_cursorRectTransform;
        private Image m_axisImage;
        private Image m_cursorImage;
        public float m_axisFadeoutAlpha = (float) 0.6;//(float)140 / (float)255;

        //Axis�����Ļ����
        private Vector2 m_axisOriginalScreenPosition;
        private Vector2 m_axisTargetScreenPosition;
        private Vector2 m_axisCurrentScreenPosition;

        //Axis Value
        private Vector2 m_axis;

        //Border��ʾ
        private RectTransform m_borderRectTransform;
        private CanvasGroup m_borderCanvasGroup;

        //--------------------------------------
        /// ��ʼ��
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            m_axisRectTransform = this.gameObject.transform.Find("Axis") as RectTransform;
            if (m_axisRectTransform != null)
            {
                //m_axisRectTransform.pivot = new Vector2(0.5f, 0.5f);
                //m_axisRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                //m_axisRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                m_axisRectTransform.anchoredPosition = Vector2.zero;
                m_axisOriginalScreenPosition = CUIUtility.WorldToScreenPoint(m_belongedFormScript.GetCamera(), m_axisRectTransform.position);
                m_axisImage = m_axisRectTransform.gameObject.GetComponent<Image>();

                m_cursorRectTransform = m_axisRectTransform.Find("Cursor") as RectTransform;
                if (m_cursorRectTransform != null)
                {
                    //m_cursorRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    //m_cursorRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    //m_cursorRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    m_cursorRectTransform.anchoredPosition = Vector2.zero;
                    m_cursorImage = m_cursorRectTransform.gameObject.GetComponent<Image>();
                }

                //Fadeout
                AxisFadeout();
            }

            m_borderRectTransform = this.gameObject.transform.Find("Axis/Border") as RectTransform;
            if (m_borderRectTransform != null)
            {
                m_borderCanvasGroup = m_borderRectTransform.gameObject.GetComponent<CanvasGroup>();
                if (m_borderCanvasGroup == null)
                {
                    m_borderCanvasGroup = m_borderRectTransform.gameObject.AddComponent<CanvasGroup>();
                }

                //����
                HideBorder();
            }
        }

        //--------------------------------------------------
        /// ����
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_axisRectTransform = null;
            m_cursorRectTransform = null;
            m_axisImage = null;
            m_cursorImage = null;
            m_borderRectTransform = null;
            m_borderCanvasGroup = null;

            base.OnDestroy();
        }

        void Update()
        {
            if (m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            {
                return;
            }

            if (m_isAxisMoveable)
            {
                UpdateAxisPosition();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //�ɷ��¼�
            DispatchOnAxisDownEvent();

            MoveAxis(eventData.position, true);
            AxisFadeIn();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            MoveAxis(eventData.position, false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveAxis(eventData.position, false);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //DispatchOnAxisReleasedEvent();
            //ResetAxis();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ResetAxis();

            //�ɷ��¼�
            DispatchOnAxisReleasedEvent();
        }

        public Vector2 GetAxis()
        {
            return m_axis;
        }

        //--------------------------------------------------
        /// ����Axis
        //--------------------------------------------------
        public void ResetAxis()
        {
            //m_downScreenPosition = Vector2.zero;

            m_axisRectTransform.anchoredPosition = Vector2.zero;
            m_cursorRectTransform.anchoredPosition = Vector2.zero;

            m_axisOriginalScreenPosition = CUIUtility.WorldToScreenPoint(m_belongedFormScript.GetCamera(), m_axisRectTransform.position);
            m_axisCurrentScreenPosition = Vector2.zero;
            m_axisTargetScreenPosition = Vector2.zero;

            UpdateAxis(Vector2.zero);

            AxisFadeout();
        }

        //--------------------------------------------------
        /// ����axis
        //--------------------------------------------------
        private void UpdateAxis(Vector2 axis)
        {
            if (m_axis != axis)
            {
                m_axis = axis;

                //�ɷ��¼�
                DispatchOnAxisChangedEvent();
            }

            if (m_axis == Vector2.zero)
            {
                HideBorder();
            }
            else
            {
                ShowBorder(m_axis);
            }
        }

        //--------------------------------------------------
        /// �ɷ�Axis�ı��¼�
        //--------------------------------------------------
        private void DispatchOnAxisChangedEvent()
        {
            if (m_onAxisChangedEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_onAxisChangedEventID;
                uiEvent.m_eventParams = m_onAxisChangedEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);                
            }
        }

        //--------------------------------------------------
        /// �ɷ�Axis�����¼�
        //--------------------------------------------------
        private void DispatchOnAxisDownEvent()
        {
            if (m_onAxisDownEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_onAxisDownEventID;
                uiEvent.m_eventParams = m_onAxisDownEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);
            }
        }

        //--------------------------------------------------
        /// �ɷ�Axis�ɿ��¼�
        //--------------------------------------------------
        private void DispatchOnAxisReleasedEvent()
        {
            if (m_onAxisReleasedEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_onAxisReleasedEventID;
                uiEvent.m_eventParams = m_onAxisReleasedEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);
            }
        }

        //--------------------------------------------------
        /// �ƶ�Axis
        /// @position   : ��Ļ����(�������������ΪUIForm����)
        /// @isDown     : �Ƿ�Ϊ���²���
        //--------------------------------------------------
        private void MoveAxis(Vector2 position, bool isDown)
        {
            if (isDown || (m_axisCurrentScreenPosition == Vector2.zero && m_axisTargetScreenPosition == Vector2.zero))
            {
                m_axisCurrentScreenPosition = GetFixAixsScreenPosition(position);
                m_axisTargetScreenPosition = m_axisCurrentScreenPosition;
                
                DebugHelper.Assert(m_belongedFormScript != null);

                m_axisRectTransform.position = CUIUtility.ScreenToWorldPoint((m_belongedFormScript != null) ? m_belongedFormScript.GetCamera() : null, m_axisCurrentScreenPosition, m_axisRectTransform.position.z);
            }

            Vector2 deltaPosition = position - m_axisCurrentScreenPosition;
            Vector2 deltaPositionInForm = deltaPosition;

            float radius = deltaPosition.magnitude;
            float radiusInForm = radius;

            //ת��ΪFormֵ
            if (m_belongedFormScript != null)
            {
                radiusInForm = m_belongedFormScript.ChangeScreenValueToForm(radius);

                deltaPositionInForm.x = m_belongedFormScript.ChangeScreenValueToForm(deltaPosition.x);
                deltaPositionInForm.y = m_belongedFormScript.ChangeScreenValueToForm(deltaPosition.y);
            }

            DebugHelper.Assert(m_cursorRectTransform != null);

            //�ƶ�cursor
            m_cursorRectTransform.anchoredPosition = (radiusInForm > m_cursorDisplayMaxRadius) ? (deltaPositionInForm.normalized * m_cursorDisplayMaxRadius) : deltaPositionInForm;

            //�ƶ�Axis
            if (m_isAxisMoveable)
            {
                if (radiusInForm > m_cursorDisplayMaxRadius)
                {
                    DebugHelper.Assert(m_belongedFormScript != null);

                    m_axisTargetScreenPosition = m_axisCurrentScreenPosition + (position - CUIUtility.WorldToScreenPoint((m_belongedFormScript != null) ? m_belongedFormScript.GetCamera() : null, m_cursorRectTransform.position));

                    //��鷶Χ
                    m_axisTargetScreenPosition = GetFixAixsScreenPosition(m_axisTargetScreenPosition);
                }
            }

            if (radiusInForm < m_cursorRespondMinRadius)
            {
                UpdateAxis(Vector2.zero);
            }
            else
            {                
                UpdateAxis(deltaPosition);
            }
        }

        //--------------------------------------------------
        /// ����Joystickλ��
        //--------------------------------------------------
        private void UpdateAxisPosition()
        {
            if (m_axisCurrentScreenPosition != m_axisTargetScreenPosition)
            {
                Vector2 delta = (m_axisTargetScreenPosition - m_axisCurrentScreenPosition);
                Vector2 speed = (m_axisTargetScreenPosition - m_axisCurrentScreenPosition) / 3;

                if (delta.sqrMagnitude <= 1)
                {
                    m_axisCurrentScreenPosition = m_axisTargetScreenPosition;
                }
                else
                {
                    m_axisCurrentScreenPosition += speed;
                }

                m_axisRectTransform.position = CUIUtility.ScreenToWorldPoint(m_belongedFormScript.GetCamera(), m_axisCurrentScreenPosition, m_axisRectTransform.position.z);
            }
        }

        //--------------------------------------------------
        /// ��ȡ�������Axis��Ļλ�ã�ȷ���ڿ��ƶ��ķ�Χ��
        /// @axisScreenPosition
        //--------------------------------------------------
        private Vector2 GetFixAixsScreenPosition(Vector2 axisScreenPosition)
        {
            axisScreenPosition.x = CUIUtility.ValueInRange(axisScreenPosition.x, m_axisOriginalScreenPosition.x + m_belongedFormScript.ChangeFormValueToScreen(m_axisScreenPositionOffsetMin.x), m_axisOriginalScreenPosition.x + m_belongedFormScript.ChangeFormValueToScreen(m_axisScreenPositionOffsetMax.x));
            axisScreenPosition.y = CUIUtility.ValueInRange(axisScreenPosition.y, m_axisOriginalScreenPosition.y + m_belongedFormScript.ChangeFormValueToScreen(m_axisScreenPositionOffsetMin.y), m_axisOriginalScreenPosition.y + m_belongedFormScript.ChangeFormValueToScreen(m_axisScreenPositionOffsetMax.y));

            return axisScreenPosition;
        }

        //--------------------------------------------------
        /// ����Border
        //--------------------------------------------------
        private void HideBorder()
        {
            if (m_borderRectTransform == null || m_borderCanvasGroup == null)
            {
                return;
            }

            if (m_borderCanvasGroup.alpha != 0f)
            {
                m_borderCanvasGroup.alpha = 0f;
            }
            
            if (m_borderCanvasGroup.blocksRaycasts)
            {
                m_borderCanvasGroup.blocksRaycasts = false;
            }
        }

        //--------------------------------------------------
        /// ��ʾborder
        /// @axis
        //--------------------------------------------------
        private void ShowBorder(Vector2 axis)
        {
            if (m_borderRectTransform == null || m_borderCanvasGroup == null)
            {
                return;
            }

            if (m_borderCanvasGroup.alpha != 1f)
            {
                m_borderCanvasGroup.alpha = 1f;
            }

            if (!m_borderCanvasGroup.blocksRaycasts)
            {
                m_borderCanvasGroup.blocksRaycasts = true;
            }

            //����Ƕ�
            m_borderRectTransform.right = axis;
        }

        //--------------------------------------------------
        /// Axis����
        //--------------------------------------------------
        private void AxisFadeIn()
        {
            if (m_axisImage != null)
            {
                m_axisImage.color = new Color(1, 1, 1, 1);
            }

            if (m_cursorImage != null)
            {
                m_cursorImage.color = new Color(1, 1, 1, 1);
            }
        }

        //--------------------------------------------------
        /// Axis����
        //--------------------------------------------------
        private void AxisFadeout()
        {
            if (m_axisImage != null)
            {
                m_axisImage.color = new Color(1, 1, 1, m_axisFadeoutAlpha);
            }

            if (m_cursorImage != null)
            {
                m_cursorImage.color = new Color(1, 1, 1, m_axisFadeoutAlpha);
            }
        }
    };
};