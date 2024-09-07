using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UMI
{
    /// <summary>
    /// 文本本地化控件
    /// 通过文本id和语言类型，读取对应的文本列和显示
    /// </summary>
    public class UILocalizationText : MonoBehaviour
    {
        public int TextId;      // 国际化表的文本id

        private void Awake()
        {
           // LocalizationMgr.Instance.AddComponent(this);
        }

        private void Start()
        {
            ChangeText(TextId);
        }

        private void OnDestroy()
        {
           // LocalizationMgr.Instance.RemoveComponent(this);
        }

        public void ChangeText()
        {
            if (TextId != 0)
            {
                ChangeText(TextId);
            }
        }

        /// <summary>
        /// 更换文本内容
        /// </summary>
        /// <param name="textId">文本id</param>
        public void ChangeText(int textId)
        {
            TextId = textId;
            
            TextMeshProUGUI label = GetComponent<TextMeshProUGUI>();
            if (label != null)
            {
                var strText = "LocalizationMgr.Instance";// LocalizationMgr.Instance.GetContent(textId);
                if (!string.IsNullOrEmpty(strText))
                {
                    label.text = strText;
                }
                return;
            }
            
            Text label2 = GetComponent<Text>();
            if (label2 != null)
            {
                var strText = "LocalizationMgr.Instance";// LocalizationMgr.Instance.GetContent(textId);
                if (!string.IsNullOrEmpty(strText))
                {
                    label2.text = strText;
                }
            }
        }
    }
}
