//==================================================================================
/// UI页面脚本
/// @UI页面需要做成prefab, 根节点为UGUI的Cavas，并且需要挂上本脚本
/// @neoyang
/// @2015.03.02
//==================================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using Framework;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    //Form优先级(越高显示在越上层)
    public enum enFormPriority
    {
        Priority0,
        Priority1,
        Priority2,
        Priority3,
        Priority4,
        Priority5,
        Priority6,
        Priority7,
        Priority8,
        Priority9
    };

    //Form事件类型
    public enum enFormEventType
    {
        Open,
        Close
    };

    //Form淡入/淡出动画类型
    public enum enFormFadeAnimationType
    {
        None,
        Animation,
        Animator
    };

    //Form Hide 标志
    public enum enFormHideFlag
    {
        HideByCustom = 1<<0,
        HideByOtherForm = 1<<1,
    };

    [ExecuteInEditMode]
    public class CUIFormScript : MonoBehaviour, IComparable
    {
        #region PROPERTIES
        //基准分辨率
        public Vector2 m_referenceResolution = new Vector2(960f, 640f);

        //是否单例
        public bool m_isSingleton;

        //是否模态
        public bool m_isModal;

        //关闭时是否清理内存
        //public bool m_clearMemoryWhenClosed;

        //优先级(0~9)
        private const int c_openOrderMask = 10;
        private const int c_priorityOrderMask = 1000;
        private const int c_overlayOrderMask = 10000;
        public enFormPriority m_priority = enFormPriority.Priority0;
        private enFormPriority m_defaultPriority = enFormPriority.Priority0;  //原始层级      

        //Form分组(同组只能有一个Form存在, 为0表示不启用)
        public int m_group = 0;

        //是否全屏背景(适配时需要进行裁切)
        public bool m_fullScreenBG;

        //禁用输入
        public bool m_disableInput;

        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[System.Enum.GetValues(typeof(enFormEventType)).Length];
        public stUIEventParams[] m_eventParams = new stUIEventParams[System.Enum.GetValues(typeof(enFormEventType)).Length];
        
        //打开时派发的WwiseEvent
        public string[] m_openedWwiseEvents = new string[1] { "UI_Default_Open_Window" };

        //关闭时派发的WwiseEvent
        public string[] m_closedWwiseEvents = new string[1] { "UI_Default_Close_Window" };

        //恢复可见时派发事件
        [HideInInspector]
        public enUIEventID m_revertToVisibleEvent;

        //被Hide时派发事件
        [HideInInspector]
        public enUIEventID m_revertToHideEvent;

        //是否隐藏在本Form之下的其他Form
        public bool m_hideUnderForms = false;

        //在其他Form打开时是否始终保持可见
        public bool m_alwaysKeepVisible = false;

        //是否允许派发多点触摸的Clicked事件
        public bool m_enableMultiClickedEvent = true;

        //Form所包含的UI控件列表
        public GameObject[] m_formWidgets = new GameObject[0];

        //Fade In
        public enFormFadeAnimationType m_formFadeInAnimationType = enFormFadeAnimationType.None;
        public string m_formFadeInAnimationName = string.Empty;
        public bool m_isPlayFadeInWithAppear = false;
        private CUIComponent m_formFadeInAnimationScript = null;

        //Fade Out
        [HideInInspector]
        public enFormFadeAnimationType m_formFadeOutAnimationType = enFormFadeAnimationType.None;

        [HideInInspector]
        public string m_formFadeOutAnimationName = string.Empty;

        private CUIComponent m_formFadeOutAnimationScript = null;

        [HideInInspector]
        public UIBasePage BelongPage;

        //clicked事件派发计数器
        [HideInInspector]
        public int m_clickedEventDispatchedCounter = 0;

        //Form带扩展名的全路径(唯一标识)
        [HideInInspector]
        public string m_formPath;

        //是否使用了FormPool
        [HideInInspector]
        public bool m_useFormPool;

        //是否需要关闭及是否处于关闭状态
        private bool m_isNeedClose;        
        private bool m_isClosed;
        private bool m_isInFadeIn;          //是否正在FadeIn
        private bool m_isInFadeOut;         //是否正在FadeOut

        //UGUI canvas
        private Canvas m_canvas;

        [HideInInspector]
        public CanvasScaler m_canvasScaler;
        
        private GraphicRaycaster m_graphicRaycaster;
        [HideInInspector, System.NonSerialized]
        public SGameGraphicRaycaster m_sgameGraphicRaycaster;
        private int m_openOrder;
        private int m_sortingOrder;

        //序列号(唯一标识)
        private int m_sequence;

        //是否处于隐藏状态及是否因为其他Form打开而被隐藏
        private bool m_isHided = false;
        private int m_hideFlags = 0;

        //渲染桢时间戳
        private int m_renderFrameStamp;

        //初始化控件位置信息
        private struct stInitWidgetPosition
        {
            public int m_renderFrameStamp;
            public GameObject m_widget;
            public Vector3 m_worldPosition;
        };

        //初始化控件位置列表
        private List<stInitWidgetPosition> m_initWidgetPositions;

        //是否初始化完成
        private bool m_isInitialized = false;

        //UIComponent列表(缓存起来以提高性能)
        private ListView<CUIComponent> m_uiComponents;

        //与Form关联的Scene
        [HideInInspector]
        private ListView<GameObject> m_relatedScenes;

        [HideInInspector]
        private ListView<ListView<Camera>> m_relatedSceneCamera;

        //等待加载sprite的Imgae列表
        private class CASyncLoadedImage
        {
            public Image m_image;
            public string m_prefabPath;
            public bool m_needCached;
            public bool m_unloadBelongedAssetBundleAfterLoaded;
            public bool m_isShowSpecMatrial = false;

            public CASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
            {
                m_image = image;
                m_prefabPath = prefabPath;
                m_needCached = needCached;
                m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
                m_isShowSpecMatrial = isShowSpecMatrial;
            }
        };

        private ListView<CASyncLoadedImage> m_asyncLoadedImages;

        //完成加载的sprite缓存
        private Dictionary<string, GameObject> m_loadedSpriteDictionary;

#if UNITY_EDITOR
        //屏幕适配相关
        private Vector2 m_screenSize = Vector2.zero;
        private uint m_checkCount = 0;
#endif
        #endregion

        void Awake() 
        {
            //初始化UI控件列表
            m_uiComponents = new ListView<CUIComponent>();

            //初始化关联的Scene列表
            m_relatedScenes = new ListView<GameObject>();

            //初始化关联的SceneCamera列表
            m_relatedSceneCamera = new ListView<ListView<Camera>>();

            //这里只能初始化Form自身相关的东西
            InitializeCanvas();
        } 

#if UNITY_EDITOR
        void Start()
        {
            //运行模式下才需要执行Form的初始化
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Initialize();
            }            
        }

        void Update()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (m_screenSize.x != Screen.width || m_screenSize.y != Screen.height)
                {
                    if (m_checkCount++ < 3)
                    {
                        //return;
                    }

                    m_checkCount = 0;
                    m_screenSize = new Vector2(Screen.width, Screen.height);

                    MatchScreen();
                    RefreshCanvasScaler();
                }
                else
                {
                    m_checkCount = 0;
                }
            }
            else
            {
                MatchScreen();
                RefreshCanvasScaler();
            }            
        }
