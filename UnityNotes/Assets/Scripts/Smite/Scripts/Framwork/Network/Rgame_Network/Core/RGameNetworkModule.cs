using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ProtoBuf;

namespace Framework
{
    public delegate void NetConnectedEvent(object sender);
    public delegate void NetDisconnectEvent(object sender);
    public delegate void NetMsgDelegate(NetPkg msg);
    public delegate void NetMsgDelegateForCsPkg(NetPkg csPkg);//(CSProtocol.CSPkg csPkg);
    public delegate void NetConnectorEvent(string log);

    public class RGameNetworkModule : Singleton<RGameNetworkModule>
    {
        //public static string test_ip = "192.168.30.237";
        //public static ushort test_port = 40003;

        public RGameLobbyConnector lobbySrv = new RGameLobbyConnector();
        public RGameGameConnector gameSrv = new RGameGameConnector();


        Dictionary<UInt32, NetMsgDelegate> mLobbyNetMsgHandlers = new Dictionary<UInt32, NetMsgDelegate>();

        Dictionary<UInt32, NetMsgDelegate> mGameNetMsgHandlers = new Dictionary<uint, NetMsgDelegate>();

        Dictionary<UInt32, NetMsgDelegateForCsPkg> mGameNetMsgHandlersForCSPkg = new Dictionary<uint, NetMsgDelegateForCsPkg>();

        public int m_GameReconnetCount = 0;
        public int m_lobbyReconnetCount = 0;

        // 是否是联网模式
        private bool bOnlineMode = true;
        public bool isOnlineMode
        {
            get { return bOnlineMode; }
            set { bOnlineMode = value; }
        }

        public UInt32 lobbyPing { get; set; }

        private UInt32 m_uiRecvGameMsgCount;
        public UInt32 RecvGameMsgCount
        {
            get { return m_uiRecvGameMsgCount; }
            set { m_uiRecvGameMsgCount = value; }
        }


        // 上一次发送心跳的时间
        private float m_fLastLobbyHeartTime;
        private float m_fLastGameHeartTime;
        public float LastGameHeartTime
        {
            get { return m_fLastGameHeartTime; }
            set { m_fLastGameHeartTime = value; }
        }


        /// <summary>
        /// 网络模块初始化
        /// </summary>
        public override void Init()
        {
            isOnlineMode = true;

            InitLobbyMsgHandler();

            InitGameMsgHandler();

            InitGameMsgHandlerForCSPkg();
        }

