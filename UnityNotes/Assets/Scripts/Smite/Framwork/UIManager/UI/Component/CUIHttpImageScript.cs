//==================================================================================
/// UI HttpImage 控件
/// @根据传入的url拉取对应的图片资源并显示
/// @neoyang
/// @2015.06.02
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

namespace Framework
{
    //state
    public enum enHttpImageState
    {
        Unload,
        Loading,
        Loaded
    };

    public class CUIHttpImageScript : CUIComponent
    {
        //图片地址
        public string m_imageUrl;

        //是否设置为原始大小
        public bool m_setNativeSize;
        
        //是否缓存纹理
        public bool m_cacheTexture = true;

        //缓存纹理有效期(单位:天数)
        public float m_cachedTextureValidDays = 2f;

        //是否强制设置图片地址（若为false，SetImageUrl时若当前url与上次url一致则不会重置url）
        public bool m_forceSetImageUrl = false;

        //Loading时显示的GameObject
        public GameObject m_loadingCover;

        //UGUI Image
        private Image m_image;
        private ImageAlphaTexLayout m_imageDefaultAlphaTexLayout = ImageAlphaTexLayout.None;  //Image2才需要用到
        private Sprite m_imageDefaultSprite;

        //加载状态
        private enHttpImageState m_httpImageState = enHttpImageState.Unload;

        //缓存相关
        private static CCachedTextureManager s_cachedTextureManager;

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

            //初始化Image
            m_image = this.gameObject.GetComponent<Image>();
            m_imageDefaultSprite = m_image.sprite;

            //若image为Image2，这里还需要保存当前alphaTexLayout
            if (m_image is Image2)
            {
                m_imageDefaultAlphaTexLayout = (m_image as Image2).alphaTexLayout;
            }

            //创建缓存纹理管理器
            if (m_cacheTexture && s_cachedTextureManager == null)
            {
                s_cachedTextureManager = new CCachedTextureManager();
            }

            m_httpImageState = enHttpImageState.Unload;

            //显示LoadingCover
            if (m_loadingCover != null)
            {
                m_loadingCover.CustomSetActive(true);
            }

            //拉取资源
            if (this.gameObject.activeInHierarchy && !string.IsNullOrEmpty(m_imageUrl))
            {
                LoadTexture(m_imageUrl);
            }
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_loadingCover = null;
            m_image = null;
            m_imageDefaultSprite = null;

            base.OnDestroy();
        }

        void OnEnable()
        {
            if (m_isInitialized && m_httpImageState == enHttpImageState.Unload && !string.IsNullOrEmpty(m_imageUrl))
            {
                LoadTexture(m_imageUrl);
            }
        }

        void OnDisable()
        {
            if (m_isInitialized && m_httpImageState == enHttpImageState.Loading)
            {
                StopAllCoroutines();
                m_httpImageState = enHttpImageState.Unload;

                //显示LoadingCover
                if (m_loadingCover != null)
                {
                    m_loadingCover.CustomSetActive(true);
                }
            }
        }

        //--------------------------------------------------
        /// Set Image 链接
        /// @url
        //--------------------------------------------------
        public void SetImageUrl(string url)
        {
            if (!m_forceSetImageUrl && string.Equals(url, m_imageUrl))
            {
                return;
            }

            m_imageUrl = url;

            //刷回默认sprite
            if (m_image != null)
            {
                m_image.SetSprite(m_imageDefaultSprite, m_imageDefaultAlphaTexLayout);
            }

            if (this.gameObject.activeInHierarchy && m_httpImageState == enHttpImageState.Loading)
            {
                StopAllCoroutines();
            }

            m_httpImageState = enHttpImageState.Unload;

            //显示LoadingCover
            if (m_loadingCover != null)
            {
                m_loadingCover.CustomSetActive(true);
            }

            if (this.gameObject.activeInHierarchy && !string.IsNullOrEmpty(m_imageUrl))
            {
                LoadTexture(m_imageUrl);
            }
        }

