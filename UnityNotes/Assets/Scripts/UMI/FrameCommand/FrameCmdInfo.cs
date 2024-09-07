using UnityEngine.Pool;

namespace UMI.FrameCommand
{ 
    public class FRAME_CMD_INFO
    {
        public void construct(byte _type)
        {

        }
    }

    public class FRAME_CMD_PKG
    {
        public byte bCmdType;
        public FRAME_CMD_INFO stCmdInfo;

        public FRAME_CMD_PKG()
        {
            stCmdInfo = GenericPool<FRAME_CMD_INFO>.Get();
        }
    }

    public class CSDT_GAMING_CSSYNCUN
    {

    }

    public class CSDT_GAMING_CSSYNCINFO
    {
        public byte bSyncType;
        public CSDT_GAMING_CSSYNCUN stCSSyncDt;

        public CSDT_GAMING_CSSYNCINFO()
        {
            stCSSyncDt = GenericPool<CSDT_GAMING_CSSYNCUN>.Get();
        }
    }
}
