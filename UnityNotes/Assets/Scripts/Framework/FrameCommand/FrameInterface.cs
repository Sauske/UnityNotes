
using System;

namespace UMI.FrameCommand
{
    /// <summary>
    /// �������ӿ�
    /// </summary>
    public interface IFrameCommand
    {
        byte cmdType { get; set; }
        UInt32 cmdId { get; set; }
        UInt32 frameNum { get; set; }
        UInt32 playerID { get; set; }
        bool isCSSync { get; set; }
        byte sendCnt { get; set; }

        bool TransProtocol(FRAME_CMD_PKG msg);
        bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg);

        void OnReceive();
        void Preprocess();
        void ExecCommand();
        void AwakeCommand();

        void Send();
    }

    /// <summary>
    /// ����ʵ����ӿ�
    /// </summary>
    public interface ICommandImplement //
    {
        bool TransProtocol(FRAME_CMD_PKG msg);//
        bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg);

        void OnReceive(IFrameCommand cmd);
        void Preprocess(IFrameCommand cmd);
        void ExecCommand(IFrameCommand cmd);
        void AwakeCommand(IFrameCommand cmd);
    }

    public interface IIdentifierAttribute<TIdentifier>
    {
        TIdentifier ID { get; }
        TIdentifier[] AdditionalIdList { get; }
    }
}
