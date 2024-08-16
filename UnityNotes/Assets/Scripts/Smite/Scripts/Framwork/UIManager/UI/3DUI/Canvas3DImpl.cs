using UnityEngine;
using System.Collections.Generic;

namespace Framework
{


    public class Canvas3DImpl : Singleton<Canvas3DImpl>
    {
        private DictionaryView<int, Sprite3D> m_childSprites = new DictionaryView<int, Sprite3D>();
        private DictionaryView<int, TextMesh> m_childText = new DictionaryView<int, TextMesh>();
        private int m_depth = 0;
        private bool m_needRefreshLayout = false;
        private DictionaryView<string, AutoAtlasInfo> m_atlas = new DictionaryView<string, AutoAtlasInfo>();
        private bool m_needRebuildAtlas = false;

        private class AutoAtlasInfo
        {
            private static int[] textureSize = 
        {
            128,
            256,
            512,
            1024
        };
            private int counter = 1;
            private DictionaryView<int, AtlasInfo.UVDetail> textures = new DictionaryView<int, AtlasInfo.UVDetail>();  //Texture的GUID和uv的映射关系
            public bool needRebuildAtlas = false;
            private bool needCompress = false;
            private int padding = 0;
            private HashSet<Sprite3D> sprites = new HashSet<Sprite3D>();
            public Texture2D atlas = null;
            public Texture2D altasAlpha = null; //做纹理压缩用的
            private Material mat = null;
            private Dictionary<int, Texture2D> waitForCombineTextures = new Dictionary<int, Texture2D>();

            public void Register(Sprite3D sprite)
            {
                if (null == sprite || null == sprite.texture)
                {
                    return;
                }

#if UNITY_4_7
                int id = sprite.texture.GetNativeTextureID();
#else
                int id = (int)sprite.texture.GetNativeTexturePtr();
#endif
                sprite.m_textureGUID = id;
                AtlasInfo.UVDetail uv = null;
                padding = Mathf.Max(padding, sprite.padding);
                needCompress = needCompress | sprite.compress;
                if (textures.TryGetValue(id, out uv))
                {
                    sprites.Add(sprite);
                    if (null != mat)
                    {
                        sprite.SetMaterial(mat);
                    }
                    sprite.SetAutoAtlas(atlas, uv);
                    return;
                }
                else
                {
                    uv = new AtlasInfo.UVDetail();
                    uv.width = 0;
                    uv.height = 0;
                    uv.width = sprite.texture.width;
                    uv.height = sprite.texture.height;
                    uv.rotate = false;
                    textures.Add(id, uv);
                    waitForCombineTextures.Add(id, sprite.texture);
                }
                needRebuildAtlas = true;
                sprites.Add(sprite);
            }

            public void Unregister(Sprite3D sprite)
            {
                sprites.Remove(sprite);
                if (sprites.Count == 0)
                {
                    textures.Clear();
                    if (mat != null)
                    {
                        Object.Destroy(mat);
                    }
                    mat = null;
                    if (atlas != null)
                    {
                        Object.Destroy(atlas);
                    }
                    atlas = null;
                }
            }

            public void Rebuild()
            {
                needRebuildAtlas = false;

                bool succ = false;
                for (int i = 0; i < textureSize.Length; ++i)
                {
                    if (atlas != null && textureSize[i] < atlas.width)
                    {
                        continue;
                    }
                    succ = Pack(textureSize[i]);
                    if (succ)
                    {
                        break;
                    }
                }
                if (!succ)
                {
                    var it = sprites.GetEnumerator();
                    it.MoveNext();
                    Debug.LogError("Dynamic Combine Atlas Failed, maybe too many pictures of atlas tag:\"" + it.Current.autoAtlasTag + "\"");
                }

            }

