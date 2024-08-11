using System;
using System.Net;
using System.Diagnostics;
using System.Globalization;

namespace UMI.Net
{
    public class CommTypeUtil
    {

        public static Int32 cstrlen(byte[] str)
        {
            byte nullChar = 0x00;
            Int32 count = 0;
            for (int i = 0; i < str.GetLength(0); i++)
            {
                if (nullChar == str[i])
                {
                    break;
                }

                count++;
            }

            return count;
        }

        public static Int32 wstrlen(Int16[] str)
        {
            Int16 nullChar = 0x0000;
            Int32 count = 0;
            for (int i = 0; i < str.GetLength(0); i++)
            {
                if (nullChar == str[i])
                {
                    break;
                }

                count++;
            }

            return count;
        }

        public static CommError.Type str2IP(out UInt32 ip, string strip)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            IPAddress address;
            byte[] szIP;
            if (IPAddress.TryParse(strip, out address))
            {
                szIP = address.GetAddressBytes();
                ip = (uint)((szIP[3] << 24) | (szIP[2] << 16) | (szIP[1] << 8) | szIP[0]);
            }
            else
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
                ip = 0;
                ret = CommError.Type.COMM_ERR_INVALID_IP_VALUE;
            }

            return ret;
        }

        public static CommError.Type IP2Str(ref VisualBuf buf, UInt32 ip)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            IPAddress address = new IPAddress((Int64)ip);
            string strip = address.ToString();

            ret = buf.sprintf("{0}", strip);

            return ret;
        }

        public static CommError.Type str2CommTime(out UInt32 time, string strTime)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            DateTime dt;
            CommTime commTime = new CommTime();

            if (DateTime.TryParse(strTime, out dt))
            {
                commTime.nHour = (short)dt.TimeOfDay.Hours;
                commTime.bMin = (byte)dt.TimeOfDay.Minutes;
                commTime.bSec = (byte)dt.TimeOfDay.Seconds;

                commTime.toTime(out time);
            }
            else
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
                time = 0;
                ret = CommError.Type.COMM_ERR_INVALID_COMMTIME_VALUE;
            }

            return ret;
        }

        public static CommError.Type Time2Str(ref VisualBuf buf, UInt32 time)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            CommTime tm = new CommTime();

            ret = tm.parse(time);
            if (CommError.Type.COMM_NO_ERROR == ret)
            {
                ret = buf.sprintf("{0:d2}:{1:d2}:{2:d2}", tm.nHour, tm.bMin, tm.bSec);
            }
            else
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
                ret = CommError.Type.COMM_ERR_INVALID_COMMTIME_VALUE;
            }

            return ret;
        }

        public static CommError.Type str2CommDate(out UInt32 date, string strDate)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            DateTime dt;
            CommDate commDate = new CommDate();

            if (DateTime.TryParse(strDate, out dt))
            {
                commDate.nYear = (short)dt.Year;
                commDate.bMon = (byte)dt.Month;
                commDate.bDay = (byte)dt.Day;

                commDate.toDate(out date);
            }
            else
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
                date = 0;
                ret = CommError.Type.COMM_ERR_INVALID_COMMDATE_VALUE;
            }

            return ret;
        }

        public static CommError.Type CommDate2Str(ref VisualBuf buf, UInt32 date)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            CommDate CommDate = new CommDate();

            ret = CommDate.parse(date);
            if (CommError.Type.COMM_NO_ERROR == ret)
            {
                ret = buf.sprintf("{0:d4}-{1:d2}-{2:d2}", CommDate.nYear, CommDate.bMon, CommDate.bDay);
            }
            else
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
                ret = CommError.Type.COMM_ERR_INVALID_COMMDATE_VALUE;
            }

            return ret;
        }

        public static CommError.Type str2CommDateTime(out UInt64 datetime, string strDateTime)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            DateTime dt;
            CommDateTime commDateTime = new CommDateTime();

            if (DateTime.TryParse(strDateTime, out dt))
            {
                commDateTime.commDate.nYear = (short)dt.Year;
                commDateTime.commDate.bMon = (byte)dt.Month;
                commDateTime.commDate.bDay = (byte)dt.Day;

                commDateTime.commTime.nHour = (short)dt.TimeOfDay.Hours;
                commDateTime.commTime.bMin = (byte)dt.TimeOfDay.Minutes;
                commDateTime.commTime.bSec = (byte)dt.TimeOfDay.Seconds;

                commDateTime.toDateTime(out datetime);
            }
            else
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
                datetime = 0;
                ret = CommError.Type.COMM_ERR_INVALID_COMMDATETIME_VALUE;
            }

            return ret;
        }

        public static CommError.Type CommDateTime2Str(ref VisualBuf buf, UInt64 datetime)
        {
            CommError.Type ret = CommError.Type.COMM_NO_ERROR;
            CommDateTime commDateTime = new CommDateTime();

            ret = commDateTime.parse(datetime);
            if (CommError.Type.COMM_NO_ERROR == ret)
            {
                ret = buf.sprintf("{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}",
                        commDateTime.commDate.nYear, commDateTime.commDate.bMon, commDateTime.commDate.bDay,
                        commDateTime.commTime.nHour, commDateTime.commTime.bMin, commDateTime.commTime.bSec);
            }
            else
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
                ret = CommError.Type.COMM_ERR_INVALID_COMMDATETIME_VALUE;
            }

            return ret;
        }
    }
}
