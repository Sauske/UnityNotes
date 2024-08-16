using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public class Canvas3D : MonoBehaviour
    {
        public void Awake()
        {
            Reset();
        }

        public void Reset()
        {
            Canvas3DImpl.GetInstance().Reset();
        }

        public void LateUpdate()
        {
            Canvas3DImpl.GetInstance().Update(transform);
        }
    }
}
