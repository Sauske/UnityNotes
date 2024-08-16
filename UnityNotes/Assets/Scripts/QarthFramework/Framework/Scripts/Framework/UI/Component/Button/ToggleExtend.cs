using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using UnityEngine.UI;

namespace Qarth
{
    public class ToggleExtend : MonoBehaviour
    {
        [SerializeField]
        private Toggle m_Toggle;
        [SerializeField]
        private GameObject m_EnableStateRoot;
        [SerializeField]
        private GameObject m_DisableStateRoot;

        public Toggle toggle
        {
            get => m_Toggle;
            set => m_Toggle = value;
        }

        public void SetEnableStateRoot(GameObject go)
        {
            //if (m_EnableStateRoot.activeSelf != toggle.isOn)
            {
                m_EnableStateRoot.SetActive(false);
                go.SetActive(toggle.isOn);
            }
            m_EnableStateRoot = go;
        }
        public void SetDisableStateRoot(GameObject go)
        {
            //if (m_DisableStateRoot.activeSelf == toggle.isOn)
            {
                m_DisableStateRoot.SetActive(false);
                go.SetActive(!toggle.isOn);
            }
            m_DisableStateRoot = go;
        }

        private void Awake()
        {
            if (m_Toggle == null)
            {
                m_Toggle = GetComponent<Toggle>();
            }

            if (m_Toggle == null)
            {
                return;
            }

            m_Toggle.onValueChanged.AddListener(OnToggleValueChange);

            OnToggleValueChange(m_Toggle.isOn);
        }

        private void OnToggleValueChange(bool value)
        {
            if (m_EnableStateRoot != null)
            {
                m_EnableStateRoot.SetActive(value);
            }

            if (m_DisableStateRoot != null)
            {
                m_DisableStateRoot.SetActive(!value);
            }
        }
    }
}
