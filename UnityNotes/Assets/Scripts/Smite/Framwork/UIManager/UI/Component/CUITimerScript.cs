//====================================
/// UI Timer 控件
/// @neoyang
/// @2015.03.14
//====================================

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Framework
{
    //Timer类型
    public enum enTimerType
    {
        CountUp,
        CountDown
    };

    //Timer显示类型
    public enum enTimerDisplayType
    {
        None,
        H_M_S,        
        M_S,
        S,
        H_M,
        D_H_M_S,
        D_H_M,
        D,
    };

    //Timer事件类型
    public enum enTimerEventType
    {
        TimeStart,
        TimeUp,
        TimeChanged
    };

    public class CUITimerScript : CUIComponent
    {
        //Timer类型
        public enTimerType m_timerType;

        //Timer显示类型
        public enTimerDisplayType m_timerDisplayType;

        //总计数时间及当前时间(s)
        public double m_totalTime;
        private double m_currentTime;

        //派发change事件时间间隔(s)
        public double m_onChangedIntervalTime = 1f;
        private double m_lastOnChangedTime;

        //是否处于运行状态
        private bool m_isRunning;

        //是否处于暂停状态
        private bool m_isPaused;

        //timer启动时的时间点
        private double m_startTime;
        //暂停的时间点
        private double m_pauseTime;
        //暂停的持续时间
        private double m_pauseElastTime;

        //是否立即运行
        public bool m_runImmediately;

        //时间到是否关闭所属Form
        public bool m_closeBelongedFormWhenTimeup;

        //游戏暂停时，timer是否也暂停
        public bool m_pausedWhenAppPaused = true;

        //----------------------------
        /// 事件相关
        //----------------------------
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[System.Enum.GetValues(typeof(enTimerEventType)).Length];

        //事件参数
        public stUIEventParams[] m_eventParams = new stUIEventParams[System.Enum.GetValues(typeof(enTimerEventType)).Length];

        //时间显示
		private Text m_timerText;

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            if (m_runImmediately)
            {
                StartTimer();
            }

            m_timerText = GetComponentInChildren<Text>(gameObject);

            if (m_timerDisplayType == enTimerDisplayType.None && m_timerText != null)
            {
                m_timerText.gameObject.CustomSetActive(false);               
            }

            RefreshTimeDisplay();
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_timerText = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Close
        /// @保证Form关闭的时候timer停止，并重置
        //--------------------------------------------------
        public override void Close()
        {
            base.Close();

            ResetTime();
        }

        void Update()
        {
            if (m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            {
                return;
            }

            UpdateTimer();
        }

        //----------------------------
        /// 设置总时间
        /// @time
        //----------------------------
        public void SetTotalTime(float time)
        {
            m_totalTime = time;

            RefreshTimeDisplay();
        }

        public void SetTimerEventId(enTimerEventType eventType, enUIEventID eventId)
        {
            int iEventType = (int)eventType;
            if (iEventType >= 0 && iEventType < m_eventIDs.Length)
            {
                m_eventIDs[iEventType] = eventId;
            }
        }

        //----------------------------
        /// 设置当前时间
        /// @time
        //----------------------------
        public void SetCurrentTime(float time)
        {
            m_currentTime = time;
        }
        public float GetCurrentTime()
        {
            return (float)m_currentTime;
        }

        //----------------------------
        /// 设置派发计时改变事件的时间间隔
        /// @intervalTime
        //----------------------------
        public void SetOnChangedIntervalTime(float intervalTime)
        {
            m_onChangedIntervalTime = intervalTime;
        }

        //----------------------------
        /// 重置时间
        //----------------------------
        public void ResetTime()
        {
            m_startTime = Time.realtimeSinceStartup;
            m_pauseTime = 0;
            m_pauseElastTime = 0;

            if (m_timerType == enTimerType.CountUp)
            {
                m_currentTime = 0;
            }
            else if (m_timerType == enTimerType.CountDown)
            {
                m_currentTime = m_totalTime;
            }

            m_lastOnChangedTime = m_currentTime;

            //重置时，让文本内容暂时为空，以免出现下一帧才刷新内容的延迟问题。
            //RefreshTimeDisplay();
        }

        //----------------------------
        /// 开始计时
        //----------------------------
        public void StartTimer()
        {
            if (m_isRunning)
            {
                return;
            }

            ResetTime();
            m_isRunning = true;

            //派发事件
            DispatchTimerEvent(enTimerEventType.TimeStart);
        }

        //----------------------------
        /// 重新开始计时
        //----------------------------
        public void ReStartTimer()
        {
            EndTimer();
            StartTimer();
        }

        //----------------------------
        /// App暂停/恢复回调
        //----------------------------
        public void OnApplicationPause(bool pause)
        {
            if (!m_pausedWhenAppPaused)
            {
                return;
            }

            //DebugHelper.Log("CUITimerScript OnApplicationPause");
            
            if (pause)
            {
                PauseTimer();
            }
            else
            {
                ResumeTimer();
            }
        }

        //----------------------------
        /// 暂停计时
        //----------------------------
        public void PauseTimer()
        {
            if (m_isPaused)
            {
                return;
            }

            m_pauseTime = Time.realtimeSinceStartup;
            m_isPaused = true;

            DebugHelper.Log(String.Format("Pause Timer Time:{0}", m_pauseTime));
        }

        //----------------------------
        /// 恢复计时
        //----------------------------
        public void ResumeTimer()
        {
            if (!m_isPaused)
            {
                return;
            }

            m_pauseElastTime += Time.realtimeSinceStartup - m_pauseTime;
            m_isPaused = false;

            DebugHelper.Log(String.Format("Resume Timer Time:{0}", Time.realtimeSinceStartup));
            DebugHelper.Log(String.Format("Pause Elast Time:{0}", m_pauseElastTime));
        }

        //----------------------------
        /// 结束计时
        //----------------------------
        public void EndTimer()
        {
            ResetTime();
            m_isRunning = false;
        }

        //----------------------------
        /// Update计时器
        //----------------------------
        private void UpdateTimer()
        {
            if (!m_isRunning || m_isPaused)
            {
                return;
            }

            bool timeUp = false;

            double lastTime = m_currentTime;

            switch (m_timerType)
            {
                case enTimerType.CountUp:
                    //需要扣除暂停的时长
                    m_currentTime = Time.realtimeSinceStartup - m_startTime - m_pauseElastTime;
                    timeUp = (m_currentTime >= m_totalTime);
                break;

                case enTimerType.CountDown:
                    //需要扣除暂停的时长
                    m_currentTime = m_totalTime - (Time.realtimeSinceStartup - m_startTime - m_pauseElastTime);
                    timeUp = (m_currentTime <= 0);
                break;
            }

            if ((int)lastTime != (int)m_currentTime)
            {
                //刷新时间显示
                RefreshTimeDisplay();
            }
            
            if (Mathf.Abs((float)(m_currentTime - m_lastOnChangedTime)) >= m_onChangedIntervalTime)
            {                
                m_lastOnChangedTime = m_currentTime;

                //派发事件
                DispatchTimerEvent(enTimerEventType.TimeChanged);
            }

            if (timeUp)
            {
                EndTimer();

                //派发事件
                DispatchTimerEvent(enTimerEventType.TimeUp);

                if (m_closeBelongedFormWhenTimeup)
                {
                    m_belongedFormScript.Close();
                }
            }
        }

        //----------------------------
        /// 刷新时间显示
        //----------------------------
		private void RefreshTimeDisplay()
		{
			if (m_timerText == null)
			{
				return;
			}

			if (m_timerDisplayType != enTimerDisplayType.None)
			{
                int displayTime = (int)m_currentTime;

                switch (m_timerDisplayType)
                {
                    case enTimerDisplayType.H_M_S:
                    {
                        int hour = displayTime / 3600;
                        displayTime -= hour * 3600;

                        int minute = displayTime / 60;
                        int second = displayTime - minute * 60;

                        m_timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
                    }                        
                    break;

                    case enTimerDisplayType.H_M:
                    {
                        int hour = displayTime / 3600;
                        displayTime -= hour * 3600;

                        int minute = displayTime / 60;

                        m_timerText.text = string.Format("{0:D2}:{1:D2}", hour, minute);
                    }
                    break;

                    case enTimerDisplayType.M_S:
                    {
                        int minute = displayTime / 60;
                        int second = displayTime - minute * 60;

                        m_timerText.text = string.Format("{0:D2}:{1:D2}", minute, second);
                    }
                    break;

                    case enTimerDisplayType.S:
                    {
                        m_timerText.text = string.Format("{0:D}", displayTime);
                    }
                    break;

                    case enTimerDisplayType.D_H_M_S:
                    {
                        int day = displayTime / 86400;
                        displayTime -= day * 86400;
                        int hour = displayTime / 3600;
                        displayTime -= hour * 3600;

                        int minute = displayTime / 60;
                        int second = displayTime - minute * 60;
                        m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}:{3:D2}", day, hour, minute, second);
                    }
                    break;

                    case enTimerDisplayType.D_H_M:
                    {
                        int day = displayTime / 86400;
                        displayTime -= day * 86400;
                        int hour = displayTime / 3600;
                        displayTime -= hour * 3600;

                        int minute = displayTime / 60;
                        m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}", day, hour, minute);
                    }
                    break;

                    case enTimerDisplayType.D:
                    {
                        int day = displayTime / 86400;
                        m_timerText.text = string.Format("{0}天", day);
                    }
                    break;
                } 
			}
		}

        //----------------------------
        /// 派发事件
        //----------------------------
        private void DispatchTimerEvent(enTimerEventType eventType)
        {
            if (m_eventIDs[(int)eventType] == enUIEventID.None)
            {
                return;
            }

            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_srcFormScript = m_belongedFormScript;
            uiEvent.m_srcWidget = this.gameObject;
            uiEvent.m_srcWidgetScript = this;
            uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
            uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
            uiEvent.m_pointerEventData = null;
            uiEvent.m_eventID = m_eventIDs[(int)eventType];
            uiEvent.m_eventParams = m_eventParams[(int)eventType];

            DispatchUIEvent(uiEvent);
        }

        //是否在运行
        public bool IsRunning()
        {
            return m_isRunning;
        }
    };
};