
using System;
using System.Net;
using UnityEngine;
using System.Diagnostics;
using System.IO;
//using Assets.Scripts.Common;

namespace Framework
{

    public class NetPkgHead : RGameProtocolObject
    {
        private UInt32 mPkgLen;          //包总的大小 = 包头 + 包体
        private UInt32 mCmdId;           //消息ID
        private UInt32 mSvrPkgSeq;      //发送的序列号
        private UInt32 mReserve;        //被relaysrv使用作为录像长度统计
        

        public UInt32 PkgLen
        {
            get { return mPkgLen; }
            set { mPkgLen = value; }
        }

        public UInt32 CmdId
        {
            get { return mCmdId; }
            set { mCmdId = value; }
        }


        public UInt32 SvrPkgSeq
        {
            get { return mSvrPkgSeq; }
            set { mSvrPkgSeq = value; }
        }

        public UInt32 Reserve
        {
            get { return mReserve; }
            set { mReserve = value; }
        }

        public static UInt32 HeadLength
        {
            get { return sizeof(UInt32) * 4; }
        }

        public NetPkgHead()
        {

        }

        public NetPkgHead(int cmdId)
        {
            this.mCmdId = (UInt32)cmdId;
        }

        public override CommError.Type construct()
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            return ret;
        }

        public CommError.Type pack(ref byte[] buffer, int size, ref int usedSize)
        {
            if (null == buffer || 0 == buffer.GetLength(0) || (size > buffer.GetLength(0)))
            {
                return CommError.Type.COMM_ERR_INVALID_BUFFER_PARAMETER;
            }

            CommWriteBuf destBuf = ClassObjPool<CommWriteBuf>.Get();
            destBuf.set(ref buffer, size, usedSize);

            CommError.Type ret = pack(ref destBuf);
            if (ret == CommError.Type.COMM_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();

                usedSize = destBuf.getUsedSize();
            }

            destBuf.Release();
            return ret;
        }

