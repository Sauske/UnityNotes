/**
 * File: queue.cs
 * Created Time: 2022-12-23
 * Author: haptear (haptear@hotmail.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_stack_and_queue
{

    public class queue
    {
        [TestAttibute]
        public void Test()
        {
            /* 初始化队列 */
            Queue<int> queue = new();

            /* 元素入队 */
            queue.Enqueue(1);
            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(5);
            queue.Enqueue(4);
            Debug.Log("队列 queue = " + string.Join(",", queue));

            /* 访问队首元素 */
            int peek = queue.Peek();
            Debug.Log("队首元素 peek = " + peek);

            /* 元素出队 */
            int pop = queue.Dequeue();
            Debug.Log("出队元素 pop = " + pop + "，出队后 queue = " + string.Join(",", queue));

            /* 获取队列的长度 */
            int size = queue.Count;
            Debug.Log("队列长度 size = " + size);

            /* 判断队列是否为空 */
            bool isEmpty = queue.Count == 0;
            Debug.Log("队列是否为空 = " + isEmpty);
        }
    }
}
