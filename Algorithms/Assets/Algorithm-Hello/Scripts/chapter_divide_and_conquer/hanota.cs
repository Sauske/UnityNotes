/**
* File: hanota.cs
* Created Time: 2023-07-18
* Author: hpstory (hpstory1024@163.com)
*/
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_divide_and_conquer
{


    public class hanota
    {
        /* 移动一个圆盘 */
        void Move(List<int> src, List<int> tar)
        {
            // 从 src 顶部拿出一个圆盘
            int pan = src[^1];
            src.RemoveAt(src.Count - 1);
            // 将圆盘放入 tar 顶部
            tar.Add(pan);
        }

        /* 求解汉诺塔问题 f(i) */
        void DFS(int i, List<int> src, List<int> buf, List<int> tar)
        {
            // 若 src 只剩下一个圆盘，则直接将其移到 tar
            if (i == 1)
            {
                Move(src, tar);
                return;
            }
            // 子问题 f(i-1) ：将 src 顶部 i-1 个圆盘借助 tar 移到 buf
            DFS(i - 1, src, tar, buf);
            // 子问题 f(1) ：将 src 剩余一个圆盘移到 tar
            Move(src, tar);
            // 子问题 f(i-1) ：将 buf 顶部 i-1 个圆盘借助 src 移到 tar
            DFS(i - 1, buf, src, tar);
        }

        /* 求解汉诺塔问题 */
        void SolveHanota(List<int> A, List<int> B, List<int> C)
        {
            int n = A.Count;
            // 将 A 顶部 n 个圆盘借助 B 移到 C
            DFS(n, A, B, C);
        }

        [TestAttibute]
        public void Test()
        {
            // 列表尾部是柱子顶部
            List<int> A = new List<int> { 5, 4, 3, 2, 1 };
            List<int> B = new List<int>();
            List<int> C = new List<int>();
            Debug.Log("初始状态下：");
            Debug.Log("A = " + string.Join(", ", A));
            Debug.Log("B = " + string.Join(", ", B));
            Debug.Log("C = " + string.Join(", ", C));

            SolveHanota(A, B, C);

            Debug.Log("圆盘移动完成后：");
            Debug.Log("A = " + string.Join(", ", A));
            Debug.Log("B = " + string.Join(", ", B));
            Debug.Log("C = " + string.Join(", ", C));
        }
    }
}
