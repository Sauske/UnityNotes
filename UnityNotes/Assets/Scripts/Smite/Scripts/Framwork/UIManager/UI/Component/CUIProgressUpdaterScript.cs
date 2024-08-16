
/// UI 进度更新器
/// 管理Filled类型的Image的fillAmount的更新
/// 和需要管理的Image放到同一gameObject中
/// @lighthuang
/// @2016.05.20
//==================================================================================

using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class CUIProgressUpdaterScript : CUIComponent
    {
        //填充方向
        public enum enFillDirection
        {
            Clockwise,              //顺时针
            CounterClockwise,       //逆时针
        }

        //填充方向
        public enFillDirection m_fillDirection = enFillDirection.Clockwise;

        //填充速率（例如1.0表示每秒填充完整个区域）
        public float m_fillAmountPerSecond = 1.0f;

        //填充结束事件ID
        [HideInInspector]
        public enUIEventID m_fillEndEventID;

        //填充到满时事件ID(fillAmount == endFillAmount)
        //note: 只适用于顺时针填充方向，逆时针方向需要另一个事件
        [HideInInspector]
        //public enUIEventID m_fillFullEventID;

        //目标填充量(0.0f到1.0f之间)
        private float m_targetFillAmount;

        //所管理的Image控件
        private Image m_image;

        //回绕计数（为1表示需要回绕，为0表示不需要回绕或者已经回绕）
        //private uint m_rewindCount;

        //是否正在更新
        private bool m_isRunning;

        //图片实际fill起始位置
        [Range(0.0f, 1.0f)]
        public float m_startFillAmount = 0.0f;
        //图片实际fill结束位置
        [Range(0.0f, 1.0f)]
        public float m_endFillAmount = 1.0f;
        //图片实际填充比例
        private float m_fillRate;


        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            m_image = gameObject.GetComponent<Image>();
            if (m_image == null)
            {
                DebugHelper.LogError("所挂的gameObject中没有Image组件");
                return;
            }

            if (m_image.type != Image.Type.Filled)
            {
                DebugHelper.LogError("Image的类型必须为Filled");
                return;
            }

            if (m_startFillAmount >= m_endFillAmount)
            {
                DebugHelper.LogError("开始填充量必须小于结束填充量");
            }

            m_fillRate = (m_endFillAmount - m_startFillAmount) / 1;

            m_isRunning = false;

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_image = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Update
        //--------------------------------------------------
        protected virtual void Update()
        {
            if (!m_isRunning || m_image == null || m_image.type != Image.Type.Filled)
            {
                return;
            }

            //需要确保每次给m_image.fillAmount赋的值都是经过fillRate换算过的
            if (m_fillDirection == enFillDirection.Clockwise)
            {
                float expiredFillAmount = m_image.fillAmount + m_fillAmountPerSecond * m_fillRate * Time.deltaTime;  //帧率无关
//                 if (m_rewindCount == 0)
//                 {
                    m_image.fillAmount = expiredFillAmount;   
                    if (m_image.fillAmount >= m_targetFillAmount)
                    {
                        m_isRunning = false;
                        DispatchFillEndEvent();
                    }
//                 }
//                 else
//                 {
//                     if (expiredFillAmount > m_endFillAmount)
//                     {
//                         //DispatchFillFullEvent();
// 
//                         m_rewindCount--;
//                         m_image.fillAmount = expiredFillAmount - m_endFillAmount + m_startFillAmount;
//                         if (m_image.fillAmount >= m_targetFillAmount)
//                         {
//                             m_isRunning = false;
//                             DispatchFillEndEvent();
//                         }
//                     }
//                     else
//                     {
//                         m_image.fillAmount = expiredFillAmount;
//                     }
//                 }
            }
            else if (m_fillDirection == enFillDirection.CounterClockwise)
            {
                float expiredFillAmount = m_image.fillAmount - m_fillAmountPerSecond * m_fillRate * Time.deltaTime;
//                 if (m_rewindCount == 0)
//                 {
                    m_image.fillAmount = expiredFillAmount;
                    if (m_image.fillAmount <= m_targetFillAmount)
                    {
                        m_isRunning = false;
                        DispatchFillEndEvent();
                    }
//                 }
//                 else
//                 {
//                     if (expiredFillAmount < m_startFillAmount)
//                     {
//                         m_rewindCount--;
//                         m_image.fillAmount = m_endFillAmount - m_startFillAmount + expiredFillAmount;
//                         if (m_image.fillAmount <= m_targetFillAmount)
//                         {
//                             m_isRunning = false;
//                             DispatchFillEndEvent();
//                         }
//                     }
//                     else
//                     {
//                         m_image.fillAmount = expiredFillAmount;
//                     }
//                 }
            }
        }

        /// <summary>
        /// 派发填充结束事件
        /// </summary>
        private void DispatchFillEndEvent()
        {
            if (m_fillEndEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_pointerEventData = null;
                uiEvent.m_eventID = m_fillEndEventID;
                uiEvent.m_eventParams = default(stUIEventParams);

                DispatchUIEvent(uiEvent);
            }
        }

        /// <summary>
        /// 开始填充
        /// </summary>
        /// <param name="targetFillAmount">目标填充量（范围0.0f到1.0f）</param>
        /// <param name="fillDirection">填充方向</param>
        /// <param name="curFillAmount">当前起始的填充量，若小于0则不设置</param>
        /// note: 调用方必须保证targetFillAmount是经过实际填充率转换过的
        public void StartFill(float targetFillAmount, enFillDirection fillDirection = enFillDirection.Clockwise, float curFillAmount = -1.0f)
        {
            m_targetFillAmount = Mathf.Clamp(targetFillAmount, m_startFillAmount, m_endFillAmount);
            m_fillDirection = fillDirection;

            if (curFillAmount >= 0)
            {
                m_image.fillAmount = curFillAmount;
            }

            //InitRewindCount();
            //m_rewindCount = rewindCount;
            m_isRunning = true;
        }

        /// <summary>
        /// 重置填充量
        /// </summary>
        public void ResetFillAmount()
        {
            if (m_image != null)
            {
                m_image.fillAmount = m_startFillAmount;
            }
        }

//         /// <summary>
//         /// 初始化回绕计数
//         /// </summary>
//         private void InitRewindCount()
//         {
//             if (m_fillDirection == enFillDirection.Clockwise && m_targetFillAmount < m_image.fillAmount)
//             {
//                 m_rewindCount = 1;
//             }
//             else if (m_fillDirection == enFillDirection.CounterClockwise && m_targetFillAmount > m_image.fillAmount)
//             {
//                 m_rewindCount = 1;
//             }
//             else
//             {
//                 m_rewindCount = 0;
//             }
//         }
    };
};