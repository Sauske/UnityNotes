//==================================================================================
/// UI Expand List �ؼ�
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
        //չ��/�����ʱ
        public float m_expandedTime = 0.15f;

        //ѡ��element�׶�content������λ�ù����ٶ�(����/s)
        public float m_contentFixingSpeed = 1200;

        //Ԫ��չ��ʱ��size
        private Vector2 m_elementExpandedSize;

        //�Ƿ���Ԫ��ѡ��״̬
        private enExpandListSelectingState m_selectingState = enExpandListSelectingState.None;

        //ѡ��Ԫ��ʱcontent��λ�á�size��ʱ��Ƭ
        private Vector2 m_contentAnchoredPosition;
        private Vector2 m_targetContentAnchoredPosition;
        private float m_timeSlice = 0;

        //��ǰѡ���Ԫ����content�����λ��
        //private Vector2 m_selectedElementLastPositionInContent;
        
        //--------------------------------------------------
        /// ��ʼ��
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            //Ŀǰ��֧��Grid����
            if (m_listType == enUIListType.VerticalGrid)
            {
                m_listType = enUIListType.Vertical;
            }
            else if (m_listType == enUIListType.HorizontalGrid)
            {
                m_listType = enUIListType.Horizontal;
            }            

            //�����Ƿ���optimizeģʽ������Ҫ��¼Ԫ�ص��Ű�λ��
            if (m_elementsRect == null)
            {
                m_elementsRect = new List<stRect>();
            }

            base.Initialize(formScript);

            //��ȡչ����Ԫ�سߴ�
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

            //����ѡ���У���Ҫ����contentλ��
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

            //�Ż�ģʽ
            if (m_useOptimized)
            {
                UpdateElementsScroll();
            }
        }

        //--------------------------------------------------
        /// ����ѡ��Ԫ��
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

            //����ȡ��ѡ��
            if (m_lastSelectedElementIndex == m_selectedElementIndex)
            {
                m_selectedElementIndex = -1;

                //if (m_alwaysDispatchSelectedChangeEvent)
                //{
                    //�ɷ��¼�
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

            //�ɷ��¼�
            DispatchElementSelectChangedEvent();

            //��¼�µ�ǰcontentλ�ã�����ʱ��Ƭ����ʼ����ѡ��λ��
            m_contentAnchoredPosition = m_contentRectTransform.anchoredPosition;
            m_timeSlice = 0f;
            
            //if (m_selectedElementIndex >= 0)
            //{
            //    stRect rect = m_elementsRect[m_selectedElementIndex];
            //    m_selectedElementLastPositionInContent = new Vector2(m_contentAnchoredPosition.x + rect.m_left, m_contentAnchoredPosition.y + rect.m_top);
            //}

            //����ϴ��б�չ�������ô��Ҫ������չ����
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
        /// ��������ѡ��Ԫ��(��Ҫ�����ÿ��Ԫ����Content������Ű�λ��)
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
                //Element����
                stRect rect = LayoutExpandElement(i, (i == index) ? 1 : 0, ref m_contentSize, ref offset);

                //��¼Element Rect
                if (i < m_elementsRect.Count)
                {
                    m_elementsRect[i] = rect;
                }
                else
                {
                    m_elementsRect.Add(rect);
                }
            }

            //�����趨content����Ĵ�С��λ��
            ResizeContent(ref m_contentSize, false);

            if (index < 0 || index >= m_elementAmount)
            {
                m_contentRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                m_contentRectTransform.anchoredPosition = GetTargetContentAnchoredPosition(index);
            }

            //�趨Ԫ��λ��
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
        /// ����Ԫ��
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
                //Element����
                stRect rect = LayoutElement(i, ref m_contentSize, ref offset);

                //��¼Element Rect
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

            //�������������С
            ResizeContent(ref m_contentSize, false);
        }

        //--------------------------------------------------
        /// ExpandԪ�ز���
        /// @index          : Ԫ������
        /// @expandedRate   : չ������(0Ϊ����1Ϊ��ȫչ��)
        /// @contentSize    : contentSize
        /// @offset         : ��content����ƫ����
        /// @return Ԫ���Ű�֮���rect
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
        /// Updateѡ�����
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
                            //Element����
                            float expandRate = 0;

                            if (i == m_lastSelectedElementIndex)
                            {
                                expandRate = 1- (m_timeSlice / m_expandedTime);
                                expandRate = Mathf.Clamp(expandRate, 0, 1);
                            }

                            stRect rect = LayoutExpandElement(i, expandRate, ref m_contentSize, ref offset);

                            //��¼Element Rect
                            if (i < m_elementsRect.Count)
                            {
                                m_elementsRect[i] = rect;
                            }
                            else
                            {
                                m_elementsRect.Add(rect);
                            }
                        }

                        //�����趨content����Ĵ�С��λ��
                        ResizeContent(ref m_contentSize, false);

                        //����contentλ�ã��������ֵ�ǰѡ�е����λ�ò������ı�
                        if (m_selectedElementIndex >= 0 && m_selectedElementIndex < m_elementAmount)
                        {
                            //stRect rect = m_elementsRect[m_selectedElementIndex];
                            //m_contentAnchoredPosition.x = m_selectedElementLastPositionInContent.x - rect.m_left;
                        }
                        else
                        {
                            //�������֮����Ҫ��չ��ĳ��ѡ�������contentλ�ã���֤�߽�
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
                            //Element����
                            float expandRate = 0;

                            if (i == m_selectedElementIndex)
                            {
                                expandRate = (m_timeSlice / m_expandedTime);
                                expandRate = Mathf.Clamp(expandRate, 0, 1);
                            }

                            stRect rect = LayoutExpandElement(i, expandRate, ref m_contentSize, ref offset);

                            //��¼Element Rect
                            if (i < m_elementsRect.Count)
                            {
                                m_elementsRect[i] = rect;
                            }
                            else
                            {
                                m_elementsRect.Add(rect);
                            }
                        }

                        //�����趨content����Ĵ�С��λ��
                        ResizeContent(ref m_contentSize, false);
                    }
                    else
                    {
                        m_selectingState = enExpandListSelectingState.None;
                    }
                }
                break;
            }

            //ͬ��elementλ��
            for (int i = 0; i < m_elementAmount; i++)
            {
                stRect rect = m_elementsRect[i];
                CUIListElementScript elementScript = GetElemenet(i);

                if (elementScript != null)
                {
                    elementScript.SetRect(ref rect);
                }
            }    

            //�ƶ�content
            m_contentRectTransform.anchoredPosition = m_contentAnchoredPosition;
        }

        //--------------------------------------------------
        /// ����ѡ��ȷ��content��Ŀ��λ��
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

            //�����content��Ŀ��λ��
            Vector2 targetContentAnchoredPosition = m_contentAnchoredPosition;

            if (m_listType == enUIListType.Horizontal)
            {
                //�����ұ߽�
                if (targetContentAnchoredPosition.x + rect.m_right > m_scrollAreaSize.x)
                {
                    targetContentAnchoredPosition.x = m_scrollAreaSize.x - rect.m_right;
                }

                //������߽�
                if (targetContentAnchoredPosition.x + rect.m_left < 0)
                {
                    targetContentAnchoredPosition.x = -rect.m_left;
                }
            }
            else if (m_listType == enUIListType.Vertical)
            {
                //�����±߽�
                if (targetContentAnchoredPosition.y + rect.m_bottom < -m_scrollAreaSize.y)
                {
                    targetContentAnchoredPosition.y = -m_scrollAreaSize.y - rect.m_bottom;
                }

                //�����ϱ߽�
                if (targetContentAnchoredPosition.y + rect.m_top > 0)
                {
                    targetContentAnchoredPosition.y = -rect.m_top;
                }
            }

/*
            //�����content��Ŀ��λ��
            Vector2 targetContentAnchoredPosition = m_contentAnchoredPosition;
            targetContentAnchoredPosition.x = -rect.m_left;

            //�ұ߽�����
            if (targetContentAnchoredPosition.x + m_contentSize.x + m_elementExpandedSize.x - m_elementDefaultSize.x < m_scrollAreaSize.x)
            {
                targetContentAnchoredPosition.x = m_scrollAreaSize.x - (m_contentSize.x + m_elementExpandedSize.x - m_elementDefaultSize.x);
            }
*/ 

            return targetContentAnchoredPosition;
        }
    };
};