using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 选择端口号的策略
    /// </summary>
    public enum ChooseSvrPolicy
    {
        Null,
        DeviceID,//设备号
        NickName,//昵称
        Random1,//第一次随机
        Random2,//第二次随机
    }

    public class RGameLobbySvrMgr : Singleton<RGameLobbySvrMgr>
    {
        public bool isFirstLogin = false; ///登录成功后只显示一次
                                          ///
        private bool mIsLogin = false;
        public bool IsLogin
        { 
            get { return mIsLogin; }
            set { mIsLogin = true; }
        }
        
        public ChooseSvrPolicy chooseSvrPol = ChooseSvrPolicy.Null;
        public bool canReconnect = true;

        //给到外部调用系统最后一次错误码，方便外部系统处理
        public delegate void ConnectFailHandler();
        public event ConnectFailHandler connectFailHandler;

    //    proto.AccountLoginReq mLoginReq;


        RGameConnectorParam mParam = null;

        //public void Login(proto.AccountLoginReq loginReq, RGameConnectorParam para)
        //{
        //    mParam = para;
        //    mIsLogin = false;

        //    mLoginReq = loginReq;
        //    if (RGameNetworkModule.GetInstance().lobbySrv.Connected)
        //    {
        //        RGameLobbyMsgHandler.SendMsg(proto.ProtoID.ProtoID_LoginReq, mLoginReq);            
        //    }
        //    else
        //    {
        //        ConnectServer();
        //    }
        //}

        public void Ping()
        {
            //proto.LobbySrvPing ping = new proto.LobbySrvPing();
            //ping.dwTime = (UInt32)(UnityEngine.Time.realtimeSinceStartup * 1000f);
            //RGameLobbyMsgHandler.SendMsg(proto.ProtoID.CSID_GAMESVRPING, ping);
        }

        public void SendReconnectCmd()
        {
            //proto.LobbyReconnect msg = new proto.LobbyReconnect();
            //msg.uin = RPlayerSys.instance.Uid;
            //NetPkg pkg = NetPkg.CreateNetReqPkg((int)proto.ProtoID.CSID_LobbyReconnect, msg);

            //RGameNetworkModule.GetInstance().lobbySrv.ImmeSendPackage(pkg);
        }

        private bool ConnectServer()
        {
            if (!RGameNetworkModule.GetInstance().isOnlineMode) return false;


            canReconnect = true;
            if (!mIsLogin)
            {
                RGameNetworkModule.GetInstance().lobbySrv.ConnectedEvent -= onLobbyConnected;
                RGameNetworkModule.GetInstance().lobbySrv.DisconnectEvent -= onLobbyDisconnected;
                RGameNetworkModule.GetInstance().lobbySrv.GetTryReconnect -= OnTryReconnect;
                RGameNetworkModule.GetInstance().lobbySrv.ConnectedEvent += onLobbyConnected;
                RGameNetworkModule.GetInstance().lobbySrv.DisconnectEvent += onLobbyDisconnected;
                RGameNetworkModule.GetInstance().lobbySrv.GetTryReconnect += OnTryReconnect;

                DebugHelper.ConsoleLog("Begin connect game server ip=" + mParam.ip + " port= " + mParam.port);

                bool Result = RGameNetworkModule.GetInstance().InitLobbyServerConnect(mParam);

                DebugHelper.ConsoleLog("connect game server result:" + Result);
                if (Result)
                {
                    CUIManager.GetInstance().OpenSendMsgAlert(null, 10);//转菊花
                }

                return Result;
            }

            if (RGameNetworkModule.GetInstance().lobbySrv!=null)
            {
                RGameNetworkModule.GetInstance().lobbySrv.Reconnect();
                CUIManager.GetInstance().OpenSendMsgAlert(null, 10);//转菊花
            }
                
            return true;
        }

        private uint OnTryReconnect(uint curConnectTime, uint maxcount)
        {
            DebugHelper.Log(string.Format("curConnectTime:{0},maxCount:{1}", curConnectTime, maxcount));

            if (canReconnect)
            {
                if (mIsLogin)
                {
                    ////RGameNetworkModule.instance.lobbySrv.InitParam.SetVip(RGameNetworkModule.test_ip);
                    ////RGameNetworkModule.instance.lobbySrv.InitParam.SetVPort();

                    //if (!Assets.Scripts.GameLogic.LobbyLogic.instance.inMultiGame
                    //    && (Assets.Scripts.GameLogic.BattleLogic.instance.isGameOver || Assets.Scripts.GameLogic.BattleLogic.instance.m_bIsPayStat))
                    //{
                    //    if (curConnectTime > maxcount)
                    //    {
                    //        if (curConnectTime == maxcount + 1)
                    //        {
                    //            Assets.Scripts.GameLogic.LobbyLogic.instance.OnSendSingleGameFinishFail();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        CUIManager.GetInstance().OpenSendMsgAlert("尝试重新连接服务器...", 5);
                    //    }

                    //    return curConnectTime;
                    //}
                    
                    //if (curConnectTime >= maxcount)
                    //{
                    //    ConnectFailed();
                    //}else if (!Assets.Scripts.GameLogic.BattleLogic.instance.isRuning)
                    //{
                    //    CUIManager.GetInstance().OpenSendMsgAlert(null, 10);
                    //}

                    return curConnectTime;
                }

                if(curConnectTime > maxcount)
                {
                    if (curConnectTime == maxcount + 1)
                        ConnectFailed();
                }else
                {
                    CUIManager.GetInstance().OpenSendMsgAlert(null, 10);//转菊花
                }
            }
            return curConnectTime;//根据最新的防止重新连接，需要重连就直接返回curConnectTime
        }

        /// <summary>
        /// 备选连接
        /// </summary>
        private void ConnectServerWithTdirCandidate(int index)
        {
            ConnectServer();
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        private void ConnectFailed()
        {
            PopConfirmingReconnecting();
            if (connectFailHandler != null)
            {
                connectFailHandler();
            }
        }

        /// <summary>
        /// 连接服务器成功
        /// </summary>
        /// <param name="sender"></param>
        private void onLobbyConnected(object sender)
        {
            if(sender is RGameLobbyConnector)
            {
                //RGameLobbyConnector lobbyConn = (RGameLobbyConnector)sender;
                //DebugHelper.Log("onLobbyConnected");
                //if (mLoginReq!=null)
                //{
                //    RGameLobbyMsgHandler.SendMsg(proto.ProtoID.ProtoID_LoginReq, mLoginReq);
                //    mLoginReq = null;
                //}else if(mIsLogin)
                //{
                //    SendReconnectCmd();
                //}
            }
          
            CUIManager.GetInstance().CloseSendMsgAlert();
        }

        private void onLobbyDisconnected(object sender)
        {
            DebugHelper.Log("onLobbyDisconnected");
        }

        private void PopConfirmingReconnecting()
        {
            CUIFormScript msgBoxForm = CUIManager.GetInstance().GetForm(CUIUtility.s_Form_Common_Dir + "Form_MessageBox.prefab");
            if (msgBoxForm == null)
            {
                //CUIManager.GetInstance().OpenMessageBoxWithCancel("重连服务器失败，请检测手机网络环境。"
                //    , enUIEventID.Net_ReconnectConfirm
                //    , enUIEventID.Net_ReconnectCancel);
                //CUIEventManager.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectConfirm, OnConfirmReconnecting);
                //CUIEventManager.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectCancel, OnCancelReconnecting);
            }
        }

        private void OnConfirmReconnecting(CUIEvent uiEvent)
        {
            //CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, OnConfirmReconnecting);
            //CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, OnCancelReconnecting);

            ConnectServerWithTdirCandidate(0);
        }

        private void OnCancelReconnecting(CUIEvent uiEvent)
        {
            //CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, OnConfirmReconnecting);
            //CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, OnCancelReconnecting);
        }
    }

    public class RGameLobbyConnector : RGameBaseConnector
    {
        private static int nBuffSize = 204800;
        private byte[] szSendBuffer = new byte[204800];

        public event NetConnectedEvent ConnectedEvent;
        public event NetDisconnectEvent DisconnectEvent;

        public delegate uint DelegateGetTryReconnect(uint curConnectTime, uint maxCount);
        public DelegateGetTryReconnect GetTryReconnect = null;

        public UInt32 curSvrPkgSeq = 0;
        private UInt32 mCurCltPkgSeq = 0;
        public UInt32 curCltPkgSeq
        { get { return mCurCltPkgSeq; } set { mCurCltPkgSeq = value; } }


        private RGameReconnectPolicy reconPolicy = new RGameReconnectPolicy();

        //发送给大厅服务器的消息队列
        List<NetPkg> mSendQueue = new List<NetPkg>();
        List<NetPkg> mConfirmQueue = new List<NetPkg>();

        ~RGameLobbyConnector()
        {
            DestroyConnector();
            reconPolicy = null;
        }

        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public bool Init(RGameConnectorParam para)
        {
            reconPolicy.SetConnector(this, onTryReconnect, 8);

            return CreateConnector(para);
        }
        public void CustomUpdate()
        {
            if(mConnector!=null)
                mConnector.CustomUpdate();
        }

        public void Reconnect()
        {
            reconPolicy.StopPolicy();
        }

        public void Disconnect()
        {
            DestroyConnector();
            reconPolicy.StopPolicy();
            reconPolicy.SetConnector(null, null, 0);
        }

        /// <summary>
        /// 清空连接上的数据收发
        /// </summary>
        public void CleanUp()
        {
            for (int idx = 0; idx < mSendQueue.Count;idx++)
            {
                mSendQueue[idx].Release();
            }
            mSendQueue.Clear();

            for (int idx = 0; idx < mConfirmQueue.Count;idx++ )
            {
                mConfirmQueue[idx].Release();
            }
            mConfirmQueue.Clear();

            reconPolicy.StopPolicy();

            szSendBuffer.Initialize();

            curSvrPkgSeq = 0;
            curCltPkgSeq = 0;
        }


        /// <summary>
        /// 重置发送缓冲
        /// </summary>
        /// <param name="bResetSeq"></param>
        public void ResetSending(bool bResetSeq)
        {
            mSendQueue.Clear();
            mConfirmQueue.Clear();

            szSendBuffer.Initialize();

            if (bResetSeq)
            {
                curCltPkgSeq = 0;
            }
        }

        /// <summary>
        /// 投递消息
        /// </summary>
        /// <param name="msg"></param>
        public void PushSendMsg(NetPkg msg)
        {
            if (CanPushMsg(msg))
            {
                curCltPkgSeq++;

                msg.Head.Reserve = curCltPkgSeq;


                mSendQueue.Add(msg);
            }
        }

        /// <summary>
        /// 处理待发送的消息
        /// </summary>
        public void HandleSending()
        {
            if (bConnected)
            {
                for (int i = 0; bConnected && i < mSendQueue.Count; )
                {
                    var msg = mSendQueue[i];

                    if (SendPackage(msg))
                    {
                        //if(msg.Head.CmdId!=(uint)proto.ProtoID.CSID_GAMESVRPING)
                        //    DebugHelper.Log(string.Format("Send Cmd:{0}", msg.Head.CmdId));

                        //if (msg.Head.CmdId != (uint)proto.ProtoID.CSID_GAMESVRPING &&
                        //    msg.Head.CmdId != (uint)proto.ProtoID.CSID_LobbyReconnect)
                        //{
                        //    //mConfirmQueue.Add(msg);//jasonbao
                        //}
                        mSendQueue.RemoveAt(i);
                        continue;
                    }

                    i++;
                }
            }
            else
            {
                // 仅当需要发消息时才重连
                reconPolicy.UpdatePolicy(false);
            }
        }

        public bool RedirectNewPort(ushort nPort)
        {
            mInitParam.SetVPort(nPort);

            reconPolicy.SetConnector(this, onTryReconnect, 8);

            return CreateConnector(mInitParam);
        }

        public bool CanSendPing()
        {
            return Connected && mSendQueue.Count == 0 && curSvrPkgSeq > 0;
        }

        private bool CanPushMsg(NetPkg msg)
        {
            if (bConnected)
            {
                return true;
            }
            return true;
            ////这几个频繁拉取的协议如果断网就不要push进来浪费带宽
            //return msg.Head.CmdId != CSProtocol.CSProtocolMacros.CSID_CMD_GET_HORNMSG &&
            //    msg.Head.CmdId != (uint)proto.ProtoID.CSID_GAMESVRPING &&
            //    msg.Head.CmdId != (uint)proto.ProtoID.CSID_LobbyReconnect &&
            //    msg.Head.CmdId != CSProtocol.CSProtocolMacros.CSID_CMD_GET_CHAT_MSG_REQ &&
            //    msg.Head.CmdId != CSProtocol.CSProtocolMacros.CSID_GET_GUILD_RECRUIT_REQ;
        }

        /// <summary>
        /// 尝试重连，返回0表示没有重连次数限制
        /// </summary>
        /// <param name="nCount"></param>
        /// <param name="nMax"></param>
        /// <returns></returns>
        private uint onTryReconnect(uint nCount, uint nMax)
        {
            var tempList = new ListView<NetPkg>();
            for (int i = 0; i < mSendQueue.Count; ++i)
            {
                tempList.Add(mSendQueue[i]);
            }
            mSendQueue.Clear();

            for (int i = 0; i < mConfirmQueue.Count; ++i)
            {
                mSendQueue.Add(mConfirmQueue[i]);
            }
            mConfirmQueue.Clear();

            for (int i = 0; i < tempList.Count; ++i)
            {
                mSendQueue.Add(tempList[i]);
            }

            RGameNetworkModule.GetInstance().m_lobbyReconnetCount++;
            if (GetTryReconnect != null)
            {
                return GetTryReconnect(nCount, nMax);
            }
            else
            {
                return 0;
            }
        }


        #region interface
        /// <summary>
        /// 发送协议包
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool SendPackage(NetPkg msg)
        {
            if (!bConnected || mConnector == null)
            {
                return false;
            }

            msg.Head.SvrPkgSeq = curSvrPkgSeq;

            int nPackSize = 0;
            if (msg.pack(ref szSendBuffer, nBuffSize, ref nPackSize) == CommError.Type.COMM_NO_ERROR)
            {
                byte[] dataBuff = new byte[nPackSize];
                Array.Copy(szSendBuffer, dataBuff, nPackSize);

                return mConnector.WriteData(dataBuff);
            }
            else
            {
                DebugHelper.LogError(string.Format("pack lobby msg fail msgid={0}", msg.Head.CmdId));
            }

            return false;
        }


        public bool ImmeSendPackage(NetPkg msg)
        {
            return SendPackage(msg);
        }

        public void PostRecvPackage(NetPkg msg)
        {
            if (msg != null)
            {
                if (msg.Head.Reserve <= curCltPkgSeq)
                {
                    for (int ix = 0; ix < mConfirmQueue.Count; )
                    {
                        var sndMsg = mConfirmQueue[ix];
                        if (sndMsg.Head.Reserve > 0 && sndMsg.Head.Reserve <= msg.Head.Reserve)
                        {
                            mConfirmQueue[ix].Release();
                            mConfirmQueue.RemoveAt(ix);
                            continue;
                        }
                        ++ix;
                    }
                }
            }
        }

        List<NetPkg> mRecvPkgList = new List<NetPkg>();

        public List<NetPkg> RecvPackage()
        {
            if (bConnected && mConnector != null)
            {
                byte[] tempBuff;
                int bufLen;

                if (mRecvPkgList.Count > 0)
                    DebugHelper.LogError("Error RecvPackage: " + mRecvPkgList.Count);

                if (mConnector.ReadData(out tempBuff, out bufLen))
                {
                    if (bufLen < NetPkgHead.HeadLength) return null;

                    int parseSize = 0;

                    while(true)
                    {

                        if ((bufLen - parseSize) < NetPkgHead.HeadLength) break;

                        NetPkg msg = NetPkg.CreateNetRevPkg();
                        CommError.Type ret = msg.unpack(ref tempBuff, bufLen, ref parseSize);

                        if (ret == CommError.Type.COMM_NO_ERROR)
                        {
                            //if (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_CMD_RELOGINNOW)     //重新登录的消息ID
                            //{
                            //    curSvrPkgSeq = 0;
                            //}


                            ////有效的包
                            //if (msg.Head.SvrPkgSeq > curSvrPkgSeq || msg.Head.SvrPkgSeq == 0)
                            //{
                            //    if (msg.Head.SvrPkgSeq > curSvrPkgSeq && msg.Head.CmdId != CSProtocol.CSProtocolMacros.SCID_OFFINGRESTART_REQ) //server通知client offing重回
                            //    {
                            //        curSvrPkgSeq = msg.Head.SvrPkgSeq;
                            //    }

                            //    mRecvPkgList.Add(msg);
                            //}
                            //else
                            //{
                            //    DebugHelper.LogWarning(string.Format("Lobby Recv out of data package cmdId ={0} curSeq={1}, pkgSeq={2}", msg.Head.CmdId, curSvrPkgSeq, msg.Head.SvrPkgSeq));
                            //}
                        }
                        else
                        {
                            DebugHelper.Assert(false, "TDR Unpack lobbyMsg Error -- {0}", ret);
                        }
                    }
                    return mRecvPkgList;
                }
            }

            return null;
        }
        #endregion

        #region override_base
        protected override void DealConnectSucc(enDealConnectStatus status)
        {
            DebugHelper.Log("Lobby connect success");

            reconPolicy.StopPolicy();
            mLastSuccessIp = mInitParam.ip;
            mLastSuccessPort = mInitParam.port;

            if (ConnectedEvent != null)
                ConnectedEvent(this);

            CUIManager.GetInstance().CloseSendMsgAlert();
        }


        protected override void DealConnectFail()
        {
            reconPolicy.StartPolicy(enNetResult.ConnectFailed, 10);
        }

        protected override void DealConnectClose()
        {
            if (DisconnectEvent != null)
            {
                DisconnectEvent(this);
            }
        }

        protected override void DealConnectError()
        {
            reconPolicy.StartPolicy(enNetResult.Error, 10);
        }
        #endregion
    }
}
