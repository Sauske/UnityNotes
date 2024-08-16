using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class SensitiveWordHelper
    {
        protected string path = "SensitiveWord/SensitiveWord.txt";
        private List<string> lsgSensitiveWords = new List<string>();
        public SensitiveWordHelper()
        {
            byte[] buff = new byte[1024];//CResourceSys.Instance.LoadBinary(EResType.EText, path);
            string strSensitiveWord = System.Text.Encoding.UTF8.GetString(buff);
            strSensitiveWord = strSensitiveWord.Replace("\r\n", "\r");
            string[] sensitiveWordArr = strSensitiveWord.Split('\r');
            lsgSensitiveWords.AddRange(sensitiveWordArr);
            //Debug.LogError(sensitiveWordArr.Length);
        }

        protected static SensitiveWordHelper m_sensitiveWordHelper;
        public static SensitiveWordHelper Instance
        {
            get
            {
                if (m_sensitiveWordHelper == null)
                {
                    m_sensitiveWordHelper = new SensitiveWordHelper();
                }

                return m_sensitiveWordHelper;
            }
        }

        public bool HavSensitiveWords(string strText)
        {
            for (int i = 0; i < lsgSensitiveWords.Count; i++)
            {
                if (strText.IndexOf(lsgSensitiveWords[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
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

        public string ProssSensitiveWords(string strText)
        {
            for (int i = 0; i < lsgSensitiveWords.Count; i++)
            {
                if (strText.IndexOf(lsgSensitiveWords[i]) > 0)
                {
                    strText = strText.Replace(lsgSensitiveWords[i], GetStar(lsgSensitiveWords[i].Length));
                }
            }

            return strText;
        }
    }
}