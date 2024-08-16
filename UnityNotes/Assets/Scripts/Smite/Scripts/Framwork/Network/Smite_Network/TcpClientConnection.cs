//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace Framework
{
    public enum EConnectionStatus
    {
        ENone = 0,
        EConnecing,
        EConnected,
        EDisConnected,
        EConnectFailed,
        EReTry,
        EDestroy,
    }

    public class TcpClientConnection
    {
        private readonly int RES_BUFFER_SIZE = 1024 * 1024 * 2;
        private readonly int RES_TIMEOUT = 5000;

        private readonly int SEND_BUFFER_SIZE = 1024 * 256;
        private readonly int SEND_TIMEOUT = 5000;

        public EConnectionType mConnectionType;
        public TcpClient tcpClient;
        public int svrPort;
        public string svrIpAddr;
        public string host;

        public Queue<ResMessageItem> resMsgQueue = new Queue<ResMessageItem>();
        public Queue<ReqMessageItem> sendMsgQueue = new Queue<ReqMessageItem>();


        public float _heartBeatTime = 10.0f;
        public bool mNeedHeartBeat = false;
        public float _heartBeatTimeCount = 0;
        public int _heartSeq = 0;
        public DateTime helloSendTime = new DateTime();
        public Action helloCallback;

        private int retryTimes = 3;
        private DateTime _curSvrTime = DateTime.Now;

        private EConnectionStatus _netState = EConnectionStatus.ENone;
        private int _netDelayMs = 0;

        protected TcpSendThread _sendThread;
        protected TcpReceiveThread _resThread;

        public EConnectionStatus NetState { get { return _netState; } set { _netState = value; } }
        public int NetDelay { get { return _netDelayMs; } }
        
        /// <summary>
        /// 链接网络回调
        /// </summary>
        /// <param name="asyncresult"></param>
        public void ResultCallBack(IAsyncResult asyncresult)
        {
            TcpClientConnection connection = asyncresult.AsyncState as TcpClientConnection;
            if (connection.tcpClient == null || !connection.tcpClient.Connected)
            {
                DebugHelper.LogError("Connect to server failed");
                _netState = EConnectionStatus.EReTry;
            }
            else
            {
                DebugHelper.Log("Connect to server sucess");
                SetParam();
            }
            connection.tcpClient.EndConnect(asyncresult);
        }

        private void SetParam()
        {
            tcpClient.NoDelay = true;
            tcpClient.ReceiveBufferSize = RES_BUFFER_SIZE;
            tcpClient.ReceiveTimeout = RES_TIMEOUT;
            tcpClient.SendBufferSize = SEND_BUFFER_SIZE;
            tcpClient.SendTimeout = SEND_TIMEOUT;

            _sendThread = new TcpSendThread(sendMsgQueue, tcpClient.GetStream());
            _sendThread.Run();

            _resThread = new TcpReceiveThread(this);
            _resThread.Run();
        }

        /// <summary>
        /// 判断网络是否是链接状态
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return (tcpClient != null && tcpClient.Connected);
        }

        /// <summary>
        /// 链接网络
        /// </summary>
        public void ConnectServer()
        {
            CloseConnection();

            if (tcpClient == null)
            {
                tcpClient = new TcpClient();
            }

            tcpClient.NoDelay = true;
            tcpClient.BeginConnect(svrIpAddr, svrPort, new AsyncCallback(ResultCallBack), this);

            Debug.Log("connect to Server ip = " + svrIpAddr + " port = " + svrPort.ToString());
            _netState = EConnectionStatus.EConnecing;
        }

        /// <summary>
        /// 关闭网络
        /// </summary>
        public void CloseConnection()
        {
            if (_resThread != null)
            {
                _resThread.SetOver();
                _resThread.WaitTermination();
                _resThread = null;
            }
            if (_sendThread != null)
            {
                _sendThread.SetOver();
                _sendThread.WaitTermination();
                _sendThread = null;
            }

            sendMsgQueue.Clear();
            resMsgQueue.Clear();

            if (tcpClient != null && tcpClient.Connected)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
            }

            mNeedHeartBeat = false;

            DebugHelper.Log("close connection");
            tcpClient = null;
        }

        /// <summary>
        /// 根据host链接网络
        /// </summary>
        public void ConnectServerWithHost()
        {
            if (string.IsNullOrEmpty(host)) return;
            IPHostEntry hostEntity = Dns.GetHostEntry(host);
            IPAddress ipAddr = hostEntity.AddressList[0];
            CloseConnection();

            if (tcpClient == null)
            {
                tcpClient = new TcpClient();
            }
            tcpClient.NoDelay = true;
            tcpClient.BeginConnect(ipAddr.ToString(), svrPort, new AsyncCallback(ResultCallBack), this);
            DebugHelper.Log("connect to Server with host ip =" + ipAddr.ToString() + " port = " + svrPort.ToString());
            _netState = EConnectionStatus.EConnecing;
        }

        /// <summary>
        /// 网络改变
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        public void NetworkChange(NetworkReachability oldState,NetworkReachability newState)
        {
            if (_netState == EConnectionStatus.EConnected)
            {
        //         CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, EConnectionStatus.EDisConnected));
        
            }
            CloseConnection();
        }

        public void UpdateNetState()
        {
          //  TimeSpan ts = DateTime.Now - m_syncTime;
          //  m_curSvrTime = SvrTime + ts;
            //   Debug.Log("Log TimeSpan ts=:" + ts.ToString() + "m_curSvrTime : " + m_curSvrTime.ToShortTimeString());
            if (EConnectionStatus.EConnecing == NetState
                && tcpClient.Connected)
            {
                NetState = EConnectionStatus.EConnected;
              //  CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, NetState));
            }
            else if (NetState == EConnectionStatus.EConnected && !tcpClient.Connected)
            {
                NetState = EConnectionStatus.EDisConnected;
             //   CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, NetState));
            }
            else if (NetState == EConnectionStatus.EReTry)
            {
                if (retryTimes > 0)
                {
                    CloseConnection();
                    if (string.IsNullOrEmpty(host))
                    {
                        ConnectServer();
                    }
                    else
                    {
                        ConnectServerWithHost();
                    }
                    retryTimes--;
                }
                else
                {
                    CloseConnection();
                    NetState = EConnectionStatus.EConnectFailed;
                  //  CEventSys.Instance.TriggerEvent(new CNetStatusEvent(GetNetStatusEvent(), mConnectionType, NetState));
                    UnityEngine.Debug.Log("Connect failed");
                }
            }
        }
              
        /// <summary>
        /// 心跳
        /// </summary>       
        public void UpdateHeart()
        {
            _heartBeatTimeCount += Time.deltaTime;
            if (_heartBeatTimeCount > _heartBeatTime)
            {
                if (IsConnected() && mNeedHeartBeat)
                {
                    //Debug.Log("HelloSend at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    //SendMsg(null, (int)CS_CMD_ID.CS_CMD_REQ_PING, 3, connection.mConnectionType);
                    helloSendTime = DateTime.Now;
                    _heartBeatTimeCount -= _heartBeatTime;
                    _heartSeq++;

                    ReqPing(0, _heartSeq);
                }
            }          
        }
        private void SendPing(ProtoBuf.IExtensible msg, int cmd, int crptType)
        {
            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (short)cmd;
            //        msgReq.head.crypt_type = (UInt32)cryptType;
            msgReq.msg = msg;

            ReqMessageItem newMsgItem = new ReqMessageItem();
            newMsgItem.mStream = tcpClient.GetStream();
            newMsgItem.msg = msgReq;
            //   newMsgItem.enQueueTimeStamp = curSvrTime();
            newMsgItem.mTcpClient = this;
            lock (sendMsgQueue)
            {
                //Debug.Log("Enqueue at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                sendMsgQueue.Enqueue(newMsgItem);
            }
        }

        /// static CSPingReq s_PingReq = new CSPingReq();
        public void ReqPing(int backServer,int nSeq)
        {
            //s_PingReq.back_server = back_server;
            //s_PingReq.seq = nSeq;
            //SendPing(s_PingReq, (int)GameSvrCmd.CS_MSGID_HEARTBEAT_REQ, 1);
        }

        void OnResHello(ResMessageItem item)
        {
        //    EConnectionType type = item.connType;
            TimeSpan timeSpan = DateTime.Now - helloSendTime;

            _netDelayMs = (int)timeSpan.TotalMilliseconds;
            if (helloCallback != null)
            {
                helloCallback();
            }
        }

        public void EnqueueRespondMsg(ResMessageItem item)
        {
            if (item.msg.head.cmd_id == (uint)CMD_ID.CS_CMD_REQ_PING)
            {
                OnResHello(item);    //心跳
                return;
            }
            lock (resMsgQueue)
            {
                resMsgQueue.Enqueue(item);
            }
        }


        public DateTime CurSvrTime()
        {            
            return _curSvrTime;
        }
        
        public void TriggerEvent()
        {
            while (resMsgQueue.Count > 0)
            {
                ResMessageItem item = resMsgQueue.Dequeue();

                NetEvent evt = new NetEvent((int)item.msg.head.cmd_id,item.msg,item.connType);
                CNetManager.GetInstance().TriggerEvent(evt);
            }
        }
    }
}