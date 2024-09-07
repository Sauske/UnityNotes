
namespace UMI
{

    /// <summary>
    /// Tcp的网络连接消息
    /// 100100 ~ 100109
    /// </summary>
    public class TCPNetEvents
    {
        public const uint TCPNetConnectSucc = 100100;           //TCP网络连接成功
        public const uint TCPNetConnectFail = 100101;           //TCP网路连接失败
        public const uint TCPNetConnectClosed = 100102;         //TCP网络关闭
        public const uint TCPNetReConnect = 100103;             //TCP网络开始重新连接
    }

    /// <summary>
    /// UDP 的网络连接消息
    /// 100110 ~ 100119
    /// </summary>
    public class UDPNetEvents
    {
        public const uint UDPNetConnectSucc = 100110;           //UDP网络连接成功
        public const uint UDPNetConnectFail = 100111;           //UDP网路连接失败
        public const uint UDPNetConnectClosed = 100112;         //UDP网络关闭
        public const uint UDPNetReConnect = 100113;             //UDP网络开始重新连接
    }

    /// <summary>
    /// 可靠UDP的网络连接消息
    /// 100120 ~ 100129
    /// </summary>
    public class RelUDPNetEvents
    {
        public const uint RelUDPNetConnectSucc = 100120;        //RelUDP网络连接成功
        public const uint RelUDPNetConnectFail = 100121;        //RelUDP网路连接失败
        public const uint RelUDPNetConnectClosed = 100122;      //RelUDP网络关闭
        public const uint RelUDPNetReConnect = 100123;          //RelUDP网络重新连接
    }

    /// <summary>
    /// WebSocket 的网路连接消息
    /// 100130 ~ 100139
    /// </summary>
    public class WebSocketEvents
    {
        public const uint WebSocketNetConnectSucc = 100130;         //WebSocket网络连接成功
        public const uint WebSocketNetConnectFail = 100131;         //WebSocket网路连接失败
        public const uint WebSocketConnectClosed = 100132;          //WebSocket网络关闭
        public const uint WebSocketDisConnect = 100133;             //WebSocket网络关闭
        public const uint WebSocketNetReConnectFail = 100134;       //WebSocket网路重连接失败
    }

    /// <summary>
    /// 权限事件
    /// 100140 ~ 100149
    /// </summary>
    public class PermissionEvents
    {
        public const uint SelectPhoto = 100140;
    }

    /// <summary>
    /// 主角的操作事件
    /// 100200 ~ 100249
    /// </summary>
    public class PlayerEvents
    {
        public const uint PlayerJumpEvent = 100200;             //角色跳跃
        public const uint MajorPlayerActionOver = 100201;             //角色动作播放完成

    }

    /// <summary>
    /// 遥感的事件
    /// 100250 ~ 100259
    /// </summary>
    public class JoystickEvents
    {
        public const uint HideJoystickEvent = 100250;               //隐藏遥感
        public const uint HideJumpBtnEvent = 100251;                //隐藏跳跃按键
        public const uint ShowJoystickEvent = 100252;               //显示遥感
        public const uint ShowJumpBtnEvent = 100253;                //显示跳跃按键

    }

    /// <summary>
    /// 场景加载事件
    /// 100260 ~ 100269
    /// </summary>
    public class LoadingEvents
    {
        public const uint StartLoadingEvent = 100260;               //开始加载
        public const uint LoadingSceneResOverEvent = 100261;        //场景资源加载完成
        public const uint LoadingAndInitOverEvent = 100262;         //加载初始化完成
        public const uint HideLoadingUIEvent = 100263;              //关闭loading界面
        public const uint LoadMainPlayerObjFinish = 100264;         //加载完主角gameobject完成
        public const uint EnterSceneSucc = 100268;                  //场景加载成功
    }

    /// <summary>
    /// 点击场景里面的物体
    /// 100270 ~ 100279
    /// </summary>
    public class HitSceneObjEvents
    {
        public const uint HitSceneObjEvent = 100270;               //点击场景里面的物体
        public const uint HitPlayerObjEvent = 100271;               //点击玩家
    }

    /// <summary>
    /// 玩家碰撞场景里的建筑
    /// 100280 ~ 100299
    /// </summary>
    public class BuildTouchEvents
    {
        public const uint PlayerEnterDoorAreaEvent = 100280;            //玩家进入门区域
        public const uint PlayerExitDoorAreaEvent = 100281;             //玩家退出门区域
        public const uint PlayerEnterChatArerEvent = 100282;            //玩家进入语音区域
        public const uint PlayerExitChatArerEvent = 100283;             //玩家退出语音区域

    }

