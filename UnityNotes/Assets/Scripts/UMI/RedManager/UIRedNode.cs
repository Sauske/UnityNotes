using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UMI
{
    public class RedPointNode
    {
        public RedNodeEnum key;

        //当前节点
        public RedPointNode rootNode;
        //有子节点的情况下 只根据子节点判断(节点红黑最终由这个值决定)
        public bool nodeValue;
        //自身fun计算出的红黑值 用来和子集 并
        public bool localValue;
        //绑定红点UI 
        public List<GameObject> redPointUIs = new List<GameObject>();
        //父节点 有可能复用节点 允许多个父节点
        public List<RedPointNode> parent = new List<RedPointNode>();
        //子节点
        public List<RedPointNode> childNode_list = new List<RedPointNode>();
        //注册判断方法
        public Func<bool> funC;
        //刷新标记
        public bool needRefresh = false;

        //最优否决权(应付策划要求 打开界面 就关闭红点 等要求...强行修改..)
        public Func<bool> firstCheck;

        public RedPointNode() { }

        public RedPointNode(RedNodeEnum urlKey, GameObject ui, Func<bool> funC)
        {
            this.key = urlKey;
            this.funC = funC;
            redPointUIs.Add(ui);

            //刷新初始状态 例如：UI默认ture 但是判断false这里nodeValue 默认也是false 会不刷新
            if (ui != null)
                RefreshUI();
        }
        //添加子节点
        public void AddChild(RedPointNode node)
        {
            if (!this.childNode_list.Contains(node))
            {
                this.childNode_list.Add(node);
                if (!node.parent.Contains(this))
                {
                    node.parent.Add(this);
                }
            }
            //刷新父信息
            this.Refresh();
        }

        protected void Refresh()
        {
            bool oldNodeValue = this.nodeValue;
            this.nodeValue = this.localValue;

            //有子节点 只看子节点综合情况
            if (!this.localValue && this.childNode_list.Count > 0)
            {
                for (int i = 0; i < this.childNode_list.Count; i++)
                {
                    nodeValue |= this.childNode_list[i].nodeValue;
                }
            }

            //被最优条件否决 则直接false
            if (firstCheck != null)
            {
                bool firstValue = firstCheck();
                if (!firstValue)
                    this.nodeValue = firstValue;
            }

            //减少刷新
            if (oldNodeValue != this.nodeValue)
            {
                RefreshUI();
                //向上传递信息
                if (this.parent.Count > 0)
                {
                    for (int i = 0; i < this.parent.Count; i++)
                    {
                        this.parent[i].Refresh();
                    }
                }

                //触发方法
                // GlobalEvent.Instance.Dispatch(EventType.RefreshRedPoint, key);
            }
        }

        /// <summary>
        /// 设置红点信息 刷新UI
        /// </summary>
        public void SetValue(bool vals)
        {
            this.localValue = vals;
            Refresh();
        }

        /// <summary>
        /// 刷新红点UI状态
        /// </summary>
        private void RefreshUI()
        {
            if (redPointUIs != null && redPointUIs.Count > 0)
            {
                for (int i = redPointUIs.Count - 1; i >= 0; i--)
                {
                    if (redPointUIs[i] != null)
                    {
                        redPointUIs[i].SetActive(this.nodeValue);
                    }
                    else
                    {
                        redPointUIs.RemoveAt(i);
                    }
                }

            }
        }

        /// <summary>
        /// 重新绑定节点GameObject
        /// </summary>
        public void BindUI(GameObject ui)
        {
            if (!this.redPointUIs.Contains(ui))
                this.redPointUIs.Add(ui);

            RefreshUI();
        }

        /// <summary>
        /// 根据逻辑方法 刷新红点数据
        /// </summary>
        /// <param name="updateChild">是否要刷新子节点数据</param>
        public void RefreshRedPoint(bool updateChild = false)
        {
            if (updateChild)
            {
                for (int i = 0; i < childNode_list.Count; i++)
                {
                    childNode_list[i].RefreshRedPoint(updateChild);
                }
            }

            bool redActive = false;
            if (funC != null)
            {
                try
                {
                    redActive = funC();
                }
                catch (Exception e)
                {
                    Debug.LogError("===RedPint  Exception ==" + e.Message);
                }
            }
            SetValue(redActive);
        }
    }

    public class RedPointAction
    {
        public int[] redParams;
        public GameObject redPoint;
        public Func<int[], bool> funC;

        public RedPointAction(GameObject _redPoint, Func<int[], bool> callback, int[] parames)
        {
            this.redPoint = _redPoint;
            this.funC = callback;
            this.redParams = parames;
        }
    }
}
