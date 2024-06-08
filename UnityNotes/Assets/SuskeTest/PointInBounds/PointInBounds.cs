using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuskeTest
{
    public class PointInBounds : MonoBehaviour
    {
        public Bounds bounds;
        public BoundingSphere bs;
        public Transform mTarget;

        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(8, 8, 8);
        void Start()
        {

            bounds = new Bounds(center, size);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRect();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(center, size);                 
        }

        private void UpdateRect()
        {
            bool inRect = bounds.Contains(mTarget.position);
            if (inRect)
            {
                RectLog(mTarget.position);
            }
        }

        private Vector3 mTempV;
        private void RectLog(Vector3 v)
        {
            if (mTempV != v)
            {
                Debug.Log(v);
                mTempV = v;
            }
        }
    }
}
