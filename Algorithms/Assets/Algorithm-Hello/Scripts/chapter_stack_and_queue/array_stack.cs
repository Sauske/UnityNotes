/**
 * File: array_stack.cs
 * Created Time: 2022-12-23
 * Author: haptear (haptear@hotmail.com)
 */
using UnityEngine;
using hello_algo.utils;
using System.Collections.Generic;

namespace hello_algo.chapter_stack_and_queue
{

    /* 基于数组实现的栈 */
    class ArrayStack
    {
        List<int> stack;
        public ArrayStack()
        {
            // 初始化列表（动态数组）
            stack = new List<int>();
        }

        /* 获取栈的长度 */
        public int Size()
        {
            return stack.Count;
        }

        /* 判断栈是否为空 */
        public bool IsEmpty()
        {
            return Size() == 0;
        }

        /* 入栈 */
        public void Push(int num)
        {
            stack.Add(num);
        }

        /* 出栈 */
        public int Pop()
        {
            if (IsEmpty())
                throw new System.Exception();
            var val = Peek();
            stack.RemoveAt(Size() - 1);
            return val;
        }

        /* 访问栈顶元素 */
        public int Peek()
        {
            if (IsEmpty())
                throw new System.Exception();
            return stack[Size() - 1];
        }

        /* 将 List 转化为 Array 并返回 */
        public int[] ToArray()
        {
            return stack.ToArray();
        }
    }

    public class array_stack
    {
        [TestAttibute]
        public void Test()
        {
            /* 初始化栈 */
            ArrayStack stack = new();

            /* 元素入栈 */
            stack.Push(1);
            stack.Push(3);
            stack.Push(2);
            stack.Push(5);
            stack.Push(4);
            Debug.Log("栈 stack = " + string.Join(",", stack.ToArray()));

            /* 访问栈顶元素 */
            int peek = stack.Peek();
            Debug.Log("栈顶元素 peek = " + peek);

            /* 元素出栈 */
            int pop = stack.Pop();
            Debug.Log("出栈元素 pop = " + pop + "，出栈后 stack = " + string.Join(",", stack.ToArray()));

            /* 获取栈的长度 */
            int size = stack.Size();
            Debug.Log("栈的长度 size = " + size);

            /* 判断是否为空 */
            bool isEmpty = stack.IsEmpty();
            Debug.Log("栈是否为空 = " + isEmpty);
        }
    }
}
