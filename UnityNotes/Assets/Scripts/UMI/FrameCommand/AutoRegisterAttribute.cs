using System;

namespace UMI.FrameCommand
{
    public class FrameCommandClassAttribute :Attribute,IIdentifierAttribute<FRAMECMD_ID_DEF>
    {
        public FRAMECMD_ID_DEF CmdID;

        public FRAMECMD_ID_DEF ID { get { return CmdID; } }
        public FRAMECMD_ID_DEF[] AdditionalIdList { get { return null; } }

        public FrameCommandClassAttribute(FRAMECMD_ID_DEF InCmdID)
        {
            CmdID = InCmdID;
        }
    }

    public class FrameCSSYNCCommandClassAttribute :Attribute,IIdentifierAttribute<CSSYNC_TYPE_DEF>
    {
        public CSSYNC_TYPE_DEF CmdID;

        public CSSYNC_TYPE_DEF ID { get { return CmdID; } }
        public CSSYNC_TYPE_DEF[] AdditionalIdList { get { return null; } }

        public FrameCSSYNCCommandClassAttribute(CSSYNC_TYPE_DEF InCmdID)
        {
            CmdID = InCmdID;
        }
    }

    public class FrameSCSYNCCommandClassAttribute :Attribute,IIdentifierAttribute<SC_FRAME_CMD_ID_DEF>
    {
        public SC_FRAME_CMD_ID_DEF CmdID;

        public SC_FRAME_CMD_ID_DEF ID { get { return CmdID; } }
        public SC_FRAME_CMD_ID_DEF[] AdditionalIdList { get { return null; } }

        public FrameSCSYNCCommandClassAttribute(SC_FRAME_CMD_ID_DEF InCmdID)
        {
            CmdID = InCmdID;
        }
    }

    public class FrameCommandCreatorAttribute : Attribute
    {

    }
}
