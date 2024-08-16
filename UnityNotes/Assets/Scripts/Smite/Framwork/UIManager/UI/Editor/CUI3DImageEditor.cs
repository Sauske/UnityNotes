//==================================================================================
/// CUI3DImageScript Inspector
/// @neoyang
/// @2015.04.14
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUI3DImageScript))]
    public class CUI3DImageEditor : Editor
    {
/*
        //3dImage信息
        public class C3DImageInfo
        {
            public CUI3DImageScript m_3dImageScript;
            public Vector2 m_lastPivotScreenPosition;
        };

        private static List<C3DImageInfo> s_3dImageInfos = new List<C3DImageInfo>();
        private static bool s_initUpdate = false;
*/ 

        protected virtual void OnEnable()
        {
/*
            //只在编辑时进行处理
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            CUI3DImageScript _3dImageScript = target as CUI3DImageScript;

            CUIFormScript formScript = GetFormScript(_3dImageScript.gameObject.transform.parent);
            formScript.InitializeCanvas();

            Camera camera = _3dImageScript.GetComponent<Camera>();
            Light light = _3dImageScript.GetComponent<Light>();

            _3dImageScript.InitializeCamera(camera, light, formScript);
         
            _3dImageScript.m_onDestroyed -= OnDestroy3DImage;
            _3dImageScript.m_onDestroyed += OnDestroy3DImage;

            AddTo3DImageScriptList(_3dImageScript);

            if (!s_initUpdate)
            {
                s_initUpdate = true;

                EditorApplication.update -= Update3DGameObjects;
                EditorApplication.update += Update3DGameObjects;
            }
*/ 
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

/*
        //--------------------------------------
        /// 添加到3DImageScript列表
        /// @_3dImageScript
        //--------------------------------------
        private static void AddTo3DImageScriptList(CUI3DImageScript _3dImageScript)
        {
            for (int i = 0; i < s_3dImageInfos.Count; i++)
            {
                if (s_3dImageInfos[i].m_3dImageScript == _3dImageScript)
                {
                    return;
                }
            }

            C3DImageInfo _3dImageInfo = new C3DImageInfo();
            _3dImageInfo.m_3dImageScript = _3dImageScript;
            _3dImageInfo.m_lastPivotScreenPosition = Vector2.zero;

            s_3dImageInfos.Add(_3dImageInfo);
        }

        //--------------------------------------
        /// 从3DImageScript列表移除
        /// @_3dImageScript
        //--------------------------------------
        private static void RemoveFrom3DImageScriptList(CUI3DImageScript _3dImageScript)
        {
            for (int i = 0; i < s_3dImageInfos.Count; i++)
            {
                if (s_3dImageInfos[i].m_3dImageScript == _3dImageScript)
                {
                    s_3dImageInfos.RemoveAt(i);
                    return;
                }
            }
        }

        //--------------------------------------
        /// 3DImageScript被释放 
        /// @_3dImageScript
        //--------------------------------------
        private static void OnDestroy3DImage(CUI3DImageScript _3dImageScript)
        {
            RemoveFrom3DImageScriptList(_3dImageScript);
        }

        //--------------------------------------------------
        /// 查找FormScript
        /// @transform
        //--------------------------------------------------
        private static CUIFormScript GetFormScript(Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            CUIFormScript formScript = transform.gameObject.GetComponent<CUIFormScript>();
            if (formScript != null)
            {
                return formScript;
            }

            return GetFormScript(transform.parent);
        }

        //--------------------------------------------------
        /// 更新3DGameObject的位置
        //--------------------------------------------------
        public static void Update3DGameObjects()
        {
            //只在编辑时进行处理
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            for (int i = 0; i < s_3dImageInfos.Count;)
            {
                if (s_3dImageInfos[i].m_3dImageScript == null)
                {
                    s_3dImageInfos.RemoveAt(i);
                    continue;
                }

                Update3DGameObject(s_3dImageInfos[i]);

                i++;
            }
        }

        //--------------------------------------------------
        /// 更新3DGameObject的位置
        //--------------------------------------------------
        public static void Update3DGameObject(C3DImageInfo _3dImageInfo)
        {
            //编辑时拖到3DImage上面的GameObject都是绑定到3DImage枢轴点上的
            bool needChangePosition = false;

            Vector2 pivotScreenPosition = _3dImageInfo.m_3dImageScript.GetPivotScreenPosition();

            if (pivotScreenPosition != _3dImageInfo.m_lastPivotScreenPosition)
            {
                needChangePosition = true;
            }

            _3dImageInfo.m_lastPivotScreenPosition = pivotScreenPosition;

            Camera camera = _3dImageInfo.m_3dImageScript.GetComponent<Camera>();

            for (int i = 0; i < _3dImageInfo.m_3dImageScript.gameObject.transform.childCount; i++)
            {
                GameObject child = _3dImageInfo.m_3dImageScript.gameObject.transform.GetChild(i).gameObject;

                CUIUtility.SetGameObjectLayer(child, CUI3DImageScript.s_cameraLayers[(int)_3dImageInfo.m_3dImageScript.m_imageLayer]);

                if (needChangePosition && camera.orthographic)
                {
                    _3dImageInfo.m_3dImageScript.ChangeScreenPositionToWorld(child, ref _3dImageInfo.m_lastPivotScreenPosition);
                }
            }

            //直接修改camera的viewport rect
            if (needChangePosition && !camera.orthographic)
            {
                float sx = _3dImageInfo.m_lastPivotScreenPosition.x / Screen.width;
                sx = sx * 2f - 1f;
                camera.rect = new Rect(0f, 0f, 1f, 1f);
                camera.ResetAspect();
                camera.SetOffsetX(sx);
            }
        }
*/ 
    };
};