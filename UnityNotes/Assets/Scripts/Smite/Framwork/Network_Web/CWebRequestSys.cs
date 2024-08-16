using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Framework
{

    public class CWebRequestSys : Singleton<CWebRequestSys>
    {
        //下载栈
        private Stack<CLoadReq> mWebReqQueues = new Stack<CLoadReq>();
        //目前一次只执行一个任务，以后若有需要可增加
        private CLoadReq mCurLoadReq;

        public void SysUpdate()
        {
            UpdateLoadTask();
        }

        public void AddLoadRequest(CLoadReq loadReq)
        {
            mWebReqQueues.Push(loadReq);
        }

        public void UpdateLoadTask()
        {
            if (mCurLoadReq == null)
            {
                if (mWebReqQueues.Count > 0)
                {
                    mCurLoadReq = mWebReqQueues.Pop();
                    mCurLoadReq.StartDownload();
                }
            }
            else
            {
                mCurLoadReq.UpdateLoadState();
                switch (mCurLoadReq.mLoadResultType)
                {
                    case ELoadResultType.ELoadSucess:
                        mCurLoadReq.LoadDone();
                        mCurLoadReq = null;
                        break;
                    case ELoadResultType.ELoadNetError:
                    case ELoadResultType.ELoadTimeOut:
                        {
                            if (mCurLoadReq.mRetryTimes > 0)
                            {
                                mCurLoadReq.mRetryTimes--;
                                mWebReqQueues.Push(mCurLoadReq);
                            }
                            else
                            {
                                mCurLoadReq.LoadDone();
                                //Debug.LogError(string.Format("File:{0}Load failed", mCurLoadReq.mUrl));
                            }
                            mCurLoadReq = null;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddLoadRequestList(CLoadRequestList list)
        {
            for (int i = 0; i < list.mLoadReqList.Count; i++)
            {
                mWebReqQueues.Push(list.mLoadReqList[i]);
            }
        }
    }

    public delegate void DLoadReqCallBack(CLoadReq loadReq);

    public enum ELoadResultType
    {
        None,
        ELoadNetError,
        ELoadTimeOut,
        ELoadSucess,
    }

    public class CLoadReq
    {
        public WWW mWWW;
        public string mUrl;
        public string mCachePath;
        public string mPath;
        public float mProgress;
        public int mFileSize;
        public CLoadRequestList mLoadReqList;
        public DLoadReqCallBack mCallBack;
        public ELoadResultType mLoadResultType = ELoadResultType.None;
        public int mRetryTimes = 3;
        public bool mIsLoadDone = false;
        private float mStartTime = 0;
        private float mTimeOut = 30.0f;


        public static CLoadReq LoadWebResource(string path, int filesize, DLoadReqCallBack callback)
        {
            CLoadReq loadReq = new CLoadReq();
            loadReq.mCachePath = string.Format("{0}/{1}", Application.persistentDataPath, path);
            loadReq.mUrl = "";// CResourceSys.Instance.GetWebPathWithoutVersion(path);
            loadReq.mFileSize = filesize;
            loadReq.mCallBack = callback;
            loadReq.mPath = path;
            return loadReq;
        }

        private void CacheFile()
        {
            try
            {
                if (mWWW == null || mWWW.bytes == null || string.IsNullOrEmpty(mCachePath)) return;

                string dir = Path.GetDirectoryName(mCachePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllBytes(mCachePath, mWWW.bytes);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        public void LoadDone()
        {
            mIsLoadDone = true;
            if (mLoadResultType == ELoadResultType.ELoadSucess)
            {
                CacheFile();
            }

            if (mCallBack != null)
            {
                mCallBack(this);
            }

            if (mWWW != null)
            {
                mWWW.Dispose();
                mWWW = null;
            }

            if (mLoadReqList != null)
            {
                mLoadReqList.NotifyLoadDone(this);
            }
        }

        public void StartDownload()
        {
            if (mWWW == null)
            {
                mWWW = new WWW(mUrl);
                mStartTime = Time.realtimeSinceStartup;
            }
        }

        public void UpdateLoadState()
        {
            if (mWWW == null) return;

            mProgress = mWWW.progress;
            if (!mWWW.isDone)
            {
                if (Time.realtimeSinceStartup - mStartTime >= mTimeOut)
                {
                    mLoadResultType = ELoadResultType.ELoadTimeOut;
                }
            }
            else if (!string.IsNullOrEmpty(mWWW.error))
            {
                mLoadResultType = ELoadResultType.ELoadNetError;
            }
            else
            {
                mLoadResultType = ELoadResultType.ELoadSucess;
            }
        }

        public void Stop()
        {
            if (mWWW != null)
            {
                mWWW.Dispose();
                mWWW = null;
            }
        }
    }

    public delegate void DLoadRequestListCallBack(CLoadRequestList loadList);
    public class CLoadRequestList
    {
        public DLoadRequestListCallBack mCallBack;
        public List<CLoadReq> mLoadReqList = new List<CLoadReq>();
        public bool AllDone = false;
        public bool HaveError = false;
        public void AddRequest(CLoadReq LoadReq)
        {
            mLoadReqList.Add(LoadReq);
            LoadReq.mLoadReqList = this;
        }

        public void NotifyLoadDone(CLoadReq loadReq)
        {
            if (loadReq.mLoadResultType != ELoadResultType.ELoadSucess)
            {
                HaveError = true;
            }
            UpdateDoneCnt();
        }

        public void UpdateDoneCnt()
        {
            int nDoneCnt = 0;
            for (int i = 0; i < mLoadReqList.Count; i++)
            {
                if (mLoadReqList[i].mIsLoadDone)
                {
                    nDoneCnt++;
                }
            }

            if (nDoneCnt == mLoadReqList.Count)
            {
                AllDone = true;
                if (mCallBack != null)
                {
                    mCallBack(this);
                }
            }
        }
    }
}