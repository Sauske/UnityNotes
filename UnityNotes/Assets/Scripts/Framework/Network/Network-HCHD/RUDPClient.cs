using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace UMI.Net
{
    public class RUDPClient
    {
        private EConnectionStatus m_eConnStatus;
        private Queue<ResMessageItem> resMsgQueue;
        private Queue<CSReqMsg> sendMsgQueue;
        public static int CLIENT_UDP_PORT = 2030;
        public DateTime SvrTime = DateTime.Now;
        protected DateTime m_syncTime;
        private bool m_hasSyncTime = false;
        private RUDPNetThread m_rudpThread;
        private int mPort = 2030;
        public void SyncTime(uint sec, uint usec)
        {
            if (!m_hasSyncTime)
            {
                SvrTime = CUility.ServerTimeToLocalTime(sec, usec + (uint)netDelays * 1000 / 2);
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

        public void Init()
        {
#if INNER_VER
#if UNITY_EDITOR
        string strLogPath = Path.Combine(Application.dataPath, "Log"); 
#else
        string strLogPath = Path.Combine(Application.persistentDataPath, "Log");
#endif
        if (!Directory.Exists(strLogPath))
        {
            Directory.CreateDirectory(strLogPath);
        }
        strLogPath += "/";
        Debug.Log(strLogPath);
        byte[] szPathBuff = System.Text.Encoding.ASCII.GetBytes(strLogPath);
        RUDP_API.SetLogPath(szPathBuff, szPathBuff.Length);
#endif
            //m_lastInternetReachability = Application.internetReachability;
            resMsgQueue = new Queue<ResMessageItem>();
            sendMsgQueue = new Queue<CSReqMsg>();
            //m_rudpThread.Run();
        }

        public void Connect(string ip, int port)
        {
            DisConnect();

            byte[] IPArr = IPAddress.Parse(ip).GetAddressBytes();
            int nIP = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(IPArr, 0));

            mPort = GetUsablePort();
            m_rudpThread = new RUDPNetThread(this, sendMsgQueue);
            m_rudpThread.SetConnectInfo(nIP, mPort, port);
            m_rudpThread.Run();
        }
        public int m_heartSeq = 0;
        CSPingReq pingReq = new CSPingReq();
        public void SendPing(int back_server, int seq)
        {
            if (!IsConnected())
            {
                return;
            }

            pingReq.back_server = back_server;
            pingReq.seq = seq;

            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (int)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_REQ;
            msgReq.head.crypt_type = 1;
            msgReq.msg = pingReq;

            lock (sendMsgQueue)
            {
                sendMsgQueue.Enqueue(msgReq);
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

        public void ProcessLog()
        {
            lock (mLogQueue)
            {
                while (mLogQueue.Count > 0)
                {
                    string strLog = mLogQueue.Dequeue();
                    CUility.Log(strLog);
                }
            }
        }

        private float m_fLastSendPingTime;
        private float m_fPingTimeDuration = 2.0f;
        public DateTime helloSendTime;
        public int netDelays;
        protected void ProcessPing()
        {
            if (Time.time - m_fLastSendPingTime > m_fPingTimeDuration)
            {
                m_heartSeq++;
                //m_lastSendPingDT = DateTime.Now;
                m_fLastSendPingTime = Time.time;
                SendPing(0, m_heartSeq);
            }
        }

        public void DisConnect()
        {
            if (m_rudpThread != null)
            {
                m_rudpThread.SetOver();
                m_rudpThread.WaitTermination();
                m_rudpThread = null;
            }
            m_eConnStatus = EConnectionStatus.ENone;
        }

        public void Destroy()
        {
            DisConnect();
            RUDP_API.FinalizeLog();
        }

        public bool IsConnected()
        {
            return m_eConnStatus == EConnectionStatus.EConnected;
        }

        public void SendMsg(ProtoBuf.IExtensible msg, int iCmdId, int cryptType, EConnection_Type eType)
        {
            if (!IsConnected())
            {
                return;
            }

            uint sec;
            uint usec;
            sec = CUility.LocalTimeToServerTime(System.DateTime.Now, out usec);
            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (UInt32)iCmdId;
            msgReq.head.crypt_type = (UInt32)cryptType;
            msgReq.msg = msg;

            msgReq.head.tv_sec = (uint)sec;
            msgReq.head.tv_usec = (uint)usec;

            lock (sendMsgQueue)
            {
                sendMsgQueue.Enqueue(msgReq);
            }
        }

        public void OnNetWorkChange(NetworkReachability oldState, NetworkReachability newState)
        {
            if (IsConnected() ||
                m_eConnStatus == EConnectionStatus.EConnecting)
            {
                DisConnect();
            }

            CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.PvpConnectStatus, EConnection_Type.EPvp_Connection, EConnectionStatus.EDisConnected));
        }

        public void Update()
        {
            //CheckInternetReachability();
#if PROFILE
        Profiler.BeginSample("Rudp.Update");
#endif
            ProcessRespondMsg();
            UpdateNetState();
            ProcessPing();
            ProcessLog();
#if PROFILE
        Profiler.EndSample();
#endif
        }

        private void UpdateNetState()
        {
            if (m_rudpThread == null)
            {
                RUDP_API.Process();

                SetConnectState(EConnectionStatus.ENone);
                return;
            }

            EConnectionStatus eConnStatus = m_rudpThread.m_eConnectionStatus;
            SetConnectState(eConnStatus);
        }

        private void SetConnectState(EConnectionStatus eConnStatus)
        {
            //EConnectionStatus eConnStatus = m_rudpThread.m_eConnectionStatus;
            if (m_eConnStatus != eConnStatus)
            {
                switch (eConnStatus)
                {
                    case EConnectionStatus.EConnected:
                        CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.PvpConnectStatus, EConnection_Type.EPvp_Connection, eConnStatus));
                        m_eConnStatus = eConnStatus;
                        break;
                    case EConnectionStatus.ENone:
                        if (m_eConnStatus == EConnectionStatus.EConnected)
                        {
                            m_eConnStatus = EConnectionStatus.EDisConnected;
                            CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.PvpConnectStatus, EConnection_Type.EPvp_Connection, m_eConnStatus));
                            DisConnect();
                        }
                        break;
                    default:
                        m_eConnStatus = eConnStatus;
                        break;
                }
            }
        }

        private void ProcessRespondMsg()
        {
            lock (resMsgQueue)
            {
                while (resMsgQueue.Count > 0)
                {
                    ResMessageItem item = resMsgQueue.Dequeue();
                    CNetSysEvent netEvt = new CNetSysEvent((int)item.msg.head.cmd_id, item.msg);
                    netEvt.mConnType = EConnection_Type.EPvp_Connection;
                    CEventSys.Instance.TriggerEvent(netEvt);
                }
            }
        }

        public void EnqueueRespondMsg(ResMessageItem item)
        {
            if (item.msg.head.cmd_id == (uint)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_RES)
            {
                SendPing(1, m_heartSeq);
                TimeSpan ts = DateTime.Now - helloSendTime;
                netDelays = (netDelays + ts.Milliseconds) / 2;
                return;
            }

            lock (resMsgQueue)
            {
                resMsgQueue.Enqueue(item);
            }
        }

        public static int GetUsablePort()
        {
            using (Socket sock = new Socket(AddressFamily.InterNetwork,
                             SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0)); // Pass 0 here.

                return ((IPEndPoint)sock.LocalEndPoint).Port;
            }
        }
    }
}
