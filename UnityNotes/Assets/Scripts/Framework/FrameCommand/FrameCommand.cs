using System;

namespace UMI.FrameCommand
{
    /// <summary>
    /// 命令结构泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct FrameCommand<T> : IFrameCommand where T : struct, ICommandImplement
    {
        // 执行的帧位
        private byte _sendCnt;
        private bool _isCSSync;
        private UInt32 _playerID;
        private UInt32 _frameNum;
        private UInt32 _cmdId;
        private byte _cmdType;

        public T cmdData;

        public UInt32 cmdId
        {
            get { return _cmdId; }
            set { _cmdId = value; }
        }

        public byte cmdType
        {
            get { return _cmdType; }
            set { _cmdType = value; }
        }

        public UInt32 frameNum
        {
            get { return _frameNum; }
            set { _frameNum = value; }
        }

        public UInt32 playerID
        {
            get { return _playerID; }
            set { _playerID = value; }
        }

        public bool isCSSync
        {
            get { return _isCSSync; }
            set { _isCSSync = value; }
        }

        public byte sendCnt
        {
            get { return _sendCnt; }
            set { _sendCnt = value; }
        }

        /// <summary>
        /// 将命令打包成网络数据流
        /// </summary>
        /// <param name = "msg" ></ param >
        /// < returns ></ returns >
        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            msg.bCmdType = cmdType;
            return cmdData.TransProtocol(msg);
        }

        public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.bSyncType = cmdType;
            return cmdData.TransProtocol(msg);
        }

        /// <summary>
        /// 从网络收到命令的处理
        /// </summary>
        public void OnReceive()
        {
            cmdData.OnReceive(this);
        }

        /// <summary>
        /// 预处理Command
        /// </summary>
        public void Preprocess()
        {
            cmdData.Preprocess(this);
        }

        /// <summary>
        /// 执行Command
        /// </summary>
        public void ExecCommand()
        {
            cmdData.ExecCommand(this);
        }

        /// <summary>
        /// 重新唤醒命令
        /// 在命令被中断的情况下用
        /// </summary>
        public void AwakeCommand()
        {
            cmdData.AwakeCommand(this);
        }

        public void Send()
        {
            ////只有在战斗状态产生的帧同步输入才是有效的
            //if (BattleLogic.instance.isFighting)
            //{
            //    playerID = GamePlayerCenter.instance.HostPlayerId;
            //    FrameWindow.GetInstance().SendGameCmd(this, LobbyLogic.instance.inMultiGame);
            //}
        }
    }
}
