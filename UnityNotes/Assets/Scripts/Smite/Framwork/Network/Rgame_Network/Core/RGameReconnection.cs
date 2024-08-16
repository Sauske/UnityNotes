using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

namespace Framework
{
    public class RGameReconnection : MonoSingleton<RGameReconnection>//, IGameModule
    {
        private ListView<NetPkg> ms_laterPkgList = new ListView<NetPkg>();
        private ListView<NetPkg> ms_cachePkgList = new ListView<NetPkg>();

        private const int nRecvBuffSize = 409600;
        private int nRecvByteSize = 0;
        private byte[] szRecvBuffer = new byte[nRecvBuffSize];
        private const float WAIT_VIDEO_TIME_OUT = 15.0f;

        private int ms_cachedPkgLen = 0; // -1
     //   private uint ms_recvVideoPieceIdx = 0;
        private uint ms_nVideoPieceSeq = 0;
        private bool ms_bWaitingRelaySync = false;       // 是否正在等待同步
        private float ms_fWaitVideoTimeout = 0.0f;
        // 接收录像前段完成，进入录像前段数据播放流程
        private bool ms_bDealRelayCachePkg = false;
    //    private uint ms_nRelayCacheEndFrameNum = 0;
        private bool ms_bProcessingRelaySync = false;    // 是否正在处理同步
        private bool ms_bChaseupGameFrames = false;      // 是否在追帧

        private bool m_bShouldSpin = true;
        private bool m_bUpdateReconnect = false;

        public bool shouldReconnect { get { return m_bShouldSpin; } }

        public float g_fBeginReconnectTime = -1;
        //protected override void Init()
        //{
        //    CUICommonSystem.GetInstance();
        //}

        public void ResetRelaySyncCache()
        {
            nRecvByteSize = 0;
            ms_cachedPkgLen = 0;
        //    ms_recvVideoPieceIdx = 0;
            ms_nVideoPieceSeq = 0;
            ms_bWaitingRelaySync = false;
            ms_fWaitVideoTimeout = 0f;
            ms_bDealRelayCachePkg = false;
            ms_bProcessingRelaySync = false;
            ms_bChaseupGameFrames = false;
            m_bShouldSpin = true;
            m_bUpdateReconnect = true;
         //   ms_nRelayCacheEndFrameNum = 0;

            StopCoroutine("ProcessRelaySyncCache");
            ms_cachePkgList.Clear();
            ms_laterPkgList.Clear();
        }

        //是否在正在处理录像累计数据
        public bool isProcessingRelaySync
        {
            get { return ms_bWaitingRelaySync || ms_bProcessingRelaySync; }
        }

        //是否断线恢复中
        public bool isProcessingRelayRecover
        {
            get { return ms_bWaitingRelaySync || ms_bProcessingRelaySync || ms_bChaseupGameFrames; }
        }

        //是否正在执行录像缓存段数据, 这个时候是没有空白帧的
        public bool isExcuteCacheMsgData
        {
            get { return ms_bDealRelayCachePkg; }
        }


        public bool FilterRelaySvrPackage(NetPkg msg)
        {
            //// 游戏已结算. 开启新游戏之前无须重连
            //if (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_MULTGAME_SETTLEGAIN)//结算
            //{
            //    m_bShouldSpin = false;
            //}

            //if (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_RECONNGAME_NTF)//重连信息
            //{
            //    onReconnectGame(msg);
            //    return true;
            //}

            //if (isProcessingRelaySync)
            //{
            //    if (msg.Head.CmdId == CSProtocol.CSProtocolMacros.SCID_RECOVERGAMEFRAP_RSP)//缓存数据帧
            //    {
            //        DebugHelper.Log("--------------------------------------rev : 缓存数据帧,SCID_RECOVERGAMEFRAP_RSP");
            //        //jasonbao
            //        SCRecoverGameFrap recmsg = RGameUtil.Deserialize<SCRecoverGameFrap>(msg);
            //        if (null == msg)
            //        {
            //            DebugHelper.Log("--------------------------------------rev : 缓存数据帧,SCID_RECOVERGAMEFRAP_RSP,null == msg");
            //            return false;
            //        }
            //        CSPkg msgPkg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.SCID_RECOVERGAMEFRAP_RSP);
            //        if (FrameMsgConverter.Pb2TdrReCoverGameRapNtf(msgPkg, recmsg))
            //        {
            //            DebugHelper.Log("--------------------------------------rev : 缓存数据帧,SCID_RECOVERGAMEFRAP_RSP,onRelaySyncCacheFrames()");
            //            onRelaySyncCacheFrames(msgPkg);//原有的
            //        }
            //        msg.Release();
            //    }
            //    else
            //    {
            //        DebugHelper.Log("--------------------------------------rev : ms_laterPkgList.Add(msg),msg.Head.CmdId = " + msg.Head.CmdId.ToString());
            //        if (msg.Head == null)
            //        {
            //            DebugHelper.Log("--------------------------------------rev : ms_laterPkgList.Add(msg),msg.Head == null");
            //        }
            //        ms_laterPkgList.Add(msg);
            //    }

            //    return true;
            //}

            return false;
        }

        /// <summary>
        /// Todo: Read from local replay file, instead of asking for all from svr, when client crashing
        /// </summary>
        /// <param name="msg"></param>
        private void AddCachePkg(NetPkg msg)
        {
            DebugHelper.Assert(msg != null);
            if (!ms_bDealRelayCachePkg)
            {
                ms_cachePkgList.Add(msg);
            }
        }

