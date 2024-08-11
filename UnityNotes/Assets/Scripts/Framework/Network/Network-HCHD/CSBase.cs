using System;
using System.Net;
using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace UMI.Net
{
    public class CSReqMsg
    {
        public CSMsgHead head;
        //public byte[] MsgContent;
        public ProtoBuf.IExtensible msg;
        private static byte[] buff = new byte[8192];
        private static MemoryStream m_cMemoryStream;
        private MemoryStream memoryStream
        {
            get
            {
                if (m_cMemoryStream == null)
                {
                    m_cMemoryStream = new MemoryStream(buff);
                }

                return m_cMemoryStream;
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
            MemoryStream memStream = memoryStream;
            memStream.SetLength(0);
            ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(memStream, msg);
            int nLength = (int)memStream.Length;
            if (buff != null && nLength > 0)
            {
                if (usedSize + nLength > size)
                {
                    return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
                }
                Array.Copy(buff, 0, buffer, usedSize, nLength);
                usedSize += nLength;
            }
            head.pkg_len = (UInt32)usedSize;
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
        /* public members */
        public UInt32 pkg_len;
        public UInt32 cmd_id;
        public UInt32 crypt_type;
        public UInt32 tv_sec;
        public UInt32 tv_usec;

        /* construct methods */
        public CSMsgHead()
        {
        }

        public CommError.Type pack(ref byte[] buffer, int size, ref int usedSize)
        {
            if (null == buffer || 0 == buffer.GetLength(0) || (size > buffer.GetLength(0)))
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
            {
                ret = destBuf.writeUInt32(this.pkg_len);
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
            }

            /* pack member: this.cmd_id */
            {
                ret = destBuf.writeUInt32(this.cmd_id);
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
            }

            /* pack member: this.crypt_type */
            {
                ret = destBuf.writeUInt32(this.crypt_type);
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
            }

            /* pack member: this.tv_sec */
            {
                ret = destBuf.writeUInt32(this.tv_sec);
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
            }


            /* pack member: this.tv_usec */
            {
                ret = destBuf.writeUInt32(this.tv_usec);
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
            }

            return ret;
        }

        public CommError.Type unpack(ref byte[] buffer, int size, ref int usedSize)
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
            CommError.Type ret = unpack(ref srcBuf);

            usedSize = srcBuf.getUsedSize();

            return ret;
        }

        public CommError.Type unpack(ref CommReadBuf srcBuf)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* unpack member: this.pkg_len */
            {
                ret = srcBuf.readUInt32(ref this.pkg_len);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.cmd_id */
            {
                ret = srcBuf.readUInt32(ref this.cmd_id);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.crypt_type */
            {
                ret = srcBuf.readUInt32(ref this.crypt_type);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.tv_sec */
            {
                ret = srcBuf.readUInt32(ref this.tv_sec);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.tv_usec */
            {
                ret = srcBuf.readUInt32(ref this.tv_usec);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
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
            {
                ret = srcBuf.readUInt32(ref this.pkg_len);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* load member: this.cmd_id */
            {
                ret = srcBuf.readUInt32(ref this.cmd_id);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* load member: this.crypt_type */
            {
                ret = srcBuf.readUInt32(ref this.crypt_type);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* load member: this.tv_sec */
            {
                ret = srcBuf.readUInt32(ref this.tv_sec);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* load member: this.tv_usec */
            {
                ret = srcBuf.readUInt32(ref this.tv_usec);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
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

        /* set indent = -1 to disable indent , default: separator = '\n' */
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

            /* visualize member: this.crypt_type */
            ret = CommBufUtil.printVariable(ref destBuf, indent, separator, "[crypt_type]", "{0:d}", this.crypt_type);
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


            /* visualize member: this.tv_sec */
            ret = CommBufUtil.printVariable(ref destBuf, indent, separator, "[tv_sec]", "{0:d}", this.tv_sec);
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


            /* visualize member: this.tv_usec */
            ret = CommBufUtil.printVariable(ref destBuf, indent, separator, "[tv_usec]", "{0:d}", this.tv_usec);
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
