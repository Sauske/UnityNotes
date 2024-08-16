﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using LitJson;
using System.IO;

namespace Qarth
{
    public abstract class IDataClass : DataDirtyHandler
    {
        protected DataDirtyRecorder m_DirtyRecorder = new DataDirtyRecorder();

        public IDataClass()
        {
            SetDirtyRecorder(m_DirtyRecorder);
        }
        
        //初始值
        public abstract void InitWithDefaultData();

        //每日是否需要更新存档数据
        public abstract void RefreshDataByDay();

        //存档加载结束后处理
        public abstract void OnDataLoadFinish();

    }

    public class DataDirtyRecorder
    {
        private bool m_IsDirty;
        public void SetIsDirty(bool dirty)
        {
            m_IsDirty = dirty;
        }

        public bool GetIsDirty()
        {
            return m_IsDirty;
        }
    }

    public class DataDirtyHandler
    {
        protected DataDirtyRecorder m_Recorder;

        public void SetDirtyRecorder(DataDirtyRecorder recorder)
        {
            m_Recorder = recorder;
        }

        public void SetDataDirty()
        {
            if (m_Recorder == null)
            {
                return;
            }

            m_Recorder.SetIsDirty(true);
        }

        public bool GetIsDataDirty()
        {
            if (m_Recorder == null)
            {
                return false;
            }

            return m_Recorder.GetIsDirty();
        }

        public void ResetDataDirty()
        {
            if (m_Recorder == null)
            {
                return;
            }
            m_Recorder.SetIsDirty(false);
        }
    }


    public class DataClassHandler<T> where T : IDataClass, new()
    {
        public static bool ENCRY = true;

        protected T m_Data;
        protected string m_FileName;
        protected bool m_AutoSave = false;
        protected int m_AutoSaveTimer;
        protected string m_FileNameKey;

        public DataClassHandler()
        {
            Load();
        }

        public T data
        {
            get { return m_Data; }
        }

        public void EnableAutoSave()
        {
            autoSave = true;
        }

        public void DisableAutoSave()
        {
            autoSave = false;
        }

        public void SetFileNameKey(string key)
        {
            m_FileNameKey = key;
        }

        public static string GetDataFilePathByType(Type t)
        {
            string fileName = t.FullName;

            if (ENCRY)
            {
                fileName = fileName.GetHashCode().ToString();
            }
            return string.Format("{0}{1}", persistentDataPath4Recorder, fileName);
        }

        public void Load()
        {
            if (m_Data != null)
            {
                return;
            }

            if (File.Exists(dataFilePath))
            {
                m_Data = SerializeHelper.DeserializeJson<T>(dataFilePath, ENCRY);
            }

            if (m_Data == null)
            {
                m_Data = new T();
                m_Data.InitWithDefaultData();
                m_Data.SetDataDirty();
            }

            try
            {
                m_Data.OnDataLoadFinish();
                return;
            }
            catch (Exception e)
            {
                Log.e(e);
            }

            m_Data = new T();
            m_Data.InitWithDefaultData();
            m_Data.SetDataDirty();

            m_Data.OnDataLoadFinish();
        }

        public void ResetAsNew()
        {
            m_Data = new T();
            m_Data.InitWithDefaultData();
            m_Data.SetDataDirty();

            m_Data.OnDataLoadFinish();
        }

        public void Save(bool force = false)
        {
            if (m_Data == null)
            {
                return;
            }

            if (m_Data.GetIsDataDirty() || force)
            {
                SerializeHelper.SerializeJson(dataFilePath, m_Data, ENCRY);
                m_Data.ResetDataDirty();
            }
        }

        protected string dataFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_FileName))
                {
                    string fileName = typeof(T).FullName;//.GetHashCode().ToString();
                    if (ENCRY)
                    {
                        fileName = fileName.GetHashCode().ToString();
                    }

                    if (string.IsNullOrEmpty(m_FileNameKey))
                    {
                        m_FileName = string.Format("{0}{1}", persistentDataPath4Recorder, fileName);
                    }
                    else
                    {
                        m_FileName = string.Format("{0}{1}{2}", persistentDataPath4Recorder, fileName, m_FileNameKey);
                    }
                }

                return m_FileName;
            }
        }

        private bool autoSave
        {
            get { return m_AutoSave; }
            set
            {
                if (m_AutoSave == value)
                {
                    return;
                }

                m_AutoSave = value;

                if (m_AutoSave)
                {
                    EnumEventSystem.S.Register(EngineEventID.OnAfterApplicationPauseChange, OnAppStateChecker);
                    EnumEventSystem.S.Register(EngineEventID.OnApplicationQuit, OnApplicationQuit);

                    if (m_AutoSaveTimer <= 0)
                    {
                        m_AutoSaveTimer = Timer.S.Post2Really(OnAutoSaveTimer, 180, -1);
                    }
                }
                else
                {
                    EnumEventSystem.S.UnRegister(EngineEventID.OnAfterApplicationPauseChange, OnAppStateChecker);
                    EnumEventSystem.S.UnRegister(EngineEventID.OnApplicationQuit, OnApplicationQuit);

                    if (m_AutoSaveTimer > 0)
                    {
                        Timer.S.Cancel(m_AutoSaveTimer);
                        m_AutoSaveTimer = -1;
                    }
                }
            }
        }

        protected void OnAutoSaveTimer(int count)
        {
            Save();
        }

        protected void OnAppStateChecker(int key, params object[] args)
        {
            bool pause = (bool)args[0];
            if (pause)
            {
                Save();
            }
        }

        protected void OnApplicationQuit(int key, params object[] args)
        {
            Save();
        }
 
        private static string m_PersistentDataPath4Recorder;
        // 外部资源目录
        public static string persistentDataPath4Recorder
        {
            get
            {
                if (null == m_PersistentDataPath4Recorder)
                {
                    m_PersistentDataPath4Recorder = FilePath.persistentDataPath + "/cache/";

                    if (!Directory.Exists(m_PersistentDataPath4Recorder))
                    {
                        Directory.CreateDirectory(m_PersistentDataPath4Recorder);
#if UNITY_IPHONE && !UNITY_EDITOR
                        UnityEngine.iOS.Device.SetNoBackupFlag(m_PersistentDataPath4Recorder);
#endif
                    }
                }

                return m_PersistentDataPath4Recorder;
            }
        }
    }
}
