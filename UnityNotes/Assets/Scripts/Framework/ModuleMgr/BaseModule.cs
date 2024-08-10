using UnityEngine.Events;

namespace UMI
{
    public class BaseModule
    {
        protected UnityAction<int> NetAction = null;

        public BaseModule()
        {
            Initialize();
        }

        /// <summary>
        /// 开始阶段
        /// </summary>
        protected virtual void Initialize()
        {

        }


        /// <summary>
        /// 更新模块
        /// </summary>
        public virtual void OnUpdate()
        {

        }


        /// <summary>
        /// 结束模块
        /// </summary>
        public virtual void OnDispose()
        {

        }

        protected void NetCallback(int proto)
        {
            if (NetAction != null)
            {
                NetAction(proto);

                NetAction = null;
            }
        }
    }
}
