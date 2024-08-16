using System;
using System.Collections.Generic;
using UnityEngine;


namespace Framework
{
    public class UIPageManager : Singleton<UIPageManager>
    {
        const int MAX_PAGES = 5;
        Dictionary<int, UIBasePage.PageCreator> mUIPageCreators = new Dictionary<int, UIBasePage.PageCreator>();

        //当前已经打开的界面
        List<UIBasePage> mPages = new List<UIBasePage>();

        List<UIBasePage> mRemovedPages = new List<UIBasePage>();

        List<UIBasePage> mHidePages = new List<UIBasePage>();

        GameObject mUIRoot;

        public GameObject UIRoot
        {
            get { return mUIRoot; }
        }

        /// <summary>
        /// 是否正在打开界面
        /// </summary>
        private bool mOpeningPage = false;
        public bool OpeningPage
        {
            get { return mOpeningPage; }
        }

        private UIBasePage mPreviousPage;
        public UIBasePage PreviousPage
        {
            get { return mPreviousPage; }
        }


        //当前打开的界面
        private UIBasePage mTopPage;
        public UIBasePage TopPage
        {
            get { return mTopPage; }
        }


        public override void Init()
        {
            EventRouter.GetInstance().AddEventHandler<int,object>(EventID.Open_Page,OnOpenPage);
            CreateUIRoot();
            RegisterPages();
        }


        public override void UnInit()
        {
            EventRouter.GetInstance().RemoveEventHandler<int, object>(EventID.Open_Page, OnOpenPage);
            mUIPageCreators.Clear();
            mPages.Clear();
            mRemovedPages.Clear();
            mHidePages.Clear();
            mOpeningPage = false;
            mTopPage = null;
            mPreviousPage = null;
        }

        void OnOpenPage(int pageId, object param)
        {
            OpenPage(pageId, param);
        }

        public void Update()
        {
            if(mRemovedPages.Count > 0)
            {
                for(int idx =0; idx < mRemovedPages.Count;idx++)
                {
                    DestroyPage(mRemovedPages[idx]);
                }
                mRemovedPages.Clear();
            }
        }

        public UIBasePage OpenDlg(int pageId,object param = null)
        {
            UIBasePage.PageCreator creator = null;
            if (!mUIPageCreators.TryGetValue(pageId, out creator))
            {
                DebugHelper.LogError("OpenPage Error: there is no page id: " + pageId.ToString());
                return null;
            }

            if (mOpeningPage)
            {
                DebugHelper.LogError("OpenPage Error: this is Opening Page");
                return null;
            }

            UIBasePage page = GetPage(pageId, true);
            if (page != null)
            {
                ShowPage(page);
                return page;
            }

            mOpeningPage = true;

            page = creator();
            if (!page.Open(param))
            {
                DestroyPage(page);
                return null;
            }

            mPages.Add(page);

            mOpeningPage = false;
            return page;
        }

        public void CloseDlg(int pageId)
        {
            for (int idx = 0; idx < mPages.Count; idx++)
            {
                if (mPages[idx].PageId == pageId)
                {
                    mRemovedPages.Add(mPages[idx]);
                    mPages.RemoveAt(idx);

                    return;
                }
            }

            for (int idx = 0; idx < mHidePages.Count; idx++)
            {
                if (mHidePages[idx].PageId == pageId)
                {
                    mRemovedPages.Add(mHidePages[idx]);
                    mHidePages.RemoveAt(idx);
                    return;
                }
            }
        }

        public UIBasePage OpenPage(int pageId,object param = null,bool overlay = false)
        {
            UIBasePage.PageCreator creator = null;
            if (!mUIPageCreators.TryGetValue(pageId, out creator))
            {
                DebugHelper.LogError("OpenPage Error: there is no page id: "+pageId.ToString());
                return null;
            }

            if (mOpeningPage)
            {
                DebugHelper.LogError("OpenPage Error: this is Opening Page");
                return null;
            }

            UIBasePage page = GetPage(pageId,true);
            if (page != null)
            {
                mPreviousPage = mTopPage;
                mTopPage = page;

                if (!overlay)
                    HidePage(mPreviousPage);

                ShowPage(page);

                return page;
            }

            mOpeningPage = true;

            //界面打开个数超过最大限制,移除最早打开的界面
            if(mPages.Count >= MAX_PAGES)
            {
                UIBasePage removePage = mPages[0];
                mPages.RemoveAt(0);
                removePage.Close();
            }

            page = creator();
            if(!page.Open(param))
            {
                DestroyPage(page);
                return null;
            }


            mPreviousPage = mTopPage;
            mTopPage = page;
            if (!overlay)
                HidePage(mPreviousPage);

            mPages.Add(page);

            mOpeningPage = false;
            return page;
        }

        public bool ShowPage(int pageId)
        {
            UIBasePage page = GetPage(pageId);
            return ShowPage(page);
        }

        public bool ShowPage(UIBasePage page)
        {
            if (page == null) return false;

            if (!page.IsOpened) return false;

            if (page.IsClosed) return false;

            if (page.IsShowed) return true;

            page.Show();
            AddPageToShowList(page);
            RemovePageFromHideList(page);
            return true;
        }

        public bool HidePage(int pageId)
        {
            UIBasePage page = GetPage(pageId);
            return HidePage(page);
        }

        public bool HidePage(UIBasePage page)
        {
            if (page == null) return false;

            if (!page.IsOpened) return false;

            if (page.IsClosed) return false;

            if (page.IsHide) return true;

            page.Hide();
            AddPageToHideList(page);
            RemovePageFromShowList(page);
            return true;
        }

