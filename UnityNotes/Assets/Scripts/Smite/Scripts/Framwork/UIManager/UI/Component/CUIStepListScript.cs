//==================================================================================
/// UI Step List 控件
/// @List特殊应用，仅支持一行元素，中心为选中位置，List元素滑动到中心自动选中
/// @位于List中心的元素scale设定为1，往两边依次递减
/// @neoyang
/// @2015.04.10
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace Framework
{
    public class CUIStepListScript : CUIListScript
    {
        //阶梯衰减率(下一个Element为上一个Element的多少倍)
        public float m_reductionRate = 0.7f;

        //滚动速度低于该速度时，需要进行scroll修正
        public float m_minSpeedToFixScroll = 50f;

        //修正时间和帧数
        public float m_fixScrollTime = 0.3f;

        //Dragging开始的事件
        [HideInInspector]
        public enUIEventID m_onStartDraggingEventID;
        public stUIEventParams m_onStartDraggingEventParams;

        //content左右两侧需要追加的宽度(因为头尾两个元素也需要在steplist的中部显示，如果不追加宽度的话，content会被自动拉到scrollrect的左边对齐)
        private float m_contentExtendSize;

        //StepList中心点X坐标
        private float m_stepListCenter;

        //Select区域
        private float m_selectAreaMin;
        private float m_selectAreaMax;

        //记录ScrollRect拖拽滚动速度
        private float m_scrollRectLastScrollSpeed;
        private bool m_scrollRectIsDragged;

        //是否正在修正scroll
        private bool m_fixingScroll = false;

        //Scroll修正速度
        private float m_fixScrollSpeed;

        //Content修正目标位置
        private float m_contentFixScrollTargetPosition;

        private bool m_bDontUpdate = false;

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            m_listType = enUIListType.Horizontal;
            m_elementSpacing = Vector2.zero;
            m_elementLayoutOffset = 0;

            //先将默认的元素数量记录下来，保证在base的Initialize函数中不进行元素数量初始化
            int defaultElementAmount = m_elementAmount;
            m_elementAmount = 0;

            base.Initialize(formScript);

            //修改元素模版的枢轴点类型
            CUIListElementScript elementScript = m_elementTemplate.GetComponent<CUIListElementScript>();
            if (elementScript != null)
            {
                elementScript.m_pivotType = enPivotType.Centre;
            }

            //计算content两侧需要增加的宽度
            m_contentExtendSize = (m_scrollAreaSize.x - m_elementDefaultSize.x) * 0.5f;
            
            //计算StepList中心位置
            m_stepListCenter = m_contentExtendSize + m_elementDefaultSize.x * 0.5f;
            
            //计算选择区域的左右边界
            m_selectAreaMin = m_contentExtendSize;
            m_selectAreaMax = m_scrollAreaSize.x - m_contentExtendSize;

            if (m_scrollRect != null)
            {
                m_scrollRectLastScrollSpeed = m_scrollRect.velocity.x;
            }

            //StepList需要使用ElementRect
            if (m_elementsRect == null)
            {
                m_elementsRect = new List<stRect>();
            }

            //这里才初始化StepList元素数量(因为初始化时需要用到上面计算出的值)
            SetElementAmount(defaultElementAmount);

            if (m_elementAmount > 0)
            {
                SelectElementImmediately(0);
            }            
        }

        public void SetDontUpdate(bool bDontUpdate)
        {
            m_bDontUpdate = bDontUpdate;
        }

        //--------------------------------------------------
        /// Update
        //--------------------------------------------------
        protected override void Update()
        {
            if (m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            {
                return;
            }

            if (m_useOptimized)
            {
                UpdateElementsScroll();
            }

            if (m_bDontUpdate || m_scrollRect == null)
            {
                return;
            }

            //修改元素大小
            for (int i = 0; i < m_elementAmount; i++)
            {
                CUIListElementScript listElement = GetElemenet(i);
                if (listElement != null)
                {
                    listElement.gameObject.transform.localScale = Vector3.one * GetElementScale(i);
                }                
            }

            //选项确定，修正选项位置
            if (m_fixingScroll && m_selectedElementIndex >= 0 && m_selectedElementIndex < m_elementAmount)
            {
                m_scrollRect.enabled = false;

                //修正
                float position = m_contentRectTransform.anchoredPosition.x + m_fixScrollSpeed;

                if (position > m_contentFixScrollTargetPosition && m_fixScrollSpeed > 0
                || position < m_contentFixScrollTargetPosition && m_fixScrollSpeed < 0
                )
                {
                    m_contentRectTransform.anchoredPosition = new Vector2(m_contentFixScrollTargetPosition, m_contentRectTransform.anchoredPosition.y);

                    m_fixScrollSpeed = 0;
                    m_fixingScroll = false;

                    //重新启用ScrollRect
                    m_scrollRect.StopMovement();
                }
                else
                {
                    m_contentRectTransform.anchoredPosition = new Vector2(position, m_contentRectTransform.anchoredPosition.y);
                }                
            }
            else
            {
                m_scrollRect.enabled = true;

                System.Reflection.FieldInfo draggingField = m_scrollRect.GetType().GetField("m_Dragging", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                bool dragging = (bool)draggingField.GetValue(m_scrollRect);

                if (!dragging)
                {
                    if (m_scrollRectIsDragged)
                    {                        
                        float scrollSpeed = Mathf.Abs(m_scrollRect.velocity.x);

                        int selectedIndex = -1;

                        //确定选项并开始进行位置修正
                        if (m_contentRectTransform.anchoredPosition.x > 0)
                        {
                            selectedIndex = 0;
                        }
                        else if (m_contentRectTransform.anchoredPosition.x + m_contentSize.x < m_scrollAreaSize.x)
                        {
                            selectedIndex = m_elementAmount - 1;
                        }
                        else if (scrollSpeed <= m_scrollRectLastScrollSpeed && scrollSpeed < m_minSpeedToFixScroll)
                        {                            
                            for (int i = 0; i < m_elementAmount; i++)
                            {
                                if (selectedIndex < 0 && IsElementInSelectedArea(i))
                                {
                                    selectedIndex = i;
                                    break;
                                }
                            }
                        }
                            
                        if (selectedIndex >= 0)
                        {
                            SelectElement(selectedIndex);
                            m_scrollRectIsDragged = false;
                        }
                    }
                    else
                    {
                        //强行修正
                        if (m_selectedElementIndex >= 0 && m_selectedElementIndex < m_elementAmount)
                        {
                            if (m_contentRectTransform.anchoredPosition.x != m_contentFixScrollTargetPosition)
                            {
                                m_contentRectTransform.anchoredPosition = new Vector2(m_contentFixScrollTargetPosition, m_contentRectTransform.anchoredPosition.y);

                                //修改元素大小
                                for (int i = 0; i < m_elementAmount; i++)
                                {
                                    CUIListElementScript listElement = GetElemenet(i);
                                    if (listElement != null)
                                    {
                                        listElement.gameObject.transform.localScale = Vector3.one * GetElementScale(i);
                                    }
                                }
                            }
                        }                         
                    }
                }
                else
                {
                    if (!m_scrollRectIsDragged)
                    {
                        DispatchOnStartDraggingEvent();
                    }

                    m_scrollRectIsDragged = true;
                }

                //记录ScrollSpeed
                m_scrollRectLastScrollSpeed = Mathf.Abs(m_scrollRect.velocity.x);
            }

            DetectScroll();
        }

        //--------------------------------------------------
        /// 设置选中元素(直接修正Content位置)
        /// @index
        //--------------------------------------------------
        public void SelectElementImmediately(int index)
        {
            base.SelectElement(index);

            if (index < 0 || index >= m_elementAmount)
            {
                return;
            }

            //判断是否需要进行位置修正
            m_contentFixScrollTargetPosition = GetContentTargetPosition(index);

            if (m_contentRectTransform.anchoredPosition.x != m_contentFixScrollTargetPosition)
            {
                m_contentRectTransform.anchoredPosition = new Vector2(m_contentFixScrollTargetPosition, m_contentRectTransform.anchoredPosition.y);
            }

            m_scrollRectIsDragged = false;
            m_fixingScroll = false;
        }

        //--------------------------------------------------
        /// 设置选中元素
        /// @index
        //--------------------------------------------------
        public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            base.SelectElement(index, isDispatchSelectedChangeEvent);

            if (index < 0 || index >= m_elementAmount)
            {
                return;
            }

            //计算Content目标位置
            m_contentFixScrollTargetPosition = GetContentTargetPosition(index);

            //判断是否需要进行位置修正
            if (m_contentRectTransform.anchoredPosition.x != m_contentFixScrollTargetPosition)
            {
                //停止控件滚动
                if (m_scrollRect != null)
                {
                    m_scrollRect.StopMovement();
                    //m_scrollRect.enabled = false;
                }

                //计算修正速度                
                m_fixScrollSpeed = (m_contentFixScrollTargetPosition - m_contentRectTransform.anchoredPosition.x) / (float)(m_fixScrollTime *30);// Assets.Scripts.Framework.GameFramework.c_renderFPS);

                if (Mathf.Abs(m_fixScrollSpeed) < 0.001f)
                {
                    m_contentRectTransform.anchoredPosition = new Vector2(m_contentFixScrollTargetPosition, m_contentRectTransform.anchoredPosition.y);
                }
                else
                {
                    m_fixingScroll = true;
                }

                m_scrollRectIsDragged = false;
            }          
        }

        //--------------------------------------------------
        /// 处理元素
        //--------------------------------------------------
        protected override void ProcessElements()
        {
            m_contentSize = Vector2.zero;
            Vector2 offset = Vector2.zero;

            m_contentSize.x += m_contentExtendSize;
            offset.x += m_contentExtendSize;

            for (int i = 0; i < m_elementAmount; i++)
            {
                //Element布局
                stRect rect = LayoutElement(i, ref m_contentSize, ref offset);

                //记录Element Rect
                if (i < m_elementsRect.Count)
                {
                    m_elementsRect[i] = rect;
                }
                else
                {
                    m_elementsRect.Add(rect);
                }

                if (!m_useOptimized || IsRectInScrollArea(ref rect))
                {
                    CreateElement(i, ref rect);
                   //   CUIListElementScript elementScript = CreateElement(i, ref rect);
                }
            }

            m_contentSize.x += m_contentExtendSize;

            //设置内容区域大小
            ResizeContent(ref m_contentSize, false);
        }

        //--------------------------------------------------
        /// 元素布局
        /// @index
        /// @elementScript
        /// @contentSize
        /// @offset
        /// @return 元素排版之后的rect
        //--------------------------------------------------
        //protected override stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        //{

        //}

        //--------------------------------------------------
        /// Resize内容区域
        //--------------------------------------------------
        protected override void ResizeContent(ref Vector2 size, bool resetPosition)
        {
            //设置内容区域
            if (m_contentRectTransform != null)
            {
                //设置大小
                m_contentRectTransform.sizeDelta = size;
                m_contentRectTransform.pivot = new Vector2(0f, 0.5f);
                m_contentRectTransform.anchorMin = new Vector2(0f, 0.5f);
                m_contentRectTransform.anchorMax = new Vector2(0f, 0.5f);
                m_contentRectTransform.anchoredPosition = Vector2.zero;

                //设定位置
                if (resetPosition)
                {
                    m_contentRectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

        //--------------------------------------------------
        /// 元素是否在选定区域内
        //--------------------------------------------------
        private bool IsElementInSelectedArea(int index)
        {
            float elementCenterXInScrollArea = m_elementsRect[index].m_center.x + m_contentRectTransform.anchoredPosition.x;

            return (elementCenterXInScrollArea > m_selectAreaMin && elementCenterXInScrollArea < m_selectAreaMax);
        }

        //--------------------------------------------------
        /// 返回元素缩放值
        //--------------------------------------------------
        private float GetElementScale(int index)
        {
            int elementCenterXInScrollArea = (int)(m_elementsRect[index].m_center.x + m_contentRectTransform.anchoredPosition.x);
            int delta = (int)Mathf.Abs(elementCenterXInScrollArea - m_stepListCenter);

            int pow = delta / (int)m_elementDefaultSize.x;
            int mod = delta % (int)m_elementDefaultSize.x;

            float maxScale = Mathf.Pow(m_reductionRate, pow);
            float minScale = Mathf.Pow(m_reductionRate, pow + 1);
            
            float scale = maxScale - ((maxScale - minScale) * (mod / m_elementDefaultSize.x));
            //scale = (int)(scale * 1000) / (float)1000;

            return scale;
        }

        //--------------------------------------------------
        /// 返回Content目标位置(x)
        /// @selectedIndex
        //--------------------------------------------------
        private float GetContentTargetPosition(int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= m_elementAmount)
            {
                return 0;
            }

            float targetPosition = m_stepListCenter - m_elementsRect[selectedIndex].m_center.x;

            if (targetPosition > 0)
            {
                targetPosition = 0;
            }
            else if (targetPosition < m_scrollAreaSize.x - m_contentSize.x)
            {
                targetPosition = m_scrollAreaSize.x - m_contentSize.x;
            }

            return targetPosition;
        }

        //--------------------------------------------------
        /// 派发开始Dragging事件
        //--------------------------------------------------
        private void DispatchOnStartDraggingEvent()
        {
            if (m_onStartDraggingEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_onStartDraggingEventID;
                uiEvent.m_eventParams = m_onStartDraggingEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);
            }
        }
    };
};