using UMI;

namespace UnityEngine.UI
{
    /// <summary>
    /// UI的基类
    /// </summary>
    public abstract class BaseUICtrl : BaseEventListener
    {
        protected string mUIName;               //UI名称，用于注册 
        protected UILayer mUILayer;             //UI 显示的层级
        protected string mUIAAName;             //UI的Addressable Name ,根据 mUIName 按照一定规则生成
        protected object mParam;                //UI显示时的传参

        protected GameObject mUIObj;            //UI的GameObject
        protected UIActiveTween mUITween;       //UI显示隐藏时的动画
        protected bool mInited = false;         //UI是否已经加载完成，初始化
        protected bool mNeetMask = false;       //是否需要遮罩
        protected bool mPreLoad = false;                //是否需要预加载
        protected bool mHideDestroyUI = false;          //是否在隐藏的直接销毁，用于不常用的UI 如：登录

        protected bool mIsShowing = false;

        public UILayer UIObjLayer
        {
            get => mUILayer;
            protected set
            {
                mUILayer = value;
            }
        }

        public bool NeetMask { get => mNeetMask; }
        public string UIName { get => mUIName; }
        public bool LoadedObj { get => mInited; }

        public BaseUICtrl(string uiName, UILayer uiLayer = UILayer.Frame, bool bNeetMask = false)
        {
            mUIName = uiName;
            mUILayer = uiLayer;
            mInited = false;
            mUIAAName = string.Empty;

            SetNeetMask(bNeetMask);
        }

        /// <summary>
        /// 设置是否需要遮罩
        /// </summary>
        /// <param name="bNeetMask"></param>
        protected virtual void SetNeetMask(bool bNeetMask = false)
        {
            if (UIObjLayer == UILayer.Frame || UIObjLayer == UILayer.Dialog_Bottom
                || UIObjLayer == UILayer.Dialog_Middle || UIObjLayer == UILayer.Dialog_Top)
            {
                mNeetMask = true;
            }
            else
            {
                mNeetMask = bNeetMask;
            }
        }

        #region UI资源的加载

        /// <summary>
        /// 获取UI的资源名称
        /// </summary>
        protected virtual void GetUIAAName()
        {
            if (string.IsNullOrEmpty(mUIAAName))
            {
                mUIAAName = "UI/" + mUIName + ConstInfo.Prefab_Suffix;//UIMgr.Instance.GetUIAAName(mUIName);
            }
        }

        /// <summary>
        /// 加载UI资源
        /// </summary>
        public virtual void LoadUIRes()
        {
            GetUIAAName();
            //ResMgr.Instance.LoadGoResAsync(mUIAAName, OnUILoadOVer);
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        public virtual void PreLoadUIRes()
        {
            if (mPreLoad)
            {
                GetUIAAName();
                //ResMgr.Instance.LoadGoResAsync(mUIAAName, OnUILoadOVer);
            }
        }

        protected virtual void OnPreloadOver(UnityEngine.Object obj, string aaName)
        {
            OnUILoadOVer(obj, aaName);

            if (mUIObj != null)
            {
                mUIObj.SetGoActive(false);
            }
        }

        /// <summary>
        /// UI加载完成
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnUILoadOVer(UnityEngine.Object obj, string aaName)
        {
            if (aaName != mUIAAName)
            {
                //GameObject.Destroy(mUIObj);
                //ResMgr.Instance.ReleaseObj(obj);
                mInited = false;
                mUIObj = null;
                Debug.LogErrorFormat("Load UI Error by mUIAAName = {0} != {1}", mUIAAName, aaName);
                HideUI();
                return;
            }

            mUIObj = obj as GameObject;

            RectTransform mUIRect = mUIObj.GetComponent<RectTransform>();
            Vector2 offsetMin = Vector2.zero;
            Vector2 offsetMax = Vector2.zero;

            if (mUIRect != null)
            {
                offsetMin = mUIRect.offsetMin;
                offsetMax = mUIRect.offsetMax;
            }

            mUIObj.SetGoParent(UIMgr.Instance.GetUILayer(mUILayer), false);
            mUIObj.SetGoLocalPos(Vector3.zero);
            mUIObj.SetGoLocalScale(Vector3.one);
            mUIObj.name = mUIName;

            if (mUIRect != null)
            {
                mUIRect.offsetMin = offsetMin;
                mUIRect.offsetMax = offsetMax;
            }
            ResetSibling();

            CreateView();
            mInited = true;
            InitComponent();

        }

        protected abstract void CreateView();

        /// <summary>
        /// 初始化脚本绑定
        /// </summary>
        protected virtual void InitComponent()
        {
            mUITween = mUIObj.GetComponent<UIActiveTween>();
            if (mUITween != null)
            {
                mUITween.SetTweenOver(OnUITweenMoveInOver, OnUITweenMoveOutOver);
            }

            if (!UIMgr.Instance.UIIsShowing(mUIName))
            {
                mUIObj.SetGoActive(false);
                return;
            }

            if (mUITween != null)
            {
                mUITween.MoveIn();
            }
            OnShow();
            mUIObj.SetGoActive(true);
        }

        /// <summary>
        /// 克隆界面下的 Item 
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual T CloneItem<T>(T obj) where T : BaseItemCtrl
        {
            if (obj == null)
            {
                return null;
            }

            return null;// ResMgr.Instance.InstantiateObj<T>(obj, obj.transform.parent);
        }

        #endregion

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual BaseUICtrl ShowUI(object param)
        {
            mParam = param;
            if(!mIsShowing)
            {
                RegEvents();
            }
            mIsShowing = true;
            if (!mInited)
            {
                LoadUIRes();
            }
            else
            {
                mUIObj.SetGoActive(true);
                ResetSibling();
                if (mUITween != null)
                {
                    mUITween.MoveIn();
                }
                OnShow();
            }

            return this;
        }

        protected virtual void OnShow()
        {

        }

        protected virtual void OnHide()
        {

        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public virtual void HideUI()
        {
            UIMgr.Instance.RemoveActiveUI(mUIName);
            mIsShowing = false;
            UnregEvents();

            if (mUITween != null)
            {
                mUITween.MoveOut();
            }
            else
            {
                OnUITweenMoveOutOver();
            }
        }

        protected virtual void DestoryUI()
        {
            mInited = false;
            //ResMgr.Instance.ReleaseObj(mUIObj);
            mUIObj = null;
            mUITween = null;
        }

        /// <summary>
        /// 界面的Update方法
        /// </summary>
        /// <param name="fDeltaTime"></param>
        public virtual void OnUpdateEx(float fDeltaTime)
        {

        }

        /// <summary>
        /// 重新设置 对应的层级
        /// </summary>
        public virtual void ResetSibling()
        {
            if (NeetMask)
            {
                UIMgr.Instance.SetUIMaskLayerLast(true, UIObjLayer);
            }
            mUIObj.transform.SetAsLastSibling();
        }

        protected virtual void OnUITweenMoveInOver()
        {

        }

        protected virtual void OnUITweenMoveOutOver()
        {
            if (mUIObj != null)
            {
                mUIObj.SetGoActive(false);
            }

            OnHide();

            if (mHideDestroyUI)
            {
                DestoryUI();
            }
        }


        /// <summary>
        /// 销毁UI
        /// </summary>
        public virtual void DestroyUI()
        {
            mInited = false;
            //ResMgr.Instance.ReleaseObj(mUIObj);
            mUITween = null;
            mUIObj = null;
            mUIAAName = string.Empty;
        }

        public virtual void SimpleClose()
        {
            UIMgr.Instance.HideUI(this.UIName);
        }
    }
}
