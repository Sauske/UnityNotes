/**
 * File: binary_tree_dfs.cs
 * Created Time: 2022-12-23
 * Author: haptear (haptear@hotmail.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_tree
{

    public class binary_tree_dfs
    {
        List<int> list = new List<int>();

        /* 前序遍历 */
        void PreOrder(TreeNode? root)
        {
            if (root == null) return;
            // 访问优先级：根节点 -> 左子树 -> 右子树
            list.Add(root.val!.Value);
            PreOrder(root.left);
            PreOrder(root.right);
        }

        /* 中序遍历 */
        void InOrder(TreeNode? root)
        {
            if (root == null) return;
            // 访问优先级：左子树 -> 根节点 -> 右子树
            InOrder(root.left);
            list.Add(root.val!.Value);
            InOrder(root.right);
        }

        /* 后序遍历 */
        void PostOrder(TreeNode? root)
        {
            if (root == null) return;
            // 访问优先级：左子树 -> 右子树 -> 根节点
            PostOrder(root.left);
            PostOrder(root.right);
            list.Add(root.val!.Value);
        }

        [TestAttibute]
        public void Test()
        {
            /* 初始化二叉树 */
            // 这里借助了一个从数组直接生成二叉树的函数
            TreeNode? root = TreeNode.ListToTree(new List<int?> { 1, 2, 3, 4, 5, 6, 7 });
            Debug.Log("\n初始化二叉树\n");
            PrintUtil.PrintTree(root);

            list.Clear();
            PreOrder(root);
            Debug.Log("\n前序遍历的节点打印序列 = " + string.Join(",", list));

            list.Clear();
            InOrder(root);
            Debug.Log("\n中序遍历的节点打印序列 = " + string.Join(",", list));

            list.Clear();
            PostOrder(root);
            Debug.Log("\n后序遍历的节点打印序列 = " + string.Join(",", list));
        }
    }
}