    /// <summary>
    /// 提醒功能事件
    /// 100290 ~ 100309
    /// </summary>
    public class ReminderEvents
    {
        public const uint ReminderClickRecord = 100290;                 //玩家点击提醒列表
        public const uint ReminderItemLongPressEvent = 100291;
        public const uint ReminderChangeSuccEvent = 100292;
        public const uint ReminderEnterSelectEvent = 100293;
        public const uint ReminderExitSelectEvent = 100294;
        public const uint ReminderCreateNewEvent = 100295;
        public const uint ReminderChangeEvent = 100296;
        public const uint ReminderRecordBack = 100297;
        public const uint ReminderRecordClose = 100298;
    }

    /// <summary>
    /// UI界面的事件
    /// 101000 - 101999
    /// </summary>
    public class UIEvent
    {
        public const uint OnGameServerSelectChanged = 101000;                       //游服选择发生变化
        public const uint OnUserAvatarChanged = 101001;                             //用户Avatar改变
        public const uint UserInfoChanged = 101002;                                 //用户头像改变
        public const uint OnUgcChanged = 101003;                                    //Ugc改变
        public const uint OnShowVideoPanel = 101004;                                //显示UIAddVideoPanel
        public const uint OnShowAudioPanel = 101005;                                //显示UIAdudioPanel
        public const uint OnShowPhotoPanel = 101006;                                //显示UIPhotoPanel
        public const uint OnPersonalZoneDelPho = 101007;                            //删除个人空间图片事件
        public const uint UserNameChanged = 1001003;                                //用户修改昵称
        public const uint OnDelFriendSuc = 1001004;                                 // 删除好友成功
        public const uint OnUpdateFairyTalkText = 1001005;                          // 更新AI聊天文本

        public const uint ShowOrHideUIHallMain = 101100;                            // 大厅界面，默认风格
        public const uint ShowOrHideUIPlayerController = 101101;                    // 摇杆控制，默认风格
        public const uint ShowOrHideUIPlayerControllerWithoutRocker = 101102;       // 摇杆控制，跳跃按钮风格
    }

    /// <summary>
    /// 登录模块事件
    /// 102000 - 102999
    /// </summary>
    public class LoginEvent
    {
        public const uint OnLoginAccountSuccess = 102000;          //游服登录成功             
        public const uint OnLoginGameEnd = 102001;                 //所有模块登录结束，可进入游戏               
    }

    /// <summary>
    /// grpc网络
    /// </summary>
    public class GrpcNetEvent
    {
        public const uint OnGrpcError = 103000;                    //grpc调用发生rpcException
        public const uint SendClientGameStreamMsg = 103001;        //grpc发送流消息-->clientGame，对应gateway

        public const uint PlayerCreate = 103010;               //玩家进入
        public const uint PlayerMove = 103011;                 //玩家移动
        public const uint PlayerRemove = 103012;               //玩家退出
        public const uint SendPrivateMessage = 103013;         //新私聊消息
        public const uint SendGroupMessage = 1030104;          //新群聊消息
                                                               //
        public const uint RecvHeartBeat = 1030110;             //心跳回复


    }

    public class CommonEvent
    {
        public const uint ChangeBindPhone = 104000;                 // 修改绑定手机
        public const uint SetPassword = 104001;                     // 设置密码
        public const uint DeleteAccount = 104002;                   // 注销账号
        public const uint BindPhone = 104003;                       // 绑定手机

        public const uint BindWeChat = 104004;                      // 绑定微信
        public const uint BindIOS = 104005;                         // 绑定苹果
        public const uint BindQQ = 104006;                          // 绑定QQ

        public const uint UnbindWeChat = 104007;                    // 解绑微信
        public const uint UnbindIOS = 104008;                       // 解绑苹果
        public const uint UnbindQQ = 104009;                        // 解绑QQ

        public const uint SetNameGender = 104010;                   // 昵称性别刷新

