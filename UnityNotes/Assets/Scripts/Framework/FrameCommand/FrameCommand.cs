using System;

namespace UMI.FrameCommand
{
    /// <summary>
    /// ����ṹ����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct FrameCommand<T> : IFrameCommand where T : struct, ICommandImplement
    {
        // ִ�е�֡λ
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
        /// ��������������������
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
        /// �������յ�����Ĵ���
        /// </summary>
        public void OnReceive()
        {
            cmdData.OnReceive(this);
        }

        /// <summary>
        /// Ԥ����Command
        /// </summary>
        public void Preprocess()
        {
            cmdData.Preprocess(this);
        }

        /// <summary>
        /// ִ��Command
        /// </summary>
        public void ExecCommand()
        {
            cmdData.ExecCommand(this);
        }

        /// <summary>
        /// ���»�������
        /// ������жϵ��������
        /// </summary>
        public void AwakeCommand()
        {
            cmdData.AwakeCommand(this);
        }

        public void Send()
        {
            ////ֻ����ս��״̬������֡ͬ�����������Ч��
            //if (BattleLogic.instance.isFighting)
            //{
            //    playerID = GamePlayerCenter.instance.HostPlayerId;
            //    FrameWindow.GetInstance().SendGameCmd(this, LobbyLogic.instance.inMultiGame);
            //}
        }
    }
}
