using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;

namespace MainGame
{

    public class UIRegister
    {
        public static void RegisterUIPanel()
        {
            //uiid 和界面的预制体名字
            UIDataTable.AddPanelData(UIID.LogoPanel, null, "LogoPanel");
        }
    }
}