            private bool Pack(int size)
            {
#if UNITY_EDITOR
            //if(Assets.Scripts.GameLogic.BattleLogic.GetInstance().isFighting)
            //{
            //    Debug.LogError("在运行时进行了动态合并批次，会顿卡的!!!");
            //}
#endif
                int columnPosition = 0;
                int x = 0;
                int y = 0;
                int spacing = padding;
                DictionaryView<int, AtlasInfo.UVDetail> tmp = new DictionaryView<int, AtlasInfo.UVDetail>();
                var iter = textures.GetEnumerator();
                while (iter.MoveNext())
                {
                    int width = iter.Current.Value.width;
                    int height = iter.Current.Value.height;
                    AtlasInfo.UVDetail uv = new AtlasInfo.UVDetail();
                    uv.rotate = false;
                    tmp.Add(iter.Current.Key, uv);
                    if (y + height + spacing <= size && x + width + spacing <= size)
                    {
                        uv.x = x;
                        uv.y = y;
                        uv.width = width;
                        uv.height = height;
                        y += height + spacing;
                        if (columnPosition < x + width + spacing)
                            columnPosition = x + width + spacing;
                    }
                    else if (columnPosition + width <= size && height <= size)
                    {
                        x = columnPosition;
                        uv.x = x;
                        uv.y = 0;
                        uv.width = width;
                        uv.height = height;
                        y = height + spacing;
                        columnPosition = x + width + spacing;
                    }
                    else
                    {
                        return false;
                    }
                }

                TextureFormat format = TextureFormat.ARGB32;
                if (needCompress)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                format = TextureFormat.ARGB32;     //format = TextureFormat.ETC_RGB4;
#elif UNITY_IOS && !UNITY_EDITOR
                format = TextureFormat.ARGB32; //format = TextureFormat.PVRTC_RGBA4;
#else
                    format = TextureFormat.ARGB32;
#endif
                }
                Texture2D newTex = new Texture2D(size, size, format, false);
                Color[] clearColor = new Color[newTex.width * newTex.height]; //默认全是0
                newTex.SetPixels(clearColor);
                newTex.name = "Auto_UI3D_Atlas_" + size + "_" + counter + "_format" + format.ToString();

                counter++;
                iter.Reset();
                while (iter.MoveNext())
                {
                    Texture2D tex = null;
                    if (!waitForCombineTextures.TryGetValue(iter.Current.Key, out tex))
                    {
                        tex = this.atlas;
                    }
                    var uv = iter.Current.Value;
                    var uvNew = tmp[iter.Current.Key];
                    Color[] orig = tex.GetPixels(uv.x, uv.y, uv.width, uv.height);
                    newTex.SetPixels(uvNew.x, uvNew.y, uv.width, uv.height, orig);
                    newTex.Apply(false, false);



                    uvNew.uvTL = new Vector2((float)uvNew.x / newTex.width, (float)(uvNew.y + uvNew.height) / newTex.height);
                    uvNew.uvTR = new Vector2((float)(uvNew.x + uvNew.width) / newTex.width, (float)(uvNew.y + uvNew.height) / newTex.height);
                    uvNew.uvBL = new Vector2((float)uvNew.x / newTex.width, (float)(uvNew.y) / newTex.height);
                    uvNew.uvBR = new Vector2((float)(uvNew.x + uvNew.width) / newTex.width, (float)(uvNew.y) / newTex.height);
                }
                textures = tmp;
                Object.Destroy(atlas);
                atlas = newTex;


                Shader shader = null;// CResourceManager.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
                mat = new Material(shader);
                mat.SetTexture("_MainTex", atlas);

                var iterSprites = sprites.GetEnumerator();
                while (iterSprites.MoveNext())
                {
                    iterSprites.Current.SetMaterial(mat);
                    iterSprites.Current.SetAutoAtlas(atlas, textures[iterSprites.Current.m_textureGUID]);
                }
            //    var iterWaitForRelease = waitForCombineTextures.GetEnumerator();
                waitForCombineTextures.Clear();
                //   CResourceManager.GetInstance().UnloadUnusedAssets();
                return true;
            }
        }

        public void Reset()
        {
            m_childSprites.Clear();
            m_childText.Clear();
#if UNITY_EDITOR
        var iter = m_atlas.GetEnumerator();
        while(iter.MoveNext())
        {
            if(iter.Current.Value.atlas != null)
            {
                Debug.LogError("还有3D UI自动生成的altas没有清理干净！");
            }
        }
#endif
            m_atlas.Clear();
            //   CResourceManager.GetInstance().UnloadUnusedAssets();
        }

        public void Update(Transform root)
        {
            if (m_needRefreshLayout)
            {
                _DoRefreshLayout(root);
                m_needRefreshLayout = false;
            }
            if (m_needRebuildAtlas)
            {
                _DoRebuildAtlas();
                m_needRebuildAtlas = true;
            }
        }

