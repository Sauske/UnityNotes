public class CommError
{   public enum Type
    {
        COMM_NO_ERROR = 0,
        COMM_ERR_SHORT_BUF_FOR_WRITE = -1,
        COMM_ERR_SHORT_BUF_FOR_READ = -2,
        COMM_ERR_STR_LEN_TOO_BIG = -3,
        COMM_ERR_STR_LEN_TOO_SMALL = -4,
        COMM_ERR_STR_LEN_CONFLICT = -5,
        COMM_ERR_MINUS_REFER_VALUE = -6,
        COMM_ERR_REFER_SURPASS_COUNT = -7,
        COMM_ERR_ARG_IS_NULL = -8,
        COMM_ERR_CUTVER_TOO_SMALL = -9,
        COMM_ERR_CUTVER_CONFILICT = -10,
        COMM_ERR_PARSE_IP_FAILED = -11,
        COMM_ERR_INVALID_IP_VALUE = -12,
        COMM_ERR_INVALID_COMMTIME_VALUE = -13,
        COMM_ERR_INVALID_COMMDATE_VALUE = -14,
        COMM_ERR_INVALID_COMMDATETIME_VALUE = -15,
        COMM_ERR_FUNC_LOCALTIME_FAILED = -16,
        COMM_ERR_INVALID_HEX_STR_LEN = -17,
        COMM_ERR_INVALID_HEX_STR_FORMAT = -18,
        COMM_ERR_INVALID_BUFFER_PARAMETER = -19,
        COMM_ERR_NET_CUTVER_INVALID = -20,
        COMM_ERR_ACCESS_VILOATION_EXCEPTION = -21,
        COMM_ERR_ARGUMENT_NULL_EXCEPTION = -22,
        COMM_ERR_USE_HAVE_NOT_INIT_VARIABLE_ARRAY = -23,
        COMM_ERR_INVALID_FORMAT = -24,
        COMM_ERR_HAVE_NOT_SET_SIZEINFO = -25,
        COMM_ERR_VAR_STRING_LENGTH_CONFILICT = -26,
    }

    private static string[] errorTab = new string[]
    {
        /* 0*/"no error",
        /* 1*/"available free space in buffer is not enough",
        /* 2*/"available data in buffer is not enough",
        /* 3*/"string length surpass defined size",
        /* 4*/"string length smaller than min string length",
        /* 5*/"string sizeinfo inconsistent with real length",
        /* 6*/"reffer value can not be minus",
        /* 7*/"reffer value bigger than count or size",
        /* 8*/"pointer-type argument is NULL",
        /* 9*/"cut-version is smaller than base-version",
        /*10*/"cut-version not covers entry refered by versionindicator",
        /*11*/"inet_ntoa failed when parse ip_t",
        /*12*/"value variable of ip_t is invalid",
        /*13*/"value variable of time_t is invalid",
        /*14*/"value variable of date_t is invalid",
        /*15*/"value variable of datetime_t is invalid",
        /*16*/"function 'localtime' or 'localtime_r' failed",
        /*17*/"invalid hex-string length, must be an even number",
        /*18*/"invalid hex-string format, each character must be a hex-digit",
        /*19*/"NULL array as parameter",
        /*20*/"cutVer from net-msg not in [BASEVERSION, CURRVERSION]",
        /*21*/"assess voliation exception cause by ptr is null , or bad formation",
        /*22*/"argument null exception cause by arguments is null",
        /*23*/"variable array have not alloc memory,you must alloc memory before use",
        /*24*/"invalid string format cause FORMATEXCETPION",
        /*25*/"the meta have not set sizeinfo attribute ",
        /*26*/"string/wstring length confilict with meta file",
    };

    public static string getErrorString(Type errorCode)
    {
        int errorIndex = -1 * (int)errorCode;
        if (0 > errorIndex || errorIndex >= errorTab.GetLength(0))
        {
            return errorTab[0];
        }else
        {
            return errorTab[errorIndex];
        }
    }
}

