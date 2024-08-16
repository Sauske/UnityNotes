using UnityEngine;
using System.Collections;

namespace Framework
{


    public class Sprite3D : MonoBehaviour
    {

        [System.Serializable]
        public enum EnumVertical
        {
            Top,
            Middle,
            Bottom
        }

        [System.Serializable]
        public enum EnumHoriontal
        {
            Left,
            Center,
            Right
        }

        [System.Serializable]
        public enum EnumFillType
        {
            Horiontal,
            Vertical,
            Radial360
        }

        public struct SpriteAttr
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }

        public static readonly int TRANSPARENT_RENDER_QUEUE = 3000;

        [UnityEngine.SerializeField]
        private bool m_useAtlas = true;

        [UnityEngine.SerializeField]
        private AtlasInfo m_atlas;
        [UnityEngine.SerializeField]
        private string m_spriteName;
        [System.NonSerialized]
        private string m_lastAtlasName;

        [UnityEngine.SerializeField]
        private Texture2D m_texture;
        [UnityEngine.SerializeField]
        private string m_tag;
        [UnityEngine.SerializeField]
        public bool compress = true;
        [UnityEngine.SerializeField]
        public int padding = 0;
        [System.NonSerialized]
        private SpriteAttr m_spriteAttr;
        [System.NonSerialized]
        public int m_textureGUID;


        [UnityEngine.SerializeField]
        private float m_width = 1f;
        [UnityEngine.SerializeField]
        private float m_height = 1f;
        [UnityEngine.SerializeField]
        private EnumVertical m_alignVertical = EnumVertical.Middle;
        [UnityEngine.SerializeField]
        private EnumHoriontal m_alignHoriontal = EnumHoriontal.Center;

        [UnityEngine.SerializeField]
        private EnumFillType m_fillType = EnumFillType.Horiontal;
        [UnityEngine.SerializeField]
        private float m_fillAmount = 1.0f;
        [UnityEngine.SerializeField]
        private Color m_color = Color.white;
        [UnityEngine.SerializeField]
        private float m_depth = 1;

        //给扇形用的
        [UnityEngine.SerializeField]
        private uint m_segments = 50;


        [System.NonSerialized]
        private bool m_propchanged = true;
        [System.NonSerialized]
        private Mesh m_mesh = null;
        [System.NonSerialized]
        private MeshRenderer m_render = null;

        [System.NonSerialized]
        private AtlasInfo.UVDetail m_uv = null;

        void Awake()
        {
            m_lastAtlasName = null;
            m_propchanged = true;
            m_depth = Mathf.Max(1, m_depth);
            m_mesh = null;
            PrepareMesh();
            RefreshUVDetail();
        }

        void Start()
        {
            Canvas3DImpl.GetInstance().registerSprite3D(this);
            Canvas3DImpl.GetInstance().registerAutoAtlas(this);
            RefreshAtlasMaterial();
        }

        void OnDestroy()
        {
            Canvas3DImpl.GetInstance().unregisterSprite3d(this);
            Canvas3DImpl.GetInstance().unregisterAutoAtlas(this);
        }


        void OnEnable()
        {
            Canvas3DImpl.GetInstance().RefreshLayout();
        }

        void Update()
        {

        }

        void LateUpdate()
        {
            if (m_propchanged == true)
            {
                GenerateMesh();
                m_propchanged = false;
            }
        }

        #region Properties
        public bool useAtlas
        {
            get
            {
                return m_useAtlas;
            }
            set
            {
                if (value == m_useAtlas)
                {
                    return;
                }
                m_useAtlas = value;
                if (m_useAtlas)
                {
                    m_texture = null;
                }
                else
                {
                    m_atlas = null;
                }
            }
        }

        public float fillAmount
        {
            get
            {
                return m_fillAmount;
            }
            set
            {
                if (m_fillAmount == value) return; m_fillAmount = value; m_propchanged = true;
            }
        }

        public Color color
        {
            get
            {
                return m_color;
            }
            set
            {
                if (m_color == value) return; m_color = value; m_propchanged = true;
            }
        }

        public float depth
        {
            get
            {
                return m_depth;
            }
            set
            {
                m_depth = value;
                RecaculateDepth();
            }
        }


        public AtlasInfo atlas
        {
            get
            {
                return m_atlas;
            }
            set
            {
                if (m_atlas == value)
                    return;
                m_atlas = value;
                useAtlas = true;
                m_propchanged = true;
            }
        }

