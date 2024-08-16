using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class UIEventSys : Singleton<UIEventSys>
    {
        public delegate void UIEventHandler(UIEvent evet);

        private UIEventHandler[] mUIEventMap;

        //通用对象，避免内存碎片
        private List<object> mUIEvents = new List<object>();

        public override void Init()
        {
            base.Init();

            mUIEventMap = new UIEventHandler[(int)EUIEventIds.MAX_TAG];
        }

        public override void UnInit()
        {
            base.UnInit();

            for (int i = 0; i < mUIEventMap.Length; i++)
            {
                mUIEventMap[i] = null;
            }
        }


        public bool HasEvent(EUIEventIds id)
        {
            return mUIEventMap[(int)id] != null;
        }


        public void AddUIEvent(EUIEventIds id, UIEventHandler handler)
        {
            int eventId = (int)id;
            if (mUIEventMap[eventId] == null)
            {
                mUIEventMap[eventId] = delegate { };
                mUIEventMap[eventId] += handler;
            }
            else
            {
                //防止重复添加委托函数
                mUIEventMap[eventId] -= handler;
                mUIEventMap[eventId] += handler;
            }
        }

        public void RemoveUIEvent(EUIEventIds id, UIEventHandler handler)
        {
            int eventId = (int)id;
            if (mUIEventMap[eventId] != null)
            {
                mUIEventMap[eventId] -= handler;
            }
        }

        public void DispatchUIEvent(UIEvent evet)
        {
            evet.isUse = true;

            UIEventHandler handler = mUIEventMap[evet.GetKey()];
            if (handler != null)
            {
                handler(evet);
            }

            //清除param里的指针
            evet.Clear();
        }

        public void DispatchUIEvent(EUIEventIds id)
        {
            UIEvent uiEvent = GetUIEvent();
            uiEvent.key = (int)id;

            DispatchUIEvent(uiEvent);
        }

        public void DispatchUIEvent(EUIEventIds id, object param) 
        {

            UIEvent uiEvent = GetUIEvent();
            uiEvent.key = (int)id;
            uiEvent.param = param;

            DispatchUIEvent(uiEvent);
        }


        public UIEvent GetUIEvent()
        {
            for (int i = 0; i < mUIEvents.Count; i++)
            {
                var obj = (UIEvent)mUIEvents[i];
                if (!obj.isUse)
                {
                    return obj; 
                }
            }

            UIEvent uiEvent = new UIEvent();

            mUIEvents.Add(uiEvent);

            return uiEvent;
        }
    }
}