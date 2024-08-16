//====================================
/// UI Animation 控件
/// @neoyang
/// @2015.04.09
//====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Framework
{
    //Timer事件类型
    public enum enAnimationEventType
    {
        AnimationStart,
        AnimationEnd
    };

    public class CUIAnimationScript : CUIComponent
    {
        //Unity Animation
        private Animation m_animation;

        //当前正在播放的动画状态
        private AnimationState m_currentAnimationState;
        private float m_currentAnimationTime;

        //----------------------------
        /// 事件相关
        //----------------------------
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[System.Enum.GetValues(typeof(enAnimationEventType)).Length];

        //事件参数
        public stUIEventParams[] m_eventParams = new stUIEventParams[System.Enum.GetValues(typeof(enAnimationEventType)).Length];

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

            m_animation = this.gameObject.GetComponent<Animation>();
            if (m_animation != null)
            {
                if (m_animation.playAutomatically && m_animation.clip != null)
                {
                    m_currentAnimationState = m_animation[m_animation.clip.name];
                    m_currentAnimationTime = 0;

                    //派发事件
                    DispatchAnimationEvent(enAnimationEventType.AnimationStart);
                }
            }
        }

        //--------------------------------------------------
        /// 销毁
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
        /// 播放动画
        /// @animName
        /// @forceRewind : animName正在播放时，是否强制从头开始
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

                //派发事件
                DispatchAnimationEvent(enAnimationEventType.AnimationStart);
            }
        }

        /*
        //直接到动画最后一帧，用于恢复动画状态效果，但又不想播放过程
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
        /// 停止播放动画
        /// @animName
        //--------------------------------------------------
        public void StopAnimation(string animName)
        {
            if (m_currentAnimationState == null || !m_currentAnimationState.name.Equals(animName))
            {
                return;
            }

            m_animation.Stop(animName);

            //派发事件
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
        /// 返回当前播放的动画名
        //--------------------------------------------------
        public string GetCurrentAnimation()
        {
            return (m_currentAnimationState == null) ? null : m_currentAnimationState.name;
        }

        //--------------------------------------------------
        /// 指定动画是否处于停止播放状态
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
        /// 派发动画相关事件
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
        /// 设置动画事件
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
        /// 设置动画速度，1为正常速度
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