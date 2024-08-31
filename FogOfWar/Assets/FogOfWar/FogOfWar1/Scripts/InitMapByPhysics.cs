using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FogOfWar
{
    public class InitMapByPhysics : MonoBehaviour
    {
        public FowManager fowManager;
        int[,] mapData;
        void Start()
        {
            PhysicsCheck();
        }
        public void PhysicsCheck()
        {
            mapData = new int[(int)(fowManager.FogSizeX / fowManager.MapTileSize), (int)(fowManager.FogSizeY / fowManager.MapTileSize)];
            for (int i = 0; i < mapData.GetLength(0); i++)
            {
                for (int j = 0; j < mapData.GetLength(1); j++)
                {
                    Vector3 center = fowManager.GetV3(new int[] { i, j });
                    float tileSize = fowManager.MapTileSize - 0.02f;
                    Vector3 halfExt = new Vector3(tileSize, 0f, tileSize) / 2;
                    if (Physics.CheckBox(center, halfExt))
                    {
                        mapData[i, j] = 1;
                    }
                    else
                    {
                        mapData[i, j] = 0;
                    }
                }
            }
            fowManager.InitMap(mapData);
        }
    }
}
