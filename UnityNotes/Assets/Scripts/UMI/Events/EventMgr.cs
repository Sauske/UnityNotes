using System.Collections.Generic;
using UnityEngine.Pool;

namespace UMI
{
    public delegate void EventMothed(object data);

    public class EventInfo
    {
        public IEventListener listener;
        public EventMothed eventMothed;

        public void Dispose()
        {
            listener = null;
            eventMothed = null;
        }
    }

    /// <summary>
    /// 事件管理单例，注册合注销事件尽量不直接调用这个单例的函数，使用 BaseEventListener 来间接调用
    /// </summary>
    public class EventMgr : Singleton<EventMgr>
    {
        private Dictionary<uint, List<EventInfo>> mListenerDict = new Dictionary<uint, List<EventInfo>>();
        private List<EventInfo> mCloneLst = new List<EventInfo>();
        private List<uint> mEventKeyLst = new List<uint>();

        #region EventPool

        private static void CreatePool()
        {

        }

        private static EventInfo CreateEventInfo()
        {
            return new EventInfo();
        }

        public EventInfo SpwanEventInfo()
        {
            return GenericPool<EventInfo>.Get();
        }

        public void DispwanEventInfo(EventInfo info)
        {
            GenericPool<EventInfo>.Release(info);
        }

        #endregion

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventInfo"></param>
        public void RegEvent(uint eventId, EventInfo eventInfo)
        {
            if (!mListenerDict.ContainsKey(eventId))
            {
                mListenerDict.Add(eventId, new List<EventInfo>());
            }

            mListenerDict[eventId].Add(eventInfo);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventMothed"></param>
        /// <param name="listener"></param>
        public void RegEvent(uint eventId, EventMothed eventMothed, IEventListener listener)
        {
            EventInfo eventInfo = SpwanEventInfo();
            eventInfo.listener = listener;
            eventInfo.eventMothed = eventMothed;
            EventMgr.Instance.RegEvent(eventId, eventInfo);
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="listener"></param>
        public void UnregEvent(uint eventId, IEventListener listener)
        {
            if (mListenerDict.ContainsKey(eventId))
            {
                int count = mListenerDict[eventId].Count;
                for (int i = 0; i < count; i++)
                {
                    if (mListenerDict[eventId][i].listener == listener)
                    {
                        mListenerDict[eventId].RemoveAt(i);
                        if (mListenerDict[eventId].Count == 0)
                        {
                            mListenerDict.Remove(eventId);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="listener"></param>
        public void UnregEvents(IEventListener listener)
        {
            mEventKeyLst.Clear();
            mEventKeyLst.AddRange(mListenerDict.Keys);
            foreach (var eventId in mEventKeyLst)
            {
                UnregEvent(eventId, listener);
            }
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="param"></param>
        public void SendEvent(uint eventId, object param = null)
        {
            if (!mListenerDict.ContainsKey(eventId))
            {
                return;
            }

            mCloneLst.Clear();
            mCloneLst.AddRange(mListenerDict[eventId]);

            for (int n = 0; n < mCloneLst.Count; n++)
            {
#if !UNITY_EDITOR
                try
                {
                    if (mCloneLst[n].eventMothed != null)
                    {
                        mCloneLst[n].eventMothed.Invoke(param);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.ErrorWithTag(LogTagConfig.EventLogTag, " Exception = {0} , StackTrace = {1}", ex.ToString(), ex.StackTrace );
                }
#else
                if (mCloneLst[n].eventMothed != null)
                {
                    mCloneLst[n].eventMothed.Invoke(param);
                }
#endif
            }

            mCloneLst.Clear();
        }
    }
}