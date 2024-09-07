
namespace UMI.FrameCommand
{
    public enum FRAMECMD_ID_DEF
    {
        FRAME_CMD_INVALID = 0, // 无效命令
        FRAME_CMD_PLAYERMOVE = 1, // 玩家移动
        FRAME_CMD_PLAYERSTOPMOVE = 3, // 玩家停止移动
        FRAME_CMD_ATTACKPOSITION = 4, // 攻击某个位置
        FRAME_CMD_ATTACKACTOR = 5, // 攻击某个角色
        FRAME_CMD_LEARNSKILL = 6, // 使用技能点升级技能
        FRAME_CMD_USECURVETRACKSKILL = 9, // 使用轨迹性技能
        FRAME_CMD_USECOMMONATTACK = 10, // 使用普攻技能
        FRAME_CMD_SWITCHAOUTAI = 11, // 切换角色的自动AI模式
        FRAME_CMD_SWITCHCAPTAIN = 12, // 切换玩家主控角色
        FRAME_CMD_SWITCHSUPERKILLER = 13, // 切换角色的一击必杀状态
        FRAME_CMD_SWITCHGODMODE = 14, // 切换角色的无敌状态
        FRAME_CMD_LEARNTALENT = 15, // 学习了一个天赋
        FRAME_CMD_TESTCOMMANDDELAY = 16, // 测试命令延迟时间
        FRAME_CMD_PLAYATTACKTARGETMODE = 20, // 玩家攻击目标模式
        FRAME_CMD_SVRNTFCHGKFRAMELATER = 21, // 服务器通知修改延后帧数
        FRAME_CMD_PLAYER_BUY_EQUIP = 24, // 玩家购买装备
        FRAME_CMD_PLAYER_SELL_EQUIP = 25, // 玩家出售装备
        FRAME_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE = 26, // 玩家增加局内金币
        FRAME_CMD_SET_SKILL_LEVEL = 27, // 设置技能等级
        FRAME_CMD_PLAYCOMMONATTACKTMODE = 28, // 普通切换模式
        FRAME_CMD_LOCKATTACKTARGET = 29, // 锁定攻击对象
        FRAME_CMD_Signal_Btn_Position = 30, // 按钮发送信号-位置类型
        FRAME_CMD_Signal_MiniMap_Position = 31, // 小地图发送信号-位置类型
        FRAME_CMD_Signal_MiniMap_Target = 32, // 小地图发送信号-目标类型
        FRAME_CMD_BUY_HORIZON_EQUIP = 34, // 使用视野类装备
        FRAME_CMD_PLAYER_IN_OUT_EQUIPSHOP = 35, // 玩家进入/退出装备商店
        FRAME_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP = 36, // 玩家改变当前使用的推荐装备Group
        FRAME_CMD_PLAYLASTHITMODE = 37, // 切换补刀模式
        FRAME_CMD_PLAYER_CHOOSE_EQUIPSKILL = 38, // 玩家选择装备主动技能外显
        FRAME_CMD_PLAYER_CHEAT = 39, // 测试策划用GM指令合集
    }
    public enum SC_FRAME_CMD_ID_DEF
    {
        SC_FRAME_CMD_INVALID = 0, // 无效命令
        SC_FRAME_CMD_PLAYERRUNAWAY = 192, // 玩家逃跑
        SC_FRAME_CMD_PLAYERDISCONNECT = 193, // 玩家掉线了
        SC_FRAME_CMD_PLAYERRECONNECT = 194, // 玩家重连进来了
        SC_FRAME_CMD_ASSISTSTATECHG = 195, // 托管状态变化
        SC_FRAME_CMD_CHGAUTOAI = 196, // 切换到自动战斗
        SC_FRAME_CMD_SVRNTF_GAMEOVER = 197, // 服务器通知游戏结束-用于投降
        SC_FRAME_CMD_PAUSE_RESUME_GAME = 198, // 暂停/恢复游戏
    }

    public enum CSSYNC_TYPE_DEF
    {
        CSSYNC_CMD_NULL = 0,
        CSSYNC_CMD_USEOBJECTIVESKILL = 128, // 指定目标技能 k总说从128开始 blahblahblah..
        CSSYNC_CMD_USEDIRECTIONALSKILL = 129, // 指定方向技能
        CSSYNC_CMD_USEPOSITIONSKILL = 130, // 指定位置技能
        CSSYNC_CMD_MOVE = 131, // 移动
        CSSYNC_CMD_BASEATTACK = 132, // 普通攻击
    }
}