/**
* File: array_binary_tree.cs
* Created Time: 2023-07-20
* Author: hpstory (hpstory1024@163.com)
*/

using UnityEngine;
using System.Collections.Generic;
using hello_algo.utils;

namespace hello_algo.chapter_tree
{

    /* 数组表示下的二叉树类 */
    public class ArrayBinaryTree
    {
        List<int?> tree;

        public ArrayBinaryTree(List<int?> arr)
        {
            tree = arr;
        }

        /* 列表容量 */
        public int Size()
        {
            return tree.Count;
        }

        /* 获取索引为 i 节点的值 */
        public int? Val(int i)
        {
            // 若索引越界，则返回 null ，代表空位
            if (i < 0 || i >= Size())
                return null;
            return tree[i];
        }

        /* 获取索引为 i 节点的左子节点的索引 */
        public int Left(int i)
        {
            return 2 * i + 1;
        }

        /* 获取索引为 i 节点的右子节点的索引 */
        public int Right(int i)
        {
            return 2 * i + 2;
        }

        /* 获取索引为 i 节点的父节点的索引 */
        public int Parent(int i)
        {
            return (i - 1) / 2;
        }

        /* 层序遍历 */
        public List<int> LevelOrder()
        {
            List<int> res = new List<int>();
            // 直接遍历数组
            for (int i = 0; i < Size(); i++)
            {
                if (Val(i).HasValue)
                    res.Add(Val(i)!.Value);
            }
            return res;
        }

        /* 深度优先遍历 */
        void DFS(int i, string order, List<int> res)
        {
            // 若为空位，则返回
            if (!Val(i).HasValue)
                return;
            // 前序遍历
            if (order == "pre")
                res.Add(Val(i)!.Value);
            DFS(Left(i), order, res);
            // 中序遍历
            if (order == "in")
                res.Add(Val(i)!.Value);
            DFS(Right(i), order, res);
            // 后序遍历
            if (order == "post")
                res.Add(Val(i)!.Value);
        }

        /* 前序遍历 */
        public List<int> PreOrder()
        {
            List<int> res = new List<int>();
            DFS(0, "pre", res);
            return res;
        }

        /* 中序遍历 */
        public List<int> InOrder()
        {
            List<int> res = new List<int>();
            DFS(0, "in", res);
            return res;
        }

        /* 后序遍历 */
        public List<int> PostOrder()
        {
            List<int> res = new List<int>();
            DFS(0, "post", res);
            return res;
        }
    }

    public class array_binary_tree
    {
        [TestAttibute]
        public void Test()
        {
            // 初始化二叉树
            // 这里借助了一个从数组直接生成二叉树的函数
            List<int?> arr = new List<int?> { 1, 2, 3, 4, null, 6, 7, 8, 9, null, null, 12, null, null, 15 };

            TreeNode? root = TreeNode.ListToTree(arr);
            Debug.Log("\n初始化二叉树\n");
            Debug.Log("二叉树的数组表示：");
            Debug.Log(arr.PrintList());
            Debug.Log("二叉树的链表表示：");
            PrintUtil.PrintTree(root);

            // 数组表示下的二叉树类
            ArrayBinaryTree abt = new(arr);

            // 访问节点
            int i = 1;
            int l = abt.Left(i);
            int r = abt.Right(i);
            int p = abt.Parent(i);
            Debug.Log("\n当前节点的索引为 " + i + " ，值为 " + abt.Val(i));
            Debug.Log("其左子节点的索引为 " + l + " ，值为 " + (abt.Val(l).HasValue ? abt.Val(l) : "null"));
            Debug.Log("其右子节点的索引为 " + r + " ，值为 " + (abt.Val(r).HasValue ? abt.Val(r) : "null"));
            Debug.Log("其父节点的索引为 " + p + " ，值为 " + (abt.Val(p).HasValue ? abt.Val(p) : "null"));

            // 遍历树
            List<int> res = abt.LevelOrder();
            Debug.Log("\n层序遍历为：" + res.PrintList());
            res = abt.PreOrder();
            Debug.Log("前序遍历为：" + res.PrintList());
            res = abt.InOrder();
            Debug.Log("中序遍历为：" + res.PrintList());
            res = abt.PostOrder();
            Debug.Log("后序遍历为：" + res.PrintList());
        }
    }
}