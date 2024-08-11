using System;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

namespace UMI.Net
{

    public enum EConnectionStatus
    {
        ENone = 0,
        EConnecting,
        EConnected,
        EDisConnected,
        EConnectFailed,
        EReTry,
    }

    public class TcpClientConnection
    {
        public EConnection_Type mConnectionType;
        public TcpClient tcpConn;
        public int svrPort;
        public string svrIpAddr;
        public string svrName;
        private int retryTimes = 3;

        private readonly int TcpClientReceiveBufferSize = 1024 * 1024 * 2;
        private readonly int TcpClientReceiveTimeout = 10000;

        private readonly int TcpClientSendBufferSize = 1024 * 256;
        private readonly int TcpClientSendTimeout = 10000;

        public int netDelayMs = 0;

        public float HeartBeatTime = 3.0f;
        public float m_heartBeatTimeCount = 0;
        public int m_heartSeq = 0;
        public DateTime helloSendTime = new DateTime();
        public float fAdjustTimeTick = 0;

        private EConnectionStatus m_NetState = EConnectionStatus.ENone;
        public DateTime SvrTime = DateTime.Now;
        protected DateTime m_syncTime;
        private bool m_hasSyncTime = false;
        public void SyncTime(uint sec, uint usec)
        {
            if (!m_hasSyncTime)
            {
                SvrTime = CUility.ServerTimeToLocalTime(sec, usec + (uint)netDelayMs * 1000 / 2);
                m_hasSyncTime = true;
                m_syncTime = DateTime.Now;
            }
        }

        public void ForceSynsTime(CSResMsg resMsg)
        {
            Stream stream = new MemoryStream(resMsg.MsgContent);
            CSRTTAdjustNotify adjustNotify = ProtoBuf.Serializer.Deserialize<CSRTTAdjustNotify>(stream);
            m_hasSyncTime = false;
            SyncTime(resMsg.head.tv_sec, resMsg.head.tv_usec);
        }

        protected DateTime m_curSvrTime = DateTime.Now;
        public DateTime curSvrTime()
        {
            TimeSpan ts = DateTime.Now - m_syncTime;
            m_curSvrTime = SvrTime + ts;

            return m_curSvrTime;
        }

        public EConnectionStatus NetState
        {
            get { return m_NetState; }
            set { m_NetState = value; }
        }
        public int NetDelayInMs
        {
            get { return netDelayMs; }
            set { netDelayMs = value; }
        }
        private void SetConnectionParam()
        {
            tcpConn.NoDelay = true;
            tcpConn.ReceiveBufferSize = TcpClientReceiveBufferSize;
            tcpConn.ReceiveTimeout = TcpClientReceiveTimeout;
            tcpConn.SendBufferSize = TcpClientSendBufferSize;
            tcpConn.SendTimeout = TcpClientSendTimeout;
        }

        public void ConnectResultCallBack(IAsyncResult asyncresult)
        {
            if (tcpConn == null || !tcpConn.Connected)
            {
                Debug.LogError("Connect to server failed");
                NetState = EConnectionStatus.EReTry;
            }
            else
            {
                SetConnectionParam();
            }
            tcpConn.EndConnect(asyncresult);
        }
        public bool IsConnected()
        {
            return (tcpConn != null && tcpConn.Connected);
        }
        private IAsyncResult connHandle;

        public void ConnectServer()
        {
            CloseConnection();
            if (tcpConn == null)
            {
                tcpConn = new TcpClient();
            }

            connHandle = tcpConn.BeginConnect(svrIpAddr, svrPort, new AsyncCallback(ConnectResultCallBack), this);
            NetState = EConnectionStatus.EConnecting;
        }

        private int seq = 0;

        public void CloseConnection()
        {
            if (tcpConn != null && tcpConn.Connected)
            {
                tcpConn.GetStream().Close();
                tcpConn.Close();
            }
            tcpConn = null;
        }

        public ENetStatusEvent GetNetStatusEvent()
        {
            switch (mConnectionType)
            {
                case EConnection_Type.EAccount_Connection:
                    return ENetStatusEvent.AccountConnectStatus;
                case EConnection_Type.EDir_Connection:
                    return ENetStatusEvent.DirConnectStatus;
                case EConnection_Type.EPvp_Connection:
                    return ENetStatusEvent.PvpConnectStatus;
                case EConnection_Type.EVer_Connection:
                    return ENetStatusEvent.VerConnectStatus;
                case EConnection_Type.EZone_Connection:
                    return ENetStatusEvent.ZoneConnectStatus;
                default:
                    Debug.LogError("Error ConnectType");
                    break;
            }

            return ENetStatusEvent.ZoneConnectStatus;
        }

        public void OnNetWorkChange(NetworkReachability oldState, NetworkReachability newState)
        {
            if (NetState != EConnectionStatus.EConnected)
            {
                CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, EConnectionStatus.EDisConnected));
            }
            CloseConnection();
        }

        public void UpdateNetState()
        {
            TimeSpan ts = DateTime.Now - m_syncTime;
            m_curSvrTime = SvrTime + ts;
            //   Debug.Log("Log TimeSpan ts=:" + ts.ToString() + "m_curSvrTime : " + m_curSvrTime.ToShortTimeString());
            if (EConnectionStatus.EConnecting == NetState
                && tcpConn.Connected)
            {
                NetState = EConnectionStatus.EConnected;
                CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, NetState));
            }
            else if (NetState == EConnectionStatus.EConnected && !tcpConn.Connected)
            {
                NetState = EConnectionStatus.EDisConnected;
                CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, NetState));
            }
            else if (NetState == EConnectionStatus.EReTry)
            {
                if (retryTimes > 0)
                {
                    CloseConnection();
                    ConnectServer();
                    retryTimes--;
                }
                else
                {
                    CloseConnection();
                    NetState = EConnectionStatus.EConnectFailed;
                    CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, NetState));
                    UnityEngine.Debug.Log("Connect failed");
                }
            }
        }
    }
}