        public const uint TouchChangeCameraDistance = 104011;       // 滑屏改变相机焦距
        public const uint UpdateFairyName = 104012;                 // 更新AI助手的名字牌
        public const uint DestroyFairyNameUI = 104013;              // 销毁AI助手的名字牌
        public const uint StartRectGuide = 104014;                  // 开始矩形框新手引导
        public const uint StartCircleGuide = 104015;                // 开始圆形框新手引导
        public const uint CloseStepGuide = 104016;                  // 结束新手引导当前步骤
        public const uint DestroyGuide = 104017;                    // 结束新手引导
        public const uint StartTaskGuide = 104018;                  // 任务驱动新手引导开启
        public const uint EndTaskGuide = 104019;                    // 任务驱动新手引导结束
        public const uint NextGuideState = 104020;                  // 下一个任务场景状态

        public const uint UpdateLocalization = 104021;              // 刷新翻译
    }

    /// <summary>
    /// IM模块
    /// 105000 - 105099
    /// </summary>
    public class IMEvent
    {
        public const uint IMGetNewMsg = 105000;            //收到新消息
        public const uint IMGetHistoryMsg = 105001;        //收到历史记录
        public const uint IMSendMsgSuccess = 105002;       //发送消息成功
        public const uint IMSendMsgFail = 105003;          //发送消息失败
        public const uint IMOnMessageUnReadCountChanged = 105004;      //未读消息数量改变通知
        public const uint IMOnJoiinGroup = 105005;         //加入群组成功
        public const uint IMOnQuitGroup = 105006;          //退出群组成功
        public const uint IMChangeMicrophoneOrSpeakerStatue = 105007;  //修改麦克风和扬声器状态
        public const uint IMJoinPhoneSuccess = 105008;     //加入语音成功
        public const uint IMJoinPhoneFail = 105009;        //加入语音失败
        public const uint IMGetNewPrivateMsg = 105010;     //收到新私聊消息
        public const uint IMGetNewGroupMsg = 105011;       //收到新群聊消息

        public const uint IMVoiceCallMsg = 105014;
        public const uint IMVoiceCallAnserRet = 105015;

        public const uint IMMicState = 105016;         //麦克风状态
        public const uint IMSpeakerState = 105017;     //喇叭状态

        public const uint IMPickupTimeout = 105018;     // 打电话超时

        public const uint OnPlayerTalkEvent = 105031;   //玩家说话
        public const uint OnSelfTalkEvent = 105032;   //自己说话
    }

    public class ScreenCaptureEvent
    {
        public const uint ScreenCaptureClose = 106000;                          // 拍照录屏退出
        public const uint ScreenCaptureOpen = 106001;                           // 拍照录屏进入
        public const uint ScreenCaptureAlbumSelect = 106002;                    // 拍照录屏相册勾选照片
        public const uint ScreenCaptureAlbumUnselect = 106003;                  // 拍照录屏相册取消勾选照片
        public const uint ScreenCaptureAlbumChooseCurrent = 106004;             // 拍照录屏相册选择当前照片
    }

    /// <summary>
    /// npc 模块相关的事件
    /// 107000 - 107999
    /// </summary>
    public class NpcEvent
    {
        public const uint RoomNpcListChange = 107000;              // 房间npc列表变动【新npc列表】
        public const uint NpcCreateSuccess = 107001;               // npc创建完成【npc实例】
        public const uint NpcDestroySuccess = 107002;              // npc移除完成【npc配置id】
        public const uint NpcInteractChange = 107003;              // 交互npc变动【npc实例】
        public const uint NpcChatStatusChange = 107004;            // npc聊天状态变动【是否聊天】
        public const uint NpcChatListChange = 107005;              // npc聊天列表变动
        public const uint NpcFairyInteractChange = 107006;         // 精灵交互变动【精灵实例】
        public const uint AddObjUIName = 107007;                    //添加3D物体2D的UI
        public const uint DestroyObjUIName = 107008;                //移除3D物体2D的UI
    }

    /// <summary>
    /// 好友管理模块
    /// 108000 - 108999
    /// </summary>
    public class FriendManagementEvent
    {
        public const uint RetrievePageData = 108011;                // 所有数据初始化

        public const uint GetRespFollowData = 108000;               // 关注页数据已返回
        public const uint GetRespFansData = 108001;                 // 粉丝页数据已返回
        public const uint GetRespMyFriendData = 108008;             // 我的好友列表数据已返回
        public const uint GetRespApplicationData = 108009;          // 申请中页数据已返回
        public const uint GetRespReviewData = 108010;               // 待审核页数据已返回

        public const uint AgreeApplication = 108002;                // 用户同意好友申请
        public const uint RefuseApplication = 108003;               // 用户拒绝好友申请
        public const uint CancelApplication = 108004;               // 用户取消已发送的好友申请

