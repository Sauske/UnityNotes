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

    public class FrameCommandCreatorAttribute : Attribute
    {

    }
}
