//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;

namespace Framework
{

    public class CSReqMsg
    {
        public CSMsgHead head;
        public ProtoBuf.IExtensible msg;
        public byte[] msgBuffer;
        private static MemoryStream _memoryStream;
        private MemoryStream Steam
        {
            get
            {
                if (_memoryStream == null)
                {
                    _memoryStream = new MemoryStream();
                }
                return _memoryStream;
            }
        }

        public CommError.Type pack(ref byte[] buffer, int size, ref int usedSize)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            if (head == null)
            {
                head = new CSMsgHead();
            }
            ret = head.pack(ref buffer, size, ref usedSize);
            if (ret != CommError.Type.COMM_NO_ERROR)
            {
                return ret;
            }
            MemoryStream memStream = Steam;
            memStream.SetLength(0);
            memStream.Write(msgBuffer, 0, msgBuffer.Length);
           // ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(memStream, msg);
            int nLength = (int)memStream.Length;
            if (buffer != null && nLength > 0)
            {
                if (usedSize + nLength > size)
                {
                    return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
                }
                Array.Copy(buffer, 0, buffer, usedSize, nLength);
                usedSize += nLength;
            }
            head.pkg_len = (short)usedSize;
            int tmpSize = 0;
            ret = head.pack(ref buffer, size, ref tmpSize);
            if (ret != CommError.Type.COMM_NO_ERROR)
            {
                return ret;
            }
            return ret;
        }
    }

    public class CSResMsg
    {
        public CSMsgHead head;
        public byte[] MsgContent;

        public CommError.Type unpack(ref byte[] buffer, int size, ref int usedSize)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            if (head == null)
            {
                head = new CSMsgHead();
            }
            ret = head.unpack(ref buffer, size, ref usedSize);
            if (ret != CommError.Type.COMM_NO_ERROR)
            {
                return ret;
            }

            if (MsgContent == null)
            {
                MsgContent = new byte[head.pkg_len - usedSize];
            }

            if (size < head.pkg_len)
            {
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            Array.Copy(buffer, usedSize, MsgContent, 0, head.pkg_len - usedSize);
            usedSize = (int)head.pkg_len;
            return ret;
        }
    }


    public class CSMsgHead
    {
        public short pkg_len = 8;
        public short separator;  //分隔符
        public int com_len;  //内容长度
        public short cmd_id;
    //    public UInt32 tv_sec;
        //    public UInt32 tv_usec;

        public CSMsgHead() { }

        public CommError.Type pack(ref byte[] buffer, int size, ref int usedSize)
        {
            if (buffer == null || buffer.GetLength(0) == 0 || buffer.GetLength(0) > size)
            {
                return CommError.Type.COMM_ERR_INVALID_BUFFER_PARAMETER;
            }

            CommWriteBuf destBuf = new CommWriteBuf(ref buffer, size);
            CommError.Type ret = pack(ref destBuf);

            if (ret == CommError.Type.COMM_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();

                usedSize = destBuf.getUsedSize();
            }
            return ret;
        }



        public CommError.Type pack(ref CommWriteBuf destBuf)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* pack member: this.pkg_len */
            ret = destBuf.writeInt16(this.pkg_len);
            if (CommError.Type.COMM_NO_ERROR != ret)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine("R8Game_TRACE:  " + st.GetFrame(i).ToString());
                    }
                }
#endif
                return ret;
            }

            ret = destBuf.writeInt16(this.separator);

            /* pack member: this.cmd_id */
            ret = destBuf.writeInt32(this.com_len);
            if (CommError.Type.COMM_NO_ERROR != ret)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine("R8Game_TRACE:  " + st.GetFrame(i).ToString());
                    }
                }
#endif
                return ret;
            }

            return ret;
        }

        public CommError.Type unpack(ref byte[] buffer,int size,ref int usedSize)
        {
            if (buffer == null || buffer.GetLength(0) == 0)// || buffer.GetLength(0) > size)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine("R8GAME_TRACE:  " + st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_INVALID_BUFFER_PARAMETER;
            }

            CommReadBuf srcBuf = new CommReadBuf(ref buffer,size);
            CommError.Type ret = unpack(ref srcBuf);

            usedSize = srcBuf.getUsedSize();

            return ret;
        }

        public CommError.Type unpack(ref CommReadBuf scrBuf)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* unpack member: this.pkg_len */
            ret = scrBuf.readInt16(ref this.pkg_len);
            if (ret != CommError.Type.COMM_NO_ERROR)
            {
                return ret;
            }

            ret = scrBuf.readInt16(ref this.separator);

            /* unpack member: this.cmd_id */
            ret = scrBuf.readInt32(ref this.com_len);
            if (ret != CommError.Type.COMM_NO_ERROR)
            {
                return ret;
            }

            return ret;
        }

        public CommError.Type load(ref byte[] buffer, int size, ref int usedSize)
        {
            if (null == buffer || 0 == buffer.GetLength(0) || size > buffer.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine("R8GAME_TRACE:  " + st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_INVALID_BUFFER_PARAMETER;
            }

            CommReadBuf srcBuf = new CommReadBuf(ref buffer, size);
            CommError.Type ret = load(ref srcBuf);

            usedSize = srcBuf.getUsedSize();

            return ret;
        }
        public CommError.Type load(ref CommReadBuf srcBuf)
        {
            srcBuf.disableEndian();
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* load member: this.pkg_len */
            ret = srcBuf.readInt16(ref this.pkg_len);
            if (CommError.Type.COMM_NO_ERROR != ret)
            {
                return ret;
            }

            /* load member: this.cmd_id */
            ret = srcBuf.readInt16(ref this.cmd_id);
            if (CommError.Type.COMM_NO_ERROR != ret)
            {
                return ret;
            }
            return ret;
        }


        /* set indent = -1 to disable indent , default: separator = '\n' */
        public CommError.Type visualize(ref string buffer, int indent, char separator)
        {
            VisualBuf destBuf = new VisualBuf();
            CommError.Type ret = visualize(ref destBuf, indent, separator);

            buffer = destBuf.getVisualBuf();

            return ret;
        }

        public CommError.Type visualize(ref VisualBuf destBuf, int indent, char separator)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* visualize member: this.pkg_len */
            ret = CommBufUtil.printVariable(ref destBuf, indent, separator, "[pkg_len]", "{0:d}", this.pkg_len);
            if (CommError.Type.COMM_NO_ERROR != ret)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine("R8GAME_TRACE:  " + st.GetFrame(i).ToString());
                    }
                }
#endif
                return ret;
            }

            /* visualize member: this.cmd_id */
            ret = CommBufUtil.printVariable(ref destBuf, indent, separator, "[cmd_id]", "{0:d}", this.cmd_id);
            if (CommError.Type.COMM_NO_ERROR != ret)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine("R8GAME_TRACE:  " + st.GetFrame(i).ToString());
                    }
                }
#endif
                return ret;
            }

            return ret;
        }
    }
}