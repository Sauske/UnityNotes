//==================================================================================
/// UI Slider事件组件
/// 封装Slider控件的事件派发
/// @lighthuang
/// @2015.05.21
//==================================================================================

using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class CUISliderEventScript : CUIComponent
    {
        //--------------------------------------
        /// 事件相关数据
        //--------------------------------------
        [HideInInspector]
        public enUIEventID m_onValueChangedEventID;

        //相关Wwise音效
        public string[] m_onValueChangedWwiseEvents = new string[0];

        //对应的Slider控件
        private Slider m_slider;

        //高中低，等描述的个数
        private int m_DescribeCount;

        //描述文字数组
        private Text[] m_Describes;

        //滑块上的文本
        private Text m_Handletext;

        //当前值
        private float m_value;

        public float value
        {
            get
            {
                if (m_slider)
                {
                    return m_slider.value;
                }
                else 
                {
                    return -1;
                }
            }
            set
            {
                m_value = value;

                if (m_slider && m_value <= m_slider.maxValue && m_value >= 0)
                {
                    m_slider.value = m_value;
                    
                    m_Handletext.text = m_Describes[(int)m_value].text;
                }
            }
        }

        public int MaxValue
        {
            get
            {
                if (m_slider)
                {
                    return (int)m_slider.maxValue;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool Enabled
        {
            get
            {
                if (m_slider)
                {
                    return m_slider.interactable;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (m_slider)
                {
                    m_slider.interactable = value;
                }
            }
        }

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            //不是一个slider返回
            m_slider = gameObject.GetComponent<Slider>();
            if (m_slider == null)
                return;

            m_DescribeCount = transform.Find("Background").childCount;
            m_Describes = new Text[m_DescribeCount];
            for (int i = 0; i < m_DescribeCount; i++)
            {
                m_Describes[i] = transform.Find(string.Format("Background/Text{0}", i + 1)).GetComponent<Text>();
            }
            m_Handletext = transform.Find("Handle Slide Area/Handle/Text").GetComponent<Text>();

            m_slider.onValueChanged.RemoveAllListeners();
            m_slider.onValueChanged.AddListener(OnSliderValueChanged);

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_slider = null;
            m_Describes = null;
            m_Handletext = null;

            base.OnDestroy();
        }

        private void OnSliderValueChanged(float value)
        {
           
            if (value < 0 || value >= m_DescribeCount) return;
            this.value = value;

            //声音时间
            PostWwiseEvent(m_onValueChangedWwiseEvents);
            //分发事件
            DispatchSliderEvent();
        }

        private void DispatchSliderEvent()
        {
            if (m_onValueChangedEventID == enUIEventID.None)
            {
                return;
            }

            //分发事件
            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_srcFormScript = m_belongedFormScript;
            uiEvent.m_srcWidget = gameObject;
            uiEvent.m_srcWidgetScript = this;
            uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
            uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
            uiEvent.m_pointerEventData = null;

            uiEvent.m_eventID = m_onValueChangedEventID;
            uiEvent.m_eventParams.param1 = value;

            DispatchUIEvent(uiEvent);
        }

        private void PostWwiseEvent(string[] wwiseEvents)
        {
            for (int i = 0; i < wwiseEvents.Length; i++)
            {
                if (!string.IsNullOrEmpty(wwiseEvents[i]))
                {
                 //   CSoundManager.GetInstance().PostEvent(wwiseEvents[i]);
                }
            }
        }

        //--------------------------------------
        /// 对外接口
        //--------------------------------------
        public Slider GetSlider()
        {
            return m_slider;
        }

    };
};