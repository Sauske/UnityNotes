//====================================
/// UI粒子控件
/// @royjin
/// @2015.06.01
//====================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Framework
{
    public class CUIParticleScript : CUIComponent  
	{	
        //渲染顺序
		// private int m_sortingOrder;

        public string m_resPath = "";
        public bool m_isFixScaleToForm = false;                 //节点TransForm缩放是否受父节点影响,特效中用到sprite之类的2d图片显示需要设置为true
        public bool m_isFixScaleToParticleSystem = false;       //如果特效用到ParticleSytem，尽量勾上，并且对应gameobject上需要挂一个ParcleScaler组件

        //渲染器缓存
        private Renderer[] m_renderers;
        private int m_rendererCount;

        //加载粒子资源
        private void LoadRes()
        {
            string realPath = m_resPath;
            if (!String.IsNullOrEmpty(realPath))
            {
                //if(GameSettings.ParticleQuality == SGameRenderQuality.Low)
                //{
                //    realPath = CUIUtility.s_Particle_Dir + m_resPath + "/" + m_resPath + "_low.prefeb";
                //}
                //else  if(GameSettings.ParticleQuality == SGameRenderQuality.Medium)
                //{
                //    realPath = CUIUtility.s_Particle_Dir + m_resPath + "/" + m_resPath + "_mid.prefeb";
                //}
                //else
                //{
                //    realPath = CUIUtility.s_Particle_Dir + m_resPath + "/" + m_resPath + ".prefeb";
                //}

                GameObject particlePrefeb = null;// CResourceManager.GetInstance().GetResource(realPath, typeof(GameObject), enResourceType.UIPrefab).m_content as GameObject;

                if (particlePrefeb != null && gameObject.transform.childCount == 0)
                {
                    GameObject newInstance = GameObject.Instantiate(particlePrefeb) as GameObject;
                    newInstance.transform.SetParent(gameObject.transform);
                    newInstance.transform.localPosition = Vector3.zero;
                    newInstance.transform.localRotation = Quaternion.identity;
                    newInstance.transform.localScale = Vector3.one;
                }
            }
        }

        /// <summary>
        /// 后期动态加载资源
        /// </summary>
        /// <param name="resName"></param>
        public void LoadRes(string resName)
        {
            if (!m_isInitialized)
            {
                return;
            }

            m_resPath = resName;

            LoadRes();

            InitializeRenderers(); 

            SetSortingOrder(m_belongedFormScript.GetSortingOrder());

            if (m_isFixScaleToForm)
            {
                ResetScale();
            }

            if (m_isFixScaleToParticleSystem)
            {
                ResetParticleScale();
            }

            if (m_belongedFormScript.IsHided())
            {
                Hide();
            }
        }

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
		{
            if (m_isInitialized)
            {
                return;
            }

            //加载资源
            LoadRes();

            //组织需要排序的Render组件
            InitializeRenderers();

            //初始化排序等
            base.Initialize(formScript);

            if (m_isFixScaleToForm)
            {
                ResetScale();
            }

            if (m_isFixScaleToParticleSystem)
            {
                ResetParticleScale();
            }

            if (m_belongedFormScript.IsHided())
            {
                Hide();
            }
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_renderers = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Hide
        //--------------------------------------------------
        public override void Hide()
        {
            base.Hide();

            CUIUtility.SetGameObjectLayer(this.gameObject, CUIUtility.c_hideLayer);
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void Appear()
        {
            base.Appear();

            CUIUtility.SetGameObjectLayer(this.gameObject, CUIUtility.c_uiLayer);
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void SetSortingOrder(int sortingOrder)
        {
            base.SetSortingOrder(sortingOrder);

            // m_sortingOrder = sortingOrder;

            for (int i = 0; i < m_rendererCount; i++)
            {
                m_renderers[i].sortingOrder = sortingOrder;
            }
        }

        //--------------------------------------------------
        /// 初始化渲染器
        //--------------------------------------------------
        private void InitializeRenderers()
        {
            m_renderers = new Renderer[100];
            m_rendererCount = 0;

            CUIUtility.GetComponentsInChildren<Renderer>(this.gameObject, m_renderers, ref m_rendererCount);
        }

        //重置Scale，保证特效中用到的Sprite图片不受Form节点的影响
        private void ResetScale()
        {
            float newScale = 1 / m_belongedFormScript.gameObject.transform.localScale.x;
            gameObject.transform.localScale = new Vector3(newScale, newScale, 0);
        }

        //重置特效发射器大小，同步屏幕适配的情况
        private void ResetParticleScale()
        {
            if (m_belongedFormScript == null)
            {
                return;
            }

            float scaleValue = 1;
            RectTransform formTrans = m_belongedFormScript.GetComponent<RectTransform>();

            if (m_belongedFormScript.m_canvasScaler.matchWidthOrHeight == 0f)
            {
                scaleValue = (formTrans.rect.width / formTrans.rect.height) / (m_belongedFormScript.m_canvasScaler.referenceResolution.x / m_belongedFormScript.m_canvasScaler.referenceResolution.y);
            }
            else if (m_belongedFormScript.m_canvasScaler.matchWidthOrHeight == 1f)
            {
                //scaleValue = (formTrans.rect.height / formTrans.rect.width) / (m_belongedFormScript.m_canvasScaler.referenceResolution.y / m_belongedFormScript.m_canvasScaler.referenceResolution.x);
            }

            InitializeParticleScaler(this.gameObject, scaleValue);
        }

        //处理粒子缩放
        private void InitializeParticleScaler(GameObject gameObject, float scale)
        {
            ParticleScaler particleScaler = gameObject.GetComponent<ParticleScaler>();
            if (particleScaler == null)
            {
                particleScaler = gameObject.AddComponent<ParticleScaler>();
            }

            if (particleScaler.particleScale != scale)
            {
                particleScaler.particleScale = scale;
                particleScaler.alsoScaleGameobject = false;
                particleScaler.CheckAndApplyScale();
            }
        }

/*
#if UNITY_EDITOR
        void Update()
        {
            if (m_isFixScaleToParticleSystem)
            {
                ResetParticleScale();
            }
        }
#endif
*/
    } 
}