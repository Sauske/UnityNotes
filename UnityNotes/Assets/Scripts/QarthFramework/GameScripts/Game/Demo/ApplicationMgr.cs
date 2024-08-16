using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MainGame;
using Qarth;
using UnityEngine;

//会在场景中生成对应的GameObject
[TMonoSingletonAttribute("[App]/ApplicationMgr")]
public class ApplicationMgr : AbstractApplicationMgr<ApplicationMgr>
{
    public void Init()
    {
        Log.i("ApplicationMgr init");
    }
    
    protected override void ShowLogoPanel()
    {
        UIMgr.S.OpenPanel(UIID.LogoPanel);
    }

    //初始化第三方插件
    protected override IEnumerator InitThirdLibConfig()
    {
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        yield return null;
    }

    //初始化运行环境
    protected override IEnumerator InitAppEnvironment()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = false;
        yield return null;
    }

    //初始化框架
    protected override IEnumerator InitFramework()
    {
        AppConfig.S.InitAppConfig();
        ResMgr.S.Init();
        I18Mgr.S.Init();
        UIRegister.RegisterUIPanel();
        ShowLogoPanel();
        TableRegister.RegisterTable();
        yield return TableModule.PreLoadTable(null);
        yield return TableModule.DelayLoadTable(null);
    }

    //开始游戏
    protected override IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        UIMgr.S.ClosePanelAsUIID(UIID.LogoPanel);
        GameMgr.S.StartGame();
    }
}