        public void UpdateCachedLen(UInt32 dwReserve)
        {
            if (dwReserve > ms_cachedPkgLen)
                ms_cachedPkgLen = (int)dwReserve;
        }
        
        /// <summary>
        /// relay向client发送cilent重连的状态信息
        /// </summary>
        /// <param name="inFrapMsg"></param>
        private void onReconnectGame(NetPkg msg)
        {
            //GamePlayerCenter.instance.HostPlayerId = msg.stPkgData.stReconnGameNtf.dwSelfObjID;

#if AUTO_TESTING
            //if (Tests.TestMachine.instance.isRunning &&
            //    Tests.TestMachine.instance.HandleReconnect(msg)
            //    )
            //{
            //    return;
            //}
#endif
//            SCReconnGameNtf recmsg = RGameUtil.Deserialize<SCReconnGameNtf>(msg);
//            if (null == msg)
//            {
//                return;
//            }
            
//            switch (recmsg.bState)
//            {
//                case CSProtocolMacros.RECONN_RELAY_STATE_BAN:
//                    {
//                        //选将阶段重连
//                        DebugHelper.Log("--------------------------------------send : 选将阶段重连，RECONN_RELAY_STATE_BAN");
//                        //转换协议信息
//                        CSPkg msgPkg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.SCID_RECONNGAME_NTF);
//                        msgPkg.stPkgData.stReconnGameNtf.stStateData.stBanInfo = (CSDT_RECONN_BANINFO)Assets.Scripts.Common.ProtocolObjectPool.Get(CSDT_RECONN_BANINFO.CLASS_ID);

//                        //HeroSelectReconectBanStep(msg.stPkgData.stReconnGameNtf.stStateData.stBanInfo);
//                        CUIManager.GetInstance().CloseSendMsgAlert();
//                    }
//                    break;
//                case CSProtocolMacros.RECONN_RELAY_STATE_PICK:
//                    {
//                        //选将pick阶段重连
//                        DebugHelper.Log("--------------------------------------rev : 选将pick阶段重连,RECONN_RELAY_STATE_PICK");
//                        //转换协议信息

//                        MatchDataMgr.instance.InitMember(recmsg.stStateData.stPickInfo.matchMember);
//                        HeroSelectReconectPickStep(recmsg.stStateData.stPickInfo.updateSelect);
//                        CUIManager.GetInstance().CloseSendMsgAlert();
//                    }

//                    break;
//                case CSProtocolMacros.RECONN_RELAY_STATE_ADJUST:
//                    {
//                        //选将swap阶段重连
//                        DebugHelper.Log("--------------------------------------rev : 选将swap阶段重连，RECONN_RELAY_STATE_ADJUST");
//                        //转换协议信息
//                        CSPkg msgPkg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.SCID_RECONNGAME_NTF);
//                        msgPkg.stPkgData.stReconnGameNtf.stStateData.stAdjustInfo = (CSDT_RECONN_ADJUSTINFO)Assets.Scripts.Common.ProtocolObjectPool.Get(CSDT_RECONN_ADJUSTINFO.CLASS_ID);

//                        //HeroSelectReconectSwapStep(msg.stPkgData.stReconnGameNtf.stStateData.stAdjustInfo);
//                        CUIManager.GetInstance().CloseSendMsgAlert();
//                    }

//                    break;
//                case CSProtocolMacros.RECONN_RELAY_STATE_LOADING:
//                    {
//                        //加载阶段重连
//                        if (LobbyLogic.instance.inMultiGame)
//                        {
//                            DebugHelper.Log("--------------------------------------rev : 加载阶段重连,inMultiGame = ture,RECONN_RELAY_STATE_LOADING");
//                            return;
//                        }
//                        DebugHelper.Log("--------------------------------------rev : 加载阶段重连,inMultiGame = false,RECONN_RELAY_STATE_LOADING");

//                        LobbyLogic.instance.inMultiGame = true;
//                        LobbyLogic.instance.inMultiRoom = true;

//                        //转换协议信息
//                        CSPkg msgPkg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.SCID_RECONNGAME_NTF);
//                        msgPkg.stPkgData.stReconnGameNtf.stStateData.stLoadingInfo = (CSDT_RECONN_LOADINGINFO)Assets.Scripts.Common.ProtocolObjectPool.Get(CSDT_RECONN_LOADINGINFO.CLASS_ID);
//                        if (RLobbyMsgConverter.Pb2TdrMultiGameLoad(msgPkg.stPkgData.stReconnGameNtf.stStateData.stLoadingInfo.stBeginLoad, 
//                            recmsg.stStateData.stLoadingInfo.multGameBeginLoadRes))
//                        {
//                            SCPKG_MULTGAME_BEGINLOAD beginLoadInfo = msgPkg.stPkgData.stReconnGameNtf.stStateData.stLoadingInfo.stBeginLoad;

//                            //ExitMultiGame();
//                            RLobbyMsgHandler.InitMultiGame();//lengbingteng 额外添加的
//                            GameBuilder.instance.StartGame(new MultiGameContext(beginLoadInfo));
//                            CUIManager.GetInstance().CloseSendMsgAlert();
//                         }

//                        //原有的
//                        //if (LobbyLogic.instance.inMultiGame)
//                        //    return;

//                        //LobbyLogic.instance.inMultiGame = true;
//                        //LobbyLogic.instance.inMultiRoom = true;

//                        //SCPKG_MULTGAME_BEGINLOAD beginLoadInfo = msg.stPkgData.stReconnGameNtf.stStateData.stLoadingInfo.stBeginLoad;

//                        //GameBuilder.instance.StartGame(new MultiGameContext(beginLoadInfo));
//                        //CUIManager.GetInstance().CloseSendMsgAlert();
//                    }

//                    break;
//                case CSProtocolMacros.RECONN_RELAY_STATE_GAMEING:
//                    {
//                        if(LobbyLogic.GetInstance().inMultiRoom)
//                            DebugHelper.Log("--------------------------------------rev : 游戏中重连, inMultiRoom = true");
//                        else
//                            DebugHelper.Log("--------------------------------------rev : 游戏中重连, inMultiRoom = false");

//                        //游戏中重连
//                        g_fBeginReconnectTime = Time.time;
//                        LobbyLogic.GetInstance().inMultiRoom = true;
//                        //语音部分
//                        //LobbyLogic.GetInstance().reconnGameInfo = msg.stPkgData.stReconnGameNtf.stStateData.stGamingInfo;
//                        //VoiceSys.instance.SyncReconnectData(LobbyLogic.GetInstance().reconnGameInfo);

//                        RGameReconnection.GetInstance().RequestRelaySyncCacheFrames();
//                        DebugHelper.Log("--------------------------------------rev : 游戏中重连, RECONN_RELAY_STATE_GAMEING,拉取录像");
//                    }

//                    break;
//                case CSProtocolMacros.RECONN_RELAY_STATE_GAMEOVER:
//                    {
//                        DebugHelper.Log("--------------------------------------rev : exit game,RECONN_RELAY_STATE_GAMEOVER");
//                        // exit game
//                        RGameReconnection.GetInstance().ExitMultiGame();
//                    }

//                    break;
//                default:
//                    DebugHelper.Assert(false);
//                    break;
//            }
        }

