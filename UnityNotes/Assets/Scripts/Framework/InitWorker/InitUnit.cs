using System.Threading.Tasks;

namespace UMI
{
    /// <summary>
    /// 游戏初始化单元
    /// </summary>
    public abstract class InitUnit
    {
        /// <summary>
        /// 初始化行为
        /// </summary>
        public async Task Init()
        {
            await OnInit();
        }

        /// <summary>
        /// 自定义初始化行为
        /// </summary>
        protected virtual async Task OnInit()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 释放行为
        /// </summary>
        public async void Dispose()
        {
           await OnDispose();
        }

        /// <summary>
        /// 自定义释放行为
        /// </summary>
        protected virtual async Task OnDispose()
        {
            await Task.CompletedTask;
        }
    }
}