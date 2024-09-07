using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UMI
{
    public class ButtonAudioComponent : MonoBehaviour
    {
        public string audioName = "btn_click";

        private Button button;

        void Start()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnPlayAudio);
            }
        }

        private void OnPlayAudio()
        {
            if (!string.IsNullOrEmpty(audioName))
            {
               // AudioMgr.Instance.PlayAudio(eAudioType.UISound, audioName);
            }
        }
    }
}
