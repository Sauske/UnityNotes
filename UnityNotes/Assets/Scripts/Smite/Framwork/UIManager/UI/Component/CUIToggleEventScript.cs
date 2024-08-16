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
    public class CUIToggleEventScript : CUIComponent
    {
        //--------------------------------------
        /// 事件相关数据
        //--------------------------------------
        [HideInInspector]
        public enUIEventID m_onValueChangedEventID;

        //对应的Slider控件
        private Toggle m_toggle;

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            m_toggle = gameObject.GetComponent<Toggle>();
            m_toggle.onValueChanged.RemoveAllListeners();
            m_toggle.onValueChanged.AddListener(OnToggleValueChanged);
            var txtTrans = gameObject.transform.Find("Label");
            if (txtTrans != null)
            {
                if (m_toggle.isOn)
                {
                    txtTrans.GetComponent<Text>().color = CUIUtility.s_Color_White;
                }
                else
                {
                    txtTrans.GetComponent<Text>().color = CUIUtility.s_Text_Color_ListElement_Normal;
                }
            }

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_toggle = null;

            base.OnDestroy();
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (m_onValueChangedEventID == enUIEventID.None)
            {
                return;
            }

            CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

            uiEvent.m_srcFormScript = m_belongedFormScript;
            uiEvent.m_srcWidget = gameObject;
            uiEvent.m_srcWidgetScript = this;
            uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
            uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
            uiEvent.m_pointerEventData = null;

            uiEvent.m_eventID = m_onValueChangedEventID;
            uiEvent.m_eventParams.param1 = isOn;

            var txtTrans = gameObject.transform.Find("Label");
            if (txtTrans != null)
            {
                if (isOn)
                {
                    txtTrans.GetComponent<Text>().color = CUIUtility.s_Color_White;
                }
                else
                {
                    txtTrans.GetComponent<Text>().color = CUIUtility.s_Text_Color_ListElement_Normal;
                }
            }
            DispatchUIEvent(uiEvent);
        }
    };
};