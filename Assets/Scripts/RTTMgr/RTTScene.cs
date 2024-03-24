using SFramework;
using System;
using UnityEngine;

namespace SFramework
{
    public class RTTScene
    {
        /// <summary>
        /// RTT����������������λ��ƫ��
        /// </summary>
        protected static int count = 0;

        /// <summary>
        /// �������λ�ü��
        /// </summary>
        protected static int space = 100;

        /// <summary>
        /// scene name
        /// </summary>
        protected string name;

        /// <summary>
        /// root GO
        /// </summary>
        protected GameObject rootGameObject;

        /// <summary>
        /// config GO
        /// </summary>
        protected GameObject configObject;

        /// <summary>
        /// target & cameraLayer
        /// </summary>
        protected int layer = LayerMask.NameToLayer("Default");// PhysicsLayer.Default;

        /// <summary>
        /// camera target
        /// </summary>
        protected Camera camera;

        /// <summary>
        /// camera backgroud
        /// </summary>
        protected Camera cameraBk;

        protected UITexture bkTexture;

        /// <summary>
        /// ���յ���Ⱦ����
        /// </summary>
        protected RenderTexture targetTexture;

        /// <summary>
        /// ��������С
        /// </summary>
        protected int renderTextureWidth = 256;
        protected int renderTextureHeight = 256;

        protected Vector3 rootPosition = Vector3.zero;

        /// <summary>
        /// Ŀ��λ��
        /// </summary>
        protected Vector3 targetPos = Vector3.zero;

        /// <summary>
        /// Ŀ������
        /// </summary>
        protected Vector3 targetScale = Vector3.one;

        /// <summary>
        /// Ŀ��Y����ת
        /// </summary>
        protected float targetRotationY;

        /// <summary>
        /// ��Ⱦģ��
        /// </summary>
        protected RTTModel renderModel;

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// ��Դ����
        /// </summary>
        //protected AssetLoadAgent LoadAgent;

        public float defaultValue = 1f;

        public int loadFlag = 0;

