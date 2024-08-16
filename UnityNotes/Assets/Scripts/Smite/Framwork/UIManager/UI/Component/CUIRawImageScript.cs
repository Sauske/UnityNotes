//==================================================================================
/// UI RawImage 控件
/// @neoyang
/// @2015.03.10
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Framework
{
    public class CUIRawImageScript : CUIComponent
    {
        //UGUI RawImage组件
        // private RawImage m_rawImage;

        //渲染RenderTexture所使用的相机、光源等
        private Camera m_renderTextureCamera;
        // private Light m_renderTextureLight;
        private GameObject m_rawRootObject;

        private const int c_uiRawLayer = 15;

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            // m_rawImage = gameObject.GetComponent<RawImage>();
            
            m_renderTextureCamera = GetComponentInChildren<Camera>(gameObject);           
            if (m_renderTextureCamera != null)
            {
                Transform rawRootTransform = m_renderTextureCamera.gameObject.transform.Find("RawRoot");
                if (rawRootTransform != null)
                {
                    m_rawRootObject = rawRootTransform.gameObject;
                }                
            }

            //重置节点大小，让其自动适配Form节点scale的影响
            //CUIUtility.ResetUIScale(m_renderTextureCamera.gameObject);

            // m_renderTextureLight = GetComponentInChildren<Light>(gameObject);
        }

        //--------------------------------------------------
        /// OnDestroy
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_renderTextureCamera = null;
            m_rawRootObject = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Hide
        //--------------------------------------------------
        public override void Hide()
        {
            base.Hide();

            CUIUtility.SetGameObjectLayer(m_rawRootObject, CUIUtility.c_hideLayer); 
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void Appear()
        {
            base.Appear();

            CUIUtility.SetGameObjectLayer(m_rawRootObject, c_uiRawLayer);
        }

        //--------------------------------------------------
        /// 添加需要渲染到RawImage的GameObject
        /// @name
        /// @rawObject
        //--------------------------------------------------
        public void AddGameObject(string name, GameObject rawObject, Vector3 position, Quaternion rotation, Vector3 scaler)
        {
            if (m_rawRootObject == null)
            {
                return;
            }
            
            SetRawObjectLayer(rawObject, LayerMask.NameToLayer("UIRaw"));

            rawObject.name = name;
            rawObject.transform.SetParent(m_rawRootObject.transform);
            rawObject.transform.localPosition = position;
            rawObject.transform.localRotation = rotation;

            rawObject.transform.localScale = scaler;
        }

        //--------------------------------------------------
        /// 移除渲染到RawImage的GameObject
        /// @name
        /// !!本函数并不对GameObject进行销毁等内存管理操作，仅仅是从root移除
        //--------------------------------------------------
        public GameObject RemoveGameObject(string name)
        {
            if (m_rawRootObject == null)
            {
                return null;
            }

            for (int i = 0; i < m_rawRootObject.transform.childCount; i++)
            {
                GameObject child = m_rawRootObject.transform.GetChild(i).gameObject;

                if (child.name.Equals(name))
                {
                    child.transform.SetParent(null);
                    return child;
                }
            }

            return null;
        }

        //查找Gameobject
        public GameObject GetGameObject(string name)
        {
            GameObject result = null;

            if (m_rawRootObject == null)
            {
                return null;
            }

            for (int i = 0; i < m_rawRootObject.transform.childCount; i++)
            {
                GameObject child = m_rawRootObject.transform.GetChild(i).gameObject;

                if (child.name.Equals(name))
                {
                    result = child;
                    break;
                }
            }

            return result;
        }

        public void SetRawObjectLayer(GameObject rawObject, int layer)
        {
            rawObject.layer = layer;

            for (int i = 0; i < rawObject.transform.childCount; i++)
            {
                SetRawObjectLayer(rawObject.transform.GetChild(i).gameObject, layer);
            }
        }
    }
};