using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;

using ProtoBuf;
//using proto.pvp;

namespace Framework
{
    public class UdpBaseMessage
    {
        public UdpBaseMessage(byte[] bytes, IPEndPoint sender)
        {
            this.bytes = bytes;
            this.sender = sender;
        }

        public byte[] bytes;
        public IPEndPoint sender;
    }

    public class UdpBaseServer
    {
        private ISocketHandler iHandler;
        private enSocketState mSocketState = enSocketState.Disconnected;
        public enSocketState State { get { return mSocketState; } }
        
        public void RegHandler(ISocketHandler handler)
        {
            iHandler = handler;
        }

        public static ulong msgNum;
        private Thread reciveThread;
        private Socket socket;
        public Socket Socket
        {
            get
            {
                return socket;
            }
        }
        private string serverIP;
        private int serverPort;
        private UInt32 _gameVersion = 19781212;
        public UInt32 gameVersion
        {
            get { return _gameVersion; }
            set { _gameVersion = value; }
        }

        private EndPoint senderRemote;         

        private Queue<UdpBaseMessage> m_WriteQueue = new Queue<UdpBaseMessage>();
        public void PushWriteMsg(UdpBaseMessage baseMsg)
        {
            lock (m_WriteQueue)
            {
                m_WriteQueue.Enqueue(baseMsg);
            }
        }

        //移交消息到可读队列
        private Queue<UdpBaseMessage> m_ReadQueue = new Queue<UdpBaseMessage>();
        public Queue<UdpBaseMessage> ReadQueue
        {
            get { return m_ReadQueue; }
            set { m_ReadQueue = value; }
        }
        public void WriteQueueToReadQueue()
        {
            lock (m_WriteQueue)
            {
                while (m_WriteQueue.Count > 0)
                {
                    m_ReadQueue.Enqueue(m_WriteQueue.Dequeue());
                }
            }           
        }
        private bool InitServer(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    //为 Socket 设置低级操作模式,不然当客户端关闭的时候，会报错
                    uint IOC_IN = 0x80000000;
                    uint IOC_VENDOR = 0x18000000;
                    uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                    socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                }               
            }
            catch (Exception ex)
            {
                //NGUIDebug.LogToScreen(ex);
                Debug.LogException(ex);
                mSocketState = enSocketState.ConnectError;
                if (iHandler != null)
                {
                    iHandler.OnError(RGameSocketType.UDP);
                }

                return false;
            }            

            Debug.Log("Server, host name is " + Dns.GetHostName());
            Debug.Log("Waiting for server message");

            //发送消息地址
            //IPEndPoint sender = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), serverPort);
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            senderRemote = (EndPoint)sender;

            //reciveThread = new Thread(new ParameterizedThreadStart(ReceiveHandle));
            //reciveThread.Priority = System.Threading.ThreadPriority.Normal;
            //reciveThread.IsBackground = true;//随着主线程而退出
            //reciveThread.Start(new object[] { serverPort });

            reciveThread = new Thread(ReceiveHandle);
            reciveThread.Priority = System.Threading.ThreadPriority.Normal;
            reciveThread.IsBackground = true;//随着主线程而退出
            reciveThread.Start();
            
