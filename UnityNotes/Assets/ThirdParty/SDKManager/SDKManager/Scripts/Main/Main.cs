using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
        
    void Awake() {
        Initialize();
    }

    void OnDestroy() {
        SDKBase.EventLoginCallBack -= SendAuthorize;
        SDKBase.EventAntiAddictionCallBack -= AntiAddictionCallBack;
        SDKBase.EventPayCallBack -= PayCallBack;
    }

    /// <summary>
    /// 根据平台做SDK初始化
    /// </summary>
    private void Initialize() {
#if UNITY_ANDROID || UNITY_EDITOR
        switch(GlobalConst.GAMEPLATFORM) {
            case PLATFORMTYPE.Android_MI:
                SDKBase.Initialize<MiSDKManager>();
                break;
            case PLATFORMTYPE.Android_360:
                SDKBase.Initialize<QihooSDKManager>();
                break;
            case PLATFORMTYPE.Android_UC:
                SDKBase.Initialize<UcSDKManager>();
                break;
            default:
                break;
        }
#endif
        SDKBase.EventLoginCallBack += SendAuthorize;
        SDKBase.EventAntiAddictionCallBack += AntiAddictionCallBack;
        SDKBase.EventPayCallBack += PayCallBack;
    }

    public void SendAuthorize(string str) {
        if(str == "loginError") {
            //登陆失败错误处理
            return;
        }
        //登陆成功根据不同的平台解析参数 发送服务器进入游戏(根据自己游戏而来)
    }

    public void AntiAddictionCallBack(string str) {
        uint code = 1;

        if(str == "1") {
            // 未成年
            code = 0;
        }
        else if(str == "2") {
            // 已成年
            code = 1;
        }
        UcSDKManager.Instance.AdultCode = code;
        //通知服务器处理 
    }

    public void PayCallBack(string str) {
        if(str == "0000") {
            // 失败
        }
        else if(str == "9999") {
            //可以自己先存一份购买的请求数据在这里做处理 也可以等服务器反馈 不做任何处理因为平台会通知服务器
        }
    }

    //调用方式大概如此 在合适的方法中调用
    public void OnGUI() {
        if(GUILayout.Button("Login")) {
            SDKBase.Instance.StartLogin();
        }

        if(GUILayout.Button("SwitchAccount")) {
            SDKBase.Instance.SwitchAccount();
        }

        if(GUILayout.Button("DoQuite")) {
            SDKBase.Instance.DoQuite();
        }

        if(GUILayout.Button("startPay")) {
            //根据不同的平台构造参数
            //ScheduleSDKManager.Instance.mSDKManager.StartPay();
        }

        if(GUILayout.Button("startPay1")) {
            //根据不同的平台构造参数
            //ScheduleSDKManager.Instance.mSDKManager.StartPay1();
        }
    }
}
