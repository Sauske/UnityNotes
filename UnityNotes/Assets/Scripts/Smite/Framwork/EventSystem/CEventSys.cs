using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public enum ENetEventIds
    {
        None = 0,
        Net_Start,


        //枚举都加在这个之前
        MAX_TAG,
    }

    public enum EUIEventIds
    {
        None = 0,
        Accout_Login,


        MAX_TAG,
    }





    public delegate void EventHandler(IEvent evet);

    public class CEventSys : Singleton<CEventSys>
    {

        public Dictionary<int, List<EventHandler>> mHandlerDic;

        public override void Init()
        {
            base.Init();
            
            mHandlerDic = new Dictionary<int, List<EventHandler>>();
        }

        public override void UnInit()
        {
            base.UnInit();

            foreach(var value in mHandlerDic)
            {
                value.Value.Clear();
            }
            mHandlerDic.Clear();
        }


        public void AddEvent(int key, EventHandler handler)
        {
            List<EventHandler> list;
            if (mHandlerDic.TryGetValue(key, out list))
            {
                list.Add(handler);
                return;
            }
            list = new List<EventHandler>();
            mHandlerDic.Add(key, list);
        }

        public void RemoveEvent(int key)
        {
            List<EventHandler> list;
            if (mHandlerDic.TryGetValue(key, out list))
            {
                list = null;
            }
            mHandlerDic.Remove(key);
        }

        public void TriggerEvent(IEvent evet)
        {
            List<EventHandler> list;
            if (mHandlerDic.TryGetValue(evet.GetKey(), out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i](evet);
                }
            }
        }
    }


    public interface IEvent
    {

        int GetKey();
        object GetParam();
    }

    public class UIEvent : IEvent
    {
        public int key;

        public object param;

        public bool isUse = false;

        public UIEvent()
        {

        }

        public UIEvent(int key, object param)
        {
            this.key = key;
            this.param = param;
        }

        public int GetKey()
        {
            return key;
        }

        public object GetParam()
        {
            return param;
        }

        public void Clear()
        {
            key = 0;
            param = null;
            isUse = false;
        }
    }


    public class NetEvent : IEvent
    {
        public int key;
        public object param1;
        public object param2;

        public NetEvent(int key, object param)
        {
            this.key = key;
            this.param1 = param;
        }

        public NetEvent(int key, object param1, object param2)
        {
            this.key = key;
            this.param1 = param1;
            this.param2 = param2;
        }

        public int GetKey()
        {
            return key;
        }

        public object GetParam()
        {
            return param1;
        }

        public object GetParam2()
        {
            return param2;
        }
    }
}