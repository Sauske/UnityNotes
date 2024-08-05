/**
* File: max_product_cutting.cs
* Created Time: 2023-07-21
* Author: hpstory (hpstory1024@163.com)
*/
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_greedy
{

    public class max_product_cutting
    {
        /* 最大切分乘积：贪心 */
        int MaxProductCutting(int n)
        {
            // 当 n <= 3 时，必须切分出一个 1
            if (n <= 3)
            {
                return 1 * (n - 1);
            }
            // 贪心地切分出 3 ，a 为 3 的个数，b 为余数
            int a = n / 3;
            int b = n % 3;
            if (b == 1)
            {
                // 当余数为 1 时，将一对 1 * 3 转化为 2 * 2
                return (int)System.Math.Pow(3, a - 1) * 2 * 2;
            }
            if (b == 2)
            {
                // 当余数为 2 时，不做处理
                return (int)System.Math.Pow(3, a) * 2;
            }
            // 当余数为 0 时，不做处理
            return (int)System.Math.Pow(3, a);
        }

        [TestAttibute]
        public void Test()
        {
            int n = 58;

            // 贪心算法
            int res = MaxProductCutting(n);
            Debug.Log("最大切分乘积为" + res);
        }
    }
}