        ////选将ban阶段重连
        //private void HeroSelectReconectBanStep(CSDT_RECONN_BANINFO banInfo)
        //{
        //    //状态保护还原
        //    //     DebugHelper.CustomLog("HeroSelectReconectBanStep");
        //    GameStateCtrl.GetInstance().GotoState("LobbyState");
        //    CRoomSystem.instance.SetRoomType(CSProtocolMacros.COM_ROOM_TYPE_MATCH);

        //    CSDT_CAMPINFO[] campInfo = new CSDT_CAMPINFO[banInfo.astCampInfo.Length];
        //    for (int i = 0; i < banInfo.astCampInfo.Length; i++)
        //    {
        //        campInfo[i] = new CSDT_CAMPINFO();
        //        campInfo[i].dwPlayerNum = banInfo.astCampInfo[i].dwPlayerNum;
        //        campInfo[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[banInfo.astCampInfo[i].astCampPlayerInfo.Length];
        //        for (int j = 0; j < banInfo.astCampInfo[i].astCampPlayerInfo.Length; j++)
        //        {
        //            campInfo[i].astCampPlayerInfo[j] = banInfo.astCampInfo[i].astCampPlayerInfo[j];
        //        }
        //    }

        //    CHeroSelectBaseSystem.StartPvpHeroSelectSystem(banInfo.stDeskInfo, campInfo, banInfo.stFreeHero, banInfo.stFreeHeroSymbol);
        //    CHeroSelectBaseSystem.instance.m_banPickStep = enBanPickStep.enBan;

        //    if (CHeroSelectBaseSystem.instance.uiType == enUIType.enBanPick)
        //    {
        //        MemberInfo selfInfo = CRoomSystem.instance.roomInfo.GetMasterMemberInfo();

        //        if (selfInfo == null)
        //        {
        //            return;
        //        }

        //        CHeroSelectBaseSystem.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, banInfo.stStateInfo.Camp1BanList);
        //        CHeroSelectBaseSystem.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, banInfo.stStateInfo.Camp2BanList);

        //        CHeroSelectBaseSystem.instance.m_curBanPickInfo.stCurState = banInfo.stStateInfo.stCurState;
        //        CHeroSelectBaseSystem.instance.m_curBanPickInfo.stNextState = banInfo.stStateInfo.stNextState;

        //        //ban上限信息
        //        CHeroSelectBaseSystem.instance.m_banHeroTeamMaxCount = banInfo.stStateInfo.bBanPosNum;

        //        //刷新UI
        //        CHeroSelectBanPickSystem.instance.InitMenu();
        //        CHeroSelectBanPickSystem.instance.RefreshAll();

        //        //播放动画
        //        CHeroSelectBanPickSystem.instance.PlayStepTitleAnimation();
        //        CHeroSelectBanPickSystem.instance.PlayCurrentBgAnimation();

        //        //自己操作提示音播放
        //        if (CHeroSelectBaseSystem.instance.IsCurBanOrPickMember(selfInfo))
        //        {
        //            Utility.VibrateHelper();

        //            //播放提示音
        //            Assets.Scripts.Sound.CSoundManager.GetInstance().PostEvent("UI_MyTurn");
        //            Assets.Scripts.Sound.CSoundManager.GetInstance().PostEvent("Play_sys_ban_3");
        //        }
        //        //我方阵营
        //        else if (CHeroSelectBaseSystem.instance.IsCurOpByCamp(selfInfo))
        //        {
        //            //播放提示音
        //            Assets.Scripts.Sound.CSoundManager.GetInstance().PostEvent("Play_sys_ban_2");
        //        }
        //        //敌方阵营
        //        else if (selfInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
        //        {
        //            //播放提示音
        //            Assets.Scripts.Sound.CSoundManager.GetInstance().PostEvent("Play_sys_ban_1");
        //        }

