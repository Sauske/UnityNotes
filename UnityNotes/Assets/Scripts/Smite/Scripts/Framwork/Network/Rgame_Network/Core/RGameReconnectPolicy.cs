using System;
using UnityEngine;

namespace Framework
{
    public delegate uint tryReconnectDelegate(uint nCount, uint nMax);

    public class RGameReconnectPolicy
    {

        private RGameBaseConnector connector = null;
        private tryReconnectDelegate callback = null;

        private bool sessionStopped = false;
        private float reconnectTime = 0;   // 重连间隔时间
        private uint reconnectCount = 4;
        private uint tryCount = 0;
        private uint connectTimeout = 10; //超时，单位秒
        public bool shouldReconnect = false;


        public void SetConnector(RGameBaseConnector inConnector, tryReconnectDelegate inEvent, uint tryMax)
        {
            StopPolicy();
            connector = inConnector;
            callback = inEvent;
            reconnectCount = tryMax;
            DebugHelper.Log(string.Format("connector:{0}, tryMax:{1}", inConnector!=null?inConnector.ToString():"", tryMax));
        }

        public void StopPolicy()
        {
            sessionStopped = false;
            shouldReconnect = false;
            reconnectTime = connectTimeout;
            tryCount = 0;
        }

        public void StartPolicy(enNetResult result, int timeWait)
        {
            switch (result)
            {
                case enNetResult.Success:
                    {
                        shouldReconnect = false;
                        sessionStopped = false;
                    }
                    break;
                case enNetResult.ConnectFailed:
                case enNetResult.Timeout:
                case enNetResult.Error:
                    {
                        shouldReconnect = true;
                        sessionStopped = true;
                        reconnectTime = (tryCount == 0 ? 0 : timeWait);
                    }
                    break;

                default:
                    {
                        shouldReconnect = true;
                        sessionStopped = true;
                        reconnectTime = (tryCount == 0 ? 0 : timeWait);
                    }
                    break;
            }
        }


        public void UpdatePolicy(bool bForce)
        {
            if (connector != null && !connector.Connected)
            {
                if (bForce)
                {
                    reconnectTime = connectTimeout;
                    tryCount = reconnectCount;

                    if (sessionStopped)
                    {
                        connector.RestartConnector();
                    }
                    else
                    {
                        connector.RestartConnector();
                    }
                }
                else
                {
                    reconnectTime -= Time.unscaledDeltaTime;

                    if (reconnectTime < 0) // 触发重连条件
                    {
                        tryCount++;
                        reconnectTime = connectTimeout;

                        var testCount = tryCount;
                        if (callback != null)
                        {
                            testCount = callback(testCount, reconnectCount);  // 上层控制是否再次重连
                        }

                        if (testCount > reconnectCount) // 超过最大次数限制
                        {
                            return;
                        }

                        tryCount = testCount;

                        if (sessionStopped)
                        {
                            connector.RestartConnector();
                        }
                        else
                        {
                            connector.RestartConnector();
                        }
                    }
                }
            }
        }
    }
}