        /// <summary>
        /// 直接设Sprite
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="formScript"></param>
        public void SetImageSprite(string prefabPath, CUIFormScript formScript)
        {
            if (m_image != null)
            {
                m_image.SetSprite(prefabPath, formScript);
            }
        }

        //--------------------------------------------------
        /// SetNativeSize
        //--------------------------------------------------
        public void SetNativeSize()
        {
            if (m_image != null)
            {
                m_image.SetNativeSize();
            }
        }

        //--------------------------------------------------
        /// 加载纹理
        //--------------------------------------------------
        private void LoadTexture(string url)
        {
            if (m_httpImageState == enHttpImageState.Loaded)
            {
                return;
            }

            if (m_cacheTexture)
            {
                Texture2D texture2D = s_cachedTextureManager.GetCachedTexture(url, m_cachedTextureValidDays);
                if (texture2D != null)
                {
                    if (m_image != null)
                    {
                        m_image.SetSprite(Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);

                        if (m_setNativeSize)
                        {
                            SetNativeSize();
                        }

                        m_httpImageState = enHttpImageState.Loaded;

                        //隐藏LoadingCover
                        if (m_loadingCover != null)
                        {
                            m_loadingCover.CustomSetActive(false);
                        }
                    }
                }
                else
                {
                    StartCoroutine(DownloadImage(url));
                }
            }
            else
            {
                StartCoroutine(DownloadImage(url));
            }
        }