        //        //播放背景音
        //        Assets.Scripts.Sound.CSoundManager.GetInstance().PostEvent("Play_Music_BanPick");
        //    }
        //}

        ////选将pick阶段重连
        //private void HeroSelectReconectPickStep(SCMatchSelectUpdateSelect pickInfo)
        //{
        //    //状态保护还原
        //    GameStateCtrl.GetInstance().GotoState("LobbyState");

        //    RSelectHeroSys.instance.OpenForm();
        //    RSelectHeroSys.instance.UpdateSelect(pickInfo);

        //}

        ////选将swap阶段重连
        //private void HeroSelectReconectSwapStep(CSDT_RECONN_ADJUSTINFO swapInfo)
        //{
        //    //状态保护还原
        //    //  DebugHelper.CustomLog("HeroSelectReconectSwapStep");
        //    GameStateCtrl.GetInstance().GotoState("LobbyState");
        //    CRoomSystem.instance.SetRoomType(CSProtocolMacros.COM_ROOM_TYPE_MATCH);

        //    CSDT_CAMPINFO[] campInfo = new CSDT_CAMPINFO[swapInfo.astCampInfo.Length];
        //    for (int i = 0; i < swapInfo.astCampInfo.Length; i++)
        //    {
        //        campInfo[i] = new CSDT_CAMPINFO();
        //        campInfo[i].dwPlayerNum = swapInfo.astCampInfo[i].dwPlayerNum;
        //        campInfo[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[swapInfo.astCampInfo[i].astPlayerInfo.Length];
        //        for (int j = 0; j < swapInfo.astCampInfo[i].astPlayerInfo.Length; j++)
        //        {
        //            campInfo[i].astCampPlayerInfo[j] = swapInfo.astCampInfo[i].astPlayerInfo[j].stPickHeroInfo;
        //        }
        //    }

        //    CHeroSelectBaseSystem.StartPvpHeroSelectSystem(swapInfo.stDeskInfo, campInfo, swapInfo.stFreeHero, swapInfo.stFreeHeroSymbol);
        //    CHeroSelectBaseSystem.instance.m_banPickStep = enBanPickStep.enSwap;

        //    //同步确认选择英雄状态
        //    for (int i = 0; i < swapInfo.astCampInfo.Length; i++)
        //    {
        //        for (int j = 0; j < swapInfo.astCampInfo[i].astPlayerInfo.Length; j++)
        //        {
        //            uint tempObjID = swapInfo.astCampInfo[i].astPlayerInfo[j].stPickHeroInfo.stPlayerInfo.dwObjId;
        //            byte tempPrepare = swapInfo.astCampInfo[i].astPlayerInfo[j].bIsPickOK;

        //            MemberInfo mInfo = CHeroSelectBaseSystem.instance.roomInfo.GetMemberInfo(tempObjID);
        //            if (mInfo != null)
        //            {
        //                mInfo.isPrepare = (tempPrepare == 1);
        //            }
        //        }
        //    }

        //    //模拟玩家选择操作执行
        //    MemberInfo selfInfo = CRoomSystem.instance.roomInfo.GetMasterMemberInfo();

        //    if (selfInfo != null)
        //    {
        //        CHeroSelectBaseSystem.instance.SetPvpHeroSelect(selfInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
        //        if (selfInfo.isPrepare)
        //        {
        //            CHeroSelectBaseSystem.instance.m_isSelectConfirm = true;
        //        }
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    //刷新UI
        //    if (CHeroSelectBaseSystem.instance.uiType == enUIType.enNormal)
        //    {
        //        CHeroSelectNormalSystem.GetInstance().m_showHeroID = selfInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;

        //        if (CHeroSelectBaseSystem.instance.selectType == enSelectType.enRandom)
        //        {
        //            CHeroSelectNormalSystem.instance.SwitchSkinMenuSelect();
        //        }

        //        CHeroSelectNormalSystem.instance.RefreshHeroPanel(false, true);
        //        CHeroSelectNormalSystem.instance.StartEndTimer((int)(swapInfo.dwLeftMs / 1000));
        //    }
        //    else if (CHeroSelectBaseSystem.instance.uiType == enUIType.enBanPick)
        //    {
        //        CHeroSelectBaseSystem.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.Camp1BanList);
        //        CHeroSelectBaseSystem.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.Camp2BanList);

        //        //交换信息
        //        CHeroSelectBaseSystem.instance.m_swapInfo.dwActiveObjID = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.dwActiveObjID;
        //        CHeroSelectBaseSystem.instance.m_swapInfo.dwPassiveObjID = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.dwPassiveObjID;
        //        CHeroSelectBaseSystem.instance.m_swapInfo.iErrCode = 0;

        //        //ban上限信息
        //        CHeroSelectBaseSystem.instance.m_banHeroTeamMaxCount = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.bBanPosNum;

        //        if (selfInfo.dwObjId == CHeroSelectBaseSystem.instance.m_swapInfo.dwActiveObjID)
        //        {
        //            CHeroSelectBaseSystem.instance.m_swapState = enSwapHeroState.enReqing;
        //        }
        //        else if (selfInfo.dwObjId == CHeroSelectBaseSystem.instance.m_swapInfo.dwPassiveObjID)
        //        {
        //            CHeroSelectBaseSystem.instance.m_swapState = enSwapHeroState.enSwapAllow;
        //        }

