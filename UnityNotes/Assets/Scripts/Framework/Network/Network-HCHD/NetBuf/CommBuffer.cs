using System;
using System.Net;
using System.Diagnostics;

namespace UMI.Net
{
    public class CommWriteBuf
    {
        private byte[] beginPtr;
        private Int32 position;
        private Int32 length;

        private void _set(ref byte[] ptr, Int32 len)
        {
            beginPtr = ptr;
            position = 0;
            length = 0;

            if (null != beginPtr)
            {
                length = len;
            }
        }

        private void _reset()
        {
            position = 0;
            length = 0;
            beginPtr = null;
        }

        public CommWriteBuf()
        {
            beginPtr = null;
            position = 0;
            length = 0;
        }

        public CommWriteBuf(ref byte[] ptr, Int32 len)
        {
            _set(ref ptr, len);
        }

        public CommWriteBuf(Int32 len)
        {
            beginPtr = new byte[len];
            position = 0;
            length = 0;

            if (null != beginPtr)
            {
                length = len;
            }
        }

        /* public function */
        public void reset()
        {
            _reset();
        }

        public void set(ref byte[] ptr, int len)
        {
            _set(ref ptr, len);
        }

        public Int32 getUsedSize()
        {
            return position;
        }

        public Int32 getTotalSize()
        {
            return length;
        }

        public Int32 getLeftSize()
        {
            return (length - position);
        }

        public byte[] getBeginPtr()
        {
            return beginPtr;
        }

        public CommError.Type reserve(int gap)
        {
            if (position > length)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (gap > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            position += gap;

            return CommError.Type.COMM_NO_ERROR;
        }

        /* write successively */
        public CommError.Type writeInt8(sbyte src)
        {
            return writeUInt8((byte)src);
        }

        public CommError.Type writeUInt8(byte src)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(byte) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            beginPtr[position++] = src;

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeUInt16(UInt16 src)
        {
            return writeInt16((Int16)src);
        }

        public CommError.Type writeInt16(Int16 src)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int16) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }

