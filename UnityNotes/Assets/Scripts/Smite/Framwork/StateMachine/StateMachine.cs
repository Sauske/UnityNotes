//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================
using System;
using System.Reflection;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class AutoRegisterAttribute : Attribute { }

    /// <summary>
    /// 状态接口
    /// </summary>
    public interface IState
    {
        string name { get; }
        /// <summary>
        /// 状态进入状态栈
        /// </summary>
        void OnStateEnter();
        /// <summary>
        /// 状态退出栈
        /// </summary>
        void OnStateLeave();
        /// <summary>
        /// 状态由栈顶变成非栈顶
        /// </summary>
        void OnStateOverride();
        /// <summary>
        /// 状态由非栈顶变成栈顶
        /// </summary>
        void OnStateResume();
    }

    public abstract class BaseState : IState
    {
        public virtual string name { get { return GetType().Name; } }

        public virtual void OnStateEnter() { }

        public virtual void OnStateLeave() { }

        public virtual void OnStateOverride() { }

        public virtual void OnStateResume() { }
    }


    public class StateMachine
    {
        //状态名-已注册的状态
        private Dictionary<string, IState> _registedState = new Dictionary<string, IState>();
        //状态堆栈
        private Stack<IState> _stateStack = new Stack<IState>();

        public IState tarState { get; private set; }

        public ClassEnumerator RegisterStateByAttributes<TAttributeType>(Assembly InAssembly,params object[] args) 
            where TAttributeType:AutoRegisterAttribute
        {
            var classes = new ClassEnumerator(typeof(TAttributeType), typeof(IState), InAssembly);

            var iter = classes.Results.GetEnumerator();

            while (iter.MoveNext())
            {
                var StateType = iter.Current;

                IState stateObj = (IState)System.Activator.CreateInstance(StateType, args);

                RegisterState(stateObj.name, stateObj);
            }
            return classes;
        }

        public ClassEnumerator RegisterStateByAttributes<TAttributeType>(Assembly InAssembly) where TAttributeType:AutoRegisterAttribute
        {
            var classes = new ClassEnumerator(typeof(TAttributeType), typeof(IState), InAssembly);

            var iter = classes.Results.GetEnumerator();

            while (iter.MoveNext())
            {
                var stateType = iter.Current;

                IState stateObj = (IState)System.Activator.CreateInstance(stateType);

                RegisterState(stateObj.name,stateObj);
            }

            return classes;
        }


        public void RegisterState(string name,IState state)
        {
            if (string.IsNullOrEmpty(name) || state == null || _registedState.ContainsKey(name)) return;

            _registedState.Add(name, state);
        }

        public void RegisterState()
        {

        }

        /// <summary>
        /// 注销状态
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IState UnregisterState(string name)
        {
            if (string.IsNullOrEmpty(name)) return default(IState);

            IState state;

            if (!_registedState.TryGetValue(name, out state))
            {
                return default(IState);
            }
            _registedState.Remove(name);
            return state;
        }

        public IState GetState(string name)
        {
            if(string.IsNullOrEmpty(name)) return default(IState);

            IState state;
            return _registedState.TryGetValue(name, out state) ? state : default(IState);
        }

        public string GetStateName(IState state)
        {
            if (state == null) return null;

            var etr = _registedState.GetEnumerator();

            KeyValuePair<string, IState> pair;
            while (etr.MoveNext())
            {
                pair = etr.Current;
                if (pair.Value == state)
                    return pair.Key;
            }
            return null;
        }

        public void Push(IState state)
        {
            if (state == null) return;

            if (_stateStack.Count > 0)
            {
                _stateStack.Peek().OnStateOverride();
            }
            _stateStack.Push(state);
            state.OnStateEnter();
        }

        public void Push(string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            IState state;
            if (!_registedState.TryGetValue(name, out state))
            {
                return;
            }
            Push(state);
        }

        /// <summary>
        ///  弹出状态
        /// </summary>
        /// <returns></returns>
        public IState Pop()
        {
            if (_stateStack.Count <= 0) return default(IState);

            IState state = _stateStack.Pop();

            state.OnStateLeave();
            if (_stateStack.Count > 0)
            {
                _stateStack.Peek().OnStateResume();
            }
            return state;
        }

        /// <summary>
        /// 修改栈顶状态
        /// </summary>
        /// <param name="state">新栈顶状态</param>
        /// <returns>原栈顶状态</returns>
        public IState Change(IState state)
        {
            if(state == null) return default(IState);

            tarState = state;

            IState oldState = default(IState);

            if (_stateStack.Count > 0)
            {
                oldState = _stateStack.Pop();
                oldState.OnStateLeave();
            }

            _stateStack.Push(state);
            state.OnStateEnter();

            return oldState;
        }
        /// <summary>
        /// 修改栈顶状态
        /// </summary>
        /// <param name="name">新栈顶状态名</param>
        /// <returns>原栈顶状态</returns>
        public IState ChangeState(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                DebugHelper.LogWarning("没有状态机类型，使用默认类型：");
                return default(IState);
            }
            IState state;
            if (!_registedState.TryGetValue(name, out state))
            {
                DebugHelper.LogError("没有找到状态机类型：" + name);
                return default(IState);
            }
            return state;
        }

        /// <summary>
        /// 获取栈顶状态
        /// </summary>
        /// <returns></returns>
        public IState Top()
        {
            if(_stateStack.Count <= 0) return default(IState);

            return _stateStack.Peek();
        }

        public string TopName()
        {
            if (_stateStack.Count <= 0) return null;

            IState state = _stateStack.Peek();
            return GetStateName(state);
        }

        public void Clear()
        {
            while (_stateStack.Count > 0)
            {
                _stateStack.Pop().OnStateLeave();
            }
        }

        public int Count { get { return _stateStack.Count; } }
    }
}