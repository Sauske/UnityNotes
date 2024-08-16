﻿//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
{
    public class EventPauseCommand : AbstractGuideCommand
    {
        protected override void OnStart()
        {
            UIMgr.S.topPanelHideMask = PanelHideMask.UnInteractive;
        }

        protected override void OnFinish(bool forceClean)
        {
            UIMgr.S.topPanelHideMask = PanelHideMask.None;
        }
    }
}
