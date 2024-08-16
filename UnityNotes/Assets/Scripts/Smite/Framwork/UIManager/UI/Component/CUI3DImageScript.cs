//==================================================================================
/// UI 3DImage 控件
/// @采用正交相机时，相机位位于屏幕正中，3DImage控件枢轴点的屏幕坐标可以作为3D物体的屏幕坐标
/// @采用透视相机时，需要修改相机的Viewport Rect来实现将3DImage控件枢轴点的屏幕坐标x作为3D物体的屏幕坐标x
/// @采用透视相机时，3D物体的y和z位置需要由3DImage控件下面的root节点来控制
/// @采用透视相机的局限性非常大！只能用在几个特殊场合，并且3D物体是强制绑定到3DImage控件枢轴点位置的
/// @只能位于所有Form的下面或者上面，无法做到夹在两个Form之间
/// @neoyang
/// @2015.04.14
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public enum en3DImageLayer
    {
        Background,
        Foreground
    };

    [ExecuteInEditMode]
    public class CUI3DImageScript : CUIComponent
    {
        //3D Image层(背景/前景)
        public en3DImageLayer m_imageLayer = en3DImageLayer.Background;

        //相机默认值
        public Vector3 m_renderCameraDefaultScale = Vector3.one;
        public float m_renderCameraDefaultSize = 20;

        //渲染3D Object所使用的相机、光源等
        public Camera m_renderCamera;
        private Light m_renderLight;

        //Camera Layer
        public static int[] s_cameraLayers = new int[] { 16, 17 };
                                                                //LayerMask.NameToLayer("UI_Background"),
                                                                //LayerMask.NameToLayer("UI_Foreground")
                                                            //};

        //Camera Depths
        public static int[] s_cameraDepths = new int[] { 9, 11 };

        //3D Image控件枢轴点的屏幕坐标
        private Vector2 m_pivotScreenPosition;
        private Vector2 m_lastPivotScreenPosition;

        //UI背景/前景使用的GameObject信息
        private class C3DGameObject
        {
            public string m_path;
            public GameObject m_gameObject;
            public bool m_useGameObjectPool;
            public bool m_protogenic;                       //是否是在编辑时就挂在控件上面的原生GameObject
            public bool m_bindPivot;                        //是否将GameObject的参考点绑定到3DImage控件的枢轴点
        };

        //3D GameObjects 信息组
        private ListView<C3DGameObject> m_3DGameObjects = new ListView<C3DGameObject>();

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

            m_renderCamera = this.gameObject.GetComponent<Camera>();
            m_renderLight = this.gameObject.GetComponent<Light>();
           
            InitializeRender();

            GetPivotScreenPosition();

            Initialize3DGameObjects();
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_renderCamera = null;
            m_renderLight = null;

            //移除GameObject指针
            m_3DGameObjects.Clear();
            m_3DGameObjects = null;

            //释放基类数据
            base.OnDestroy();
        }

        //--------------------------------------------------
        /// Close
        //--------------------------------------------------
        public override void Close()
        {
            base.Close();

            //这里需要移除所有运行时Add的GameObject
            for (int i = 0; i < m_3DGameObjects.Count; )
            {
                if (!m_3DGameObjects[i].m_protogenic)
                {
                    if (m_3DGameObjects[i].m_gameObject != null)
                    {
                        if (m_3DGameObjects[i].m_useGameObjectPool)
                        {
                          //  CGameObjectPool.GetInstance().RecycleGameObject(m_3DGameObjects[i].m_gameObject);
                        }
                        else
                        {
                           // CUICommonSystem.DestoryObj(m_3DGameObjects[i].m_gameObject);
                        }
                    }

                    m_3DGameObjects[i].m_path = null;
                    m_3DGameObjects[i].m_gameObject = null;
                    m_3DGameObjects.RemoveAt(i);
                    continue;
                }
                    
                i++;
            }
        }

        //--------------------------------------------------
        /// Hide
        //--------------------------------------------------
        public override void Hide()
        {
            base.Hide();

            if (m_renderCamera != null)
            {
                m_renderCamera.enabled = false;
            }            

            for (int i = 0; i < m_3DGameObjects.Count; i++)
            {
                CUIUtility.SetGameObjectLayer(m_3DGameObjects[i].m_gameObject, CUIUtility.c_hideLayer);
            }
        }

        //--------------------------------------------------
        /// Appear
        //--------------------------------------------------
        public override void Appear()
        {
            base.Appear();

            if (m_renderCamera != null)
            {
                m_renderCamera.enabled = true;
            }            

            for (int i = 0; i < m_3DGameObjects.Count; i++)
            {
                CUIUtility.SetGameObjectLayer(m_3DGameObjects[i].m_gameObject, s_cameraLayers[(int)m_imageLayer]);
            }
        }