        public string spriteName
        {
            get
            {
                return m_spriteName;
            }
            set
            {
                if (m_spriteName == value) return; m_spriteName = value; m_propchanged = true;
            }
        }

        public Texture2D texture
        {
            get
            {
                return m_texture;
            }
            set
            {
                if (m_texture == value)
                    return;
                m_texture = value;
                useAtlas = false;
                Canvas3DImpl.GetInstance().registerAutoAtlas(this);
                m_propchanged = true;

#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GenerateMesh();
            }
#endif
            }
        }

        public int textureWidth
        {
            get
            {
                if (m_uv == null)
                {
                    if (m_texture != null)
                    {
                        return m_texture.width;
                    }
                    return 0;
                }
                return m_uv.width;
            }
        }

        public int textureHeight
        {
            get
            {
                if (m_uv == null)
                {
                    if (m_texture != null)
                    {
                        return m_texture.height;
                    }
                    return 0;
                }
                return m_uv.height;
            }
        }

        public float HalfTextureWidth
        {
            get { return textureWidth * 0.5f; }
        }

        public float HalfTextureHeight
        {
            get { return textureHeight * 0.5f; }
        }


        public void SetAutoAtlas(Texture2D atlas, AtlasInfo.UVDetail uv)
        {
            m_texture = atlas;
            SetUV(uv);
        }

        public string autoAtlasTag
        {
            get
            {
                if (string.IsNullOrEmpty(m_tag))
                {
                    return m_tag;
                }
                return m_tag.Trim();
            }
            set
            {
                if (m_tag == value)
                    return;
                m_tag = value;
                Canvas3DImpl.GetInstance().registerAutoAtlas(this);
                m_propchanged = true;
            }
        }

        public float width
        {
            get
            {
                return m_width;
            }
            set
            {
                if (m_width == value) return; m_width = value; m_propchanged = true;
            }
        }

        public float height
        {
            get
            {
                return m_height;
            }
            set
            {
                if (m_height == value) return; m_height = value; m_propchanged = true;
            }
        }

        public EnumHoriontal alignHoriontal
        {
            get
            {
                return m_alignHoriontal;
            }
            set
            {
                if (m_alignHoriontal == value) return; m_alignHoriontal = value; m_propchanged = true;
            }
        }

        public EnumVertical alignVertical
        {
            get
            {
                return m_alignVertical;
            }
            set
            {
                if (m_alignVertical == value) return; m_alignVertical = value; m_propchanged = true;
            }
        }

        public EnumFillType fillType
        {
            get
            {
                return m_fillType;
            }
            set
            {
                if (m_fillType == value) return; m_fillType = value; m_propchanged = true;
            }
        }

        public uint segments
        {
            get
            {
                return m_segments;
            }
            set
            {
                if (m_segments == value) return; m_segments = value; m_propchanged = true;
            }
        }
        #endregion

        public void RefreshAtlasMaterial()
        {
            if (m_atlas != null)
            {
                m_render.sharedMaterial = m_atlas.material;
            }
        }


        public void SetMaterial(Material mat)
        {
            Material oldMat = m_render.sharedMaterial;
            m_render.sharedMaterial = mat;
            Object.DestroyObject(oldMat);
        }

        public void RefreshAutoAtlasMaterial()
    {

        if (m_texture != null)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || m_render.sharedMaterial == null)   //如果在编辑模式下，这个material一定要重新生成，因为很有可能这个OBJ是拷贝的，那么他会和源OBJ共享供一个材质球
#else
            if(m_render.sharedMaterial == null)
#endif
            {
#if UNITY_EDITOR
                Shader shader = null;
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                  //  shader = CResourceManager.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
                }
                else
                {
                    shader = Shader.Find("UI/UI3D");
                }

#else
            Shader shader = null/// CResourceManager.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
