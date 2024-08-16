//==================================================================================
/// UI HttpText �ؼ�
/// @���ݴ����url��ȡ��Ӧ���ı�����ʾ
/// @neoyang
/// @2016.01.01
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

namespace Framework
{
    //state
    public enum enHttpTextState
    {
        Unload,
        Loading,
        Loaded
    };

    public class CUIHttpTextScript : CUIComponent
    {
        //�ı���ַ
        public string m_textUrl;

        //Loadingʱ��ʾ��GameObject
        public GameObject m_loadingCover;

        //������
        private ScrollRect m_scrollRectScript;
        private Text m_titleTextScript;
        private Text m_textScript;        

        //����״̬
        private enHttpTextState m_httpTextState = enHttpTextState.Unload;

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

            //��ʼ��ScrollRect && Text
            m_scrollRectScript = CUIUtility.GetComponentInChildren<ScrollRect>(this.gameObject);
            m_textScript = (m_scrollRectScript != null) ? CUIUtility.GetComponentInChildren<Text>(m_scrollRectScript.gameObject) : null;

            //��ʼ��Title
            Transform titleTransform = this.gameObject.transform.Find("Title");
            m_titleTextScript = (titleTransform != null) ? CUIUtility.GetComponentInChildren<Text>(titleTransform.gameObject) : null;

            m_httpTextState = enHttpTextState.Unload;

            //��ʾLoadingCover
            if (m_loadingCover != null)
            {
                m_loadingCover.CustomSetActive(true);
            }

            //��ȡ��Դ
            if (this.gameObject.activeInHierarchy && !string.IsNullOrEmpty(m_textUrl))
            {
                LoadText(m_textUrl);
            }
        }

        //--------------------------------------------------
        /// ����
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_loadingCover = null;
            m_scrollRectScript = null;
            m_titleTextScript = null;
            m_textScript = null;

            base.OnDestroy();
        }

        void OnEnable()
        {
            if (m_isInitialized && m_httpTextState == enHttpTextState.Unload && !string.IsNullOrEmpty(m_textUrl))
            {
                LoadText(m_textUrl);
            }
        }

        void OnDisable()
        {
            if (m_isInitialized && m_httpTextState == enHttpTextState.Loading)
            {
                StopAllCoroutines();
                m_httpTextState = enHttpTextState.Unload;

                //��ʾLoadingCover
                if (m_loadingCover != null)
                {
                    m_loadingCover.CustomSetActive(true);
                }
            }
        }

        //--------------------------------------------------
        /// Set Text ����
        /// @url
        /// @forceReset
        //--------------------------------------------------
        public void SetTextUrl(string url, bool forceReset = false)
        {
            if (string.IsNullOrEmpty(url) || (string.Equals(url, m_textUrl) && !forceReset))
            {
                return;
            }

            m_textUrl = url;

            //clear Title
            if (m_titleTextScript != null)
            {
                m_titleTextScript.text = string.Empty;
            }

            //clear �ı�
            if (m_textScript != null)
            {
                m_textScript.text = string.Empty;
            }

            if (this.gameObject.activeInHierarchy && m_httpTextState == enHttpTextState.Loading)
            {
                StopAllCoroutines();
            }

            m_httpTextState = enHttpTextState.Unload;

            //��ʾLoadingCover
            if (m_loadingCover != null)
            {
                m_loadingCover.CustomSetActive(true);
            }

            if (this.gameObject.activeInHierarchy)
            {
                LoadText(m_textUrl);
            }
        }

        //--------------------------------------------------
        /// �����ı�
        //--------------------------------------------------
        private void LoadText(string url)
        {
            if (m_httpTextState == enHttpTextState.Loaded)
            {
                return;
            }

            StartCoroutine(DownloadText(url));
        }

        //--------------------------------------------------
        /// �����ı�
        /// @url
        //--------------------------------------------------
        private IEnumerator DownloadText(string url)
        {
            m_httpTextState = enHttpTextState.Loading;

            WWW www = new WWW(url);
            yield return www;

            //�������ص���������ȷ���״̬����Ҫ��ΪLoaded
            m_httpTextState = enHttpTextState.Loaded;

            if (string.IsNullOrEmpty(www.error))
            {
                //����LoadingCover
                if (m_loadingCover != null)
                {
                    m_loadingCover.CustomSetActive(false);
                }

                string text = www.text;
                string title = string.Empty;
                string content = string.Empty;

                //����title��content
                bool hasTitle = false;

                int lineBreakPosition = text.IndexOf('\n');
                if (lineBreakPosition >= 0)
                {
                    string _title = text.Substring(0, lineBreakPosition).Trim();
                    if (_title != null && _title.Length >= 2)
                    {
                        if (_title[0] == '[' && _title[_title.Length - 1] == ']')
                        {
                            hasTitle = true;

                            title = _title.Substring(1, _title.Length - 2).Trim();
                            content = text.Substring(lineBreakPosition).Trim();
                        }
                    }
                }
                
                if (!hasTitle)
                {
                    title = string.Empty;
                    content = text;
                }

                if (m_titleTextScript != null)
                {
                    m_titleTextScript.text = title;
                }

                if (m_textScript != null)
                {
                    m_textScript.text = content;
                    m_textScript.rectTransform.anchoredPosition = new Vector2(m_textScript.rectTransform.anchoredPosition.x, 0);
                }
            }
        }
    };
};