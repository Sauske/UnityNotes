using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UMI
{
    [AddComponentMenu("UI/Extensions/UIToggleEx")]
    [RequireComponent(typeof(RectTransform))]
    public class UIToggleEx : MonoBehaviour
    {
        public class ToggleEvent : UnityEvent<bool>
        { }
        public Button buttonOn;
        public Button buttonOff;
        [SerializeField]
        private UIToggleExGroup m_Group;

        public Image imgOn;
        public Image imgOff;
        //public ImageSetter imgSetterOn = new ImageSetter();
       // public ImageSetter imgSetterOff = new ImageSetter();

        public UIToggleExGroup group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
#if WINDOWS && DEBUG
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(m_Group, true);
                }
            }
        }

        [Header("只能被选中，而不能被取消选中")]
        public bool canOnlyOn = false;
        public ToggleEvent onValueChanged = new ToggleEvent();
        public Action action = null;
        // Whether the toggle is on
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;
        [SerializeField]
        private bool m_IsSetAsLast = false;

        protected UIToggleEx()
        { }
        void Awake()
        {
            buttonOn.onClick.RemoveAllListeners();
            buttonOn.onClick.AddListener(() =>
            {
                InternalToggle();
            });
            buttonOff.onClick.RemoveAllListeners();
            buttonOff.onClick.AddListener(() =>
            {
                InternalToggle();
            });
            RefreshButtonState();
        }
        void OnEnable()
        {
            SetToggleGroup(m_Group, false);
#if v_0_7_0
            if (imgOn != null)
            {
                imgSetterOn.Init(imgOn);
            }

            if (imgOff != null)
            {
                imgSetterOff.Init(imgOff);
            }
#endif
        }

        void OnDisable()
        {
            SetToggleGroup(null, false);
#if v_0_7_0
            if (imgOn != null)
            {
                imgSetterOn.Uninit();
            }

            if (imgOff != null)
            {
                imgSetterOff.Uninit();
            }
#endif
        }

        private void SetToggleGroup(UIToggleExGroup newGroup, bool setMemberValue)
        {
            UIToggleExGroup oldGroup = m_Group;

            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            if (setMemberValue)
                m_Group = newGroup;

            if (newGroup != null)
                newGroup.RegisterToggle(this);

            if (newGroup != null && newGroup != oldGroup && isOn)
                newGroup.NotifyToggleOn(this);
        }

        public bool isOn
        {
            get { return m_IsOn; }
            set
            {
                Set(value);
            }
        }

        public bool IsSetAsLast
        {
            get
            {
                return m_IsSetAsLast;
            }

            set
            {
                m_IsSetAsLast = value;
            }
        }

        void Set(bool value)
        {
            Set(value, true);
        }

        void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null)
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }
            RefreshButtonState();

            if (sendCallback)
            {
                if (action != null)
                {
                    action();
                    return;
                }
                onValueChanged.Invoke(m_IsOn);
                if (m_Group.TogCallBackDic.ContainsKey(this))
                {
                    m_Group.TogCallBackDic[this]();
                }
            }
        }
        private void RefreshButtonState()
        {
            buttonOn.gameObject.SetActive(m_IsOn);
            buttonOff.gameObject.SetActive(!m_IsOn);
            if (m_IsOn)
            {
                if (IsSetAsLast)
                {
                    transform.SetAsLastSibling();
                }
            }
        }

        private void InternalToggle()
        {
            
            // 点击只能被选中
            if (canOnlyOn && isOn) return;
            isOn = !isOn;
            if(isOn){
                if(m_Group.TogCallBackDic.ContainsKey(this)){
                    m_Group.TogCallBackDic[this]();
                }
            }
        }

        /// <summary>
        /// 设置去色
        /// </summary>
        public void SetGray(bool gray)
        {
            if (imgOn)
            {
                imgOn.color = gray ? Color.black : Color.white;
            }
            if (imgOff)
            {
                imgOff.color = gray ? Color.black : Color.white;
            }
            if (buttonOn)
            {
                Image imgOnBg = buttonOn.transform.GetComponent<Image>();
                if (imgOnBg)
                    imgOnBg.color = gray ? Color.black : Color.white;
            }
            if (buttonOff)
            {
                Image imgOffBg = buttonOff.transform.GetComponent<Image>();
                if (imgOffBg)
                    imgOffBg.color = gray ? Color.black : Color.white;
            }
        }



        private void OnDestroy()
        {
            clearEvents();
        }

        private void clearEvents()
        {
            buttonOn.onClick.RemoveAllListeners();
            buttonOff.onClick.RemoveAllListeners();
        }
    }
}
