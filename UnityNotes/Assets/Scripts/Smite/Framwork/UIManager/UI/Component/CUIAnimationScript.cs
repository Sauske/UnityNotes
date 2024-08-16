//====================================
/// UI Animation �ؼ�
/// @neoyang
/// @2015.04.09
//====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Framework
{
    //Timer�¼�����
    public enum enAnimationEventType
    {
        AnimationStart,
        AnimationEnd
    };

    public class CUIAnimationScript : CUIComponent
    {
        //Unity Animation
        private Animation m_animation;

        //��ǰ���ڲ��ŵĶ���״̬
        private AnimationState m_currentAnimationState;
        private float m_currentAnimationTime;

        //----------------------------
        /// �¼����
        //----------------------------
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[System.Enum.GetValues(typeof(enAnimationEventType)).Length];

        //�¼�����
        public stUIEventParams[] m_eventParams = new stUIEventParams[System.Enum.GetValues(typeof(enAnimationEventType)).Length];

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

            m_animation = this.gameObject.GetComponent<Animation>();
            if (m_animation != null)
            {
                if (m_animation.playAutomatically && m_animation.clip != null)
                {
                    m_currentAnimationState = m_animation[m_animation.clip.name];
                    m_currentAnimationTime = 0;

                    //�ɷ��¼�
                    DispatchAnimationEvent(enAnimationEventType.AnimationStart);
                }
            }
        }

        //--------------------------------------------------
        /// ����
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_animation = null;
            m_currentAnimationState = null;

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

            if (m_currentAnimationState == null)
            {
                return;
            }

            if (m_currentAnimationState.wrapMode != WrapMode.Loop
            && m_currentAnimationState.wrapMode != WrapMode.PingPong
            && m_currentAnimationState.wrapMode != WrapMode.ClampForever
            )
            {
                if (m_currentAnimationTime > m_currentAnimationState.length)
                {
                    m_currentAnimationState = null;
                    m_currentAnimationTime = 0;

                    DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
                    //m_animation.Stop();
                }
                else
                {
                    m_currentAnimationTime += Time.deltaTime;
                }
            }
        }

        public bool IsHaveAnimation(string aniName)
        {
            if (m_animation == null)
            {
                return false;
            }

            if (m_animation[aniName] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //--------------------------------------------------
        /// ���Ŷ���
        /// @animName
        /// @forceRewind : animName���ڲ���ʱ���Ƿ�ǿ�ƴ�ͷ��ʼ
        //--------------------------------------------------
        public void PlayAnimation(string animName, bool forceRewind)
        {
            if (m_currentAnimationState != null && m_currentAnimationState.name.Equals(animName) && !forceRewind)
            {
                return;
            }

            if (m_currentAnimationState != null)
            {
                m_animation.Stop(m_currentAnimationState.name);
                m_currentAnimationState = null;
                m_currentAnimationTime = 0;
            }

            m_currentAnimationState = m_animation[animName];
            m_currentAnimationTime = 0;

            if (m_currentAnimationState != null)
            {
                //m_currentAnimationState.time = 0;
                m_animation.Play(animName);                

                //�ɷ��¼�
                DispatchAnimationEvent(enAnimationEventType.AnimationStart);
            }
        }

        /*
        //ֱ�ӵ��������һ֡�����ڻָ�����״̬Ч�������ֲ��벥�Ź���
        public void GotoAnimationEnd(string animName)
        {
            AnimationState aniState = m_animation[animName];

            if (aniState == null)
            {
                return;
            }

            aniState.time = aniState.clip.length;
            m_animation.Play(animName);
            //m_animation.Stop(animName);
        }
        */

        //--------------------------------------------------
        /// ֹͣ���Ŷ���
        /// @animName
        //--------------------------------------------------
        public void StopAnimation(string animName)
        {
            if (m_currentAnimationState == null || !m_currentAnimationState.name.Equals(animName))
            {
                return;
            }

            m_animation.Stop(animName);

            //�ɷ��¼�
            DispatchAnimationEvent(enAnimationEventType.AnimationEnd);

            m_currentAnimationState = null;
            m_currentAnimationTime = 0;
        }

        public void StopAnimation()
        {
            if (m_animation != null)
            {
                m_animation.Stop();
            }
        }

        //--------------------------------------------------
        /// ���ص�ǰ���ŵĶ�����
        //--------------------------------------------------
        public string GetCurrentAnimation()
        {
            return (m_currentAnimationState == null) ? null : m_currentAnimationState.name;
        }

        //--------------------------------------------------
        /// ָ�������Ƿ���ֹͣ����״̬
        /// @animationName
        //--------------------------------------------------
        public bool IsAnimationStopped(string animationName)
        {
            if (string.IsNullOrEmpty(animationName) || m_currentAnimationState == null || m_currentAnimationTime == 0)
            {
                return true;
            }

            return (!string.Equals(m_currentAnimationState.name, animationName));
        }

        //--------------------------------------------------
        /// �ɷ���������¼�
        /// @animationEventType
        //--------------------------------------------------
        public void DispatchAnimationEvent(enAnimationEventType animationEventType)
        {
            if (m_eventIDs[(int)animationEventType] == enUIEventID.None)
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
            uiEvent.m_eventID = m_eventIDs[(int)animationEventType];
            uiEvent.m_eventParams = m_eventParams[(int)animationEventType];

            DispatchUIEvent(uiEvent);                 
        }

        /// <summary>
        /// ���ö����¼�
        /// </summary>
        /// <param name="animationEventType"></param>
        /// <param name="eventId"></param>
        /// <param name="eventParams"></param>
        public void SetAnimationEvent(enAnimationEventType animationEventType, enUIEventID eventId, stUIEventParams eventParams = default(stUIEventParams))
        {
            m_eventIDs[(int)animationEventType] = eventId;
            m_eventParams[(int)animationEventType] = eventParams;
        }

        /// <summary>
        /// ���ö����ٶȣ�1Ϊ�����ٶ�
        /// </summary>
        /// <param name="animName"></param>
        /// <param name="speed"></param>
        public void SetAnimationSpeed(string animName, float speed)
        {
            if (m_animation != null && m_animation[animName] != null)
            {
                m_animation[animName].speed = speed;
            }
        }
    };
};