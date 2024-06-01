using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuskeTest
{
    [ExecuteInEditMode]
    public class PointInSector : MonoBehaviour
    {       
        public Transform mTarget;

        Sector sector;
        public  Vector3 center = Vector3.zero;
        public Vector3 dir = new Vector3(8, 0, 0);
        public float radius = 3f;
        public float angle = 120;
        void Start()
        {
            sector = new Sector(center, dir, radius, angle);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRect();
        }

        private void OnDrawGizmos()
        {
            DrawSectorGizmo(center, radius, angle, dir);
        }

        private void UpdateRect()
        {
            bool inRect = sector.Contains(mTarget.position);
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

        private void DrawSectorGizmo(Vector3 origin, float radius, float angle, Vector3 direction)
        {
            float halfAngle = angle / 2f;
            Quaternion rotation = Quaternion.LookRotation(dir);

            Vector3 fromVector = rotation * Quaternion.Euler(0f, -halfAngle, 0f) * Vector3.forward;
            Vector3 toVector = rotation * Quaternion.Euler(0f, halfAngle, 0f) * Vector3.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, fromVector * radius);
            Gizmos.DrawRay(origin, toVector * radius);

            int segment = 128;
            float angleStep = angle / segment;

            for(int idx = 0; idx < segment;idx++)
            {
                float curAngle = -halfAngle + idx * angleStep;
                Vector3 curVector = rotation * Quaternion.Euler(0f, curAngle, 0f) * Vector3.forward;
                Gizmos.DrawLine(origin, origin + curVector * radius);
            }
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////             Sector              //////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// 

    public struct Sector
    {
        public Vector3 origin;   //扇形原点
        public Vector3 direction;//扇形方向
        public float radius;     //扇形半径
        public float angle;      //扇形角度

        public Sector(Vector3 _origin,Vector3 _dir, float _radius,float _angle)
        {
            this.origin = _origin;
            this.direction = _dir;
            this.radius = _radius;
            this.angle = _angle;
        }

        public bool Contains(Vector3 point)
        {
            Vector3 toPoint = point - origin;
            float distance = toPoint.magnitude;
            
            if (distance > radius) return false;

            Vector3 toPointNormalized = toPoint.normalized;
            float dot = Vector3.Dot(toPointNormalized, direction.normalized);
            float pointAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            float halfAngle = angle / 2f;

            if (pointAngle <= halfAngle) return true;

            return false;
        }
    }
}
