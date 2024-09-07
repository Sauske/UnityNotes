using System;
using System.Collections.Generic;

namespace UMI
{
    // 事件分发器
    public class UMIEventDispatch
    {
        protected Dictionary<string, List<Delegate>> Events
        {
            get
            {
                if (mEventMap == null)
                    mEventMap = new Dictionary<string, List<Delegate>>();
                return mEventMap;
            }
        }

        public void Reset()
        {
            Events.Clear();
        }

        public void AddEvent(string evtName, Action evt)
        {
            List<Delegate> evts;
            if (Events.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>(4);
                    evts.Add(evt);
                    Events[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>(4);
                evts.Add(evt);
                Events.Add(evtName, evts);
            }

        }


        public void RemoveEvent(string evtName, Action evt)
        {
            if ((mEventMap == null) || (evt == null))
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T>(string evtName, Action<T> evt)
        {
            List<Delegate> evts;
            if (Events.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    Events[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                Events.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T>(string evtName, Action<T> evt)
        {
            if ((mEventMap == null) || (evt == null))
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U>(string evtName, Action<T, U> evt)
        {
            List<Delegate> evts;
            if (Events.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    Events[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                Events.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T, U>(string evtName, Action<T, U> evt)
        {
            if ((mEventMap == null) || (evt == null))
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U, V>(string evtName, Action<T, U, V> evt)
        {
            List<Delegate> evts;
            if (Events.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    Events[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                Events.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T, U, V>(string evtName, Action<T, U, V> evt)
        {
            if ((mEventMap == null) || (evt == null))
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U, V, W>(string evtName, Action<T, U, V, W> evt)
        {
            List<Delegate> evts;
            if (Events.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    Events[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                Events.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T, U, V, W>(string evtName, Action<T, U, V, W> evt)
        {
            if ((mEventMap == null) || (evt == null))
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void TriggerEvent(string evtName)
        {
            if (mEventMap == null)
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action act = evts[i] as Action;
                    if (act != null)
                        act();
                }
            }
        }

        public void TriggerEvent<T>(string evtName, T V1)
        {
            if (mEventMap == null)
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T> act = evts[i] as Action<T>;
                    if (act != null)
                        act(V1);
                }
            }
        }

        public void TriggerEvent<T, U>(string evtName, T V1, U V2)
        {
            if (mEventMap == null)
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U> act = evts[i] as Action<T, U>;
                    if (act != null)
                        act(V1, V2);
                }
            }
        }

        public void TriggerEvent<T, U, V>(string evtName, T V1, U V2, V V3)
        {
            if (mEventMap == null)
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U, V> act = evts[i] as Action<T, U, V>;
                    if (act != null)
                        act(V1, V2, V3);
                }
            }
        }

        public void TriggerEvent<T, U, V, W>(string evtName, T V1, U V2, V V3, W V4)
        {
            if (mEventMap == null)
                return;

            List<Delegate> evts;
            if (mEventMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U, V, W> act = evts[i] as Action<T, U, V, W>;
                    if (act != null)
                        act(V1, V2, V3, V4);
                }
            }
        }

        // event Name, function
        private Dictionary<string, List<Delegate>> mEventMap = null;
    }


    /// <summary>
    /// 多事件分发器
    /// </summary>
    public class MultiEventDispatch
    {
        protected Dictionary<uint, Dictionary<string, List<Delegate>>> Events
        {
            get
            {
                if (mEventMap == null)
                    mEventMap = new Dictionary<uint, Dictionary<string, List<Delegate>>>();
                return mEventMap;
            }
        }

        public void Reset()
        {
            Events.Clear();
        }

        public void AddEvent(uint id, string evtName, Action evt)
        {
            if (!Events.ContainsKey(id))
            {
                Events.Add(id, new Dictionary<string, List<Delegate>>());
            }

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>(10);
                    evts.Add(evt);
                    evtMap[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>(10);
                evts.Add(evt);
                evtMap.Add(evtName, evts);
            }

        }

        public void RemoveEvent(uint id)
        {
            if ((mEventMap == null) || !mEventMap.ContainsKey(id))
                return;

            mEventMap.Remove(id);
        }

        public void RemoveEvent(uint id, string evtName, Action evt)
        {
            if ((mEventMap == null) || (evt == null) || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T>(uint id, string evtName, Action<T> evt)
        {
            if (!Events.ContainsKey(id))
            {
                Events.Add(id, new Dictionary<string, List<Delegate>>());
            }

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    evtMap[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                evtMap.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T>(uint id, string evtName, Action<T> evt)
        {
            if ((mEventMap == null) || (evt == null) || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U>(uint id, string evtName, Action<T, U> evt)
        {
            if (!Events.ContainsKey(id))
            {
                Events.Add(id, new Dictionary<string, List<Delegate>>());
            }

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    evtMap[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                evtMap.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T, U>(uint id, string evtName, Action<T, U> evt)
        {
            if ((mEventMap == null) || (evt == null) || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U, V>(uint id, string evtName, Action<T, U, V> evt)
        {
            if (!Events.ContainsKey(id))
            {
                Events.Add(id, new Dictionary<string, List<Delegate>>());
            }

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    evtMap[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                evtMap.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T, U, V>(uint id, string evtName, Action<T, U, V> evt)
        {
            if ((mEventMap == null) || (evt == null) || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U, V, W>(uint id, string evtName, Action<T, U, V, W> evt)
        {
            if (!Events.ContainsKey(id))
            {
                Events.Add(id, new Dictionary<string, List<Delegate>>());
            }

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    evtMap[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                evtMap.Add(evtName, evts);
            }
        }

        public void RemoveEvent<T, U, V, W>(uint id, string evtName, Action<T, U, V, W> evt)
        {
            if ((mEventMap == null) || (evt == null) || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void AddEvent<T, U, V, W, K>(uint id, string evtName, Action<T, U, V, W, K> evt)
        {
            if (!Events.ContainsKey(id))
            {
                Events.Add(id, new Dictionary<string, List<Delegate>>());
            }

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Add(evt);
                }
                else
                {
                    evts = new List<Delegate>();
                    evts.Add(evt);
                    evtMap[evtName] = evts;
                }
            }
            else
            {
                evts = new List<Delegate>();
                evts.Add(evt);
                evtMap.Add(evtName, evts);
            }
        }
        public void RemoveEvent<T, U, V, W, K>(uint id, string evtName, Action<T, U, V, W, K> evt)
        {
            if ((mEventMap == null) || (evt == null) || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = Events[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts))
            {
                if (evts != null)
                {
                    evts.Remove(evt);
                }
            }
        }

        public void TriggerEvent(uint id, string evtName)
        {
            if (mEventMap == null || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = mEventMap[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action act = evts[i] as Action;
                    if (act != null)
                        act();
                }
            }
        }

        public void TriggerEvent<T>(uint id, string evtName, T V1)
        {
            if (mEventMap == null || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = mEventMap[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T> act = evts[i] as Action<T>;
                    if (act != null)
                        act(V1);
                }
            }
        }

        public void TriggerEvent<T, U>(uint id, string evtName, T V1, U V2)
        {
            if (mEventMap == null || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = mEventMap[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U> act = evts[i] as Action<T, U>;
                    if (act != null)
                        act(V1, V2);
                }
            }
        }

        public void TriggerEvent<T, U, V>(uint id, string evtName, T V1, U V2, V V3)
        {
            if (mEventMap == null || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = mEventMap[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U, V> act = evts[i] as Action<T, U, V>;
                    if (act != null)
                        act(V1, V2, V3);
                }
            }
        }

        public void TriggerEvent<T, U, V, W>(uint id, string evtName, T V1, U V2, V V3, W V4)
        {
            if (mEventMap == null || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = mEventMap[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U, V, W> act = evts[i] as Action<T, U, V, W>;
                    if (act != null)
                        act(V1, V2, V3, V4);
                }
            }
        }

        public void TriggerEvent<T, U, V, W, K>(uint id, string evtName, T V1, U V2, V V3, W V4, K V5)
        {
            if (mEventMap == null || !mEventMap.ContainsKey(id))
                return;

            Dictionary<string, List<Delegate>> evtMap = mEventMap[id];
            List<Delegate> evts;
            if (evtMap.TryGetValue(evtName, out evts) && evts != null)
            {
                for (int i = 0; i < evts.Count; ++i)
                {
                    Action<T, U, V, W, K> act = evts[i] as Action<T, U, V, W, K>;
                    if (act != null)
                        act(V1, V2, V3, V4, V5);
                }
            }
        }

        private Dictionary<uint, Dictionary<string, List<Delegate>>> mEventMap = null;
    }
}
