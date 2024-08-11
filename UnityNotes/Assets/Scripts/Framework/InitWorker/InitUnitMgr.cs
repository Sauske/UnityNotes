using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UMI
{
    /// <summary>
    /// 各种游戏管理器的初始化
    /// </summary>
    public class InitUnitMgr : InitUnit
    {
        protected override async Task OnInit()
        {
            //await Task.WhenAll(NetModule.Init(), UIMgr.Instance.Init());

            //HearBeatManager.Instance.Init();
            //// await NetModule.Init();
            //// await UIMgr.Instance.Init();
            //// 权限
            //NativePermissionMgr.Instance.Init();

            ////TODO:
            //UIHandler.Instance.RegEvent();

            //EventMgr.Instance.Init();
            //PlayerMgr.Instance.Init();
            //// 网络文件管理器
            //HttpFileMgr.Instance.Init();
            //// 自动化
            //AutomateMgr.Instance.AddAutomateLogic(new AutomateReplayShare());
            //// app 启动 参数处理
            //NativeSchemeMgr.Instance.OnCodeInitFinish();

            //// 游戏内声音系统
            //AudioMgr.Instance.Init();
            //// 地图数据
            //MapDataMgr.Instance.Init();
            //// 敏感词
            //SensitiveWordMgr.Instance.Init();
            //// 打电话
            //IMPhoneMgr.Instance.Init();
            //// 相机管理
            //await CreateCameraMgr();

            //FriendMgr.Instance.Init();
            //// 群聊天Hud
            //IMHudTalkMgr.Instance.Init();
            //UserPermissionMgr.Instance.Init();
            ////新手引导
            //GuideMgr.Instance.Init();
            ////任务系统
            //TaskMgr.Instance.Init();
            //PlayerTalkMgr.Instance.Init();
            //UmiFaceMgr.Instance.Init();
            //GamePlayerController.Instance.Init();

            await base.OnInit();
        }

        protected override async Task OnDispose()
        {
            //await NetModule.Dispose();
            //MapDataMgr.Instance.Dispose();
            //LocalizationMgr.Instance.Dispose();
            //SqliteMgr.Instance.Dispose();

            ////TODO:
            //UIHandler.Instance.UnRegEvent();
            await base.OnDispose();
        }

        /// <summary>
        /// 创建相机管理器
        /// </summary>
        private async Task CreateCameraMgr()
        {
            GameObject goInst = await Addressables.InstantiateAsync("Special/CameraMgr.prefab").Task;
            //CameraMgr cameraMgr = goInst.GetComponent<CameraMgr>();
            //cameraMgr.MountToRootNode();
            //cameraMgr.Init();
        }
    }
}