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
using System.Net.Sockets;

namespace Framework
{
    
    public class TcpSendThread : NetThread
    {
        public const int MAX_PACKET_SIZE = 8192;

        private byte[] _buffer = new byte[MAX_PACKET_SIZE];
        private NetworkStream _netSteam;
        private Queue<ReqMessageItem> _reqQueue;

        public TcpSendThread(Queue<ReqMessageItem> reqQueue,NetworkStream ns)
        {
            _reqQueue = reqQueue;
            _netSteam = ns;
        }

        System.Text.StringBuilder steamStr = new System.Text.StringBuilder();

        protected override void Main()
        {
            ReqMessageItem item = default(ReqMessageItem);
            while (!IsOver())
            {
                try
                {
                    if (_netSteam.CanWrite)
                    {
                        bool bHavePkg = false;
                        lock (_reqQueue)
                        {
                            if (_reqQueue.Count > 0)
                            {
                                item = _reqQueue.Dequeue();
                                bHavePkg = true;
                            }
                        }
                        if (bHavePkg)
                        {
                            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
                            int pack_len = 0;
                            ret = item.msg.pack(ref _buffer, _buffer.Length, ref pack_len);
                            if (ret != CommError.Type.COMM_NO_ERROR)
                            {
                                Debug.LogErrorFormat("封包错误：CMD ：{0}", item.msg.head.cmd_id);
                                continue;
                            }
                            if (item.msg.head.cmd_id == (uint)CMD_ID.CS_CMD_REQ_PING) //心跳
                            {
                                //TODO
                                item.mTcpClient.helloSendTime = DateTime.Now;
                            }
                            if (pack_len == 0)
                            {
                                Debug.LogErrorFormat("封包错误,长度为0。CMD ：{0}", item.msg.head.cmd_id);
                                continue;
                            }
                            try
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    for (int j = 0; j < 10; j++)
                                    {
                                        steamStr.Append(_buffer[i * 10 + j]).Append(",");
                                    }
                                    steamStr.Append("\n");
                                }
                                Debug.Log(steamStr.ToString());

                                item.mStream.Write(_buffer, 0, pack_len);
                                item.mStream.Flush();
                            }
                            catch (IOException ioEx)
                            {
                                DebugHelper.LogError(ioEx.Message);
                            }
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError(ex.Message);
                }
            }
            base.Main();
        }
    }
}