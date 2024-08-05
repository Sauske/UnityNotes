using System;
using SQLite4Unity3d;

namespace UMI
{
    public class SqliteMsgData
    {

    }

    /// <summary>
    /// 聊天消息类型
    /// </summary>
    public enum IMMsgDataType
    {
        //文本
        Txt,
        //语音
        Voice,
        //图片
        Image,
        //通话开始
        PhoneBegin,
        //通话结束
        PhoneEnd,
        //人物交互动作
        Action,
    }

    public enum IMMsgTalkType
    {
        //单聊
        Single,
        //群聊
        Group
    }

    public class IMMsgData : IComparable<IMMsgData>
    {
        const long TIME_OUT = 10;   //群聊10秒过时

        [PrimaryKey]
        public long Id { get; set; }
        /// <summary>
        /// 只有msgTalkType 为群聊的时候才有效，代表群聊id
        /// </summary>
        public long chatId { get; set; }
        public long senderId { get; set; }
        public long receiverId { get; set; }
        public string content { get; set; }
        public long createTime { get; set; }

        public IMMsgDataType msgDataType = IMMsgDataType.Txt;
        public IMMsgTalkType msgTalkType = IMMsgTalkType.Single;

        //////////////////////////////  Sqlite里使用 /////////////////////////////
        public int MsgDataType { get { return (int)msgDataType; } set { msgDataType = (IMMsgDataType)value; } }
        public int MsgTalkType { get { return (int)msgTalkType; } set { msgTalkType = (IMMsgTalkType)value; } }
        //////////////////////////////  Sqlite里使用 /////////////////////////////

        public long voiceLen = 0;
        public bool readed = false;
        public bool fileDownloaded = false;

        /// <summary>
        /// 群聊超时
        /// </summary>
        public bool GroupTimeout
        {
            get
            {
                long time = 0; //TimeTool.GetUnixTimeStamp();
                return time > createTime + TIME_OUT;
            }
        }

        public void Reset()
        {
            Id = 0;
            chatId = 0;
            senderId = 0;
            receiverId = 0;
            content = string.Empty;
            createTime = 0;
            msgDataType = IMMsgDataType.Txt;
            msgTalkType = IMMsgTalkType.Single;

            voiceLen = 0;
            readed = false;
            fileDownloaded = false;
        }

        public static IMMsgData GetNew()
        {
            return new IMMsgData();
        }

        public int CompareTo(IMMsgData data)
        {
            return this.Id.CompareTo(data.Id);
        }
    }
}
