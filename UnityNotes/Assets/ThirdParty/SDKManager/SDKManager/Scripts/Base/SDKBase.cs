using UnityEngine;
using System.Collections;

public class SDKBase : MonoBehaviour {

    public delegate void SDKCallBack(string str);
    public static SDKCallBack EventLoginCallBack;
    public static SDKCallBack EventAntiAddictionCallBack;
    public static SDKCallBack EventPayCallBack;

#if UNITY_ANDROID
    public AndroidJavaObject jo;
    public AndroidJavaObject jc;
#endif
    public string AuthorizeCode = "";
    public string UserId = "";
    public string Token = "";
    public string CDKey = "";
    public string UserName = "";
    public uint IsAdultCode = 0;

    public uint AdultCode {
        get { return IsAdultCode; }
        set { IsAdultCode = value; }
    }

    private static SDKBase Instance_;

    public static SDKBase Instance {
        get {
            return Instance_;
        }
    }

    public static void Initialize<T>() where T : SDKBase {
        if(Instance_ == null) {
            Instance_ = GameObject.FindObjectOfType(typeof(T)) as T;
            if(Instance_ == null) {
                Instance_ = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                GameObject go = GameObject.Find("Main");
                if(go == null) {
                    go = new GameObject("Main");
                }
                Instance_.transform.parent = go.transform;
            }
        }
        Instance_.InitSDK();
    }

    /// <summary>
    /// 实例化Android相关
    /// </summary>
    private void GetAndroidClass() {
#if UNITY_ANDROID
        if(jo == null) {
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }
#endif
    }

    /// <summary>
    /// 初始化SDK
    /// </summary>
    private void InitSDK() {
#if UNITY_ANDROID
        CallFunc("initSDK", new string[] { gameObject.name });
#endif
    }

    protected void CallFunc(string funcName, params object[] param) {
#if UNITY_ANDROID
        GetAndroidClass();
        if(jo == null)
            return;
        if(param == null)
            jo.Call(funcName);
        else
            jo.Call(funcName, param);
#endif
    }

    /// <summary>
    /// 登陆
    /// </summary>
    public virtual void StartLogin() { }

    /// <summary>
    /// 切换账号
    /// </summary>
    public virtual void SwitchAccount() { }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public virtual void DoQuite() { }

    /// <summary>
    /// 防沉迷验证
    /// </summary>
    /// <param name="userid "></param>
    /// <param name="accessToken"></param>
    public virtual void DoAntiAddictionQuery(string userid, string accessToken) { }

    /// <summary>
    /// 实名注册
    /// </summary>
    /// <param name="userid"></param>
    public virtual void DoRealNameRegister(string userid) { }

    /// <summary>
    /// 打开论坛
    /// </summary>
    public virtual void DoBBSPost() { }

    /// <summary>
    /// 定额支付
    /// </summary>
    public virtual void StartPay(object[] param) { }

    /// <summary>
    /// 不定额支付
    /// </summary>
    /// <param name="param"></param>
    public virtual void StartPay1(object[] param) { }

    /// <summary>
    /// 登陆回调
    /// </summary>
    /// <param name="result"></param>
    public virtual void LoginCallback(string result) { }

    /// <summary>
    /// 支付回调
    /// </summary>
    /// <param name="result"></param>
    public virtual void PayCallback(string result) { }

    /// <summary>
    /// 防沉迷回调
    /// </summary>
    /// <param name="result"></param>
    public virtual void QueryCallBack(string result) { }

    /// <summary>
    /// 实名注册回调
    /// </summary>
    /// <param name="result"></param>
    public virtual void RealRegCallBack(string result) { }

    /**
     * 获取登录会话session
     * 
     * */
    public virtual void GetSessionID() { }

    /**
     * 设置玩家选择的游戏分区及角色信息 
     **/
    public virtual void NotifyZoneInfo(string zoneName, string roleId, string roleName) { }

    public virtual void AddExtendData(string json) { }

    public virtual void onReLoginCallback(string result) { }

    public virtual void ShowGameCenter() { }

    public virtual void onShowGameCenterCallback(string result) { }

    public virtual void GetUserBalance() { }

    public virtual void onGetUserBalanceCallback(string result) { }

    public virtual void onADClose(string result) { }

    /// <summary>
    /// 清空平台数据
    /// </summary>
    public void ResetUserInfo() {
        UserName = "";
        UserId = "";
        CDKey = "";
        Token = "";
    }

    /// <summary>
    /// 设置平台数据
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="token"></param>
    /// <param name="cdkey"></param>
    public void SetUserInfo(string username, string userid, string token, string cdkey) {
        UserName = username;
        UserId = userid;
        CDKey = cdkey;
        Token = token;
    }    
}
