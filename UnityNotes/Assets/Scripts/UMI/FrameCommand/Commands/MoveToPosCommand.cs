using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor.PackageManager;
using UnityEngine;

namespace UMI.FrameCommand
{

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    [FrameCommandClassAttribute(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERMOVE)]
    public struct MoveToPosCommand : ICommandImplement
    {
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<MoveToPosCommand> cmd = FrameCommandFactory.CreateFrameCommand<MoveToPosCommand>();
            //cmd.cmdData.destPosition = CommonTools.ToVector3(msg.stCmdInfo.stCmdPlayerMove.stWorldPos);
            return cmd;
        }

        public VInt3 destPosition;

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            //CommonTools.FromVector3(destPosition, ref msg.stCmdInfo.stCmdPlayerMove.stWorldPos);
            return true;
        }

        public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
        {
            return true;
        }

        public void OnReceive(IFrameCommand cmd)
        {
        }
        public void Preprocess(IFrameCommand cmd)
        {
        }

        public void ExecCommand(IFrameCommand cmd)
        {
            //var player = GamePlayerCenter.GetInstance().GetPlayer(cmd.playerID);
            //if (player != null && player.Captain)
            //{
            //    player.Captain.handle.ActorControl.CmdMovePosition(cmd, destPosition);
            //    if (player.m_bMoved == false)
            //    {
            //        player.m_bMoved = true;
            //        EventRouter.instance.BroadCastEvent(EventID.FirstMoved, player);
            //    }
            //}
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
            //var player = GamePlayerCenter.GetInstance().GetPlayer(cmd.playerID);
            //if (player != null && player.Captain)
            //{
            //    player.Captain.handle.ActorControl.CmdMovePosition(cmd, destPosition);
            //    if (player.m_bMoved == false)
            //    {
            //        player.m_bMoved = true;
            //        EventRouter.instance.BroadCastEvent(EventID.FirstMoved, player);
            //    }
            //}
        }
    }
}
