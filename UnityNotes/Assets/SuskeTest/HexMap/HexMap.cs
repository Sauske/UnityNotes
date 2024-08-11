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
        public HexCell startCell; // 起始单元格
        public HexCell goalCell; // 目标单元格

        public List<HexCell> openList; // 开放列表
        public List<HexCell> closedList; // 封闭列表

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
                    // 找到路径
                    List<HexCell> path = RetracePath(start, goal);
                    // 在这里处理路径
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
            // 未找到路径
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
        /// 最少成本点集
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
        /// 距离
        /// </summary>
        /// <param name="cellA"></param>
        /// <param name="cellB"></param>
        /// <returns></returns>
        private int GetDistance(HexCell cellA, HexCell cellB)
        {
            // 计算两个单元格之间的距离
            // 这里可以使用不同的距离计算方法，如直线距离或曼哈顿距离
            return 1;
        }
    }
}
