/**
 * File: preorder_traversal_ii_compact.cs
 * Created Time: 2023-04-17
 * Author: hpstory (hpstory1024@163.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;
using System.Linq;

namespace hello_algo.chapter_backtracking
{


    public class preorder_traversal_ii_compact
    {
        List<TreeNode> path = new List<TreeNode>();
        List<List<TreeNode>> res = new List<List<TreeNode>>();

        /* 前序遍历：例题二 */
        void PreOrder(TreeNode? root)
        {
            if (root == null)
            {
                return;
            }
            // 尝试
            path.Add(root);
            if (root.val == 7)
            {
                // 记录解
                res.Add(new List<TreeNode>(path));
            }
            PreOrder(root.left);
            PreOrder(root.right);
            // 回退
            path.RemoveAt(path.Count - 1);
        }

        [TestAttibute]
        public void Test()
        {
            TreeNode? root = TreeNode.ListToTree(new List<int?> { 1, 7, 3, 4, 5, 6, 7 });
            Debug.Log("\n初始化二叉树");
            PrintUtil.PrintTree(root);

            // 前序遍历
            PreOrder(root);

            Debug.Log("\n输出所有根节点到节点 7 的路径");
            foreach (List<TreeNode> path in res)
            {
                PrintUtil.PrintList(path.Select(p => p.val).ToList());
            }
        }
    }
}
