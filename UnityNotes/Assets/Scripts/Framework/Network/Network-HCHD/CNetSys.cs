using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;
using System;

namespace UMI.Net
{

    public class CNetSys : Singleton<CNetSys>
    {

        public static DateTime m_PvpPingTime = DateTime.Now;
        public bool m_useUDP = true;
        private NavtiveNetwork mNetWork;
        private RUDPClient mRudpClient;
        public class ServerInfo
        {
            public string ip;
            public int port;
        }

        public Dictionary<EConnection_Type, ServerInfo> m_serverInfo = new Dictionary<EConnection_Type, ServerInfo>
    {
        {
            EConnection_Type.EAccount_Connection,
            new ServerInfo()
            {
                ip = "106.75.135.220",
                port = 15001,
            }
        },
        {
            EConnection_Type.EDir_Connection,
            new ServerInfo()
            {
                ip = "106.75.135.220",
                port = 40011,
            }
        },
        {
            EConnection_Type.EVer_Connection,
            new ServerInfo()
            {
                ip = "192.168.2.20",
                port = 10002,
            }
        },
        {
            EConnection_Type.EZone_Connection,
            new ServerInfo()
            {
                ip = "192.168.2.36",
                port = 10001,
            }
        },
        {
            EConnection_Type.EPvp_Connection,
            new ServerInfo()
            {
                ip = "192.168.2.36",
                port = 5000,
            }
        }
    };

        public void SetServerInfo(EConnection_Type type, string strIP, int nPort)
        {
            if (m_serverInfo.ContainsKey(type))
            {
                m_serverInfo[type].ip = strIP;
                m_serverInfo[type].port = nPort;
            }
        }

        private static CNetSys m_cNetSys = null;
        public static CNetSys Instance { get { return m_cNetSys; } }
        public override void RegEvent()
        {
            CEventSys.Instance.RegEvent((int)ESysEvent.AwakeFromPauseStart, OnAwakeFromePauseStart);
            CEventSys.Instance.RegEvent((int)ESysEvent.GamePause, OnGamePause);
        }

        private DateTime m_gamePauseTime;
        public bool OnGamePause(IEvent evt)
        {
            m_gamePauseTime = DateTime.Now;
            return false;
        }

        public bool OnAwakeFromePauseStart(IEvent evt)
        {
            if (IsZoneConnected())
            {
                TimeSpan ts = DateTime.Now - m_gamePauseTime;
                if (ts.TotalSeconds > 60)
                {
                    CloseConnection(EConnection_Type.EZone_Connection);
                    CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.ZoneReConnect));
                }
            }