        public override CommError.Type pack(ref CommWriteBuf destBuf)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* pack member: this.pkg_len */
            {
                ret = destBuf.writeUInt32(this.mPkgLen);
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
                ret = destBuf.writeUInt32(this.mCmdId);
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

            /* pack member: this.mSvrPkgSeq */
            {
                ret = destBuf.writeUInt32(this.mSvrPkgSeq);
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

            /* pack member: this.mReserve */
            {
                ret = destBuf.writeUInt32(this.mReserve);
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

            CommReadBuf srcBuf = ClassObjPool<CommReadBuf>.Get();
            srcBuf.set(ref buffer, size, usedSize);

            CommError.Type ret = unpack(ref srcBuf);

            usedSize = srcBuf.getUsedSize();

            srcBuf.Release();

            return ret;
        }

        public override CommError.Type unpack(ref CommReadBuf srcBuf)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* unpack member: this.pkg_len */
            {
                ret = srcBuf.readUInt32(ref this.mPkgLen);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.cmd_id */
            {
                ret = srcBuf.readUInt32(ref this.mCmdId);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.mSvrPkgSeq */
            {
                ret = srcBuf.readUInt32(ref this.mSvrPkgSeq);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            /* unpack member: this.mReserve */
            {
                ret = srcBuf.readUInt32(ref this.mReserve);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            return ret;
        }

        public static readonly int CLASS_ID = 1;
        public override int GetClassID()
        {
            return CLASS_ID;
        }


        public override void OnRelease()
        {
            mPkgLen = 0;
            mCmdId = 0;
            mSvrPkgSeq = 0;
            mReserve = 0;
        }
    }

    public class NetPkgBody : RGameProtocolObject
    {
        private PBAgent mPBAgent = null;

        //C->S发送的消息内容
        private ProtoBuf.IExtensible mMsg = null;

        //S->C 接收到的包体内容
        private byte[] mRecvMsgData = null;

        private int mRecvMsgLen = 0;

        public ProtoBuf.IExtensible Msg
        {
            get { return mMsg; }
            set { mMsg = value; }
        }

        public PBAgent PBAgent
        {
            get { return mPBAgent; }
            set { mPBAgent = value; }
        }


        public byte[] RecvMsgData
        {
            get { return mRecvMsgData; }
        }

        public int RecvMsgLen
        {
            get { return mRecvMsgLen; }
        }

        public NetPkgBody()
        {
            this.mPBAgent = null;
            this.Msg = null;
            this.mRecvMsgData = null;
            this.mRecvMsgLen = 0;
        }

        public override CommError.Type construct()
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            return ret;
        }

        public void InitRecvMessage(int packLen)
        {
            mRecvMsgLen = packLen - (int)NetPkgHead.HeadLength;
            if (mRecvMsgLen <= 0) return;

            if (mRecvMsgData == null)
            {
                mRecvMsgData = new byte[mRecvMsgLen];
            }
            else
            {
                if (mRecvMsgLen <= mRecvMsgData.Length)
                {
                    mRecvMsgData.Initialize();
                }
                else
                {
                    mRecvMsgData = null;
                    mRecvMsgData = new byte[mRecvMsgLen];
                }
            }
        }


        public CommError.Type pack(ref byte[] buffer, int size, ref int usedSize)
        {
            if (null == buffer || 0 == buffer.GetLength(0) || (size > buffer.GetLength(0)))
            {
                return CommError.Type.COMM_ERR_INVALID_BUFFER_PARAMETER;
            }

            if(mMsg == null && (mPBAgent == null || mPBAgent.GetMsg() == null))
            {
                return CommError.Type.COMM_NO_ERROR;
            }

            CommMemoryStream ms = ClassObjPool<CommMemoryStream>.Get();
            if (mMsg != null)
                ms.set(mMsg);
            else if (mPBAgent != null)
                ms.set(mPBAgent.GetMsg());

            int nLen = ms.len;
            if (nLen == 0)
            {
                return CommError.Type.COMM_NO_ERROR;
            }

            CommWriteBuf destBuf = ClassObjPool<CommWriteBuf>.Get();
            destBuf.set(ref buffer,size,usedSize);
            
            CommError.Type ret = pack(ref destBuf,ms.buffer,nLen);
            if (ret == CommError.Type.COMM_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();

                usedSize = destBuf.getUsedSize();
            }

            ms.Release();
            destBuf.Release();

            return ret;
        }

        public CommError.Type pack(ref CommWriteBuf destBuf, byte[] data, int len)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /*pack member mMsg data*/
            ret = destBuf.writeByteArrar(data, len);
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

        public CommError.Type unpack(ref byte[] buffer, int size, ref int usedSize)
        {
            if (mRecvMsgLen == 0) return CommError.Type.COMM_NO_ERROR;

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

            if(null == this.mRecvMsgData)
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
                return CommError.Type.COMM_ERR_USE_HAVE_NOT_INIT_VARIABLE_ARRAY;
            }

            CommReadBuf srcBuf = ClassObjPool<CommReadBuf>.Get();
            srcBuf.set(ref buffer, size, usedSize);


            CommError.Type ret = unpack(ref srcBuf);

            usedSize = srcBuf.getUsedSize();

            srcBuf.Release();

            return ret;
        }

        public override CommError.Type unpack(ref CommReadBuf srcBuf)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            /* unpack member: this.mRecvMsgData */
            {
                ret = srcBuf.readByteArray(ref this.mRecvMsgData, mRecvMsgLen);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }

            return ret;
        }

        public static readonly int CLASS_ID = 2;

        public override int GetClassID()
        {
            return CLASS_ID;
        }


        public override void OnRelease()
        {
            if(this.mPBAgent!=null)
            {
                this.mPBAgent.Release();
                this.mPBAgent = null;
            }

            mMsg = null;
            mRecvMsgLen = 0;
            
            if(mRecvMsgData != null)
            {
                mRecvMsgData.Initialize();
            }
        }
    }

    public class NetPkg : RGameProtocolObject
    {
        /// <summary>
        /// 消息包头
        /// </summary>
        public NetPkgHead Head = null;

        public NetPkgBody Body = null;

        
        public static NetPkg CreateNetReqPkg(int cmdId,PBAgent agent)
        {
            NetPkg reqPkg = (NetPkg)RGameProtocolObjectPool.Get(CLASS_ID);
            reqPkg.InitWithAgent(cmdId, agent);
            return reqPkg;
        }

        /// <summary>
        /// 创建网络发送消息包
        /// </summary>
        /// <param name="cmdId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static NetPkg CreateNetReqPkg(int cmdId, ProtoBuf.IExtensible msg)
        {
            NetPkg reqPkg = (NetPkg)RGameProtocolObjectPool.Get(CLASS_ID);
            reqPkg.Init(cmdId, msg);
            return reqPkg;
        }


        /// <summary>
        /// 创建接收消息包
        /// </summary>
        /// <returns></returns>
        public static NetPkg CreateNetRevPkg()
        {
            NetPkg revPkg = (NetPkg)RGameProtocolObjectPool.Get(CLASS_ID);
            revPkg.Init(0, null);
            return revPkg;
        }


        public NetPkg()
        {

        }

        public override CommError.Type construct()
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            if(this.Head!=null)
            {
                ret = this.Head.construct();

                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }
           

            if(this.Body!=null)
            {
                ret = this.Body.construct();
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    return ret;
                }
            }
          

            return ret;
        }

