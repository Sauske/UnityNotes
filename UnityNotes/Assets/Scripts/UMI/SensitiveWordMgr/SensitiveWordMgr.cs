using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UMI
{
    public class SensitiveWordMgr: Singleton<SensitiveWordMgr>
    {
        const string path = "SensitiveWord.txt";
        private List<string> words = new List<string>();

        protected override void OnInit()
        {
            LoadTxt();
        }


        public void LoadTxt()
        {
            TextAsset ta = Resources.Load<TextAsset>(path);
            if (ta != null)
            {
                string strSensitiveWord = System.Text.Encoding.UTF8.GetString(ta.bytes);
                strSensitiveWord = strSensitiveWord.Replace("\r\n", "\r");
                string[] sensitiveWordArr = strSensitiveWord.Split('\r');
                words.AddRange(sensitiveWordArr);
            }

            //ResMgr.Instance.LoadAssetResAsync<TextAsset>(path, (aData, aName) =>
            //{
            //    if (aData == null) return;

            //    TextAsset ta = aData as TextAsset;
            //    if (ta != null)
            //    {
            //        string strSensitiveWord = System.Text.Encoding.UTF8.GetString(ta.bytes);
            //        strSensitiveWord = strSensitiveWord.Replace("\r\n", "\r");
            //        string[] sensitiveWordArr = strSensitiveWord.Split('\r');
            //        words.AddRange(sensitiveWordArr);
            //    }
            //});          
        }

        /// <summary>
        /// 检查是否有敏感词
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public bool HavSensitiveWords(string strText)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (strText.IndexOf(words[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 处理敏感词
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public string ProssSensitiveWords(string strText)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (strText.IndexOf(words[i]) >= 0)
                {
                    strText = strText.Replace(words[i], GetStar(words[i].Length));
                }
            }

            return strText;
        }

        protected string GetStar(int nCount)
        {
            string sRet = "";
            for (int i = 0; i < nCount; i++)
            {
                sRet += "*";
            }
            return sRet;
        }
    }
}
