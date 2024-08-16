//模拟凸多边形
//UGUI最小的单元是一个矩形  所以顶点数必须是4的倍数
//矩形模拟三角形最多组成凸多边形
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Framework
{
    public class CUIPolygon : Graphic
    {
        //菊部坐标系
        public Vector3 deltaCenterV3 = new Vector3();

        public Vector3[] vertexs = new Vector3[]
    {
        new Vector3( 0 , 100),
         new Vector3( -50 , 0),
          new Vector3( 50 , 0),
    };
        //重置坐标
        protected override void Start()
        {
            this.transform.localPosition = Vector3.zero;
            base.Start();
        }

        [Obsolete("This class writed in unity4.7.2")]
        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            if (vertexs == null || vertexs.Length < 3) return;
            UpdateVBO(vbo);
        }

        private void UpdateVBO(List<UIVertex> vbo)
        {
            UIVertex vert;
            if (vertexs.Length == 3)
            {
                for (int i = 0; i < vertexs.Length; i++)
                {
                    vert = UIVertex.simpleVert;
                    vert.color = this.color;
                    vert.position = vertexs[i];
                    vbo.Add(vert);
                }
                vert = UIVertex.simpleVert;
                vert.color = this.color;
                vert.position = vertexs[0];
                vbo.Add(vert);
            }
            else if (vertexs.Length == 4)
            {
                for (int i = 0; i < vertexs.Length; i++)
                {
                    vert = UIVertex.simpleVert;
                    vert.color = this.color;
                    vert.position = vertexs[i];
                    vbo.Add(vert);
                }
            }
            else
            {
                for (int i = 0; i < vertexs.Length; i++)
                {
                    vert = UIVertex.simpleVert;
                    vert.color = this.color;
                    vert.position = vertexs[i];
                    vbo.Add(vert);

                    if (i != vertexs.Length - 1)
                    {
                        vert = UIVertex.simpleVert;
                        vert.color = this.color;
                        vert.position = vertexs[i + 1];
                        vbo.Add(vert);
                    }
                    else
                    {
                        vert = UIVertex.simpleVert;
                        vert.color = this.color;
                        vert.position = vertexs[0];
                        vbo.Add(vert);
                    }

                    vert = UIVertex.simpleVert;
                    vert.color = this.color;
                    vert.position = deltaCenterV3;
                    vbo.Add(vert);

                    vert = UIVertex.simpleVert;
                    vert.color = this.color;
                    vert.position = deltaCenterV3;
                    vbo.Add(vert);
                }
            }
        }
    }
}
