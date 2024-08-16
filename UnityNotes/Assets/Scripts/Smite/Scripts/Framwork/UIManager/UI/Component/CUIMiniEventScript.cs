//==================================================================================
/// UI事件组件
/// @对于需要响应事件的控件，需要挂上此脚本并设定操作对应的
/// @目前Drag和Hold相关的操作不为互斥
/// @neoyang
/// @2015.03.02
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class CUIMiniEventScript : CUIComponent, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler
    {
        //--------------------------------------
        /// 事件相关数据
        //--------------------------------------
        [HideInInspector]
        public enUIEventID m_onDownEventID;
        [System.NonSerialized]
        public stUIEventParams m_onDownEventParams;

        [HideInInspector]
        public enUIEventID m_onUpEventID;
        [System.NonSerialized]
        public stUIEventParams m_onUpEventParams;
        
        [HideInInspector]
        public enUIEventID m_onClickEventID;
        [System.NonSerialized]
        public stUIEventParams m_onClickEventParams;

        //Click时是否关闭所属的Form
        public bool m_closeFormWhenClicked;

        //相关Wwise音效
        public string[] m_onDownWwiseEvents = new string[0];
        public string[] m_onClickedWwiseEvents = new string[0];

        //hold需要用到的Pointer事件参数
     //   private PointerEventData m_holdPointerEventData;


        // scotzeng++
        public delegate void OnUIEventHandler(CUIEvent uiEvent);
        public OnUIEventHandler onClick = null;
        // scotzeng--


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
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            onClick = null;

            base.OnDestroy();
        }

        //--------------------------------------
        /// 设置UIEvent
        /// @eventType
        /// @eventID
        //--------------------------------------
        public void SetUIEvent(enUIEventType eventType, enUIEventID eventID)
        {
            switch (eventType)
            {
                case enUIEventType.Down:
                    m_onDownEventID = eventID;
                break;

                case enUIEventType.Up:
                    m_onUpEventID = eventID;
                break;
                    
                case enUIEventType.Click:
                    m_onClickEventID = eventID;
                break;
            }
        }

        //--------------------------------------
        /// 设置UIEvent
        /// @eventType
        /// @eventID
        /// @eventParams
        //--------------------------------------
        public void SetUIEvent(enUIEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
        {
            switch (eventType)
            {
                case enUIEventType.Down:
                    m_onDownEventID = eventID;
                    m_onDownEventParams = eventParams;
                break;

                case enUIEventType.Up:
                    m_onUpEventID = eventID;
                    m_onUpEventParams = eventParams;
                break;
                    
                case enUIEventType.Click:
                    m_onClickEventID = eventID;
                    m_onClickEventParams = eventParams;
                break;

            }
        }

        //--------------------------------------
        /// 相关接口函数
        //--------------------------------------
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //派发事件
            DispatchUIEvent(enUIEventType.Down, eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            //派发事件
            DispatchUIEvent(enUIEventType.Up, eventData);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //不支持多点Click事件的Form每一桢只能派发一次Click事件
            bool canClick = true;
            if (m_belongedFormScript != null && !m_belongedFormScript.m_enableMultiClickedEvent)
            {
                canClick = (m_belongedFormScript.m_clickedEventDispatchedCounter == 0);
                m_belongedFormScript.m_clickedEventDispatchedCounter++;
            }

            if (canClick)
            {
                //如果是作为List元素，执行List选中操作
                if (m_belongedListScript != null && m_indexInlist >= 0)
                {
                    m_belongedListScript.SelectElement(m_indexInlist);
                }

                DispatchUIEvent(enUIEventType.Click, eventData);

                if (m_closeFormWhenClicked && m_belongedFormScript != null)
                {
                    m_belongedFormScript.Close();
                }
            }
        }
       
        void Update()
        {

        }

        //--------------------------------------
        /// 派发UI事件
        //--------------------------------------
        private void DispatchUIEvent(enUIEventType eventType, PointerEventData pointerEventData)
        {
            //string[] names = System.Enum.GetNames(typeof(enUIEventType));
            //Debug.Log("====== " + names[(int)eventType] + " ======");

            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            switch (eventType)
            {
                case enUIEventType.Down:
                {
                    //触发音效
                    PostWwiseEvent(m_onDownWwiseEvents);

                    if (m_onDownEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onDownEventID;
                    uiEvent.m_eventParams = m_onDownEventParams;
                }
                break;

                case enUIEventType.Up:
                {
                    if (m_onUpEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onUpEventID;
                    uiEvent.m_eventParams = m_onUpEventParams;
                }
                break;

                case enUIEventType.Click:
                {
                    //触发音效
                    PostWwiseEvent(m_onClickedWwiseEvents);

                    if (m_onClickEventID == enUIEventID.None)
                    {
                        if (onClick != null)
                        {
                            uiEvent.m_eventID = enUIEventID.None;
                            uiEvent.m_eventParams = m_onClickEventParams;
                            onClick(uiEvent);
                        }

                        return;
                    }

                    uiEvent.m_eventID = m_onClickEventID;
                    uiEvent.m_eventParams = m_onClickEventParams;
                }
                break;
            }

            uiEvent.m_srcFormScript = m_belongedFormScript;
            uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
            uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
            uiEvent.m_srcWidget = this.gameObject;
            uiEvent.m_srcWidgetScript = this;
            uiEvent.m_pointerEventData = pointerEventData;

            if (eventType == enUIEventType.Click && onClick != null)
            {
                onClick(uiEvent);
            }

            DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 派发wwise事件
        /// @wwiseEvents
        //--------------------------------------
        protected void PostWwiseEvent(string[] wwiseEvents)
        {
            for (int i = 0; i < wwiseEvents.Length; i++)
            {
                if (!string.IsNullOrEmpty(wwiseEvents[i]))
                {
                   // CSoundManager.GetInstance().PostEvent(wwiseEvents[i]);
                }
            }
        } 
    };
};