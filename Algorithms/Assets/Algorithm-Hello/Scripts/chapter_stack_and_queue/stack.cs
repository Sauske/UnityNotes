/**
 * File: stack.cs
 * Created Time: 2022-12-23
 * Author: haptear (haptear@hotmail.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_stack_and_queue
{

    public class stack
    {

        [TestAttibute]
        public void Test()
        {
            /* 初始化栈 */
            Stack<int> stack = new();

            /* 元素入栈 */
            stack.Push(1);
            stack.Push(3);
            stack.Push(2);
            stack.Push(5);
            stack.Push(4);
            // 请注意，stack.ToArray() 得到的是倒序序列，即索引 0 为栈顶
            Debug.Log("栈 stack = " + string.Join(",", stack));

            /* 访问栈顶元素 */
            int peek = stack.Peek();
            Debug.Log("栈顶元素 peek = " + peek);

            /* 元素出栈 */
            int pop = stack.Pop();
            Debug.Log("出栈元素 pop = " + pop + "，出栈后 stack = " + string.Join(",", stack));

            /* 获取栈的长度 */
            int size = stack.Count;
            Debug.Log("栈的长度 size = " + size);

            /* 判断是否为空 */
            bool isEmpty = stack.Count == 0;
            Debug.Log("栈是否为空 = " + isEmpty);
        }
    }
}