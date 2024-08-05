﻿/**
* File: build_tree.cs
* Created Time: 2023-07-18
* Author: hpstory (hpstory1024@163.com)
*/
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_divide_and_conquer
{

    public class build_tree
    {
        /* 构建二叉树：分治 */
        TreeNode? DFS(int[] preorder, Dictionary<int, int> inorderMap, int i, int l, int r)
        {
            // 子树区间为空时终止
            if (r - l < 0)
                return null;
            // 初始化根节点
            TreeNode root = new(preorder[i]);
            // 查询 m ，从而划分左右子树
            int m = inorderMap[preorder[i]];
            // 子问题：构建左子树
            root.left = DFS(preorder, inorderMap, i + 1, l, m - 1);
            // 子问题：构建右子树
            root.right = DFS(preorder, inorderMap, i + 1 + m - l, m + 1, r);
            // 返回根节点
            return root;
        }

        /* 构建二叉树 */
        TreeNode? BuildTree(int[] preorder, int[] inorder)
        {
            // 初始化哈希表，存储 inorder 元素到索引的映射
            Dictionary<int, int> inorderMap = new Dictionary<int, int>();
            for (int i = 0; i < inorder.Length; i++)
            {
                inorderMap.TryAdd(inorder[i], i);
            }
            TreeNode? root = DFS(preorder, inorderMap, 0, 0, inorder.Length - 1);
            return root;
        }

        [TestAttibute]
        public void Test()
        {
            int[] preorder = { 3, 9, 2, 1, 7 };
            int[] inorder = { 9, 3, 1, 2, 7 };
            Debug.Log("前序遍历 = " + string.Join(", ", preorder));
            Debug.Log("中序遍历 = " + string.Join(", ", inorder));

            TreeNode? root = BuildTree(preorder, inorder);
            Debug.Log("构建的二叉树为：");
            PrintUtil.PrintTree(root);
        }
    }
}
