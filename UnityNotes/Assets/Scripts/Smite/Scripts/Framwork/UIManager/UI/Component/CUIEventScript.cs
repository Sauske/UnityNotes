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
    //UI事件触发类型
    public enum enUIEventType
    {
        Down,                       //按下
        Click,                      //点击
        HoldStart,                  //开始按住
        Hold,                       //按住
        HoldEnd,                    //结束按住
        DragStart,                  //开始拖拽
        Drag,                       //拖拽
        DragEnd,                    //结束拖拽
        Drop,                        //拖拽放下
        Up                           //弹起
    };

    public class CUIEventScript : CUIComponent, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerUpHandler
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

        [HideInInspector]
        public enUIEventID m_onHoldStartEventID;
        [System.NonSerialized]
        public stUIEventParams m_onHoldStartEventParams;

        [HideInInspector]
        public enUIEventID m_onHoldEventID;
        [System.NonSerialized]
        public stUIEventParams m_onHoldEventParams;

        [HideInInspector]
        public enUIEventID m_onHoldEndEventID;
        [System.NonSerialized]
        public stUIEventParams m_onHoldEndEventParams;

        [HideInInspector]
        public enUIEventID m_onDragStartEventID;
        [System.NonSerialized]
        public stUIEventParams m_onDragStartEventParams;

        [HideInInspector]
        public enUIEventID m_onDragEventID;
        [System.NonSerialized]
        public stUIEventParams m_onDragEventParams;

        [HideInInspector]
        public enUIEventID m_onDragEndEventID;
        [System.NonSerialized]
        public stUIEventParams m_onDragEndEventParams;

        [HideInInspector]
        public enUIEventID m_onDropEventID;
        [System.NonSerialized]
        public stUIEventParams m_onDropEventParams;

        //Click时是否关闭所属的Form
        [HideInInspector]
        public bool m_closeFormWhenClicked;

        //是否向list传递拖拽事件,如果属于一个list
        [HideInInspector]
        public bool m_isDispatchDragEventForBelongList = true;

        //相关Wwise音效
        public string[] m_onDownWwiseEvents = new string[0];
        public string[] m_onClickedWwiseEvents = new string[0];

		//public Animator m_reactAnimator = null;
		//是否启用事件：禁用事件后，CUIEventScript仍然会监听系统消息，但是不会分发事件，如果收到消息会播放禁用音效
		//private bool m_enableEvent = true;

        //操作标志
        private bool m_isDown = false;
        private bool m_isHold = false;
        private bool m_canClick = false;
        
        //相关伐值及参数
        public float c_holdTimeValue = 1.0f;
        private const float c_clickAreaValue = 40f;
        private float m_downTimestamp = 0;
        private Vector2 m_downPosition;

        //hold需要用到的Pointer事件参数
        private PointerEventData m_holdPointerEventData;

        //是否需要clear输入状态
        private bool m_needClearInputStatus = false;

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

        //--------------------------------------
        /// 销毁
        //--------------------------------------
        protected override void OnDestroy()
        {
            m_holdPointerEventData = null;
            onClick = null;

            base.OnDestroy();
        }

        //--------------------------------------
        /// 关闭
        //--------------------------------------
        public override void Close()
        {
            ExecuteClearInputStatus();
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

                case enUIEventType.HoldStart:
                    m_onHoldStartEventID = eventID;
                break;

                case enUIEventType.Hold:
                    m_onHoldEventID = eventID;
                break;

                case enUIEventType.HoldEnd:
                    m_onHoldEndEventID = eventID;
                break;

                case enUIEventType.DragStart:
                    m_onDragStartEventID = eventID;
                break;

                case enUIEventType.Drag:
                    m_onDragEventID = eventID;
                break;

                case enUIEventType.DragEnd:
                    m_onDragEndEventID = eventID;
                break;

                case enUIEventType.Drop:
                    m_onDropEventID = eventID;
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

                case enUIEventType.HoldStart:
                    m_onHoldStartEventID = eventID;
                    m_onHoldStartEventParams = eventParams;
                break;

                case enUIEventType.Hold:
                    m_onHoldEventID = eventID;
                    m_onHoldEventParams = eventParams;
                break;

                case enUIEventType.HoldEnd:
                    m_onHoldEndEventID = eventID;
                    m_onHoldEndEventParams = eventParams;
                break;

                case enUIEventType.DragStart:
                    m_onDragStartEventID = eventID;
                    m_onDragStartEventParams = eventParams;
                break;

                case enUIEventType.Drag:
                    m_onDragEventID = eventID;
                    m_onDragEventParams = eventParams;
                break;

                case enUIEventType.DragEnd:
                    m_onDragEndEventID = eventID;
                    m_onDragEndEventParams = eventParams;
                break;

                case enUIEventType.Drop:
                    m_onDropEventID = eventID;
                    m_onDropEventParams = eventParams;
                break;
            }
        }

        //--------------------------------------
        /// 相关接口函数
        //--------------------------------------
        public void OnPointerDown(PointerEventData eventData)
        {
            m_isDown = true;
            m_isHold = false;
            m_canClick = true;
            m_downTimestamp = Time.realtimeSinceStartup;
            m_downPosition = eventData.position;
            m_holdPointerEventData = eventData;

            m_needClearInputStatus = false;

            //派发事件
            DispatchUIEvent(enUIEventType.Down, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //处理holdend
            if (m_isHold && m_holdPointerEventData != null)
            {
                DispatchUIEvent(enUIEventType.HoldEnd, m_holdPointerEventData);
            }
           
            //派发事件
            DispatchUIEvent(enUIEventType.Up, eventData);

            //clear输入状态
            ClearInputStatus();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //不支持多点Click事件的Form每一桢只能派发一次Click事件
            bool canClick = true;
            if (m_belongedFormScript != null && !m_belongedFormScript.m_enableMultiClickedEvent)
            {
                canClick = (m_belongedFormScript.m_clickedEventDispatchedCounter == 0);
                m_belongedFormScript.m_clickedEventDispatchedCounter++;
            }

            if (m_canClick && canClick)
            {
                //如果是作为List元素，执行List选中操作
                if (m_belongedListScript != null && m_indexInlist >= 0)
                {
                    m_belongedListScript.SelectElement(m_indexInlist);
                }

                //派发事件
                DispatchUIEvent(enUIEventType.Click, eventData);

                if (m_closeFormWhenClicked && m_belongedFormScript != null)
                {
                    m_belongedFormScript.Close();
                }
            }

            //clear输入状态
            ClearInputStatus();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //Drag值超出伐值，不能再响应click
            if (m_canClick && m_belongedFormScript != null && m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > c_clickAreaValue)
            {
                m_canClick = false;
            }            

            //派发事件
            DispatchUIEvent(enUIEventType.DragStart, eventData);

            //冒泡
            if (m_belongedListScript != null && m_belongedListScript.m_scrollRect != null && m_isDispatchDragEventForBelongList == true)
            {
                m_belongedListScript.m_scrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Drag值超出伐值，不能再响应click
            if (m_canClick && m_belongedFormScript != null && m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > c_clickAreaValue)
            {
                m_canClick = false;
            }  

            //派发事件
            DispatchUIEvent(enUIEventType.Drag, eventData);

            //冒泡
            if (m_belongedListScript != null && m_belongedListScript.m_scrollRect != null && m_isDispatchDragEventForBelongList == true)
            {
                m_belongedListScript.m_scrollRect.OnDrag(eventData);
            }            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Drag值超出伐值，不能再响应click
            if (m_canClick && m_belongedFormScript != null && m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, m_downPosition)) > c_clickAreaValue)
            {
                m_canClick = false;
            }

            //派发事件
            DispatchUIEvent(enUIEventType.DragEnd, eventData);         

            //冒泡
            if (m_belongedListScript != null && m_belongedListScript.m_scrollRect != null && m_isDispatchDragEventForBelongList == true)
            {
                m_belongedListScript.m_scrollRect.OnEndDrag(eventData);
            }

            //clear输入状态
            ClearInputStatus();
        }

        public void OnDrop(PointerEventData eventData)
        {
            DispatchUIEvent(enUIEventType.Drop, eventData);
        }

        //--------------------------------------
        /// Clear输入状态
        //--------------------------------------
        public bool ClearInputStatus()
        {
            m_needClearInputStatus = true;
            return m_isDown;
        }

        //--------------------------------------
        /// 执行Clear输入状态
        //--------------------------------------
        public void ExecuteClearInputStatus()
        {
            m_isDown = false;
            m_isHold = false;
            m_canClick = false;
            m_downTimestamp = 0;
            m_downPosition = Vector2.zero;
            m_holdPointerEventData = null;
        }

        void Update()
        {
            if (m_needClearInputStatus)
            {
                return;
            }

            if (m_isDown)
            {
                if (!m_isHold)
                {
                    if (Time.realtimeSinceStartup - m_downTimestamp >= c_holdTimeValue)
                    {
                        m_isHold = true;
                        m_canClick = false;
                        DispatchUIEvent(enUIEventType.HoldStart, m_holdPointerEventData);
                    }                    
                }
                else
                {
                    DispatchUIEvent(enUIEventType.Hold, m_holdPointerEventData);
                }
            }
        }

        void LateUpdate()
        {
            if (m_needClearInputStatus)
            {
                ExecuteClearInputStatus();
                m_needClearInputStatus = false;
            }
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
                    //PostWwiseEvent(m_onDownWwiseEvents);

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
                    PostWwiseEvent(m_onDownWwiseEvents);
                    PostWwiseEvent(m_onClickedWwiseEvents);

                    if (m_onClickEventID == enUIEventID.None)
                    {
                        if (onClick != null)
                        {
                            uiEvent.m_eventID = enUIEventID.None;
                            uiEvent.m_eventParams = m_onClickEventParams;
                            uiEvent.m_srcWidget = this.gameObject;
                            onClick(uiEvent);
                        }

                        return;
                    }

                    uiEvent.m_eventID = m_onClickEventID;
                    uiEvent.m_eventParams = m_onClickEventParams;
                }
                break;

                case enUIEventType.HoldStart:
                {
                    if (m_onHoldStartEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onHoldStartEventID;
                    uiEvent.m_eventParams = m_onHoldStartEventParams;
                }
                break;

                case enUIEventType.Hold:
                {
                    if (m_onHoldEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onHoldEventID;
                    uiEvent.m_eventParams = m_onHoldEventParams;
                }
                break;

                case enUIEventType.HoldEnd:
                {
                    if (m_onHoldEndEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onHoldEndEventID;
                    uiEvent.m_eventParams = m_onHoldEndEventParams;
                }
                break;

                case enUIEventType.DragStart:
                {
                    if (m_onDragStartEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onDragStartEventID;
                    uiEvent.m_eventParams = m_onDragStartEventParams;
                }
                break;

                case enUIEventType.Drag:
                {
                    if (m_onDragEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onDragEventID;
                    uiEvent.m_eventParams = m_onDragEventParams;
                }
                break;

                case enUIEventType.DragEnd:
                {
                    if (m_onDragEndEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onDragEndEventID;
                    uiEvent.m_eventParams = m_onDragEndEventParams;
                }
                break;

                case enUIEventType.Drop:
                {
                    if (m_onDropEventID == enUIEventID.None)
                    {
                        return;
                    }

                    uiEvent.m_eventID = m_onDropEventID;
                    uiEvent.m_eventParams = m_onDropEventParams;
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
        private void PostWwiseEvent(string[] wwiseEvents)
        {
            for (int i = 0; i < wwiseEvents.Length; i++)
            {
                if (!string.IsNullOrEmpty(wwiseEvents[i]))
                {
                 //   CSoundManager.GetInstance().PostEvent(wwiseEvents[i]);
                }
            }
        }
        /*
		public bool enableEvent
		{
			get { return m_enableEvent; }
			set
			{
				if (value != m_enableEvent)
				{
					m_enableEvent = value;
					doReact(m_enableEvent ? ReactState.normal : ReactState.disable);
				}
			}
		}

		//#region 交互反馈表现
		///本段CODE：一个交互GameObject对应的交互反馈表现，依赖的表现主件有Animator与Image
		///可以分离成一个单独的组件实现，但由于必须与CUIEventScript的事件响应强交互，所以暂时实现在一起
		private enum ReactState
		{
			normal,
			pressDown,
			pressUp,
			disable
		}
		private void doEventReact(enUIEventType eventType)
		{
			if (!m_reactAnimator)
				return;

			switch (eventType)
			{
			case enUIEventType.Down:
				doReact(ReactState.pressDown);
				break;
			case enUIEventType.Click:
			case enUIEventType.HoldEnd:
			case enUIEventType.DragEnd:
				doReact(ReactState.pressUp);
				break;
			}
		}
		private void doReact(ReactState reactState)
		{
			if (!m_reactAnimator)
				return;

			Utility.PlayAnimOnce(m_reactAnimator, reactState.ToString());
		}
        */
		//#endregion
    };
};