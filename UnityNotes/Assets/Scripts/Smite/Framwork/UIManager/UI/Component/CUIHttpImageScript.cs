//==================================================================================
/// UI HttpImage �ؼ�
/// @���ݴ����url��ȡ��Ӧ��ͼƬ��Դ����ʾ
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
        //ͼƬ��ַ
        public string m_imageUrl;

        //�Ƿ�����Ϊԭʼ��С
        public bool m_setNativeSize;
        
        //�Ƿ񻺴�����
        public bool m_cacheTexture = true;

        //����������Ч��(��λ:����)
        public float m_cachedTextureValidDays = 2f;

        //�Ƿ�ǿ������ͼƬ��ַ����Ϊfalse��SetImageUrlʱ����ǰurl���ϴ�urlһ���򲻻�����url��
        public bool m_forceSetImageUrl = false;

        //Loadingʱ��ʾ��GameObject
        public GameObject m_loadingCover;

        //UGUI Image
        private Image m_image;
        private ImageAlphaTexLayout m_imageDefaultAlphaTexLayout = ImageAlphaTexLayout.None;  //Image2����Ҫ�õ�
        private Sprite m_imageDefaultSprite;

        //����״̬
        private enHttpImageState m_httpImageState = enHttpImageState.Unload;

        //�������
        private static CCachedTextureManager s_cachedTextureManager;

        //--------------------------------------------------
        /// ��ʼ��
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            //��ʼ��Image
            m_image = this.gameObject.GetComponent<Image>();
            m_imageDefaultSprite = m_image.sprite;

            //��imageΪImage2�����ﻹ��Ҫ���浱ǰalphaTexLayout
            if (m_image is Image2)
            {
                m_imageDefaultAlphaTexLayout = (m_image as Image2).alphaTexLayout;
            }

            //�����������������
            if (m_cacheTexture && s_cachedTextureManager == null)
            {
                s_cachedTextureManager = new CCachedTextureManager();
            }

            m_httpImageState = enHttpImageState.Unload;

            //��ʾLoadingCover
            if (m_loadingCover != null)
            {
                m_loadingCover.CustomSetActive(true);
            }

            //��ȡ��Դ
            if (this.gameObject.activeInHierarchy && !string.IsNullOrEmpty(m_imageUrl))
            {
                LoadTexture(m_imageUrl);
            }
        }

        //--------------------------------------------------
        /// ����
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

                //��ʾLoadingCover
                if (m_loadingCover != null)
                {
                    m_loadingCover.CustomSetActive(true);
                }
            }
        }

        //--------------------------------------------------
        /// Set Image ����
        /// @url
        //--------------------------------------------------
        public void SetImageUrl(string url)
        {
            if (!m_forceSetImageUrl && string.Equals(url, m_imageUrl))
            {
                return;
            }

            m_imageUrl = url;

            //ˢ��Ĭ��sprite
            if (m_image != null)
            {
                m_image.SetSprite(m_imageDefaultSprite, m_imageDefaultAlphaTexLayout);
            }

            if (this.gameObject.activeInHierarchy && m_httpImageState == enHttpImageState.Loading)
            {
                StopAllCoroutines();
            }

            m_httpImageState = enHttpImageState.Unload;

            //��ʾLoadingCover
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
        /// ֱ����Sprite
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
        /// ��������
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

                        //����LoadingCover
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
        /// ����ͼƬ
        /// @url
        //--------------------------------------------------
        private IEnumerator DownloadImage(string url)
        {
            m_httpImageState = enHttpImageState.Loading;

         //   float startTime = Time.realtimeSinceStartup;

            WWW www = new WWW(url);
            yield return www;

            //�������ص���������ȷ���״̬����Ҫ��ΪLoaded
            m_httpImageState = enHttpImageState.Loaded;

            if (string.IsNullOrEmpty(www.error))
            {
                //����LoadingCover
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

                //���contentType
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

                        //��������
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
    /// �������������
    //--------------------------------------------------
    public class CCachedTextureManager
    {
        //��س���
        private const int c_cachedTextureMaxAmount = 100;
        private static string s_cachedTextureDirectory = CFileManager.CombinePath(CFileManager.GetCachePath(), "HttpImage");
        private static string s_cachedTextureInfoSetFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, "httpimage.bytes");
        private static byte[] s_buffer = new byte[c_cachedTextureMaxAmount * 100];

        //����������Ϣ��
        private CCachedTextureInfoSet m_cachedTextureInfoSet;

        //--------------------------------------------------
        /// ���캯��
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
        /// ��ȡ��������
        /// @url        : ��ַ
        /// @validDays  : ��Ч����
        //--------------------------------------------------
        public Texture2D GetCachedTexture(string url, float validDays)
        {
            string key = CFileManager.GetMd5(url.ToLower());

            CCachedTextureInfo cachedTextureInfo = m_cachedTextureInfoSet.GetCachedTextureInfo(key);

            //����Ƿ����
            if (cachedTextureInfo == null || (DateTime.Now - cachedTextureInfo.m_lastModifyTime).TotalDays >= validDays)
            {
                return null;
            }

            //����ļ��Ƿ����
            string cachedTextureFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, key + ".bytes");

            if (!CFileManager.IsFileExist(cachedTextureFileFullPath))
            {
                return null;
            }

            //��ȡTexture�ļ�
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
        /// ��ӻ�������
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

                //�޸���Ϣ
                cachedTextureInfo.m_width = width;
                cachedTextureInfo.m_height = height;
                cachedTextureInfo.m_lastModifyTime = DateTime.Now;
                cachedTextureInfo.m_isGif = isGif;
            }
            else
            {
                //��������ﵽ���ޣ��Ƴ�������ǰ�������
                if (m_cachedTextureInfoSet.m_cachedTextureInfos.Count >= c_cachedTextureMaxAmount)
                {
                    string removeKey = m_cachedTextureInfoSet.RemoveEarliestTextureInfo();

                    //ɾ�������ļ�
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

            //����
            m_cachedTextureInfoSet.SortTextureInfo();

            //д����Ϣ�ļ�
            int offset = 0;
            m_cachedTextureInfoSet.Write(s_buffer, ref offset);

            if (CFileManager.IsFileExist(s_cachedTextureInfoSetFileFullPath))
            {
                CFileManager.DeleteFile(s_cachedTextureInfoSetFileFullPath);
            }

            CFileManager.WriteFile(s_cachedTextureInfoSetFileFullPath, s_buffer, 0, offset);

            //д��Texture�����ļ�
            string cachedTextureFileFullPath = CFileManager.CombinePath(s_cachedTextureDirectory, key + ".bytes");
            if (CFileManager.IsFileExist(cachedTextureFileFullPath))
            {
                CFileManager.DeleteFile(cachedTextureFileFullPath);
            }

            CFileManager.WriteFile(cachedTextureFileFullPath, data);
        }
    }

    //--------------------------------------------------
    /// ����������Ϣ��
    /// @���������ݽṹ
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
        //�汾�ţ���CCachedTextureInfoSet���ݽṹ�����仯ʱ��һ��Ҫ�޸İ汾�ţ��汾�Ų����ݵĻ����ܴ��Ѵ洢�Ķ������ļ���ȡ����
        public const int c_version = 10003;

        public ListView<CCachedTextureInfo> m_cachedTextureInfos = new ListView<CCachedTextureInfo>();
        public DictionaryView<string, CCachedTextureInfo> m_cachedTextureInfoMap = new DictionaryView<string, CCachedTextureInfo>();

        //--------------------------------------------------
        /// д������
        /// @data
        /// @offset
        //--------------------------------------------------
        public void Write(byte[] data, ref int offset)
        {
            int startOffset = offset;

            //�����ļ�����
            offset += 4;

            //д��汾��
            MemoryManager.WriteShort((short)c_version, data, ref offset);

            //д������
            MemoryManager.WriteShort((short)m_cachedTextureInfos.Count, data, ref offset);

            //д������
            for (int i = 0; i < m_cachedTextureInfos.Count; i++)
            {
                MemoryManager.WriteString(m_cachedTextureInfos[i].m_key, data, ref offset);
                MemoryManager.WriteShort((short)m_cachedTextureInfos[i].m_width, data, ref offset);
                MemoryManager.WriteShort((short)m_cachedTextureInfos[i].m_height, data, ref offset);
                MemoryManager.WriteDateTime(ref m_cachedTextureInfos[i].m_lastModifyTime, data, ref offset);
                MemoryManager.WriteByte((byte)(m_cachedTextureInfos[i].m_isGif ? 1 : 0), data, ref offset);
            }

            //д���ļ�����
            MemoryManager.WriteInt(offset - startOffset, data, ref startOffset);
        }

        //--------------------------------------------------
        /// ��������
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

            //���ݳ��Ȳ��Ϸ������ܶ�ȡ����
            if (dataLength < 6)
            {
                return;
            }

            //У�����ݳ���
            int storedDataLength = MemoryManager.ReadInt(data, ref offset);
            if (storedDataLength < 6 || storedDataLength > dataLength)
            {
                return;
            }

            //У��汾��
            int version = MemoryManager.ReadShort(data, ref offset);
            if (version != c_version)
            {
                return;
            }

            //��ȡ����
            int amount = MemoryManager.ReadShort(data, ref offset);

            for (int i = 0; i < amount; i++)
            {
                CCachedTextureInfo cachedTextureInfo = new CCachedTextureInfo();
                cachedTextureInfo.m_key = MemoryManager.ReadString(data, ref offset);
                cachedTextureInfo.m_width = MemoryManager.ReadShort(data, ref offset);
                cachedTextureInfo.m_height = MemoryManager.ReadShort(data, ref offset);
                cachedTextureInfo.m_lastModifyTime = MemoryManager.ReadDateTime(data, ref offset);
                cachedTextureInfo.m_isGif = (MemoryManager.ReadByte(data, ref offset) > 0);

                //��ֹkey�ظ�
                if (!m_cachedTextureInfoMap.ContainsKey(cachedTextureInfo.m_key))
                {
                    m_cachedTextureInfoMap.Add(cachedTextureInfo.m_key, cachedTextureInfo);
                    m_cachedTextureInfos.Add(cachedTextureInfo);
                }
            }

            //������޸�ʱ������
            m_cachedTextureInfos.Sort();
        }

        //--------------------------------------------------
        /// ���ػ���������Ϣ
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
        /// �������
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
        /// �Ƴ����������
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
        /// ����
        //--------------------------------------------------
        public void SortTextureInfo()
        {
            m_cachedTextureInfos.Sort();
        }
    };

    //--------------------------------------------------
    /// ����������Ϣ
    //--------------------------------------------------
    public class CCachedTextureInfo : IComparable
    {
        public string m_key;
        public int m_width;
        public int m_height;
        public DateTime m_lastModifyTime;
        public bool m_isGif;

        //--------------------------------------
        /// ������
        /// @��m_lastModifyTime��������
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