        //>-----------------------------------------------------------------------------
        public Texture TargeTexture
        {
            get { return targetTexture; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        //>-----------------------------------------------------------------------------
        public int RenderTextureWidth
        {
            set
            {
                renderTextureWidth = value;
                if (camera)
                {
                    camera.aspect = renderTextureWidth / (float)renderTextureHeight;
                }
            }
        }

        public int RenderTextureHeight
        {
            set
            {
                renderTextureHeight = value;
                if (camera)
                {
                    camera.aspect = renderTextureWidth / (float)renderTextureHeight;
                }
            }
        }


        //>-----------------------------------------------------------------------------
        public RTTModel Model
        {
            get { return renderModel; }
        }

        //>-----------------------------------------------------------------------------
        public bool Active
        {
            set
            {
                rootGameObject.SetActive(value);

                if (Active == false)
                {
                    //RenderTexture.Destroy(targetTexture);
                    //RenderTexture.ReleaseTemporary(targetTexture);
                    //targetTexture = null;
                    //camera.targetTexture = null;
                }
                else
                {
                    //targetTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 32, RenderTextureFormat.ARGB32);
                    camera.targetTexture = targetTexture;
                }
            }
            get { return rootGameObject != null && rootGameObject.activeSelf; }
        }

        public GameObject GetRootGameObject()
        {
            return rootGameObject;
        }
        //>-----------------------------------------------------------------------------

        public RTTScene(string name, GameObject parent, int layer)
        {
            this.name = name;
            this.layer = layer;
            count++;

            rootGameObject = new GameObject(name);
            rootGameObject.transform.parent = parent.transform;
            rootGameObject.transform.localPosition = new Vector3(space * count, 0);
            rootGameObject.transform.localScale = Vector3.one;
            rootGameObject.SetLayerRecursively(layer);
        }

        //>-----------------------------------------------------------------------------

        public virtual void Init(string sceneConfigPrefab)
        {
            loadFlag++;
            //targetTexture = new RenderTexture(renderTextureWidth, renderTextureHeight,32, RenderTextureFormat.ARGB32);
            //targetTexture.name = name;
            if (!LoadSceneResource(sceneConfigPrefab))
                return;

            // �󶨵����ڵ�
            configObject = new GameObject();// (GameObject)GameObject.Instantiate(LoadAgent.AssetObject);

            configObject.transform.parent = rootGameObject.transform;
            configObject.transform.localPosition = Vector3.zero;
            configObject.transform.localScale = Vector3.one;
            configObject.SetLayerRecursively(layer);

            // Camera
            camera = configObject.transform.Find("Camera").GetComponent<Camera>();
            Transform tmp = configObject.transform.Find("CameraBK");
            if (tmp != null)
                cameraBk = tmp.GetComponent<Camera>();
            tmp = configObject.transform.FindChildRecursively("BK");
            if (tmp != null)
                bkTexture = tmp.GetComponent<UITexture>();

            if (camera == null)
            {
                Debug.LogError("Can`t found camera component!");
                return;
            }

            initCameraSize = camera.orthographicSize;
            initCameraPos = camera.transform.localPosition;
            initCameraHeight = camera.transform.localPosition.y;
            if (cameraHeight != defaultValue)
            {
                camera.transform.localPosition = new Vector3(initCameraPos.x, cameraHeight, initCameraPos.z);
            }

            if (cameraDistance != defaultValue)
            {
                camera.orthographicSize = cameraDistance;
            }

            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            camera.cullingMask = 1 << layer;
            camera.aspect = renderTextureWidth / (float)renderTextureHeight;
            camera.targetTexture = targetTexture;


        }

        //>-----------------------------------------------------------------------------

        protected virtual bool LoadSceneResource(string sceneConfigPrefab)
        {
            //LoadAgent = GreatWall.ResourceMgr.ins.LoadAssetFromeAssetsFolderFirstSync(ResourcesPath.UIPrefabPath,
            //    sceneConfigPrefab, "prefab", typeof(UnityEngine.Object), null, loadFlag);

            //if (LoadAgent.AssetObject == null)
            //{
            //    Debug.LogError("RTT scene prefab is null!");
            //    return false;
            //}

            //if (loadFlag != (int)LoadAgent.UserData)
            //{
            //    LoadAgent.Release();
            //    return false;
            //}
            return true;
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// ���ٳ���
        /// </summary>
        public virtual void Destory()
        {
            if (renderModel != null)
            {
                renderModel.DestoryModel();
            }

            //if (LoadAgent != null) LoadAgent.Release();

            ReleaseRenderTexture();
            GameObject.Destroy(rootGameObject);
        }

        public virtual void Update()
        {
            if (Active && targetTexture != null && !targetTexture.IsCreated())
            {
                //RenderTexture.Destroy(targetTexture);
                //RenderTexture.ReleaseTemporary(targetTexture);
                //targetTexture = null;

                Active = true;
            }
        }

        //>-----------------------------------------------------------------------------

        private Action loadComplateAction = null;

        /// <summary>
        /// ������Ⱦ��ģ��
        /// </summary>
        /// <param name="model"></param>
        public virtual void SetModel(RTTModel newModel, Action loadComplate = null)
        {
            if (renderModel != null)
            {
                string oldModelPath = renderModel.modelPath;
                string newModelPath = newModel.modelPath;

                if (renderModel == newModel)
                {
                    // ���һ�¶���
                    if (!renderModel.IsPlayingAnimation())
                    {
                        renderModel.PlayModelAction(PlayerAnimName.Stand, WrapMode.Loop);
                    }

                    return;
                }
            }

            if (renderModel != null)
            {
                renderModel.DestoryModel();
                renderModel = null;
            }

            renderModel = newModel;
            renderModel.SetLayer(layer);
            renderModel.SetParent(rootGameObject);
            renderModel.SetTransform(targetPos, Quaternion.AngleAxis(targetRotationY, Vector3.up), targetScale);

            renderModel.LoadModel(LoadModelComplate);
            loadComplateAction = loadComplate;

        }

        protected virtual void LoadModelComplate()
        {
            if (loadComplateAction != null)
            {
                loadComplateAction();
            }
        }

        //>-----------------------------------------------------------------------------
        public virtual void SetTargetTransform(Vector3 pos, float rotationY, Vector3 scale)
        {
            targetPos = pos;
            targetRotationY = rotationY;
            targetScale = scale;

            if (configObject != null)
            {
                configObject.transform.localPosition = targetPos;

                camera = configObject.GetComponentInChildren<Camera>();

                if (camera == null)
                {
                    Debug.LogError("Can`t found camera component!");
                    return;
                }

                initCameraSize = camera.orthographicSize;
                initCameraPos = camera.transform.localPosition;

                if (cameraHeight != defaultValue)
                {
                    camera.transform.localPosition = new Vector3(targetPos.x, cameraHeight, initCameraPos.z);
                }
            }

            if (renderModel != null)
            {
                renderModel.SetTransform(targetPos, Quaternion.AngleAxis(targetRotationY, Vector3.up), targetScale);
            }
        }

        //>-----------------------------------------------------------------------------

        public float initCameraSize = 0f;
        public float initCameraHeight = 0f;
        public Vector3 initCameraPos = Vector3.zero;

        protected float cameraHeight = 999f;
        protected float cameraDistance = 999f;
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="height">����߶�</param>
        /// <param name="distance">�������</param>
        public virtual void SetCameraParm(float height, float distance)
        {
            cameraHeight = height;
            cameraDistance = distance;

            if (camera != null)
            {
                if (cameraHeight == defaultValue)
                {
                    camera.transform.localPosition = new Vector3(initCameraPos.x, initCameraHeight, initCameraPos.z);
                }
                else
                {
                    camera.transform.localPosition = new Vector3(initCameraPos.x, cameraHeight, initCameraPos.z);
                }

                if (cameraDistance != defaultValue && cameraDistance != 0)
                {
                    camera.orthographicSize = cameraDistance;
                }
                else
                {
                    camera.orthographicSize = initCameraSize;
                }

                CreateRenderTexture(renderTextureWidth, renderTextureHeight);
                camera.targetTexture = targetTexture;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.enabled = true;
            }

            if (cameraBk != null)
            {
                cameraBk.enabled = false;
            }
        }

        /// <summary>
        /// ����UI���֣� ��ģ�ͻ�������ͼ��ĳ��������
        /// </summary>
        /// <param name="uiModelSize">ģ�ͻ���ui�ϵĴ�С��vector4(left, bottom, width, height)</param>
        /// <param name="bkSize">������Ĵ�С</param>
        /// <param name="texture">����ͼƬ</param>
        /// <param name="rtScale">RenderTexture����</param>
        public void SetCameraParm(float height, float distance, Vector4 uiModelSize, Vector2 bkSize, Texture texture, float rtScale = 1)
        {
            rtScale = rtScale <= 0 ? 1 : rtScale;
            renderTextureWidth = (int)(bkSize.x * rtScale);
            renderTextureHeight = (int)(bkSize.y * rtScale);

            //����modelCamera
            SetCameraParm(height, distance);
            if (cameraBk == null || camera == null)
                return;

            //����modelCamera viewport
            camera.rect = new Rect(uiModelSize.x / bkSize.x, uiModelSize.y / bkSize.y, uiModelSize.z / bkSize.x, uiModelSize.w / bkSize.y);


            //����BKCamera
            if (bkTexture != null)
            {
                bkTexture.mainTexture = texture;
                bkTexture.width = (int)bkSize.x;
                bkTexture.height = (int)bkSize.y;
            }

            cameraBk.enabled = true;

            cameraBk.projectionMatrix = Matrix4x4.Ortho(-bkSize.x / 200, bkSize.x / 200, -bkSize.y / 200, bkSize.y / 200, -10, 10000);
            cameraBk.depth = 0;
            cameraBk.targetTexture = targetTexture;

            camera.depth = 1;
            camera.clearFlags = CameraClearFlags.Nothing;
        }

        public void ShowBgTexture()
        {
            if (cameraBk == null || camera == null)
                return;

            cameraBk.enabled = true;
            cameraBk.depth = 0;
            cameraBk.targetTexture = targetTexture;

            camera.depth = 1;
            camera.clearFlags = CameraClearFlags.Nothing;
        }

        /// <summary>
        /// ���� ��Ϊ����
        /// </summary>
        public void SetCaptureScreen(Camera screenCamera, Rect rect)
        {
            // ����һ��RenderTexture����
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            screenCamera.targetTexture = rt;
            screenCamera.Render();

            RenderTexture.active = rt;

            //��Ҫ��ȷ���ú�ͼƬ�����ʽ
            Texture2D texture2D = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            //�����趨�����ȡ���أ�ע���������½�Ϊԭ���ȡ
            texture2D.ReadPixels(rect, 0, 0, false);
            texture2D.Apply();

            //����BKCamera
            if (bkTexture != null)
            {
                bkTexture.mainTexture = texture2D;
                bkTexture.width = (int)rect.width;
                bkTexture.height = (int)rect.height;
            }
            cameraBk.enabled = true;
            cameraBk.projectionMatrix = Matrix4x4.Ortho(-rect.width / 200, rect.width / 200, -rect.height / 200, rect.height / 200, -10, 10000);
            cameraBk.depth = 0;
            cameraBk.targetTexture = targetTexture;
            camera.depth = 1;
            camera.clearFlags = CameraClearFlags.Nothing;

            screenCamera.targetTexture = null;
        }

        private void CreateRenderTexture(int width, int height)
        {
            if (renderTextureWidth == height && renderTextureWidth == width && targetTexture != null)
                return;

            ReleaseRenderTexture();
            targetTexture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32);
            targetTexture.name = name;
        }

        private void ReleaseRenderTexture()
        {
            if (targetTexture != null)
            {
                RenderTexture.ReleaseTemporary(targetTexture);
                targetTexture = null;
            }
        }
    }
}