            return true;
        }

        private void ExitServer()
        {
            reciveThread.Abort();
            socket.Close();
        }

        public bool Connect(string serverIP, int serverPort)
        {
            if(InitServer(serverIP, serverPort))
            {
                mSocketState = enSocketState.Connected;
                if (iHandler != null)
                {
                    iHandler.OnConnect(RGameSocketType.UDP);
                }

                return true;
            }

            mSocketState = enSocketState.ConnectError;
            if (iHandler != null)
            {
                iHandler.OnError(RGameSocketType.UDP);
            }

            return false;
        }
        public void Disconnect(bool ignoreEvent = false)
        {
            ExitServer();
            mSocketState = enSocketState.Disconnected;
            if (!ignoreEvent)
            {
                if (iHandler != null)
                {
                    iHandler.OnDisconnect(RGameSocketType.UDP);
                }
            }
        }

        public bool Reconnect()
        {
            Disconnect(true);
            if (Connect(serverIP, serverPort))
                return true;
            
            return false;
        }

        //private void ReceiveHandle(object arg)
        private void ReceiveHandle()
        {
            //接收消息地址
            int recv;
            byte[] bytes = new byte[1600];
            //object[] objs = arg as object[];
            //IPEndPoint sender = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), (int)objs[0]);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)sender;
            //object[] objs = arg as object[];
            //IPEndPoint sender = new IPEndPoint(IPAddress.Parse(objs[0].ToString()), (int)objs[1]);
            //senderRemote = (EndPoint)sender;

            //发消息，因为不绑定端口，目的是获取自动端口
            byte[] tbytes = new byte[1];
            SendMessage(tbytes, senderRemote);            

            try
            {
                while (reciveThread.IsAlive)
                {
                    recv = socket.ReceiveFrom(bytes, ref remote);
                    //Debug.Log("Message received from " + remote.ToString());

                    if (recv < 8)
                        continue;

                    if (recv > 1024 * 1024)
                    {
                        Debug.Log("Recv Message Length > 1024 * 1024");
                        continue;
                    }

                    byte[] savebytes = new byte[recv];
                    Array.Copy(bytes, savebytes, recv);

                    IPEndPoint savesender = new IPEndPoint(sender.Address, sender.Port);
                    PushWriteMsg(new UdpBaseMessage(savebytes, savesender));

                    //Debug.Log("Reveived Message :" + System.Text.Encoding.ASCII.GetString(savebytes, 0, savebytes.Length));                    
                }
            }
            catch (ThreadAbortException)
            {
                //
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                socket.Close();
            }
        }

        public void SendMessage(byte[] bytes)
        {
            if (bytes.Length > 1024 * 1024)
            {
                Debug.Log("Send Message Length > 1024 * 1024");
                return;
            }

            socket.SendTo(bytes, senderRemote);
        }

        public void SendMessage(byte[] bytes,EndPoint remote)
        {
            if (bytes.Length > 1024 * 1024)
            {
                Debug.Log("Send Message Length > 1024 * 1024");
                return;
            }

            socket.SendTo(bytes, remote);
        }


        public void SendMessage(ulong userID, uint msgID, System.Object data)
        {
            msgNum++;

            //包头
            MemoryStream mshead = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mshead);
            writer.Write((uint)gameVersion);
            writer.Write((ulong)userID);
            writer.Write((ulong)msgNum);
            writer.Write((uint)msgID);

            ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(mshead, data);

            if (mshead.Length > 1024 * 1024)
            {
                msgNum--;
                Debug.Log("Send Message Length > 1024 * 1024");
                return;
            }

            //发送
            socket.SendTo(mshead.GetBuffer(), (int)mshead.Length, System.Net.Sockets.SocketFlags.None, senderRemote);           
        }

        private void SendMsg(ref IExtensible msgBody, ProtoID cmdId)
        {
            /*
            //包头
            MemoryStream mshead = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mshead);
            writer.Write((uint)gameVersion);
            writer.Write((ulong)userID);
            writer.Write((ulong)msgNum);
            writer.Write((uint)msgID);

            ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(mshead, data);

            if (mshead.Length > 1024)
            {
                msgNum--;
                Debug.Log("Send Message Length > 1024");
                return;
            }

            //第一次发送
            socket.SendTo(mshead.GetBuffer(), (int)mshead.Length, System.Net.Sockets.SocketFlags.None, senderRemote);
            ////////////////////////////////////////////////////////////////
            CSReqMsg msgReq = new CSReqMsg();
            msgReq.head = new CSMsgHead();
            msgReq.head.cmd_id = (UInt32)cmdId;
            //msgReq.head.crypt_type = (UInt32)cryptType;        
            msgReq.msg = msgBody;

            ReqMessageItem newMsgItem = new ReqMessageItem();
            newMsgItem.connStream = connection.tcpConn.GetStream();
            newMsgItem.msg = msgReq;
            newMsgItem.enQueueTimeStamp = connection.curSvrTime();
            newMsgItem.conn = connection;
            UpdateNetState();
            lock (connection.sendMsgQueue)
            {
                //Debug.Log("Enqueue at:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                connection.sendMsgQueue.Enqueue(newMsgItem);
            }

            socket.SendTo(mshead.GetBuffer(), (int)mshead.Length, System.Net.Sockets.SocketFlags.None, senderRemote);
             */
        }

        public void CustomUpdate()
        {
            //写数据
            WriteQueueToReadQueue();
        }

        // 获取本机在局域网的IP地址
        public static string GetLocalIPAddress()
        {
            System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

            //获取ipv4
            StringCollection IpCollection = new StringCollection();
            foreach (IPAddress ip in addressList)
            {
                //根据AddressFamily判断是否为ipv4,如果是InterNetWork则为ipv6
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    IpCollection.Add(ip.ToString());
            }
            string[] IpArray = new string[IpCollection.Count];
            IpCollection.CopyTo(IpArray, 0);

            //获取唯一ip
            //string strNativeIP = "";
            string strServerIP = "";
            if (IpArray.Length > 1)
            {
                //strNativeIP = addressList[0].ToString();
                strServerIP = IpArray[1].ToString();
            }
            else if (IpArray.Length == 1)
            {
                strServerIP = IpArray[0].ToString();
            }

            return strServerIP;
        }

        //public static string GetLocalIPAddress()
        //{
        //    System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        //    //string strNativeIP = "";
        //    string strServerIP = "";
        //    if (addressList.Length > 1)
        //    {
        //        //strNativeIP = addressList[0].ToString();
        //        strServerIP = addressList[1].ToString();
        //    }
        //    else if (addressList.Length == 1)
        //    {
        //        strServerIP = addressList[0].ToString();
        //    }

        //    return strServerIP;
        //}

        //端口是否被占用
        public static bool UDPPortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveUdpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }
    }
}

