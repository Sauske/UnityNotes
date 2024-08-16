using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum enSocketState
    {
        Connecting,     //建立连接中
        Reconnecting,   //重新连接中
        Connected,      //连接成功
        Disconnecting,  //断开连接中
        Disconnected,   //连接断开
        ConnectedTimeout,   //连接超时
        ConnectError,       //连接出错
    }

    public class SocketConn
    {
     //   string mName;

        string mLastIP;
        int mLastPort;

        Socket mSocket;

        enSocketState mPreSocketState;
        enSocketState mSocketState;
        ISocketHandler iHandler;
        
        float m_fConnectTimeout = 5f;
        float m_fBeginConnectTime = -1;

        string m_sConnectHost;
        int m_iConnectPort;

        int mConnectedFrameCount = 0;
        

        public enSocketState State { get { return mSocketState; } }
        public Socket Sock { get { return mSocket; } }

        public SocketConn(string name = "")
        {
          //  mName = name;
            mPreSocketState = enSocketState.Disconnected;
            mSocketState = enSocketState.Disconnected;
            mConnectedFrameCount = 0;
        }

        public void RegHandler(ISocketHandler handler)
        {
            iHandler = handler;
        }

        public void CustomUpdate()
        {
            if (m_fBeginConnectTime > 0 && Time.realtimeSinceStartup - m_fBeginConnectTime > m_fConnectTimeout)
            {
                Timeout();
                return;
            }

            if (mSocketState != enSocketState.Connected) 
                return;

            // 连接成功后，延迟一帧通知上层
            if (mConnectedFrameCount == 1)
            {
                mConnectedFrameCount++;
                if(iHandler!=null)
                {
                    if (mPreSocketState == enSocketState.Reconnecting)
                        iHandler.OnReconnect(RGameSocketType.TCP);
                    else
                        iHandler.OnConnect(RGameSocketType.TCP);
                }
            }

            try
            {
                //检测网络是否出错
                if (mSocket != null && mSocket.Poll(1, SelectMode.SelectError))
                {
                    if (iHandler != null)
                    {
                        mPreSocketState = mSocketState;
                        mSocketState = enSocketState.ConnectError;
                        try { iHandler.OnError(RGameSocketType.TCP); }
                        catch (SocketException se) 
                        {
                            iHandler.OnException(se, RGameSocketType.TCP); 
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugHelper.LogError("socket exception: " +  ex.Message);
            }
        }

        public bool Connect(string host, int port, float timeout = 1f)
        {
            if (mSocketState != enSocketState.Disconnected)
            {
                DebugHelper.LogWarning("CONNECT DISCONNECT");
                Disconnect();
            }

            mPreSocketState = mSocketState;
            mSocketState = enSocketState.Connecting;
            m_fConnectTimeout = timeout;
            mConnectedFrameCount = 0;

            String newServerIp = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;
            IPv6SupportMidleware.GetIPType(host, port.ToString(), out newServerIp, out newAddressFamily);
            try
            {
                mSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IPAddress addr = IPAddress.Parse(host);
                IPEndPoint endpoint = new IPEndPoint(addr, port);

                DebugHelper.LogWarning("Begin connect to Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + newServerIp + ", Port:" + port);
                mSocket.NoDelay = true;
                mSocket.BeginConnect(endpoint, new AsyncCallback(EndConnect), new KeyValuePair<string, int>(host, port));
                m_fBeginConnectTime = Time.realtimeSinceStartup;
                
                return true;
            }
            catch (SocketException e)
            {
                DebugHelper.LogError("Create Socket Connect error:" + e.Message);
                Disconnect();
            }

            return false;
        }

        public bool Reconnect()
        {
            if (string.IsNullOrEmpty(mLastIP))
                throw new Exception("cannot reconnect, last EndPoint is null");

            if (mSocketState != enSocketState.Disconnected)
            {
                DebugHelper.LogWarning("Reconnect AAAA");
                Disconnect();
                return false;
            }

            mPreSocketState = mSocketState;
            mSocketState = enSocketState.Reconnecting;
            mConnectedFrameCount = 0;

            String newServerIp = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;
            IPv6SupportMidleware.GetIPType(mLastIP, mLastPort.ToString(), out newServerIp, out newAddressFamily);
            try
            {
                mSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mSocket.NoDelay = true;
                DebugHelper.LogWarning("Begin connect to Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + mLastIP + ", Port:" + mLastPort);
                mSocket.BeginConnect(newServerIp, mLastPort, new AsyncCallback(EndConnect), new KeyValuePair<string, int>(mLastIP, mLastPort));
                m_fBeginConnectTime = Time.realtimeSinceStartup;
                return true;
            }
            catch (SocketException e)
            {
                DebugHelper.LogError("Reconnect error: "+e.Message);
                Disconnect();
            }
            return false;
        }

        void EndConnect(IAsyncResult async)
        {
            Debug.Log("EndConnect!");
            try
            {
                mSocket.EndConnect(async);


                if (mSocketState != enSocketState.Reconnecting)
                {
                    KeyValuePair<string, int> pair = (KeyValuePair<string, int>)async.AsyncState;
                    mLastIP = pair.Key;
                    mLastPort = pair.Value;
                }

                mConnectedFrameCount = 1;
                mPreSocketState = mSocketState;
                mSocketState = enSocketState.Connected;
            }
            catch (SocketException e)
            {
                if (!e.NativeErrorCode.Equals(10035))
                {
                    DebugHelper.LogError("socket connect error: " + e.Message);
                    Disconnect();
                }
            }
            m_fBeginConnectTime = -1f;
        }

        public void Disconnect(bool ignoreEvent = false)
        {
            DebugHelper.Log("socketconn:Disconnect");
            if (mSocket != null)
            {
                mPreSocketState = mSocketState;
                mSocketState = enSocketState.Disconnecting;
                try { mSocket.Shutdown(SocketShutdown.Both); }
                catch (SocketException) { }
                finally
                {
                    mSocket.Close(0);
                    mSocket = null;
                }
            }

            mPreSocketState = mSocketState;
            mSocketState = enSocketState.Disconnected;
            m_fBeginConnectTime = -1f;
            mConnectedFrameCount = 0;

            if (!ignoreEvent)
            {
                if (iHandler != null)
                {
                    iHandler.OnDisconnect(RGameSocketType.TCP); 
                }
            }
        }


        private void Timeout()
        {
            mPreSocketState = mSocketState;
            mSocketState = enSocketState.ConnectedTimeout;
            m_fBeginConnectTime = -1f;

            if (iHandler != null)
            {
                iHandler.OnTimeout(RGameSocketType.TCP);
            }
        }
    }
}
