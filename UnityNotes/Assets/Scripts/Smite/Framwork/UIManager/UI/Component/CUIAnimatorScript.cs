//====================================
/// CUIAnimatorScript 控件
/// @royjin
/// @2015.05.08
//====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Framework
{
    //Timer事件类型
    public enum enAnimatorEventType
    {
        AnimatorStart,
        AnimatorEnd
    };

    public class CUIAnimatorScript : CUIComponent
    {
        //Unity Animator
        private Animator m_animator;

        //正在播放的Animator State 信息
        public string m_currentAnimatorStateName
        {
            get;
            private set;
        }
        private int m_currentAnimatorStateCounter;


   //     private bool m_isEnableBeHide = true;             //被隐藏前自己是否启用
     //   private bool m_isEnableAnimatorBeHide = true;     //被隐藏前Animator是否启用

        //----------------------------
        /// 事件相关
        //----------------------------
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[System.Enum.GetValues(typeof(enAnimatorEventType)).Length];

        //事件参数
        public stUIEventParams[] m_eventParams = new stUIEventParams[System.Enum.GetValues(typeof(enAnimatorEventType)).Length];

        //设置事件参数
        public void SetUIEvent(enAnimatorEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
        {
            m_eventIDs[(int)eventType] = eventID;
            m_eventParams[(int)eventType] = eventParams;
        }

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

            m_animator = this.gameObject.GetComponent<Animator>();

         //   m_isEnableBeHide = this.enabled;
         //   m_isEnableAnimatorBeHide = m_animator.enabled;
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_animator = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Update
        //--------------------------------------------------
        void Update()
        {
            if (m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            {
                return;
            }

            if (m_currentAnimatorStateName == null)
            {
                return;
            }

            //normalizedTime的整数位表示第几次播放，小数位表示播放进度(很奇怪的animator会循环播放，这个数字会不断增大)
            //if (false)//((m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 1.0f && !m_animator.IsInTransition(0))
            if ((int)m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > m_currentAnimatorStateCounter)
            {
                m_animator.StopPlayback();

                string tempStr = m_currentAnimatorStateName;
                m_currentAnimatorStateName = null;
                m_currentAnimatorStateCounter = 0;

                //派发事件
                DispatchAnimatorEvent(enAnimatorEventType.AnimatorEnd, tempStr);


                /*
                //禁用组件节省开销
                if (m_animator.enabled == true)
                {
                    m_animator.enabled = false;
                }

                this.enabled = false;
                */
            }
        }

        //--------------------------------------------------
        /// 播放动画
        /// @stateName
        //--------------------------------------------------
        public void PlayAnimator(string stateName)
        {
            // Initialize not called
            if (m_animator == null)
            {
                m_animator = this.gameObject.GetComponent<Animator>();
            }

			if (!m_animator.enabled)
			{
				m_animator.enabled = true;
			}

            m_animator.Play(stateName, 0, 0f);
            m_currentAnimatorStateName = stateName;

            //为了正确调用[animator.GetCurrentAnimatorStateInfo(0)]，必须要两次Update
            m_animator.Update(0);
            m_animator.Update(0);

            //记录normalizedTime
            m_currentAnimatorStateCounter = (int)m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            //派发事件
            DispatchAnimatorEvent(enAnimatorEventType.AnimatorStart, m_currentAnimatorStateName);
        }

        //--------------------------------------------------
        /// 设置动画条件值
        /// @name
        /// @value
        //--------------------------------------------------
        public void SetBool(string name, bool value)
        {
            m_animator.SetBool(name, value);
        }

        public void SetAnimatorEnable(bool isEnable)
        {
            if (m_animator)
            {
                m_animator.enabled = isEnable;
                this.enabled = isEnable;
            }
        }

        //--------------------------------------------------
        /// 设置动画条件值
        /// @name
        /// @value
        //--------------------------------------------------
        public void SetInteger(string name, int value)
        {
            m_animator.SetInteger(name, value);
        }

        //--------------------------------------------------
        /// 停止播放动画
        //--------------------------------------------------
        public void StopAnimator()
        {
            //m_animator.StopPlayback();
        }

        //--------------------------------------------------
        /// 指定动画是否处于停止播放状态
        /// @animationName
        //--------------------------------------------------
        public bool IsAnimationStopped(string animationName)
        {
            if (string.IsNullOrEmpty(animationName))
            {
                return true;
            }

            return (!string.Equals(m_currentAnimatorStateName, animationName));
        }

        //--------------------------------------------------
        /// 派发动画相关事件
        /// @animationEventType
        //--------------------------------------------------
        private void DispatchAnimatorEvent(enAnimatorEventType animatorEventType, string stateName)
        {
            if (m_eventIDs[(int)animatorEventType] == enUIEventID.None)
            {
                return;
            }

            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_srcFormScript = m_belongedFormScript;
            uiEvent.m_srcWidget = this.gameObject;
            uiEvent.m_srcWidgetScript = this;
            uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
            uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
            uiEvent.m_pointerEventData = null;
            uiEvent.m_eventID = m_eventIDs[(int)animatorEventType];
            uiEvent.m_eventParams = m_eventParams[(int)animatorEventType];
            uiEvent.m_eventParams.param1 = stateName;

            DispatchUIEvent(uiEvent);
        }

        /*
        --------------------------------------------------
         Hide
        --------------------------------------------------
        public override void Hide()
        {
            base.Hide();

            m_isEnableBeHide = this.enabled;
            m_isEnableAnimatorBeHide = m_animator.enabled;

            this.enabled = false;
            this.m_animator.enabled = false;
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void Appear()
        {
            base.Appear();

            this.enabled = m_isEnableBeHide;
            this.m_animator.enabled = m_isEnableAnimatorBeHide;
        }
        */
    };
};