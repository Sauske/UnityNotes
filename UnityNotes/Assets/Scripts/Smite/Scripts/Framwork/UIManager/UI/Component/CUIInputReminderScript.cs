//==================================================================================
/// UI inputfield的输入字数提醒组件
/// 跟inputfield控件挂在同一个gameobject上
/// @lighthuang
/// @2015.07.03
//==================================================================================

using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class CUIInputReminderScript : CUIComponent
    {
        //计数类型
        public enum enCountType
        {
            CountDown,  //倒计数（还可输入字数）
            CountUp,    //正计数（当前字数）
        }

        //文本字符数的计数类型
        public enCountType m_countType;

        //用于显示提醒数字的Text控件
        public Text m_displayReminderText;

        //对应的inputfield控件
        private InputField m_inputField;

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            m_inputField = gameObject.GetComponent<InputField>();
#if UNITY_4_7
            m_inputField.onValueChange.RemoveAllListeners();
            m_inputField.onValueChange.AddListener(OnTextContentChanged);
#else
            m_inputField.onValueChanged.RemoveAllListeners();
            m_inputField.onValueChanged.AddListener(OnTextContentChanged);
#endif

            if (m_displayReminderText != null)
            {
                if (m_countType == enCountType.CountDown)
                {
                    m_displayReminderText.text = m_inputField.characterLimit.ToString();
                }
                else if (m_countType == enCountType.CountUp)
                {
                    m_displayReminderText.text = 0.ToString();
                }
            }

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_displayReminderText = null;
            m_inputField = null;

            base.OnDestroy();
        }

        private void OnTextContentChanged(string text)
        {
            if (m_displayReminderText == null)
            {
                return;
            }

            if (m_countType == enCountType.CountDown)
            {
                m_displayReminderText.text = (m_inputField.characterLimit - text.Length).ToString();
            }
            else if (m_countType == enCountType.CountUp)
            {
                m_displayReminderText.text = text.Length.ToString();
            }
        }
    };
};