using System.Collections.Generic;
using UnityEngine;

namespace UMI
{
    /// <summary>
    /// 需要接收事件的抽象类，添加了统一注销事件的函数，防止漏注销
    /// </summary>
    public abstract class BaseEventListener : IEventListener
    {
        protected Dictionary<uint, EventInfo> mEventIdDict = new Dictionary<uint, EventInfo>();
        protected List<uint> mIDLst = new List<uint>();

        protected virtual void RegEvents()
        {
            
        }

        /// <summary>
        /// 注册单个事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventMothed"></param>
        public virtual void RegEvent(uint eventId, EventMothed eventMothed)
        {
            if (eventMothed == null)
            {
                Debug.LogFormat("AddEvent error by eventMothed is null listenertype = {0} eventId = {1}", this.GetType().Name, eventId);
                return;
            }

            if (mEventIdDict.ContainsKey(eventId))
            {
                Debug.LogFormat("eventId = {0} has add in listenertype = {1}", eventId, this.GetType().Name);
                return;
            }

            EventInfo eventInfo = EventMgr.Instance.SpwanEventInfo();
            eventInfo.listener = this;
            eventInfo.eventMothed = eventMothed;

            mEventIdDict.Add(eventId, eventInfo);

            EventMgr.Instance.RegEvent(eventId, eventInfo);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="eventId"></param>
        public virtual void UnregEvent(uint eventId)
        {
            if (mEventIdDict.ContainsKey(eventId))
            {
                EventMgr.Instance.UnregEvent(eventId, mEventIdDict[eventId].listener);
                mEventIdDict[eventId].Dispose();
                EventMgr.Instance.DispwanEventInfo(mEventIdDict[eventId]);
                mEventIdDict.Remove(eventId);
            }
        }

        /// <summary>
        /// 统一注销事件
        /// </summary>
        protected virtual void UnregEvents()
        {
            mIDLst.Clear();
            mIDLst.AddRange(mEventIdDict.Keys);
            foreach (var id in mIDLst)
            {
                UnregEvent(id);
            }

            mIDLst.Clear();
            mEventIdDict.Clear();
        }

    }
}
