using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

namespace Framework
{
    public class RGameGameConnector : RGameBaseConnector
    {
        private int nBuffSize = 204800;
        private byte[] szSendBuffer = new byte[204800];
        private RGameReconnectPolicy reconPolicy = new RGameReconnectPolicy();

        // 发送给Relay服务器的消息队列
        private List<NetPkg> gameMsgSendQueue = new List<NetPkg>();
        private List<NetPkg> confirmSendQueue = new List<NetPkg>();

        // 帧同步命令冗余循环队列
        public const int nCmdRedQueueCount = 3;
     //   private int nCmdRedQueueIndex = 0;
     //   private Assets.Scripts.Framework.IFrameCommand[] cmdRedundancyQueue = new Assets.Scripts.Framework.IFrameCommand[nCmdRedQueueCount];

        private bool netStateChanged = false;
        //private Apollo.NetworkState changedNetState;

        public NetworkReachability curNetworkReachability;


        public Action<enDealConnectStatus> ConnectEvent = null;  // new


        ~RGameGameConnector()
        {
            DestroyConnector();
            reconPolicy = null;
        }

        public void Disconnect()
        {
            //Apollo.ApolloNetworkService.Intance.NetworkChangedEvent -= NetworkStateChanged;

            DestroyConnector();
            reconPolicy.StopPolicy();
            reconPolicy.SetConnector(null, null, 0);
            mInitParam = null;
        }

        public void Update()
        {
            reconPolicy.UpdatePolicy(false);

            if (netStateChanged)
            {
                //if (changedNetState == Apollo.NetworkState.NotReachable)
                //{
                //    CUIManager.GetInstance().OpenSendMsgAlert(CTextManager.GetInstance().GetText("NetworkConnecting"), 10);
                //}
                //else
                //{
                //    CUIManager.GetInstance().CloseSendMsgAlert();
                //}
                netStateChanged = false;
            }
        }

        public void ForceReconnect()
        {
            bConnected = false;
            reconPolicy.UpdatePolicy(true);
        }

        public bool Init(RGameConnectorParam para)
        {
            //设置重连策略参数
            reconPolicy.SetConnector(this, onTryReconnect, 8);
            //Apollo.ApolloNetworkService.Intance.NetworkChangedEvent -= NetworkStateChanged;
            //Apollo.ApolloNetworkService.Intance.NetworkChangedEvent += NetworkStateChanged;
            curNetworkReachability = Application.internetReachability;
            return CreateConnector(para);
        }

        /// <summary>
        /// 消息驱动
        /// </summary>
        public void CustomUpdate()  
        {
            if (mConnector != null)
                mConnector.CustomUpdate();
        }

        public void CleanUp()
        {
            gameMsgSendQueue.Clear();
            reconPolicy.StopPolicy();
            ClearBuffer();
        //    nCmdRedQueueIndex = 0;
        //    Array.Clear(cmdRedundancyQueue, 0, cmdRedundancyQueue.Length);
        }


        private void ClearBuffer()
        {
            szSendBuffer.Initialize();
        }


        private uint onTryReconnect(uint nCount, uint nMax)
        {
            //string urlOrIP = Assets.Scripts.Framework.ReconnectIpSelect.instance.GetConnectUrl(Assets.Scripts.Framework.ConnectorType.Relay, nCount);
            //mInitParam.SetVip(urlOrIP);

            //if (nCount >= 2)//第二次及以后的连接弹出提示
            //{
            //    try
            //    {
            //        RGameReconnection.GetInstance().ShowReconnectMsgAlert((int)nCount - 1, (int)nMax - 1);
            //    }
            //    catch (Exception e)
            //    {
            //        DebugHelper.Assert(false, "Exception In GameConnector Try Reconnect, {0} {1}", e.Message, e.StackTrace);
            //    }
            //}

            RGameNetworkModule.GetInstance().m_GameReconnetCount++;
            return nCount;
        }
        
        //private void NetworkStateChanged(Apollo.NetworkState state)
        //{
        //    changedNetState = state;
        //    netStateChanged = true;
        //}