#endif

        void OnDestroy()
        {
            if (m_asyncLoadedImages != null)
            {
                m_asyncLoadedImages.Clear();
                m_asyncLoadedImages = null;
            }

            if (m_loadedSpriteDictionary != null)
            {
                m_loadedSpriteDictionary.Clear();
                m_loadedSpriteDictionary = null;
            }
        }

        //--------------------------------------
        /// 自定义Update
        //--------------------------------------
        public void CustomUpdate()
        {
            UpdateFadeIn();
            UpdateFadeOut();
        }

        //--------------------------------------
        /// 自定义LateUpdate
        //--------------------------------------
        public void CustomLateUpdate()
        {
            if (m_initWidgetPositions != null)
            {
                for (int i = 0; i < m_initWidgetPositions.Count; )
                {
                    stInitWidgetPosition initWidgetPosition = m_initWidgetPositions[i];

                    if (m_renderFrameStamp - initWidgetPosition.m_renderFrameStamp <= 1)
                    {
                        if (initWidgetPosition.m_widget != null)
                        {
                            initWidgetPosition.m_widget.transform.position = initWidgetPosition.m_worldPosition;
                        }
                    }
                    else
                    {
                        m_initWidgetPositions.RemoveAt(i);
                        continue;
                    }

                    i++;
                }
            }

            UpdateASyncLoadedImage();

            m_clickedEventDispatchedCounter = 0;

            m_renderFrameStamp++;
        }

        //--------------------------------------
        /// 返回序列号
        //--------------------------------------
        public int GetSequence()
        {
            return m_sequence;
        }

        //--------------------------------------
        /// 设置显示顺序
        //--------------------------------------
        public void SetDisplayOrder(int openOrder)
        {
            DebugHelper.Assert(openOrder > 0, "openOrder = {0}, 该值必须大于0", openOrder);

            m_openOrder = openOrder;

            if (m_canvas != null)
            {
                m_sortingOrder = CalculateSortingOrder(m_priority, m_openOrder);
                m_canvas.sortingOrder = m_sortingOrder;

                //for解决Unity的挨球BUG，SortingOrder在某些情况下会出诡异BUG<neoyang>
                try
                {
                    if (m_canvas.enabled)
                    {
                        m_canvas.enabled = false;
                        m_canvas.enabled = true;
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.Assert(false, "Error form {0}: message: {1}, callstack: {2}", name, e.Message, e.StackTrace);
                }
            }

            //重置UI粒子order
            SetComponentSortingOrder(m_sortingOrder);
        }

        //--------------------------------------
        /// 设置开启标志
        /// @name       : Form名
        /// @camera     : Camrea
        /// @sequence   : 序列号
        /// @exist      : 是否已经存在
        /// @openOrder  : 打开顺序
        //--------------------------------------
        public void Open(string formPath, Camera camera, int sequence, bool exist, int openOrder)
        {
            m_formPath = formPath;

            if (m_canvas != null)
            {
                m_canvas.worldCamera = camera;

                if (camera == null)
                {
                    if (m_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    {
                        m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    }
                }
                else
                {
                    if (m_canvas.renderMode != RenderMode.ScreenSpaceCamera)
                    {
                        m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    }
                }
                
                m_canvas.pixelPerfect = true;// (camera == null);
            }

            //这里必须要刷新一次CanvasScaler，让Canvas的Transform的Scale立即生效
            RefreshCanvasScaler();

            Open(sequence, exist, openOrder);
        }

        //--------------------------------------
        /// 设置开启标志
        /// @sequence   : 序列号
        /// @exist      : 是否已经存在
        //--------------------------------------
        public void Open(int sequence, bool exist, int openOrder)
        {
            m_isNeedClose = false;
            m_isClosed = false;
            m_isInFadeIn = false;
            m_isInFadeOut = false;
            m_clickedEventDispatchedCounter = 0;

            m_sequence = sequence;
            
            SetDisplayOrder(openOrder);

            m_renderFrameStamp = 0;

            if (!exist)
            {
                //初始化
                Initialize();

                //这里需要重置一下输入射线，防止Form从Pool池里取出来的时候被改变了
                if (m_graphicRaycaster)
                {
                    m_graphicRaycaster.enabled = !m_disableInput;
                }                

                //派发事件
                DispatchFormEvent(enFormEventType.Open);

                //播放音效
                for (int i = 0; i < m_openedWwiseEvents.Length; i++)
                {
                    if (string.IsNullOrEmpty(m_openedWwiseEvents[i]))
                    {
                        continue;
                    }

                 //   CSoundManager.GetInstance().PostEvent(m_openedWwiseEvents[i]);
                }

                //淡入
                if (IsNeedFadeIn())
                {
                    StartFadeIn();
                }

            }                
        }

        protected  virtual void InitForm()
        {

        }

        protected virtual void CloseForm()
        {

        }

        //--------------------------------------
        /// 设置关闭标志
        //--------------------------------------
        public void Close()
        {
            //防止多次调用close
            if (m_isNeedClose)
            {
                return;
            }

            m_isNeedClose = true;

            //派发事件
            DispatchFormEvent(enFormEventType.Close);

            //播放音效
            for (int i = 0; i < m_closedWwiseEvents.Length; i++)
            {
                if (string.IsNullOrEmpty(m_closedWwiseEvents[i]))
                {
                    continue;
                }

             //   CSoundManager.GetInstance().PostEvent(m_closedWwiseEvents[i]);
            }
            //Close控件
            CloseComponent();
        }

        //--------------------------------------
        /// 是否需要关闭
        //--------------------------------------
        public bool IsNeedClose()
        {
            return m_isNeedClose;
        }

        //--------------------------------------
        /// 转为关闭状态
        /// @ignoreFadeOut : 是否忽略FadeOut
        /// @返回true表示不需要FadeOut
        //--------------------------------------
        public bool TurnToClosed(bool ignoreFadeOut)
        {
            m_isNeedClose = false;
            m_isClosed = true;

            EventRouter.GetInstance().BroadCastEvent(EventID.UI_FORM_CLOSED, m_formPath);

            if (ignoreFadeOut || !IsNeedFadeOut())
            {
                return true;
            }
            else
            {
                StartFadeOut();
                return false;
            }
        }

        //--------------------------------------
        /// 是否处于关闭状态
        //--------------------------------------
        public bool IsClosed()
        {
            return m_isClosed;
        }

        //--------------------------------------
        /// Canvas是否启用
        //--------------------------------------
        public bool IsCanvasEnabled()
        {
            if (m_canvas == null)
            {
                return false;
            }

            return m_canvas.enabled;
        }

        //--------------------------------------
        /// 屏幕取值转换为Form取值
        /// @value
        //--------------------------------------
        public float ChangeScreenValueToForm(float value)
        {
            if (m_canvasScaler.matchWidthOrHeight == 0f) //按照宽度进行缩放
            {
                return (value * m_canvasScaler.referenceResolution.x / Screen.width);
            }
            else if (m_canvasScaler.matchWidthOrHeight == 1f) // 按照高度进行缩放
            {
                return (value * m_canvasScaler.referenceResolution.y / Screen.height);
            }

            return value;
        }

        //--------------------------------------
        /// Form取值转换为Screen取值
        /// @value
        //--------------------------------------
        public float ChangeFormValueToScreen(float value)
        {
            if (m_canvasScaler.matchWidthOrHeight == 0f)
            {
                return (value * Screen.width / m_canvasScaler.referenceResolution.x);
            }
            else if (m_canvasScaler.matchWidthOrHeight == 1f)
            {
                return (value * Screen.height / m_canvasScaler.referenceResolution.y);
            }

            return value;
        }

        //--------------------------------------
        /// 初始化控件位置
        /// @index
        /// @worldPosition
        //--------------------------------------
        public void InitializeWidgetPosition(int widgetIndex, Vector3 worldPosition)
        {
            InitializeWidgetPosition(GetWidget(widgetIndex), worldPosition);
        }

        //--------------------------------------
        /// 初始化控件位置
        /// @widget
        /// @worldPosition
        //--------------------------------------
        public void InitializeWidgetPosition(GameObject widget, Vector3 worldPosition)
        {
            if (m_initWidgetPositions == null)
            {
                m_initWidgetPositions = new List<stInitWidgetPosition>();
            }

            stInitWidgetPosition initWidgetPosition = new stInitWidgetPosition();
            initWidgetPosition.m_renderFrameStamp = m_renderFrameStamp;
            initWidgetPosition.m_widget = widget;
            initWidgetPosition.m_worldPosition = worldPosition;

            m_initWidgetPositions.Add(initWidgetPosition);
        }

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public void Initialize()
        {
            if (m_isInitialized)
            {
                return;
            }

            //初始化属性
            m_defaultPriority = m_priority;

            //初始化组件
            InitializeComponent(this.gameObject);

            m_isInitialized = true;
        }

        //--------------------------------------
        /// 重置优先级
        /// @priority
        //--------------------------------------
        public void SetPriority(enFormPriority priority)
        {
            if (m_priority == priority)
            {
                return;
            }

            m_priority = priority;

            //这里需要即时修改sortingOrder
            SetDisplayOrder(m_openOrder);

            //派发事件
            DispatchChangeFormPriorityEvent();
        }

        //--------------------------------------
        /// 恢复原始优先级
        //--------------------------------------
        public void RestorePriority()
        {
            SetPriority(m_defaultPriority);
        }

        //--------------------------------------
        /// 设置Active
        //--------------------------------------
        public void SetActive(bool active)
        {
            this.gameObject.CustomSetActive(active);

            if (active)
            {
                Appear();
            }
            else
            {
                Hide();
            }
        }

        //--------------------------------------
        /// 隐藏
        /// @hideFlag
        /// @dispatchVisibleChangedEvent
        //--------------------------------------
        public void Hide(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
        {
            //始终保持可见的Form，不能被隐藏
            if (m_alwaysKeepVisible)
            {
                return;
            }

            //Add flag
            m_hideFlags |= (int)hideFlag;

            //隐藏标志为0或者已经被隐藏，不能再次进行隐藏操作
            if (m_hideFlags == 0 || m_isHided)
            {
                return;
            }

            m_isHided = true;

            if (m_canvas != null)
            {
                m_canvas.enabled = false;
            }

            /*
            if (m_graphicRaycaster)
            {
                m_graphicRaycaster.enabled = false;
            }
            */

            TryEnableInput(false);

            //Hide related Scene
            for (int i = 0; i < m_relatedScenes.Count; i++)
            {
                CUIUtility.SetGameObjectLayer(m_relatedScenes[i], CUIUtility.c_hideLayer);

                SetSceneCameraEnable(i, false);
            }

            HideComponent();

            //如果有hide事件需要派发
            if (enUIEventID.None != m_revertToHideEvent)
            {
                CUIEventManager.GetInstance().DispatchUIEvent(m_revertToHideEvent);
            }

            //派发VisibleChanged事件
            if (dispatchVisibleChangedEvent)
            {
                DispatchVisibleChangedEvent();
            }
        }

        //--------------------------------------
        /// SetSceneCameraEnable
        //--------------------------------------
        public void SetSceneCameraEnable(int index, bool bEnable)
        {
            if (index < 0 || index >= m_relatedSceneCamera.Count || null == m_relatedSceneCamera[index])
            {
                return;
            }

            for (int i = 0; i < m_relatedSceneCamera[index].Count; ++ i)
            {
                if (m_relatedSceneCamera[index][i] != null)
                {
                    m_relatedSceneCamera[index][i].enabled = bEnable;
                }
            }
        }

        //--------------------------------------
        /// 是否隐藏
        //--------------------------------------
        public bool IsHided()
        {
            return m_isHided;
        }

        //--------------------------------------
        /// 显示
        /// @hideFlag
        /// @dispatchVisibleChangedEvent
        //--------------------------------------
        public void Appear(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
        {
            //始终保持可见的Form，直接return
            if (m_alwaysKeepVisible)
            {
                return;
            }

            //Remove Flag
            m_hideFlags &= ~((int)hideFlag);

            //隐藏标志不为0或没有被隐藏，不能进行恢复操作
            if (m_hideFlags != 0 || !m_isHided)
            {
                return;
            }

            m_isHided = false;

            if (m_canvas != null)
            {
                m_canvas.enabled = true;
                m_canvas.sortingOrder = m_sortingOrder;
            }

            /*
            if (m_graphicRaycaster && !m_disableInput)
            {
                m_graphicRaycaster.enabled = true;
            }
            */
            TryEnableInput(true);

            //Appear related Scene
            for (int i = 0; i < m_relatedScenes.Count; i++)
            {
                CUIUtility.SetGameObjectLayer(m_relatedScenes[i], CUIUtility.c_UIBottomBg);

                SetSceneCameraEnable(i, true);
            }

            AppearComponent();

            //派发RevertVisible事件
            DispatchRevertVisibleFormEvent();

            //派发VisibleChanged事件
            if (dispatchVisibleChangedEvent)
            {
                DispatchVisibleChangedEvent();
            }

            /* 临时关闭动画
            if (IsNeedFadeIn() && m_isPlayFadeInWithAppear)
            {
                StartFadeIn();
            }
            */
        }

        //设置form是否接收事件，如果m_disableInput为true,设置为true也无效，设置为false以后要确保后面逻辑会开启
        public void TryEnableInput(bool isEnable)
        {
            if (m_graphicRaycaster == null)
            {
                return;
            }

            if (!isEnable)
            {
                m_graphicRaycaster.enabled = false;
            }
            else if (isEnable && !m_disableInput)
            {
                m_graphicRaycaster.enabled = true;
            }
        }

        //--------------------------------------
        /// 排序函数
        /// @按m_sortingOrder升序排列
        //--------------------------------------
        public int CompareTo(object obj)
        {
            CUIFormScript formScript = obj as CUIFormScript;

            if (m_sortingOrder > formScript.m_sortingOrder)
            {
                return 1;
            }
            else if (m_sortingOrder == formScript.m_sortingOrder)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        //--------------------------------------
        /// 初始化Canvas
        //--------------------------------------
        public void InitializeCanvas()
        {
            m_canvas = gameObject.GetComponent<Canvas>();
            m_canvasScaler = gameObject.GetComponent<CanvasScaler>();

            m_graphicRaycaster = GetComponent<GraphicRaycaster>();
            m_sgameGraphicRaycaster = m_graphicRaycaster as SGameGraphicRaycaster;

            //确定适配方式
            MatchScreen();
        }

        //--------------------------------------
        /// 适配屏幕
        //--------------------------------------
        public void MatchScreen()
        {
            if (m_canvasScaler == null)
            {
                return;
            }

            //以form上面设置的基准分辨率为准
            m_canvasScaler.referenceResolution = m_referenceResolution;

            //通常按高高比和宽宽比中较小的那边比例为准，防止适配出屏
            //如果需要全屏显示的背景图片，为了保证缩放比例，需要裁切掉一部分，这里就需要按高高比和宽宽比较大的那边比例为准
            if (Screen.width / m_canvasScaler.referenceResolution.x > Screen.height / m_canvasScaler.referenceResolution.y)
            {
                if (m_fullScreenBG)
                {
                    m_canvasScaler.matchWidthOrHeight = 0f;
                }
                else
                {
                    m_canvasScaler.matchWidthOrHeight = 1.0f;
                }                
            }
            else
            {
                if (m_fullScreenBG)
                {
                    m_canvasScaler.matchWidthOrHeight = 1.0f;
                }
                else
                {
                    m_canvasScaler.matchWidthOrHeight = 0f;
                }                
            }
        }

        //--------------------------------------
        /// 返回控件
        //--------------------------------------
        public GameObject GetWidget(int index)
        {
            if (index < 0 || index >= m_formWidgets.Length)
            {
                return null;
            }

            return m_formWidgets[index];
        }

        //--------------------------------------
        /// 返回GraphicRaycaster
        //--------------------------------------
        public GraphicRaycaster GetGraphicRaycaster()
        {
            return m_graphicRaycaster;
        }

        //--------------------------------------
        /// 返回Camera
        //--------------------------------------
        public Camera GetCamera()
        {
            if (m_canvas == null || m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return m_canvas.worldCamera;
        }

        //--------------------------------------
        /// 返回基准尺寸
        //--------------------------------------
        public Vector2 GetReferenceResolution()
        {
            return (m_canvasScaler == null) ? Vector2.zero : m_canvasScaler.referenceResolution;
        }

        //--------------------------------------
        /// 返回SortingOrder
        //--------------------------------------
        public int GetSortingOrder()
        {
            return m_sortingOrder;
        }

        //--------------------------------------
        /// 添加UI组件到缓存List
        //--------------------------------------
        public void AddUIComponent(CUIComponent uiComponent)
        {
            if (uiComponent != null && !m_uiComponents.Contains(uiComponent))
            {
                m_uiComponents.Add(uiComponent);
            }
        }

        //--------------------------------------
        /// 从缓存List移除UI组件
        //--------------------------------------
        public void RemoveUIComponent(CUIComponent uiComponent)
        {
            if (m_uiComponents.Contains(uiComponent))
            {
                m_uiComponents.Remove(uiComponent);
            }
        }

        //--------------------------------------
        /// 检查RelatedScene是否存在
        //--------------------------------------
        public bool IsRelatedSceneExist(string sceneName)
        {
            for (int i = 0; i < m_relatedScenes.Count; i++)
            {
                if (string.Equals(sceneName, m_relatedScenes[i].name))
                {
                    return true;
                }
            }

            return false;
        }

        //--------------------------------------
        /// 刷新CanvasScaler
        //--------------------------------------
        public void AddRelatedScene(GameObject scene, string sceneName)
        {
            scene.name = sceneName;
            scene.transform.SetParent(this.gameObject.transform);
            scene.transform.localPosition = Vector3.zero;
            scene.transform.localRotation = Quaternion.identity;
            //sceneObj.transform.lossyScale = Vector3.one;  //不要设置为1，应该让unity根据父容器大小自动计算新加入节点的scale变化。

            m_relatedScenes.Add(scene);

            m_relatedSceneCamera.Add(new ListView<Camera>());

            AddRelatedSceneCamera(m_relatedSceneCamera.Count - 1, scene);
        }

        public void AddRelatedSceneCamera(int index, GameObject go)
        {
            if (index < 0 
            || index >= m_relatedSceneCamera.Count 
            || go == null
            )
            {
                return;
            }

            Camera cam = go.GetComponent<Camera>();

            if (cam != null)
            {
                m_relatedSceneCamera[index].Add(cam);
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                AddRelatedSceneCamera(index, go.transform.GetChild(i).gameObject);
            }
        }

        //--------------------------------------
        /// 添加异步加载的Image
        /// @image
        /// @prefabPath
        //--------------------------------------
        public void AddASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
        {
            if (m_asyncLoadedImages == null)
            {
                m_asyncLoadedImages = new ListView<CASyncLoadedImage>();
            }

            if (m_loadedSpriteDictionary == null)
            {
                m_loadedSpriteDictionary = new Dictionary<string, GameObject>();
            }

            for (int i = 0; i < m_asyncLoadedImages.Count; i++)
            {
                if (m_asyncLoadedImages[i].m_image == image)
                {
                    m_asyncLoadedImages[i].m_prefabPath = prefabPath;
                    return;
                }
            }

            CASyncLoadedImage asyncLoadedImage = new CASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded,isShowSpecMatrial);
            m_asyncLoadedImages.Add(asyncLoadedImage);
        }

        //--------------------------------------
        /// Update异步加载的Image
        //--------------------------------------
        private void UpdateASyncLoadedImage()
        {
            if (m_asyncLoadedImages == null)
            {
                return;
            }

            //每桢只允许执行一次load操作
            bool doLoad = false;
            
            for (int i = 0; i < m_asyncLoadedImages.Count; )
            {
                var loadedImg = m_asyncLoadedImages[i];
                Image image = loadedImg.m_image;
                if (image != null)
                {
                    GameObject sprite = null;
                    var prefabPath = loadedImg.m_prefabPath;
                    if (!m_loadedSpriteDictionary.TryGetValue(prefabPath, out sprite))
                    {
                        //加载sprite
                        if (!doLoad)
                        {
                            sprite = null; // CUIUtility.GetSpritePrefeb(prefabPath, loadedImg.m_needCached, loadedImg.m_unloadBelongedAssetBundleAfterLoaded);
                            m_loadedSpriteDictionary.Add(prefabPath, sprite);

                            doLoad = true;
                        }
                    }

                    //设置Sprite
                    if (sprite != null)
                    {
                        //image alpha设置回1
                        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                     //   image.SetSprite(sprite, loadedImg.m_isShowSpecMatrial);

                        m_asyncLoadedImages.RemoveAt(i);
                        continue;
                    }
                    else
                    {
                        i++;
                    }                    
                }
                else
                {
                    m_asyncLoadedImages.RemoveAt(i);
                    continue;
                }
            }
        }

        //--------------------------------------
        /// 是否需要淡入
        //--------------------------------------
        public bool IsNeedFadeIn()
        {
            return false;// (GameSettings.RenderQuality != SGameRenderQuality.Low && m_formFadeInAnimationType != enFormFadeAnimationType.None && !string.IsNullOrEmpty(m_formFadeInAnimationName));
            //return (CUICommonSystem.IsOpenFormFadeFunc() && m_formFadeInAnimationType != enFormFadeAnimationType.None && !string.IsNullOrEmpty(m_formFadeInAnimationName));
        }

        //--------------------------------------
        /// 是否需要淡出
        /// @目前的版本暂不支持fadeout
        //--------------------------------------
        public bool IsNeedFadeOut()
        {
            return false;//(GameSettings.RenderQuality != SGameRenderQuality.Low && m_formFadeOutAnimationType != enFormFadeAnimationType.None && !string.IsNullOrEmpty(m_formFadeOutAnimationName));
        }

        //外部恢复大厅初始状态用
        public void RePlayFadIn()
        {
            StartFadeIn();
        }

        //--------------------------------------
        /// 刷新CanvasScaler
        //--------------------------------------
        private void RefreshCanvasScaler()
        {
            try
            {
                if (m_canvasScaler != null)
                {
                    m_canvasScaler.enabled = false;
                    m_canvasScaler.enabled = true;
                }
            }
            catch (Exception e)
            {
                DebugHelper.Assert(false, "Error form {0}: message: {1}, callstack: {2}", name, e.Message, e.StackTrace);
            }
        }

        //--------------------------------------
        /// 派发Form事件
        /// @formEventType
        //--------------------------------------
        private void DispatchFormEvent(enFormEventType formEventType)
        {
            if (m_eventIDs[(int)formEventType] == enUIEventID.None)
            {
                return;
            }

            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_eventID = m_eventIDs[(int)formEventType];
            uiEvent.m_eventParams = m_eventParams[(int)formEventType];
            uiEvent.m_srcFormScript = this;
            uiEvent.m_srcWidget = null;
            uiEvent.m_srcWidgetScript = null;
            uiEvent.m_srcWidgetBelongedListScript = null;
            uiEvent.m_srcWidgetIndexInBelongedList = 0;
            uiEvent.m_pointerEventData = null;

            CUIEventManager.GetInstance().DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 派发改变页面优先级事件
        //--------------------------------------
        private void DispatchChangeFormPriorityEvent()
        {
            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_eventID = enUIEventID.UI_OnFormPriorityChanged;
            uiEvent.m_srcFormScript = this;
            uiEvent.m_srcWidget = null;
            uiEvent.m_srcWidgetScript = null;
            uiEvent.m_srcWidgetBelongedListScript = null;
            uiEvent.m_srcWidgetIndexInBelongedList = 0;
            uiEvent.m_pointerEventData = null;

            CUIEventManager.GetInstance().DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 派发隐藏页面事件
        //--------------------------------------
        private void DispatchVisibleChangedEvent()
        {
            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_eventID = enUIEventID.UI_OnFormVisibleChanged;
            uiEvent.m_srcFormScript = this;
            uiEvent.m_srcWidget = null;
            uiEvent.m_srcWidgetScript = null;
            uiEvent.m_srcWidgetBelongedListScript = null;
            uiEvent.m_srcWidgetIndexInBelongedList = 0;
            uiEvent.m_pointerEventData = null;

            CUIEventManager.GetInstance().DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 派发显示页面事件
        //--------------------------------------
        private void DispatchRevertVisibleFormEvent()
        {
            if (enUIEventID.None == m_revertToVisibleEvent)
            {
                return;
            }

            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_eventID = m_revertToVisibleEvent;
            uiEvent.m_srcFormScript = this;
            uiEvent.m_srcWidget = null;
            uiEvent.m_srcWidgetScript = null;
            uiEvent.m_srcWidgetBelongedListScript = null;
            uiEvent.m_srcWidgetIndexInBelongedList = 0;
            uiEvent.m_pointerEventData = null;

            CUIEventManager.GetInstance().DispatchUIEvent(uiEvent);
        }

        //--------------------------------------
        /// 是否是Overlay RenderMode
        //--------------------------------------
        private bool IsOverlay()
        {
            if (m_canvas == null)
            {
                return false;
            }

            return (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || m_canvas.worldCamera == null);
        }

        //--------------------------------------
        /// 返回计算得到的sortingOrder
        //--------------------------------------
        private int CalculateSortingOrder(enFormPriority priority, int openOrder)
        {
            if (openOrder * c_openOrderMask >= c_priorityOrderMask)
            {
                openOrder %= (c_priorityOrderMask / c_openOrderMask);
            }

            return ((IsOverlay() ? c_overlayOrderMask : 0) + ((int)priority * c_priorityOrderMask) + (openOrder * c_openOrderMask));
        }

        //--------------------------------------
        /// 遍历初始化UI组件
        //--------------------------------------
        public void InitializeComponent(GameObject root)
        {
            CUIComponent[] uiComponents = root.GetComponents<CUIComponent>();

            if (uiComponents != null && uiComponents.Length > 0)
            {
                for (int i = 0; i < uiComponents.Length; i++)
                {
                    uiComponents[i].Initialize(this);
                }
            }

            for (int i = 0; i < root.transform.childCount; i++)
            {
                InitializeComponent(root.transform.GetChild(i).gameObject);
            }
        }

        //--------------------------------------
        /// UI组件操作:Close
        //--------------------------------------
        private void CloseComponent()
        {
            for (int i = 0; i < m_uiComponents.Count; i++)
            {
                m_uiComponents[i].Close();
            }
        }

        //--------------------------------------
        /// UI组件操作:Hide
        //--------------------------------------
        private void HideComponent()
        {
            for (int i = 0; i < m_uiComponents.Count; i++)
            {
                m_uiComponents[i].Hide();
            }
        }

        //--------------------------------------
        /// UI组件操作:Appear
        //--------------------------------------
        private void AppearComponent()
        {
            for (int i = 0; i < m_uiComponents.Count; i++)
            {
                m_uiComponents[i].Appear();
            }
        }

        //--------------------------------------
        /// UI组件操作:设置渲染顺序
        //--------------------------------------
        private void SetComponentSortingOrder(int sortingOrder)
        {
            for (int i = 0; i < m_uiComponents.Count; i++)
            {
                m_uiComponents[i].SetSortingOrder(sortingOrder);
            }
        }

        public void ClearFadeAnimationEndEvent()
        {
            CUIAnimationScript aniScript = gameObject.GetComponent<CUIAnimationScript>();

            if (aniScript != null)
            {
                //防止使用FadeOut的End事件，FadeIn,FadeRecover等肯定没有事件处理
                aniScript.SetAnimationEvent(enAnimationEventType.AnimationEnd, enUIEventID.None);
            }
        }

        //--------------------------------------
        /// 开始淡入
        //--------------------------------------
        private void StartFadeIn()
        {
            if (m_formFadeInAnimationType == enFormFadeAnimationType.None || string.IsNullOrEmpty(m_formFadeInAnimationName))
            {
                return;
            }

            switch (m_formFadeInAnimationType)
            {
                case enFormFadeAnimationType.Animation:
                {
                    m_formFadeInAnimationScript = gameObject.GetComponent<CUIAnimationScript>();
                    if (m_formFadeInAnimationScript != null)
                    {
                        //防止使用FadeOut的End事件，FadeIn肯定没有事件处理
                        ClearFadeAnimationEndEvent();

                        ((CUIAnimationScript)m_formFadeInAnimationScript).PlayAnimation(m_formFadeInAnimationName, true);
                        m_isInFadeIn = true;
                    }
                }
                break;

                case enFormFadeAnimationType.Animator:
                {
                    m_formFadeInAnimationScript = gameObject.GetComponent<CUIAnimatorScript>();
                    if (m_formFadeInAnimationScript != null)
                    {
                        ((CUIAnimatorScript)m_formFadeInAnimationScript).PlayAnimator(m_formFadeInAnimationName);
                        m_isInFadeIn = true;
                    }
                }
                break;
            }
        }

        //--------------------------------------
        /// 开始淡出
        //--------------------------------------
        private void StartFadeOut()
        {
            if (m_formFadeOutAnimationType == enFormFadeAnimationType.None || string.IsNullOrEmpty(m_formFadeOutAnimationName))
            {
                return;
            }

            switch (m_formFadeOutAnimationType)
            {
                case enFormFadeAnimationType.Animation:
                {
                    m_formFadeOutAnimationScript = gameObject.GetComponent<CUIAnimationScript>();
                    if (m_formFadeOutAnimationScript != null)
                    {
                        ((CUIAnimationScript)m_formFadeOutAnimationScript).PlayAnimation(m_formFadeOutAnimationName, true);
                        m_isInFadeOut = true;
                    }
                }
                break;

                case enFormFadeAnimationType.Animator:
                {
                    m_formFadeOutAnimationScript = gameObject.GetComponent<CUIAnimatorScript>();
                    if (m_formFadeOutAnimationScript != null)
                    {
                        ((CUIAnimatorScript)m_formFadeOutAnimationScript).PlayAnimator(m_formFadeOutAnimationName);
                        m_isInFadeOut = true;
                    }
                }
                break;
            }
        }

        //--------------------------------------
        /// Update淡入
        //--------------------------------------
        private void UpdateFadeIn()
        {
            if (m_isInFadeIn)
            {
                switch (m_formFadeInAnimationType)
                {
                    case enFormFadeAnimationType.Animation:
                    {
                        if (m_formFadeInAnimationScript == null || ((CUIAnimationScript)m_formFadeInAnimationScript).IsAnimationStopped(m_formFadeInAnimationName))
                        {
                            m_isInFadeIn = false;
                        }
                    }
                    break;

                    case enFormFadeAnimationType.Animator:
                    {
                        if (m_formFadeInAnimationScript == null || ((CUIAnimatorScript)m_formFadeInAnimationScript).IsAnimationStopped(m_formFadeInAnimationName))
                        {
                            m_isInFadeIn = false;
                        }
                    }
                    break;
                }
            }
        }

        //--------------------------------------
        /// Update淡出
        //--------------------------------------
        private void UpdateFadeOut()
        {
            if (m_isInFadeOut)
            {
                switch (m_formFadeOutAnimationType)
                {
                    case enFormFadeAnimationType.Animation:
                    {
                        if (m_formFadeOutAnimationScript == null || ((CUIAnimationScript)m_formFadeOutAnimationScript).IsAnimationStopped(m_formFadeOutAnimationName))
                        {
                            m_isInFadeOut = false;
                        }
                    }
                    break;

                    case enFormFadeAnimationType.Animator:
                    {
                        if (m_formFadeOutAnimationScript == null || ((CUIAnimatorScript)m_formFadeOutAnimationScript).IsAnimationStopped(m_formFadeOutAnimationName))
                        {
                            m_isInFadeOut = false;
                        }
                    }
                    break;
                }
            }
        }

        //--------------------------------------
        /// 是否处于淡入状态
        //--------------------------------------
        public bool IsInFadeIn()
        {
            return m_isInFadeIn;
        }

        //--------------------------------------
        /// 是否处于淡出状态
        //--------------------------------------
        public bool IsInFadeOut()
        {
            return m_isInFadeOut;
        }

        public float GetScreenScaleValue()
        {
            float fScaleValue = 1;

            RectTransform formTrans = this.GetComponent<RectTransform>();
            if (formTrans)
            {
                if (m_canvasScaler.matchWidthOrHeight == 0f) //以宽度为基准计算
                {
                    fScaleValue = (formTrans.rect.width / formTrans.rect.height) / (m_canvasScaler.referenceResolution.x / m_canvasScaler.referenceResolution.y);
                }
            }

            return fScaleValue;
        }

        //通过代码设置hideUnderForm，用于特殊逻辑，比如设置界面在局外需要hideunder，但局内不需要
        public void SetHideUnderForm(bool isHideUnderForm)
        {
            m_hideUnderForms = isHideUnderForm;
          //  CUIManager.instance.ResetAllFormHideOrShowState();
        }

        //设置页面事件的参数
        public void SetFormEventParams(enFormEventType formEventType, stUIEventParams formEventParams)
        {
            m_eventParams[(int) formEventType] = formEventParams;
        }

        //获取页面事件的参数
        public stUIEventParams GetFormEventParams(enFormEventType formEventType)
        {
            return m_eventParams[(int) formEventType];
        }
    };
};