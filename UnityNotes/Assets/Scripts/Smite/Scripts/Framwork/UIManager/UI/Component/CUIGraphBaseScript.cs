///用的GL函数 绑定的摄像机深度
///简单的说只能在UGUI上面或者下面
///传入的是屏幕坐标
///挂在form下面
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public enum CUIGraphType
    {
        Line ,      //线段
        Triangle ,  //三角形
    }
    public class CUIGraphBaseScript : CUIComponent
    {

        //因为CUI窗口全部是10，一般再说画线是画在UI上面的 所以默认11
        public static readonly int s_depth = 11;
        //UI
        public static readonly int s_cullingMask = LayerMask.NameToLayer("UI");
        //填充颜色
        public Color color = Color.white;
        //摄像机深度 
        public int cameraDepth = s_depth;
        //这里保存的是 屏幕坐标
        [SerializeField]
        protected Vector3[] m_vertexs;
        //材质
        private static Material s_lineMaterial = null;

        //摄像机 使用GL库必须要绑定一个摄像机
        private Camera m_camera = null;
        protected bool vertexChanged = false;
        
        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
            m_camera = GetComponent<Camera>();
            if (GetComponent<Camera>() == null)
            {
                m_camera = gameObject.AddComponent<Camera>();
                GetComponent<Camera>().depth = s_depth;
                GetComponent<Camera>().cullingMask = s_cullingMask;
                GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
            }
            if (s_lineMaterial == null)
            {
                s_lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                    "SubShader { Pass { " +
                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                    "    BindChannels {" +
                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                    "} } }");
                s_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                s_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_camera = null;

            base.OnDestroy();
        }

        /// <summary>
        /// 返回摄像机
        /// </summary>
        /// <returns></returns>
        public Camera GetCamera()
        {
            return m_camera;
        }
        /// <summary>
        /// 设置顶点数组
        /// </summary>
        /// <param name="vertexs">屏幕坐标</param>
        public void SetVertexs(Vector3[] vertexs)
        {
            if (vertexs == null) return;
            m_vertexs = new Vector3[vertexs.Length];
            for(int i = 0 ; i < vertexs.Length ; i++)
            {
                m_vertexs[i] = new Vector3(vertexs[i].x , vertexs[i].y , 0);
            }
            vertexChanged = true;
        }
        //public override void Close()
        //{
        //    base.Close();
        //}
        //public override void Hide()
        //{
        //    base.Hide();
        //}
        //public override void Appear()
        //{
        //    base.Appear();
        //}

        //void OnPreRender()
        //{

        //}
        /// <summary>
        /// 渲染
        /// </summary>
        void OnPostRender()
        {
            if (m_vertexs == null) return;
            if (s_lineMaterial == null) return;
            s_lineMaterial.SetPass(0);
            OnDraw();
        }
        virtual protected void OnDraw()
        {

        }
    }
};