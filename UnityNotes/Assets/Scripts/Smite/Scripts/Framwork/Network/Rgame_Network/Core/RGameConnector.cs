using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

namespace Framework
{
    public enum enNetResult
    {
        Success = 0,
        Error,
        ConnectFailed,
        NetworkException,
        Timeout,
        InvalidArgument,
        LengthError,
        Unknown,
        Empty,
    }

    public class RGameConnector:ISocketHandler
    {
        const int RECV_BUFF_SIZE = 1024 * 1024;

        private SocketConn mSocketConn = null;
        public UdpBaseServer mUdpServer { get; set; }//jasonbao

        public Action ConnectEvent = null;
        public Action ReconnectEvent = null;
        public Action DisconnectEvent = null;
        public Action ErrorEvent = null;
        public Action ConnectFailedEvent = null;
        public Action TimeoutEvent = null;

        private string ip;  //IP地址
        private ushort port; //端口
        private bool bUdp;  //是否是udp
        private string uIp; //udp ip地址
        private ushort uShort; //udp 的端口
        private float m_fConnectTimeout = 5;

        private byte[] mRecvBuffer = new byte[RECV_BUFF_SIZE];

        public bool Connected { get; private set; }

        public RGameConnector(string ip,ushort port,bool udp,string uip="",ushort uport=0)
        {
            this.ip = ip;
            this.port = port;
            this.bUdp = udp;

            Connected = false;

            if (!this.bUdp)
            {
                mSocketConn = new SocketConn("rgameTcp");
                mSocketConn.RegHandler(this);
            }
            else
            {
                this.uIp = uip;
                this.uShort = port;

                //UDP服务器的创建
                mUdpServer = new UdpBaseServer();
                mUdpServer.RegHandler(this);

                mSocketConn = new SocketConn("relayTcp");
                mSocketConn.RegHandler(this);
            }
        }

        ~RGameConnector()
        {
            DebugHelper.Log("RGameConnector");
            Disconnect();
        }

        public void CustomUpdate()
        {
            if(!bUdp)
            {
                if (mSocketConn != null)
                    mSocketConn.CustomUpdate();
            }
            else
            {
                if(mUdpServer != null)
                    mUdpServer.CustomUpdate();

                if (mSocketConn != null)
                    mSocketConn.CustomUpdate();
            }
           
        }

        public bool WriteUdpData(byte[] data,int len)
        {
            if (mUdpServer == null)
            {
                return false;
            }

            if (len == -1)
            {
                len = data.Length;
                return false;
            }

            mUdpServer.SendMessage(data);
            return true;
        }

