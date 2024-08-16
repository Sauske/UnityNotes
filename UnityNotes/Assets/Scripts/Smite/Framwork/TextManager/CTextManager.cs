//====================================
/// 文本管理器
/// @neoyang
/// @2015.03.23
//====================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{


    public class CTextManager : Singleton<CTextManager>
    {
        //Text Map
        private Dictionary<int, string> _textMap;

        public override void Init()
        {
            _textMap = new Dictionary<int, string>();
        }

        //--------------------------------------------------
        /// 从Resources目录下读取Text
        /// @textBinPath
        //--------------------------------------------------
        public void LoadLocalText()
        {
            //DatabinTable<ResText, UInt16> textDatabin = new DatabinTable<ResText, UInt16>("Databin/Client/Text/Text.bytes", "wID");
            //LoadText(textDatabin);
            //textDatabin.Unload();
        }

        //--------------------------------------------------
        /// 读取Text
        /// @textDataBin
        //--------------------------------------------------
        //    public void LoadText(DatabinTable<ResText, UInt16> textDataBin)
        //    {
        //        //增加保护
        //        if (textDataBin == null)
        //        {
        //            return;
        //        }

        //        m_textMap = new Dictionary<int, string>();

        //        var emr = textDataBin.GetEnumerator();

        //        while (emr.MoveNext())
        //        {
        //            var Cfg = (ResText)emr.Current.Value;

        //#if UNITY_EDITOR
        //            if (m_textMap.ContainsKey(StringHelper.UTF8BytesToString(ref Cfg.szKey).JavaHashCode()))
        //            {
        //                Debug.LogError("Text Key : \"" + StringHelper.UTF8BytesToString(ref Cfg.szKey) + "\"的HashCode和其他文本冲突了，换一个名字呗^_^");
        //            }
        //            m_textMap.Add(StringHelper.UTF8BytesToString(ref Cfg.szKey).JavaHashCode(), StringHelper.UTF8BytesToString(ref Cfg.szValue));
        //#else
        //            m_textMap.Add(StringHelper.UTF8BytesToString(ref Cfg.szKey).JavaHashCode(), StringHelper.UTF8BytesToString(ref Cfg.szValue));
        //#endif
        //        }
        //    }

        //--------------------------------------------------
        /// Text是否加载完成
        //--------------------------------------------------
        public bool IsTextLoaded()
        {
            return (_textMap != null);
        }

        //--------------------------------------------------
        /// 获取Text
        /// @key
        //--------------------------------------------------
        public string GetText(string key)
        {
            string text = string.Empty;
            _textMap.TryGetValue(key.JavaHashCode(), out text);

            if (string.IsNullOrEmpty(text))
            {
                text = key;
            }
            return text;
        }

        //--------------------------------------------------
        /// 获取Combine Text
        /// @key1
        /// @key2
        //--------------------------------------------------
        public string GetCombineText(string key1, string key2)
        {
            return GetText(key1) + GetText(key2);
        }

        public string GetCombineText(string key1, string key2, string key3)
        {
            return GetText(key1) + GetText(key2) + GetText(key3);
        }

        public string GetCombineText(string key1, string key2, string key3, string key4)
        {
            return GetText(key1) + GetText(key2) + GetText(key3) + GetText(key4);
        }

        //获取Text,并替换{0}{1}{.}符号内容为replace
        public string GetText(string key, params string[] args)
        {
            string result = CTextManager.GetInstance().GetText(key);
            if (result == null) result = "text with tag [" + key + "] was not found!";
            else result = String.Format(result, args);

            return result;
        }

        // 不用重复定义这么多函数 
        //获取Text,并替换{0}符号内容为replace
        public string GetText(string tag, string replace1)
        {
            string result = CTextManager.GetInstance().GetText(tag);
            if (result == null) result = "text with tag [" + tag + "] was not found!";
            else result = String.Format(result, replace1);

            return result;
        }

        //获取Text,并替换{0}{1}符号内容为replace
        public string GetText(string tag, string replace1, string replace2)
        {
            string result = CTextManager.GetInstance().GetText(tag);
            if (result == null) result = "text with tag [" + tag + "] was not found!";
            else result = String.Format(result, replace1, replace2);

            return result;
        }

        //获取Text,并替换{0}{1}{2}符号内容为replace
        public string GetText(string tag, string replace1, string replace2, string replace3)
        {
            string result = CTextManager.GetInstance().GetText(tag);
            if (result == null) result = "text with tag [" + tag + "] was not found!";
            else result = String.Format(result, replace1, replace2, replace3);

            return result;
        }

        //获取Text,并替换{0}{1}{2}{3}符号内容为replace
        public string GetText(string tag, string replace1, string replace2, string replace3, string replace4)
        {
            string result = CTextManager.GetInstance().GetText(tag);
            if (result == null) result = "text with tag [" + tag + "] was not found!";
            else result = String.Format(result, replace1, replace2, replace3, replace4);

            return result;
        }

        //获取Text,并替换{0}{1}{2}{3}符号内容为replace
        public string GetText(string tag, string replace1, string replace2, string replace3, string replace4, string replace5)
        {
            string result = CTextManager.GetInstance().GetText(tag);
            if (result == null) result = "text with tag [" + tag + "] was not found!";
            else result = String.Format(result, replace1, replace2, replace3, replace4, replace5);

            return result;
        }
        
    }
}