        private void InitLobbyMsgHandler()
        {
            ClassEnumerator Enumerator = new ClassEnumerator(
                typeof(RGameMessageHandlerClassAttribute),
                null,
                typeof(RGameNetworkModule).Assembly
                );

            var Iter = Enumerator.Results.GetEnumerator();

            while (Iter.MoveNext())
            {
                var ClassType = Iter.Current;

                MethodInfo[] Methods = ClassType.GetMethods();

                for (int m = 0; Methods != null && m < Methods.Length; ++m)
                {
                    var Method = Methods[m];

                    // only find static functions
                    if (Method.IsStatic)
                    {
                        object[] Attributes = Method.GetCustomAttributes(typeof(RGameMessageHandlerAttribute), true);

                        for (int i = 0; i < Attributes.Length; ++i)
                        {
                            RGameMessageHandlerAttribute Attr = Attributes[i] as RGameMessageHandlerAttribute;

                            if (Attr != null)
                            {
                                RegisterMsgHandler(
                                    Attr.ID,
                                    (NetMsgDelegate)(object)Delegate.CreateDelegate(typeof(NetMsgDelegate), Method),
                                    true);

                                if (Attr.AdditionalIdList != null)
                                {
                                    int additionalIdNum = Attr.AdditionalIdList.Length;
                                    for (int idIndex = 0; idIndex < additionalIdNum; ++idIndex)
                                    {
                                        RegisterMsgHandler(
                                            Attr.AdditionalIdList[idIndex],
                                            (NetMsgDelegate)(object)Delegate.CreateDelegate(typeof(NetMsgDelegate), Method),
                                            true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitGameMsgHandler()
        {
            ClassEnumerator Enumerator = new ClassEnumerator(
                typeof(RGameRelayMessageHandlerClassAttribute),
                null,
                typeof(RGameNetworkModule).Assembly
                );

            var Iter = Enumerator.Results.GetEnumerator();

            while (Iter.MoveNext())
            {
                var ClassType = Iter.Current;

                MethodInfo[] Methods = ClassType.GetMethods();

                for (int m = 0; Methods != null && m < Methods.Length; ++m)
                {
                    var Method = Methods[m];

                    // only find static functions
                    if (Method.IsStatic)
                    {
                        object[] Attributes = Method.GetCustomAttributes(typeof(RGameRelayMessageHandlerAttribute), true);

                        for (int i = 0; i < Attributes.Length; ++i)
                        {
                            RGameRelayMessageHandlerAttribute Attr = Attributes[i] as RGameRelayMessageHandlerAttribute;

                            if (Attr != null)
                            {
                                RegisterMsgHandler(
                                    Attr.ID,
                                    (NetMsgDelegate)(object)Delegate.CreateDelegate(typeof(NetMsgDelegate), Method)
                                    );

                                if (Attr.AdditionalIdList != null)
                                {
                                    int additionalIdNum = Attr.AdditionalIdList.Length;
                                    for (int idIndex = 0; idIndex < additionalIdNum; ++idIndex)
                                    {
                                        RegisterMsgHandler(
                                            Attr.AdditionalIdList[idIndex],
                                            (NetMsgDelegate)(object)Delegate.CreateDelegate(typeof(NetMsgDelegate), Method)
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitGameMsgHandlerForCSPkg()
        {
            ClassEnumerator Enumerator = new ClassEnumerator(
                typeof(RGameRelayMessageHandlerClassAttribute),
                null,
                typeof(RGameNetworkModule).Assembly
                );

            var Iter = Enumerator.Results.GetEnumerator();

            while (Iter.MoveNext())
            {
                var ClassType = Iter.Current;

                MethodInfo[] Methods = ClassType.GetMethods();

                for (int m = 0; Methods != null && m < Methods.Length; ++m)
                {
                    var Method = Methods[m];

                    // only find static functions
                    if (Method.IsStatic)
                    {
                        object[] Attributes = Method.GetCustomAttributes(typeof(RGameRelayMessageHandlerForCSPkgAttribute), true);

                        for (int i = 0; i < Attributes.Length; ++i)
                        {
                            RGameRelayMessageHandlerForCSPkgAttribute Attr = Attributes[i] as RGameRelayMessageHandlerForCSPkgAttribute;

                            if (Attr != null)
                            {
                                RegisterMsgHandlerForCSPkg(
                                    Attr.ID,
                                    (NetMsgDelegateForCsPkg)(object)Delegate.CreateDelegate(typeof(NetMsgDelegateForCsPkg), Method)
                                    );

                                if (Attr.AdditionalIdList != null)
                                {
                                    int additionalIdNum = Attr.AdditionalIdList.Length;
                                    for (int idIndex = 0; idIndex < additionalIdNum; ++idIndex)
                                    {
                                        RegisterMsgHandlerForCSPkg(
                                            Attr.AdditionalIdList[idIndex],
                                            (NetMsgDelegateForCsPkg)(object)Delegate.CreateDelegate(typeof(NetMsgDelegateForCsPkg), Method)
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public NetMsgDelegate GetLobbyMsgHandler(uint msgId)
        {
            NetMsgDelegate msgHandler;

            mLobbyNetMsgHandlers.TryGetValue(msgId, out msgHandler);
            return msgHandler;
        }
        
        public NetMsgDelegate GetGameMsgHandler(uint msgId)
        {
            NetMsgDelegate msgHandler;
            mGameNetMsgHandlers.TryGetValue(msgId, out msgHandler);
            return msgHandler;
        }

        public NetMsgDelegateForCsPkg GetGameMsgHandlerForCSPKG(uint msgId)
        {
            NetMsgDelegateForCsPkg msgHandler;
            mGameNetMsgHandlersForCSPkg.TryGetValue(msgId, out msgHandler);
            return msgHandler;
        }

        /// <summary>
        /// 注册网络消息处理
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="handler"></param>
        public void RegisterMsgHandler(UInt32 cmdID, NetMsgDelegate handler,bool lobbyMsg = false)
        {
            if(lobbyMsg)
            {
                if (mLobbyNetMsgHandlers.ContainsKey(cmdID))
                {
                    DebugHelper.LogError(string.Format("重复的网络事件监听, id={0}", cmdID));
                    return;
                }

                mLobbyNetMsgHandlers.Add(cmdID, handler);
            }
            else
            {
                if(mGameNetMsgHandlers.ContainsKey(cmdID))
                {
                    DebugHelper.LogError(string.Format("重复的网络事件监听, id={0}", cmdID));
                    return;
                }
                mGameNetMsgHandlers.Add(cmdID, handler);
            }
            
        }

        public void RegisterMsgHandlerForCSPkg(UInt32 cmdID, NetMsgDelegateForCsPkg handler)
        {
            if (mGameNetMsgHandlersForCSPkg.ContainsKey(cmdID))
            {
                DebugHelper.LogError(string.Format("重复的网络事件监听, id={0}", cmdID));
                return;
            }
            mGameNetMsgHandlersForCSPkg.Add(cmdID, handler);
        }

        public void RemoveLobbyMsgHandler(UInt32 cmdId)
        {
            if (mLobbyNetMsgHandlers.ContainsKey(cmdId))
            {
                mLobbyNetMsgHandlers.Remove(cmdId);
            }
        }

        public void RemoveGameMsgHandler(UInt32 cmdId)
        {
            if (mGameNetMsgHandlers.ContainsKey(cmdId))
            {
                mGameNetMsgHandlers.Remove(cmdId);
            }

            if(mGameNetMsgHandlersForCSPkg.ContainsKey(cmdId))
            {
                mGameNetMsgHandlersForCSPkg.Remove(cmdId);
            }
        }


        /// <summary>
        /// 连接大厅服务器，单人模式只需要大厅服务器就够了
        /// </summary>
        /// <returns>是否成功</returns>
        public bool InitLobbyServerConnect(RGameConnectorParam para)
        {
            m_lobbyReconnetCount = 0;
            isOnlineMode = true;
            return lobbySrv.Init(para);
        }

        /// <summary>
        /// 连接中转服务器，多人模式需要集中收集分发帧同步消息
        /// </summary>
        /// <returns></returns>
        public bool InitGameServerConnect(RGameConnectorParam para)
        {
            m_GameReconnetCount = 0;
            //RGameReconnection.GetInstance().ResetRelaySyncCache();
            //Assets.Scripts.Framework.FrameWindow.GetInstance().Reset();
            return gameSrv.Init(para);
        }

        /// <summary>
        /// 重置大厅连接的发送队列
        /// </summary>
        public void ResetLobbySending()
        {
            lobbySrv.ResetSending(true);
        }

        public void CloseAllServerConnect()
        {
            CloseLobbyServerConnect();
            CloseGameServerConnect();
        }

        public void CloseLobbyServerConnect()
        {
            lobbySrv.CleanUp();
            lobbySrv.Disconnect();
            lobbyPing = 0;
            m_lobbyReconnetCount = 0;
        }

        public void CloseGameServerConnect(bool switchLocal = true)
        {
            //if (switchLocal)
            //{
            //    Assets.Scripts.Framework.FrameSynchr.instance.SwitchSynchrLocal();
            //}
            //RGameReconnection.GetInstance().ResetRelaySyncCache();
            //Assets.Scripts.Framework.FrameWindow.GetInstance().Reset();
            gameSrv.CleanUp();
            gameSrv.Disconnect();
            m_GameReconnetCount = 0;
        }

        public bool SendLobbyMsg(IExtensible msgBody, ProtoID cmdId, bool isShowAlert = false)
        {
            NetPkg msg = NetPkg.CreateNetReqPkg((int)cmdId, msgBody);
            return SendLobbyMsg(ref msg, isShowAlert);
        }

        public  bool SendLobbyMsg(ref NetPkg msg, bool isShowAlert = false)
        {
            if (isOnlineMode)
            {
                if (isShowAlert)
                {
                    ////是否显示网络请求提示窗口
                    //if (!Assets.Scripts.GameLogic.BattleLogic.instance.isRuning)
                    //{
                    //    CUIManager.GetInstance().OpenSendMsgAlert();
                    //}
                }

                lobbySrv.PushSendMsg(msg);

                return true;
            }

            return false;
        }

        public bool SendGameMsg(PBAgent protolBufAgent, ProtoID cmdId, uint confirmMsgID = 0)
        {
            //NetPkg msg = NetPkg.CreateNetReqPkg((int)cmdId, msgBody);
            //return SendGameMsg(ref msg, confirmMsgID);
            return false;
        }

        public bool SendGameMsg(IExtensible msgBody, ProtoID cmdId, uint confirmMsgID = 0)
        {
            NetPkg msg = NetPkg.CreateNetReqPkg((int)cmdId, msgBody);
            return SendGameMsg(ref msg, confirmMsgID);
        }

        public bool SendGameMsg(ref NetPkg msg, uint confirmMsgID = 0)
        {
            if (isOnlineMode)
            {
                if (gameSrv.Connected)
                {
                    msg.Head.Reserve = confirmMsgID;
                    gameSrv.PushSendMsg(msg);
                    return true;
                }else
                {
                    if (confirmMsgID > 0)
                    {
                        msg.Head.Reserve = confirmMsgID;
                       // msg.Head.SvrPkgSeq = Assets.Scripts.Framework.GameLogic.instance.GameRunningTick;
                        gameSrv.PushConfirmMsg(msg);
                    }
                }       
            }
            return false;
        }

        /// <summary>
        /// 高频率调用网络收发
        /// </summary>
        public void UpdateFrame()
        {
            if (isOnlineMode)
            {
                UpdateLobbyConnection();

                UpdateGameConnection();

                try
                {
                    HandleLobbyMsgSend();
                }
                catch (Exception e)
                {
                    DebugHelper.Assert(false, "Error In HandleLobbyMsgSend: {0}, Call stack : {1}", e.Message, e.StackTrace);
                }


                try
                {
                    HandleGameMsgSend();
                }
                catch (Exception e)
                {
                    DebugHelper.Assert(false, "Error In HandleGameMsgSend: {0}, Call stack : {1}", e.Message, e.StackTrace);
                }


#if AUTO_TESTING && ENABLE_LOGSYNCHR
                HandleLoggerMsgSend();
#endif

                try
                {
                    HandleLobbyMsgRecv();
                }
                catch (Exception e)
                {
                    DebugHelper.Assert(false, "Error In HandleLobbyMsgRecv: {0}, Call stack : {1}", e.Message, e.StackTrace);
                }


                try
                {
                    HandleGameMsgRecv();
                }
                catch (Exception e)
                {
                    DebugHelper.Assert(false, "Error In HandleGameMsgRecv: {0}, Call stack : {1}", e.Message, e.StackTrace);
                }


#if AUTO_TESTING && ENABLE_LOGSYNCHR
                HandleLoggerMsgRecv();                
#endif
            }
        }

        private void UpdateLobbyConnection()
        {
            if (lobbySrv!=null)
            {
                lobbySrv.CustomUpdate();

                // 发送心跳包
                if (lobbySrv.CanSendPing() && Time.realtimeSinceStartup - m_fLastLobbyHeartTime > 5.0f)
                {
                    m_fLastLobbyHeartTime = Time.realtimeSinceStartup;

                    RGameLobbySvrMgr.GetInstance().Ping();
                }
            }
        }

        private void UpdateGameConnection()
        {
            //SProfiler.BeginSample("UpdateGameConnection");
            //RGameReconnection.GetInstance().UpdateFrame();
            //SProfiler.EndSample();

            if(gameSrv!=null)
            {
                gameSrv.CustomUpdate();

                // 发送ping包
                if (gameSrv.Connected && Time.realtimeSinceStartup - m_fLastGameHeartTime > 3.0f)
                {
                    m_fLastGameHeartTime = Time.realtimeSinceStartup;

                    // 发送新的心跳包协议
                    //RLobbyMsgHandler.SendRelaySvrPing((UInt32)(Time.realtimeSinceStartup * 1000f), (UInt32)Assets.Scripts.Framework.FrameSynchr.GetInstance().m_SendHeartSeq);
                    //CSPkg msg = CreateDefaultCSPKG(CSProtocolMacros.CSID_RELAYSVRPING);
                    //msg.stPkgData.stRelaySvrPing.dwTime = (UInt32)(Time.realtimeSinceStartup * 1000f);
                    //msg.stPkgData.stRelaySvrPing.dwSeqNo = (uint)Assets.Scripts.Framework.FrameSynchr.GetInstance().m_SendHeartSeq;
                    //Assets.Scripts.Framework.FrameSynchr.GetInstance().m_SendHeartSeq++;
                    //gameSrv.PushSendMsg(msg);
                }
            }
            
        }

        /// <summary>
        /// 发送缓存的大厅消息
        /// </summary>
        private void HandleLobbyMsgSend()
        {
            if (lobbySrv!=null && isOnlineMode)
            {
                lobbySrv.HandleSending();
            }
        }

        /// <summary>
        /// 尝试从大厅服务器收消息
        /// </summary>
        private void HandleLobbyMsgRecv()
        {
            if (lobbySrv != null)
            {
                List<NetPkg> msgList = lobbySrv.RecvPackage();
                while (msgList != null && msgList.Count > 0)
                {
                    for( int idx =0; idx < msgList.Count;idx++)
                    {
                        NetPkg msg = msgList[idx];
                        NetMsgDelegate msgHandler = null;

                        UInt32 msgId = msg.Head.CmdId;

                        if(msgId != (uint)ProtoID.CSID_GAMESVRPING)
                            DebugHelper.LogWarning(" Receive Lobby Cmd ID = " + msgId);

                        if (mLobbyNetMsgHandlers.TryGetValue(msgId, out msgHandler))
                        {
                            msgHandler(msg);
                        }

                        lobbySrv.PostRecvPackage(msg);
                    }

                    for (int idx = 0; idx < msgList.Count; idx++)
                        msgList[idx].Release();

                    msgList.Clear();

                    msgList = lobbySrv.RecvPackage();
                }
            }
        }

        private void HandleGameMsgSend()
        {
          //  SProfiler.BeginSample("HandleGameMsgSend");

            if (isOnlineMode && gameSrv!=null)
            {
                gameSrv.HandleSending();
            }

          //  SProfiler.EndSample();
        }

        Queue<NetPkg> msgList = new Queue<NetPkg>();
        private void HandleGameMsgRecv()
        {
            if (gameSrv != null)
            {
                List<NetPkg> temp = gameSrv.RecvPackage();
                if(temp!=null && temp.Count > 0)
                {
                    for (int idx = 0; idx < temp.Count; idx++)
                    {
                        msgList.Enqueue(temp[idx]);
                    }
                    temp.Clear();
                }
             
                while(msgList.Count > 0)
                {
                   // NetPkg msg = msgList.Dequeue();
                    m_uiRecvGameMsgCount++;

                    ////DebugHelper.Log(" Receive Game Cmd ID = " + msg.Head.CmdId);
                    //if (!RGameReconnection.GetInstance().FilterRelaySvrPackage(msg))
                    //{// 如果断线恢复不过滤，说明是正常流程的消息，直接处理
                    //    gameSrv.HandleMsg(msg);
                    //    msg.Release();
                    //}

                    //msg.Release();
                }
            }
        }

         /// <summary>
        /// 初始化同步服务器链接
        /// </summary>
        //public static void InitRelayConnnecting()
        //{
        //    RGameConnectorParam param = new RGameConnectorParam();
        //    param.bUDP = false;
        //    param.ip = "192.168.30.237";
        //    param.port = 40009;

        //    GetInstance().InitGameServerConnect(param);
        //}

         public static NetPkg CreateDefaultNetPKG(ProtoID cmdId,IExtensible msgBody)
         {
             NetPkg pkg = NetPkg.CreateNetReqPkg((int)cmdId, msgBody);
             return pkg;
         }
    }
}
