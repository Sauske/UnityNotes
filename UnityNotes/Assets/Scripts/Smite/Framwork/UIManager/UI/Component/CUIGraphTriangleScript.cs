using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CUIGraphTriangleScript : CUIGraphBaseScript
    {
        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
        }
        override protected void OnDraw()
        {
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(GL.TRIANGLES);
            GL.Color(color);
            for (int i = 0; i < m_vertexs.Length; i++)
            {
                if (i + 2 < m_vertexs.Length)
                {
                    GL.Vertex3(m_vertexs[i].x, m_vertexs[i].y, m_vertexs[i].z);
                    GL.Vertex3(m_vertexs[i + 1].x, m_vertexs[i + 1].y, m_vertexs[i + 1].z);
                    GL.Vertex3(m_vertexs[i + 2].x, m_vertexs[i + 2].y, m_vertexs[i + 2].z);
                }
                i += 2;
            }
            GL.End();
            GL.PopMatrix();
        }
    }
};