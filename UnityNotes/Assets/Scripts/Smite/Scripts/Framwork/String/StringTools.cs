using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework
{
    public static class StringTools
    {
        public static StringBuilder _stringBuilder = new StringBuilder(1024);

        public static void ClearSBuider()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
        }

        public static string Bytes2String(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        }

        public static string ASCIIByte2String(byte[] bytes)
        {
            if (bytes == null) return string.Empty;
            try
            {
                return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(string.Format("ASCIIByte2String is error:{0}", ex.Message));
                return string.Empty;
            }
        }

        public static byte[] String2UTF8Bytes(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                DebugHelper.LogError(string.Format("String2UTF8Bytes is error."));
                return null;
            }
            return Encoding.UTF8.GetBytes(data);
        }

        //--------------------------------------
        /// 是否为合法的字符串
        //--------------------------------------
        public static bool IsAvailableString(string str)
        {
            int ret = 0;
            int in_pos = 0;
            char ch = (char)0;
            bool surrogate = false;

            int len = str.Length;
            while (in_pos < len)
            {
                ch = str[in_pos];

                if (surrogate)
                {
                    if (ch >= 0xDC00 && ch <= 0xDFFF)
                    {
                        ret += 4;
                    }
                    else
                    {
                        /* invalid surrogate pair */
                        Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate tail)", in_pos));
                        return false;
                    }
                    surrogate = false;
                }
                else
                {
                    /* fast path optimization */
                    if (ch < 0x80)
                    {
                        for (; in_pos < len; in_pos++)
                        {
                            if (str[in_pos] < 0x80)
                            {
                                ++ret;
                            }
                            else
                            {
                                break;
                            }
                        }
                        continue;
                    }
                    else if (ch < 0x0800)
                    {
                        ret += 2;
                    }
                    else if (ch >= 0xD800 && ch <= 0xDBFF)
                    {
                        surrogate = true;
                    }
                    else if (ch >= 0xDC00 && ch <= 0xDFFF)
                    {
                        /* invalid surrogate pair */
                        DebugHelper.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate head)", in_pos));
                        return false;
                    }
                    else
                    {
                        ret += 3;
                    }
                }
                in_pos++;
            }
            return true;
        }

        /// <summary>
        /// 得到字符串中的中文字符的个数（主要用来位置居中）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetHanNumFromString(string str)
        {
            int count = 0;
            Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");

            for (int i = 0; i < str.Length; i++)
            {
                if (regex.IsMatch(str[i].ToString()))
                {
                    count++;
                }
            }
            return count;
        }


        #region StringExtension
        public static readonly string asset_str = "Assets/";
        public static string RemoveExtension(this string s)
        {
            if (s == null)
                return null;

            int index = s.LastIndexOf('.');
            if (index == -1)
                return s;

            return s.Substring(0, index);
        }       

        public static string FullPathToAssetPath(this string s)
        {
            if (s == null)
                return null;

            string path = asset_str + s.Substring(Application.dataPath.Length + 1);
            return path.Replace('\\', '/');
        }

        public static string AssetPathToFullPath(this string s)
        {
            if (s == null)
                return null;

            if (!s.StartsWith(asset_str))
                return null;

            string path = Application.dataPath;
            path += "/";
            path += s.Remove(0, asset_str.Length);
            return path;
        }

        public static string GetFileExtension(this string s)
        {
            int num = s.LastIndexOf('.');
            if (num == -1)
                return null;
            return s.Substring(num + 1);
        }

        public static string GetFileExtensionUpper(this string s)
        {
            string ext = s.GetFileExtension();
            if (ext == null)
                return null;
            return ext.ToUpper();
        }

        public static string GetHierarchyName(this GameObject go)
        {
            if (go == null)
                return "<null>";

            string name = "";
            while (go != null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = go.name;
                }
                else
                {
                    name = go.name + "." + name;
                }
                var tr = go.transform.parent;
                go = tr != null ? tr.gameObject : null;
            }

            return name;
        }

        //这是Java的HashCode代码
        public static int JavaHashCode(this string s)
        {
            int h = 0;
            int len = s.Length;
            if (len > 0)
            {
                int off = 0;

                for (int i = 0; i < len; i++)
                {
                    char c = s[off++];
                    h = 31 * h + c;
                }
            }
            return h;
        }

        //这是Java的HashCode代码，并且ignoreCase
        public static int JavaHashCodeIgnoreCase(this string s)
        {
            int h = 0;
            int len = s.Length;
            if (len > 0)
            {
                int off = 0;

                for (int i = 0; i < len; i++)
                {
                    char c = s[off++];
                    if (c >= 'A' && c <= 'Z')
                    {
                        c += (char)('a' - 'A');
                    }
                    h = 31 * h + c;
                }
            }
            return h;
        }
        #endregion
    }
}