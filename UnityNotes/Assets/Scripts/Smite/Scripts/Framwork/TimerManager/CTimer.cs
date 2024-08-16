//====================================
/// Timer
/// @neoyang
/// @2015.03.18
//====================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//计时器参数
public struct stTimerParams
{
    public UInt64 commonUInt64;
}

public class CTimer
{
    //delegate
    public delegate void OnTimeUpHandler(int timerSequence);
    private OnTimeUpHandler m_timeUpHandler;

    //delegate with custom params
    public delegate void OnTimeUpWithParamsHandler(int timerSequence, stTimerParams timerParams);
    private OnTimeUpWithParamsHandler m_timeUpWithParamsHandler;

    //循环次数( < 0 表示无限循环)
    private int m_loop = 1;

    //计时(ms)
    private int m_totalTime;
    private int m_currentTime;

    //是否完成
    private bool m_isFinished;

    //是否处于运行状态
    private bool m_isRunning;

    //序列号
    private int m_sequence;

    public int CurrentTime
    {
        get { return m_currentTime; }
    }

    //计时器参数
    private stTimerParams m_timerParams;

    //--------------------------------------
    /// 构造函数
    //--------------------------------------
    public CTimer(int time, int loop, OnTimeUpHandler timeUpHandler, int sequence)
    {
        if (loop == 0)
        {
            loop = -1;
        }

        m_totalTime = time;
        m_loop = loop;
        m_timeUpHandler = timeUpHandler;
        m_sequence = sequence;

        m_currentTime = 0;
        m_isRunning = true;
        m_isFinished = false;
    }

    public CTimer(int time, int loop, OnTimeUpWithParamsHandler timeUpWithParamsHandler, int sequence, stTimerParams timerParams)
    {
        if (loop == 0)
        {
            loop = -1;
        }

        m_totalTime = time;
        m_loop = loop;
        m_timeUpWithParamsHandler = timeUpWithParamsHandler;
        m_sequence = sequence;
        m_timerParams = timerParams;

        m_currentTime = 0;
        m_isRunning = true;
        m_isFinished = false;
    }

    //--------------------------------------
    /// Update
    /// @deltaTime
    //--------------------------------------
    public void Update(int deltaTime)
    {
        if (m_isFinished || !m_isRunning)
        {
            return;
        }

        if (m_loop == 0)
        {
            m_isFinished = true;
        }
        else
        {
            m_currentTime += deltaTime;

            if (m_currentTime >= m_totalTime)
            {
                if (m_timeUpHandler != null)
                {
                    m_timeUpHandler(m_sequence);
                }
                if (m_timeUpWithParamsHandler != null)
                {
                    m_timeUpWithParamsHandler(m_sequence, m_timerParams);
                }

                m_currentTime = 0;
                m_loop--;
            }
        }
    }

    //--------------------------------------
    /// 还差多少时间
    //--------------------------------------
    public int GetLeftTime()
    {
        return m_totalTime - m_currentTime;
    }

    //--------------------------------------
    /// 结束Timer
    //--------------------------------------
    public void Finish()
    {
        m_isFinished = true;
    }

    //--------------------------------------
    /// 是否完成
    //--------------------------------------
    public bool IsFinished()
    {
        return m_isFinished;
    }

    //--------------------------------------
    /// 暂停
    //--------------------------------------
    public void Pause()
    {
        m_isRunning = false;
    }

    //--------------------------------------
    /// 恢复
    //--------------------------------------
    public void Resume()
    {
        m_isRunning = true;
    }

    //--------------------------------------
    /// 重置
    //--------------------------------------
    public void Reset()
    {
        m_currentTime = 0;
    }

    //--------------------------------------
    /// 重设倒计时
    //--------------------------------------
    public void ResetTotalTime(int totalTime)
    {
        if (m_totalTime == totalTime) return;
        m_currentTime = 0;
        m_totalTime = totalTime;
    }

    //--------------------------------------
    /// 检查sequence是否匹配
    //--------------------------------------
    public bool IsSequenceMatched(int sequence)
    {
        return (m_sequence == sequence);
    }

    //--------------------------------------
    /// 检查delegate是否匹配
    //--------------------------------------
    public bool IsDelegateMatched(OnTimeUpHandler timeUpHandler)
    {
        return (m_timeUpHandler == timeUpHandler);
    }

    public bool IsDelegateWithParamsMatched(OnTimeUpWithParamsHandler timeUpWithParamsHandler)
    {
        return (m_timeUpWithParamsHandler == timeUpWithParamsHandler);
    }

    //--------------------------------------
    /// 获取计时器参数
    //--------------------------------------
    public stTimerParams GetTimerParams()
    {
        return m_timerParams;
    }
};