        public void InitWithAgent(int cmdId,PBAgent agent)
        {
            if (Head == null)
            {
                Head = (NetPkgHead)RGameProtocolObjectPool.Get(NetPkgHead.CLASS_ID);
            }

            Head.CmdId = (uint)cmdId;

            if (Body == null)
                Body = (NetPkgBody)RGameProtocolObjectPool.Get(NetPkgBody.CLASS_ID);

            Body.Msg = null;
            Body.PBAgent = agent;
        }

        public void Init(int cmdId,ProtoBuf.IExtensible msg)
        {
            if (Head == null)
            {
                Head = (NetPkgHead)RGameProtocolObjectPool.Get(NetPkgHead.CLASS_ID);
            }

            Head.CmdId = (uint)cmdId;

            if (Body == null)
                Body = (NetPkgBody)RGameProtocolObjectPool.Get(NetPkgBody.CLASS_ID);

            Body.Msg = msg;
            Body.PBAgent = null;
        }
        
        /// <summary>
        /// 序列号
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="usedSize"></param>
        /// <returns></returns>
        public CommError.Type pack(ref byte[] buffer, int size, ref int usedSize)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            if (Head == null) 
                Head = (NetPkgHead)RGameProtocolObjectPool.Get(NetPkgHead.CLASS_ID);

            int start = usedSize;
            ret = Head.pack(ref buffer, size, ref usedSize);
            if (ret != CommError.Type.COMM_NO_ERROR) return ret;

            if(null == Body) return CommError.Type.COMM_ERR_ARGUMENT_NULL_EXCEPTION;

            ret = Body.pack(ref buffer, size, ref usedSize);

            if (ret != CommError.Type.COMM_NO_ERROR) return ret;

            int pack_len = usedSize - start;
            Head.PkgLen = (UInt32)pack_len;

            Head.pack(ref buffer, size, ref start);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="usedSize"></param>
        /// <returns></returns>
        public CommError.Type unpack(ref byte[] buffer, int size, ref int usedSize)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;

            if (Head == null) 
                Head = (NetPkgHead)RGameProtocolObjectPool.Get(NetPkgHead.CLASS_ID);

            ret = Head.unpack(ref buffer, size, ref usedSize);
            if (ret != CommError.Type.COMM_NO_ERROR) return ret;

            if (size < Head.PkgLen) return CommError.Type.COMM_ERR_SHORT_BUF_FOR_READ;

            Body.InitRecvMessage((int)Head.PkgLen);
            ret = Body.unpack(ref buffer, size, ref usedSize);
            if (ret != CommError.Type.COMM_NO_ERROR) return ret;


            return ret;
        }


        public static readonly int CLASS_ID = 0;

        public override int GetClassID()
        {
            return CLASS_ID;
        }
    

        void ClearVariables()
        {
            if(Head!=null)
            {
                Head.Release();
                Head = null;
            }

            if(Body != null)
            {
                Body.Release();
                Body = null;
            }
        }

        public override void OnRelease()
        {
            ClearVariables();
        }

        public override void OnUse()
        {
            ClearVariables();

            Head = (NetPkgHead)RGameProtocolObjectPool.Get(NetPkgHead.CLASS_ID);

            Body = (NetPkgBody)RGameProtocolObjectPool.Get(NetPkgBody.CLASS_ID);
        }
    }
}
