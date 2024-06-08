using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuskeTest
{
    public class ClosestPointToIrregularRegion : MonoBehaviour
    {

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
              //  navMeshSurface.BuildNavMesh(); // 重新构建NavMesh
            }
        }

        private void OnDrawGizmos()
        {
           // Gizmos.DrawCube(center, size);                 
        }

        private void UpdateRect()
        {
            //bool inRect = bounds.Contains(mTarget.position);
            //if (inRect)
            //{
            //    RectLog(mTarget.position);
            //}
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
