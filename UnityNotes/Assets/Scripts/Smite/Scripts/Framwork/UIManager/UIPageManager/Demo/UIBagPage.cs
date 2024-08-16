using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Framework
{
    public enum enItemMenuType
    {
        All = 0,        //全部
        RecentGet = 1,  //最近获得  
        Item = 2,       //道具
        Gift = 3,       //礼包
        ExpCard = 4,    //经验卡
        Symbol = 5,     //铭文
    }

    public class UIBagPage : UIBasePage
    {
        public static UIBasePage Creator()
        {
            UIBagPage page = UIPageManager.GetInstance().UIRoot.AddComponent<UIBagPage>();
            page.PageId = UIPageIDs.PAGE_ID_BAG;
            page.IsTop = true;
            page.AssetPath = CUIUtility.s_Form_System_Dir + "Bag/Form_Bag.prefab";
            return page;
        }
        protected override void PreInitialize()
        {

            base.PreInitialize();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
        }

        protected override void FillContent()
        {
            base.FillContent();
        }

        protected override void RegisterEvents()
        {
          //  CUIEventManager.GetInstance().AddUIEventListener(enUIEventID.Bag_MenuSelect, OnMenuSelect);
        }

        protected override void UnRegisterEvents()
        {
          //  CUIEventManager.GetInstance().RemoveUIEventListener(enUIEventID.Bag_MenuSelect, OnMenuSelect);
        }
        #region events
        void OnMenuSelect(CUIEvent uiEvent)
        {
          //  CUIFormScript form = uiEvent.m_srcFormScript;
        }
        #endregion
    }
}
