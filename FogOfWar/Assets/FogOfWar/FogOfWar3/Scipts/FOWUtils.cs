using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMI.FogOfWar
{
    public static class FOWUtils
    {
        public static void OnDrawGizmosSelected(bool[,] mapData,Vector3 center,float MapTileSize,float fogSize)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(center, new Vector3(fogSize, 0.1f, fogSize));
            if (mapData != null)
            {
                for (int i = 0; i < mapData.GetLength(0); i++)
                {
                    for (int j = 0; j < mapData.GetLength(1); j++)
                    {
                        Gizmos.color = mapData[i, j] ? Color.red : Color.white;
                        Vector3 drawCenter = GetV3(new int[] { i, j }, center, MapTileSize, fogSize);
                        Gizmos.DrawWireCube(drawCenter, new Vector3(MapTileSize - 0.02f, 0.1f, MapTileSize - 0.02f));
                    }
                }
                //foreach (var pos in viewerPos)
                //{
                //    Gizmos.color = Color.green;
                //    Gizmos.DrawCube(GetV3(pos), new Vector3(MapTileSize - 0.02f, 1f, MapTileSize - 0.02f));
                //}
            }
        }
        public static Vector3 GetV3(int[] pos,Vector3 orgPos, float mapTileSize,float fogSize)
        {
            return new Vector3(pos[0] * mapTileSize, 0, pos[1] * mapTileSize) +
                new Vector3(mapTileSize / 2, 0, mapTileSize / 2) +
                orgPos - new Vector3(fogSize / 2, 0, fogSize / 2);
        }
    }
}