        //        //刷新UI
        //        CHeroSelectBanPickSystem.instance.InitMenu();
        //        CHeroSelectBanPickSystem.instance.RefreshAll();
        //        CHeroSelectBanPickSystem.instance.StartEndTimer((int)(swapInfo.dwLeftMs / 1000));

        //        //播放背景音
        //        Assets.Scripts.Sound.CSoundManager.GetInstance().PostEvent("Set_BanPickEnd");
        //    }
        //}

        /// <summary>
        /// relay向client发送缓存的数据帧
        /// </summary>
        /// <param name="msg"></param>
        private void onRelaySyncCacheFrames(CSPkg inFrapMsg)
        {
            //DebugHelper.Assert(ms_bWaitingRelaySync);
            //if (!ms_bWaitingRelaySync || inFrapMsg.stPkgData.stRecoverFrapRsp.dwCltSeq != ms_nVideoPieceSeq)
            //{
            //    DebugHelper.Log("--------------------------------------onRelaySyncCacheFrames : !ms_bWaitingRelaySync || inFrapMsg.stPkgData.stRecoverFrapRsp.dwCltSeq != ms_nVideoPieceSeq");
            //    DebugHelper.Log("--------------------------------------rev: ms_bWaitingRelaySync : " + ms_bWaitingRelaySync.ToString() + ",dwCltSeq:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwCltSeq.ToString() + ",ms_nVideoPieceSeq:" + ms_nVideoPieceSeq.ToString());
            //    DebugHelper.Log("dwBufLen:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen.ToString() + ",dwTotalNum:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwTotalNum.ToString() + ",dwThisPos:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwThisPos.ToString() + ",dwCurKFrapsNo:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwCurKFrapsNo.ToString() + ",dwCurSvrPkgSeq:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwCurSvrPkgSeq.ToString() + ",dwCltSeq:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwCltSeq.ToString());
            //    //dwBufLen;
            //    //szBuf;
            //    //dwTotalNum;
            //    //dwThisPos;
            //    //dwCurKFrapsNo;
            //    //dwCurSvrPkgSeq;
            //    //dwCltSeq;
            //    return;
            //}

            ////不是我期望的，丢包了
            //if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwThisPos != ms_recvVideoPieceIdx)
            //{
            //    DebugHelper.Log("--------------------------------------onRelaySyncCacheFrames : 丢包," + "dwThisPos:" + inFrapMsg.stPkgData.stRecoverFrapRsp.dwThisPos.ToString() + ",ms_recvVideoPieceIdx:" + ms_recvVideoPieceIdx.ToString());
            //    RequestRelaySyncCacheFrames(true);
            //    return;
            //}

            //ms_recvVideoPieceIdx++;

            //if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen > 0)
            //{
            //    CacheFramesData(inFrapMsg);
            //}

            //if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwTotalNum == ms_recvVideoPieceIdx)
            //{
            //    DebugHelper.Log("--------------------------------------接收录像完毕");
            //    ParseFramesData();

            //    //DebugHelper.Assert(nRecvByteSize == 0);

            //    ms_bWaitingRelaySync = false;
            //    ms_bProcessingRelaySync = true;
            //    ms_nRelayCacheEndFrameNum = inFrapMsg.stPkgData.stRecoverFrapRsp.dwCurKFrapsNo;

            //    StartCoroutine("ProcessRelaySyncCache");
            //}
            //else
            //{
            //    //需要替换为自己的消息
            //    //CSPkg msg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.CSID_RECOVERGAMEFRAP_REQ);
            //    //msg.stPkgData.stRecoverFrapReq.bIsNew = 0;

            //    //NetworkModule.GetInstance().SendGameMsg(ref msg);

            //    //jasonbao
            //    //服务器原来的逻辑会记录发到哪个位置，这里考虑客户端通知对方发到哪个位置。
            //    DebugHelper.Log("--------------------------------------继续请求余下的录像");
            //    DebugHelper.Log("--------------------------------------send: ms_cachedPkgLen: " + ms_cachedPkgLen.ToString() + ",ms_nVideoPieceSeq : " + ms_nVideoPieceSeq.ToString());
            //    RGame.FrameMsgHandler.SendRecoverGameFrap((uint)inFrapMsg.stPkgData.stRecoverFrapRsp.dwCurKFrapsNo, false, ms_nVideoPieceSeq, ms_recvVideoPieceIdx);

            //    ms_fWaitVideoTimeout = WAIT_VIDEO_TIME_OUT;
            //}
        }

        /// <summary>
        /// HandleGameSettle
        /// onBackToHall
        /// </summary>
        private void ExitMultiGame()
        {
            m_bShouldSpin = false;
            m_bUpdateReconnect = false;

            ms_bChaseupGameFrames = false;
         //   Assets.Scripts.GameLogic.GameBuilder.instance.EndGame();

         //   CUIManager.instance.CloseAllFormExceptLobby();
        }


        private IEnumerator ProcessRelaySyncCache()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
            DebugHelper.LogInternal(SLogCategory.Normal, "---BeginParseRelaySync");
#endif

