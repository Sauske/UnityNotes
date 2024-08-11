using System;

namespace SFramework
{

    public interface IModule : IDisposable
    {
        void Initialize();

        void OnUpdate(float delta);
    }

    public interface IUpdate
    {
        void OnUpdate(float delta);
    }

    /// <summary>
    /// 登录模块接口，需要在登录成功做初始化或者注销后处理事件的需要实现该接口
    /// 这里调用无法保证各个模块的调用顺序
    /// </summary>
    public interface ILogin
    {
        /// <summary>
        /// 登录成功，这里只负责处理自己模块内部的数据和状态维护
        /// </summary>
        public void OnLogin();
        /// <summary>
        /// 退出登录成功
        /// </summary>
        public void OnLogout();
    }


    /// <summary>
    /// 案例 Bag
    /// </summary>
    public class BagModule : IModule
    {
        public void Initialize() { }

        public void OnUpdate(float delta) { }

        public void Dispose() { }
    }
}


