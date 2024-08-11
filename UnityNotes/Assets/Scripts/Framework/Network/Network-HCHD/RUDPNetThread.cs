using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace UMI.Net
{
    public class RUDPNetThread : NetThread
    {
        protected RUDPClient m_rudpClient;
        protected Queue<CSReqMsg> m_sendMsgQueue;
        protected const int MaxPacketSize = 1024 * 256;
        protected byte[] rudpBuff = new byte[MaxPacketSize];
        private int m_nIp;
        protected int m_localPort;
        protected int m_svrPort;
        public void SetConnectInfo(int ip, int localPort, int svrPort)
        {
            m_nIp = ip;
            m_localPort = localPort;
            m_svrPort = svrPort;
        }

        public RUDPNetThread(RUDPClient client, Queue<CSReqMsg> sendMsgQueue)
        {
            m_rudpClient = client;
            m_sendMsgQueue = sendMsgQueue;
        }

        public EConnectionStatus m_eConnectionStatus;
        protected override void Main()
        {
            RUDP_API.CreateSocket(m_localPort);
            RUDP_API.ConnectWithIp(m_nIp, m_svrPort, m_localPort);
            while (!IsOver())
            {
                m_eConnectionStatus = (EConnectionStatus)RUDP_API.GetConnectState();
                RUDP_API.Process();
                ProcessReceive();
                ProcessSend();
            }
            RUDP_API.DisConnect();
            RUDP_API.CloseSocket();
        }

        protected void ProcessSend()
        {
            bool hasSendSucess = false;
            while (m_sendMsgQueue.Count > 0)
            {
                CSReqMsg reqItem = null;
                lock (m_sendMsgQueue)
                {
                    reqItem = m_sendMsgQueue.Dequeue();
                }
                if (reqItem != null)
                {
                    if (reqItem.head.cmd_id == (uint)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_PING_REQ)
                    {
                        CSPingReq req = reqItem.msg as CSPingReq;
                        if (req.back_server == 0)
                        {
                            m_rudpClient.helloSendTime = DateTime.Now;
                        }
                    }

                    CommError.Type ret = CommError.Type.COMM_NO_ERROR;
                    int pack_len = 0;
                    uint sec;
                    uint usec;
                    sec = CUility.LocalTimeToServerTime(m_rudpClient.curSvrTime(), out usec);
                    reqItem.head.tv_sec = (uint)sec;
                    reqItem.head.tv_usec = (uint)usec;
                    ret = reqItem.pack(ref rudpBuff, rudpBuff.Length, ref pack_len);
                    if (ret != CommError.Type.COMM_NO_ERROR || pack_len == 0)
                    {
                        continue;
                    }
                    //m_rudpClient.EnqueueLogStr(string.Format("send msg {0} size = {1}", reqItem.head.cmd_id.ToString(), pack_len.ToString()));
                    if (RUDP_API.Send(rudpBuff, pack_len) == 0)
                        hasSendSucess = true;
                }
            }

            if (hasSendSucess)
            {
                RUDP_API.EndSend();
            }
        }

        protected void ProcessReceive()
        {
            while (RUDP_API.Readable())
            {
                int nReciveCount = RUDP_API.Receive(rudpBuff);
                if (nReciveCount > 0)
                {
                    CommError.Type ret = CommError.Type.COMM_NO_ERROR;
                    CSResMsg resMsg = new CSResMsg();
                    int usedSize = 0;
                    ret = resMsg.unpack(ref rudpBuff, MaxPacketSize, ref usedSize);
                    if (ret != CommError.Type.COMM_NO_ERROR)
                    {
                        Debug.LogError("un pack buff error!!");
                        return;
                    }

                    //强制对时
                    if (resMsg.head.cmd_id == (int)BattleSvrCmd.CS_SUB_MSGID_BATTLESVR_RTT_NOTIFY)
                    {
                        m_rudpClient.ForceSynsTime(resMsg);
                    }
                    else
                    {
                        m_rudpClient.SyncTime(resMsg.head.tv_sec, resMsg.head.tv_usec);
                        ResMessageItem item = new ResMessageItem();
                        item.msg = resMsg;
                        item.headTime = CUility.ServerTimeToLocalTime(resMsg.head.tv_sec, resMsg.head.tv_usec);

                        m_rudpClient.EnqueueRespondMsg(item);
                        //m_rudpClient.EnqueueLogStr(string.Format("receive msg {0} size = {1}", resMsg.head.cmd_id.ToString(), nReciveCount.ToString()));
                    }
                }
            }
        }
    }
}
