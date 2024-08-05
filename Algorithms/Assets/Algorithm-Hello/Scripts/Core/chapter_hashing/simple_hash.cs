/**
* File: simple_hash.cs
* Created Time: 2023-06-26
* Author: hpstory (hpstory1024@163.com)
*/
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_hashing
{

    public class simple_hash
    {
        /* 加法哈希 */
        int AddHash(string key)
        {
            long hash = 0;
            const int MODULUS = 1000000007;
            foreach (char c in key)
            {
                hash = (hash + c) % MODULUS;
            }
            return (int)hash;
        }

        /* 乘法哈希 */
        int MulHash(string key)
        {
            long hash = 0;
            const int MODULUS = 1000000007;
            foreach (char c in key)
            {
                hash = (31 * hash + c) % MODULUS;
            }
            return (int)hash;
        }

        /* 异或哈希 */
        int XorHash(string key)
        {
            int hash = 0;
            const int MODULUS = 1000000007;
            foreach (char c in key)
            {
                hash ^= c;
            }
            return hash & MODULUS;
        }

        /* 旋转哈希 */
        int RotHash(string key)
        {
            long hash = 0;
            const int MODULUS = 1000000007;
            foreach (char c in key)
            {
                hash = ((hash << 4) ^ (hash >> 28) ^ c) % MODULUS;
            }
            return (int)hash;
        }

        [TestAttibute]
        public void Test()
        {
            string key = "Hello 算法";

            int hash = AddHash(key);
            Debug.Log("加法哈希值为 " + hash);

            hash = MulHash(key);
            Debug.Log("乘法哈希值为 " + hash);

            hash = XorHash(key);
            Debug.Log("异或哈希值为 " + hash);

            hash = RotHash(key);
            Debug.Log("旋转哈希值为 " + hash);
        }
    }
}
