//==================================================================================
/// UI 自动滚动组件
/// @lighthuang
/// @2015.08.17
//==================================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class CUIAutoScroller : CUIComponent
    {
        //自动滚动组件依赖的恒定目标帧率
        private const int TargetFrameRate = 30;

        //滚动内容对象（一般为Text对象，也可以是其他的控件对象）
        //必须是AutoScroller所在节点的子节点
        public GameObject m_content;

        //滚动速度
        public int m_scrollSpeed = 1;
        //循环次数
        public int m_loop = 1;
        //循环技术
        private int m_loopCnt = 0;

        //滚动内容的rect transform
        private RectTransform m_contentRectTransform;

        //是否正在滚动
        private bool m_isScrollRunning = false;

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            if (m_content != null)
            {
                m_contentRectTransform = m_content.transform as RectTransform;
            }

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_content = null;
            m_contentRectTransform = null;

            base.OnDestroy();
        }

        //--------------------------------------
        /// 开始滚动
        //--------------------------------------
        public void StartAutoScroll(bool bForce = false)
        {
            if (!bForce && m_isScrollRunning)
            {
                return;
            }
            m_loopCnt = m_loop;
            m_isScrollRunning = true;

            ResetContentTransform();

            StartCoroutine("UpdateScroll");
        }

        //--------------------------------------
        /// 停止滚动
        //--------------------------------------
        public void StopAutoScroll()
        {
            if (!m_isScrollRunning)
            {
                return;
            }
            m_isScrollRunning = false;

            StopCoroutine("UpdateScroll");

            ResetContentTransform();
        }

        /// <summary>
        /// 是否正在滚动
        /// </summary>
        /// <returns></returns>
        public bool IsScrollRunning()
        {
            return m_isScrollRunning;
        }

        /// <summary>
        /// 设置文本（若滚动控件是Text）
        /// </summary>
        /// <param name="contentText"></param>
        public void SetText(string contentText)
        {
            if (m_content == null)
            {
                return;
            }

            Text textComponent = m_content.GetComponent<Text>();
            if (textComponent == null)
            {
                return;
            }

            textComponent.text = contentText;
        }

        //--------------------------------------
        /// 重置滚动内容transform
        //--------------------------------------
        private void ResetContentTransform()
        {
            if (m_contentRectTransform == null)
            {
                return;
            }

            //锚点和枢轴点都设为左中点，后续需要可以增加参数来定制对齐方式
            m_contentRectTransform.pivot = new Vector2(0, 0.5f);
            m_contentRectTransform.anchorMin = new Vector2(0, 0.5f);
            m_contentRectTransform.anchorMax = new Vector2(0, 0.5f);

            //初始用自己的左中点对到父节点的右中点
            m_contentRectTransform.anchoredPosition = new Vector2((transform as RectTransform).rect.width, 0);
        }

        /// <summary>
        /// 滚动更新
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateScroll()
        {
            if (m_contentRectTransform != null)
            {
                //在区域内则滚动
                while (m_contentRectTransform.anchoredPosition.x > -m_contentRectTransform.rect.width)
                {
                    //默认从右向左滚动，后续需要可以增加参数来定制方向（这里模拟了不随帧率变化的恒定横向速度）
                    m_contentRectTransform.anchoredPosition = new Vector2(m_contentRectTransform.anchoredPosition.x - Time.deltaTime * TargetFrameRate * m_scrollSpeed, m_contentRectTransform.anchoredPosition.y);
                    //DebugHelper.Log("Time.deltaTime=" + Time.deltaTime + ", targetFrameRate=" + Application.targetFrameRate);

                    if (m_contentRectTransform.anchoredPosition.x <= -m_contentRectTransform.rect.width) //一圈跑完了
                    {
                        if (m_loopCnt > 0)
                        {
                            m_loopCnt--;
                        }
                        if (m_loopCnt != 0)
                        {
                            ResetContentTransform();
                        }
                    }
                    yield return null;
                }

                m_isScrollRunning = false;

                DispatchScrollFinishEvent();
            }
        }

        //private void Update()
        //{
        //    if (m_isScrollRunning && m_contentRectTransform != null)
        //    {
        //        if (m_contentRectTransform.anchoredPosition.x > -m_contentRectTransform.rect.width)
        //        {
        //            //默认从右向左滚动，后续需要可以增加参数来定制方向（这里模拟了不随帧率变化的恒定横向速度）
        //            m_contentRectTransform.anchoredPosition = new Vector2(m_contentRectTransform.anchoredPosition.x - Time.deltaTime * TargetFrameRate * m_scrollSpeed, m_contentRectTransform.anchoredPosition.y);
        //            //DebugHelper.Log("Time.deltaTime=" + Time.deltaTime + ", targetFrameRate=" + Application.targetFrameRate);
        //        }
        //        else
        //        {
        //            if (m_loop > 0)
        //            {
        //                m_loop--;
        //            }
        //            if(m_loop == 0)
        //            {
        //                m_isScrollRunning = false;
        //                DispatchScrollFinishEvent();
        //            }
        //            else
        //            {
        //                ResetContentTransform();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 派发滚动结束事件
        /// </summary>
        private void DispatchScrollFinishEvent()
        {
            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();
            uiEvent.m_eventID = enUIEventID.UIComponent_AutoScroller_Scroll_Finish;
            uiEvent.m_srcFormScript = m_belongedFormScript;
            uiEvent.m_srcWidget = gameObject;
            uiEvent.m_srcWidgetScript = this;

            CUIEventManager.GetInstance().DispatchUIEvent(uiEvent);
        }
    };
};