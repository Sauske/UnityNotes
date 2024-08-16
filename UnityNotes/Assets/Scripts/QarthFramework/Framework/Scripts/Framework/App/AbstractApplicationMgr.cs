//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public abstract class AbstractApplicationMgr<T> : TMonoSingleton<T> where T : TMonoSingleton<T>
    {
        private Action m_OnApplicationUpdate = null;
        private Action m_OnApplicationOnGUI = null;
        private Action<bool> m_OnApplicationPause = null;
        private Action<bool> m_OnApplicationFocus = null;
        private Action m_OnApplicationQuit = null;

        protected void Start()
        {
            StartCoroutine(StartApp());
        }

        private float m_LoadTime;
        protected IEnumerator StartApp()
        {
            m_LoadTime = Time.time;
            yield return InitFramework();
            Log.i("init framework time: " + (Time.time - m_LoadTime));
            m_LoadTime = Time.time;
            yield return InitThirdLibConfig();
            Log.i("int third lib time: " + (Time.time - m_LoadTime));
            m_LoadTime = Time.time;
            yield return InitAppEnvironment();
            Log.i("init app env time: " + (Time.time - m_LoadTime));
            m_LoadTime = Time.time;
            yield return StartGame();
        }

        #region 子类实现

        protected abstract void ShowLogoPanel();

        protected abstract IEnumerator InitThirdLibConfig();

        protected abstract IEnumerator InitAppEnvironment();

        protected abstract IEnumerator InitFramework();

        protected abstract IEnumerator StartGame();

        #endregion

        #region 注册事件
        
        public void AddListenerOnGUI(Action action)
        {
            m_OnApplicationOnGUI += action;
        }
        
        public void RemoveListenerOnGUI(Action action)
        {
            m_OnApplicationOnGUI -= action;
        }

        public void AddListenerOnApplicationUpdate(Action action)
        {
            m_OnApplicationUpdate += action;
        }
        
        public void RemoveListenerOnApplicationUpdate(Action action)
        {
            m_OnApplicationUpdate -= action;
        }
        
        public void AddListenerOnApplicationPause(Action<bool> action)
        {
            m_OnApplicationPause += action;
        }
        
        public void RemoveListenerOnApplicationPause(Action<bool> action)
        {
            m_OnApplicationPause -= action;
        }
        
        public void AddListenerOnApplicationFocus(Action<bool> action)
        {
            m_OnApplicationFocus += action;
        }

        public void RemoveListenerOnApplicationFocus(Action<bool> action)
        {
            m_OnApplicationFocus -= action;
        }
        
        public void AddListenerOnApplicationQuit(Action action)
        {
            m_OnApplicationQuit += action;
        }
        
        public void RemoveListenerOnApplicationQuit(Action action)
        {
            m_OnApplicationQuit -= action;
        }
        #endregion

        #region 生命周期函数

        void OnApplicationPause(bool pauseStatus)
        {
            if (m_OnApplicationPause != null)
            {
                m_OnApplicationFocus(pauseStatus);
            }
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (m_OnApplicationFocus != null)
            {
                m_OnApplicationFocus(focusStatus);
            }
        }

        void Update()
        {
            if (m_OnApplicationUpdate != null)
            {
                m_OnApplicationUpdate();
            }
        }

        private void OnDestroy()
        {
            isApplicationQuit = true;
            if (m_OnApplicationQuit != null)
            {
                m_OnApplicationQuit();
            }
            EnumEventSystem.S.Send(EngineEventID.OnApplicationQuit);
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            if (m_OnApplicationOnGUI != null)
            {
                m_OnApplicationOnGUI();
            }
        }
#endif

        #endregion
    }
}