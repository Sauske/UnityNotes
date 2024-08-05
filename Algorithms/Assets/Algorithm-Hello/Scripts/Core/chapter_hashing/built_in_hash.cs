/**
* File: built_in_hash.cs
* Created Time: 2023-06-26
* Author: hpstory (hpstory1024@163.com)
*/
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_hashing
{

    public class built_in_hash
    {
        [TestAttibute]
        public void Test()
        {
            int num = 3;
            int hashNum = num.GetHashCode();
            Debug.Log("整数 " + num + " 的哈希值为 " + hashNum);

            bool bol = true;
            int hashBol = bol.GetHashCode();
            Debug.Log("布尔量 " + bol + " 的哈希值为 " + hashBol);

            double dec = 3.14159;
            int hashDec = dec.GetHashCode();
            Debug.Log("小数 " + dec + " 的哈希值为 " + hashDec);

            string str = "Hello 算法";
            int hashStr = str.GetHashCode();
            Debug.Log("字符串 " + str + " 的哈希值为 " + hashStr);

            object[] arr = { 12836, "小哈" };
            int hashTup = arr.GetHashCode();
            Debug.Log("数组 [" + string.Join(", ", arr) + "] 的哈希值为 " + hashTup);

            ListNode obj = new(0);
            int hashObj = obj.GetHashCode();
            Debug.Log("节点对象 " + obj + " 的哈希值为 " + hashObj);
        }
    }
}
