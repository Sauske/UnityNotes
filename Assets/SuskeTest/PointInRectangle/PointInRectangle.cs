using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuskeTest
{
    public class PointInRectangle : MonoBehaviour
    {
        public RectInt rectangle;
        public Transform mTarget;
        void Start()
        {
            rectangle = new RectInt(0, 0, 8, 8);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRect();
        }

        private void OnDrawGizmos()
        {
            Vector3 center = new Vector3(rectangle.x, rectangle.y, 0);
            Vector3 size = new Vector3(rectangle.width, rectangle.height, 2);
            Gizmos.DrawCube(center, size);
        }

        private void UpdateRect()
        {
            Vector2Int pos = new Vector2Int((int)mTarget.position.x, (int)mTarget.position.y);
            bool inRect = rectangle.Contains(pos);
            if (inRect)
            {
                RectLog(pos);
            }
        }

        private Vector2Int mTempV;
        private void RectLog(Vector2Int v)
        {
            if (mTempV != v)
            {
                Debug.Log(v);
                mTempV = v;
            }
        }
    }
}