#if UNITY_EDITOR
        void Start()
        {
            //编辑模式下，才需要自己调用初始化(运行模式下会由Form来进行初始化)
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
             //   Initialize(CUIUtility.GetFormScript(this.gameObject.transform.parent));
            }            
        }
#endif

        void Update()
        {
#if UNITY_EDITOR
            //编辑模式下，需要在每次Update的时候重新遍历3DObject(因为可能手动设置上去)
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Initialize3DGameObjects();
            }
#endif

            if (m_belongedFormScript != null && m_belongedFormScript.IsClosed())
            {
                return;
            }

            //考虑到3DImage控件本身的位置可能会发生变化(如FadeInOut的时候),需要在PreRender中计算控件位置并同步给3D物体
            GetPivotScreenPosition();

            if (m_lastPivotScreenPosition != m_pivotScreenPosition)
            {
                //DebugHelper.ConsoleLog("Last Pivot Screen Position = " + m_lastPivotScreenPosition + ", Current Pivot Screen Position = " + m_pivotScreenPosition);
                if (m_renderCamera != null)
                {
                    if (m_renderCamera.orthographic)
                    {
                        for (int i = 0; i < m_3DGameObjects.Count; i++)
                        {
                            if (m_3DGameObjects[i].m_bindPivot)
                            {
                                ChangeScreenPositionToWorld(m_3DGameObjects[i].m_gameObject, ref m_pivotScreenPosition);
                            }
                        }
                    }
                    else
                    {
#if UNITY_EDITOR || UNITY_STANDALONE
                        float sx = m_pivotScreenPosition.x / Screen.width;
#else
                        float sx = m_pivotScreenPosition.x / Mathf.Max(Screen.width, Screen.height);
#endif
                        sx = sx * 2f - 1f;

                        m_renderCamera.rect = new Rect(0f, 0f, 1f, 1f);
                        m_renderCamera.ResetAspect();
                        m_renderCamera.SetOffsetX(sx);
                    }
                }

                m_lastPivotScreenPosition = m_pivotScreenPosition;
            }
        }

#if UNITY_EDITOR
        //--------------------------------------------------
        /// 初始化Camrea
        /// @camera
        /// @formScript
        //--------------------------------------------------
        public void InitializeCamera(Camera camera, Light light, CUIFormScript formScript)
        {
            if (m_renderCamera == null)
            {
                m_renderCamera = camera;
            }
            
            m_renderLight = light;
            m_belongedFormScript = formScript;

            InitializeRender();
        }