            if (IsPvpSvrConnected())
            {
                TimeSpan ts = DateTime.Now - m_gamePauseTime;
                if (ts.TotalSeconds > 60)
                {
                    DisConnectPvpServer();
                    CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.PvpConnectStatus));
                }
            }
            return false;
        }

        public override void SysInitial()
        {
            base.SysInitial();
            m_lastInternetReachability = Application.internetReachability;
            if (mNetWork == null)
            {
                mNetWork = new NavtiveNetwork();
            }
            m_cNetSys = this;
            GameConfig gameCfg = CResourceSys.Instance.gameCfg;
            if (gameCfg != null)
            {
                ServerInfo svInfo = m_serverInfo[EConnection_Type.EAccount_Connection];
                svInfo.ip = gameCfg.accountSvrIp;
                svInfo.port = gameCfg.accountSvrPort;

                svInfo = m_serverInfo[EConnection_Type.EDir_Connection];
                svInfo.ip = gameCfg.accountSvrIp;
                svInfo.port = gameCfg.accountSvrPort;
            }
            //ConnectDirServer();
        }

        protected NetworkReachability m_lastInternetReachability = NetworkReachability.NotReachable;
        private void CheckInternetReachability()
        {
            if (Application.internetReachability != m_lastInternetReachability)
            {
                if (mNetWork != null)
                {
                    mNetWork.OnNetWorkChange(m_lastInternetReachability, Application.internetReachability);
                }

                if (mRudpClient != null)
                {
                    mRudpClient.OnNetWorkChange(m_lastInternetReachability, Application.internetReachability);
                }
                //DisConnect();
                m_lastInternetReachability = Application.internetReachability;
            }
        }

        public override void SysUpdate()
        {
            mNetWork.Update();
            CheckInternetReachability();
        }

        public override void SysLogicFrameTurn()
        {
            mNetWork.LogicFrameTurn();
            if (mRudpClient != null)
            {
                mRudpClient.Update();
            }
        }

        public void SendZoneMsg(IExtensible msgBody, int Cmd)
        {
            SendZoneMsg(msgBody, Cmd, 0);
        }

        public void SendZoneMsgWithMask(IExtensible msgBody, int Cmd, int resCmd)
        {
            SendZoneMsg(msgBody, Cmd);

            StopCoroutine("SendZoneMsgWithMaskCo");
            StartCoroutine("SendZoneMsgWithMaskCo", resCmd);
        }

        protected IEnumerator SendZoneMsgWithMaskCo(int resCmd)
        {
            CEvtWaiter waitNet = new CEvtWaiter(resCmd);
            yield return StartCoroutine(CEventSys.Instance.WaitEvent(waitNet));
            if (waitNet.mState == EEvtWaiterState.OutOfTime)
            {
                CUility.Log("没有接受到回包：{0}", resCmd);
            }
        }

        public void SendZoneMsg(IExtensible msgBody, int Cmd, int cryptType)
        {
            if (!mNetWork.IsConnected(EConnection_Type.EZone_Connection))
            {
                CUility.Log("Zone Server is not Connected! ");
                return;
            }

            mNetWork.SendMsg(msgBody, Cmd, cryptType, EConnection_Type.EZone_Connection);
        }

        public void ConnectDirServer()
        {
            ConnectServer(EConnection_Type.EDir_Connection);
        }
        public void ConnectAccountServer()
        {
            ConnectServer(EConnection_Type.EAccount_Connection);
        }
        public void ConnectZoneServer()
        {
            CUility.Log("ConectToZoneServer:" + m_serverInfo[EConnection_Type.EZone_Connection].ip);
            ConnectServer(EConnection_Type.EZone_Connection);
        }

        public void ConnectServer(EConnection_Type connectType)
        {
            ServerInfo svrInfo = null;
            if (m_serverInfo.TryGetValue(connectType, out svrInfo))
            {
                mNetWork.CreateConnection(connectType, svrInfo.ip, svrInfo.port);
            }
        }

        public void SendAccountMsg(IExtensible msgBody, int Cmd)
        {
            if (!mNetWork.IsConnected(EConnection_Type.EAccount_Connection))
            {
                CUility.Log("Account Server is not Connected!! ");
                return;
            }

            mNetWork.SendMsg(msgBody, Cmd, 1, EConnection_Type.EAccount_Connection);
        }

        public void SendDirMsg(IExtensible msgBody, int Cmd)
        {
            if (!mNetWork.IsConnected(EConnection_Type.EDir_Connection))
            {
                ConnectDirServer();
                CUility.Log("Dir Server is not Connected!! ");
                return;
            }

            mNetWork.SendMsg(msgBody, Cmd, 1, EConnection_Type.EDir_Connection);
        }
        public int GetZoneNetworkDelay()
        {
            return mNetWork.GetNetworkDelay(EConnection_Type.EZone_Connection);
        }
        public int GetPvpNetWorkDelay()
        {
            if (mRudpClient != null)
            {
                return mRudpClient.netDelays;
            }
            return mNetWork.GetNetworkDelay(EConnection_Type.EPvp_Connection);
            //return mNetWork.GetNetworkDelay(EConnection_Type.EPvp_Connection);
        }
        public bool IsZoneConnected()
        {
            return mNetWork.IsConnected(EConnection_Type.EZone_Connection);
        }
        public bool IsDirConnected()
        {
            return mNetWork.IsConnected(EConnection_Type.EDir_Connection);
        }
        public bool IsConnected(EConnection_Type connectType)
        {
            return mNetWork.IsConnected(connectType);
        }

        public bool IsPvpSvrConnected()
        {
            if (mRudpClient != null)
            {
                return mRudpClient.IsConnected();
            }
            return IsConnected(EConnection_Type.EPvp_Connection);
        }

        public int GetNetworkDelay()
        {
            return GetZoneNetworkDelay();
        }

        /* Close */
        public void CloseConnection(EConnection_Type connectType)
        {
            mNetWork.CloseConnection(connectType);
        }
        public void CloseZoneConnection()
        {
            CloseConnection(EConnection_Type.EZone_Connection);
        }
        public void CloseDirConnection()
        {
            CloseConnection(EConnection_Type.EDir_Connection);
        }
        public void CloseAccountConnection()
        {
            CloseConnection(EConnection_Type.EAccount_Connection);
        }


        public override void SysFinalize()
        {
            //if (IsConnected(EConnection_Type.EZone_Connection) && AccountSys.Instance.isLogined)
            //{
            //    AccountSys.Instance.ReqAccountLoginOut();
            //}

            if (mNetWork != null)
            {
                mNetWork.StopRecvAndSend();
                mNetWork.WaitTermination();
                mNetWork.CloseAll();
            }

            if (mRudpClient != null)
            {
                mRudpClient.DisConnect();
                mRudpClient.Destroy();
            }
            base.SysFinalize();
        }

        public delegate void DMsgReceiveCallback(bool success, CNetSysEvent netEvt);
        public void SendZoneMsgSync(IExtensible msgBody, int Cmd)
        {
            StartCoroutine(SendZoneMsgSyncCo(msgBody, Cmd, 0, null));
        }

        public void SendZoneMsgSync(IExtensible msgBody, int Cmd, int reciveMsgId, DMsgReceiveCallback callBack)
        {
            StartCoroutine(SendZoneMsgSyncCo(msgBody, Cmd, reciveMsgId, callBack));
        }

        private const float fWaitTime = 10.0f;
        protected IEnumerator SendZoneMsgSyncCo(IExtensible msgBody, int Cmd, int reciveMsgId, DMsgReceiveCallback callBack)
        {
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    //MessageTipSys.Instance.ShowMessageTip("无网络连接，请检查您的网络状态并重试！");
            //    callBack(false, null);
            //    yield break;
            //}

            //using (MaskHold maskHold = new MaskHold(0.5f))
            //{
            //CMaskSys.Instance.ShowMask(0.5f);
            //if (!AccountSys.Instance.isLogined)
            //{
            //    CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.ZoneReConnect));
            //    float fStartTime = Time.time;
            //    int nCount = 4;
            //    while (!AccountSys.Instance.isLogined && nCount > 0)
            //    {
            //        if (Time.time - fStartTime > fWaitTime)
            //        {
            //            nCount--;
            //            CMaskSys.Instance.ShowTip();
            //            CEventSys.Instance.TriggerEvent(new CNetStatusEvent(ENetStatusEvent.ZoneReConnect));
            //            fStartTime = Time.time;
            //            //MessageTipSys.Instance.ShowMessageTip("网络异常，请检查您的网络连接状态是否通畅并重试！");
            //            //Debug.LogError("网络异常，发包超时");
            //            //yield break;
            //        }
            //        yield return null;
            //    }
            //    if (nCount == 0)
            //    {
            //        CMaskSys.Instance.ShowWindow(delegate()
            //        {
            //            CMaskSys.Instance.HideMask();
            //            SendZoneMsgSync(msgBody, Cmd, reciveMsgId, callBack);
            //        });
            //        yield break;
            //    }
            //}

            //mNetWork.SendMsg(msgBody, Cmd, 0, EConnection_Type.EZone_Connection);

            //if (reciveMsgId == 0)
            //{
            //    CMaskSys.Instance.HideMask();
            //    yield break;
            //}

            //CEvtWaiter waitNet = new CEvtWaiter(reciveMsgId);
            //StartCoroutine(CEventSys.Instance.WaitEvent(waitNet));
            ////float fTimeStart = Time.time;
            //while (waitNet.isWaiting)
            //{
            //    if (!AccountSys.Instance.isLogined)
            //    {
            //        CMaskSys.Instance.HideMask();
            //        SendZoneMsgSync(msgBody, Cmd, reciveMsgId, callBack);
            //        yield break;
            //    }

            yield return null;
            //}

            //if (waitNet.mState == EEvtWaiterState.OutOfTime)
            //{
            //    if (callBack != null)
            //    {
            //        callBack(false, null);
            //    }
            //    MessageTipSys.Instance.ShowMessageTip("网络请求超时，请重试！");
            //    Debug.LogError("发包超时, msgId=" + Cmd.ToString());
            //}
            //else
            //{
            //    if (callBack != null)
            //    {
            //        callBack(true, (CNetSysEvent)waitNet.mEvt);
            //    }
            //}
            //CMaskSys.Instance.HideMask();
        }

        public void SendPvpMsg(IExtensible msgBody, int cmd)
        {
            if (m_useUDP)
            {
                if (mRudpClient != null)
                {
                    mRudpClient.SendMsg(msgBody, cmd, 1, EConnection_Type.EPvp_Connection);
                }
            }
            else
            {
                if (!mNetWork.IsConnected(EConnection_Type.EPvp_Connection))
                {
                    CUility.Log(string.Format("{0} is not connected", EConnection_Type.EPvp_Connection.ToString()));
                }
                mNetWork.SendMsg(msgBody, cmd, 1, EConnection_Type.EPvp_Connection);
            }

        }

        public void ConnectPvpServer(string ip, int port)
        {
            if (mRudpClient == null)
            {
                mRudpClient = new RUDPClient();
                mRudpClient.Init();
            }

            if (m_useUDP)
            {
                mRudpClient.Connect(ip, port);
            }
            else
            {
                SetServerInfo(EConnection_Type.EPvp_Connection, ip, port);
                ConnectServer(EConnection_Type.EPvp_Connection);
            }
        }

        public void DisConnectPvpServer()
        {
            if (m_useUDP)
            {
                if (mRudpClient != null)
                {
                    mRudpClient.DisConnect();
                }
            }
            else
            {
                mNetWork.CloseConnection(EConnection_Type.EPvp_Connection);
            }
        }

        public DateTime GetCurrentSvrTime(EConnection_Type connType)
        {
            return mNetWork.GetCurrentSvrTime(connType);
        }

        public DateTime GetCurrentPvpSvrTime()
        {
            if (mRudpClient != null)
            {
                return mRudpClient.curSvrTime();
            }
            return DateTime.Now;
        }

        public int SendMsgQueueLen()
        {
            return mNetWork.SendMsgQueueLength;
        }

        //void OnGUI()
        //{
        //    if (GUILayout.Button("tt"))
        //    {
        //        Debug.Log(RUDPClient.GetFirstAvailablePort());
        //    }
        //}
    }
}