            {
                ms_bDealRelayCachePkg = true;

                for (int i = 0; i < ms_cachePkgList.Count; ++i)
                {
                    NetPkg msg = ms_cachePkgList[i];

                    //while (Assets.Scripts.GameLogic.GameLoader.instance.isLoadStart)
                    //{
                    //    yield return new WaitForEndOfFrame();
                    //}

                    //jasonbao
                    //FrameWindow.onFrapBootInfoSingleNtf(msg);
                    if (msg == null || msg.Head == null)
                    {
                        DebugHelper.LogWarning("--------------------------------------:msg == null || msg.Head == null,  ms_cachePkgList.Count= " + ms_cachePkgList.Count.ToString() + ",i = " + i.ToString());
                    }
                    else
                    {
                        RGameNetworkModule.GetInstance().gameSrv.HandleMsg(msg);
                    }
                    
                    if (i % 500 == 0)
                    {
                        RGameNetworkModule.GetInstance().UpdateFrame();
                    }
                }
#if SGAME_LOG2FILE
                //DebugHelper.LogMisc(string.Format("FrameNum={0}  ProcessRelaySyncCache Fin Later={1} EndFrame={2} SvrFrameIdx={3} cacheLen={4} setSvrKeyIdx={5}",
                //    FrameSynchr.instance.CurFrameNum,
                //    FrameSynchr.instance.SvrFrameLater,
                //    FrameSynchr.instance.EndFrameNum,
                //    FrameSynchr.instance.svrLogicFrameNum,
                //    ms_cachedPkgLen,
                //    ms_nRelayCacheEndFrameNum));
#endif

                ms_bDealRelayCachePkg = false;

                for (int i = 0; i < ms_cachePkgList.Count; i++)
                {
                    ms_cachePkgList[i].Release();
                }

                ms_cachePkgList.Clear();
            }


            {
                for (int j = 0; j < ms_laterPkgList.Count; ++j)
                {
                    NetPkg msg1 = ms_laterPkgList[j];

                    if (msg1 == null || msg1.Head == null)
                    {
                        if (msg1 == null)
                            DebugHelper.LogWarning("--------------------------------------:msg1 == null,  ms_laterPkgList.Count= " + ms_laterPkgList.Count.ToString() + ",j = " + j.ToString());
                        else if(msg1.Head == null)
                            DebugHelper.LogWarning("--------------------------------------:msg1.Head == null,  ms_laterPkgList.Count= " + ms_laterPkgList.Count.ToString() + ",j = " + j.ToString());
                    }
                    else
                        RGameNetworkModule.GetInstance().gameSrv.HandleMsg(msg1);

                    if (j % 500 == 0)
                    {
                        RGameNetworkModule.GetInstance().UpdateFrame();
                    }
                }
                for (int i = 0; i < ms_laterPkgList.Count; i++)
                {
                    ms_laterPkgList[i].Release();
                }
                ms_laterPkgList.Clear();
            }


            // hide blackboard
            CUIManager.GetInstance().CloseSendMsgAlert();


            // 数据同步完成，开始追帧
            DebugHelper.Log("--------------------------------------数据同步完成，开始追帧");
            ms_bChaseupGameFrames = true;
            ms_bProcessingRelaySync = false;

            yield break;
        }


        //private void CacheFramesData(CSPkg inFrapMsg)
        //{
        //    // 读取网络数据
        //    int tempBuffLen = (int)inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen;
        //    byte[] tempBuff = new byte[tempBuffLen];
        //    System.Buffer.BlockCopy(inFrapMsg.stPkgData.stRecoverFrapRsp.szBuf, 0, tempBuff, 0, tempBuffLen);

        //    if (nRecvByteSize + tempBuff.Length > nRecvBuffSize)
        //    {
        //        System.Array.Resize<byte>(ref szRecvBuffer, nRecvByteSize + tempBuff.Length);
        //        System.Buffer.BlockCopy(tempBuff, 0, szRecvBuffer, nRecvByteSize, tempBuff.Length);
        //        nRecvBuffSize = nRecvByteSize + tempBuff.Length;
        //        nRecvByteSize = nRecvBuffSize;
        //    }
        //    else
        //    {
        //        System.Buffer.BlockCopy(tempBuff, 0, szRecvBuffer, nRecvByteSize, tempBuff.Length);
        //        nRecvByteSize += tempBuff.Length;
        //    }
        //}

        private void ParseFramesData()
        {
            // 尝试解包数据
            try
            {
                int parseSize = 0;
                while (true)
                {

                    NetPkg msg = NetPkg.CreateNetRevPkg();
                    CommError.Type ret = msg.unpack(ref szRecvBuffer, nRecvByteSize, ref parseSize);
                    if (ret == CommError.Type.COMM_NO_ERROR)
                    {
                        AddCachePkg(msg);
                    }
                    else
                    { 
                        break; 
                    }
                }

                
                //while (nRecvByteSize > 0)
                //{
                //    int nParseSize = 0;
                //    CSProtocol.CSPkg msg = CSProtocol.CSPkg.New();
                //    TdrError.ErrorType ret = msg.unpack(ref szRecvBuffer, nRecvByteSize, ref nParseSize, 0);
                //    if (ret == TdrError.ErrorType.TDR_NO_ERROR && nParseSize > 0)
                //    {
                //        System.Buffer.BlockCopy(szRecvBuffer, nParseSize, szRecvBuffer, 0, nRecvByteSize - nParseSize);
                //        nRecvByteSize -= nParseSize;

                //        AddCachePkg(msg);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                // try ignore Message
                // BugLocateLogSys.Log("ParseFramesCacheData " + ex.Message);
                DebugHelper.Log("ParseFramesCacheData " + ex.Message);
            }
        }

