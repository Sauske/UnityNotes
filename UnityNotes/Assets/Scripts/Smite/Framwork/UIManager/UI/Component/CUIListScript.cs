//==================================================================================
/// UI List 控件
/// @neoyang
/// @2015.03.05
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace Framework
{
    //List类型
    public enum enUIListType
    {
        Vertical,           //垂直列表，上下滚动
        Horizontal,         //水平列表，左右滚动
        VerticalGrid,       //垂直表格，上下滚动
        HorizontalGrid,     //水平表格，左右滚动
    };

    public class CUIListScript : CUIComponent
    {
        //List类型
        public enUIListType m_listType = enUIListType.Vertical;

        //元素数量
        public int m_elementAmount;

        //元素之间的间隔
        public Vector2 m_elementSpacing;

        //元素排版offset(Vertical和Grid为y方向偏移，Horizontal为x方向偏移)
        public float m_elementLayoutOffset = 0;

        //在content区域未超过滚动区域的时候，是否自动调整元素位置使其居中显示
        public bool m_autoCenteredElements = false;

        //在m_autoCenteredElements为true的前提下，横纵坐标都居中
        public bool m_autoCenteredBothSides = false;

        //List中没有元素时需要显示的提示Object
        public GameObject m_ZeroTipsObj = null;

        //当前选中的元素索引
        protected int m_selectedElementIndex = -1;

        //上次选中的元素索引
        protected int m_lastSelectedElementIndex = -1;

        //-------------------------------
        /// 事件
        //-------------------------------
        //List有元素被选择时是否总是派发selectedChanged事件
        public bool m_alwaysDispatchSelectedChangeEvent;

        //选中项发生改变派发事件
        [HideInInspector]
        public enUIEventID m_listSelectChangedEventID;
        public stUIEventParams m_listSelectChangedEventParams;

        //Scroll发生改变派发事件
        [HideInInspector]
        public enUIEventID m_listScrollChangedEventID;
        public stUIEventParams m_listScrollChangedEventParams;

        //元素
        //[HideInInspector]
        protected ListView<CUIListElementScript> m_elementScripts;
        protected ListView<CUIListElementScript> m_unUsedElementScripts;
        protected GameObject m_elementTemplate;     //元素模板
        protected string m_elementName;             //元素名字
        protected Vector2 m_elementDefaultSize;     //元素模板的默认大小
        protected List<Vector2> m_elementsSize;
        protected List<stRect> m_elementsRect;

        //Scroll区域相关
        [HideInInspector]
        public ScrollRect m_scrollRect;             //滚动控件

        [HideInInspector]
        public Vector2 m_scrollAreaSize;
        protected GameObject m_content;
        protected RectTransform m_contentRectTransform;
        protected Vector2 m_contentSize;
        protected Scrollbar m_scrollBar;
        //protected Vector2 m_scrollValue;
        protected Vector2 m_lastContentPosition;

        //--------------------------------------------------
        /// Optimized
        //--------------------------------------------------
        public bool m_useOptimized = false;      

        //--------------------------------------------------
        /// 元素来源
        //--------------------------------------------------
        [HideInInspector]
        public bool m_useExternalElement = false;

        [HideInInspector]
        public string m_externalElementPrefabPath = null;

        //--------------------------------------------------
        /// 是否动态调整ListSize
        //--------------------------------------------------
        [HideInInspector]
        public bool m_autoAdjustScrollAreaSize = false;

        [HideInInspector]
        public Vector2 m_scrollRectAreaMaxSize = new Vector2(100, 100);

        //--------------------------------------------------
        /// 外部控制滚动
        //--------------------------------------------------
        public bool m_scrollExternal = false;

        //额外内容(加到所有element的后面，用于显示某些特殊内容或响应某些特殊事件，如"请求下一页数据")
        public GameObject m_extraContent = null;
        //额外内容是否生效
        private bool m_isExtraContentEnabled = true;

        //是否禁用ScrollRect（适用于诸如List嵌套的情形，此时内层List往往不响应滚动而应该将滚动输入冒泡给上层）
        public bool m_isDisableScrollRect = false;

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

            m_selectedElementIndex = -1;
            m_lastSelectedElementIndex = -1;

            //初始化Scroll Content           
            m_scrollRect = GetComponentInChildren<ScrollRect>(gameObject);

            if (m_scrollRect != null)
            {                
                m_scrollRect.enabled = false;

                RectTransform scrollRectRectTransform = m_scrollRect.transform as RectTransform;
                m_scrollAreaSize = new Vector2(scrollRectRectTransform.rect.width, scrollRectRectTransform.rect.height);

                m_content = m_scrollRect.content.gameObject;
            }

            //初始化ScrollBar
            m_scrollBar = GetComponentInChildren<Scrollbar>(gameObject);

            if (m_listType == enUIListType.Vertical || m_listType == enUIListType.VerticalGrid)
            {
                if (m_scrollBar != null)
                {
                    m_scrollBar.direction = Scrollbar.Direction.BottomToTop;
                }

                DebugHelper.Assert(m_scrollRect != null);

                if (m_scrollRect != null)
                {
                    m_scrollRect.horizontal = false;
                    m_scrollRect.vertical = true;
                    m_scrollRect.horizontalScrollbar = null;
                    m_scrollRect.verticalScrollbar = m_scrollBar;
                }                
            }
            else if (m_listType == enUIListType.Horizontal || m_listType == enUIListType.HorizontalGrid)
            {
                if (m_scrollBar != null)
                {
                    m_scrollBar.direction = Scrollbar.Direction.LeftToRight;
                }

                DebugHelper.Assert(m_scrollRect != null);

                if( m_scrollRect != null )
                {
                    m_scrollRect.horizontal = true;
                    m_scrollRect.vertical = false;
                    m_scrollRect.horizontalScrollbar = m_scrollBar;
                    m_scrollRect.verticalScrollbar = null;
                }                
            }

            //初始化元素列表
            m_elementScripts = new ListView<CUIListElementScript>();
            m_unUsedElementScripts = new ListView<CUIListElementScript>();

            if (m_useOptimized)
            {
                if (m_elementsRect == null)
                {
                    m_elementsRect = new List<stRect>();
                }                
            }

            CUIListElementScript listElementScript = null;

            if (m_useExternalElement)
            {
                GameObject elementPrefab = null;
                if (m_externalElementPrefabPath != null)
                {
                    elementPrefab = new GameObject();// (GameObject)CResourceManager.GetInstance().GetResource(m_externalElementPrefabPath, typeof(GameObject), enResourceType.UIPrefab).m_content;
                    if (elementPrefab != null)
                        listElementScript = elementPrefab.GetComponent<CUIListElementScript>();
                }


                if (listElementScript != null && elementPrefab != null)
                {
                    listElementScript.Initialize(formScript);

                    m_elementTemplate = elementPrefab;
                    m_elementName = elementPrefab.name;
                    m_elementDefaultSize = listElementScript.m_defaultSize;
                }
            }
            else
            {
                listElementScript = GetComponentInChildren<CUIListElementScript>(gameObject);

                if (listElementScript != null)
                {
                    listElementScript.Initialize(formScript);

                    m_elementTemplate = listElementScript.gameObject;
                    m_elementName = listElementScript.gameObject.name;

                    m_elementDefaultSize = listElementScript.m_defaultSize;

                    if (m_elementTemplate != null)
                    {
                        m_elementTemplate.name = m_elementName + "_Template";
                    }                  
                }
            }
            
            //初始化为不可见
            if (m_elementTemplate != null)
            {
                CUIListElementScript elementScript = m_elementTemplate.GetComponent<CUIListElementScript>();
                if (elementScript != null && elementScript.m_useSetActiveForDisplay)
                {
                    m_elementTemplate.CustomSetActive(false);
                }
                else
                {
                    //确保Element被激活
                    if (!m_elementTemplate.activeSelf)
                    {
                        m_elementTemplate.SetActive(true);
                    }

                    //为List元素模版添加Canvas Group
                    CanvasGroup canvasGroup = m_elementTemplate.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = m_elementTemplate.AddComponent<CanvasGroup>();
                    }

                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false;
                }
            }

            //初始化content位置
            if (m_content != null)
            {
                m_contentRectTransform = m_content.transform as RectTransform;
                m_contentRectTransform.pivot = new Vector2(0, 1);
                m_contentRectTransform.anchorMin = new Vector2(0, 1);
                m_contentRectTransform.anchorMax = new Vector2(0, 1);
                m_contentRectTransform.anchoredPosition = Vector2.zero;
                m_contentRectTransform.localRotation = Quaternion.identity;
                m_contentRectTransform.localScale = new Vector3(1, 1, 1);

                m_lastContentPosition = m_contentRectTransform.anchoredPosition;
            }

            //初始化extraContent if exist
            if (m_extraContent != null)
            {
                RectTransform extraContentTransform = m_extraContent.transform as RectTransform;
                extraContentTransform.pivot = new Vector2(0, 1);
                extraContentTransform.anchorMin = new Vector2(0, 1);
                extraContentTransform.anchorMax = new Vector2(0, 1);
                extraContentTransform.anchoredPosition = Vector2.zero;
                extraContentTransform.localRotation = Quaternion.identity;
                extraContentTransform.localScale = Vector3.one;

                //宽度与listElement保持一致
                if (m_elementTemplate != null)
                {
                    extraContentTransform.sizeDelta = new Vector2((m_elementTemplate.transform as RectTransform).rect.width, extraContentTransform.sizeDelta.y);
                }

                if (extraContentTransform.parent != null && m_content != null)
                {
                    extraContentTransform.parent.SetParent(m_content.transform);
                }
                m_extraContent.SetActive(false);
            }
            m_isExtraContentEnabled = true;

            if (m_elementTemplate != null)
                SetElementAmount(m_elementAmount);
        }

        public void SetExternalElementPrefab(string externalElmtPrefPath)
        {
            CUIListElementScript listElementScript = null;
            GameObject elementPrefab = null;
            if (externalElmtPrefPath != null)
            {
                elementPrefab = new GameObject();// (GameObject)CResourceManager.GetInstance().GetResource(externalElmtPrefPath, typeof(GameObject), enResourceType.UIPrefab).m_content;
                if (elementPrefab != null)
                    listElementScript = elementPrefab.GetComponent<CUIListElementScript>();
            }


            if (listElementScript != null && elementPrefab != null)
            {
                listElementScript.Initialize(m_belongedFormScript);

                m_elementTemplate = elementPrefab;
                m_elementName = elementPrefab.name;
                m_elementDefaultSize = listElementScript.m_defaultSize;
            }

            if (m_elementTemplate != null)
                SetElementAmount(m_elementAmount);
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(this.gameObject);
            }

            m_ZeroTipsObj = null;
            m_elementTemplate = null;
            m_scrollRect = null;
            m_content = null;
            m_contentRectTransform = null;
            m_scrollBar = null;
            m_extraContent = null;

            if (m_elementScripts != null)
            {
                m_elementScripts.Clear();
                m_elementScripts = null;
            }

            if (m_unUsedElementScripts != null)
            {
                m_unUsedElementScripts.Clear();
                m_unUsedElementScripts = null;
            }

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Update
        //--------------------------------------------------
        protected virtual void Update()
        {
            if (m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            {
                return;
            }

            if (m_useOptimized)
            {
                UpdateElementsScroll();
            }

            //根据需求屏蔽滚动
            if (m_scrollRect != null && !m_scrollExternal && !m_isDisableScrollRect)
            {
                if (m_contentSize.x > m_scrollAreaSize.x || m_contentSize.y > m_scrollAreaSize.y)
                {
                    if (!m_scrollRect.enabled)
                    {
                        m_scrollRect.enabled = true;
                    }                    
                }
                else
                {
                    if (Mathf.Abs(m_contentRectTransform.anchoredPosition.x) < 0.001 && Mathf.Abs(m_contentRectTransform.anchoredPosition.y) < 0.001)
                    {
                        if (m_scrollRect.enabled)
                        {
                            m_scrollRect.enabled = false;
                        }                        
                    }
                }

                //检测是否发生了滚动
                DetectScroll();
            } 
        }

        //--------------------------------------------------
        /// 检测是否发生滚动
        //--------------------------------------------------
        protected void DetectScroll()
        {
            if (m_contentRectTransform.anchoredPosition != m_lastContentPosition)
            {
                m_lastContentPosition = m_contentRectTransform.anchoredPosition;

                //派发事件
                DispatchScrollChangedEvent();
            }
        }

        //--------------------------------------------------
        /// 设置元素数量
        /// !!该函数会触发Element OnCreate
        /// @amount
        //--------------------------------------------------
        public void SetElementAmount(int amount)
        {
            SetElementAmount(amount, null);
        }

        //--------------------------------------------------
        /// 设置元素数量
        /// !!该函数会触发Element OnCreate
        /// @amount
        /// @elementsSize
        //--------------------------------------------------
        public virtual void SetElementAmount(int amount, List<Vector2> elementsSize)
        {
            if (amount < 0)
            {
                amount = 0;
            }

            if (elementsSize != null && amount != elementsSize.Count)
            {
                DebugHelper.ConsoleLog("List element amount is not equal to the count of elementsSize!!!");
                return;
            }

            //回收所有Element，但不设置为disable
            RecycleElement(false);

            //设定元素数量
            m_elementAmount = amount;
            m_elementsSize = elementsSize;

            //处理元素
            ProcessElements();
  
            //集中处理一次UnUsedElement
            ProcessUnUsedElement();

            //处理没有元素的时候提示物件的显示
            if (m_ZeroTipsObj != null)
            {
                if (amount <= 0)
                {
                    //m_ZeroTipsObj.CustomSetActive(true);
                    m_ZeroTipsObj.SetActive(true);
                }
                else
                {
                    //m_ZeroTipsObj.CustomSetActive(false);
                    m_ZeroTipsObj.SetActive(false);
                }
            }
        }

        //--------------------------------------------------
        /// SetEvent
        /// @eventID
        //--------------------------------------------------
        public void SetElementSelectChangedEvent(enUIEventID eventID)
        {
            m_listSelectChangedEventID = eventID;
        }

        //--------------------------------------------------
        /// SetEvent
        /// @eventID
        /// @eventParams
        //--------------------------------------------------
        public void SetElementSelectChangedEvent(enUIEventID eventID, stUIEventParams eventParams)
        {
            m_listSelectChangedEventID = eventID;
            m_listSelectChangedEventParams = eventParams;
        }

        //--------------------------------------------------
        /// 设置选中元素
        /// @index
        /// @isDispatchSelectedChangeEvent 是否派发SelectedChange事件（增加此参数用以处理某些不需要派发默认SelectedChange事件的特殊情形）
        //--------------------------------------------------
        public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            //DebugHelper.ConsoleLog("List " + gameObject.name + " selected element " + index + " !!!");
            m_lastSelectedElementIndex = m_selectedElementIndex;
            m_selectedElementIndex = index;

            if (m_lastSelectedElementIndex == m_selectedElementIndex)
            {
                if (m_alwaysDispatchSelectedChangeEvent && isDispatchSelectedChangeEvent)
                {
                    //派发事件
                    DispatchElementSelectChangedEvent();
                }

                return;
            }

            if (m_lastSelectedElementIndex >= 0)
            {
                CUIListElementScript elementScript = GetElemenet(m_lastSelectedElementIndex);

                if (elementScript != null)
                {
                    elementScript.ChangeDisplay(false);
                }                
            }

            if (m_selectedElementIndex >= 0)
            {
                CUIListElementScript elementScript = GetElemenet(m_selectedElementIndex);

                if (elementScript != null)
                {
                    elementScript.ChangeDisplay(true);

                    if (elementScript.onSelected != null)
                    {
                        elementScript.onSelected();
                    }
                }  
            }

            //派发事件
            if (isDispatchSelectedChangeEvent)
            {
                DispatchElementSelectChangedEvent();
            }
        }

        //--------------------------------------------------
        /// 返回元素数量
        //--------------------------------------------------
        public int GetElementAmount()
        {
            return m_elementAmount;
        }

        //--------------------------------------------------
        /// 返回List元素
        //--------------------------------------------------
        public CUIListElementScript GetElemenet(int index)
        {
            if (index < 0 || index >= m_elementAmount)
            {
                return null;
            }

            //优化模式下，element不能再以index作为下标直接从m_elementScripts获取
            if (m_useOptimized)
            {
                for (int i = 0; i < m_elementScripts.Count; i++)
                {
                    if (m_elementScripts[i].m_index == index)
                    {
                        return m_elementScripts[i];
                    }
                }

                return null;
            }
            else
            {
                return m_elementScripts[index];
            }            
        }

        //--------------------------------------------------
        /// 返回当前选中的List元素
        //--------------------------------------------------
        public CUIListElementScript GetSelectedElement()
        {
            return GetElemenet(m_selectedElementIndex);
        }

        //--------------------------------------------------
        /// 返回上次选中的List元素
        //--------------------------------------------------
        public CUIListElementScript GetLastSelectedElement()
        {
            return GetElemenet(m_lastSelectedElementIndex);
        }

        //--------------------------------------------------
        /// 返回List元素默认size
        //--------------------------------------------------
        public Vector2 GetEelementDefaultSize()
        {
            return m_elementDefaultSize;
        }

        //--------------------------------------------------
        /// 返回当前选中的元素Index
        //--------------------------------------------------
        public int GetSelectedIndex()
        {
            return m_selectedElementIndex;
        }

        //--------------------------------------------------
        /// 返回上次选中的元素Index
        //--------------------------------------------------
        public int GetLastSelectedIndex()
        {
            return m_lastSelectedElementIndex;
        }

        //--------------------------------------------------
        /// 某项元素是否在滚动区域内(是否可见)
        //--------------------------------------------------
        public bool IsElementInScrollArea(int index)
        {
            if (index < 0 || index >= m_elementAmount)
            {
                return false;
            }

            stRect rect = (m_useOptimized) ? m_elementsRect[index] : m_elementScripts[index].m_rect;
            return IsRectInScrollArea(ref rect);
        }

        //--------------------------------------------------
        /// 返回Content大小
        //--------------------------------------------------
        public Vector2 GetContentSize()
        {
            return m_contentSize;
        }

        //--------------------------------------------------
        /// 返回滚动区域大小
        //--------------------------------------------------
        public Vector2 GetScrollAreaSize()
        {
            return m_scrollAreaSize;
        }

        //--------------------------------------------------
        /// 返回Content位置
        //--------------------------------------------------
        public Vector2 GetContentPosition()
        {
            return m_lastContentPosition;
        }

        //--------------------------------------------------
        /// 是否需要滚动
        //--------------------------------------------------
        public bool IsNeedScroll()
        {
            return (m_contentSize.x > m_scrollAreaSize.x || m_contentSize.y > m_scrollAreaSize.y);
        }

        //--------------------------------------------------
        /// 重置content位置
        //--------------------------------------------------
        public void ResetContentPosition()
        {
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(this.gameObject);
            }

            if (m_contentRectTransform != null)
            {
                m_contentRectTransform.anchoredPosition = Vector2.zero;
            }
        }

        //--------------------------------------------------
        /// 将某项元素移动到滚动区域内(使其可见)
        //--------------------------------------------------
        public void MoveElementInScrollArea(int index, bool moveImmediately)
        {
            if (index < 0 || index >= m_elementAmount)
            {
                return;
            }

            Vector2 fixedOffset = Vector2.zero;
            Vector2 position = Vector2.zero;

            stRect rect = (m_useOptimized) ? m_elementsRect[index] : m_elementScripts[index].m_rect;            
            position.x = m_contentRectTransform.anchoredPosition.x + rect.m_left;
            position.y = m_contentRectTransform.anchoredPosition.y + rect.m_top;

            if (position.x < 0)
            {
                fixedOffset.x = -position.x;
            }
            else if (position.x + rect.m_width > m_scrollAreaSize.x)
            {
                fixedOffset.x = m_scrollAreaSize.x - (position.x + rect.m_width);
            }

            if (position.y > 0)
            {
                fixedOffset.y = -position.y;
            }
            else if (position.y - rect.m_height < -m_scrollAreaSize.y)
            {
                fixedOffset.y = -m_scrollAreaSize.y - (position.y - rect.m_height);
            }
           
            if (moveImmediately)
            {
                m_contentRectTransform.anchoredPosition += fixedOffset;
            }
            else
            {
                Vector2 contentTargetAnchoredPosition = m_contentRectTransform.anchoredPosition + fixedOffset;
                LeanTween.value(this.gameObject, delegate(Vector2 pos) { m_contentRectTransform.anchoredPosition = pos; }, m_contentRectTransform.anchoredPosition, contentTargetAnchoredPosition, m_fSpeed);
            }
        }

        public float m_fSpeed = 0.2f;
        //--------------------------------------------------
        /// index项element是否被选中
        //--------------------------------------------------
        public virtual bool IsSelectedIndex(int index)
        {
            return (m_selectedElementIndex == index);
        }

        public void ShowExtraContent()
        {
            if (m_extraContent != null && m_isExtraContentEnabled)
            {
                m_extraContent.CustomSetActive(true);
            }
        }

        public void HideExtraContent()
        {
            if (m_extraContent != null && m_isExtraContentEnabled)
            {
                m_extraContent.CustomSetActive(false);
            }
        }

        //--------------------------------------------------
        /// 移动content
        /// @offset {-1, 0, 1} : 表示移动
        //--------------------------------------------------
        public void MoveContent(Vector2 offset)
        {
            if (m_contentRectTransform != null)
            {
                Vector2 contentAnchoredPosition = (m_content.transform as RectTransform).anchoredPosition;
                contentAnchoredPosition += offset;

                //边界保护
                if (offset.x != 0)
                {
                    if (contentAnchoredPosition.x > 0)
                    {
                        contentAnchoredPosition.x = 0;
                    }
                    else if (contentAnchoredPosition.x + m_contentSize.x < m_scrollAreaSize.x)
                    {
                        contentAnchoredPosition.x = m_scrollAreaSize.x - m_contentSize.x;
                    }
                }

                if (offset.y != 0)
                {
                    if (contentAnchoredPosition.y < 0)
                    {
                        contentAnchoredPosition.y = 0;
                    }
                    else if (m_contentSize.y - contentAnchoredPosition.y < m_scrollAreaSize.y)
                    {
                        contentAnchoredPosition.y = m_contentSize.y - m_scrollAreaSize.y;
                    }
                }

                m_contentRectTransform.anchoredPosition = contentAnchoredPosition;
            }                      
        }

        //--------------------------------------------------
        /// 派发选项改变事件
        //--------------------------------------------------
        protected void DispatchElementSelectChangedEvent()
        {
            if (m_listSelectChangedEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_listSelectChangedEventID;
                uiEvent.m_eventParams = m_listSelectChangedEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);
            }
        }

        //--------------------------------------------------
        /// 派发Scroll改变事件
        //--------------------------------------------------
        protected void DispatchScrollChangedEvent()
        {
            if (m_listScrollChangedEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_listScrollChangedEventID;
                uiEvent.m_eventParams = m_listScrollChangedEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);
            }
        }

        //--------------------------------------------------
        /// 处理元素
        //--------------------------------------------------
        protected virtual void ProcessElements()
        {
            m_contentSize = Vector2.zero;
            Vector2 offset = Vector2.zero;

            if (m_listType == enUIListType.Vertical || m_listType == enUIListType.VerticalGrid)
            {
                offset.y += m_elementLayoutOffset;
            }
            else
            {
                offset.x += m_elementLayoutOffset;
            }

            for (int i = 0; i < m_elementAmount; i++)
            {
                //Element布局
                stRect rect = LayoutElement(i, ref m_contentSize, ref offset);

                //记录Element Rect
                if (m_useOptimized)
                {
                    if (i < m_elementsRect.Count)
                    {
                        m_elementsRect[i] = rect;
                    }
                    else
                    {
                        m_elementsRect.Add(rect);
                    }
                }

                if (!m_useOptimized || IsRectInScrollArea(ref rect))
                {
                    CreateElement(i, ref rect);
                }                
            }

            //设置extraContent if exist
            if (m_extraContent != null)
            {
                if (m_elementAmount > 0 && m_isExtraContentEnabled)
                {
                    ProcessExtraContent(ref m_contentSize, offset);
                }
                else
                {
                    m_extraContent.CustomSetActive(false);
                }
            }

            //设置内容区域大小
            ResizeContent(ref m_contentSize, false);
        }

        /// <summary>
        /// 处理特殊内容
        /// </summary>
        /// <param name="offset"></param>
        private void ProcessExtraContent(ref Vector2 contentSize, Vector2 offset)
        {
            RectTransform extraContentRectTransform = m_extraContent.transform as RectTransform;
            extraContentRectTransform.anchoredPosition = offset;
            m_extraContent.CustomSetActive(true);

            //计算内容区域大小时需加上extraContent的大小和element spacing的大小
            if (m_listType == enUIListType.Horizontal || m_listType == enUIListType.HorizontalGrid)
            {
                contentSize.x += extraContentRectTransform.rect.width + m_elementSpacing.x;
            }
            else
            {
                contentSize.y += extraContentRectTransform.rect.height + m_elementSpacing.y;
            }
        }

        //--------------------------------------------------
        /// 处理元素滚动(优化模式)
        //--------------------------------------------------
        protected void UpdateElementsScroll()
        {
            //回收显示区域以外的Element
            for (int i = 0; i < m_elementScripts.Count; )
            {
                if (!IsRectInScrollArea(ref m_elementScripts[i].m_rect))
                {
                    RecycleElement(m_elementScripts[i], true);
                    continue;
                }

                i++;
            }

            //展示显示区域以内的Element
            for (int i = 0; i < m_elementAmount; i++)
            {
                stRect rect = m_elementsRect[i];

                if (IsRectInScrollArea(ref rect))
                {
                    bool exist = false;

                    for (int j = 0; j < m_elementScripts.Count; j++)
                    {
                        if (m_elementScripts[j].m_index == i)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        CreateElement(i, ref rect);
                    }
                }
            }
        }

        //--------------------------------------------------
        /// 元素布局
        /// @index
        /// @elementScript
        /// @contentSize
        /// @offset
        /// @return 元素排版之后的rect
        //--------------------------------------------------
        protected stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect = new stRect();
            rect.m_width = (int)((m_elementsSize == null || m_listType == enUIListType.Vertical || m_listType == enUIListType.VerticalGrid || m_listType == enUIListType.HorizontalGrid) ? m_elementDefaultSize.x : m_elementsSize[index].x);
            rect.m_height = (int)((m_elementsSize == null || m_listType == enUIListType.Horizontal || m_listType == enUIListType.VerticalGrid || m_listType == enUIListType.HorizontalGrid) ? m_elementDefaultSize.y : m_elementsSize[index].y);
            rect.m_left = (int)offset.x;
            rect.m_top = (int)offset.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;
            rect.m_center = new Vector2(rect.m_left + rect.m_width * 0.5f, rect.m_top - rect.m_height * 0.5f);

            if (rect.m_right > contentSize.x)
            {
                contentSize.x = rect.m_right;
            }

            if (-rect.m_bottom > contentSize.y)
            {
                contentSize.y = -rect.m_bottom;
            }

            switch (m_listType)
            {
                case enUIListType.Vertical:
                {                        
                    offset.y -= (rect.m_height + m_elementSpacing.y);
                }
                break;

                case enUIListType.Horizontal:
                {
                    offset.x += (rect.m_width + m_elementSpacing.x);
                }
                break;

                case enUIListType.VerticalGrid:
                {
                    offset.x += (rect.m_width + m_elementSpacing.x);

                    if (offset.x + rect.m_width > m_scrollAreaSize.x)
                    {
                        offset.x = 0;
                        offset.y -= (rect.m_height + m_elementSpacing.y);
                    }                        
                }
                break;

                case enUIListType.HorizontalGrid:
                {
                    offset.y -= (rect.m_height + m_elementSpacing.y);

                    if (-offset.y + rect.m_height > m_scrollAreaSize.y)
                    {
                        offset.y = 0;
                        offset.x += (rect.m_width + m_elementSpacing.x);
                    }
                }
                break;
            }

            return rect;
        }

        //--------------------------------------------------
        /// Resize内容区域
        //--------------------------------------------------
        protected virtual void ResizeContent(ref Vector2 size, bool resetPosition)
        {            
            float contentOffsetX = 0;
            float contentOffsetY = 0;

            if (m_autoAdjustScrollAreaSize)
            {
                Vector2 lastScrollAreaSize = m_scrollAreaSize;

                m_scrollAreaSize = size;

                if (m_scrollAreaSize.x > m_scrollRectAreaMaxSize.x)
                {
                    m_scrollAreaSize.x = m_scrollRectAreaMaxSize.x;
                }

                if (m_scrollAreaSize.y > m_scrollRectAreaMaxSize.y)
                {
                    m_scrollAreaSize.y = m_scrollRectAreaMaxSize.y;
                }

                Vector2 deltaSize = m_scrollAreaSize - lastScrollAreaSize;

                //调整List大小从而达到调整scrollRect大小的目的
                if (deltaSize != Vector2.zero)
                {
                    RectTransform rectTransform = gameObject.transform as RectTransform;
                    if (rectTransform.anchorMin == rectTransform.anchorMax)
                    {
                        rectTransform.sizeDelta = rectTransform.sizeDelta + deltaSize;
                    }
                }
            }
            else
            {
                //如果需要用到自动居中，这里需要计算出content的offset
                if (m_autoCenteredElements)
                {
                    if (m_listType == enUIListType.Vertical && size.y < m_scrollAreaSize.y)
                    {
                        contentOffsetY = (size.y - m_scrollAreaSize.y) / 2;
                        //调整x轴方向
                        if (m_autoCenteredBothSides)
                        {
                            contentOffsetX = (m_scrollAreaSize.x - size.x)/2;
                        }
                    }
                    else if (m_listType == enUIListType.Horizontal && size.x < m_scrollAreaSize.x)
                    {
                        contentOffsetX = (m_scrollAreaSize.x - size.x) / 2;
                        //调整y轴方向
                        if (m_autoCenteredBothSides)
                        {
                            contentOffsetY = (size.y - m_scrollAreaSize.y)/2;
                        }
                    }
                    else if (m_listType == enUIListType.VerticalGrid && size.x < m_scrollAreaSize.x)
                    {
                        //Grid需要以整个Grid所占的区域来居中
                        float width = size.x;
                        float offset = width + m_elementSpacing.x;

                        while (true)
                        {
                            width = offset + m_elementDefaultSize.x;

                            if (width > m_scrollAreaSize.x)
                            {
                                break;
                            }
                            else
                            {
                                size.x = width;
                                offset = width + m_elementSpacing.x;
                            }
                        }

                        contentOffsetX = (m_scrollAreaSize.x - size.x) / 2;
                    }
                    else if (m_listType == enUIListType.HorizontalGrid && size.y < m_scrollAreaSize.y)
                    {
                        //Grid需要以整个Grid所占的区域来居中
                        float height = size.y;
                        float offset = height + m_elementSpacing.y;

                        while (true)
                        {
                            height = offset + m_elementDefaultSize.y;

                            if (height > m_scrollAreaSize.y)
                            {
                                break;
                            }
                            else
                            {
                                size.y = height;
                                offset = height + m_elementSpacing.y;
                            }
                        }

                        contentOffsetY = (size.y - m_scrollAreaSize.y) / 2;
                    }
                }
            }

            //修正size
            if (size.x < m_scrollAreaSize.x)
            {
                size.x = m_scrollAreaSize.x;
            }

            if (size.y < m_scrollAreaSize.y)
            {
                size.y = m_scrollAreaSize.y;
            }

            //设置内容区域
            if (m_contentRectTransform != null)
            {
                //设置大小
                m_contentRectTransform.sizeDelta = size;

                //设定位置
                if (resetPosition)
                {
                    m_contentRectTransform.anchoredPosition = Vector2.zero;
                }

                //自动调整位置使content居中
                if (m_autoCenteredElements)
                {
                    if (contentOffsetX != 0)
                    {
                        m_contentRectTransform.anchoredPosition = new Vector2(contentOffsetX, m_contentRectTransform.anchoredPosition.y);
                    }

                    if (contentOffsetY != 0)
                    {
                        m_contentRectTransform.anchoredPosition = new Vector2(m_contentRectTransform.anchoredPosition.x, contentOffsetY);
                    }
                }
            }
        }

        //--------------------------------------------------
        /// rect是否位于滚动可视区域
        //--------------------------------------------------
        protected bool IsRectInScrollArea(ref stRect rect)
        {
            Vector2 position = Vector2.zero;
            position.x = m_contentRectTransform.anchoredPosition.x + rect.m_left;
            position.y = m_contentRectTransform.anchoredPosition.y + rect.m_top;

            if (position.x + rect.m_width < 0
            || position.x > m_scrollAreaSize.x
            || position.y - rect.m_height > 0
            || position.y < -m_scrollAreaSize.y
            )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //--------------------------------------------------
        /// 回收Element
        /// @disableElement : 是否将element置为disable
        //--------------------------------------------------
        protected void RecycleElement(bool disableElement)
        {
            //将当前所有elements设置为unUsed
            while (m_elementScripts.Count > 0)
            {
                CUIListElementScript elementScript = m_elementScripts[0];
                m_elementScripts.RemoveAt(0);

                if (disableElement)
                {
                    elementScript.Disable();
                }
                
                m_unUsedElementScripts.Add(elementScript);
            }
        }

        //--------------------------------------------------
        /// 回收Element
        /// @elementScript
        /// @disableElement : 是否将element置为disable
        //--------------------------------------------------
        protected void RecycleElement(CUIListElementScript elementScript, bool disableElement)
        {
            if (disableElement)
            {
                elementScript.Disable();
            }

            m_elementScripts.Remove(elementScript);            
            m_unUsedElementScripts.Add(elementScript);
        }

        //--------------------------------------------------
        /// 创建Element
        /// @index
        //--------------------------------------------------
        protected CUIListElementScript CreateElement(int index, ref stRect rect)
        {
            CUIListElementScript elementScript = null;

            if (m_unUsedElementScripts.Count > 0)
            {
                elementScript = m_unUsedElementScripts[0];
                m_unUsedElementScripts.RemoveAt(0);
            }
            else if (m_elementTemplate != null)
            {
                GameObject elementObject = Instantiate(m_elementTemplate);
                elementObject.transform.SetParent(m_content.transform);
                elementObject.transform.localScale = Vector3.one;

                InitializeComponent(elementObject);

                elementScript = elementObject.GetComponent<CUIListElementScript>();
            }

            if (elementScript != null)
            {
                elementScript.Enable(this, index, m_elementName, ref rect, IsSelectedIndex(index));
                m_elementScripts.Add(elementScript);
            }

            return elementScript;
        }

        //--------------------------------------------------
        /// Update UnUsed Element
        //--------------------------------------------------
        protected void ProcessUnUsedElement()
        {
            if (m_unUsedElementScripts != null && m_unUsedElementScripts.Count > 0)
            {
                for (int i = 0; i < m_unUsedElementScripts.Count; i++)
                {
                    m_unUsedElementScripts[i].Disable();
                }
            }            
        }

        /// <summary>
        /// 使额外内容生效/不生效
        /// </summary>
        /// <param name="isEnabled">true有效，false无效</param>
        public void EnableExtraContent(bool isEnabled)
        {
            m_isExtraContentEnabled = isEnabled;
        }
    };
};