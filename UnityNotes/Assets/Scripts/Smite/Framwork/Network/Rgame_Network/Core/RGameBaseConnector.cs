using System;

namespace Framework
{
    #pragma warning disable 0618

    public enum enDealConnectStatus
    {
        Deal_Connect_Success,
        Deal_Reconect_Success,
        Deal_Connect_Fail,
        Deal_Connect_Error,
        Deal_Connect_Close,
    }

    public class RGameConnectorParam
    {
        public bool bUDP = false;
        public string ip = "";
        public ushort port = 0;

        private string mUIp = "";
        public string UIp { get { return mUIp; } set { mUIp = value; } }

        private ushort mUPort = 0;
        public ushort UPort { get { return mUPort; } set { mUPort = value; } }


        public void SetVPort(ushort nPort)
        {
            port = nPort;
        }


        public void SetVip(string Vip)
        {
            ip = Vip;
        }
    }

    public  class RGameBaseConnector
    {
        protected RGameConnector mConnector = null;

        protected RGameConnectorParam mInitParam = null;

        protected bool bConnected = false;

        public static uint connectTimeout = 10;


        protected string mLastSuccessIp;
        protected int mLastSuccessPort;

        public RGameConnectorParam InitParam
        { get { return mInitParam; } }

        public bool Connected
        {
            get { return bConnected; }
        }

        public bool CreateConnector(RGameConnectorParam param)
        {
            DestroyConnector();
            if(null == param)
                return false;

            mInitParam = param;
            bConnected = false;
            mConnector = new RGameConnector(mInitParam.ip, mInitParam.port,mInitParam.bUDP,mInitParam.UIp,mInitParam.UPort);

            mConnector.ConnectEvent += onConnectEvent;
            mConnector.DisconnectEvent += onDisconnectEvent;
            mConnector.ReconnectEvent += onReconnectEvent;
            mConnector.ErrorEvent += onConnectError;
            mConnector.TimeoutEvent += onConnectTimeout;
            mConnector.ConnectFailedEvent += onConnectFailed;

            return mConnector.Connect(RGameBaseConnector.connectTimeout);
        }

        public void DestroyConnector()
        {
            if(mConnector!=null)
            {
                mConnector.ConnectEvent -= onConnectEvent;
                mConnector.DisconnectEvent -= onDisconnectEvent;
                mConnector.ReconnectEvent -= onReconnectEvent;
                mConnector.ErrorEvent -= onConnectError;
                mConnector.TimeoutEvent -= onConnectTimeout;
                mConnector.ConnectFailedEvent -= onConnectFailed;

                mConnector.Disconnect();
                mConnector = null;
                bConnected = false;
            }
        }

        public void RestartConnector()
        {
            DebugHelper.Log("Connector Restart");
            DestroyConnector();
            CreateConnector(mInitParam);
        }


        protected virtual void DealConnectSucc(enDealConnectStatus status)
        {

        }

        protected virtual void DealConnectFail()
        {

        }

        protected virtual void DealConnectError()
        {

        }

        protected virtual void DealConnectClose()
        {

        }

        private void onConnectEvent()
        {
            if (mConnector == null) return;

            bConnected = true;
            DealConnectSucc(enDealConnectStatus.Deal_Connect_Success);
        }

        private void onDisconnectEvent()
        {
            bConnected = false;
            DealConnectClose();
        }

        private void onReconnectEvent()
        {
            if (mConnector == null)
                return;

            bConnected = true;
            DealConnectSucc(enDealConnectStatus.Deal_Reconect_Success);
        }

        private void onConnectError()
        {
            bConnected = false;
            DealConnectError();
        }

        private void onConnectTimeout()
        {
            bConnected = false;
            DealConnectFail();
        }

        private void onConnectFailed()
        {
            bConnected = false;
            DealConnectFail();
        }
    }
}
