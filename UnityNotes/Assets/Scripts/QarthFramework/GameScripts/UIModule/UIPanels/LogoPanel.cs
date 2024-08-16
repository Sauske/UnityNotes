using System;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;
using UnityEngine.UI;

namespace MainGame
{
    public class LogoPanel : AbstractPanel
    {
        [SerializeField] private Image m_FillImg;

        private bool m_Refresh;
        private float m_FillAmount;

        protected override void OnUIInit()
        {
            base.OnUIInit();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            m_Refresh = true;
        }

        protected override void OnClose()
        {
            base.OnClose();
            m_Refresh = false;
        }

        private void Update()
        {
            if (m_Refresh)
            {
                if (m_FillAmount < 1)
                {
                    m_FillAmount += 0.02f;
                }

                m_FillImg.fillAmount = m_FillAmount;
            }
        }
    }
}