        protected override void DealConnectSucc(enDealConnectStatus status)
        {
            DebugHelper.Log("Relay connect success !!!");
            reconPolicy.StopPolicy();

           // Assets.Scripts.Framework.ReconnectIpSelect.instance.SetRelaySuccessUrl(mInitParam.ip);

            RGameNetworkModule.GetInstance().LastGameHeartTime = Time.realtimeSinceStartup;

            if (ConnectEvent != null)
                ConnectEvent(status);
        }

        protected override void DealConnectError()
        {
            reconPolicy.StartPolicy(enNetResult.Error, 6);

            if (ConnectEvent != null)
                ConnectEvent(enDealConnectStatus.Deal_Connect_Error);
        }

        protected override void DealConnectFail()
        {
            reconPolicy.StartPolicy(enNetResult.ConnectFailed, 6);

          //  RGameReconnection.GetInstance().QueryIsRelayGaming(enNetResult.ConnectFailed);//询问服务器自己是否还在游戏中

            if (ConnectEvent != null)
                ConnectEvent(enDealConnectStatus.Deal_Connect_Fail);
        }

        protected override void DealConnectClose()
        {
            mRecvPkgList.Clear();

            DebugHelper.LogError("GameConnector ConnectClose");
            if (ConnectEvent != null)
                ConnectEvent(enDealConnectStatus.Deal_Connect_Close);
        }


        #region interface

        public void PushSendMsg(NetPkg msg)
        {
            gameMsgSendQueue.Add(msg);
        }

        public void PushConfirmMsg(NetPkg msg)
        {
         //   bool find = false;
            for (int idx = 0; idx < confirmSendQueue.Count;idx++)
            {
                if(confirmSendQueue[idx].Head.CmdId == msg.Head.CmdId)
                {
                 //   find = true;
                    return;
                }
            }

            confirmSendQueue.Add(msg);
        }

        /// <summary>
        /// 立即发送帧同步命令，通过普通UDP模式，
        /// 带前面输入命令冗余，分CC和CS两种消息
        /// </summary>
        /// <param name="cmd"></param>
        //public void ImmeSendCmd(ref Assets.Scripts.Framework.IFrameCommand cmd)
        //{
        //   // FlushSendCmd(cmd);

        //    ////只有技能施放才冗余
        //    //if (cmd.cmdType == (byte)CSProtocol.CSSYNC_TYPE_DEF.CSSYNC_CMD_USEOBJECTIVESKILL
        //    //    || cmd.cmdType == (byte)CSProtocol.CSSYNC_TYPE_DEF.CSSYNC_CMD_USEDIRECTIONALSKILL
        //    //    || cmd.cmdType == (byte)CSProtocol.CSSYNC_TYPE_DEF.CSSYNC_CMD_USEPOSITIONSKILL
        //    //    || cmd.cmdType == (byte)CSProtocol.CSSYNC_TYPE_DEF.CSSYNC_CMD_BASEATTACK)
        //    //{
        //    //    nCmdRedQueueIndex = ++nCmdRedQueueIndex % nCmdRedQueueCount;
        //    //    cmdRedundancyQueue[nCmdRedQueueIndex] = cmd;
        //    //}
        //}

        //private void FlushSendCmd(Assets.Scripts.Framework.IFrameCommand inCmd)
        //{
        //    Assets.Scripts.Framework.IFrameCommand cmd0 = null;
        //    Assets.Scripts.Framework.IFrameCommand cmd1 = null;
        //    Assets.Scripts.Framework.IFrameCommand cmd2 = null;

        //    if (inCmd != null)
        //    {
        //        cmd0 = inCmd;
        //        cmd1 = cmdRedundancyQueue[nCmdRedQueueIndex];
        //        cmd2 = cmdRedundancyQueue[(nCmdRedQueueIndex - 1 + nCmdRedQueueCount) % nCmdRedQueueCount];
        //    }
        //    else
        //    {
        //        cmd0 = cmdRedundancyQueue[nCmdRedQueueIndex];
        //        cmd1 = cmdRedundancyQueue[(nCmdRedQueueIndex - 1 + nCmdRedQueueCount) % nCmdRedQueueCount];
        //        cmd2 = cmdRedundancyQueue[(nCmdRedQueueIndex - 2 + nCmdRedQueueCount) % nCmdRedQueueCount];
        //    }

