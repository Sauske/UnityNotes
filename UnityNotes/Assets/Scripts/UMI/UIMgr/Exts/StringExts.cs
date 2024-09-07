using System.Text;
using System;
using UnityEngine;

namespace UMI
{
    /// <summary>
    /// string 的扩展 方便后面统一转码
    /// </summary>
    public static class StringExts
    {
        public static string StringLogTag = "StringLogTag";
        public static string EncodingType = "UTF8";
        public static Encoding UTF8Encoding = Encoding.UTF8;

        /// <summary>
        /// string 转 bytes
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return UTF8Encoding.GetBytes(str);
        }

        /// <summary>
        /// bytes 转str
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToString(this byte[] data, Encoding encoding)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            return encoding.GetString(data);
        }

        /// <summary>
        /// bytes 转str
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToString(this byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            return UTF8Encoding.GetString(data);
        }

        /// <summary>
        /// bytes 转 16进制 str
        /// </summary>
        /// <param name="data"></param>
		/// <param name="bSpaceSplit"> 是否用空格隔开</param>
        /// <returns>里面没有空格</returns>
        public static string BytesToHexString(this byte[] byteDatas, bool bSpaceSplit = false)
        {
            if (byteDatas == null || byteDatas.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", byteDatas[i]));
                if (bSpaceSplit)
                {
                    builder.Append(" ");
                }
            }
            return builder.ToString().Trim();
        }

        /// <summary>
        /// 用指定编码将给定的字符串转16进制格式字符串
        /// </summary>
        /// <param name="plainString">待转换的字符串</param>
        /// <param name="encode">编码规则</param>
		/// <param name="bSpaceSplit"> 是否有空格隔开</param>
        /// <returns></returns>
        public static string ToHexString(this string plainString, Encoding encode, bool bSpaceSplit = false)
        {
            byte[] byteDatas = encode.GetBytes(plainString);
            return byteDatas.BytesToHexString(bSpaceSplit);
        }

        /// <summary>
        /// 16进制格式字符串转字节数组
        /// </summary>
        /// <param name="hexStr"></param>
		/// <param name="bSpaceSplit"> 是否有空格隔开</param>
        /// <returns></returns>
        public static byte[] ToBytesFromHexString(this string hexStr, bool bSpaceSplit = false)
        {
            string[] chars;
            if (!bSpaceSplit)
            {

                if (hexStr.Length % 2 != 0)
                {
                    Debug.LogFormat("hexStr = {0} is not good HexString length = {1}", hexStr, hexStr.Length);
                    hexStr = "0" + hexStr;
                }

                chars = new string[hexStr.Length / 2];
                int i = 0;
                while (i < chars.Length)
                {
                    chars[i] = hexStr.Substring(i * 2, 2);
                }
            }
            else
            {
                //以 ' ' 分割字符串，并去掉空字符
                chars = hexStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            byte[] returnBytes = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(chars[i], 16);
            }
            return returnBytes;
        }
    }
}
