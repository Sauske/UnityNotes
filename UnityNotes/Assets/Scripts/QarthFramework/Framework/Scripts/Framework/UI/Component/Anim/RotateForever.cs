using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using DG;
using DG.Tweening;

namespace Qarth
{
    public class RotateForever : MonoBehaviour
    {
        [SerializeField]
        private Transform m_object;
        [SerializeField]
        private float m_Duration = 5;
        [SerializeField]
        public bool clockwise = true;

        [SerializeField] public bool y_Axis = false;
        public void OnEnable()
        {
            StartAnim(m_object);
        }

        public void StartAnim(Transform m_Transform)
        {
            if (m_Transform == null)
            {
                m_Transform = this.transform;
            }
            if (clockwise)
                m_Transform.DORotate(new Vector3(0, y_Axis ? 360 : 0, y_Axis ? 0 : 360), m_Duration, RotateMode.LocalAxisAdd)
                    .SetLoops(-1)
                    .SetEase(Ease.Linear);
            else
                m_Transform.DORotate(new Vector3(0, y_Axis ? -360 : 0, y_Axis ? 0 : -360), m_Duration, RotateMode.LocalAxisAdd)
                  .SetLoops(-1)
                  .SetEase(Ease.Linear);
        }
    }
}