using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

namespace Framework
{
    public class UdpServerManager
    {
        public static UdpServerManager Instance
        {
            get
            {
                if (m_singleton == null)
                {
                    m_singleton = new UdpServerManager();
                }
                return m_singleton;
            }
        }
        private static UdpServerManager m_singleton = null;
        
        private UdpBaseServer _server;
        public UdpBaseServer Server
        {
            get
            {
                return _server;
            }
        }

        public bool InitServer(string serverIP, int serverPort)
        {
            _server = new UdpBaseServer();
            if (_server == null)
                return false;

            return _server.Connect(serverIP, serverPort);
        }

        public void ExitServer()
        {
            if (_server != null)
                _server.Disconnect();
        }

        public void Update()
        {
            if (_server != null)
            {
                _server.CustomUpdate();
                ReadQueuePro();
            }                
        }

        //可读队列消息处理
        public void ReadQueuePro()
        {
            while (_server.ReadQueue.Count != 0)
            {
                //读取并删除
                UdpBaseMessage bs = _server.ReadQueue.Dequeue();

                //读取版本号 用户ID 消息包号  消息协议号
                UInt32 version = BitConverter.ToUInt32(bs.bytes, 0);
                if (version != _server.gameVersion)
                    continue;

                //用户ID
            //    UInt64 _userid = BitConverter.ToUInt64(bs.bytes, 4);

                //消息包号
            //    UInt64 msgNum = BitConverter.ToUInt64(bs.bytes, 12);               

                //消息ID
           //     UInt32 msgID = BitConverter.ToUInt32(bs.bytes, 20);

                //消息体
                MemoryStream ms = new MemoryStream();
                ms.Write(bs.bytes, 24, bs.bytes.Length - 24);
                ms.Position = 0;

                //消息处理
                //switch ((proto.Udp_Proto)msgID)
                //{
                //    case proto.Udp_Proto.Proto_S2C_Udp_Hollo:
                //        {
                //            proto.S2C_Udp_Hollo msg = new proto.S2C_Udp_Hollo();

                //            msg = ProtoBuf.Serializer.Deserialize<proto.S2C_Udp_Hollo>(ms);
                //            ms.Close();
                //        }
                //        break; 
                //    default:
                //        break;
                //}
            }
        }

        public void OnSendHollo()
        {
            //proto.C2S_Udp_Hollo msg = new proto.C2S_Udp_Hollo();
            //msg.str = proto.Udp_Proto.Proto_C2S_Udp_Hollo.ToString();
            //UdpServerManager.Instance.Server.SendMessage(1, (uint)proto.Udp_Proto.Proto_C2S_Udp_Hollo,msg);
        }
    }
}
