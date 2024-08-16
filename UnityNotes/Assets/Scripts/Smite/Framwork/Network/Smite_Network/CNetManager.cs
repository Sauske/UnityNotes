//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

//#if UNITY_IPHONE
//#define NEWTCP
//#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public enum CMD_ID
    {
        CS_CMD_REQ_PING = 10000,   //ping心跳
    }

    public class CNetManager : Singleton<CNetManager>
    {

        public class ServerInfo
        {
            public string ip;
            public string host;
            public int port;
        }

        public Dictionary<EConnectionType, ServerInfo> m_serverInfo = new Dictionary<EConnectionType, ServerInfo>
        {
           {
               EConnectionType.EAccount_Connection, 
               new ServerInfo()
               { 
                    ip = "106.75.135.220", 
                    port = 15001,
                    host = "dir.haichuangame.com",
                }
            },
            {
                EConnectionType.EDir_Connection,
                new ServerInfo()
                {
                    ip = "106.75.135.220",
                    port = 40011,
                    host = "acc.haichuangame.com",
                }
            },
            {
                EConnectionType.EVer_Connection,
                new ServerInfo()
                {
                    ip = "192.168.2.20",
                    port = 10002,
                }
            },
            {
                EConnectionType.EZone_Connection,
                new ServerInfo()
                {
                    ip = "112.124.4.37",
                    port = 14522,
                }
            },
            {
                EConnectionType.EPvp_Connection,
                new ServerInfo()
                {
                    ip = "192.168.2.36",
                    port = 5000,
                }
            }
        };

        private NavtiveNetwork mNetWork;

        protected NetworkReachability _lastInternetReachability = NetworkReachability.NotReachable;

        public override void Init()
        {
            base.Init();

            mNetWork = new NavtiveNetwork();
        }

        public override void UnInit()
        {
            base.UnInit();
            if (mNetWork != null)
            {
                mNetWork.Close();
            }
        }

        public  void Update()
        {
            CheckInternetReachability();

            mNetWork.Update();
        }

        public bool TriggerEvent(IEvent evt)
        {
            return true;
        }

        private void CheckInternetReachability()
        {
            if (Application.internetReachability != _lastInternetReachability)
            {
                if (mNetWork != null)
                {
                      mNetWork.OnNetWorkChange(_lastInternetReachability, Application.internetReachability);
                }
                _lastInternetReachability = Application.internetReachability;
            }
        }


        public void SetServerInfo(EConnectionType type, string strIP, int nPort)
        {
            if (m_serverInfo.ContainsKey(type))
            {
                m_serverInfo[type].ip = strIP;
                m_serverInfo[type].port = nPort;
            }
        }

        public void ConnectServer(EConnectionType connType)
        {
            ServerInfo svrInfo = null;
            if (m_serverInfo.TryGetValue(connType, out svrInfo))
            {
                mNetWork.CreateConnection(connType, svrInfo);
            }           
        }

        public void ConnectServer(EConnectionType connType,string ip,int port, string host)
        {
            ServerInfo svrInfo = null;
            if (m_serverInfo.TryGetValue(connType, out svrInfo))
            {
                svrInfo.ip = ip;
                svrInfo.port = port;
                svrInfo.host = host;

                mNetWork.CreateConnection(connType, svrInfo);
            }
        }

        public void CloseConnect(EConnectionType eType)
        {
            mNetWork.CloseConnection(eType);
        }

        public bool IsConnected(EConnectionType eType)
        {
            return mNetWork.IsConnected(eType);
        }

        public int GetDelay(EConnectionType eType)
        {
            return mNetWork.GetNetworkDelay(eType);
        }

        public void SentZone(string comtent)
        {
            mNetWork.SendMsg(EConnectionType.EZone_Connection, 0, comtent);
        }
    }
}