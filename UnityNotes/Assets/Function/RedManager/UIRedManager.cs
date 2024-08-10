using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UMI
{
    public class UIRedManager : Singleton<UIRedManager>
    {
        /// <summary>
        /// 循环计时器
        /// </summary>
        // private CDBot cdBot;
        private Dictionary<string, RedPointAction> listCallback = new Dictionary<string, RedPointAction>();
        private Dictionary<string, RedPointAction>.Enumerator enumerator;

        // 红点集合
        private Dictionary<RedNodeEnum, RedPointNode> listRedNode = new Dictionary<RedNodeEnum, RedPointNode>();
        //记录需要刷新的红点节点
        private Queue<RedPointNode> refreshNodeQueue = new Queue<RedPointNode>();

        public override void OnInitialize()
        {
            base.OnInitialize();
            //CTimerManager.GetInstance().AddTimer(3000, -1, UpdateRedUI);

            enumerator = listCallback.GetEnumerator();
        }

        public  void OnUpdate()
        {
            //还是减少红点刷新 而且延后判断 有利于一些条件更新后 再刷新
            //每X帧刷新一次
            if ((Time.frameCount & 5) == 0)
            {
                RefreshRedPointQueue();
            }
        }

        public  void UnInit()
        {
            listCallback.Clear();

            listRedNode.Clear();
            refreshNodeQueue.Clear();
        }

        /// <summary>
        /// 刷新注册回调的函数
        /// </summary>
        /// <param name="arg0"></param>
        private void UpdateRedUI(int arg0)
        {
            enumerator = listCallback.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //ProfilerSample.BeginSample("UpdateRedUI {0} {1}", enumerator.Current.Value.funC.Method.Name, enumerator.Current.Value.funC.Method.DeclaringType.ToString());
                if (enumerator.Current.Value.redPoint != null)
                {
                    try
                    {
                        enumerator.Current.Value.redPoint.SetActive(
                            enumerator.Current.Value.funC(enumerator.Current.Value.redParams));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("===RedPint  Exception ==" + e.Message);
                    }
                }
                //ProfilerSample.EndSample();
            }
        }


        /// <summary>
        /// 注册函数
        /// </summary>
        /// <param name="redType"></param>
        /// <param name="regist"></param>
        public void Regist(string redType, GameObject _redPoint, Func<int[], bool> callback, int[] redParams = null)
        {
            RedPointAction regist = new RedPointAction(_redPoint, callback, redParams);
            if (listCallback.ContainsKey(redType) == false)
            {
                listCallback[redType] = regist;
                bool redActive = regist.funC(regist.redParams);
                if (redActive != regist.redPoint.activeSelf)
                {
                    regist.redPoint.SetActive(redActive);
                }
            }
            else
            {
                Debug.LogError("---重复注册红点提示---" + redType);
            }
        }

        /// <summary>
        /// 移除注册红点
        /// </summary>
        /// <param name="redType"></param>
        /// <param name="unRegist"></param>
        public void UnRegist(string redType)
        {
            RedPointAction red = null;
            if (listCallback.TryGetValue(redType, out red))
            {
                listCallback.Remove(redType);
            }
        }

        /// <summary>
        /// 注册节点
        /// </summary>
        /// <param name="callback">节点红黑判断条件 如果有子节点 这个为辅助判断条件 会与子节点或操作</param>
        /// <returns></returns>
        public RedPointNode RegistRedPoint(RedNodeEnum redType, GameObject redPointUI = null, Func<bool> callback = null)
        {
            RedPointNode redPointNode = null;
            listRedNode.TryGetValue(redType, out redPointNode);
            if (redPointNode == null)
            {
                redPointNode = new RedPointNode(redType, redPointUI, callback);
            }
            redPointNode.RefreshRedPoint();
            listRedNode[redType] = redPointNode;

            return redPointNode;
        }

        /// <summary>
        /// 绑定红点GameObject
        /// </summary>
        public RedPointNode BindUI(RedNodeEnum redType, GameObject go, bool refresh = false)
        {
            RedPointNode redPointNode = null;
            listRedNode.TryGetValue(redType, out redPointNode);
            if (redPointNode != null)
            {
                redPointNode.BindUI(go);

                if (refresh)
                    redPointNode.RefreshRedPoint();
            }
            return redPointNode;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        public RedPointNode GetRedPointNode(RedNodeEnum redType)
        {
            RedPointNode redPointNode = null;
            listRedNode.TryGetValue(redType, out redPointNode);
            return redPointNode;
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        public RedPointNode UnBindUI(RedNodeEnum redType, GameObject redPoint = null)
        {
            RedPointNode redPointNode = null;
            listRedNode.TryGetValue(redType, out redPointNode);
            if (redPointNode != null)
            {
                if (redPoint != null)
                {
                    redPointNode.redPointUIs.Remove(redPoint);
                }
                else
                {
                    redPointNode.redPointUIs.Clear();
                }
            }
            return redPointNode;
        }

        /// <summary>
        /// 检查红黑
        /// </summary>
        public bool CheckRedPoint(RedNodeEnum redType)
        {
            RedPointNode redPointNode = null;
            listRedNode.TryGetValue(redType, out redPointNode);
            if (redPointNode != null)
            {
                return redPointNode.nodeValue;
            }
            return false;
        }

        /// <summary>
        /// 注销节点
        /// </summary>
        public void UnRegistRedPoint(RedNodeEnum redType)
        {
            if (listRedNode.ContainsKey(redType))
            {
                listRedNode.Remove(redType);
            }
        }

        /// <summary>
        /// 刷新某个节点红点
        /// </summary>
        public void UpdateRedPoint(RedNodeEnum redType, bool updateChild = false)
        {
            RedPointNode redPointNode = null;
            listRedNode.TryGetValue(redType, out redPointNode);
            if (redPointNode != null && !redPointNode.needRefresh)
            {
                //改为标记脏数据 防止多个条件触发刷新
                redPointNode.needRefresh = true;
                refreshNodeQueue.Enqueue(redPointNode);
                //redPointNode.RefreshRedPoint();
            }
        }


        //刷新 需要刷新的红点队列
        private void RefreshRedPointQueue()
        {
            if (refreshNodeQueue.Count <= 0)
                return;

            while (refreshNodeQueue.Count > 0)
            {
                RedPointNode redPoint = refreshNodeQueue.Dequeue();
                redPoint.needRefresh = false;
                redPoint.RefreshRedPoint(true);
            }
        }



        /// <summary>
        /// 注册红点
        /// </summary>
        public void RegistRedPoint()
        {
            RegistPlayerInfoRed();
        }

        private void RegistPlayerInfoRed()
        {
            RedPointNode RedPointNode = UIRedManager.Instance.RegistRedPoint(RedNodeEnum.PlayerInfo_Red);
           // RedPointNode.AddChild(UIRedManager.Instance.RegistRedPoint(RedNodeEnum.PlayerInfoEditor_Red, null, GetModule<PlayerInfoModule>().CheckEditorRed));
            RedPointNode.RefreshRedPoint(true);
        }
    }
}