        public bool ReadUdpData(out byte[] buffer, out int realLength)
        {
            buffer = null;
            realLength = 0;

            if (mUdpServer != null)
            {
                if (mUdpServer.ReadQueue.Count != 0)
                {
                    //读取并删除
                    UdpBaseMessage bs = mUdpServer.ReadQueue.Dequeue();
                    if (bs != null)
                    {
                        buffer = bs.bytes;
                        realLength = bs.bytes.Length;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool WriteData(byte[] data,int len)
        {
            try
            {
                if (data == null) return false;

                if (mSocketConn == null || mSocketConn.Sock == null || mSocketConn.State != enSocketState.Connected)
                    return false;

                if (!mSocketConn.Sock.Poll(1, SelectMode.SelectWrite)) return false;

                try
                {
                    mSocketConn.Sock.Send(data, 0, len, SocketFlags.None);
                    return true;
                }
                catch (SocketException se)
                {
                    DebugHelper.LogError("Socket Send Exception: " + se.Message);
                }

            }
            catch(System.Exception ex)
            {
                DebugHelper.LogError("Tcp Write Data Error: " + ex.Message);
            }
            
            return false;
        }

        public bool WriteData(byte[] data)
        {
            return WriteData(data, data.Length);
        }

        public bool ReadData(out byte[] data, out int len)
        {
            data = null;
            len = 0;

            try
            {
                if (mSocketConn == null || mSocketConn.Sock == null || mSocketConn.State != enSocketState.Connected || !mSocketConn.Sock.Poll(1, SelectMode.SelectRead))
                {      
                    return false;
                }

                int recvSize = 0;
                try
                {
                    recvSize = mSocketConn.Sock.Receive(mRecvBuffer, 0, RECV_BUFF_SIZE, SocketFlags.None);  
                }catch(SocketException ex)
                {
                    DebugHelper.LogError(ex.Message);
                    OnError(RGameSocketType.TCP);
                    return false;
                }

                if (recvSize <= 0)
                {
                    return false;
                }

                len = recvSize;
                data = new byte[recvSize];
                Array.Copy(mRecvBuffer, 0, data, 0, recvSize);

                return true;

            }
            catch (Exception e)
            {
                DebugHelper.LogError(e.Message);
            }
            return false;
        }

        public bool Connect()
        {
            if(!bUdp)
            {
                if (mSocketConn == null) return false;

                return mSocketConn.Connect(this.ip, this.port, m_fConnectTimeout);
            }
            else
            {
                if (mUdpServer == null || mSocketConn == null)
                    return false;

                if (mUdpServer.Connect(this.uIp, this.uShort) && mSocketConn.Connect(this.ip,this.port,m_fConnectTimeout))
                {
                    return true;
                }

                return false;
            }
        
        }

        public bool Connect(float timeout)
        {
            m_fConnectTimeout = timeout;

            return Connect();
        }

        public bool Reconnect()
        {
            if (!bUdp)
            {
                if (mSocketConn == null) return false;

                return mSocketConn.Reconnect();
            }
            else
            {
                if (mUdpServer == null) return false;

                return mUdpServer.Reconnect();
            }
        }

        public enNetResult Reconnect(float timeout)
        {
            return enNetResult.Empty;
        }

        public void Disconnect()
        {
            if(!bUdp)
            {
                if (mSocketConn != null)
                {
                    DebugHelper.LogWarning("Disconnect For TCP");
                    mSocketConn.Disconnect();
                }
            }
            else
            {
                if (mUdpServer != null)
                {

                    DebugHelper.LogWarning("Disconnect For udp");

                    mUdpServer.Disconnect();
                }

                if(mSocketConn!=null)
                {

                    DebugHelper.LogWarning("Disconnect For TCP IN UDP");

                    mSocketConn.Disconnect();
                }
            }
            
        }


        public void OnConnect(RGameSocketType type)
        {
            if (!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    Connected = true;

                    if (ConnectEvent != null)
                        ConnectEvent();
                }
            }else
            {
                if(mSocketConn!=null && mSocketConn.State == enSocketState.Connected && mUdpServer!=null && mUdpServer.State == enSocketState.Connected)
                {
                    Connected = true;

                    if (ConnectEvent != null)
                        ConnectEvent();
                }
            }
          
        }

        public void OnReconnect(RGameSocketType type)
        {
            if(!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    Connected = true;

                    if (ReconnectEvent != null)
                        ReconnectEvent();
                }    
            }else
            {
                if (mSocketConn != null && mSocketConn.State == enSocketState.Connected && mUdpServer != null && mUdpServer.State == enSocketState.Connected)
                {
                    Connected = true;

                    if (ReconnectEvent != null)
                        ReconnectEvent();
                }
            }
           
        }

        public void OnDisconnect(RGameSocketType type)
        {
            if(!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    Connected = false;

                    if (DisconnectEvent != null)
                        DisconnectEvent();
                }         
            }else
            {
                if (mSocketConn != null && mSocketConn.State == enSocketState.Disconnected && mUdpServer != null && mUdpServer.State == enSocketState.Disconnected)
                {
                    Connected = false;

                    if (DisconnectEvent != null)
                        DisconnectEvent();
                }
            }
        }

        public void OnTimeout(RGameSocketType type)
        {
            DebugHelper.Log("Connect Timeout");

            if(!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    if (TimeoutEvent != null)
                        TimeoutEvent();
                }
            }else
            {
                if (mSocketConn != null && mSocketConn.State == enSocketState.ConnectedTimeout && type == RGameSocketType.TCP)
                {
                    if (TimeoutEvent != null)
                        TimeoutEvent();
                }
            }
        }

        public void OnError(RGameSocketType type)
        {
            if(!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    Connected = false;

                    DebugHelper.Log("Connect Error");

                    if (ErrorEvent != null)
                        ErrorEvent();
                }
            }else
            {
                if ((mSocketConn != null && mSocketConn.State == enSocketState.ConnectError && type == RGameSocketType.TCP) || 
                    (mUdpServer != null && mUdpServer.State == enSocketState.ConnectError &&  type == RGameSocketType.UDP))
                {
                    Connected = false;

                    DebugHelper.Log("Connect Error");

                    if (ErrorEvent != null)
                        ErrorEvent();
                }
            }
          
        }

        public void OnConnectFailed(RGameSocketType type)
        {
            if(!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    Connected = false;

                    if (ConnectFailedEvent != null)
                        ConnectFailedEvent();
                }
            }
        }

        public void OnException(System.Net.Sockets.SocketException se, RGameSocketType type)
        {
            if(!bUdp)
            {
                if(type == RGameSocketType.TCP)
                {
                    DebugHelper.LogError("OnException" + se.Message);
                    mSocketConn.Disconnect();
                }
            }else
            {
                if ((mSocketConn != null && mSocketConn.State == enSocketState.ConnectError && type == RGameSocketType.TCP))
                {
                    DebugHelper.LogError(se.Message);
                    mSocketConn.Disconnect();
                }
            }
           
        }
    }
}
