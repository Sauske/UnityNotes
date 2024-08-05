﻿/**
 * File: permutations_i.cs
 * Created Time: 2023-04-24
 * Author: hpstory (hpstory1024@163.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_backtracking
{

    public class permutations_i
    {
        /* 回溯算法：全排列 I */
        void Backtrack(List<int> state, int[] choices, bool[] selected, List<List<int>> res)
        {
            // 当状态长度等于元素数量时，记录解
            if (state.Count == choices.Length)
            {
                res.Add(new List<int>(state));
                return;
            }
            // 遍历所有选择
            for (int i = 0; i < choices.Length; i++)
            {
                int choice = choices[i];
                // 剪枝：不允许重复选择元素
                if (!selected[i])
                {
                    // 尝试：做出选择，更新状态
                    selected[i] = true;
                    state.Add(choice);
                    // 进行下一轮选择
                    Backtrack(state, choices, selected, res);
                    // 回退：撤销选择，恢复到之前的状态
                    selected[i] = false;
                    state.RemoveAt(state.Count - 1);
                }
            }
        }

        /* 全排列 I */
        List<List<int>> PermutationsI(int[] nums)
        {
            List<List<int>> res = new List<List<int>>();
            Backtrack(new List<int>(), nums, new bool[nums.Length], res);
            return res;
        }

        [TestAttibute]
        public void Test()
        {
            int[] nums = { 1, 2, 3 };

            List<List<int>> res = PermutationsI(nums);

            Debug.Log("输入数组 nums = " + string.Join(", ", nums));
            Debug.Log("所有排列 res = ");
            foreach (List<int> permutation in res)
            {
                PrintUtil.PrintList(permutation);
            }
        }
    }
}
