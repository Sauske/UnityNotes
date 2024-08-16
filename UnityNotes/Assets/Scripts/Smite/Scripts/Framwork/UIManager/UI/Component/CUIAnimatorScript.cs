//====================================
/// CUIAnimatorScript �ؼ�
/// @royjin
/// @2015.05.08
//====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Framework
{
    //Timer�¼�����
    public enum enAnimatorEventType
    {
        AnimatorStart,
        AnimatorEnd
    };

    public class CUIAnimatorScript : CUIComponent
    {
        //Unity Animator
        private Animator m_animator;

        //���ڲ��ŵ�Animator State ��Ϣ
        public string m_currentAnimatorStateName
        {
            get;
            private set;
        }
        private int m_currentAnimatorStateCounter;


   //     private bool m_isEnableBeHide = true;             //������ǰ�Լ��Ƿ�����
     //   private bool m_isEnableAnimatorBeHide = true;     //������ǰAnimator�Ƿ�����

        //----------------------------
        /// �¼����
        //----------------------------
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[System.Enum.GetValues(typeof(enAnimatorEventType)).Length];

        //�¼�����
        public stUIEventParams[] m_eventParams = new stUIEventParams[System.Enum.GetValues(typeof(enAnimatorEventType)).Length];

        //�����¼�����
        public void SetUIEvent(enAnimatorEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
        {
            m_eventIDs[(int)eventType] = eventID;
            m_eventParams[(int)eventType] = eventParams;
        }

        //--------------------------------------------------
        /// ��ʼ��
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
        /// ����
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

            //normalizedTime������λ��ʾ�ڼ��β��ţ�С��λ��ʾ���Ž���(����ֵ�animator��ѭ�����ţ�������ֻ᲻������)
            //if (false)//((m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 1.0f && !m_animator.IsInTransition(0))
            if ((int)m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > m_currentAnimatorStateCounter)
            {
                m_animator.StopPlayback();

                string tempStr = m_currentAnimatorStateName;
                m_currentAnimatorStateName = null;
                m_currentAnimatorStateCounter = 0;

                //�ɷ��¼�
                DispatchAnimatorEvent(enAnimatorEventType.AnimatorEnd, tempStr);


                /*
                //���������ʡ����
                if (m_animator.enabled == true)
                {
                    m_animator.enabled = false;
                }

                this.enabled = false;
                */
            }
        }

        //--------------------------------------------------
        /// ���Ŷ���
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

            //Ϊ����ȷ����[animator.GetCurrentAnimatorStateInfo(0)]������Ҫ����Update
            m_animator.Update(0);
            m_animator.Update(0);

            //��¼normalizedTime
            m_currentAnimatorStateCounter = (int)m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            //�ɷ��¼�
            DispatchAnimatorEvent(enAnimatorEventType.AnimatorStart, m_currentAnimatorStateName);
        }

        //--------------------------------------------------
        /// ���ö�������ֵ
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
        /// ���ö�������ֵ
        /// @name
        /// @value
        //--------------------------------------------------
        public void SetInteger(string name, int value)
        {
            m_animator.SetInteger(name, value);
        }

        //--------------------------------------------------
        /// ֹͣ���Ŷ���
        //--------------------------------------------------
        public void StopAnimator()
        {
            //m_animator.StopPlayback();
        }

        //--------------------------------------------------
        /// ָ�������Ƿ���ֹͣ����״̬
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
        /// �ɷ���������¼�
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