        //    if (cmd0 != null && (inCmd != null || cmd0.sendCnt < 3))
        //    {
        //        CSProtocol.CSPkg msg = NetworkModule.CreateDefaultCSPKG(CSProtocol.CSProtocolMacros.CSID_GAMING_UPERMSG);
        //        msg.stPkgData.stGamingUperMsg.bNum = 0;

        //        PackCmd2Msg(ref cmd0, msg.stPkgData.stGamingUperMsg.astUperInfo[msg.stPkgData.stGamingUperMsg.bNum]);
        //        msg.stPkgData.stGamingUperMsg.bNum++;
        //        cmd0.sendCnt++;

        //        if (cmd1 != null && cmd1.sendCnt < 3 && cmd1.frameNum + 10 > Time.frameCount)
        //        {
        //            PackCmd2Msg(ref cmd1, msg.stPkgData.stGamingUperMsg.astUperInfo[msg.stPkgData.stGamingUperMsg.bNum]);
        //            msg.stPkgData.stGamingUperMsg.bNum++;
        //            cmd1.sendCnt++;

        //            if (cmd2 != null && cmd2.sendCnt < 3 && cmd2.frameNum + 10 > Time.frameCount)
        //            {
        //                PackCmd2Msg(ref cmd2, msg.stPkgData.stGamingUperMsg.astUperInfo[msg.stPkgData.stGamingUperMsg.bNum]);
        //                msg.stPkgData.stGamingUperMsg.bNum++;
        //                cmd2.sendCnt++;
        //            }
        //        }

        //        // lengbingteng 转换包体
        //        CSGamingUperMsgAgent agent = ClassObjPool<CSGamingUperMsgAgent>.Get();
        //        if (FrameMsgConverter.Tdr2PbGamingUperMsg(msg.stPkgData.stGamingUperMsg, agent)) 
        //        {
        //            NetPkg netPkg = NetPkg.CreateNetReqPkg((int)proto.pvp.ProtoID.CSID_GAMING_UPERMSG, agent);
        //            SendPackage(netPkg);

        //            netPkg.Release();
        //        }
        //        msg.Release();
        //    }
        //}

        //private void PackCmd2Msg(ref Assets.Scripts.Framework.IFrameCommand cmd, CSProtocol.CSDT_GAMING_UPER_INFO msg)
        //{
        //    // 帧同步命令在上行时frameNum没用，这里复用来记录命令产生时的帧编号
        //    // 帧同步命令在上行方cmdId没用，这里复用来作为CmdSeq了
        //    if (cmd.isCSSync)
        //    {
        //        msg.bType = (byte)CSProtocol.BOOTFRAP_TYPEDEF.BOOTFRAP_TYPE_CS;
        //        msg.dwCmdSeq = cmd.cmdId;
        //        msg.stUperDt.construct(msg.bType);
        //        msg.stUperDt.stCSInfo.stCSSyncDt.construct(cmd.cmdType);

        //        cmd.TransProtocol(msg.stUperDt.stCSInfo);
        //    }
        //    else
        //    {
        //        msg.bType = (byte)CSProtocol.BOOTFRAP_TYPEDEF.BOOTFRAP_TYPE_CC;
        //        msg.dwCmdSeq = cmd.cmdId;
        //        msg.stUperDt.construct(msg.bType);
        //        msg.stUperDt.stCCInfo.construct();

        //        CSProtocol.FRAME_CMD_PKG pkg = FrameCommandFactory.CreateCommandPKG(cmd);
        //        cmd.TransProtocol(pkg);

        //        int packedLen = 0;
        //        tsf4g_tdr_csharp.TdrError.ErrorType err = pkg.pack(
        //            ref msg.stUperDt.stCCInfo.szBuff,
        //            CSProtocol.CSProtocolMacros.CS_CCSYNC_COMMON_BUFF_LEN,
        //            ref packedLen,
        //            0
        //            );
        //        msg.stUperDt.stCCInfo.wLen = (ushort)packedLen;
        //        DebugHelper.Assert(err == tsf4g_tdr_csharp.TdrError.ErrorType.TDR_NO_ERROR);

        //        pkg.Release();
        //    }
        //}