        public const uint FriendApply = 108005;                     // 发送好友申请

        public const uint Follow = 108006;                          // 关注某位用户
        public const uint Unfollow = 108007;                        // 取消关注某位用户

        public const uint IncrementFriend = 108012;
        public const uint IncrementApply = 108013;
        public const uint IncrementReceive = 108014;
        public const uint IncrementFollow = 108015;
        public const uint IncrementFans = 108016;

    }

    public class CameraEvent
    {
        public const uint SwitchCamera = 109000;                    // 选择相机
    }

    /// <summary>
    /// 任务模块
    /// 110000 - 110099
    /// </summary>
    public class TaskEvent
    {
        public const uint MainTaskStart = 110000;                   // 主线任务开始
        public const uint MainTaskExcute = 110001;                  // 主线任务执行
        public const uint MainTaskEnd = 110002;                     // 主线任务结束
        public const uint BranchTaskStart = 110003;                 // 支线任务开始
        public const uint BranchTaskExcute = 110004;                // 支线任务开始
        public const uint BranchTaskEnd = 110005;                   // 支线任务开始
        public const uint RefreshMainTask = 110006;                 // 刷新主线任务

        public const uint TaskEnd = 110010;                         // 任务结束
        public const uint TaskNewTask = 110011;                     // 服务器下发的新任务        
        public const uint TaskCanAdd = 110100;                      // 可接收任务

    }

    /// <summary>
    /// UGC模块
    /// 111000 - 111099
    /// </summary>
    public class UgcEvent
    {
        public const uint UgcCommentFresh = 111000;         // 刷新UGC评论
        public const uint UgcCommonScreen = 111001;         // 公共UGC数据
        public const uint UgcFriendScreen = 111002;         // 好友UGC数据
        public const uint UgcMeScreen = 111003;             // 本人UGC数据
    }

    /// <summary>
    /// 夜店蹦迪UI模块
    /// 111100 - 111199
    /// </summary>
    public class UIDibaEvent
    {
        public const uint DibaRoomStatesSelect = 111101;         //界面选择查询状态
        public const uint DibaRoomNoBtnClick = 111102;         //点击数字按键
        public const uint OnTimeSelectEvent = 111103;         //选择时间完成

        public const uint OnSelectDibaMusicEvent = 111104;      //选择迪吧音乐
        public const uint OnDanceStartEvent = 111105;           //开始实时跳舞

        public const uint OnDibaChangeMusicSuccessEvent = 111106;        //切换音乐成功
        public const uint OnDibaChangeRoomOwnerSuccessEvent = 111107;        //切换房主
        public const uint OnDibaFreshRoomList = 111108;        //刷新房间列表

        public const uint OnSelectDanceEvent = 111109;  //选择自动跳舞动作
        public const uint OnCancelSelectDanceEvent = 111110;  //取消选择自动跳舞动作
        public const uint OnChangeToFirstPlayerViewEvent = 111111;  //切换到第一视角
        public const uint OnChangeToFirstViewOverEvent = 111112;  //切换到第一视角结束
    }

    /// <summary>
    /// 副本
    /// </summary> <summary>
    /// 
    /// </summary>
    public static class GamePlayEvent
    {
        public const uint OnHeartBeatTimeOut = 130000;          // 副本心跳超时
        public const uint OnSceneLoadSuccess = 130001;          // 场景加载完成
    }

    public static class ZegoSDKEvent
    {
        public const uint OnRoomStateChanged = 140000;
        public const uint OnRoomUserUpdate = 140001;
        public const uint OnPublisherStateUpdate = 140002;
        public const uint OnPlayerStateUpdate = 140003;
        public const uint OnDebugError = 140004;
    }


    /// <summary>
    /// 家园
    /// 141000 - 141999
    /// </summary>
    public static class HomeEvent
    {
        public const uint OnHomeBagDataChanged = 141000;                    //家园背包数据变更
        public const uint OnHomeBagItemDataUpdate = 141001;                 //家园背包单个物品更新
        public const uint OnHomeBagAddItem = 141002;                        //家园背包新增单个物品
        public const uint OnHomeForgeRes = 141003;                          //家园锻造结果返回
        public const uint OnHomeForgeStateChanged = 141004;                 //家园锻造状态变化
        public const uint ShowUIForgeTip = 141005;                          //家园锻造提示按钮UI
        public const uint HideUIForgeTip = 141006;                          //家园锻造提示按钮UI


    }
}

