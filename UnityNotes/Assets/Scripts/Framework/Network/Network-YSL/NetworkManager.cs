using UnityEngine;
using UnityWebSocket;
using System;
using System.Collections.Generic;

namespace UnityWebSocket
{
    public class NetworkManager : Singleton<NetworkManager>
    {

        const int RECONECT_COUONT = 5;

        public const string gpAddress = "ws://192.168.1.131:8001/websocket";

        public static string address = "ws://156.251.145.166:8001/websocket";

        private IWebSocket webSocket;

        private static int reConnectCount = 0;

        public static string mToken = "";


        /// <summary>
        /// 重连期中的消息
        /// </summary>
        private readonly Queue<OutgoingBase> reconQueue = new Queue<OutgoingBase>();

        public override void OnInitialize()
        {
            base.OnInitialize();

            // OpenSocket();

          //  EventRouter.GetInstance().AddEventHandler<bool>(EventID.ON_APPLICATION_FOCUS, ON_APPLICATION_FOCUS);
        }

        public bool bUnConectTest = true;

        public void OnUpdate()
        {
            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    bUnConectTest = !bUnConectTest;
            //}

            if (bUnConectTest)
            {
                SendHeart();
            }
        }

        public void OnDispose()
        {
            base.OnInitialize();

            CloseSocket();

           // EventRouter.GetInstance().RemoveEventHandler<bool>(EventID.ON_APPLICATION_FOCUS, ON_APPLICATION_FOCUS);
        }

        public void OpenSocket(EventHandler<OpenEventArgs> action = null)
        {
            OpenSocket(address, action);
        }

        /// <summary>
        /// 强制新建新网络
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        public void OpenSocket(string url, EventHandler<OpenEventArgs> action = null)
        {
            CloseSocket();

            if (webSocket == null)
            {
                Debug.LogFormat("链接网络地址：{0}", url);

                address = url;

                webSocket = new WebSocket(url);
                webSocket.ConnectAsync();
            }

            if (webSocket.ReadyState == WebSocketState.Connecting)
            {
                return;
            }
            if (webSocket != null)
            {
                if (action != null)
                {
                    webSocket.OnOpen += action;
                }
                webSocket.OnOpen += OnOpen;
                webSocket.OnMessage += OnMessage;
                webSocket.OnClose += OnClose;
                webSocket.OnError += OnError;
            }
        }

        public void ReconnetSocket()
        {
            if (webSocket.ReadyState == WebSocketState.Connecting)
            {
                return;
            }

            CloseSocket();

            Debug.Log("重连网络。");

            webSocket = new WebSocket(address);
            webSocket.ConnectAsync();


            if (webSocket != null)
            {
                webSocket.OnOpen += OnOpen;
                webSocket.OnMessage += OnMessage;
                webSocket.OnClose += OnClose;
                webSocket.OnError += OnError;
            }
        }

        public void CloseSocket()
        {
            if (webSocket != null && webSocket.ReadyState != WebSocketState.Closed)
            {
                webSocket.CloseAsync();
                webSocket.OnOpen -= OnOpen;
                webSocket.OnMessage -= OnMessage;
                webSocket.OnClose -= OnClose;
                webSocket.OnError -= OnError;
            }
            webSocket = null;
        }

        private void OnOpen(object sender, OpenEventArgs args)
        {
            Debug.LogFormat("Connected:{0},reconQueue.count:{1}", address, reconQueue.Count);

            reConnectCount = 0;
        }

        private void OnMessage(object sender, ResEventArgs args)
        {
           // EventRouter.GetInstance().BroadNetEvent<IncommingBase>(args.Proto, args.Deserialize);
        }


        private void OnClose(object sender, CloseEventArgs args)
        {
            Debug.LogFormat("Closed: StatusCode:{0},Reason:{1}", args.StatusCode, args.Reason);

            if (reConnectCount < RECONECT_COUONT)
            {
                reConnectCount++;
               // ModuleManager.GetModule<LoginModule>().OpenSocket();
            }
            else
            {
                //UIPageManager.OpenMessageBox("网络重连", "网络5次重连无法重连成功，是否继续重连。", "重连", "取消", ReConnectConfim, null);
            }
        }


        private void OnError(object sender, ErrorEventArgs args)
        {
            Debug.LogFormat("Error:{0}", args.Message);
        }



        /// <summary>
        /// 心跳
        /// </summary>
        const int HEART_DELTA = 30;    //心跳间隔5秒
        float heartTime = 0;
        private void SendHeart()
        {
            heartTime += Time.deltaTime;
            if (heartTime >= HEART_DELTA)
            {
                heartTime = 0;

                if (webSocket == null || webSocket.ReadyState != WebSocketState.Open)
                {
                    //ModuleManager.GetModule<LoginModule>().OpenSocket();
                    return;
                }

                //P10011 heart = new P10011();
                //SendMessage(heart);
            }
        }

        /// <summary>
        /// 新消息加入消息队列
        /// </summary>
        /// <param name="packet"></param>
        public void SendMessage(OutgoingBase packet)
        {
            if (webSocket != null && webSocket.ReadyState == WebSocketState.Open)
            {
                reConnectCount = 0;

                // if (packet.proto != 10011)   //屏蔽心跳包
                Debug.Log(packet.ToString());

                webSocket.SendAsync(packet.GetBuffer());
            }
            else
            {
                bool bFind = false;
                foreach (var item in reconQueue)
                {
                    if (item.proto == packet.proto)
                        bFind = true;
                }

                if (!bFind)
                    reconQueue.Enqueue(packet);

                if (reConnectCount < RECONECT_COUONT)
                {
                   // ModuleManager.GetModule<LoginModule>().OpenSocket();
                    reConnectCount++;
                }
                else
                {

                   // UIPageManager.OpenMessageBox("网络重连", "网络5次重连无法重连成功，是否继续重连。", "重连", "取消", ReConnectConfim, null);
                    Debug.Log("网络已经断开..........................");
                }
            }
        }

        private void ReConnectConfim()
        {
            string LoginUrl = "ws://192.168.1.141:8001/websocket";
            OpenSocket(LoginUrl/*AppConst.LoginUrl*/, OpenNetCallback);

            reConnectCount = 0;
        }

        void OpenNetCallback(object sender, OpenEventArgs args)
        {
            //string account = PlayerManager.GetInstance().Account;
            //Player player = PlayerManager.GetInstance().Player;

            //if (player != null && !string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(player.Name))
            //{
            //    ModuleManager.GetModule<LoginModule>().SendLogin(account, player.Name);
            //}
        }

        /// <summary>
        /// 发送断网期间缓存的消息
        /// </summary>
        public void SendReconQueue()
        {
            while (reconQueue.Count > 0)
            {
                OutgoingBase outgoing = reconQueue.Dequeue();
                //if (outgoing is P10011)
                //{
                //    //过滤心跳包
                //    continue;
                //}
                SendMessage(outgoing);
            }
        }

        private void ON_APPLICATION_FOCUS(bool focus)
        {
            if (focus)
            {
                bool inRoom = true;// PVPManager.GetInstance().InRoom;
                if (inRoom)
                {
                    heartTime = 30;
                    SendHeart();
                }
            }
        }
    }
}
