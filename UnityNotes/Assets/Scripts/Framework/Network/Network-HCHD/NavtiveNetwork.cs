using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace UMI.Net
{

    public class ReqMessageItem
    {
        public NetworkStream connStream;
        public DateTime enQueueTimeStamp;
        public TcpClientConnection conn;
        public DateTime msgReqTime;
        public CSReqMsg msg;
    }

    public class ResMessageItem
    {
        public EConnection_Type connType;
        public DateTime enQueueTimeStamp;
        public DateTime headTime;
        public CSResMsg msg;
    }

    public enum EConnection_Type
    {
        EAccount_Connection,
        EZone_Connection,
        EDir_Connection,
        EVer_Connection,
        EPvp_Connection,
    }

    public class NavtiveNetwork
    {
        public DateTime m_PingTime = DateTime.Now;
        private Queue<ResMessageItem> resMsgQueue;
        private Queue<ReqMessageItem> sendMsgQueue;
        private Dictionary<int, TcpClientConnection> mClientDict;
        private List<TcpClientConnection> mClientList;

        private ReceiverThread m_receiver;
        private SenderThread m_sender;

        public int SendMsgQueueLength
        {
            get { return sendMsgQueue.Count; }
        }

        public NavtiveNetwork()
        {
            mClientDict = new Dictionary<int, TcpClientConnection>();
            mClientList = new List<TcpClientConnection>();

            resMsgQueue = new Queue<ResMessageItem>();
            sendMsgQueue = new Queue<ReqMessageItem>();

            StartSendThread();
            StartReceiveThread();
        }

        /* Create A new Connection to a Server */
        public void CreateConnection(EConnection_Type eType, string svrIpAddr, int svrPort, string svrName = "")
        {
            TcpClientConnection conn = null;
            if (!mClientDict.ContainsKey((int)eType))
            {
                conn = new TcpClientConnection();
                conn.tcpConn = new TcpClient();

                conn.mConnectionType = eType;
                mClientDict.Add((int)eType, conn);
                mClientList.Add(conn);
            }
            else
            {
                conn = mClientDict[(int)eType];
            }

            conn.svrIpAddr = svrIpAddr;
            conn.svrPort = svrPort;
            conn.svrName = svrName;

            conn.ConnectServer();

            //return mClientDict.Count - 1;
        }

        public void SendMsg(ProtoBuf.IExtensible msg, int iCmdId, int cryptType, EConnection_Type eType)
        {
            TcpClientConnection connection;
            if (!mClientDict.TryGetValue((int)eType, out connection))
            {
                return;
            }
            if (!connection.tcpConn.Connected)
            {
                return;
            }

            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (UInt32)iCmdId;
            msgReq.head.crypt_type = (UInt32)cryptType;
            msgReq.msg = msg;

            ReqMessageItem newMsgItem = new ReqMessageItem();
            newMsgItem.connStream = connection.tcpConn.GetStream();
            newMsgItem.msg = msgReq;
            newMsgItem.enQueueTimeStamp = connection.curSvrTime();
            newMsgItem.conn = connection;
            UpdateNetState();
            lock (sendMsgQueue)
            {
                //Debug.Log("Enqueue at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                sendMsgQueue.Enqueue(newMsgItem);
            }
        }

        public void SendPing(ProtoBuf.IExtensible msg, int iCmdId, int cryptType, EConnection_Type eType)
        {
            TcpClientConnection connection;
            if (!mClientDict.TryGetValue((int)eType, out connection))
            {
                return;
            }
            if (!connection.tcpConn.Connected)
            {
                return;
            }

            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (UInt32)iCmdId;
            msgReq.head.crypt_type = (UInt32)cryptType;
            msgReq.msg = msg;

            ReqMessageItem newMsgItem = new ReqMessageItem();
            newMsgItem.connStream = connection.tcpConn.GetStream();
            newMsgItem.msg = msgReq;
            newMsgItem.enQueueTimeStamp = connection.curSvrTime();
            newMsgItem.conn = connection;
            lock (sendMsgQueue)
            {
                //Debug.Log("Enqueue at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                sendMsgQueue.Enqueue(newMsgItem);
            }
        }

        public void EnqueueRespondMsg(ResMessageItem item)
        {
            //Debug.Log(string.Format("CNetSys: RespondMsg {0}", item.msg.head.crypt_type));
            //if (item.msg.head.cmd_id == (int)CS_CMD_ID.CS_CMD_RES_PING)
            //{
            //    onHelloResponse(item);
            //    return;
            //}
            //if (item.msg.head.cmd_id == (uint)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_RES)
            if (item.msg.head.cmd_id == (uint)GameSvrCmd.CS_MSGID_HEARTBEAT_RES)
            {
                OnPvpHello(item);
                return;
            }

            lock (resMsgQueue)
            {
                resMsgQueue.Enqueue(item);
            }
        }

        protected Queue<string> mLogQueue = new Queue<string>();
        public void EnqueueLogStr(string strLog)
        {
            lock (mLogQueue)
            {
                mLogQueue.Enqueue(strLog);
            }
        }

        public void OnNetWorkChange(NetworkReachability oldState, NetworkReachability newState)
        {
            for (int i = 0; i < mClientList.Count; i++)
            {
                mClientList[i].OnNetWorkChange(oldState, newState);
            }
        }

        public void ProcessLog()
        {
            lock (mLogQueue)
            {
                while (mLogQueue.Count > 0)
                {
                    string strLog = mLogQueue.Dequeue();
                    CLogSys.Log(ELogLevel.Verbose, ELogTag.NetSys, strLog);
                }
            }
        }

        public bool IsConnected(EConnection_Type connType)
        {
            TcpClientConnection connection;
            if (mClientDict.TryGetValue((int)connType, out connection))
            {
                return connection.IsConnected();
            }

            return false;
        }

        public DateTime GetCurrentSvrTime(EConnection_Type connType)
        {
            TcpClientConnection connection;
            if (mClientDict.TryGetValue((int)connType, out connection))
            {
                return connection.curSvrTime();
            }

            return DateTime.Now;
        }

        public void Update()
        {
            UpdateNetState();
            UpdateHeart();
            ProcessLog();
            //  SendHello();
        }

        public void LogicFrameTurn()
        {
            TriggerEvent();
        }

        void UpdateHeart()
        {
            for (int i = 0; i < mClientList.Count; i++)
            {
                TcpClientConnection connection = mClientList[i];
                connection.m_heartBeatTimeCount += Time.deltaTime;
                if (connection.m_heartBeatTimeCount > connection.HeartBeatTime)
                {
                    if (connection.IsConnected() && connection.mConnectionType == EConnection_Type.EPvp_Connection)
                    {
                        //Debug.Log("HelloSend at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        //SendMsg(null, (int)CS_CMD_ID.CS_CMD_REQ_PING, 3, connection.mConnectionType);
                        connection.helloSendTime = DateTime.Now;
                        connection.m_heartBeatTimeCount -= connection.HeartBeatTime;
                        connection.m_heartSeq++;
                        CUility.LogError("send");
                        ReqPing(0, connection.m_heartSeq, connection.mConnectionType);
                    }
                }
            }
        }

        private int seq = 0;
        static CSPingReq s_PingReq = new CSPingReq();
        public void ReqPing(int back_server, int seq, EConnection_Type eType)
        {
            s_PingReq.back_server = back_server;
            s_PingReq.seq = seq;
            //CNetSys.Instance.SendPvpMsg(req, (int)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_REQ);
            //SendPing(s_PingReq, (int)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_REQ, 1, eType);
            SendPing(s_PingReq, (int)GameSvrCmd.CS_MSGID_HEARTBEAT_REQ, 1, eType);
            DateTime dt = DateTime.Now;
            TcpClientConnection connection;
            if (mClientDict.TryGetValue((int)eType, out connection))
            {
                dt = connection.curSvrTime();
            }
            //"RecvMsg Id = {0} seq = {1} SvrTime:{2} localTime = {3}"

            //string strLog = string.Format("SendMsg Id = {0} seq = {1} SvrTime:{2} localTime = {3}", (int)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_REQ,
            //    seq,
            //    dt.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //EnqueueLogStr(strLog);
        }


        void onHelloResponse(ResMessageItem item)
        {
            EConnection_Type type = item.connType;
            TcpClientConnection connection;
            if (mClientDict.TryGetValue((int)type, out connection))
            {
                //Debug.Log("HelloResponse at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                TimeSpan netDelay = DateTime.Now - connection.helloSendTime;
                connection.netDelayMs = (int)netDelay.TotalMilliseconds;
            }
        }
        void OnPvpHello(ResMessageItem item)
        {
            EConnection_Type type = item.connType;
            TcpClientConnection connection;
            mClientDict.TryGetValue((int)type, out connection);
            if (connection != null && type == EConnection_Type.EPvp_Connection)
            {
                TimeSpan pvpNetDelayMs = DateTime.Now - connection.helloSendTime;

                connection.netDelayMs = (int)pvpNetDelayMs.TotalMilliseconds;
                ReqPing(1, connection.m_heartSeq, connection.mConnectionType);
                //    Debug.Log("Pvp net Delay" + connection.netDelayMs);
            }
        }

        public int GetNetworkDelay(EConnection_Type connType)
        {
            TcpClientConnection connection;
            if (mClientDict.TryGetValue((int)connType, out connection))
            {
                return connection.netDelayMs;
            }
            return 0;
        }

        void TriggerEvent()
        {
            lock (resMsgQueue)
            {
                while (resMsgQueue.Count > 0)
                {
                    ResMessageItem item = resMsgQueue.Dequeue();
                    CNetSysEvent netEvt = new CNetSysEvent((int)item.msg.head.cmd_id, item.msg);
                    netEvt.mConnType = item.connType;
                    CEventSys.Instance.TriggerEvent(netEvt);
                }
            }
        }

        void UpdateNetState()
        {
            for (int i = mClientList.Count - 1; i >= 0; i--)
            {
                TcpClientConnection connection = mClientList[i];
                if (connection.tcpConn == null)
                {
                    mClientList.Remove(connection);
                    mClientDict.Remove((int)connection.mConnectionType);
                    continue;
                }
                connection.UpdateNetState();
            }
        }

        public void StopRecvAndSend()
        {
            if (m_sender != null)
                m_sender.SetOver();

            if (m_receiver != null)
                m_receiver.SetOver();
        }

        public void WaitTermination()
        {
            if (m_sender != null)
                m_sender.WaitTermination();
            if (m_receiver != null)
                m_receiver.WaitTermination();
        }

        public void CloseAll()
        {
            for (int i = 0; i < mClientList.Count; i++)
            {
                mClientList[i].CloseConnection();
            }
        }

        protected void StartReceiveThread()
        {
            m_receiver = new ReceiverThread(mClientList, this);
            m_receiver.Run();
        }

        protected void StartSendThread()
        {
            m_sender = new SenderThread(sendMsgQueue, this);
            m_sender.Run();
        }

        public void CloseConnection(EConnection_Type type)
        {
            TcpClientConnection connection;
            if (mClientDict.TryGetValue((int)type, out connection))
            {
                connection.CloseConnection();
                mClientDict.Remove((int)type);
            }
        }

        class ReceiverThread : NetThread
        {
            const uint MaxPacketSize = 1024 * 256;
            const uint MinPacketSize = 20;

            private byte[] m_recBuf;
            private int m_recBufOffset;

            private int mHeadLen = 0;
            private List<TcpClientConnection> mClientList;
            private NavtiveNetwork mConnInstance;
            public int GetHeadLen()
            {
                if (mHeadLen == 0)
                {
                    byte[] Buffer = new byte[128];
                    CommReadBuf unpackBuf = new CommReadBuf(ref Buffer, 128);
                    CSMsgHead head = new CSMsgHead();
                    head.unpack(ref unpackBuf);
                    mHeadLen = unpackBuf.getUsedSize();
                }

                return mHeadLen;
            }

            public ReceiverThread(List<TcpClientConnection> clients, NavtiveNetwork connInstance)
            {
                m_recBuf = new byte[MaxPacketSize];
                m_recBufOffset = 0;
                mConnInstance = connInstance;
                mClientList = clients;
            }

            protected override void Main()
            {
                try
                {
                    while (!IsOver())
                    {
                        CheckNetworkRecv();
                    }
                }
                catch (System.Exception ex)
                {
                    mConnInstance.EnqueueLogStr(ex.ToString());
                }
            }

            protected void CheckNetworkRecv()
            {
                bool packetGot = false;
                try
                {
                    for (int i = 0; i < mClientList.Count; i++)
                    {
                        TcpClientConnection connection = mClientList[i];
                        if (connection != null && connection.IsConnected())
                        {
                            NetworkStream ns = connection.tcpConn.GetStream();

                            if (ns.DataAvailable)
                            {
                                try
                                {
                                    m_recBufOffset += ns.Read(m_recBuf, m_recBufOffset, m_recBuf.Length - m_recBufOffset);
                                }
                                catch (System.Exception ex)
                                {
                                    mConnInstance.EnqueueLogStr("CNetSys:ReadFromStream:" + ex.Message);
                                }
                                //Debug.Log("RecvAPacket at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                ScanPackets(connection);
                                packetGot = true;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    mConnInstance.EnqueueLogStr(ex.ToString());
                }

                if (!packetGot)
                {
                    Thread.Sleep(1);
                }
            }

            protected void ScanPackets(TcpClientConnection tcpConn)
            {
                bool packetFound = false;
                do
                {
                    packetFound = false;
                    if (m_recBufOffset >= MinPacketSize)
                    {
                        CommError.Type ret = CommError.Type.COMM_NO_ERROR;
                        CSResMsg resMsg = new CSResMsg();
                        int usedSize = 0;
                        ret = resMsg.unpack(ref m_recBuf, m_recBufOffset, ref usedSize);
                        if (ret != CommError.Type.COMM_NO_ERROR)
                        {
                            //Debug.Log("NetSys: Scan packet Head error: " + CommError.getErrorString(ret));
                            return;
                        }

                        //强制对时
                        if (resMsg.head.cmd_id == (int)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_RTT_NOTIFY)
                        {
                            //Stream stream = new MemoryStream(resMsg.MsgContent);
                            //CSRTTAdjustNotify backinfo = ProtoBuf.Serializer.Deserialize<CSRTTAdjustNotify>(stream);
                            tcpConn.ForceSynsTime(resMsg);
                            //continue;
                        }
                        else
                        {
                            tcpConn.SyncTime(resMsg.head.tv_sec, resMsg.head.tv_usec);
                            ResMessageItem item = new ResMessageItem();
                            item.msg = resMsg;
                            item.enQueueTimeStamp = tcpConn.curSvrTime();
                            item.connType = tcpConn.mConnectionType;
                            item.headTime = CUility.ServerTimeToLocalTime(resMsg.head.tv_sec, resMsg.head.tv_usec);

                            //if (item.msg.head.cmd_id == (uint)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_RES)
                            //{
                            //    mConnInstance.EnqueueLogStr(string.Format("RecvMsg Id = {0} seq = {1} SvrTime:{2} localTime = {3} delays = {4} HeadTime = {5}", 
                            //        item.msg.head.cmd_id, 
                            //        tcpConn.m_heartSeq, 
                            //        tcpConn.curSvrTime().ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                            //        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            //        tcpConn.netDelayMs,
                            //        item.headTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                            //}
                            mConnInstance.EnqueueRespondMsg(item);
                        }

                        packetFound = true;
                        int offset = (int)resMsg.head.pkg_len;
                        if (m_recBufOffset > offset)
                        {
                            for (int i = offset, j = 0; i < m_recBufOffset; i++, j++)
                            {
                                m_recBuf[j] = m_recBuf[i];
                            }
                            m_recBufOffset -= offset;
                        }
                        else
                            m_recBufOffset = 0;
                    }
                }
                while (packetFound && !IsOver());
            }
        }

        class SenderThread : NetThread
        {
            const int MaxPacketSize = 8192;
            private byte[] buffer = new byte[MaxPacketSize];
            private Queue<ReqMessageItem> mSendMsgQueue = null;
            protected NavtiveNetwork mNavNetWork;

            public SenderThread(Queue<ReqMessageItem> sendMsgQueue, NavtiveNetwork netWork)
            {
                mSendMsgQueue = sendMsgQueue;
                mNavNetWork = netWork;
            }

            protected override void Main()
            {
                try
                {
                    while (!IsOver() || mSendMsgQueue.Count != 0)
                    {
                        ReqMessageItem item = null;

                        /* Search for one Msg to send */
                        lock (mSendMsgQueue)
                        {
                            if (mSendMsgQueue.Count > 0)
                            {
                                item = mSendMsgQueue.Dequeue();
                                //Debug.Log("Dequeue at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            }
                        }

                        if (item != null && item.connStream != null)
                        {
                            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
                            int pack_len = 0;
                            uint sec;
                            uint usec;
                            sec = CUility.LocalTimeToServerTime(item.conn.curSvrTime(), out usec);
                            //Debug.Log(string.Format("sendmsg time;second：{0};mirco sec：{1}", sec, usec));
                            item.msg.head.tv_sec = (uint)sec;
                            item.msg.head.tv_usec = (uint)usec;
                            ret = item.msg.pack(ref buffer, buffer.Length, ref pack_len);
                            if (ret != CommError.Type.COMM_NO_ERROR)
                            {
                                mNavNetWork.EnqueueLogStr("NetSys: sender pack fail!");
                                continue;
                            }

                            //if (item.conn.netDelayMs > 500.0f)
                            //{
                            //Debug.Log(string.Format("SendMsg Id = {0} at:" + item.conn.curSvrTime().ToString("yyyy-MM-dd HH:mm:ss.fff"), item.msg.head.cmd_id));
                            //}

                            if (item.msg.head.cmd_id == (uint)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_REQ)
                            {
                                CSPingReq req = item.msg.msg as CSPingReq;
                                if (req.back_server == 0)
                                {

                                    item.conn.helloSendTime = DateTime.Now;
                                }
                            }

                            if (pack_len == 0)
                            {
                                continue;
                            }

                            try
                            {
                                item.connStream.Write(buffer, 0, pack_len);
                                item.connStream.Flush();
                                //Debug.Log(string.Format("Send msg {0} sucess", item.msg.head.cmd_id));
                            }
                            catch (IOException e)
                            {
                                mNavNetWork.EnqueueLogStr(string.Format("send package size = {0}; command id =", pack_len, item.msg.head.cmd_id));
                                mNavNetWork.EnqueueLogStr("CNetSys:SenderThread, Main: " + e.Message);
                                mNavNetWork.EnqueueLogStr("CNetSys:SenderThread, Main: " + e.StackTrace);
                                mNavNetWork.EnqueueLogStr("CNetSys:SenderThread, Main: " + e.InnerException.Message);
                            }
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    mNavNetWork.EnqueueLogStr(ex.ToString());
                }
            }
        }
    }
}
