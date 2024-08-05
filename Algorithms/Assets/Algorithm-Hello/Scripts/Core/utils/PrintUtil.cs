/**
* File: PrintUtil.cs
* Created Time: 2022-12-23
* Author: haptear (haptear@hotmail.com), krahets (krahets@163.com)
*/
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace hello_algo.utils
{

    public class Trunk
    {
        public Trunk prev;
        public string str;

        public Trunk(Trunk _prev,string _str)
        {
            this.prev = _prev;
            this.str = _str;
        }
    };

    public static class PrintUtil
    {
        /* 打印列表 */
        public static void PrintList<T>(IList<T> list)
        {
            Debug.Log("[" + string.Join(", ", list) + "]");
        }

        public static string PrintList<T>(this IEnumerable<T?> list)
        {
            return $"[ {string.Join(", ", list.Select(x => x?.ToString() ?? "null"))} ]";
        }

        /* 打印矩阵 (Array) */
        public static void PrintMatrix<T>(T[][] matrix)
        {
            Debug.Log("[");
            foreach (T[] row in matrix)
            {
                Debug.Log("  " + string.Join(", ", row) + ",");
            }
            Debug.Log("]");
        }

        /* 打印矩阵 (List) */
        public static void PrintMatrix<T>(List<List<T>> matrix)
        {
            Debug.Log("[");
            foreach (List<T> row in matrix)
            {
                Debug.Log("  " + string.Join(", ", row) + ",");
            }
            Debug.Log("]");
        }

        /* 打印链表 */
        public static void PrintLinkedList(ListNode? head)
        {
            List<string> list = new List<string>();
            while (head != null)
            {
                list.Add(head.val.ToString());
                head = head.next;
            }
            Debug.Log(string.Join(" -> ", list));
        }

        /**
         * 打印二叉树
         * This tree printer is borrowed from TECHIE DELIGHT
         * https://www.techiedelight.com/c-program-print-binary-tree/
         */
        public static void PrintTree(TreeNode root)
        {
            PrintTree(root, null, false);
        }

        /* 打印二叉树 */
        public static void PrintTree(TreeNode root, Trunk prev, bool isRight)
        {
            if (root == null)
            {
                return;
            }

            string prev_str = "    ";
            Trunk trunk = new(prev, prev_str);

            PrintTree(root.right, trunk, true);

            if (prev == null)
            {
                trunk.str = "———";
            }
            else if (isRight)
            {
                trunk.str = "/———";
                prev_str = "   |";
            }
            else
            {
                trunk.str = "\\———";
                prev.str = prev_str;
            }

            ShowTrunks(trunk);
            Debug.Log(" " + root.val);

            if (prev != null)
            {
                prev.str = prev_str;
            }
            trunk.str = "   |";

            PrintTree(root.left, trunk, false);
        }

        public static void ShowTrunks(Trunk p)
        {
            if (p == null)
            {
                return;
            }

            ShowTrunks(p.prev);
            Debug.Log(p.str);
        }

        /* 打印哈希表 */
        public static void PrintHashMap<K, V>(Dictionary<K, V> map) where K : notnull
        {
            foreach (var kv in map.Keys)
            {
                Debug.Log(kv.ToString() + " -> " + map[kv]?.ToString());
            }
        }

        public static void PrintHeap(Queue<int> queue)
        {
            Debug.Log("堆的数组表示：");
            List<int> list = new List<int>(queue);
            Debug.Log(string.Join(',', list));
            Debug.Log("堆的树状表示：");
            TreeNode? tree = TreeNode.ListToTree(list);
            PrintTree(tree);
        }

        /* 打印堆 */
        public static void PrintHeap(Queue<int?> queue)
        {
            Debug.Log("堆的数组表示：");
            List<int?> list = new List<int?>(queue);
            Debug.Log(string.Join(',', list));
            Debug.Log("堆的树状表示：");
            TreeNode? tree = TreeNode.ListToTree(list);
            PrintTree(tree);
        }

        /* 打印优先队列 */
        public static void PrintHeap(PriorityQueue<int, int> queue)
        {
            var newQueue = new PriorityQueue<int, int>(queue);
            Debug.Log("堆的数组表示：");
            List<int?> list = new List<int?>();
            int temp = -1;
            while (newQueue.TryDequeue(out int element, temp))
            {
                list.Add(element);
            }
            Debug.Log("堆的树状表示：");
            Debug.Log(string.Join(',', list.ToList()));
            TreeNode? tree = TreeNode.ListToTree(list);
            PrintTree(tree);
        }
    }
}