        /// <summary>
        /// client向relay请求被缓存的数据帧
        /// CSID_RECOVERGAMEFRAP_REQ
        /// </summary>
        /// <returns></returns>
        public bool RequestRelaySyncCacheFrames(bool force = false)
        {
            //if (WatchController.GetInstance().IsRelayCast)
            //    return false;
            if (isProcessingRelayRecover && !force)
                return false;

            StopCoroutine("ProcessRelaySyncCache");
            ms_cachePkgList.Clear();
            ms_laterPkgList.Clear();

         //   ms_nRelayCacheEndFrameNum = 0;
            ms_bDealRelayCachePkg = false;
            ms_bProcessingRelaySync = false;
            ms_bChaseupGameFrames = false;
            m_bShouldSpin = true;
            m_bUpdateReconnect = true;
          //  ms_recvVideoPieceIdx = 0;
            ms_nVideoPieceSeq = (uint)Time.frameCount;
            nRecvByteSize = 0;

            //BHY : 需要替换为自己的消息
            //CSPkg msg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.CSID_RECOVERGAMEFRAP_REQ);
            //msg.stPkgData.stRecoverFrapReq.dwCurLen = (uint) ms_cachedPkgLen;
            //msg.stPkgData.stRecoverFrapReq.bIsNew = 1;
            //msg.stPkgData.stRecoverFrapReq.dwCltSeq = ms_nVideoPieceSeq;

            //NetworkModule.GetInstance().SendGameMsg(ref msg);

            //jasonbao
            DebugHelper.Log("--------------------------------------send: ms_nVideoPieceSeq : " + ms_nVideoPieceSeq.ToString());
         //   RGame.FrameMsgHandler.SendRecoverGameFrap((uint)ms_cachedPkgLen, true, ms_nVideoPieceSeq, ms_recvVideoPieceIdx);

            ms_bWaitingRelaySync = true;
            ms_fWaitVideoTimeout = WAIT_VIDEO_TIME_OUT;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
            DebugHelper.LogInternal(SLogCategory.Normal, "RecordMsgBeginDiscardingBroadcast");
#endif

#if SGAME_LOG2FILE
            //DebugHelper.LogMisc(string.Format("FrameNum={0}  RequestRelaySyncCacheFrames Later={1} EndFrame={2} SvrFrameIdx={3} cacheLen={4}",
            //    FrameSynchr.instance.CurFrameNum,
            //    FrameSynchr.instance.SvrFrameLater,
            //    FrameSynchr.instance.EndFrameNum,
            //    FrameSynchr.instance.svrLogicFrameNum,
            //    ms_cachedPkgLen));
#endif

#if FORCE_LOG
            DebugHelper.Log("断线重连数据恢复中，嗷了个嗷...");
#endif

            // show blackboard
            CUIManager.GetInstance().OpenSendMsgAlert("断线重连数据恢复中，嗷了个嗷...", (int)WAIT_VIDEO_TIME_OUT);

            return true;
        }

        /// <summary>
        /// client向relay上报重连恢复游戏成功
        /// CSID_RECOVERGAMESUCC
        /// </summary>
        /// <returns></returns>
        public bool SendReconnectSucceeded()
        {
            //CSPkg msg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.CSID_RECOVERGAMESUCC);

            //NetworkModule.GetInstance().SendGameMsg(ref msg);
            DebugHelper.Log("--------------------------------------client向relay上报重连恢复游戏成功, send: CSID_RECOVERGAMESUCC()--");
        //    CSReconnGameNtf info = new CSReconnGameNtf();
         //   RGameNetworkModule.GetInstance().SendGameMsg(info, proto.pvp.ProtoID.CSID_RECOVERGAMESUCC);

            return true;
        }

        public void UpdateReconnect()
        {
            if(RGameNetworkModule.GetInstance().isOnlineMode)
            {
                if(m_bShouldSpin && m_bUpdateReconnect)
                {
                    RGameNetworkModule.GetInstance().gameSrv.Update();
                }
            }
        }

        public void UpdateFrame()
        {
            //if (ms_bChaseupGameFrames)//追帧
            //{
            //    if (FrameSynchr.instance.EndFrameNum > 0 && FrameSynchr.instance.CurFrameNum > 0)
            //    {
            //        CUILoadingSystem.OnSelfLoadProcess(0.99f * (float)FrameSynchr.instance.CurFrameNum / (float)FrameSynchr.instance.EndFrameNum);
            //    }

            //    ms_bChaseupGameFrames = FrameSynchr.instance.CurFrameNum < FrameSynchr.instance.EndFrameNum - 15; //断线重回差10帧认为还不能去掉loading遮罩
            //    if (!ms_bChaseupGameFrames)
            //    {
            //        SendReconnectSucceeded();//发送重连成功消息
            //        Assets.Scripts.Common.ProtocolObjectPool.Clear(50);
            //        RGameProtocolObjectPool.Clear(50);
            //        Assets.Scripts.GameLogic.GameEventSys.instance.SendEvent(Assets.Scripts.GameLogic.GameEventDef.Event_MultiRecoverFin);//多人游戏恢复完成
            //    }
            //}

            if (ms_bWaitingRelaySync)
            {
                if (ms_fWaitVideoTimeout > 0.0f)//在发送请求后，重新计时
                {
                    ms_fWaitVideoTimeout -= Time.unscaledDeltaTime;
                    if (ms_fWaitVideoTimeout <= 0.0f)
                    {
                        DebugHelper.Log("--------------------------------------接受缓存帧超时，强制请求缓存帧, send: RequestRelaySyncCacheFrames()");
                        RequestRelaySyncCacheFrames(true);//接受缓存帧超时，强制请求缓存帧
                    }
                }
            }
        }

