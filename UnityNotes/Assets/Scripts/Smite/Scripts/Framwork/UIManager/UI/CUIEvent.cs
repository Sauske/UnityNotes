//==================================================================================
/// UI事件枚举
/// @neoyang
/// @2015.03.02
//==================================================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using System.Reflection;
#endif

namespace Framework
{
    //--------------------------------------------------
    /// UI事件ID
    //--------------------------------------------------    
    public enum enUIEventID
    {
        None = 0,

        Common_OpenUrl = 6,
        Common_ParticlHide = 7,
        Common_ParticlShow = 8,
        Common_ParticlTimer = 9,
        Common_SendMsgAlertOpen = 10,
        Common_SendMsgAlertClose = 11,
        Common_WifiCheckTimer = 12,
        Common_ShowOrHideWifiInfo = 13,
        Common_BattleWifiCheckTimer = 14,
        Common_BattleShowOrHideWifiInfo = 15,
        Common_NewHeroOrSkinFormClose = 16,
        Common_RedDotParsEvent = 17,                    //保存小红点是否删除标记用事件

        //版本更新
        VersionUpdate_ConfirmYYBSaveUpdateApp = 18,
        VersionUpdate_JumpToHomePage = 19,
        VersionUpdate_ConfirmUpdateApp = 20,
        VersionUpdate_QuitApp = 21,
        VersionUpdate_CancelUpdateApp = 22,
        VersionUpdate_ConfirmUpdateResource = 23,
        VersionUpdate_RetryCheckAppVersion = 24,
        VersionUpdate_ConfirmUpdateAppNoWifi = 25,
        VersionUpdate_ConfirmUpdateResourceNoWifi = 26,
        VersionUpdate_RetryCheckFirstExtractResource = 27,
        VersionUpdate_RetryCheckResourceVersion = 28,
        VersionUpdate_OnAnnouncementListElementEnable = 52,
        VersionUpdate_SwitchAnnouncementListElementToFront = 53,
        VersionUpdate_SwitchAnnouncementListElementToBehind = 54,
        VersionUpdate_OnAnnouncementListSelectChanged = 55,
        VersionUpdate_UpdateToPreviousVersion = 56,

        //UI Form 通用事件
        UI_OnFormPriorityChanged = 45,
        UI_OnFormVisibleChanged = 46,
        UI_OnFormCloseWithAnimation = 47,

        //autoscroll自动滚动结束
        UIComponent_AutoScroller_Scroll_Finish = 70,


        // bodong add
        // used to change Dictionary to Array
        // 这个只能放最后 谁在它后面加东西 拖出去枪毙
        MAX_Tag
        // 别在我后面加东西 不谢
    };

    //--------------------------------------------------
    /// UI事件参数
    //--------------------------------------------------
    public struct stUIEventParams
    {
        public object param1;
        public object param2;
    }
       
    //--------------------------------------------------
    /// UI事件
    //--------------------------------------------------
    public class CUIEvent
    {
        public CUIFormScript m_srcFormScript;                           //源Form
        public GameObject m_srcWidget;                                  //源控件
        public CUIComponent m_srcWidgetScript;                          //源控件UI组件基类
        public CUIListScript m_srcWidgetBelongedListScript;             //源控件所属List(仅当源控件作为List元素时有效)
        public int m_srcWidgetIndexInBelongedList { get; set; } //源控件在所属List中的索引(仅当源控件作为List元素时有效)
        public PointerEventData m_pointerEventData;                     //源操作事件数据
        public stUIEventParams m_eventParams;                           //事件参数
        public enUIEventID m_eventID;                                   //事件ID

        public bool m_inUse = false;

        //--------------------------------------
        /// Clear  
        //--------------------------------------
        public void Clear()
        {
            m_srcFormScript = null;
            m_srcWidget = null;
            m_srcWidgetScript = null;
            m_srcWidgetBelongedListScript = null;
            m_srcWidgetIndexInBelongedList = -1;
            m_pointerEventData = null;
            m_eventID = enUIEventID.None;

            m_inUse = false;
        }
    };

#if UNITY_EDITOR

    public static class CUIEventEnumCheker
    {
        public static bool IsIgnore(int Val, int[] IgnoreLists)
        {
            foreach(int i in IgnoreLists)
            {
                if( i == Val)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Check()
        {
            int[] IgnoreList = new int[]
            {
                // 如果是故意设置成一样的 请将数值添加到这里
                // 比如：503, 1153
            };


            string[] EnumNames = Enum.GetNames(typeof(enUIEventID));
            Dictionary<int, string> Collections = new Dictionary<int, string>();

            foreach (string name in EnumNames)
            {
                int Value = Convert.ToInt32(Enum.Parse(typeof(enUIEventID), name));

                string ExistsName = "";

                if (Collections.TryGetValue(Value, out ExistsName))
                {
                    if( !IsIgnore(Value, IgnoreList))
                    {
                        DebugHelper.Assert(false, "repeat enUIEventID enum value, {0} == {1}, value={2}", ExistsName, name, Value);
                    }
                }
                else
                {
                    Collections.Add(Value, name);
                }
            }

        }
    }

#endif
};