            byte[] buffer = BitConverter.GetBytes(src);

            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                beginPtr[position++] = buffer[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeUInt32(UInt32 src)
        {
            return writeInt32((Int32)src);
        }

        public CommError.Type writeInt32(Int32 src)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int32) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }

            byte[] buffer = BitConverter.GetBytes(src);

            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                beginPtr[position++] = buffer[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeUInt64(UInt64 src)
        {
            return writeInt64((Int64)src);
        }

        public CommError.Type writeInt64(Int64 src)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int64) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }

            byte[] buffer = BitConverter.GetBytes(src);

            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                beginPtr[position++] = buffer[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeFloat(float src)
        {
            Int32 tmp = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
            return writeInt32(tmp);
        }

        public CommError.Type writeDouble(double src)
        {
            Int64 tmp = BitConverter.DoubleToInt64Bits(src);
            return writeInt64(tmp);
        }

        public CommError.Type writeCString(byte[] src, Int32 count)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (count > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            for (int i = 0; i < count; i++)
            {
                beginPtr[position++] = src[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeWString(Int16[] src, Int32 count)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if ((2 * count) > (length - position))
            {
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            for (int i = 0; i < count; i++)
            {
                byte[] buffer = BitConverter.GetBytes(src[i]);
                for (int j = 0; j < buffer.GetLength(0); j++)
                {
                    beginPtr[position++] = buffer[j];
                }
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        /* write directly */
        public CommError.Type writeInt8(sbyte src, int pos)
        {
            return writeUInt8((byte)src, pos);
        }

        public CommError.Type writeUInt8(byte src, int pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(byte) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            beginPtr[pos] = src;

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeUInt16(UInt16 src, int pos)
        {
            return writeInt16((Int16)src, pos);
        }

        public CommError.Type writeInt16(Int16 src, int pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int16) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }

            byte[] buffer = BitConverter.GetBytes(src);
            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                beginPtr[pos + i] = buffer[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeUInt32(UInt32 src, int pos)
        {
            return writeInt32((Int32)src, pos);
        }

        public CommError.Type writeInt32(Int32 src, int pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int32) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }

            byte[] buffer = BitConverter.GetBytes(src);
            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                beginPtr[pos + i] = buffer[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeUInt64(UInt64 src, int pos)
        {
            return writeInt64((Int64)src, pos);
        }

        public CommError.Type writeInt64(Int64 src, int pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int64) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }

            byte[] buffer = BitConverter.GetBytes(src);
            for (int i = 0; i < buffer.GetLength(0); i++)
            {
                beginPtr[pos + i] = buffer[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeFloat(float src, int pos)
        {
            Int32 tmp = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
            return writeInt32(tmp, pos);
        }

        public CommError.Type writeDouble(double src, int pos)
        {
            Int64 tmp = BitConverter.DoubleToInt64Bits(src);
            return writeInt64(tmp, pos);
        }

        public CommError.Type writeCString(byte[] src, Int32 count, Int32 pos)
        {
            if (null == beginPtr || count > src.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (count > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            for (int i = 0; i < count; i++)
            {
                beginPtr[pos + i] = src[i];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type writeWString(Int16[] src, Int32 count, Int32 pos)
        {
            if (null == beginPtr || count > src.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if ((2 * count) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            for (int i = 0; i < count; i++)
            {
                byte[] buffer = BitConverter.GetBytes(src[i]);
                for (int j = 0; j < buffer.GetLength(0); j++)
                {
                    beginPtr[pos + (2 * i + j)] = buffer[j];
                }
            }

            return CommError.Type.COMM_NO_ERROR;
        }
    }


    public class CommReadBuf
    {
        private byte[] beginPtr;
        private Int32 position;
        private Int32 length;
        private bool IsNetEndian;

        public CommReadBuf()
        {
            length = 0;
            position = 0;
            beginPtr = null;
            IsNetEndian = true;
        }

        public CommReadBuf(ref CommWriteBuf CommWriteBuf)
        {
            byte[] ptr = CommWriteBuf.getBeginPtr();
            set(ref ptr, CommWriteBuf.getUsedSize());
        }

        public CommReadBuf(ref byte[] ptr, Int32 len)
        {
            set(ref ptr, len);
        }

        public void reset()
        {
            length = 0;
            position = 0;
            beginPtr = null;
            IsNetEndian = true;
        }


        public void set(ref byte[] ptr, Int32 len)
        {
            beginPtr = ptr;
            position = 0;
            length = 0;
            IsNetEndian = true;

            if (null != beginPtr)
            {
                length = len;
            }
        }

        public Int32 getUsedSize()
        {
            return position;
        }

        public Int32 getTotalSize()
        {
            return length;
        }

        public Int32 getLeftSize()
        {
            return length - position;
        }

        public void disableEndian()
        {
            IsNetEndian = false;
        }

        /* read successively */

        public CommError.Type readInt8(ref sbyte dest)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            byte tmp = 0;
            ret = readUInt8(ref tmp);
            dest = (sbyte)tmp;

            return ret;
        }

        public CommError.Type readUInt8(ref byte dest)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(byte) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = beginPtr[position++];

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readInt16(ref Int16 dest)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int16) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = BitConverter.ToInt16(beginPtr, position);
            position += sizeof(Int16);

            if (IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readUInt16(ref UInt16 dest)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int16 tmp = 0;
            ret = readInt16(ref tmp);
            dest = (UInt16)tmp;

            return ret;
        }

        public CommError.Type readInt32(ref Int32 dest)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int32) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = BitConverter.ToInt32(beginPtr, position);
            position += sizeof(Int32);

            if (IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readUInt32(ref UInt32 dest)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int32 tmp = 0;
            ret = readInt32(ref tmp);
            dest = (UInt32)tmp;

            return ret;
        }

        public CommError.Type readInt64(ref Int64 dest)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int64) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = BitConverter.ToInt64(beginPtr, position);
            position += sizeof(Int64);

            if (IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readUInt64(ref UInt64 dest)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int64 tmp = 0;
            ret = readInt64(ref tmp);
            dest = (UInt64)tmp;

            return ret;
        }

        public CommError.Type readFloat(ref float dest)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int32 tmp = 0;
            ret = readInt32(ref tmp);
            dest = BitConverter.ToSingle(BitConverter.GetBytes(tmp), 0);

            return ret;
        }

        public CommError.Type readDouble(ref double dest)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int64 tmp = 0;
            ret = readInt64(ref tmp);
            dest = BitConverter.Int64BitsToDouble(tmp);

            return ret;
        }

        public CommError.Type readCString(ref byte[] dest, Int32 count)
        {
            if (null == beginPtr || count > dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (null == dest || 0 == dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (count > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            for (int i = 0; i < count; i++)
            {
                dest[i] = beginPtr[position++];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readWString(ref Int16[] dest, Int32 count)
        {
            if (null == beginPtr || count > dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (null == dest || 0 == dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if ((2 * count) > (length - position))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            for (int i = 0; i < count; i++)
            {
                dest[i] = BitConverter.ToInt16(beginPtr, position);
                position += sizeof(Int16);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        /* read directly */
        public CommError.Type readInt8(ref sbyte dest, Int32 pos)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            byte tmp = 0;
            ret = readUInt8(ref tmp, pos);
            dest = (sbyte)tmp;

            return ret;
        }

        public CommError.Type readUInt8(ref byte dest, Int32 pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(byte) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = beginPtr[pos];

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readInt16(ref Int16 dest, Int32 pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int16) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = BitConverter.ToInt16(beginPtr, pos);

            if (IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readUInt16(ref UInt16 dest, Int32 pos)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int16 tmp = 0;
            ret = readInt16(ref tmp, pos);
            dest = (UInt16)tmp;

            return ret;
        }

        public CommError.Type readInt32(ref Int32 dest, Int32 pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int32) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = BitConverter.ToInt32(beginPtr, pos);

            if (IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readUInt32(ref UInt32 dest, Int32 pos)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int32 tmp = 0;
            ret = readInt32(ref tmp, pos);
            dest = (UInt32)tmp;

            return ret;
        }

        public CommError.Type readInt64(ref Int64 dest, Int32 pos)
        {
            if (null == beginPtr)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (sizeof(Int64) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            dest = BitConverter.ToInt64(beginPtr, pos);

            if (IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readUInt64(ref UInt64 dest, Int32 pos)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int64 tmp = 0;
            ret = readInt64(ref tmp, pos);
            dest = (UInt64)tmp;

            return ret;
        }

        public CommError.Type readFloat(ref float dest, Int32 pos)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int32 tmp = 0;
            ret = readInt32(ref tmp, pos);
            dest = BitConverter.ToSingle(BitConverter.GetBytes(tmp), 0);

            return ret;
        }

        public CommError.Type readDouble(ref double dest, Int32 pos)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            Int64 tmp = 0;
            ret = readInt64(ref tmp, pos);
            dest = BitConverter.Int64BitsToDouble(tmp);

            return ret;
        }

        public CommError.Type readCString(ref byte[] dest, Int32 count, Int32 pos)
        {
            if (null == beginPtr || count > dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (null == dest || 0 == dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (count > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            for (int i = 0; i < count; i++)
            {
                dest[i] = beginPtr[pos + count];
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type readWString(ref Int16[] dest, Int32 count, Int32 pos)
        {
            if (null == beginPtr || count > dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if (null == dest || 0 == dest.GetLength(0))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_ARG_IS_NULL;
            }

            if ((2 * count) > (length - pos))
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;
            }

            for (int i = 0; i < count; i++)
            {
                dest[i] = BitConverter.ToInt16(beginPtr, pos + 2 * count);
            }

            return CommError.Type.COMM_NO_ERROR;
        }

        public CommError.Type toHexStr(ref char[] buffer, out int usedsize)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            int dataLen = length - position;
            int hexDatLen = dataLen * 2 + 1;

            if (buffer.GetLength(0) < hexDatLen)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                usedsize = 0;
                return CommError.Type.COMM_ERR_SHORT_BUF_FOR_WRITE;
            }

            string s = "";
            byte[] bArray = new byte[length - position];
            for (int i = 0; i < length - position; i++)
            {
                ret = readUInt8(ref bArray[i], position + i);
                if (ret != CommError.Type.COMM_NO_ERROR)
                {
                    usedsize = 0;
                    return ret;
                }

                s += string.Format("{0:x2}", bArray[i]);
            }

            s += string.Format("{0:x}", 0x00);
            buffer = s.ToCharArray();
            usedsize = hexDatLen;

            return ret;
        }

        public CommError.Type toHexStr(ref string buffer)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            byte[] bArray = new byte[length - position];
            for (int i = 0; i < length - position; i++)
            {
                ret = readUInt8(ref bArray[i], position + i);
                if (ret != CommError.Type.COMM_NO_ERROR)
                {
                    return ret;
                }

                buffer += string.Format("{0:x2}", bArray[i]);
            }

            buffer += string.Format("{0:x}", 0x00);

            return ret;
        }

    }

    public class VisualBuf
    {
        private string visualBuf;

        public VisualBuf()
        {
            visualBuf = "";
        }

        public CommError.Type sprintf(string format, params object[] args)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            string str = "";

            try
            {
                str = string.Format(format, args);
            }
            catch (System.ArgumentNullException ex)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                Console.WriteLine("Error: " + ex.Message);
                ret = CommError.Type.COMM_ERR_ARGUMENT_NULL_EXCEPTION;
            }
            catch (System.FormatException ex)
            {
#if (DEBUG)
                StackTrace st = new StackTrace(true);
                for (int i = 0; i < st.FrameCount; i++)
                {
                    if (null != st.GetFrame(i).GetFileName())
                    {
                        Console.WriteLine(st.GetFrame(i).ToString());
                    }
                }
#endif
                Console.WriteLine("Error: " + ex.Message);
                ret = CommError.Type.COMM_ERR_INVALID_FORMAT;
            }

            if (CommError.Type.COMM_NO_ERROR == ret)
            {
                visualBuf += str;
            }

            return ret;
        }

        public string getVisualBuf()
        {
            return visualBuf;
        }
    }
}