        public void HandleSending()
        {
            if (bConnected)
            {
              //  //重发冗余输入命令
             //   FlushSendCmd(null);

                //重发超时的包
                for (int i = 0; i < confirmSendQueue.Count; ++i)
                {
                    //var msg = confirmSendQueue[i];
                    //if (Assets.Scripts.Framework.GameLogic.instance.GameRunningTick - msg.Head.SvrPkgSeq > 5000)
                    //{
                    //    DebugHelper.Log("Send confirmSendQueue msg: " + msg.Head.CmdId);
                    //    SendPackage(msg);
                    //    msg.Head.SvrPkgSeq = Assets.Scripts.Framework.GameLogic.instance.GameRunningTick;
                    //}
                }

                while (bConnected && gameMsgSendQueue.Count > 0)
                {
                    var msg = gameMsgSendQueue[0];
                    if (SendPackage(msg))
                    {
                        if (msg.Head.Reserve > 0)
                        {
                         //   msg.Head.SvrPkgSeq = Assets.Scripts.Framework.GameLogic.instance.GameRunningTick;
                            confirmSendQueue.Add(msg);
                        }else
                        {
                            msg.Release();
                        }
                        gameMsgSendQueue.RemoveAt(0);
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
              //  RGameReconnection.GetInstance().UpdateReconnect();
            }
        }

        public bool SendPackage(NetPkg msg) 
        {
            if (!bConnected || mConnector == null) 
            {
                return false;
            }

            int nPackSize = 0;
            if (msg.pack(ref szSendBuffer, nBuffSize, ref nPackSize) == CommError.Type.COMM_NO_ERROR)
            {

                //if (mInitParam.bUDP && (msg.Head.CmdId == (uint)proto.pvp.ProtoID.CSID_GAMING_UPERMSG   // CSProtocol.CSProtocolMacros.CSID_GAMING_UPERMSG
                //    || msg.Head.CmdId == (uint)proto.pvp.ProtoID.CSID_RELAYSVRPING))   // CSProtocol.CSProtocolMacros.CSID_RELAYSVRPING))
                //{
                //    return mConnector.WriteUdpData(szSendBuffer, nPackSize);
                //}
                //else
                //{
                //    return mConnector.WriteData(szSendBuffer, nPackSize);
                //}
            }
            else
            {
                DebugHelper.LogError(string.Format("pack game msg fail msgid={0}", msg.Head.CmdId));
            }

            return false;
        }

        List<NetPkg> mRecvPkgList = new List<NetPkg>();
        public List<NetPkg> RecvPackage()
        {
            if (bConnected && mConnector != null)
            {
                if (mRecvPkgList.Count > 0)
                {
                    for (int idx = 0; idx < mRecvPkgList.Count;idx++ )
                    {
                        DebugHelper.LogWarning("Error RecvPackage: " + mRecvPkgList[idx].Head.CmdId);
                    }
                    DebugHelper.LogError("Error RecvPackage: " + mRecvPkgList.Count);
                }

                byte[] tempBuff;
                int bufLen;
                if (mConnector.ReadUdpData(out tempBuff, out bufLen))
                {
                    if(bufLen >= NetPkgHead.HeadLength)
                    {
                        int parseSize = 0;

                        while(true)
                        {
                            if ((bufLen - parseSize) < NetPkgHead.HeadLength) break;

                            NetPkg msg = NetPkg.CreateNetRevPkg();
                            CommError.Type ret = msg.unpack(ref tempBuff, bufLen, ref parseSize);

                            if (ret == CommError.Type.COMM_NO_ERROR)
                            {
                                mRecvPkgList.Add(msg);
                            }else
                            {
                                DebugHelper.LogError(string.Format("unpack relay udp msg error, totoal len={0}", bufLen));
                            }
                        }

                        if (mRecvPkgList.Count > 0)
                            return mRecvPkgList;
                    }
                }

                if (mConnector.ReadData(out tempBuff, out bufLen))
                {
                    // 尝试解包数据
                    if(bufLen >= NetPkgHead.HeadLength)
                    {
                        int parseSize = 0;
                        while(true)
                        {
                            if ((bufLen - parseSize) < NetPkgHead.HeadLength) break;

                            NetPkg msg = NetPkg.CreateNetRevPkg();
                            CommError.Type ret = msg.unpack(ref tempBuff, bufLen, ref parseSize);
                            if (ret == CommError.Type.COMM_NO_ERROR)
                            {
#if AUTO_TESTING
                                //if (GlobalConfig.instance.bSimulateLosePackage)
                                //{
                                //    //jasonbao
                                //    if (UnityEngine.Random.Range(0, GlobalConfig.instance.iSimulateLosePackageNum) == 0 ||
                                //        FrameSynchr.instance.CurFrameNum == 0 ||
                                //        (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_KFRAPLATERCHG_NTF && UnityEngine.Random.Range(0, 2) == 0)
                                //        )
                                //    {
                                //        DebugHelper.LogWarning(string.Format("模拟丢包 msgId={0} pkgSeq={1}", msg.Head.CmdId, msg.Head.SvrPkgSeq));
                                //        return null;
                                //    }
                                //}
#endif
                                // 处理需要回包确认的发送消息
                                for (int ix = 0; ix < confirmSendQueue.Count; )
                                {
                                    var sndMsg = confirmSendQueue[ix];
                                    if (sndMsg.Head.Reserve > 0 && sndMsg.Head.Reserve == msg.Head.CmdId)
                                    {
                                        sndMsg.Release();
                                        confirmSendQueue.RemoveAt(ix);
                                        continue;
                                    }
                                    ++ix;
                                }

                                mRecvPkgList.Add(msg);
                            }
                            else
                            {
                                DebugHelper.LogError(string.Format("unpack relay tcp msg error, totoal len={0}", bufLen));
                            }
                        }
                        return mRecvPkgList;
                    }
                }
            }

            return null;
        }

        public void HandleMsg(NetPkg msg)
        {
            //if (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_MULTGAME_BEGINLOAD)
            //{
            //    MultGameBeginLoadRes ret = RGameUtil.Deserialize<MultGameBeginLoadRes>(msg);
            //    if (null == ret)
            //    {
            //        return;
            //    }

            //    CSProtocol.CSPkg msgPkg = NetworkModule.CreateDefaultCSPKG(CSProtocol.CSProtocolMacros.SCID_MULTGAME_BEGINLOAD);
            //    if (RLobbyMsgConverter.Pb2TdrMultiGameLoad(msgPkg.stPkgData.stMultGameBeginLoad, ret))
            //    {
            //        //缓存
            //        GameReplayModule.instance.CacheRecord(msgPkg);
            //    }
            //}

            //if (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_MULTGAME_BEGINFIGHT)
            //{
            //    DebugHelper.Log("--------------------------------------rev :开战斗:SCID_MULTGAME_BEGINFIGHT");
            //    MultGameBeginFightRes res = RGameUtil.Deserialize<MultGameBeginFightRes>(msg);
            //    if (null == res)
            //    {
            //        return;
            //    }

            //    CSProtocol.CSPkg msgPkg = NetworkModule.CreateDefaultCSPKG(CSProtocol.CSProtocolMacros.SCID_MULTGAME_BEGINFIGHT);

            //    //缓存
            //    GameReplayModule.instance.CacheRecord(msgPkg);
            //}

            //NetMsgDelegate msgHandler = RGameNetworkModule.instance.GetGameMsgHandler(msg.Head.CmdId);
            //if (msgHandler != null)
            //{
            //    msgHandler(msg);
            //}
        }

        //public void HandleMsg(CSProtocol.CSPkg pkg)
        //{
        //    if (pkg.stPkgHead.dwMsgID == CSProtocol.CSProtocolMacros.SCID_MULTGAME_BEGINLOAD ||
        //        pkg.stPkgHead.dwMsgID == CSProtocol.CSProtocolMacros.SCID_MULTGAME_BEGINFIGHT)
        //    {
        //        GameReplayModule.instance.CacheRecord(pkg);
        //    }

        //    NetMsgDelegateForCsPkg msgHandler = RGameNetworkModule.instance.GetGameMsgHandlerForCSPKG(pkg.stPkgHead.dwMsgID);
        //    if (msgHandler != null)
        //    {
        //        msgHandler(pkg);
        //    }
        //}

        #endregion

        public void ClearRecvCache()
        {
            mRecvPkgList.Clear();
        }

    }
}
