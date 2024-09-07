using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UMI
{
    public class InputFieldComponent : MonoBehaviour
    {
        private TMP_InputField input;

        void Start()
        {
            input = GetComponent<TMP_InputField>();
            if (input != null)
            {
                input.onValueChanged.AddListener(OnSubmit);
            }
        }

        private void OnSubmit(string value)
        {
            if(!string.IsNullOrEmpty(value))
            {                
                input.text = SensitiveWordMgr.Instance.ProssSensitiveWords(value);
            }
        }
    }
}
