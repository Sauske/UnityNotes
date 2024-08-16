using System;
using System.Diagnostics;

public class CommTime
{
    public short nHour;
    public byte  bMin ;
    public byte  bSec ;

    public CommTime(){}

    public CommTime(uint time)
    {
        nHour = (short)(time & 0xFFFF)      ;
        bMin  = (byte) ((time >> 16) & 0xFF);
        bSec  = (byte) ((time >> 24) & 0xFF);
    }

    public CommError.Type parse(uint time)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        nHour  = (short)(time & 0xFFFF)      ;
        bMin   = (byte) ((time >> 16) & 0xFF);
        bSec   = (byte) ((time >> 24) & 0xFF);

        if(!isValid())
        {
            ret = CommError.Type.COMM_ERR_INVALID_COMMTIME_VALUE;
        }

        return ret;
    }

    public  bool isValid()
    {
        string str = string.Format("{0:d2}:{1:d2}:{2:d2}", nHour, bMin, bSec);
        DateTime dt;

        if (!DateTime.TryParse(str, out dt))
        {
#if (DEBUG)
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                if (null != st.GetFrame(i).GetFileName())
                {
                    Console.WriteLine(st.GetFrame(i).ToString());
                }
            }
#endif
            return false;
        }

        return true;
    }

    public void toTime(out uint time)
    {
        time = (uint)(((ushort)nHour | ((uint)bMin << 16) | ((uint)bSec << 24)));
    }
}

public class CommDate
{
    public short nYear;
    public byte  bMon ;
    public byte  bDay ;

    public CommDate(){}

    public CommDate(uint date)
    {
        nYear = (short)(date & 0xFFFF)     ;
        bMon  = (byte)((date >> 16) & 0xFF);
        bDay  = (byte)((date >> 24) & 0xFF);
    }

    public CommError.Type parse(uint date)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        nYear = (short)(date & 0xFFFF)     ;
        bMon  = (byte)((date >> 16) & 0xFF);
        bDay  = (byte)((date >> 24) & 0xFF);

        if (!isValid())
        {
            ret = CommError.Type.COMM_ERR_INVALID_COMMTIME_VALUE;
        }

        return ret;
    }

    public  bool isValid()
    {
        string str = string.Format("{0:d4}-{1:d2}-{2:d2}", nYear, bMon, bDay);
        DateTime dt;

        if (!DateTime.TryParse(str, out dt))
        {
#if (DEBUG)
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                if (null != st.GetFrame(i).GetFileName())
                {
                    Console.WriteLine(st.GetFrame(i).ToString());
                }
            }
#endif
            return false;
        }

        return true;
    }

    public void toDate(out uint date)
    {
        date = (uint)(((ushort)nYear | ((uint)bMon << 16) | ((uint)bDay << 24)));
    }

}

public class CommDateTime
{
    public CommDate commDate;
    public CommTime commTime;

    public CommDateTime()
    {
        commDate = new CommDate();
        commTime = new CommTime();
    }

    public CommDateTime(ulong datetime)
    {
        commDate = new CommDate((uint)(datetime & 0xFFFFFFFF));
        commTime = new CommTime((uint)((datetime >> 32) & 0xFFFFFFFF));
    }

    public CommError.Type parse(ulong datetime)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        uint date = (uint)(datetime & 0xFFFFFFFF);
        uint time = (uint)((datetime>>32) & 0xFFFFFFFF);

        ret = commDate.parse(date);
        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            ret = commTime.parse(time);
        }

        return ret;
    }

    public void toDateTime(out ulong datetime)
    {
        uint date = 0;
        uint time = 0;

        commDate.toDate(out date);
        commTime.toTime(out time);

        datetime = ((ulong)date | (ulong)time << 32);
    }

    public bool isValid()
    {
        return commDate.isValid() && commTime.isValid();
    }

}

