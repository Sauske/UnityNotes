/**
 * File: preorder_traversal_i_compact.cs
 * Created Time: 2023-04-17
 * Author: hpstory (hpstory1024@163.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;
using System.Linq;

namespace hello_algo.chapter_backtracking
{


    public class preorder_traversal_i_compact
    {
        List<TreeNode> res = new List<TreeNode>();

        /* 前序遍历：例题一 */
        void PreOrder(TreeNode? root)
        {
            if (root == null)
            {
                return;
            }
            if (root.val == 7)
            {
                // 记录解
                res.Add(root);
            }
            PreOrder(root.left);
            PreOrder(root.right);
        }

        [TestAttibute]
        public void Test()
        {
            TreeNode? root = TreeNode.ListToTree(new List<int?>{ 1, 7, 3, 4, 5, 6, 7});
            Debug.Log("\n初始化二叉树");
            PrintUtil.PrintTree(root);

            // 前序遍历
            PreOrder(root);

            Debug.Log("\n输出所有值为 7 的节点");
            PrintUtil.PrintList(res.Select(p => p.val).ToList());
        }
    }
}
