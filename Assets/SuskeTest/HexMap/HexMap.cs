using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuskeTest
{
   //[ExecuteInEditMode]
    public class HexMapTest : MonoBehaviour
    {       
        public Transform mTarget;

        public  Vector3 center = Vector3.zero;
        public Vector3 dir = new Vector3(8, 0, 0);
        public float radius = 3f;
        public float angle = 120;
        void Start()
        {

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
            bool inRect = false;// sector.Contains(mTarget.position);
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
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////             HexMap              //////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// 

    public class HexMap
    {
        public HexCell startCell; // ��ʼ��Ԫ��
        public HexCell goalCell; // Ŀ�굥Ԫ��

        public List<HexCell> openList; // �����б�
        public List<HexCell> closedList; // ����б�

        public List<HexCell> FindPath(HexCell start, HexCell goal)
        {
            openList = new List<HexCell>();
            closedList = new List<HexCell>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                HexCell currentCell = GetLowestFCostCell(openList);

                openList.Remove(currentCell);
                closedList.Add(currentCell);

                if (currentCell == goal)
                {
                    // �ҵ�·��
                    List<HexCell> path = RetracePath(start, goal);
                    // �����ﴦ��·��
                    return path;
                }

                foreach (HexCell neighbor in currentCell.GetNeighbors())
                {
                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    int newGCost = currentCell.GCost + GetDistance(currentCell, neighbor);
                    if (newGCost < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = GetDistance(neighbor, goal);
                        neighbor.Parent = currentCell;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
            // δ�ҵ�·��
            return null;           
        }

        private List<HexCell> RetracePath(HexCell start, HexCell goal)
        {
            List<HexCell> path = new List<HexCell>();
            HexCell currentCell = goal;

            while (currentCell != start)
            {
                path.Add(currentCell);
                currentCell = currentCell.Parent;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// ���ٳɱ��㼯
        /// </summary>
        /// <param name="cellList"></param>
        /// <returns></returns>
        private HexCell GetLowestFCostCell(List<HexCell> cellList)
        {
            HexCell lowestFCostCell = cellList[0];

            for (int i = 1; i < cellList.Count; i++)
            {
                if (cellList[i].FCost < lowestFCostCell.FCost)
                {
                    lowestFCostCell = cellList[i];
                }
            }

            return lowestFCostCell;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="cellA"></param>
        /// <param name="cellB"></param>
        /// <returns></returns>
        private int GetDistance(HexCell cellA, HexCell cellB)
        {
            // ����������Ԫ��֮��ľ���
            // �������ʹ�ò�ͬ�ľ�����㷽������ֱ�߾���������پ���
            return 1;
        }
    }
}
