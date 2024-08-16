//==================================================================================
/// UI事件管理器
/// @作为观察者派发UI相关事件，游戏各个业务模块需要在UI事件管理器上添加对UI事件的监听
/// @neoyang
/// @2015.03.02
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CUIEventManager : Singleton<CUIEventManager>
    {
        //UI事件委托
        public delegate void OnUIEventHandler(CUIEvent uiEvent);
        //private Dictionary<enUIEventID, OnUIEventHandler> m_uiEventHandlerMap = new Dictionary<enUIEventID, OnUIEventHandler>();
        private OnUIEventHandler[] m_uiEventHandlerMap = new OnUIEventHandler[(int)enUIEventID.MAX_Tag];

        //通用对象，避免内存碎片
        private List<object> m_uiEvents = new List<object>();


        //--------------------------------------
        /// 判断是否有注册过 eventID 这个事件
        /// @eventID
        //--------------------------------------
        public bool HasUIEventListener(enUIEventID eventID)
        {
            return m_uiEventHandlerMap[(uint) eventID] != null;
        }

        //--------------------------------------
        /// 添加UIEvent监听
        /// @eventID
        /// @onUIEventHandler
        //--------------------------------------
        public void AddUIEventListener(enUIEventID eventID, OnUIEventHandler onUIEventHandler)
        {
            if (m_uiEventHandlerMap[(uint)eventID] == null)
            {
                m_uiEventHandlerMap[(uint)eventID] = delegate { };
                m_uiEventHandlerMap[(uint)eventID] += onUIEventHandler;
            }
            else
            {
                //防止重复添加委托函数
                m_uiEventHandlerMap[(uint)eventID] -= onUIEventHandler;
                m_uiEventHandlerMap[(uint)eventID] += onUIEventHandler;                
            }
        }

        //--------------------------------------
        /// 移除UIEvent监听
        /// @eventID
        /// @onUIEventHandler
        //--------------------------------------
        public void RemoveUIEventListener(enUIEventID eventID, OnUIEventHandler onUIEventHandler)
        {
            //OnUIEventHandler onUIEventHandlers = m_uiEventHandlerMap[(uint)eventID];
            
            //if (onUIEventHandlers != null)
            //{
            //    onUIEventHandlers -= onUIEventHandler;                
            //}

            if (m_uiEventHandlerMap[(uint)eventID] != null)
            {
                m_uiEventHandlerMap[(uint)eventID] -= onUIEventHandler;
            }
        }

        //--------------------------------------
        /// 派发UI事件
        /// @uiEvent
        //--------------------------------------
        public void DispatchUIEvent(CUIEvent uiEvent)
        {
            uiEvent.m_inUse = true;

            OnUIEventHandler onUIEventHandlers = m_uiEventHandlerMap[(uint)uiEvent.m_eventID];
            
            if (onUIEventHandlers != null)
            {
                onUIEventHandlers(uiEvent);
            }

            //这里必须要Clear掉Form、控件、Script的指针
            uiEvent.Clear();
        }

        //--------------------------------------
        /// 派发UI事件
        /// @eventID
        //--------------------------------------
        public void DispatchUIEvent(enUIEventID eventID)
        {
            CUIEvent uiEvent = GetUIEvent();
            uiEvent.m_eventID = eventID;

            DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 派发UI事件
        /// @eventID
        /// @par
        //--------------------------------------
        public void DispatchUIEvent(enUIEventID eventID, stUIEventParams par)
        {
            CUIEvent uiEvent = GetUIEvent();
            uiEvent.m_eventID = eventID;
            uiEvent.m_eventParams = par;

            DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 返回公用UI事件对象
        //--------------------------------------
        public CUIEvent GetUIEvent()
        {
            for (int i = 0; i < m_uiEvents.Count; i++)
            {
                var obj = (CUIEvent)m_uiEvents[i];
                if (!obj.m_inUse)
                {
                    return obj;
                }
            }

            CUIEvent uiEvent = new CUIEvent();
            m_uiEvents.Add(uiEvent);

            return uiEvent;
        }
    };
};