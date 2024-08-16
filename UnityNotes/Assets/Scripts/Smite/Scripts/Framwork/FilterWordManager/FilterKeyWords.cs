using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public static class FilterKeyWords
    {
        /// <summary>
        /// 敏感词树
        /// </summary>
        private class FilterKeyWordsNode
        {
            public Dictionary<Char, FilterKeyWordsNode> Child;
            public bool IsEnd;
        }

        static readonly object s_LockObj = new object();
        static FilterKeyWordsNode s_Root;
        const string TraditionalChinese = "";       //繁体字库
        const string SimplifiedChinese = "";        //简体字库
        static readonly Dictionary<char, char> TranslationChinese = TraditionalChinese.Select((c, i) => new { c, i }).ToDictionary(p => p.c, p => SimplifiedChinese[p.i]);

        /// <summary>
        /// 初始化，使用前必须调用一次
        /// </summary>
        /// <param name="keyWords">敏感词列表</param>
        public static void Init(string[] keyWords)
        {
            if (s_Root != null) return;

            lock (s_LockObj)
            {
                s_Root = new FilterKeyWordsNode();
                var list = keyWords.Select(p => new string(Translation(p))).Distinct().OrderBy(p => p).ThenBy(p => p.Length).ToList();
                for (int idx = list.Min(p => p.Length); idx <= list.Max(p => p.Length); idx++)
                {
                    int i1 = idx;
                    var startList = list.Where(p => p.Length >= i1).Select(p => p.Substring(0, i1)).Distinct();
                    foreach (var startWord in startList)
                    {
                        var tmp = s_Root;
                        for (int jdx = 0; jdx < startWord.Length; jdx++)
                        {
                            char ch = startWord[jdx];
                            if (tmp.Child == null)
                                tmp.Child = new Dictionary<char, FilterKeyWordsNode>();

                            if (!tmp.Child.ContainsKey(ch))
                            {
                                tmp.Child.Add(ch, new FilterKeyWordsNode { IsEnd = list.Contains(startWord.Substring(0, 1 + jdx)) });
                            }

                            tmp = tmp.Child[ch];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查找含有的敏感词
        /// </summary>
        /// <param name="content">输入的文本内容</param>
        /// <param name="keyWords">文本中包含的敏感词</param>
        /// <returns>是否存在敏感词</returns>
        public static bool Find(string content, out string[] keyWords)
        {
            keyWords = Find(content).Select(p => content.Substring(p.Key, p.Value)).Distinct().ToArray();
            return keyWords.Length > 0;
        }
        /// <summary>
        /// 查找是否含有的敏感词
        /// </summary>
        /// <param name="content">输入的文本内容</param>
        /// <param name="keyWords">文本中包含的敏感词</param>
        /// <returns>是否存在敏感词</returns>
        public static bool Contain(string content)
        {
            for (int idx = 0; idx <= content.Length; idx++)
            {
                string subStr = content.Substring(0, idx);

                string[] keyWords = Find(subStr).Select(p => content.Substring(p.Key, p.Value)).Distinct().ToArray();

                if (keyWords.Length > 0) return true;
            }

            return false;
        }

        /// <summary>
        /// 替换敏感词
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ReplaceKeywords(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            Dictionary<int, int> replacePosDict = Find(content);
            if (replacePosDict == null || replacePosDict.Count == 0)
                return content;

            char[] list = content.ToArray();
            foreach (var pair in replacePosDict)
            {
                int start = pair.Key;
                int count = pair.Value;
                for (int jdx = start; jdx < start + count; jdx++)
                {
                    list[jdx] = '*';
                }
            }
            return new string(list);
        }

        /// <summary>
        /// 字符串预处理
        /// </summary>
        /// <param name="content">字符串内容</param>
        /// <returns></returns>
        static char[] Translation(string content)
        {
            char[] c = content.ToCharArray();
            for (int idx = 0; idx < c.Length; idx++)
            {
                /*全角=>半角*/
                if (c[idx] > 0xFF00 && c[idx] < 0xFF5F)
                    c[idx] = (char)(c[idx] - 0xFEE0);



                /*大写=>小写*/
                if (c[idx] > 0x40 && c[idx] < 0x5b)
                    c[idx] = (char)(c[idx] + 0x20);



                /*繁体=>简体*/
                if (c[idx] > 0x4E00 && c[idx] < 0x9FFF)
                {
                    char chinese;
                    if (TranslationChinese.TryGetValue(c[idx], out chinese))
                        c[idx] = chinese;

                }
            }
            return c;
        }

        /// <summary>
        /// 跳过特殊字符， ASCII范围 排除 数字字母
        /// </summary>
        /// <param name="firstChar"></param>
        /// <returns></returns>
        static bool IsSkip(char firstChar)
        {
            //if (firstChar < '0')
            //    return true;

            //if (firstChar > '9' && firstChar < 'A')
            //    return true;

            //if (firstChar > 'Z' && firstChar < 'a')
            //    return true;

            //if (firstChar > 'Z' && firstChar < 128)
            //    return true;

            return false;
        }

        /// <summary>
        /// 位置查找
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        static Dictionary<int, int> Find(string content)
        {
            if (s_Root == null)
                throw new InvalidOperationException("未初始化");

            Dictionary<int, int> findRet = new Dictionary<int, int>();
            if (string.IsNullOrEmpty(content))
                return findRet;

            char[] text = Translation(content);
            int start = 0;
            while (start < text.Length)
            {
                int length = 0;
                char firstChar = text[start + length];
                var node = s_Root;

                //跳过特殊符号
                while (IsSkip(firstChar) && (start + length + 1) < text.Length)
                {
                    start++;
                    firstChar = text[start + length];
                }


                //不匹配首字符 移动起始位置
                while (!node.Child.ContainsKey(firstChar) && start < text.Length - 1)
                {
                    start++;
                    firstChar = text[start + length];
                }

                //部分匹配 移动结束位置 直到不匹配
                while (node.Child != null && node.Child.ContainsKey(firstChar))
                {
                    node = node.Child[firstChar];
                    length++;

                    if ((start + length) == text.Length)
                        break;

                    firstChar = text[start + length];

                    //跳过忽略词
                    while (IsSkip(firstChar) && !node.IsEnd && (start + length + 1) < text.Length)
                    {
                        length++;
                        firstChar = text[start + length];
                    }
                }

                //完整匹配 把起始位置移到结束位置
                if (node.IsEnd)
                {
                    findRet.Add(start, length);
                    start += length - 1;
                }

                start++;

            }

            return findRet;
        }
    }
}