        public void registerAutoAtlas(Sprite3D sprite)
        {
            if (sprite.texture == null)
            {
                return;
            }
#if UNITY_EDITOR
        //非运行时也不做任何合并工作
        if (string.IsNullOrEmpty(sprite.autoAtlasTag) || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
#else
            if (string.IsNullOrEmpty(sprite.autoAtlasTag))
#endif
            {
                //不打图集的直接就生成uv了
                AtlasInfo.UVDetail uv = new AtlasInfo.UVDetail();
                uv.uvBL = new Vector2(0, 0);
                uv.uvTL = new Vector2(0, 1);
                uv.uvBR = new Vector2(1, 0);
                uv.uvTR = new Vector2(1, 1);
                uv.rotate = false;
                uv.x = 0;
                uv.y = 0;
                uv.width = sprite.texture.width;
                uv.height = sprite.texture.height;
                sprite.SetUV(uv);
                return;
            }
            AutoAtlasInfo info = null;
            if (!m_atlas.TryGetValue(sprite.autoAtlasTag, out info))
            {
                info = new AutoAtlasInfo();
                m_atlas.Add(sprite.autoAtlasTag, info);
            }
            info.Register(sprite);
            m_needRebuildAtlas = true;
        }


        public void unregisterAutoAtlas(Sprite3D sprite)
        {
            if (sprite.texture == null || string.IsNullOrEmpty(sprite.autoAtlasTag))
            {
                return;
            }
            AutoAtlasInfo info = null;
            if (!m_atlas.TryGetValue(sprite.autoAtlasTag, out info))
            {
                return;
            }
            info.Unregister(sprite);
        }

        private void _DoRebuildAtlas()
        {
            var iter = m_atlas.GetEnumerator();
            while (iter.MoveNext())
            {
                AutoAtlasInfo info = iter.Current.Value;
                if (!info.needRebuildAtlas)
                {
                    continue;
                }
                info.Rebuild();
            }
        }


        public void registerSprite3D(Sprite3D sprite)
        {
            if (!m_childSprites.ContainsKey(sprite.transform.GetInstanceID()))
            {
                m_childSprites.Add(sprite.transform.GetInstanceID(), sprite);
            }
        }


        public void unregisterSprite3d(Sprite3D sprite)
        {
            m_childSprites.Remove(sprite.transform.GetInstanceID());
        }


        private void RefreshHierachy(Transform root)
        {
            if (!root.gameObject.activeSelf)
            {
                return;
            }

            for (int i = root.childCount - 1; i >= 0; --i)
            {
                RefreshHierachy(root.GetChild(i));
            }

            Sprite3D sprite = null;
            if (m_childSprites.TryGetValue(root.GetInstanceID(), out sprite))
            {
                if (null != sprite)
                {
                    ++m_depth;
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    sprite.PrepareMesh();
                    registerAutoAtlas(sprite);
                    sprite.GenerateMesh();
                    sprite.RefreshAtlasMaterial();
                    sprite.RefreshAutoAtlasMaterial();
                }
#endif
                    sprite.depth = m_depth;
                }
            }
            else
            {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                //Debug.LogError("GameObject \"" + root.name + "\" with Sprite3D is not registered in Canvas3D!");
            }
#endif
                sprite = root.GetComponent<Sprite3D>();
                m_childSprites.Add(root.GetInstanceID(), sprite);
                if (null != sprite)
                {
                    ++m_depth;
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    sprite.PrepareMesh();
                    registerAutoAtlas(sprite);
                    sprite.GenerateMesh();
                    sprite.RefreshAtlasMaterial();
                    sprite.RefreshAutoAtlasMaterial();
                }
#endif
                }
#endif
                    sprite.depth = m_depth;
                }
            }


            TextMesh text = null;
            //针对文本的排序
            if (sprite == null)
            {
                if (m_childText.TryGetValue(root.GetInstanceID(), out text))
                {
                    if (null != text)
                    {
                        ++m_depth;
                        text.offsetZ = (float)m_depth / 10;
                    }
                }
                else
                {
                    text = root.GetComponent<TextMesh>();
                    m_childText.Add(root.GetInstanceID(), text);
                    if (null != text)
                    {
                        ++m_depth;
                        text.offsetZ = (float)m_depth / 10;
                    }
                }
            }
        }

        public void RebuildAtlasImmediately()
        {
            _DoRebuildAtlas();
        }


        public void RefreshLayout(Transform root = null)
        {
            m_needRefreshLayout = true;
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            _DoRefreshLayout(root);
        }
#endif
        }

        private void _DoRefreshLayout(Transform root)
        {
            m_depth = 0;
            RefreshHierachy(root);
        }
    }
}