        //--------------------------------------------------
        /// 下载图片
        /// @url
        //--------------------------------------------------
        private IEnumerator DownloadImage(string url)
        {
            m_httpImageState = enHttpImageState.Loading;

         //   float startTime = Time.realtimeSinceStartup;

            WWW www = new WWW(url);
            yield return www;

            //无论下载到的数据正确与否，状态都需要置为Loaded
            m_httpImageState = enHttpImageState.Loaded;

            if (string.IsNullOrEmpty(www.error))
            {
                //隐藏LoadingCover
                if (m_loadingCover != null)
                {
                    m_loadingCover.CustomSetActive(false);
                }

                string contentType = null;
                www.responseHeaders.TryGetValue("CONTENT-TYPE", out contentType);

                if (contentType != null)
                {
                    contentType = contentType.ToLower();
                }

                //检查contentType
                if (string.IsNullOrEmpty(contentType) || !contentType.Contains("image/"))
                {
                    //BeaconHelper.GetInstance().EventPhotoReport("1", Time.realtimeSinceStartup - startTime, "CONTENT-TYPE = " + contentType);
                }
                else
                {
                    bool isGif = string.Equals(contentType, "image/gif");

                    Texture2D texture2D = null;

                    if (isGif)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(www.bytes))
                        {
                            texture2D = GifHelper.GifToTexture(memoryStream, 0);
                        }
                    }
                    else
                    {
                        texture2D = www.texture;
                    }

                    if (texture2D != null)
                    {
                        if (m_image != null)
                        {
                            m_image.SetSprite(Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);

                            if (m_setNativeSize)
                            {
                                SetNativeSize();
                            }
                        }

                        //缓存纹理
                        if (m_cacheTexture)
                        {
                            s_cachedTextureManager.AddCachedTexture(url, texture2D.width, texture2D.height, isGif, www.bytes);
                        }

                        //BeaconHelper.GetInstance().EventPhotoReport("0", Time.realtimeSinceStartup - startTime, "SUCC");
                    }
                }                
            }
            else
            {
                //BeaconHelper.GetInstance().EventPhotoReport("1", Time.realtimeSinceStartup - startTime, (www != null) ? www.error : string.Empty);
            }
        }
    };

    //--------------------------------------------------
    /// 缓存纹理管理器
    //--------------------------------------------------
    public class CCachedTextureManager
    {
        //相关常量
        private const int c_cachedTextureMaxAmount = 100;
        private static string s_cachedTextureDirectory = CFileManager.CombinePath(CFileManager.GetCachePath(), "HttpImage");
        private static string s_cachedTextureInfoSetFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, "httpimage.bytes");
        private static byte[] s_buffer = new byte[c_cachedTextureMaxAmount * 100];

        //缓存纹理信息集
        private CCachedTextureInfoSet m_cachedTextureInfoSet;

        //--------------------------------------------------
        /// 构造函数
        //--------------------------------------------------
        public CCachedTextureManager()
        {
            m_cachedTextureInfoSet = new CCachedTextureInfoSet();

            if (!CFileManager.IsDirectoryExist(s_cachedTextureDirectory))
            {
                CFileManager.CreateDirectory(s_cachedTextureDirectory);
            }
            
            if (CFileManager.IsFileExist(s_cachedTextureInfoSetFileFullPath))
            {
                byte[] data = CFileManager.ReadFile(s_cachedTextureInfoSetFileFullPath);
                int offset = 0;

                m_cachedTextureInfoSet.Read(data, ref offset);
            }
        }

        //--------------------------------------------------
        /// 获取缓存纹理
        /// @url        : 地址
        /// @validDays  : 有效天数
        //--------------------------------------------------
        public Texture2D GetCachedTexture(string url, float validDays)
        {
            string key = CFileManager.GetMd5(url.ToLower());

            CCachedTextureInfo cachedTextureInfo = m_cachedTextureInfoSet.GetCachedTextureInfo(key);

            //检查是否过期
            if (cachedTextureInfo == null || (DateTime.Now - cachedTextureInfo.m_lastModifyTime).TotalDays >= validDays)
            {
                return null;
            }

            //检查文件是否存在
            string cachedTextureFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, key + ".bytes");

            if (!CFileManager.IsFileExist(cachedTextureFileFullPath))
            {
                return null;
            }

            //读取Texture文件
            byte[] data = CFileManager.ReadFile(cachedTextureFileFullPath);

            if (data == null || data.Length <= 0)
            {
                return null;
            }
            else
            {
                Texture2D texture2D = null;

                if (cachedTextureInfo.m_isGif)
                {
                    using (MemoryStream memoryStream = new MemoryStream(data))
                    {
                        texture2D = GifHelper.GifToTexture(memoryStream, 0);
                    }
                }
                else
                {
                    texture2D = new Texture2D(cachedTextureInfo.m_width, cachedTextureInfo.m_height, TextureFormat.ARGB32, false);
                    texture2D.LoadImage(data);
                }

                return texture2D;
            }
        }

        //--------------------------------------------------
        /// 添加缓存纹理
        /// @url
        /// @width
        /// @height
        /// @isGif
        /// @data
        //--------------------------------------------------
        public void AddCachedTexture(string url, int width, int height, bool isGif, byte[] data)
        {
            string key = CFileManager.GetMd5(url.ToLower());

            if (m_cachedTextureInfoSet.m_cachedTextureInfoMap.ContainsKey(key))
            {
                CCachedTextureInfo cachedTextureInfo = null;
                m_cachedTextureInfoSet.m_cachedTextureInfoMap.TryGetValue(key, out cachedTextureInfo);

                DebugHelper.Assert(m_cachedTextureInfoSet.m_cachedTextureInfos.Contains(cachedTextureInfo), "zen me ke neng?");

                //修改信息
                cachedTextureInfo.m_width = width;
                cachedTextureInfo.m_height = height;
                cachedTextureInfo.m_lastModifyTime = DateTime.Now;
                cachedTextureInfo.m_isGif = isGif;
            }
            else
            {
                //如果数量达到上限，移除排在最前面的纹理
                if (m_cachedTextureInfoSet.m_cachedTextureInfos.Count >= c_cachedTextureMaxAmount)
                {
                    string removeKey = m_cachedTextureInfoSet.RemoveEarliestTextureInfo();

                    //删除缓存文件
                    if (!string.IsNullOrEmpty(removeKey))
                    {
                        string removeCachedTextureFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, removeKey + ".bytes");
                        if (CFileManager.IsFileExist(removeCachedTextureFileFullPath))
                        {
                            CFileManager.DeleteFile(removeCachedTextureFileFullPath);
                        }
                    }
                }

                CCachedTextureInfo cachedTextureInfo = new CCachedTextureInfo();
                cachedTextureInfo.m_key = key;
                cachedTextureInfo.m_width = width;
                cachedTextureInfo.m_height = height;
                cachedTextureInfo.m_lastModifyTime = DateTime.Now;
                cachedTextureInfo.m_isGif = isGif;

                m_cachedTextureInfoSet.AddTextureInfo(key, cachedTextureInfo);
            }

            //排序
            m_cachedTextureInfoSet.SortTextureInfo();

            //写入信息文件
            int offset = 0;
            m_cachedTextureInfoSet.Write(s_buffer, ref offset);

            if (CFileManager.IsFileExist(s_cachedTextureInfoSetFileFullPath))
            {
                CFileManager.DeleteFile(s_cachedTextureInfoSetFileFullPath);
            }

            CFileManager.WriteFile(s_cachedTextureInfoSetFileFullPath, s_buffer, 0, offset);

            //写入Texture数据文件
            string cachedTextureFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, key + ".bytes");
            if (CFileManager.IsFileExist(cachedTextureFileFullPath))
            {
                CFileManager.DeleteFile(cachedTextureFileFullPath);
            }

            CFileManager.WriteFile(cachedTextureFileFullPath, data);
        }
    }

    //--------------------------------------------------
    /// 缓存纹理信息集
    /// @二进制数据结构
    /// @Length     (4 byte)
    /// @Version    (2 byte)
    /// @Amount     (2 byte)
    /// @{
    /// @   info.key       (string)
    /// @   info.width     (2 byte)
    /// @   info.height    (2 byte)
    /// @   info.dateTime  (byte[])
    /// @   info.isGif      (1 byte)
    /// @}
    //--------------------------------------------------
    public class CCachedTextureInfoSet
    {
        //版本号，当CCachedTextureInfoSet数据结构发生变化时候一定要修改版本号，版本号不兼容的话不能从已存储的二进制文件读取数据
        public const int c_version = 10003;

        public ListView<CCachedTextureInfo> m_cachedTextureInfos = new ListView<CCachedTextureInfo>();
        public DictionaryView<string, CCachedTextureInfo> m_cachedTextureInfoMap = new DictionaryView<string, CCachedTextureInfo>();

        //--------------------------------------------------
        /// 写入数据
        /// @data
        /// @offset
        //--------------------------------------------------
        public void Write(byte[] data, ref int offset)
        {
            int startOffset = offset;

            //跳过文件长度
            offset += 4;

            //写入版本号
            MemoryManager.WriteShort((short)c_version, data, ref offset);

            //写入数量
            MemoryManager.WriteShort((short)m_cachedTextureInfos.Count, data, ref offset);

            //写入数据
            for (int i = 0; i < m_cachedTextureInfos.Count; i++)
            {
                MemoryManager.WriteString(m_cachedTextureInfos[i].m_key, data, ref offset);
                MemoryManager.WriteShort((short)m_cachedTextureInfos[i].m_width, data, ref offset);
                MemoryManager.WriteShort((short)m_cachedTextureInfos[i].m_height, data, ref offset);
                MemoryManager.WriteDateTime(ref m_cachedTextureInfos[i].m_lastModifyTime, data, ref offset);
                MemoryManager.WriteByte((byte)(m_cachedTextureInfos[i].m_isGif ? 1 : 0), data, ref offset);
            }

            //写入文件长度
            MemoryManager.WriteInt(offset - startOffset, data, ref startOffset);
        }

        //--------------------------------------------------
        /// 读出数据
        /// @data
        /// @offset
        //--------------------------------------------------
        public void Read(byte[] data, ref int offset)
        {
            m_cachedTextureInfos.Clear();
            m_cachedTextureInfoMap.Clear();

            if (data == null)
            {
                return;
            }

            int dataLength = data.Length - offset;

            //数据长度不合法，不能读取数据
            if (dataLength < 6)
            {
                return;
            }

            //校验数据长度
            int storedDataLength = MemoryManager.ReadInt(data, ref offset);
            if (storedDataLength < 6 || storedDataLength > dataLength)
            {
                return;
            }

            //校验版本号
            int version = MemoryManager.ReadShort(data, ref offset);
            if (version != c_version)
            {
                return;
            }

            //读取数据
            int amount = MemoryManager.ReadShort(data, ref offset);

            for (int i = 0; i < amount; i++)
            {
                CCachedTextureInfo cachedTextureInfo = new CCachedTextureInfo();
                cachedTextureInfo.m_key = MemoryManager.ReadString(data, ref offset);
                cachedTextureInfo.m_width = MemoryManager.ReadShort(data, ref offset);
                cachedTextureInfo.m_height = MemoryManager.ReadShort(data, ref offset);
                cachedTextureInfo.m_lastModifyTime = MemoryManager.ReadDateTime(data, ref offset);
                cachedTextureInfo.m_isGif = (MemoryManager.ReadByte(data, ref offset) > 0);

                //防止key重复
                if (!m_cachedTextureInfoMap.ContainsKey(cachedTextureInfo.m_key))
                {
                    m_cachedTextureInfoMap.Add(cachedTextureInfo.m_key, cachedTextureInfo);
                    m_cachedTextureInfos.Add(cachedTextureInfo);
                }
            }

            //按最后修改时间排序
            m_cachedTextureInfos.Sort();
        }

        //--------------------------------------------------
        /// 返回缓存纹理信息
        /// @key
        //--------------------------------------------------
        public CCachedTextureInfo GetCachedTextureInfo(string key)
        {
            if (m_cachedTextureInfoMap.ContainsKey(key))
            {
                CCachedTextureInfo cachedTextureInfo = null;
                m_cachedTextureInfoMap.TryGetValue(key, out cachedTextureInfo);

                return cachedTextureInfo;
            }
            else
            {
                return null;
            }
        }

        //--------------------------------------------------
        /// 添加数据
        /// @url
        /// @data
        //--------------------------------------------------
        public void AddTextureInfo(string key, CCachedTextureInfo cachedTextureInfo)
        {
            if (m_cachedTextureInfoMap.ContainsKey(key))
            {
                return;
            }

            m_cachedTextureInfoMap.Add(key, cachedTextureInfo);
            m_cachedTextureInfos.Add(cachedTextureInfo);
        }

        //--------------------------------------------------
        /// 移除最早的数据
        /// @return key
        //--------------------------------------------------
        public string RemoveEarliestTextureInfo()
        {
            if (m_cachedTextureInfos.Count <= 0)
            {
                return null;
            }

            CCachedTextureInfo removeCachedTextureInfo = m_cachedTextureInfos[0];

            m_cachedTextureInfos.RemoveAt(0);
            m_cachedTextureInfoMap.Remove(removeCachedTextureInfo.m_key);

            return removeCachedTextureInfo.m_key;
        }

        //--------------------------------------------------
        /// 排序
        //--------------------------------------------------
        public void SortTextureInfo()
        {
            m_cachedTextureInfos.Sort();
        }
    };

    //--------------------------------------------------
    /// 缓存纹理信息
    //--------------------------------------------------
    public class CCachedTextureInfo : IComparable
    {
        public string m_key;
        public int m_width;
        public int m_height;
        public DateTime m_lastModifyTime;
        public bool m_isGif;

        //--------------------------------------
        /// 排序函数
        /// @按m_lastModifyTime升序排列
        //--------------------------------------
        public int CompareTo(object obj)
        {
            CCachedTextureInfo cachedTextureInfo = obj as CCachedTextureInfo;

            if (m_lastModifyTime > cachedTextureInfo.m_lastModifyTime)
            {
                return 1;
            }
            else if (m_lastModifyTime == cachedTextureInfo.m_lastModifyTime)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    };
};