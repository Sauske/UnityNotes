using UnityEngine;
using System.Collections;
public class MiSDKManager : SDKBase {

    // 登录 
    public override void StartLogin() {
        Debug.Log("start login");
#if UNITY_ANDROID
        AuthorizeCode = "";
        CallFunc("doLogin");
#endif
    }

    /**
     *  定额支付
     *   String orderID, String info, int money,  
             String name, String servername, String token, String vipLev, String playerLve
     * 
     *  //object[] MIparam = new object[] { 订单号String, 描述 String,(int)价格,物品名 ,服务器名字, 
                 //用户余额String,VIP等级String, 角色等级String};
     * */
    public override void StartPay(object[] param) {
#if UNITY_ANDROID
        CallFunc("startPay", param);
#endif
    }

    /**
     * uid + "..." + session
     * */
    public override void LoginCallback(string result) {
        if(EventLoginCallBack != null) {
            EventLoginCallBack(result);
        }
        Debug.Log("login callback:" + result);
    }

    /**
     * 支付回调
     * **/
    public override void PayCallback(string result) {
        if(EventPayCallBack != null)
            EventPayCallBack(result);
        Debug.Log("pay callback:" + result);
    }
}
