using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;

namespace UMI.FrameCommand
{
    public delegate IFrameCommand CreatorDelegate(ref FRAME_CMD_PKG msg);
    public delegate IFrameCommand CreatorSCSyncDelegate();
    public delegate IFrameCommand CreatorCSSyncDelegate(ref CSDT_GAMING_CSSYNCINFO msg);

    public class FrameCommandFactory
    {
        //public static Dictionary<FRAMECMD_ID_DEF, CreatorDelegate> s_CommandCreator = new Dictionary<FRAMECMD_ID_DEF, CreatorDelegate>();

        public static CreatorDelegate[] s_CommandCreator = null;
        public static CreatorCSSyncDelegate[] s_CSSyncCommandCreator = null;
        //public static CreatorSCSyncDelegate[] s_SCSyncCommandCreator = null;  //暂时还不需要SC命令的Creator

        public static Dictionary<Type, FRAMECMD_ID_DEF> s_CommandTypeDef = new Dictionary<Type, FRAMECMD_ID_DEF>();

        public static Dictionary<Type, CSSYNC_TYPE_DEF> s_CSSyncCommandTypeDef = new Dictionary<Type, CSSYNC_TYPE_DEF>();

        public static Dictionary<Type, SC_FRAME_CMD_ID_DEF> s_SCSyncCommandTypeDef = new Dictionary<Type, SC_FRAME_CMD_ID_DEF>();

        public static void PrepareRegisterCommand()
        {
            Array Values = Enum.GetValues(typeof(FRAMECMD_ID_DEF));

            int MaxValue = 0;

            for (int i = 0; i < Values.Length; ++i)
            {
                int Value = Convert.ToInt32(Values.GetValue(i));

                if (Value > MaxValue)
                {
                    MaxValue = Value;
                }
            }

            s_CommandCreator = new CreatorDelegate[MaxValue + 1];

            Values = Enum.GetValues(typeof(CSSYNC_TYPE_DEF));
            MaxValue = 0;

            for (int i = 0; i < Values.Length; ++i)
            {
                int Value = Convert.ToInt32(Values.GetValue(i));

                if (Value > MaxValue)
                {
                    MaxValue = Value;
                }
            }
            s_CSSyncCommandCreator = new CreatorCSSyncDelegate[MaxValue + 1];

            /*Values = Enum.GetValues(typeof(CSProtocol.SC_FRAME_CMD_ID_DEF));
            MaxValue = 0;

            for (int i = 0; i < Values.Length; ++i)
            {
                int Value = Convert.ToInt32(Values.GetValue(i));

                if (Value > MaxValue)
                {
                    MaxValue = Value;
                }
            }
            s_SCSyncCommandCreator = new CreatorSCSyncDelegate[MaxValue + 1];*/
        }

        public static void RegisterCommandCreator(FRAMECMD_ID_DEF CmdID, Type CmdType, CreatorDelegate Creator)
        {
            s_CommandCreator[(int)CmdID] = Creator;

            s_CommandTypeDef.Add(CmdType, CmdID);
        }

        public static void RegisterCSSyncCommandCreator(CSSYNC_TYPE_DEF CmdID, Type CmdType, CreatorCSSyncDelegate Creator)
        {
            s_CSSyncCommandCreator[(int)CmdID] = Creator;

            s_CSSyncCommandTypeDef.Add(CmdType, CmdID);
        }

        public static void RegisterSCSyncCommandCreator(SC_FRAME_CMD_ID_DEF CmdID, Type CmdType, CreatorSCSyncDelegate Creator)
        {
            s_SCSyncCommandTypeDef.Add(CmdType, CmdID);
        }

        /// <summary>
        /// 创建明确的命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FrameCommand<T> CreateFrameCommand<T>() where T : struct, ICommandImplement
        {
            FrameCommand<T> cmd = new FrameCommand<T>();

            cmd.isCSSync = false;
            cmd.cmdType = (byte)s_CommandTypeDef[typeof(T)];
            cmd.cmdData = new T();
            return cmd;
        }

        public static FrameCommand<T> CreateCSSyncFrameCommand<T>() where T : struct, ICommandImplement
        {
            FrameCommand<T> cmd = new FrameCommand<T>();

            cmd.isCSSync = true;
            cmd.cmdType = (byte)s_CSSyncCommandTypeDef[typeof(T)];
            cmd.cmdData = new T();
            return cmd;
        }

        public static FrameCommand<T> CreateSCSyncFrameCommand<T>() where T : struct, ICommandImplement
        {
            FrameCommand<T> cmd = new FrameCommand<T>();

            cmd.isCSSync = false;
            cmd.cmdType = (byte)s_SCSyncCommandTypeDef[typeof(T)];
            cmd.cmdData = new T();
            return cmd;
        }

        /// <summary>
        /// 通过命令ID创建命令
        /// </summary>
        /// <param name="CmdID"></param>
        /// <returns></returns>
        public static IFrameCommand CreateFrameCommand(ref FRAME_CMD_PKG msg)
        {
            if (msg.bCmdType >= 0 && msg.bCmdType < s_CommandCreator.Length)
            {
                CreatorDelegate Creator = s_CommandCreator[msg.bCmdType];

                Debug.LogFormat("Creator is null at index {0}", msg.bCmdType);

                return Creator(ref msg);
            }
            else
            {
                Debug.LogFormat("not register framec ommand creator {0}", msg.bCmdType);
            }

            return null;
        }


        /// <summary>
        /// 通过命令ID创建命令
        /// </summary>
        /// <param name="CmdID"></param>
        /// <returns></returns>
        public static IFrameCommand CreateFrameCommandByCSSyncInfo(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            if (msg.bSyncType >= 0 && msg.bSyncType < s_CSSyncCommandCreator.Length)
            {
                CreatorCSSyncDelegate Creator = s_CSSyncCommandCreator[msg.bSyncType];

                Debug.LogFormat("Creator is null at index {0}", msg.bSyncType);
                return Creator(ref msg);
            }
            else
            {
                Debug.LogFormat("not register framec ommand creator {0}", msg.bSyncType);
            }

            return null;
        }

        /// <summary>
        /// 为命令构造与之匹配的协议包
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static FRAME_CMD_PKG CreateCommandPKG(IFrameCommand cmd)
        {
            FRAME_CMD_PKG pkg = GenericPool<FRAME_CMD_PKG>.Get();
            pkg.bCmdType = cmd.cmdType;
            pkg.stCmdInfo.construct(pkg.bCmdType);
            return pkg;
        }

        public static FRAMECMD_ID_DEF GetCommandType(Type t)
        {
            object[] Attributes = t.GetCustomAttributes(typeof(FrameCommandClassAttribute), false);

            if (Attributes.Length > 0)
            {
                return ((FrameCommandClassAttribute)Attributes[0]).ID;
            }

            return FRAMECMD_ID_DEF.FRAME_CMD_INVALID;
        }
    }
}