#endif
                Material mat = new Material(shader);
                mat.SetTexture("_MainTex", texture);
                m_render.sharedMaterial = mat;
            }
            else
            {
                m_render.sharedMaterial.SetTexture("_MainTex", texture);
            }

        }
    }

        private void RefreshUVDetail()
        {
            if (null == m_atlas)
            {
                return;
            }
            if (m_lastAtlasName == m_spriteName)
            {
                return;
            }
            SetUV(m_atlas.GetUV(m_spriteName));
            m_lastAtlasName = m_spriteName;
        }

        public void SetUV(AtlasInfo.UVDetail uv)
        {
            PrepareMesh();
            if (m_mesh == null
                || m_mesh.triangles.Length == 0
                || m_uv == null
                || m_uv.uvBL != uv.uvBL
                || m_uv.uvBR != uv.uvBR
                || m_uv.uvTL != uv.uvTL
                || m_uv.uvTR != uv.uvTR)
            {
                m_propchanged = true;
            }
            m_uv = uv;

            RefreshAutoAtlasMaterial();
        }

        private void RecaculateDepth()
        {
            if (m_mesh == null)
            {
                return;
            }
            Bounds bounds = new Bounds();
            bounds.center = new Vector3(0.5f * m_width, 0.5f * m_height, m_depth / 10);
            bounds.size = new Vector3(m_width, m_height, m_depth / 4 * 2);
            m_mesh.bounds = bounds;
        }

        public void PrepareMesh()
        {
            if (m_mesh == null)
            {
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if (null == meshFilter)
                {
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                }
                m_mesh = new Mesh();
                meshFilter.mesh = m_mesh;

                m_render = gameObject.GetComponent<MeshRenderer>();
                if (null == m_render)
                {
                    m_render = gameObject.AddComponent<MeshRenderer>();
                }
            }
        }

        public void GenerateMesh()
        {
            RefreshUVDetail();
            if (null == m_uv)
            {
                return;
            }
            PrepareMesh();
            m_mesh.Clear();
            if (m_fillType == EnumFillType.Horiontal)
            {
                GenerateHoriontalFillMesh();
            }
            else if (m_fillType == EnumFillType.Vertical)
            {
                GenerateVerticalFillMesh();
            }
            else if (m_fillType == EnumFillType.Radial360)
            {
                GenerateRadial360FillMesh();
            }
        }

        /// <summary>
        /// 假设屏幕都是1280宽度，避免不同分辨率开高清模式变小（很丑陋的patch，实际上做血条UI根本不该用这个函数）
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="depth"></param>
        public void SetNativeSize(Camera camera, float depth)
        {
            //int standardWidth = 1280;
            //int width = Mathf.Max(Screen.width, Screen.height);
            float ratio = Ratio();
            RefreshUVDetail();

            if (camera != null)
            {
                Vector3 basePoint = camera.transform.TransformPoint(0f, 0f, depth);
                Vector3 posInScreen = camera.WorldToScreenPoint(basePoint);

                Vector3 posInScreenWidthDistance = new Vector3(posInScreen.x + textureWidth * ratio, posInScreen.y, posInScreen.z);
                Vector3 posInScreenHeightDistance = new Vector3(posInScreen.x, posInScreen.y + textureHeight * ratio, posInScreen.z);

                Vector3 widthPoint = camera.ScreenToWorldPoint(posInScreenWidthDistance);
                Vector3 heightPoint = camera.ScreenToWorldPoint(posInScreenHeightDistance);

                this.width = Vector3.Distance(basePoint, widthPoint);
                this.height = Vector3.Distance(basePoint, heightPoint);
            }
            else
            {
                DebugHelper.LogError("3DCamera is NULL!!!");
            }
        }

        //
        public void SetNativeSize(Camera camera, float depth, float screenWidth, float screenHeight)
        {
            //int standardWidth = 1280;
            //int width = Mathf.Max(Screen.width, Screen.height);
            //float ratio = Ratio();
            //RefreshUVDetail();

            if (camera != null)
            {
                Vector3 basePoint = camera.transform.TransformPoint(0f, 0f, depth);
                Vector3 posInScreen = camera.WorldToScreenPoint(basePoint);

                Vector3 posInScreenWidthDistance = new Vector3(posInScreen.x + screenWidth, posInScreen.y, posInScreen.z);
                Vector3 posInScreenHeightDistance = new Vector3(posInScreen.x, posInScreen.y + screenHeight, posInScreen.z);

                Vector3 widthPoint = camera.ScreenToWorldPoint(posInScreenWidthDistance);
                Vector3 heightPoint = camera.ScreenToWorldPoint(posInScreenHeightDistance);

                this.width = Vector3.Distance(basePoint, widthPoint);
                this.height = Vector3.Distance(basePoint, heightPoint);
            }
            else
            {
                DebugHelper.LogError("3DCamera is NULL!!!");
            }
        }

        //
        static private float S_Ratio = -1.0f;
        static public float Ratio()
        {
            if (S_Ratio == -1.0f)
            {
                int standardWidth = 960;
                int standardHeight = 640;
                S_Ratio = Mathf.Min((float)Screen.height / standardHeight, (float)Screen.width / standardWidth);
            }
            return S_Ratio;
        }

        static public float SetRatio(int newWidth, int newHeight)
        {
            int standardWidth = 960;
            int standardHeight = 640;
            S_Ratio = Mathf.Min((float)newHeight / standardHeight, (float)newWidth / standardWidth);
            return S_Ratio;
        }

        private void GenerateRadial360FillMesh()
        {
            m_fillAmount = Mathf.Clamp01(m_fillAmount);
            if (m_fillAmount <= 0)
            {
                return;
            }

          //  Vector3 localPosition = transform.localPosition;
            //transform.localPosition = new Vector3(localPosition.x, localPosition.y, m_depth);
            m_mesh.MarkDynamic();

            float left = 0f;
            float bottom = 0f;
            if (m_alignHoriontal == EnumHoriontal.Center)
            {
                left = -0.5f * m_width;
            }
            else if (m_alignHoriontal == EnumHoriontal.Right)
            {
                left = -m_width;
            }
            if (m_alignVertical == EnumVertical.Middle)
            {
                bottom = -0.5f * m_height;
            }
            else if (m_alignVertical == EnumVertical.Top)
            {
                bottom = -m_height;
            }

            int pointsOnSides = (int)(m_segments * m_fillAmount) + 1;
            float step = 2 * (width + height) / (m_segments);

            Vector3[] vertices = new Vector3[pointsOnSides + 1];
            Vector2[] uvs = new Vector2[pointsOnSides + 1];
            vertices[0] = new Vector3(left + 0.5f * m_width, bottom + 0.5f * height, 0f); //中心点
            uvs[0] = m_uv.uvTL.Lerp(m_uv.uvBR, 0.5f);   //uv中心点

            int phase = 0; //0上边右， 1右边， 2下边， 3左边， 4上边左
            int cursor = 0;
            float compensate = 0f;
            for (int i = 0; i < pointsOnSides; ++i)
            {
                if (phase == 0)
                {
                    float x = left + 0.5f * width + cursor * step;
                    if (x >= left + width)
                    {
                        compensate = x - left - width;
                        x = left + width;
                        phase = 1;
                        cursor = 1;
                    }
                    else
                    {
                        cursor++;
                    }
                    vertices[i + 1] = new Vector3(x, bottom + height, 0);
                }
                else if (phase == 1)
                {
                    float y = bottom + height - cursor * step - compensate;
                    if (y <= bottom)
                    {
                        compensate = bottom - y;
                        y = bottom;
                        phase = 2;
                        cursor = 1;
                    }
                    else
                    {
                        cursor++;
                    }
                    vertices[i + 1] = new Vector3(left + width, y, 0);
                }
                else if (phase == 2)
                {
                    float x = left + width - cursor * step - compensate;
                    if (x <= left)
                    {
                        compensate = left - x;
                        x = left;
                        phase = 3;
                        cursor = 1;
                    }
                    else
                    {
                        cursor++;
                    }
                    vertices[i + 1] = new Vector3(x, bottom, 0);
                }
                else if (phase == 3)
                {
                    float y = bottom + cursor * step + compensate;
                    if (y >= bottom + height)
                    {
                        compensate = y - bottom - height;
                        y = bottom + height;
                        phase = 4;
                        cursor = 1;
                    }
                    else
                    {
                        cursor++;
                    }
                    vertices[i + 1] = new Vector3(left, y);
                }
                else if (phase == 4)
                {
                    float x = left + cursor * step + compensate;
                    if (x > left + 0.5f * width)
                    {
                        x = left + 0.5f * width;
                    }
                    ++cursor;
                    vertices[i + 1] = new Vector3(x, bottom + height, 0);
                }
                float vx = vertices[i + 1].x;
                float vy = vertices[i + 1].y;
                uvs[i + 1] = new Vector2(Mathf.Lerp(m_uv.uvTL.x, m_uv.uvTR.x, (vx - left) / width), Mathf.Lerp(m_uv.uvBL.y, m_uv.uvTL.y, (vy - bottom) / height));
            }


            Color[] colors = new Color[pointsOnSides + 1];
            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i] = m_color;
            }

            int[] indices = new int[(pointsOnSides - 1) * 3];
            for (int i = 0; i < pointsOnSides - 1; ++i)
            {
                indices[i * 3] = 0;
                indices[i * 3 + 1] = i + 1;
                indices[i * 3 + 2] = i + 2;
            }

            m_mesh.vertices = vertices;
            m_mesh.uv = uvs;
            m_mesh.colors = colors;
            m_mesh.triangles = indices;

            RecaculateDepth();

        }

        private void GenerateHoriontalFillMesh()
        {
            m_fillAmount = Mathf.Clamp01(m_fillAmount);
            if (m_fillAmount <= 0)
            {
                return;
            }

           // Vector3 localPosition = transform.localPosition;
            //transform.localPosition = new Vector3(localPosition.x, localPosition.y, m_depth);
            m_mesh.MarkDynamic();

            float left = 0f;
            float bottom = 0f;
            if (m_alignHoriontal == EnumHoriontal.Center)
            {
                left = -0.5f * m_width;
            }
            else if (m_alignHoriontal == EnumHoriontal.Right)
            {
                left = -m_width;
            }
            if (m_alignVertical == EnumVertical.Middle)
            {
                bottom = -0.5f * m_height;
            }
            else if (m_alignVertical == EnumVertical.Top)
            {
                bottom = -m_height;
            }


            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(left, bottom + m_height, 0);
            vertices[1] = new Vector3(left + m_width * m_fillAmount, bottom + m_height, 0);
            vertices[2] = new Vector3(left, bottom, 0);
            vertices[3] = new Vector3(left + m_width * m_fillAmount, bottom, 0);

            Vector2[] uvs = new Vector2[4];
            uvs[0] = m_uv.uvTL;
            uvs[1] = m_uv.uvTL.Lerp(m_uv.uvTR, m_fillAmount);
            uvs[2] = m_uv.uvBL;
            uvs[3] = m_uv.uvBL.Lerp(m_uv.uvBR, m_fillAmount);

            Color[] colors = new Color[4] { m_color, m_color, m_color, m_color };

            int[] indices = new int[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 3;
            indices[4] = 2;
            indices[5] = 1;

            m_mesh.vertices = vertices;
            m_mesh.uv = uvs;
            m_mesh.colors = colors;
            m_mesh.triangles = indices;

            RecaculateDepth();

        }

        private void GenerateVerticalFillMesh()
        {
            m_fillAmount = Mathf.Clamp01(m_fillAmount);
            if (m_fillAmount <= 0)
            {
                return;
            }

         //   Vector3 localPosition = transform.localPosition;
            //transform.localPosition = new Vector3(localPosition.x, localPosition.y, m_depth);
            m_mesh.MarkDynamic();

            float left = 0f;
            float bottom = 0f;
            if (m_alignHoriontal == EnumHoriontal.Center)
            {
                left = -0.5f * m_width;
            }
            else if (m_alignHoriontal == EnumHoriontal.Right)
            {
                left = -m_width;
            }
            if (m_alignVertical == EnumVertical.Middle)
            {
                bottom = -0.5f * m_height;
            }
            else if (m_alignVertical == EnumVertical.Top)
            {
                bottom = -m_height;
            }


            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(left, bottom + m_height * m_fillAmount, 0);
            vertices[1] = new Vector3(left + m_width, bottom + m_height * m_fillAmount, 0);
            vertices[2] = new Vector3(left, bottom, 0);
            vertices[3] = new Vector3(left + m_width, bottom, 0);

            Vector2[] uvs = new Vector2[4];
            uvs[0] = m_uv.uvBL.Lerp(m_uv.uvTL, m_fillAmount);
            uvs[1] = m_uv.uvBR.Lerp(m_uv.uvTR, m_fillAmount);
            uvs[2] = m_uv.uvBL;
            uvs[3] = m_uv.uvBR;

            Color[] colors = new Color[4] { m_color, m_color, m_color, m_color };

            int[] indices = new int[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 3;
            indices[4] = 2;
            indices[5] = 1;

            m_mesh.vertices = vertices;
            m_mesh.uv = uvs;
            m_mesh.colors = colors;
            m_mesh.triangles = indices;

            RecaculateDepth();
        }
    }
}
