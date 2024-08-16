using System;
using System.Diagnostics;
using System.Text;

public class CommBufUtil
{
    public  static CommError.Type printMultiStr(ref VisualBuf buf, string str, int times)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        for (int i = 0; i < times; i++)
        {
            ret = buf.sprintf("{0}", str);
            if (ret != CommError.Type.COMM_NO_ERROR)
            {
                break;
            }
        }
        return ret;
    }

    public static CommError.Type printVariable(ref VisualBuf buf, int indent, char sep, string variable, bool withSep)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            if (withSep)
            {
                ret = buf.sprintf("{0}{1}", variable, sep);
            }
            else
            {
                ret = buf.sprintf("{0}: ", variable);
            }
        }

        return ret;
    }

    public static CommError.Type printVariable(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx, bool withSep)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            if (withSep)
            {
                ret = buf.sprintf("{0}[{1:d}]{2}", variable, arrIdx, sep);
            }
            else
            {
                ret = buf.sprintf("{0}[{1:d}]: ", variable, arrIdx);
            }
        }

        return ret;
    }

    public static CommError.Type printVariable(ref VisualBuf buf, int indent, char sep, string variable, string format, params object[] args)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}: ", variable);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf(format, args);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}", sep);
        }

        return ret;
    }

    public static CommError.Type printVariable(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx, string format, params object[] args)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}[{1:d}]: ", variable,arrIdx);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf(format, args);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}", sep);
        }

        return ret;
    }

    public static CommError.Type printArray(ref VisualBuf buf, int indent, char sep, string variable,Int64 count)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}[0:{1:d}]: ", variable, count);
        }

        return ret;
    }

    public static CommError.Type printString(ref VisualBuf buf, int indent, char sep, string variable, byte[] bStr)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;
        string strUni = "";
        int count = CommTypeUtil.cstrlen(bStr);

        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            ret = printMultiStr(ref buf, "    ", indent);
        }

        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            strUni = Encoding.ASCII.GetString(bStr, 0, count);
        }

        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            ret = buf.sprintf("{0}: {1}{2}", variable, strUni, sep);
        }

        return ret;
    }

    public static CommError.Type printWString(ref VisualBuf buf, int indent, char sep, string variable, Int16[] str)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;
        //int count = CommTypeUtil.wstrlen(str) + 1;
        ret = buf.sprintf("{0}:  ",variable);
        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            int len = CommTypeUtil.wstrlen(str);
            for (int i = 0; i < len; i++)
            {
                ret = buf.sprintf("0x{0:X4}", str[i]);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    break;
                }
            }
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}", sep);
        }

        return ret;
    }
    public static CommError.Type printString(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx,byte[] bStr)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;
        string strUni = "";
        int count = CommTypeUtil.cstrlen(bStr);

        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            ret = printMultiStr(ref buf, "    ", indent);
        }

        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            strUni = Encoding.ASCII.GetString(bStr, 0, count);
        }

        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            ret = buf.sprintf("{0}[{1:d}]: {2}{3}", variable, arrIdx,strUni, sep);
        }

        return ret;
    }

    public static CommError.Type printWString(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx,Int16[] str)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;
        //int count = CommTypeUtil.wstrlen(str) + 1;
        ret = buf.sprintf("{0}[{1:d}]",variable,arrIdx);
        if (ret == CommError.Type.COMM_NO_ERROR)
        {
            int len = CommTypeUtil.wstrlen(str);
            for (int i = 0; i < len; i++)
            {
                ret = buf.sprintf("0x{0:X4}", str[i]);
                if (CommError.Type.COMM_NO_ERROR != ret)
                {
                    break;
                }
            }
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}", sep);
        }

        return ret;
    }

    public static CommError.Type printTdrIP(ref VisualBuf buf, int indent, char sep, string variable, UInt32 ip)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}: ", variable);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.IP2Str(ref buf,ip);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrIP(ref VisualBuf buf, int indent, char sep, string variable,int arrIdx, UInt32 ip)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}[{1:d}]: ", variable, arrIdx);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.IP2Str(ref buf,ip);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrTime(ref VisualBuf buf, int indent, char sep, string variable, UInt32 time)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}: ", variable);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.Time2Str(ref buf,time);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrTime(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx, UInt32 time)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}[{1:d}]: ", variable, arrIdx);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.Time2Str(ref buf,time);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrDate(ref VisualBuf buf, int indent, char sep, string variable, UInt32 date)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}: ", variable);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.CommDate2Str(ref buf,date);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrDate(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx, UInt32 date)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}[{1:d}]: ", variable, arrIdx);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.CommDate2Str(ref buf,date);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrDateTime(ref VisualBuf buf, int indent, char sep, string variable, UInt64 datetime)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}: ", variable);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.CommDateTime2Str(ref buf,datetime);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

    public static CommError.Type printTdrDateTime(ref VisualBuf buf, int indent, char sep, string variable, int arrIdx, UInt64 datetime)
    {
        CommError.Type ret = CommError.Type.COMM_NO_ERROR;

        ret = printMultiStr(ref buf, "    ", indent);
        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}[{1:d}]: ", variable, arrIdx);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = CommTypeUtil.CommDateTime2Str(ref buf,datetime);
        }

        if (CommError.Type.COMM_NO_ERROR == ret)
        {
            ret = buf.sprintf("{0}",sep);
        }

        return ret;
    }

}
