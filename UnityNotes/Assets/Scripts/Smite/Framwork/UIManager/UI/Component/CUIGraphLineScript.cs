using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CUIGraphLineScript : CUIGraphBaseScript
    {
        public float thickness = 1f;    //单位：像素
        public float drawSpeed = 0f;    //单位：像素/秒

        private float _curPathLen = 0f;
        private float _fixPathLen = 0f;
        private float _lastDrawTime = 0f;
        private int _drawPathIndex = 0;

        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
        }
        override protected void OnDraw()
        {
            if (vertexChanged)
            {
                vertexChanged = false;
                _curPathLen = 0f;
                _fixPathLen = 0f;
                _drawPathIndex = drawSpeed > 0f ? 1 : m_vertexs.Length;
                _lastDrawTime = Time.time;
            }

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(thickness <= 1f ? GL.LINES : GL.QUADS);
            GL.Color(color);
            
            for (int vi = 1; vi < _drawPathIndex; ++vi)
            {
                GLLine(ref m_vertexs[vi - 1], ref m_vertexs[vi], thickness);
            }

            if (_drawPathIndex < m_vertexs.Length)
            {
                float deltaLen = (Time.time - _lastDrawTime) * drawSpeed;   //速度公式在此调整
                _lastDrawTime = Time.time;
                _curPathLen += deltaLen;

                while (_drawPathIndex < m_vertexs.Length)
                {
                    Vector3 prevFixPos = m_vertexs[_drawPathIndex - 1];
                    Vector3 nextFixPos = m_vertexs[_drawPathIndex];
                    float fixMag = (nextFixPos - prevFixPos).magnitude;

                    if (_fixPathLen + fixMag > _curPathLen)
                        break;

                    _fixPathLen += fixMag;
                    ++_drawPathIndex;

                    GLLine(ref prevFixPos, ref nextFixPos, thickness);
                }

                if (_drawPathIndex < m_vertexs.Length)
                {
                    Vector3 prevFixPos = m_vertexs[_drawPathIndex - 1];
                    Vector3 nextFixPos = m_vertexs[_drawPathIndex];
                    float dist = _curPathLen - _fixPathLen;
                    Vector3 floatPos = prevFixPos + (nextFixPos - prevFixPos).normalized * dist;

                    GLLine(ref prevFixPos, ref floatPos, thickness);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
        static void GLLine(ref Vector3 start, ref Vector3 end, float thickness = 1f)
        {
            if (thickness <= 1f)
            {
                GL.Vertex3(start.x, start.y, start.z);
                GL.Vertex3(end.x, end.y, end.z);
            }
            else
            {
                Vector3 directVec = end - start;
                Vector3 directNormal = directVec.normalized;
                Vector3 verticalNoraml = new Vector3(directNormal.y, -directNormal.x);
                Vector3 quadVec0 = start + verticalNoraml * thickness * 0.5f;
                Vector3 quadVec1 = quadVec0 + directVec;
                Vector3 quadVec2 = quadVec1 - verticalNoraml * thickness;
                Vector3 quadVec3 = quadVec2 - directVec;

                GL.Vertex3(quadVec0.x, quadVec0.y, quadVec0.z);
                GL.Vertex3(quadVec1.x, quadVec1.y, quadVec1.z);
                GL.Vertex3(quadVec2.x, quadVec2.y, quadVec2.z);
                GL.Vertex3(quadVec3.x, quadVec3.y, quadVec3.z);
            }
        }
    }
};