#endif

        //--------------------------------------------------
        /// 添加需要渲染到3DImage的GameObject
        /// @path
        /// @useGameObjectPool
        /// @needCached
        //--------------------------------------------------
        public GameObject AddGameObject(string path, bool useGameObjectPool, bool needCached = false)
        {
            return AddGameObject(path, useGameObjectPool, ref m_pivotScreenPosition, true, needCached, null);
        }

        //--------------------------------------------------
        /// 添加需要渲染到3DImage的GameObject
        /// @path
        /// @useGameObjectPool
        /// @screenPosition
        /// @needCached
        //--------------------------------------------------
        public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool needCached = false)
        {
            return AddGameObject(path, useGameObjectPool, ref screenPosition, false, needCached, null);
        }

        //--------------------------------------------------
        /// 添加需要渲染到3DImage的GameObject
        /// @path
        /// @useGameObjectPool
        /// @screenPosition
        /// @pathToAdd
        //--------------------------------------------------
        public GameObject AddGameObjectToPath(string path, bool useGameObjectPool, string pathToAdd)
        {
            return AddGameObject(path, useGameObjectPool, ref m_pivotScreenPosition, false, false, pathToAdd);
        }        

        //--------------------------------------------------
        /// 添加需要渲染到3DImage的GameObject
        /// @path
        /// @useGameObjectPool
        /// @screenPosition
        /// @bindPivot
        //--------------------------------------------------
        public GameObject AddGameObject(string path, bool useGameObjectPool, ref Vector2 screenPosition, bool bindPivot, bool needCached = false, string pathToAdd = null)
        {
            GameObject gameObject = null;

            if (useGameObjectPool)
            {
              //  gameObject = CGameObjectPool.GetInstance().GetGameObject(path, enResourceType.UI3DImage);
            }
            else
            {
                GameObject prefab = new GameObject();// (GameObject)CResourceManager.GetInstance().GetResource(path, typeof(GameObject), enResourceType.UI3DImage, needCached).m_content;
                if (prefab != null)
                {
                    gameObject = (GameObject)GameObject.Instantiate(prefab);
                }                
            }

            if (gameObject == null)
            {
                return null;
            }

            Vector3 localScale = gameObject.transform.localScale;
            if (pathToAdd == null)
            {
                gameObject.transform.SetParent(this.gameObject.transform, true);    
            }
            else
            {
                Transform parentTrans = this.gameObject.transform.Find(pathToAdd);
                if (parentTrans)
                {
                    gameObject.transform.SetParent(parentTrans, true);   
                }
            }
            
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            //gameObject.transform.localScale = localScale; //不要设置，应该让unity根据父容器大小自动计算新加入节点的scale变化，让其保持原来大小。
            

            C3DGameObject _3DGameObject = new C3DGameObject();
            _3DGameObject.m_gameObject = gameObject;
            _3DGameObject.m_path = path;
            _3DGameObject.m_useGameObjectPool = useGameObjectPool;
            _3DGameObject.m_protogenic = false;
            _3DGameObject.m_bindPivot = bindPivot;

            m_3DGameObjects.Add(_3DGameObject);

            if (m_renderCamera.orthographic)
            {
                //设置粒子缩放 会有和原始位置大小对不上的问题
                //InitializeParticleScaler(gameObject, m_belongedFormScript.gameObject.transform.localScale.x / m_renderCameraSize);

                //修改位置
                ChangeScreenPositionToWorld(gameObject, ref screenPosition);

                if (!m_renderCamera.enabled && m_3DGameObjects.Count > 0)
                {
                    m_renderCamera.enabled = true;
                }
            }
            else
            {
                Transform modelRoot = transform.Find("_root");
                if (modelRoot != null)
                {
                    if (pathToAdd == null)
                    {
                        gameObject.transform.SetParent(modelRoot, true);
                    }
                    else
                    {
                        Transform parentTrans = this.gameObject.transform.Find(pathToAdd);
                        if (parentTrans)
                        {
                            gameObject.transform.SetParent(parentTrans, true);
                        }
                    }                    
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = localScale;
                }
            }

            //设置Layer
            CUIUtility.SetGameObjectLayer(gameObject, (m_renderCamera.enabled) ? s_cameraLayers[(int)m_imageLayer] : CUIUtility.c_hideLayer);

            return gameObject;
        }

        //--------------------------------------------------
        /// 移除GameObject
        /// @path
        //--------------------------------------------------
        public void RemoveGameObject(string path)
        {
            for (int i = 0; i < m_3DGameObjects.Count; )
            {
                if (string.Equals(m_3DGameObjects[i].m_path, path, StringComparison.OrdinalIgnoreCase))
                {
                    if (m_3DGameObjects[i].m_useGameObjectPool)
                    {
                     //   CGameObjectPool.GetInstance().RecycleGameObject(m_3DGameObjects[i].m_gameObject);
                    }
                    else
                    {
                      //  CUICommonSystem.DestoryObj(m_3DGameObjects[i].m_gameObject);
                    }

                    m_3DGameObjects.RemoveAt(i);
                    continue;
                }

                i++;
            }

            if (m_3DGameObjects.Count <= 0)
            {
                m_renderCamera.enabled = false;
            }
        }

        //--------------------------------------------------
        /// 移除GameObject
        /// @path
        //--------------------------------------------------
        public void RemoveGameObject(GameObject removeObj)
        {
            if (removeObj == null)
            {
                return;
            }

            for (int i = 0; i < m_3DGameObjects.Count; )
            {
                if (m_3DGameObjects[i].m_gameObject == removeObj)
                {
                    if (m_3DGameObjects[i].m_useGameObjectPool)
                    {
                      //  CGameObjectPool.GetInstance().RecycleGameObject(m_3DGameObjects[i].m_gameObject);
                    }
                    else
                    {
                      //  CUICommonSystem.DestoryObj(m_3DGameObjects[i].m_gameObject);
                    }

                    m_3DGameObjects.RemoveAt(i);
                    continue;
                }

                i++;
            }

            if (m_3DGameObjects.Count <= 0)
            {
                m_renderCamera.enabled = false;
            }
        }

        //--------------------------------------------------
        /// 查找GameObject
        /// @path
        //--------------------------------------------------
        public GameObject GetGameObject(string path)
        {
            for (int i = 0; i < m_3DGameObjects.Count; i++)
            {
                if (m_3DGameObjects[i].m_path.Equals(path))
                {
                    return m_3DGameObjects[i].m_gameObject;
                }
            }

            return null;
        }

        //--------------------------------------------------
        /// GameObject屏幕坐标转世界坐标
        /// @path
        /// @screenPosition
        //--------------------------------------------------
        public void ChangeScreenPositionToWorld(string path, ref Vector2 screenPosition)
        {
            ChangeScreenPositionToWorld(GetGameObject(path), ref screenPosition);
        }

        //--------------------------------------------------
        /// GameObject屏幕坐标转世界坐标
        /// @gameObject
        /// @screenPosition
        //--------------------------------------------------
        public void ChangeScreenPositionToWorld(GameObject gameObject, ref Vector2 screenPosition)
        {
            if (gameObject == null)
            {
                return;
            }

            gameObject.transform.position = CUIUtility.ScreenToWorldPoint(m_renderCamera, screenPosition, 100f/*gameObject.transform.position.z*/);
        }

        //--------------------------------------------------
        /// 获取3D Image枢轴点的屏幕坐标
        //--------------------------------------------------
        public Vector2 GetPivotScreenPosition()
        {
            m_pivotScreenPosition = CUIUtility.WorldToScreenPoint(m_belongedFormScript.GetCamera(), this.gameObject.transform.position);
            return m_pivotScreenPosition;
        }

        //--------------------------------------------------
        /// 初始化Render
        //--------------------------------------------------
        public void InitializeRender()
        {
            if (m_renderCamera != null)
            {
                m_renderCamera.clearFlags = CameraClearFlags.Depth;
                m_renderCamera.cullingMask = 1 << s_cameraLayers[(int)m_imageLayer];
                m_renderCamera.depth = s_cameraDepths[(int)m_imageLayer];

                if (m_renderCamera.orthographic)
                {
                    //这样子改只是针对粒子
                    m_renderCamera.orthographicSize = m_renderCameraDefaultSize * ((m_belongedFormScript.transform as RectTransform).rect.height / m_belongedFormScript.GetReferenceResolution().y);
                }
                else
                {
                    m_renderCamera.gameObject.transform.localScale = m_renderCameraDefaultScale * (1 / ((m_belongedFormScript.gameObject.transform.localScale.x == 0) ? 1 : m_belongedFormScript.gameObject.transform.localScale.x));
                }
            }

            if (m_renderLight != null)
            {
                m_renderLight.cullingMask = 1 << s_cameraLayers[(int)m_imageLayer];
            }
        }

        //--------------------------------------------------
        /// 初始化3D GameObjects
        //--------------------------------------------------
        private void Initialize3DGameObjects()
        {
            m_3DGameObjects.Clear();

            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                GameObject child = this.gameObject.transform.GetChild(i).gameObject;

                //设置层次
                CUIUtility.SetGameObjectLayer(child, s_cameraLayers[(int)m_imageLayer]);

                if (m_renderCamera.orthographic)
                {
                    //设置粒子缩放
                    //InitializeParticleScaler(child, m_belongedFormScript.gameObject.transform.localScale.x);

                    ChangeScreenPositionToWorld(child, ref m_pivotScreenPosition);
                }

                C3DGameObject _3DGameObject = new C3DGameObject();
                _3DGameObject.m_path = child.name;
                _3DGameObject.m_gameObject = child;
                _3DGameObject.m_useGameObjectPool = false;
                _3DGameObject.m_protogenic = true;
                _3DGameObject.m_bindPivot = true;

                m_3DGameObjects.Add(_3DGameObject);
            }

            m_renderCamera.enabled = (m_3DGameObjects.Count > 0);
        }

        //--------------------------------------------------
        /// 初始化粒子系统Scaler
        /// @gameObject
        /// @scale
        //--------------------------------------------------
//         private void InitializeParticleScaler(GameObject gameObject, float scale)
//         {
//             if (gameObject.GetComponent<ParticleSystem>() != null)
//             {
//                 ParticleScaler particleScaler = gameObject.AddComponent<ParticleScaler>();
//                 particleScaler.particleScale = scale;
//             }
// 
//             for (int i=0; i<gameObject.transform.childCount; i++)
//             {
//                 InitializeParticleScaler(gameObject.transform.GetChild(i).gameObject, scale);
//             }
//         }
    }
};