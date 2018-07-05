using System.Collections;
using System.Collections.Generic;
using System;

namespace ResUpdater
{
    public class ResUpdaterInfo
    {
        public double Size;
        public string Format;
        public int UpdateCount;
    }

    public class ResUpdaterUtil
	{
        public const int KB = 1024;
        public const int MB = 1024 * 1024;
        public const int GB = 1024 * 1024 * 1024;

        public static ResUpdaterInfo GetUpdaterInfo(long lenght)
        {
            ResUpdaterInfo info = new ResUpdaterInfo();
            if(lenght < 1000)
            {
                info.Size = lenght;
                info.Format = "B";
            }
            else if(lenght <= KB * 1000)
            {
                info.Size = Math.Round(lenght / (float)KB, 1);
                info.Format = "KB";
            }
            else if(lenght <= MB * 1000)
            {
                info.Size = Math.Round(lenght / (float)MB, 1);
                info.Format = "MB";
            }
            else
            {
                info.Size = Math.Round(lenght / (float)GB, 1);
                info.Format = "GB";
            }
            return info;
        }
    }
}
