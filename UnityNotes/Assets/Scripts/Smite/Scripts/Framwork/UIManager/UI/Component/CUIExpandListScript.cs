//==================================================================================
/// UI Expand List 控件
/// @neoyang
/// @2015.06.23
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Framework
{
    public enum enExpandListSelectingState
    {
        None,
        Retract,
        Move,
        Expand
    };

    public class CUIExpandListScript : CUIListScript
    {
        //展开/收起耗时
        public float m_expandedTime = 0.15f;

        //选择element阶段content的修正位置滚动速度(像素/s)
        public float m_contentFixingSpeed = 1200;

        //元素展开时的size
        private Vector2 m_elementExpandedSize;

        //是否处于元素选择状态
        private enExpandListSelectingState m_selectingState = enExpandListSelectingState.None;

        //选择元素时content的位置、size及时间片
        private Vector2 m_contentAnchoredPosition;
        private Vector2 m_targetContentAnchoredPosition;
        private float m_timeSlice = 0;

        //当前选择的元素在content上面的位置
        //private Vector2 m_selectedElementLastPositionInContent;
        
        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            //目前不支持Grid类型
            if (m_listType == enUIListType.VerticalGrid)
            {
                m_listType = enUIListType.Vertical;
            }
            else if (m_listType == enUIListType.HorizontalGrid)
            {
                m_listType = enUIListType.Horizontal;
            }            

            //无论是否是optimize模式，都需要记录元素的排版位置
            if (m_elementsRect == null)
            {
                m_elementsRect = new List<stRect>();
            }

            base.Initialize(formScript);

            //获取展开的元素尺寸
            if (m_elementTemplate != null)
            {
                CUIExpandListElementScript expandListElementScrpit = m_elementTemplate.GetComponent<CUIExpandListElementScript>();
                if (expandListElementScrpit != null)
                {
                    m_elementExpandedSize = expandListElementScrpit.m_expandedSize;
                }
            }
            
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

            //处于选择中，需要修正content位置
            if (m_selectingState != enExpandListSelectingState.None)
            {
                if (m_scrollRect.enabled)
                {
                    m_scrollRect.StopMovement();
                    m_scrollRect.enabled = false;
                }

                UpdateSelectedElement(m_selectingState);
            }
            else
            {
                if (!m_scrollRect.enabled)
                {
                    m_scrollRect.StopMovement();
                    m_scrollRect.enabled = true;
                }
            }

            //优化模式
            if (m_useOptimized)
            {
                UpdateElementsScroll();
            }
        }

        //--------------------------------------------------
        /// 设置选中元素
        /// @index
        //--------------------------------------------------
        public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            if (m_selectingState != enExpandListSelectingState.None)
            {
                return;
            }

            //DebugHelper.ConsoleLog("List " + gameObject.name + " selected element " + index + " !!!");
            m_lastSelectedElementIndex = m_selectedElementIndex;
            m_selectedElementIndex = index;

            //可以取消选择
            if (m_lastSelectedElementIndex == m_selectedElementIndex)
            {
                m_selectedElementIndex = -1;

                //if (m_alwaysDispatchSelectedChangeEvent)
                //{
                    //派发事件
                //    DispatchElementSelectChangedEvent();
                //}

                //return;
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
            DispatchElementSelectChangedEvent();

            //记录下当前content位置，重置时间片并开始修正选项位置
            m_contentAnchoredPosition = m_contentRectTransform.anchoredPosition;
            m_timeSlice = 0f;
            
            //if (m_selectedElementIndex >= 0)
            //{
            //    stRect rect = m_elementsRect[m_selectedElementIndex];
            //    m_selectedElementLastPositionInContent = new Vector2(m_contentAnchoredPosition.x + rect.m_left, m_contentAnchoredPosition.y + rect.m_top);
            //}

            //如果上次有被展开的项，那么需要先缩进展开项
            if (m_lastSelectedElementIndex >= 0)
            {
                m_selectingState = enExpandListSelectingState.Retract;
            }
            else if (m_selectedElementIndex >= 0)
            {
                m_targetContentAnchoredPosition = GetTargetContentAnchoredPosition(m_selectedElementIndex);

                m_selectingState = enExpandListSelectingState.Move;
                m_timeSlice = 0;
            }
        }

        //--------------------------------------------------
        /// 立即设置选中元素(需要计算出每个元素在Content上面的排版位置)
        /// @index
        //--------------------------------------------------
        public void SelectElementImmediately(int index)
        {
            base.SelectElement(index);

            m_contentSize = Vector2.zero;
            Vector2 offset = Vector2.zero;

            if (m_listType == enUIListType.Horizontal)
            {
                offset.x += m_elementLayoutOffset;
            }
            else if (m_listType == enUIListType.Vertical)
            {
                offset.y += m_elementLayoutOffset;
            }

            for (int i = 0; i < m_elementAmount; i++)
            {
                //Element布局
                stRect rect = LayoutExpandElement(i, (i == index) ? 1 : 0, ref m_contentSize, ref offset);

                //记录Element Rect
                if (i < m_elementsRect.Count)
                {
                    m_elementsRect[i] = rect;
                }
                else
                {
                    m_elementsRect.Add(rect);
                }
            }

            //重新设定content区域的大小及位置
            ResizeContent(ref m_contentSize, false);

            if (index < 0 || index >= m_elementAmount)
            {
                m_contentRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                m_contentRectTransform.anchoredPosition = GetTargetContentAnchoredPosition(index);
            }

            //设定元素位置
            for (int i = 0; i < m_elementAmount; i++)
            {
                stRect rect = m_elementsRect[i];
                CUIListElementScript elementScript = GetElemenet(i);

                if (elementScript != null)
                {
                    elementScript.SetRect(ref rect);
                }
                else if (!m_useOptimized || IsRectInScrollArea(ref rect))
                {
                    CreateElement(i, ref rect);
                }
            }
        }

        //--------------------------------------------------
        /// 处理元素
        //--------------------------------------------------
        protected override void ProcessElements()
        {
            m_contentSize = Vector2.zero;
            Vector2 offset = Vector2.zero;

            if (m_listType == enUIListType.Horizontal)
            {
                offset.x += m_elementLayoutOffset;
            }
            else if (m_listType == enUIListType.Vertical)
            {
                offset.y += m_elementLayoutOffset;
            }

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
                }
            }

            //设置内容区域大小
            ResizeContent(ref m_contentSize, false);
        }

        //--------------------------------------------------
        /// Expand元素布局
        /// @index          : 元素索引
        /// @expandedRate   : 展开比率(0为收起，1为完全展开)
        /// @contentSize    : contentSize
        /// @offset         : 在content区域偏移量
        /// @return 元素排版之后的rect
        //--------------------------------------------------
        private stRect LayoutExpandElement(int index, float expandedRate, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect = new stRect();

            if (m_listType == enUIListType.Horizontal)
            {
                rect.m_width = (int)(m_elementDefaultSize.x + ((m_elementExpandedSize.x - m_elementDefaultSize.x) * expandedRate));
                rect.m_height = (int)m_elementDefaultSize.y;
            }
            else
            {
                rect.m_width = (int)m_elementDefaultSize.x;
                rect.m_height = (int)(m_elementDefaultSize.y + ((m_elementExpandedSize.y - m_elementDefaultSize.y) * expandedRate));
            }

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

            if (m_listType == enUIListType.Horizontal)
            {
                offset.x += (rect.m_width + m_elementSpacing.x);
            }
            else if (m_listType == enUIListType.Vertical)
            {
                offset.y -= (rect.m_height + m_elementSpacing.y);
            }
            
            return rect;
        }

        //--------------------------------------------------
        /// Update选择过程
        /// @selectingState
        //--------------------------------------------------
        private void UpdateSelectedElement(enExpandListSelectingState selectingState)
        {
            switch (selectingState)
            {
                case enExpandListSelectingState.Retract:
                {
                    if (m_timeSlice < m_expandedTime)
                    {
                        m_timeSlice += Time.deltaTime;

                        m_contentSize = Vector2.zero;
                        Vector2 offset = Vector2.zero;

                        if (m_listType == enUIListType.Horizontal)
                        {
                            offset.x += m_elementLayoutOffset;
                        }
                        else if (m_listType == enUIListType.Vertical)
                        {
                            offset.y += m_elementLayoutOffset;
                        }

                        for (int i = 0; i < m_elementAmount; i++)
                        {
                            //Element布局
                            float expandRate = 0;

                            if (i == m_lastSelectedElementIndex)
                            {
                                expandRate = 1- (m_timeSlice / m_expandedTime);
                                expandRate = Mathf.Clamp(expandRate, 0, 1);
                            }

                            stRect rect = LayoutExpandElement(i, expandRate, ref m_contentSize, ref offset);

                            //记录Element Rect
                            if (i < m_elementsRect.Count)
                            {
                                m_elementsRect[i] = rect;
                            }
                            else
                            {
                                m_elementsRect.Add(rect);
                            }
                        }

                        //重新设定content区域的大小及位置
                        ResizeContent(ref m_contentSize, false);

                        //修正content位置，尽量保持当前选中的项的位置不发生改变
                        if (m_selectedElementIndex >= 0 && m_selectedElementIndex < m_elementAmount)
                        {
                            //stRect rect = m_elementsRect[m_selectedElementIndex];
                            //m_contentAnchoredPosition.x = m_selectedElementLastPositionInContent.x - rect.m_left;
                        }
                        else
                        {
                            //如果收起之后不需要再展开某个选项，则修正content位置，保证边界
                            if (m_listType == enUIListType.Horizontal)
                            {
                                if (m_contentAnchoredPosition.x > 0)
                                {
                                    m_contentAnchoredPosition.x = 0;
                                }
                                else if (m_contentAnchoredPosition.x + m_contentSize.x < m_scrollAreaSize.x)
                                {
                                    m_contentAnchoredPosition.x = m_scrollAreaSize.x - m_contentSize.x;
                                }
                            }
                            else if (m_listType == enUIListType.Vertical)
                            {
                                if (m_contentAnchoredPosition.y < 0)
                                {
                                    m_contentAnchoredPosition.y = 0;
                                }
                                else if (m_contentAnchoredPosition.y - m_contentSize.y > -m_scrollAreaSize.y)
                                {
                                    m_contentAnchoredPosition.y = -m_scrollAreaSize.y + m_contentSize.y;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_selectedElementIndex >= 0 && m_selectedElementIndex < m_elementAmount)
                        {
                            m_targetContentAnchoredPosition = GetTargetContentAnchoredPosition(m_selectedElementIndex);

                            m_selectingState = enExpandListSelectingState.Move;
                            m_timeSlice = 0;
                        }
                        else
                        {
                            m_selectingState = enExpandListSelectingState.None;
                        }
                    }
                }
                break;

                case enExpandListSelectingState.Move:
                {
                    if (m_contentAnchoredPosition != m_targetContentAnchoredPosition)
                    {
                        if (m_listType == enUIListType.Horizontal)
                        {
                            int sign = (m_targetContentAnchoredPosition.x > m_contentAnchoredPosition.x) ? 1 : -1;

                            m_contentAnchoredPosition.x += Time.deltaTime * m_contentFixingSpeed * sign;

                            if ((sign > 0 && m_contentAnchoredPosition.x >= m_targetContentAnchoredPosition.x) || (sign < 0 && m_contentAnchoredPosition.x <= m_targetContentAnchoredPosition.x))
                            {
                                m_contentAnchoredPosition = m_targetContentAnchoredPosition;
                            }
                        }
                        else if (m_listType == enUIListType.Vertical)
                        {
                            int sign = (m_targetContentAnchoredPosition.y > m_contentAnchoredPosition.y) ? 1 : -1;

                            m_contentAnchoredPosition.y += Time.deltaTime * m_contentFixingSpeed * sign;

                            if ((sign > 0 && m_contentAnchoredPosition.y >= m_targetContentAnchoredPosition.y) || (sign < 0 && m_contentAnchoredPosition.y <= m_targetContentAnchoredPosition.y))
                            {
                                m_contentAnchoredPosition = m_targetContentAnchoredPosition;
                            }
                        }
                    }
                    else
                    {
                        m_selectingState = enExpandListSelectingState.Expand;
                        m_timeSlice = 0;
                    }
                }
                break;

                case enExpandListSelectingState.Expand:
                {
                    if (m_timeSlice < m_expandedTime)
                    {
                        m_timeSlice += Time.deltaTime;

                        m_contentSize = Vector2.zero;
                        Vector2 offset = Vector2.zero;

                        if (m_listType == enUIListType.Horizontal)
                        {
                            offset.x += m_elementLayoutOffset;
                        }
                        else if (m_listType == enUIListType.Vertical)
                        {
                            offset.y += m_elementLayoutOffset;
                        }

                        for (int i = 0; i < m_elementAmount; i++)
                        {
                            //Element布局
                            float expandRate = 0;

                            if (i == m_selectedElementIndex)
                            {
                                expandRate = (m_timeSlice / m_expandedTime);
                                expandRate = Mathf.Clamp(expandRate, 0, 1);
                            }

                            stRect rect = LayoutExpandElement(i, expandRate, ref m_contentSize, ref offset);

                            //记录Element Rect
                            if (i < m_elementsRect.Count)
                            {
                                m_elementsRect[i] = rect;
                            }
                            else
                            {
                                m_elementsRect.Add(rect);
                            }
                        }

                        //重新设定content区域的大小及位置
                        ResizeContent(ref m_contentSize, false);
                    }
                    else
                    {
                        m_selectingState = enExpandListSelectingState.None;
                    }
                }
                break;
            }

            //同步element位置
            for (int i = 0; i < m_elementAmount; i++)
            {
                stRect rect = m_elementsRect[i];
                CUIListElementScript elementScript = GetElemenet(i);

                if (elementScript != null)
                {
                    elementScript.SetRect(ref rect);
                }
            }    

            //移动content
            m_contentRectTransform.anchoredPosition = m_contentAnchoredPosition;
        }

        //--------------------------------------------------
        /// 根据选项确定content的目标位置
        /// @selectedElementIndex
        //--------------------------------------------------
        private Vector2 GetTargetContentAnchoredPosition(int selectedElementIndex)
        {
            if (selectedElementIndex < 0 || selectedElementIndex >= m_elementAmount)
            {
                return m_contentAnchoredPosition;
            }

            stRect rect = m_elementsRect[m_selectedElementIndex];
            rect.m_width = (int)m_elementExpandedSize.x;
            rect.m_height = (int)m_elementExpandedSize.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;

            //计算出content的目标位置
            Vector2 targetContentAnchoredPosition = m_contentAnchoredPosition;

            if (m_listType == enUIListType.Horizontal)
            {
                //修正右边界
                if (targetContentAnchoredPosition.x + rect.m_right > m_scrollAreaSize.x)
                {
                    targetContentAnchoredPosition.x = m_scrollAreaSize.x - rect.m_right;
                }

                //修正左边界
                if (targetContentAnchoredPosition.x + rect.m_left < 0)
                {
                    targetContentAnchoredPosition.x = -rect.m_left;
                }
            }
            else if (m_listType == enUIListType.Vertical)
            {
                //修正下边界
                if (targetContentAnchoredPosition.y + rect.m_bottom < -m_scrollAreaSize.y)
                {
                    targetContentAnchoredPosition.y = -m_scrollAreaSize.y - rect.m_bottom;
                }

                //修正上边界
                if (targetContentAnchoredPosition.y + rect.m_top > 0)
                {
                    targetContentAnchoredPosition.y = -rect.m_top;
                }
            }

/*
            //计算出content的目标位置
            Vector2 targetContentAnchoredPosition = m_contentAnchoredPosition;
            targetContentAnchoredPosition.x = -rect.m_left;

            //右边界修正
            if (targetContentAnchoredPosition.x + m_contentSize.x + m_elementExpandedSize.x - m_elementDefaultSize.x < m_scrollAreaSize.x)
            {
                targetContentAnchoredPosition.x = m_scrollAreaSize.x - (m_contentSize.x + m_elementExpandedSize.x - m_elementDefaultSize.x);
            }
*/ 

            return targetContentAnchoredPosition;
        }
    };
};