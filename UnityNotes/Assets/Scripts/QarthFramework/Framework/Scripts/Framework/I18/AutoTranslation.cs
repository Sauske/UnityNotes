using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Qarth
{
    public class AutoTranslation : MonoBehaviour
    {
        public enum TransTxtPostType
        {
            None,
            ToUpper,
            ToLower,
            Capital1st,
            Capital1stAny,
        }

        [SerializeField] protected string m_Key;
        [SerializeField] protected Text m_Text;
        [SerializeField] protected TransTxtPostType m_Type;

        protected object[] m_Value;

        public string sKey
        {
            get
            {
                return m_Key;
            }
        }

        public static void ReTranslationAll()
        {
            AutoTranslation[] coms = UIMgr.S.uiRoot.GetComponentsInChildren<AutoTranslation>();
            if (coms != null && coms.Length > 0)
            {
                for (int i = 0; i < coms.Length; ++i)
                {
                    coms[i].Translate();
                }
            }
        }

        private void Awake()
        {
            Translate();
        }

        public void SetKey(string key)
        {
            m_Key = key;
            Translate();
        }

        public void SetKeyValue(string key, object[] value)
        {
            m_Key = key;
            m_Value = value;
            Translate();
        }
        public virtual void Translate()
        {
            if (m_Text == null)
            {
                m_Text = GetComponent<Text>();
            }

            if (m_Text == null)
            {
                Log.e("Not Find Text Componment On:" + gameObject.name);
                return;
            }
            if (m_Value != null)
            {
                m_Text.text = string.Format(TDLanguageTable.Get(m_Key), m_Value);
            }
            else
            {
                m_Text.text = TDLanguageTable.Get(m_Key);
            }

            switch (m_Type)
            {
                case TransTxtPostType.ToLower:
                    m_Text.text = m_Text.text.ToLower();
                    break;
                case TransTxtPostType.ToUpper:
                    m_Text.text = m_Text.text.ToUpper();
                    break;
                case TransTxtPostType.Capital1st:
                    m_Text.text = string.Format("{0}{1}", m_Text.text[0].ToString().ToUpper(), m_Text.text.Substring(1));
                    break;
                case TransTxtPostType.Capital1stAny:
                    m_Text.text = Regex.Replace(m_Text.text, "^[a-z]", m => m.Value.ToUpper());
                    break;
                case TransTxtPostType.None:
                    break;
            }
        }
    }
}
