
using System;
using System.Net.Sockets;

namespace Framework
{
    public enum RGameSocketType
    {
        TCP = 1,
        UDP = 2,
    }

    public interface ISocketHandler
    {
        /// <summary>
        /// socket连接
        /// </summary>
        void OnConnect(RGameSocketType type);

        /// <summary>
        /// 重新连接
        /// </summary>
        void OnReconnect(RGameSocketType type);

        /// <summary>
        /// 断开连接
        /// </summary>
        void OnDisconnect(RGameSocketType type);

        /// <summary>
        /// 连接超时
        /// </summary>
        void OnTimeout(RGameSocketType type);

        /// <summary>
        /// 连接失败
        /// </summary>
        void OnConnectFailed(RGameSocketType type);


        void OnError(RGameSocketType type);

        void OnException(SocketException se, RGameSocketType type);
    }
}
