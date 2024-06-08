using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuskeTest
{
    public class HexCell : IEquatable<HexCell>
    {
        public int x;
        public int y;
        public int z;

        /// <summary>
        /// ����ʼ��Ԫ�񵽵�ǰ��Ԫ����ƶ��ɱ�
        /// </summary>
        public int GCost { get; set; }

        /// <summary>
        /// �ӵ�ǰ��Ԫ��Ŀ�굥Ԫ��Ĺ���ɱ�
        /// </summary>
        public int HCost { get; set; }

        /// <summary>
        /// �ܳɱ�
        /// </summary>
        public int FCost => GCost + HCost;

        /// <summary>
        /// ���ڵ�
        /// </summary>
        public HexCell Parent { get; set; }

        /// <summary>
        /// �����ĵ�
        /// </summary>
        public List<HexCell> Neighbors = new List<HexCell>();

        private static Vector3Int mDir0 = new Vector3Int(1, -1, 0);
        private static Vector3Int mDir1 = new Vector3Int(1, 0, -1);
        private static Vector3Int mDir2 = new Vector3Int(0, 1, -1);
        private static Vector3Int mDir3 = new Vector3Int(-1, 1, 0);
        private static Vector3Int mDir4 = new Vector3Int(-1, 0, 1);
        private static Vector3Int mDir5 = new Vector3Int(0, -1, 1);
        public static List<Vector3Int> Directions = new List<Vector3Int> { mDir0, mDir1, mDir2, mDir3, mDir4, mDir5 };

        public HexCell(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public List<HexCell> GetNeighbors()
        {
            for (int idx = 0; idx < Directions.Count; idx++)
            {
                int tempX = x + Directions[idx].x;
                int tempY = y + Directions[idx].y;
                int tempZ = z + Directions[idx].z;
                Neighbors.Add(new HexCell(tempX, tempY, tempZ));
            }
            return Neighbors;
        }

        public void DrawMesh()
        {
#if UNITY_EDITOR

#endif
        }

        public bool Equals(HexCell obj)
        {
            return obj != null && x == obj.x && y == obj.y && z == obj.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public override string ToString()
        {
            return $"HexCell({x}, {y}, {z})";
        }
    }
}
