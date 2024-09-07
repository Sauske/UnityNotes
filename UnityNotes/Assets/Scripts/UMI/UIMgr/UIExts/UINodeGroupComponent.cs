using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMI
{
    /// <summary>
    /// 界面节点组信息定义
    /// </summary>
    [Serializable]
    public class UINodeGroupInfo
    {
        public string groupName;            // 组名称
        public GameObject[] arrNode;        // 组包含的对象集合
    }
    
    /// <summary>
    /// 界面节点组组件
    /// 将节点分成多个组，用于多情况下要显示不同节点
    /// </summary>
    public class UINodeGroupComponent : MonoBehaviour
    {
        public UINodeGroupInfo[] arrGroupInfo;      // 节点组信息集合

        /// <summary>
        /// 切换要显示的节点组
        /// </summary>
        /// <param name="arrGroupName">需要显示的节点组集合</param>
        public void Switch(params string[] arrGroupName)
        {
            if (arrGroupName == null || arrGroupInfo == null)
            {
                return;
            }
            
            // 先因隐藏不需要显示的组
            for (int i = 0; i < arrGroupInfo.Length; i++)
            {
                UINodeGroupInfo groupInfo = arrGroupInfo[i];
                if (IsNameInGroup(arrGroupName, groupInfo.groupName))
                {
                    // 需要显示的，先不处理
                    continue;
                }

                SetOneGroupAllNodeActive(groupInfo, false);
            }
            
            // 显示所有需要显示的组
            for (int i = 0; i < arrGroupInfo.Length; i++)
            {
                UINodeGroupInfo groupInfo = arrGroupInfo[i];
                if (!IsNameInGroup(arrGroupName, groupInfo.groupName))
                {
                    // 需要因此的，不处理
                    continue;
                }

                SetOneGroupAllNodeActive(groupInfo, true);
            }
        }

        /// <summary>
        /// 隐藏所有的节点组
        /// </summary>
        public void HideAll()
        {
            Switch();
        }
        
        /// <summary>
        /// 设置一个组的所有对象的激活状态
        /// </summary>
        /// <param name="groupInfo">要处理的组信息</param>
        /// <param name="active">是否要激活对象显示</param>
        private void SetOneGroupAllNodeActive(UINodeGroupInfo groupInfo, bool active)
        {
            for (int i = 0; i < groupInfo.arrNode.Length; i++)
            {
                groupInfo.arrNode[i].SetActive(active);
            }
        }

        /// <summary>
        /// 判断某个名称是否在名称集合里面
        /// </summary>
        /// <param name="arrGroupName">所有名称集合</param>
        /// <param name="groupName">要判断的名称</param>
        /// <returns>名称在数组里面返回true；否则返回false</returns>
        private bool IsNameInGroup(string[] arrGroupName, string groupName)
        {
            for (int i = 0; i < arrGroupName.Length; i++)
            {
                if (arrGroupName[i].Equals(groupName))
                {
                    return true;
                }
            }

            return false;
        }
    }

}