        public UIBasePage GetPage(int pageId,bool all=false)
        {
            for(int idx=0;idx < mPages.Count;idx++)
            {
                if(mPages[idx].PageId == pageId)
                {
                    return mPages[idx];
                }
            }

            if(all)
            {
               for(int idx =0; idx < mRemovedPages.Count;idx++)
               {
                   if(mRemovedPages[idx].PageId == pageId)
                   {
                       return mRemovedPages[idx];
                   }
               }

                for(int idx =0; idx < mHidePages.Count;idx++)
                {
                    if(mHidePages[idx].PageId == pageId)
                    {
                        return mHidePages[idx];
                    }
                }
            }
            return null;
        }

        public void CloseAllPage(int[] exceptPageIds = null)
        {
            if (exceptPageIds != null && exceptPageIds.Length > 0)
            {
                List<int> removeIds = new List<int>();
                for (int idx = 0; idx < mPages.Count; idx++)
                {
                    UIBasePage page = mPages[idx];
                    bool find = false;
                    for (int jdx = 0; jdx < exceptPageIds.Length; jdx++)
                    {
                        if (page.PageId == exceptPageIds[jdx])
                        {
                            find = true;
                            break;
                        }
                    }

                    if (find) continue;

                    removeIds.Add(page.PageId);
                }

                for (int idx = 0; idx < removeIds.Count; idx++)
                    ClosePage(removeIds[idx]);

                mRemovedPages.AddRange(mHidePages);
                mHidePages.Clear();
            }else
            {
                mRemovedPages.AddRange(mPages);
                mRemovedPages.AddRange(mHidePages);
                mHidePages.Clear();
                mPages.Clear();
            }

        }

        public void ClosePage(int pageId)
        {
            for(int idx =0; idx < mPages.Count;idx++)
            {
                if(mPages[idx].PageId == pageId)
                {
                    mRemovedPages.Add(mPages[idx]);
                    mPages.RemoveAt(idx);

                    if (mTopPage != null && mTopPage.PageId == pageId)
                    {
                        mTopPage = mPreviousPage;
                        ShowPage(mTopPage);

                        mPreviousPage = GetLatelyHidePage();
                        
                    }

                    return;
                }
            }

            for(int idx =0; idx < mHidePages.Count;idx++)
            {
                if(mHidePages[idx].PageId == pageId)
                {
                    mRemovedPages.Add(mHidePages[idx]);
                    mHidePages.RemoveAt(idx);
                    return;
                }
            }
        }

        public void ClosePage(UIBasePage page)
        {
            if (page == null) return;

            ClosePage(page.PageId);
        }

        private void DestroyPage(UIBasePage page)
        {
            if (page == null) return;

            page.Close();
            GameObject.Destroy(page);
            page = null;
        }

        private void CreateUIRoot()
        {
            mUIRoot = new GameObject("RUIManager");

            GameObject bootObj = GameObject.Find("BootObj");
            if (bootObj != null)
                mUIRoot.transform.parent = bootObj.transform;
        }

        private void RegisterPages()
        {
            //RegisterPage(UIPageIDs.PAGE_ID_LOGIN, UILoginPage.Creator);
            //RegisterPage(UIPageIDs.PAGE_ID_MATCH, UIMatchPage.Creator);
            //RegisterPage(UIPageIDs.PAGE_ID_FRIEND, UIFriendPage.Creator);
        }

        private void RegisterPage(int pageId, UIBasePage.PageCreator creator)
        {
            //! Already register
            if (mUIPageCreators.ContainsKey(pageId))
                return;

            mUIPageCreators.Add(pageId, creator);
        }
    
        /// <summary>
        /// 获取最近隐藏的界面
        /// </summary>
        /// <returns></returns>
        private UIBasePage GetLatelyHidePage()
        {
            if (mHidePages.Count == 0) return null;

            return mHidePages[mHidePages.Count - 1];
        }

        private void AddPageToShowList(UIBasePage page)
        {
            if (page == null || HasPageInShowList(page.PageId)) return;

            mPages.Add(page);
        }

        private void RemovePageFromShowList(UIBasePage page)
        {
            if (page == null || !HasPageInShowList(page.PageId)) return;

            mPages.Remove(page);
        }

        private void AddPageToHideList(UIBasePage page)
        {
            if (page == null || HasPageInHideList(page.PageId)) return;

            mHidePages.Add(page);
        }

        private void RemovePageFromHideList(UIBasePage page)
        {
            if (page == null || !HasPageInHideList(page.PageId)) return;

            mHidePages.Remove(page);
        }

        private bool HasPageInShowList(UIBasePage page)
        {
            if (page == null) return false;

            return HasPageInShowList(page.PageId);
        }

        public bool HasPageInShowList(int pageId)
        {
            for(int idx =0; idx < mPages.Count;idx++)
            {
                if (mPages[idx].PageId == pageId)
                    return true;
            }

            return false;
        }

        private bool HasPageInHideList(UIBasePage page)
        {
            if (page == null) return false;

            return HasPageInHideList(page.PageId);
        }

        public bool HasPageInHideList(int pageId)
        {
            for (int idx = 0; idx < mHidePages.Count; idx++)
            {
                if (mHidePages[idx].PageId == pageId)
                    return true;
            }

            return false;
        }

        public void OpenTips(string strContent, bool bReadDatabin = false, float timeDuration = 1.5f, GameObject referenceGameObject = null, params object[] replaceArr)
        {
          //  CUIManager.GetInstance().OpenTips(strContent, bReadDatabin, timeDuration, referenceGameObject, replaceArr);
        }
    }
}
