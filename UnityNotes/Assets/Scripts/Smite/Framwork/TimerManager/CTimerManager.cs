//====================================
/// Timer管理器
/// @neoyang
/// @2015.03.18
//====================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{

    public class CTimerManager : Singleton<CTimerManager>
    {
        //Timer类型
        private enum enTimerType
        {
            Normal,
            FrameSync,
        };

        //Timer List
        private List<CTimer>[] m_timers;
        private int m_timerSequence;

        //----------------------------------------------
        /// 初始化
        //----------------------------------------------
        public override void Init()
        {
            m_timers = new List<CTimer>[System.Enum.GetValues(typeof(enTimerType)).Length];

            for (int i = 0; i < m_timers.Length; i++)
            {
                m_timers[i] = new List<CTimer>();
            }

            m_timerSequence = 0;
        }

        //----------------------------------------------
        /// Update
        /// @这里只更新Normal类型的Timer
        //----------------------------------------------
        public void Update()
        {
            UpdateTimer((int)(Time.deltaTime * 1000), enTimerType.Normal);
        }

        //----------------------------------------------
        /// UpdateLogic
        /// @这里只更新FrameSync类型的Timer
        //----------------------------------------------
        public void UpdateLogic(int delta)
        {
            UpdateTimer(delta, enTimerType.FrameSync);
        }

        //----------------------------------------------
        /// UpdateTimer
        /// @delata
        /// @timerType
        //----------------------------------------------
        private void UpdateTimer(int delta, enTimerType timerType)
        {
            List<CTimer> timers = m_timers[(int)timerType];

            for (int i = 0; i < timers.Count;)
            {
                if (timers[i].IsFinished())
                {
                    timers.RemoveAt(i);
                    continue;
                }

                timers[i].Update(delta);
                i++;
            }
        }

        //----------------------------------------------
        /// 添加Timer
        /// @time               : 计时时间(ms)
        /// @loop               : 循环次数
        /// @onTimeUpHandler    : 时间到时的回调函数
        /// @return sequence of timer
        //----------------------------------------------
        public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler)
        {
            return AddTimer(time, loop, onTimeUpHandler, false);
        }

        //----------------------------------------------
        /// 添加Timer（带参数版本）
        //----------------------------------------------
        public int AddTimer(int time, int loop, CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler, stTimerParams timerParams)
        {
            return AddTimer(time, loop, onTimeUpWithParamsHandler, false, timerParams);
        }

        //----------------------------------------------
        /// 添加Timer
        /// @time               : 计时时间(ms)
        /// @loop               : 循环次数
        /// @onTimeUpHandler    : 时间到时的回调函数
        /// @useFrameSync       : 是否使用桢同步
        /// @return sequence of timer
        //----------------------------------------------
        public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
        {
            m_timerSequence++;
            m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)].Add(new CTimer(time, loop, onTimeUpHandler, m_timerSequence));

            return m_timerSequence;
        }

        //----------------------------------------------
        /// 添加Timer（带参数版本）
        //----------------------------------------------
        public int AddTimer(int time, int loop, CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler, bool useFrameSync, stTimerParams timerParams)
        {
            m_timerSequence++;
            m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)].Add(new CTimer(time, loop, onTimeUpWithParamsHandler, m_timerSequence, timerParams));

            return m_timerSequence;
        }

        //----------------------------------------------
        /// 移除Timer
        /// @sequence
        //----------------------------------------------
        public void RemoveTimer(int sequence)
        {
            for (int i = 0; i < m_timers.Length; i++)
            {
                List<CTimer> timers = m_timers[i];

                for (int j = 0; j < timers.Count; j++)
                {
                    if (timers[j].IsSequenceMatched(sequence))
                    {
                        timers[j].Finish();
                        return;
                    }
                }
            }
        }

        //----------------------------------------------
        /// 移除Timer
        /// @sequence: ref，移除后清空
        //----------------------------------------------
        public void RemoveTimerSafely(ref int sequence)
        {
            if (sequence != 0)
            {
                RemoveTimer(sequence);
                sequence = 0;
            }
        }

        //----------------------------------------------
        /// 暂停Timer
        /// @sequence
        //----------------------------------------------
        public void PauseTimer(int sequence)
        {
            CTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.Pause();
            }
        }

        //----------------------------------------------
        /// 恢复Timer
        /// @sequence
        //----------------------------------------------
        public void ResumeTimer(int sequence)
        {
            CTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.Resume();
            }
        }

        //----------------------------------------------
        /// 重置Timer
        /// @sequence
        //----------------------------------------------
        public void ResetTimer(int sequence)
        {
            CTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.Reset();
            }
        }

        //----------------------------------------------
        /// 重设Timer倒计时
        /// @sequence
        /// @totalTime
        //----------------------------------------------
        public void ResetTimerTotalTime(int sequence, int totalTime)
        {
            CTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.ResetTotalTime(totalTime);
            }
        }

        //----------------------------------------------
        /// 获取Timer的当前时间
        /// @sequence
        //----------------------------------------------
        public int GetTimerCurrent(int sequence)
        {
            CTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                return timer.CurrentTime;
            }

            return -1;
        }

        //--------------------------------------
        /// 还差多少时间
        //--------------------------------------
        public int GetLeftTime(int sequence)
        {
            CTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                return timer.GetLeftTime() / 1000;  //转成多少秒
            }

            return -1;
        }

        //----------------------------------------------
        /// 返回指定sequence的Timer
        //----------------------------------------------
        public CTimer GetTimer(int sequence)
        {
            for (int i = 0; i < m_timers.Length; i++)
            {
                List<CTimer> timers = m_timers[i];

                for (int j = 0; j < timers.Count; j++)
                {
                    if (timers[j].IsSequenceMatched(sequence))
                    {
                        return timers[j];
                    }
                }
            }

            return null;
        }

        //----------------------------------------------
        /// 移除Timer
        /// @onTimeUpHandler
        //----------------------------------------------
        public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler)
        {
            RemoveTimer(onTimeUpHandler, false);
        }

        //----------------------------------------------
        /// 移除Timer（带参数版本）
        //----------------------------------------------
        public void RemoveTimer(CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler)
        {
            RemoveTimer(onTimeUpWithParamsHandler, false);
        }

        //----------------------------------------------
        /// 移除Timer
        /// @onTimeUpHandler
        /// @useFrameSync
        //----------------------------------------------
        public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
        {
            List<CTimer> timers = m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)];

            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].IsDelegateMatched(onTimeUpHandler))
                {
                    timers[i].Finish();
                    continue;
                }
            }
        }

        //----------------------------------------------
        /// 移除Timer（带参数版本）
        //----------------------------------------------
        public void RemoveTimer(CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler, bool useFrameSync)
        {
            List<CTimer> timers = m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)];

            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].IsDelegateWithParamsMatched(onTimeUpWithParamsHandler))
                {
                    timers[i].Finish();
                    continue;
                }
            }
        }

        //----------------------------------------------
        /// 移除所有Timer
        /// @timerType
        //----------------------------------------------
        public void RemoveAllTimer(bool useFrameSync)
        {
            m_timers[(int)(useFrameSync ? enTimerType.FrameSync : enTimerType.Normal)].Clear();
        }

        //----------------------------------------------
        /// 移除所有Timer
        //----------------------------------------------
        public void RemoveAllTimer()
        {
            for (int i = 0; i < m_timers.Length; i++)
            {
                m_timers[i].Clear();
            }
        }
    }
}