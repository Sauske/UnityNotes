using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UMI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIMgr : Singleton<UIMgr>, IUpdate
    {
        public static int UI_SCREEN_WIDTH = 1334;           //屏幕宽
        public static int UI_SCREEN_HEIGHT = 750;           //屏幕宽
        public static int UI_CANVAS_DISTANCE = 300;         //UI上画布的距离
        public static int UI_CANVAS_ORDER_DIS = 100;        //ui画布层级差


        /// <summary>
        /// 尽量不要在里面添加过多的规则，非必要不要修改
        /// 打破UI规则的，原有规则是 Frame <= Layer <= Dialog_top 中的一个层级只显示一个界面
        /// 这个是一个层级里面是可以有多个界面同时存在的
        /// </summary>
        /// <returns></returns>
        private readonly List<List<string>> mBreakUIRuleLst = new List<List<string>>(){
            new List<string> { UITypeName.UIPersonalCenter,UITypeName.UIFriendManagement},
        };

        /// <summary>
        /// 已经注册的UI
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="BaseUICtrl"></typeparam>
        /// <returns></returns>
        private Dictionary<string, BaseUICtrl> mUIDic = new Dictionary<string, BaseUICtrl>();

        /// <summary>
        /// 当前正在显示的UI
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="BaseUICtrl"></typeparam>
        /// <returns></returns>
        private Dictionary<string, BaseUICtrl> mActiveUIDic = new Dictionary<string, BaseUICtrl>();
        private List<BaseUICtrl> mActiveUILst = new List<BaseUICtrl>();

        /// <summary>
        /// 关闭所有的UI时，忽略的UI
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        private List<string> mIgnoreUILst = new List<string>() { };

        /// <summary>
        /// 带有遮罩的UI显示顺序
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        private List<string> mUIMaskLst = new List<string>(5);

        /// <summary>
        /// UI的分层
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <typeparam name="GameObject"></typeparam>
        /// <returns></returns>
        private Dictionary<int, GameObject> mLayerRoots = new Dictionary<int, GameObject>();

        /// <summary>
        /// 覆盖的UI缓存栈
        /// </summary>
        /// <returns></returns>
        private Stack mRecoverStack = new Stack();


        public GameObject UIRootObj { get; private set; }
        public GameObject UIMaskObj { get; private set; }
        public Camera UICamera { get; private set; }
        public CanvasScaler UICanvasScaler { get; private set; }


        public async Task Init()
        {
            mUIDic.Clear();

            //var obj = await ResMgr.Instance.LoadGoRes(ConstInfo.UIRootAAName);
            //if (obj == null)
            //{
            //    return;
            //}
            //LoadUIRootOver(obj, ConstInfo.UIRootAAName);

            //obj = await ResMgr.Instance.LoadGoRes(ConstInfo.UIMaskAAName);
            //if (obj == null)
            //{
            //    return;
            //}
            //LoadUIMaskOver(obj, ConstInfo.UIMaskAAName);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 注册运行的基础UI ，不在热更新里面的
        /// </summary>
        private void RegisterBaseUI()
        {
            Type winType = typeof(UITypeName);
            var properties = winType.GetFields();
            foreach (System.Reflection.FieldInfo info in properties)
            {
                //UMILogger.Log($"Start RegisterWindow uiName = {info.Name}", LogTagConfig.UILogTag);

                try
                {
                    string className = info.Name;
                    if (!className.StartsWith(ConstInfo.UICtrlStart))
                    {
                        className = ConstInfo.UICtrlStart + className;
                    }
                    string ctrlClass = $"UMI.{className}Ctrl";
                    Type type = Type.GetType(ctrlClass);
                    BaseUICtrl uiCtrl = (BaseUICtrl)System.Activator.CreateInstance(type);
                    RegisterWindow(info.Name, uiCtrl);
                    uiCtrl.PreLoadUIRes();

                    //UMILogger.Log($"RegisterWindow over uiName = {info.Name}", LogTagConfig.UILogTag);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("RegisterWindow error uiName = {0} , error = {1} \n {2}", info.Name, ex.ToString(), ex.StackTrace);
                }
            }
        }

        #region Load Res

        /// <summary>
        /// UIRoot加载完成后的初始化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="aaName"></param>
        private void LoadUIRootOver(UnityEngine.Object obj, string aaName)
        {
            if (aaName != ConstInfo.UIRootAAName)
            {
                Debug.LogErrorFormat("UIMgr Init Error by UIRootAAName = {0} != {1}", ConstInfo.UIRootAAName, aaName);
                //ResMgr.Instance.ReleaseObj(obj);
                return;
            }

            UIRootObj = obj as GameObject;
            if (UIRootObj != null)
            {
                UIRootObj.name = "UIRoot";
                UIRootObj.SetGoActive(true);
                GameObject.DontDestroyOnLoad(UIRootObj);
                UIRootObj.SetGoLocalPos(Vector3.up * 500);
                UIRootObj.SetGoLocalScale(Vector3.one);
            }

            UICanvasScaler = UIRootObj.GetComponent<CanvasScaler>();

            UICamera = UIRootObj.FindObj<Camera>("UICamera");
            //GameMain.Instance.AddCameraStack(UICamera);
            UICamera.nearClipPlane = -10;
            UICamera.farClipPlane = 300;
            CreateUILayer();

            RegisterBaseUI();
        }

        /// <summary>
        /// UIRoot加载完成后的初始化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="aaName"></param>
        private void LoadUIMaskOver(UnityEngine.Object obj, string aaName)
        {
            if (aaName != ConstInfo.UIMaskAAName)
            {
                Debug.LogErrorFormat("UIMgr Init Error by UIMaskAAName = {0} != {1}", ConstInfo.UIMaskAAName, aaName);
                //ResMgr.Instance.ReleaseObj(obj);
                return;
            }

            UIMaskObj = obj as GameObject;
            UIMaskObj.name = "UIMask";
            UIMaskObj.SetGoActive(false);
            UIMaskObj.SetGoParent(mLayerRoots[(int)UILayer.Frame]);
            UIMaskObj.SetGoLocalPos(Vector3.zero);
            UIMaskObj.SetGoLocalScale(Vector3.one);

            RectTransform uiMaskRect = UIMaskObj.GetComponent<RectTransform>();
            if (uiMaskRect != null)
            {
                uiMaskRect.offsetMin = Vector2.zero;
                uiMaskRect.offsetMax = Vector2.zero;
            }
        }

        private void CreateUILayer()
        {
            for (UILayer layer = UILayer.Bottom; layer < UILayer.Max; ++layer)
            {
                mLayerRoots[(int)layer] = UITools.CreateUIObject(layer.ToString(), UIRootObj);
            }

            Canvas canvas;
            for (int i = (int)UILayer.ActorHP; i < (int)UILayer.Max; i++)
            {
                if (mLayerRoots[i] != null)
                {
                    canvas = mLayerRoots[i].GetComponent<Canvas>();
                    if (canvas == null)
                    {
                        canvas = mLayerRoots[i].AddComponent<Canvas>();
                    }
                    if (mLayerRoots[i].GetComponent<GraphicRaycaster>() == null)
                    {
                        mLayerRoots[i].AddComponent<GraphicRaycaster>();
                    }

                    canvas.overrideSorting = true;
                    canvas.planeDistance = i * UI_CANVAS_DISTANCE;
                    canvas.transform.localPosition = new Vector3(0, 0, canvas.planeDistance);
                    canvas.sortingOrder = (i - 1) * UI_CANVAS_ORDER_DIS + 1;
                }
            }
        }

        #endregion

        #region Interface 

        public void FreeMemory() { }

        public void Dispose()
        {
            mUIDic.Clear();
            mActiveUIDic.Clear();
            mLayerRoots.Clear();
            mRecoverStack.Clear();

            //ResMgr.Instance.ReleaseObj(UIRootObj);
            //ResMgr.Instance.ReleaseObj(UIMaskObj);

            UIRootObj = null;
            UIMaskObj = null;
            UICamera = null;
            UICanvasScaler = null;
        }

        public void OnUpdateEx()
        {
            OnUpdateEx(Time.deltaTime);
        }

        public void OnUpdateEx(float fDeltaTime)
        {
#if UNITY_EDITOR 
            UpdateSceneResolution();
#endif

            mActiveUILst.Clear();
            mActiveUILst.AddRange(mActiveUIDic.Values);
            foreach (var ui in mActiveUILst)
            {
                if (ui != null && ui.LoadedObj)
                {
                    ui.OnUpdateEx(fDeltaTime);
                }
            }
        }

        #endregion


        #region 分辨率设置

        /// <summary>
        /// 切换屏幕的缩放
        /// 界面设计大多数都是横屏的，竖屏相对少
        /// </summary>
        /// <param name="isPortrait">是否是竖屏</param>
        public void SwitchCanvasScaler(bool isPortrait)
        {
            if (isPortrait)
            {
                // 切换到竖屏
                UICanvasScaler.referenceResolution = new Vector2(UI_SCREEN_HEIGHT, UI_SCREEN_WIDTH);
                UICanvasScaler.matchWidthOrHeight = 0;
            }
            else
            {
                // 切换到横屏
                UICanvasScaler.referenceResolution = new Vector2(UI_SCREEN_WIDTH, UI_SCREEN_HEIGHT);
                UICanvasScaler.matchWidthOrHeight = 1;
            }
        }

        /// <summary>
        /// 更新分辨率 编辑器和PC上
        /// </summary>
        private void UpdateSceneResolution()
        {
            if (UICanvasScaler == null)
            {
                return;
            }

            int iWidth = UI_SCREEN_WIDTH;
            int iHeight = UI_SCREEN_HEIGHT;
            float aspect = (float)Screen.width / (float)Screen.height;
            if (Screen.width > Screen.height)       //横屏
            {
                if ((float)UI_SCREEN_WIDTH / (float)UI_SCREEN_HEIGHT > aspect)
                {
                    iWidth = UI_SCREEN_WIDTH;
                    iHeight = Screen.height * UI_SCREEN_WIDTH / Screen.width;
                    //UICanvasScaler.matchWidthOrHeight = 0;
                }
                else
                {
                    iHeight = UI_SCREEN_HEIGHT;
                    iWidth = Screen.width * UI_SCREEN_HEIGHT / Screen.height;
                    //UICanvasScaler.matchWidthOrHeight = 1;
                }
            }
            else        //竖屏
            {
                if ((float)UI_SCREEN_HEIGHT / (float)UI_SCREEN_WIDTH > aspect)
                {
                    iWidth = UI_SCREEN_HEIGHT;
                    iHeight = Screen.height * UI_SCREEN_HEIGHT / Screen.width;
                    //UICanvasScaler.matchWidthOrHeight = 0;
                }
                else
                {
                    iHeight = UI_SCREEN_WIDTH;
                    iWidth = Screen.width * UI_SCREEN_WIDTH / Screen.height;
                    //UICanvasScaler.matchWidthOrHeight = 1;
                }
            }

            Screen.SetResolution(iWidth, iHeight, true);
            UICanvasScaler.referenceResolution = new Vector2(iWidth, iHeight);
        }

        private void SetPhoneResolution(bool isPortrait)
        {
            if (UICanvasScaler == null)
            {
                return;
            }

            int iWidth = UI_SCREEN_WIDTH;
            int iHeight = UI_SCREEN_HEIGHT;
            float aspect = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
            if (isPortrait)
            {
                if ((float)UI_SCREEN_HEIGHT / (float)UI_SCREEN_WIDTH > aspect)
                {
                    iWidth = UI_SCREEN_HEIGHT;
                    iHeight = Screen.currentResolution.height * UI_SCREEN_HEIGHT / Screen.currentResolution.width;
                    UICanvasScaler.matchWidthOrHeight = 0;
                }
                else
                {
                    iHeight = UI_SCREEN_WIDTH;
                    iWidth = Screen.currentResolution.width * UI_SCREEN_WIDTH / Screen.currentResolution.height;
                    UICanvasScaler.matchWidthOrHeight = 1;
                }
            }
            else
            {
                if ((float)UI_SCREEN_WIDTH / (float)UI_SCREEN_HEIGHT > aspect)
                {
                    iWidth = UI_SCREEN_WIDTH;
                    iHeight = Screen.currentResolution.height * UI_SCREEN_WIDTH / Screen.currentResolution.width;
                    UICanvasScaler.matchWidthOrHeight = 0;
                }
                else
                {
                    iHeight = UI_SCREEN_HEIGHT;
                    iWidth = Screen.currentResolution.width * UI_SCREEN_HEIGHT / Screen.currentResolution.height;
                    UICanvasScaler.matchWidthOrHeight = 1;
                }
            }

            Screen.SetResolution(iWidth, iHeight, true);
            UICanvasScaler.referenceResolution = new Vector2(iWidth, iHeight);
        }

        public Vector2 GetUIScreenSize()
        {
            if (UICanvasScaler != null)
            {
                return UICanvasScaler.referenceResolution;
            }
            return new Vector2(UI_SCREEN_WIDTH, UI_SCREEN_HEIGHT);
        }

        public Vector2 GetUIScreenRealSize()
        {
            if (UIRootObj == null)
            {
                return new Vector2(UI_SCREEN_WIDTH, UI_SCREEN_HEIGHT);
            }
            return UIRootObj.GetComponent<RectTransform>().sizeDelta;
        }

        #endregion

        /// <summary>
        /// 注册UI
        /// </summary>
        public void RegisterWindow(string uiName, BaseUICtrl baseUI)
        {
            if (!mUIDic.ContainsKey(uiName))
            {
                mUIDic.Add(uiName, baseUI);
                Debug.LogFormat("RegisterWindow uiName = {0} UICtrl = {1}", uiName, baseUI.GetType());
            }
        }

        /// <summary>
        /// 设置UI的层级
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uILayer"></param>
        public void SetUIObjLayer(GameObject obj, UILayer uiLayer)
        {
            if (obj != null)
            {
                obj.SetGoParent(mLayerRoots[(int)uiLayer]);
            }
        }

        /// <summary>
        /// 获取UI层级的GameObject
        /// </summary>
        /// <param name="uilayer"></param>
        /// <returns></returns>
        public GameObject GetUILayer(UILayer uilayer)
        {
            if (mLayerRoots.ContainsKey((int)uilayer))
            {
                return mLayerRoots[(int)uilayer];
            }
            return null;
        }

        /// <summary>
        /// 设置统一的遮罩的位置
        /// </summary>
        /// <param name="bActive"></param>
        /// <param name="uilayer"></param>
        /// <param name="orderIndex"></param>
        public void SetUIMaskLayer(bool bActive, UILayer uilayer, int orderIndex)
        {
            if (UIMaskObj != null)
            {
                if (!bActive)
                {
                    UIMaskObj.SetGoActive(bActive);
                    return;
                }

                UIMaskObj.SetGoParent(mLayerRoots[(int)uilayer]);
                UIMaskObj.SetGoActive(bActive);
                UIMaskObj.transform.SetSiblingIndex(orderIndex);
            }
        }

        /// <summary>
        /// 设置统一的遮罩的位置
        /// </summary>
        /// <param name="bActive"></param>
        /// <param name="uilayer"></param>
        public void SetUIMaskLayerLast(bool bActive, UILayer uilayer)
        {
            if (UIMaskObj != null)
            {
                if (!bActive)
                {
                    UIMaskObj.SetGoActive(bActive);
                    return;
                }

                UIMaskObj.SetGoParent(mLayerRoots[(int)uilayer]);
                UIMaskObj.SetGoActive(bActive);
                UIMaskObj.transform.SetAsFirstSibling();
            }
        }

        #region 界面显示隐藏

        /// <summary>
        /// 获取正在显示的UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetActiveUI<T>(string uiName) where T : BaseUICtrl
        {
            if (mActiveUIDic.ContainsKey(uiName))
            {
                return mActiveUIDic[uiName] as T;
            }
            return null;
        }


        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUI<T>(string uiName) where T : BaseUICtrl
        {
            if (mUIDic.ContainsKey(uiName))
            {
                return mUIDic[uiName] as T;
            }
            return null;
        }

        /// <summary>
        /// 显示界面 这个一般是在同一个层级的界面调用，或者由下级界面调度上级界面时
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="recoverName"></param>
        /// <param name="showParam"></param>
        /// <param name="recoverparam"></param>
        /// <returns></returns>
        public BaseUICtrl ShowRecoverUI(string uiName, string recoverName, object showParam = null, object recoverparam = null)
        {
            if (!string.IsNullOrEmpty(recoverName))
            {
                PushRecoverUI(new UIRecoveryParam(uiName, recoverName, recoverparam));
            }
            return ShowUI(uiName, showParam);
        }

        /// <summary>
        /// 将打开UI时关闭的UI保存
        /// </summary>
        /// <param name="_param"></param>
        private void PushRecoverUI(UIRecoveryParam _param)
        {
            if (_param == null)
                return;
            mRecoverStack.Push(_param);
        }

        /// <summary>
        /// 关闭UI时，检测是否需要弹出UI
        /// </summary>
        /// <param name="uiName"></param>
        private void CheckRecoverUI(string uiName)
        {
            if (mRecoverStack.Count == 0)
                return;
            object recoverObj = mRecoverStack.Peek();
            if (recoverObj == null)
                return;

            UIRecoveryParam recobj = (UIRecoveryParam)recoverObj;
            if (recobj.UIName == uiName)
            {
                string recoverName = recobj.RecoverName;

                ShowUI(recoverName, recobj.RecoverParam);

                if (mRecoverStack.Count != 0)
                    mRecoverStack.Pop();
            }
            else
            {
                if (mUIMaskLst.Count > 0)
                {
                    string uiMaskName = mUIMaskLst[mUIMaskLst.Count - 1];
                    if (mActiveUIDic.ContainsKey(uiMaskName))
                    {
                        mActiveUIDic[uiMaskName].ResetSibling();
                    }
                    mUIMaskLst.RemoveAt(mUIMaskLst.Count - 1);
                }
            }
        }

        private List<BaseUICtrl> mActiveCtrlLst = new List<BaseUICtrl>();


        private bool BreakUIRule(BaseUICtrl showingUICtrl, BaseUICtrl toShowUICtrl)
        {
            if (showingUICtrl == null || toShowUICtrl == null || showingUICtrl.UIObjLayer != toShowUICtrl.UIObjLayer)
            {
                return false;
            }

            foreach (var breakRuleLst in mBreakUIRuleLst)
            {
                if (breakRuleLst.Contains(toShowUICtrl.UIName) && breakRuleLst.Contains(showingUICtrl.UIName))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="param"></param>
        public BaseUICtrl ShowUI(string uiName, object param = null)
        {
            if (mActiveUIDic.ContainsKey(uiName))
            {
                if (mActiveUIDic[uiName].LoadedObj)
                {
                    if (mActiveUIDic[uiName].NeetMask)
                    {
                        mUIMaskLst.Remove(uiName);
                        mUIMaskLst.Add(uiName);
                    }
                    mActiveUIDic[uiName].ResetSibling();
                    mActiveUIDic[uiName].ShowUI(param);
                }
                else
                {
                    Debug.LogFormat("ShowUI uiName = {0} is loading cant show again", uiName);
                }

                Debug.LogFormat("Showing uiName = {0}", uiName);
                return mActiveUIDic[uiName];
            }

            if (!mUIDic.ContainsKey(uiName))
            {
                Debug.LogErrorFormat("ShowUI error by uiname = {0} is not Register", uiName);
                return null;
            }

            Debug.LogFormat("ShowUI uiName = {0}", uiName);
            BaseUICtrl uictrl = mUIDic[uiName].ShowUI(param);
            UILayer uilayer = uictrl.UIObjLayer;

            if (uilayer == UILayer.Frame || uilayer == UILayer.Dialog_Bottom
                || uilayer == UILayer.Dialog_Middle || uilayer == UILayer.Dialog_Top)
            {
                mActiveCtrlLst.Clear();
                mActiveCtrlLst.AddRange(mActiveUIDic.Values);
                foreach (var ctrl in mActiveCtrlLst)
                {
                    if (ctrl.UIObjLayer == uilayer && ctrl.UIName != uiName)
                    {
                        if (!BreakUIRule(ctrl, uictrl))
                        {
                            ctrl.HideUI();
                        }
                    }
                }
            }

            mActiveUIDic[uiName] = uictrl;
            if (mActiveUIDic[uiName].NeetMask)
            {
                mUIMaskLst.Remove(uiName);
                mUIMaskLst.Add(uiName);
            }
            return uictrl;
        }

        /// <summary>
        /// 判断UI是否已经打开
        /// </summary>
        /// <param name="uiName"></param>
        /// <returns></returns>
        public bool UIIsShowing(string uiName)
        {
            return mActiveUIDic.ContainsKey(uiName);
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI(string uiName)
        {
            if (mActiveUIDic.ContainsKey(uiName))
            {
                mActiveUIDic[uiName].HideUI();
            }
            Debug.LogFormat("HideUI uiName = {0}", uiName);

            CheckRecoverUI(uiName);
        }

        public void HideUI(BaseUICtrl ui)
        {
            if (ui != null)
            {
                HideUI(ui.UIName);
            }
        }


        public void RemoveActiveUI(string uiName)
        {
            if (mActiveUIDic.ContainsKey(uiName))
            {
                if (mActiveUIDic[uiName].NeetMask)
                {
                    mUIMaskLst.Remove(uiName);
                }

                if (mUIMaskLst.Count > 0)
                {
                    SetUIMaskLayerLast(true, mUIDic[mUIMaskLst[mUIMaskLst.Count - 1]].UIObjLayer);
                }
                else
                {
                    SetUIMaskLayerLast(false, UILayer.ActorHP);
                }

                mActiveUIDic.Remove(uiName);
            }
        }

        /// <summary>
        /// 隐藏所有的UI 会有几个是不会隐藏的(主界面，Loading界面,Tips 界面)
        /// </summary>
        public void HideAllUI()
        {
            mRecoverStack.Clear();

            List<string> activeUILst = new List<string>(mActiveUIDic.Keys) { };
            foreach (var uiName in activeUILst)
            {
                if (!mIgnoreUILst.Contains(uiName)
                    && (mUIDic[uiName].UIObjLayer == UILayer.Frame
                        || mUIDic[uiName].UIObjLayer == UILayer.Dialog_Bottom
                        || mUIDic[uiName].UIObjLayer == UILayer.Dialog_Middle
                        || mUIDic[uiName].UIObjLayer == UILayer.Dialog_Top))
                {
                    mActiveUIDic[uiName].HideUI();
                }
            }
        }

        /// <summary>
        /// 真实关掉所有层的界面，一般业务层不调用这个界面
        /// </summary>
        public void RealHideAll()
        {
            mRecoverStack.Clear();

            List<string> activeUILst = new List<string>(mActiveUIDic.Keys) { };
            foreach (var uiName in activeUILst)
            {
                mActiveUIDic[uiName].HideUI();
            }
        }

        /// <summary>
        /// 屏幕坐标转局部坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="screenPos"></param>
        /// <param name="localPos"></param>
        public void ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPos, out Vector2 localPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, UICamera, out localPos);
        }

        public void OnLoginSuccess()
        {
            ShowUI(UITypeName.UIPlayerController);
            ShowUI(UITypeName.UIPlayerLook);
        }

        public void OnLogout()
        {

        }

        #endregion



        /// <summary>
        /// 界面覆盖保存的信息
        /// </summary>
        class UIRecoveryParam
        {
            public string UIName { get; private set; }
            public string RecoverName { get; private set; }
            public object RecoverParam { get; private set; }

            public UIRecoveryParam(string uiName, string recoverName, object _recoverParam)
            {
                UIName = uiName;
                RecoverName = recoverName;
                RecoverParam = _recoverParam;
            }
        }
    }
}