        private void PopConfirmingReconnecting()
        {
            CUIFormScript msgBoxForm = CUIManager.GetInstance().GetForm(CUIUtility.s_Form_Common_Dir + "Form_MessageBox.prefab");
            if (msgBoxForm == null)
            {
                //m_bUpdateReconnect = false;
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

            ////if (!NetworkModule.instance.gameSvr.connected)
            ////{
            //m_bUpdateReconnect = true;
            //NetworkModule.instance.gameSvr.ForceReconnect();
            //CUIManager.GetInstance().OpenSendMsgAlert("手动重连游戏尝试...", 10);
            ////}
        }

        private void OnCancelReconnecting(CUIEvent uiEvent)
        {
            //CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, OnConfirmReconnecting);
            //CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, OnCancelReconnecting);

            ExitMultiGame();
        }

        public void PopMsgBoxConnectionClosed()
        {
            //CUIManager.GetInstance().OpenMessageBox("游戏已经结束，点击确定返回游戏大厅。", enUIEventID.Net_ReconnectClosed);
            //CUIEventManager.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectClosed, OnConnectionClosedExitGame);
        }

        private void OnConnectionClosedExitGame(CUIEvent uiEvent)
        {
          //  CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectClosed, OnConnectionClosedExitGame);
            ExitMultiGame();
        }

        /// <summary>
        /// 当relay断线要重连时，需要询问大厅自己是否还在游戏中
        /// 因为有可能断得太久已经不在游戏中了，这样会出现反复重连不成功
        /// </summary>
        public void QueryIsRelayGaming(enNetResult result)
        {
            if (!m_bShouldSpin) return;

            //if (result == Apollo.ApolloResult.PeerStopSession ||
            //    result == Apollo.ApolloResult.PeerCloseConnection)
            //{
            //    if (!WatchController.GetInstance().IsWatching || WatchController.GetInstance().IsRelayCast)
            //    {
            //        //CSProtocol.CSPkg reqMsg = NetworkModule.CreateDefaultCSPKG(CSProtocolMacros.CSID_ASKINMULTGAME_REQ);
            //        //NetworkModule.GetInstance().SendLobbyMsg(ref reqMsg);
            //    }
            //}
            //else if (result == Apollo.ApolloResult.AccessTokenExpired)
            //{
            //    //ApolloHelper.GetInstance().Login(ApolloConfig.platform);
            //    //ApolloHelper.GetInstance().ApolloRepoertEvent("AccessTokenExpired", null, true);
            //}
            //else
            //{
            //    m_bUpdateReconnect = true;
            //}

            //if (result == enNetResult.ConnectFailed)//请求大厅服务器询问玩家是否在战斗服务器
            //{
            //    if (!WatchController.GetInstance().IsWatching || WatchController.GetInstance().IsRelayCast)
            //    {
            //        CSAskInMultGameRsp rsp = new CSAskInMultGameRsp();
            //        RGame.RGameLobbyMsgHandler.SendMsg(proto.ProtoID.CSID_ASKINMULTGAME_REQ, rsp);
            //    }
            //}
            //else
            //{
            //    m_bUpdateReconnect = true;
            //}
        }

        //[MessageHandlerAttribute(CSProtocolMacros.SCID_ASKINMULTGAME_RSP)]
        public static void onQueryIsRelayGaming(CSPkg msg)//询问自己是否在房间战斗
        {
            //if (!RGameReconnection.instance.shouldReconnect || WatchController.GetInstance().IsRelayCast)
            //{
            //    return;
            //}

            //if (msg.stPkgData.stAskInMultGameRsp.bYes != 0)
            //{
            //    RGameReconnection.instance.m_bUpdateReconnect = true;
            //}
            //else
            //{
            //    RGameReconnection.instance.PopMsgBoxConnectionClosed();
            //}
        }


        //[MessageHandlerAttribute(CSProtocolMacros.SCID_ASKINMULTGAME_RSP)]
        //public static void onQueryIsRelayGaming(CSPkg msg)
        //{
        //    if (!Reconnection.instance.shouldReconnect || WatchController.GetInstance().IsRelayCast)
        //    {
        //        return;
        //    }

        //    if (msg.stPkgData.stAskInMultGameRsp.bYes != 0)
        //    {
        //        Reconnection.instance.m_bUpdateReconnect = true;
        //    }
        //    else
        //    {
        //        Reconnection.instance.PopMsgBoxConnectionClosed();
        //    }
        //}

        public void OnConnectSuccess()
        {
            //m_bUpdateReconnect = false;
            //CUIManager.GetInstance().CloseSendMsgAlert();	//由GameBuilder.StartGame关闭
        }

        public void ShowReconnectMsgAlert(int nCount, int nMax)
        {
            if (nCount > nMax)
            {
                if (nCount == nMax + 1)
                {
                    RGameReconnection.GetInstance().PopConfirmingReconnecting();
                }
            }
            else
            {
                CUIManager.GetInstance().OpenSendMsgAlert(string.Format("自动重连 第[{0}/{1}]尝试...", nCount, nMax), 10);
            }
        }
    }
}
