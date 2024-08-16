//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    
    public class TimeUtil
    {
        //server 取的时间是从1970-1-1 8：00开始计时，Datetime从0000-1-1：00：00开始计时
        public static DateTime UnixEoich = new DateTime(1970, 1, 1, 8, 0, 0);

        public static uint Local2ServerTime(DateTime localTime,out uint usec)
        {
            uint serverTime = (uint)((localTime.Ticks - UnixEoich.Ticks)/10000000);
            usec = (uint)((localTime.Ticks - UnixEoich.Ticks) % 10000000) / 10;
            return serverTime;
        }

        public static DateTime ServerTimeToLocalTime(uint nTime, uint uTick)
        {
            long uServerTime = UnixEoich.Ticks + (long)nTime * 10000000 + uTick * 10;

            return new DateTime(uServerTime);
        }
    }
}