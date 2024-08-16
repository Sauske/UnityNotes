//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Framework
{
    public enum EConnectionType
    {
        EAccount_Connection,
        EZone_Connection,
        EDir_Connection,
        EVer_Connection,
        EPvp_Connection,
    }

    public struct ReqMessageItem
    {
        public NetworkStream mStream;
        public DateTime mTime;
        public TcpClientConnection mTcpClient;
        public DateTime msgReqTime;
        public CSReqMsg msg;
    }

    public struct ResMessageItem
    {
        public EConnectionType connType;
        public DateTime enQueueTimeStamp;
        public DateTime headTime;
        public CSResMsg msg;
        public object deserializedObj;
    }

    public class NavtiveNetwork
    {
        public DateTime _pingTime = DateTime.Now;
        private Dictionary<EConnectionType, TcpClientConnection> mClientDic;
        private List<TcpClientConnection> mClientList;

        public NavtiveNetwork()
        {
            mClientDic = new Dictionary<EConnectionType, TcpClientConnection>();
            mClientList = new List<TcpClientConnection>();
        }

        public void CreateConnectionByHost(EConnectionType connType, string host, int port)
        {
            IPHostEntry hostEntity = Dns.GetHostEntry(host);
            IPAddress ipAddr = hostEntity.AddressList[0];

            CreateConnection(connType, ipAddr.ToString(), port);
        }

        public void CreateConnection(EConnectionType eType, CNetManager.ServerInfo svrInfo)
        {
            CreateConnection(eType, svrInfo.ip, svrInfo.port, svrInfo.host);
        }

        /* Create A new Connection to a Server */
        public void CreateConnection(EConnectionType eType, string svrIpAddr, int svrPort, string svrName = "")
        {
            TcpClientConnection conn = null;
            if (!mClientDic.ContainsKey(eType))
            {
                conn = new TcpClientConnection();
                conn.tcpClient = new TcpClient();

                conn.mConnectionType = eType;
                mClientDic.Add(eType, conn);
                mClientList.Add(conn);
            }
            else
            {
                conn = mClientDic[eType];
            }

            conn.svrIpAddr = svrIpAddr;
            conn.svrPort = svrPort;
            conn.host = svrName;

            conn.ConnectServer();
        }

        public void CloseConnection(EConnectionType type)
        {
            TcpClientConnection connection;
            if (mClientDic.TryGetValue(type, out connection))
            {
                connection.CloseConnection();
                mClientDic.Remove(type);
                mClientList.Remove(connection);
            }
        }

        public TcpClientConnection GetConnection(EConnectionType connType)
        {
            TcpClientConnection tcpConn = null;
            if (mClientDic.TryGetValue(connType, out tcpConn))
            {
                return tcpConn;
            }
            return tcpConn;
        }

        public bool IsConnected(EConnectionType connType)
        {
            TcpClientConnection connection;
            if (mClientDic.TryGetValue(connType, out connection))
            {
                return connection.IsConnected();
            }
            return false;
        }

        public DateTime GetCurrentSvrTime(EConnectionType connType)
        {
            TcpClientConnection connection;
            if (mClientDic.TryGetValue(connType, out connection))
            {
                return connection.CurSvrTime();
            }

            return DateTime.Now;
        }

        public int GetNetworkDelay(EConnectionType connType)
        {
            TcpClientConnection connection;
            if (mClientDic.TryGetValue(connType, out connection))
            {
                return connection.NetDelay;
            }
            return 0;
        }

        public void OnNetWorkChange(NetworkReachability oldState, NetworkReachability newState)
        {
            for (int i = 0; i < mClientList.Count; i++)
            {
                mClientList[i].NetworkChange(oldState, newState);
            }
        }

        public void Update()
        {
            int count = mClientList.Count;
            for (int i = 0; i < count; i++)
            {
                mClientList[i].UpdateHeart();
                mClientList[i].UpdateNetState();

                mClientList[i].TriggerEvent();
            }
        }

        public void Close()
        {
            int count = mClientList.Count;
            for (int i = 0; i < count; i++)
            {
                mClientList[i].CloseConnection();
            }

            mClientList.Clear();
            mClientDic.Clear();          
        }


        public void StartHeartBeat(EConnectionType connType)
        {
            TcpClientConnection tcpConn = null;
            if (mClientDic.TryGetValue(connType, out tcpConn))
            {
                tcpConn.mNeedHeartBeat = true;
            }
        }

        void UpdateNetState()
        {
            for (int i = mClientList.Count - 1; i >= 0; i--)
            {
                TcpClientConnection connection = mClientList[i];
                if (connection.tcpClient == null)
                {
                    continue;
                }
                connection.UpdateNetState();
            }
        }

        void UpdateHeart()
        {
            for (int i = 0; i < mClientList.Count; i++)
            {
                TcpClientConnection connection = mClientList[i];
                connection.UpdateHeart();
            }
        }



        public void SendMsg(ProtoBuf.IExtensible msg, int iCmdId, EConnectionType eType)
        {
            TcpClientConnection connection;
            if (!mClientDic.TryGetValue(eType, out connection))
            {
                return;
            }
            if (!connection.tcpClient.Connected)
            {
                return;
            }

            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (short)iCmdId;   
            msgReq.msg = msg;

            ReqMessageItem newMsgItem = new ReqMessageItem();
            newMsgItem.mStream = connection.tcpClient.GetStream();
            newMsgItem.msg = msgReq;
         //   newMsgItem.msgReqTime = connection.curSvrTime();
            newMsgItem.mTcpClient = connection;
            UpdateNetState();
            lock (connection.sendMsgQueue)
            {
                connection.sendMsgQueue.Enqueue(newMsgItem);
            }
        }

        public void SendMsg(EConnectionType eType,short iCmdId,string content)
        {
            TcpClientConnection connection;
            if (!mClientDic.TryGetValue(eType, out connection))
            {
                return;
            }
            if (!connection.tcpClient.Connected)
            {
                return;
            }

            byte[] conBytes = new byte[50];
            //System.Text.Encoding.UTF8.GetBytes(content);

            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.separator = 0x38AF;

           
            msgReq.head.com_len = conBytes.Length;
            msgReq.msgBuffer = conBytes;

            // msgReq.head.type = (SByte)1;
            // msgReq.head.cmd_id = (short)iCmdId;
            // msgReq.head.checksum = "checksum";
            // msgReq.head.checksum_len = System.Text.Encoding.UTF8.GetByteCount("checksum");
            // Debug.Log("checksum_len:" + msgReq.head.checksum_len);

            // msgReq.head.data_len = conBytes.Length;


            ReqMessageItem newMsgItem = new ReqMessageItem();
            newMsgItem.mStream = connection.tcpClient.GetStream();
            newMsgItem.msg = msgReq;
            newMsgItem.mTcpClient = connection;
            UpdateNetState();
            lock (connection.sendMsgQueue)
            {
                connection.sendMsgQueue.Enqueue(newMsgItem);
            }
        }
    }
}