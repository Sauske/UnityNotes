namespace UnityWebSocket
{
    public class NetId
    {

        /// <summary>
        /// 游客登录消息
        /// </summary>
        public const ushort NET_LOGIN_VISITOR = 10000;

        /// <summary>
        /// Token登录
        /// </summary>
        public const ushort NET_LOGIN_TOKEN = 10002;

        /// <summary>
        /// 微信登录
        /// </summary>
        public const ushort NET_LOGIN_WECHAT = 10003;

        /// <summary>
        /// 手机登录
        /// </summary>
        public const ushort NET_LOGIN_PHONE = 10004;

        /// <summary>
        /// Facebook登录
        /// </summary>
        public const ushort NET_LOGIN_FACEBOOK = 10005;


        /// <summary>
        /// 心跳
        /// </summary>
        public const ushort NET_HEART = 10011;

        /// <summary>
        /// 获取玩家信息
        /// </summary>
        public const ushort NET_GET_PLAYER_MSG = 10012;

        /// <summary>
        /// 重新进入游戏
        /// </summary>
        public const ushort NET_REENTER_GAME = 10013;

        /// <summary>
        /// 创建角色
        /// </summary>
        public const ushort NET_CREATE_ROLE = 10014;

        /// <summary>
        /// 玩家拥有的角色
        /// </summary>
        public const ushort NET_ROLES_INFO = 10015;

        /// <summary>
        /// 玩家好友信息
        /// </summary>
        public const ushort NET_FRIEND_INFO = 10016;

        /// <summary>
        /// 个人资料
        /// </summary>
        public const ushort NET_PLAYER_INFO = 10017;

        /// <summary>
        /// 回复用户隐私开关设置
        /// </summary>
        public const ushort NET_UPDATE_SET = 10018;

        /// <summary>
        /// 回复修改昵称
        /// </summary>
        public const ushort NET_CHANGE_NAME = 10019;

        /// <summary>
        /// 回复修改性别
        /// </summary>
        public const ushort NET_CHANGE_SEX = 10020;

        /// <summary>
        /// 回复修改头像
        /// </summary>
        public const ushort NET_CHANGE_HEAD = 10021;

        /// <summary>
        /// 回复修改展示角色
        /// </summary>
        public const ushort NET_CHANGE_ROLE = 10022;

        /// <summary>
        /// 回复用户下单
        /// </summary>
        public const ushort NET_SHOP_USER_ORDER = 10031;

        /// <summary>
        /// 统一回复用户发货结果
        /// </summary>
        public const ushort NET_SHOP_USER_BUY_GOODS = 10032;

        /// <summary>
        /// ios交易补单回应
        /// </summary>
        public const ushort NET_SHOP_IOS_SUPPLE = 10035;

        /// <summary>
        /// google交易补单回应
        /// </summary>
        public const ushort NET_SHOP_GOOGLE_SUPPLE = 10036;

        /// <summary>
        /// 回应个人牌局记录
        /// </summary>
        public const ushort NET_CARD_RECORD = 10037;

        /// <summary>
        /// 用户购买物品
        /// </summary>
        public const ushort NET_SHOP_BUY = 10038;


        /// <summary>
        /// 匹配进入房间
        /// </summary>
        public const ushort NET_MATCH_ENTER_ROOM = 10101;

        /// <summary>
        /// 离开房间
        /// </summary>
        public const ushort NET_LEAVE_ROOM = 10102;

        /// <summary>
        /// 增加玩家
        /// </summary>
        public const ushort NET_ADD_PLAYER = 10103;

        /// <summary>
        /// 换桌
        /// </summary>
        public const ushort NET_RECHANGE_ROOM = 10104;




        /// <summary>
        /// 游戏中进入房间
        /// </summary>
        public const ushort NET_PVP_ENTER_ROOM = 20001;

        /// <summary>
        /// 添加玩家
        /// </summary>
        public const ushort NET_PVP_ADD_PLAYER = 20002;

        /// <summary>
        /// 玩家准备好
        /// </summary>
        public const ushort NET_PVP_READY = 20003;

        /// <summary>
        /// 开始游戏
        /// </summary>
        public const ushort NET_PVP_START_GAME = 20004;

        /// <summary>
        /// 骰子发牌
        /// </summary>
        public const ushort NET_PVP_DICES = 20005;

        /// <summary>
        /// 发牌
        /// </summary>
        public const ushort NET_PVP_TAKE_CARDS = 20006;

        /// <summary>
        /// 补花
        /// </summary>
        public const ushort NET_PVP_ADD_FLOWER = 20007;

        /// <summary>
        /// 打牌
        /// </summary>
        public const ushort NET_PVP_PLAY_CARDS = 20008;

        /// <summary>
        /// 摸牌
        /// </summary>
        public const ushort NET_PVP_TOUCH_CARDS = 20009;

        /// <summary>
        /// 操作牌吃碰杠
        /// </summary>
        public const ushort NET_PVP_OPERATE_CARDS = 20010;

        /// <summary>
        /// 胡牌
        /// </summary>
        public const ushort NET_PVP_HU_CARDS = 20011;

        /// <summary>
        /// 结算
        /// </summary>
        public const ushort NET_PVP_SETTLEMENT = 20012;

        /// <summary>
        /// 玩家中途状态改变
        /// </summary>
        public const ushort NET_PVP_STATE_CHANGE = 20014;

        /// <summary>
        /// 回应玩家设置能胡就胡和只胡自摸
        /// </summary>
        public const ushort NET_PVP_HU_STATE = 20015;

        /// <summary>
        /// 牌局流水详情
        /// </summary>
        public const ushort NET_PVP_END_MSG = 20016;

        /// <summary>
        /// 牌局记录
        /// </summary>
        public const ushort NET_PVP_RECORD = 20030;












        /// <summary>
        /// 系统提示，并且断掉客户端
        /// </summary>
        public const ushort NET_SYSTEM_TIPS = 50000;

        /// <summary>
        /// 更新玩家信息
        /// </summary>
        public const ushort NET_PLAYER_UPDATE = 50001;

        /// <summary>
        /// 获取邮件
        /// </summary>
        public const ushort NET_MAIL_GET = 50101;

        /// <summary>
        /// 回复读邮件
        /// </summary>
        public const ushort NET_MAIL_READ = 50102;

        /// <summary>
        /// 回复领取邮件
        /// </summary>
        public const ushort NET_MAIL_GET_GOODS = 50103;

        /// <summary>
        /// 删除已读邮件回复
        /// </summary>
        public const ushort NET_MAIL_DELETE = 50103;

        /// <summary>
        /// 获取背包数据
        /// </summary>
        public const ushort NET_GET_ITEM = 50201;

        /// <neymar>
        /// 更新背包数据
        /// </neymar>
        public const ushort NET_UPDATA_ITEM = 50202;

        /// <neymar>
        /// 系统消息
        /// </neymar>
        public const ushort NET_SYSTEM_MESSAGE = 50301;

        /// <neymar>
        /// 世界聊天
        /// </neymar>
        public const ushort NET_WORLD_MESSAGE = 50302;

        /// <neymar>
        /// 好友聊天
        /// </neymar>
        public const ushort NET_FRIEND_MESSAGE = 50303;
    }
}
