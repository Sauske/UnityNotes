//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Threading;

namespace Framework
{
    
    public class TcpReceiveThread :NetThread
    {
        public const uint MAX_PACKET_SIZE = 100;// * 256;
        public const uint MIN_PACKET_SIZE = 8;

        private byte[] _buffer;
        private int _bufOffset;
        private TcpClientConnection _tcpConn;
        private NetworkStream _stream;
       // private int _headLenght = 0;
        
        public TcpReceiveThread(TcpClientConnection tcpConn)
        {
            _buffer = new byte[MAX_PACKET_SIZE];
            _bufOffset = 0;
            _tcpConn = tcpConn;
            _stream = tcpConn.tcpClient.GetStream();
        }

        System.Text.StringBuilder steamStr = new System.Text.StringBuilder();
        protected override void Main()
        {
            try
            {
                while (!IsOver())
                {
                    bool packetGot = false;
                    try
                    {
                        if (_tcpConn != null)
                        {
                            if (_stream.DataAvailable)
                            {
                                _bufOffset += _stream.Read(_buffer, _bufOffset, _buffer.Length - _bufOffset);

                                
                                for(int i = 0;i < 10; i++)
                                {
                                   // steamStr.Remove(0, steamStr.Length);
                                    for(int j = 0;j < 10; j++)
                                    {
                                        steamStr.Append(_buffer[i * 10 + j]).Append(",");
                                    }
                                    steamStr.Append("\n");
                                }
                                Debug.Log(steamStr.ToString());
                                
                                ScanPacket(_tcpConn);
                                packetGot = true;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        DebugHelper.LogError(e.Message);
                    }

                    if (!packetGot)
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

        protected void ScanPacket(TcpClientConnection tcpConn)
        {
            bool packetFound = false;
            do
            {
                try
                {
                    packetFound = false;
                    if (_bufOffset >= MIN_PACKET_SIZE)
                    {
                        CommError.Type ret = CommError.Type.COMM_NO_ERROR;
                        CSResMsg resMsg = new CSResMsg();
                        int usedSize = 0;
                        ret = resMsg.unpack(ref _buffer,_bufOffset,ref usedSize);
                        if (ret != CommError.Type.COMM_NO_ERROR)
                        {
                            DebugHelper.LogError("NetSys:ScanPakct head Error:" + CommError.getErrorString(ret));
                            return;
                        }

                        ///强制对时
                        if (resMsg.head.cmd_id == (uint)CMD_ID.CS_CMD_REQ_PING)
                        {
                           // _tcpConn.ForceSynsTime(resMsg);
                        }
                        else
                        {
                            ResMessageItem item = new ResMessageItem();
                            item.msg = resMsg;
                            item.connType = tcpConn.mConnectionType;
                            Debug.Log(System.Text.Encoding.UTF8.GetString(resMsg.MsgContent));
                            tcpConn.EnqueueRespondMsg(item);
                        }

                        packetFound = true;
                        int offset = (int)resMsg.head.pkg_len;
                        if (_bufOffset > offset)
                        {
                            for (int i = offset, j = 0; i < _bufOffset; i++, j++)
                            {
                                _buffer[j] = _buffer[i];
                            }
                            _bufOffset -= offset;
                        }
                        else
                        {
                            _bufOffset = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError(ex.Message);
                }
            }
            while (packetFound && !IsOver());
        }
    }
}