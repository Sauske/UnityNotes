
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UMI
{
    [ExecuteInEditMode]
    public class GamePlayTriggerCfg : MonoBehaviour
    {
        public float cubeSize = 2f;

        public List<Transform> CubePointList = new List<Transform>();

        public List<Transform> RandomList = new List<Transform>();

        public int RandomNumber = 4;

        public float GapDistance = 1f;

        public void Awake()
        {

            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }


        private void Refresh()
        {
            var a = this.transform.position + Vector3.forward * cubeSize * 0.5f;
            var b = this.transform.position + Vector3.right * cubeSize * 0.5f;
            var c = this.transform.position - Vector3.forward * cubeSize * 0.5f;
            var d = this.transform.position - Vector3.right * cubeSize * 0.5f;

            if (CubePointList.Count == 0)
            {
                var A = new GameObject("a").transform;
                A.parent = this.transform;
                CubePointList.Add(A);
                var B = new GameObject("b").transform;
                B.parent = this.transform;
                CubePointList.Add(B);
                var C = new GameObject("c").transform;
                C.parent = this.transform;
                CubePointList.Add(C);
                var D = new GameObject("d").transform;
                D.parent = this.transform;
                CubePointList.Add(D);
            }


            if (CubePointList.Count != 4)
            {
                return;
            }
            else
            {
                bool hasValue = false;
                foreach (var t in CubePointList)
                {
                    if (Vector3.Distance(t.position, Vector3.zero) > 0.2f)
                    {
                        hasValue = true;
                        break;
                    }
                }
                if (hasValue)
                {
                    return;
                }
            }



            CubePointList[0].position = a;
            CubePointList[1].position = b;
            CubePointList[2].position = c;
            CubePointList[3].position = d;
        }

        private void OnDrawGizmos()
        {
            //var pos = this.transform.position;
            ////var forward = this.transform.forward;
            Gizmos.color = Color.yellow;
            //Gizmos.DrawCube(pos, cubeSize * Vector3.one);

            foreach (var tf in CubePointList)
            {
                Gizmos.DrawCube(tf.position, cubeSize * Vector3.one * 0.1f);
            }

            Gizmos.color = Color.blue;

            if (CubePointList.Count == 4)
            {
                Gizmos.DrawLine(CubePointList[0].position, CubePointList[1].position);
                Gizmos.DrawLine(CubePointList[1].position, CubePointList[2].position);
                Gizmos.DrawLine(CubePointList[2].position, CubePointList[3].position);
                Gizmos.DrawLine(CubePointList[3].position, CubePointList[0].position);
            }

            Gizmos.color = Color.red;

            foreach (var tf in RandomList)
            {
                Gizmos.DrawCube(tf.position, cubeSize * Vector3.one * 0.1f);
            }
        }
    }
}