
using System;

namespace UMI.FrameCommand
{
    /// <summary>
    /// 命令基类接口
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
    /// 命令实现体接口
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
