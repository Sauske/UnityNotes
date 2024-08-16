//==================================================================================
/// UI 3DTouch事件组件
/// @在Down - Up周期内，Touch力度超过阀值即派发一次Touched事件(1个Down-Up周期最多只派发一次)
/// @neoyang
/// @2016.06.07
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class CUI3DTouchEventScript : CUIMiniEventScript
    {
        //--------------------------------------
        /// 事件相关数据
        //--------------------------------------
        [HideInInspector]
        public enUIEventID m_onTouchedEventID;
        [System.NonSerialized]
        public stUIEventParams m_onTouchedEventParams;

        //3DTouch力度阀值
        public float m_3DTouchStrength = 4.0f;

        //相关Wwise音效
        public string[] m_onTouchedWwiseEvents = new string[0];

#if UNITY_IPHONE && !UNITY_EDITOR
        //是否接受到Down操作
        private bool m_isDown = false;

        //一个Down-Up周期内3DTouch事件派发次数
        private uint m_onTouchedEventDispatchedCount = 0;
       
        //控件四个顶点的world位置
        private Vector3[] m_corners = new Vector3[4];
#endif

        //触摸位置相关数据
        private PointerEventData m_touchedPointerEventData;

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

#if UNITY_IPHONE && !UNITY_EDITOR
            Unity3DTouch.GetInstance().Unity3DTouchAction += OnTouched;
#endif
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            base.OnDestroy();

#if UNITY_IPHONE && !UNITY_EDITOR
            Unity3DTouch.GetInstance().Unity3DTouchAction -= OnTouched;
#endif
        }

        //--------------------------------------
        /// 相关接口函数
        //--------------------------------------
        public override void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            m_isDown = true;
            m_onTouchedEventDispatchedCount = 0;

            m_touchedPointerEventData = eventData;
#endif
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            m_isDown = false;
            //m_onTouchedEventDispatchedCount = 0;

            m_touchedPointerEventData = null;
#endif

            base.OnPointerUp(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            if (m_onTouchedEventDispatchedCount >= 1)
            {
                return;
            }
#endif
            base.OnPointerClick(eventData);
        }

#if UNITY_IPHONE && !UNITY_EDITOR
        //--------------------------------------
        /// 3DTouch回调
        //--------------------------------------
        private void OnTouched(object sender, ref Unity3DTouchEvent e)
        {
            if ((m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            || !m_isDown
            || m_onTouchedEventDispatchedCount >= 1
            )
            {
                return;
            }

            //检查力度是否超过伐值
            if (e.force >= m_3DTouchStrength)
            {
                (this.gameObject.transform as RectTransform).GetWorldCorners(m_corners);

                int xMin = int.MaxValue;
                int xMax = int.MinValue;
                int yMin = int.MaxValue;
                int yMax = int.MinValue;

                for (int i = 0; i < m_corners.Length; ++i)
                {
                    Vector3 v = CUIUtility.WorldToScreenPoint(m_belongedFormScript.GetCamera(), m_corners[i]);
            
                    xMin = Mathf.Min((int)v.x, xMin);
                    xMax = Mathf.Max((int)v.x, xMax);
                    yMin = Mathf.Min((int)v.y, yMin);
                    yMax = Mathf.Max((int)v.y, yMax);
                }

                if (e.x >= xMin && e.x <= xMax && e.y >= yMin && e.y <= yMax)
                {
                    //修改touchedPosition
                    if (m_touchedPointerEventData != null)
                    {
                        m_touchedPointerEventData.position = new Vector2(e.x, e.y);
                    }

                    Dispatch3DTouchUIEvent();
                    m_onTouchedEventDispatchedCount++;
                }
            }            
        }

        //--------------------------------------
        /// 派发UI事件
        //--------------------------------------
        private void Dispatch3DTouchUIEvent()
        {
            //触发音效
            PostWwiseEvent(m_onTouchedWwiseEvents);

            if (m_onTouchedEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_onTouchedEventID;
                uiEvent.m_eventParams = m_onTouchedEventParams;

                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = this.gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = m_touchedPointerEventData;

                DispatchUIEvent(uiEvent);
            }  
        }

        //--------------------------------------
        /// 派发wwise事件
        /// @wwiseEvents
        //--------------------------------------
        private void PostWwiseEvent(string[] wwiseEvents)
        {
            for (int i = 0; i < wwiseEvents.Length; i++)
            {
                if (!string.IsNullOrEmpty(wwiseEvents[i]))
                {
                    CSoundManager.GetInstance().PostEvent(wwiseEvents[i]);
                }
            }
        }
#endif
    };
};