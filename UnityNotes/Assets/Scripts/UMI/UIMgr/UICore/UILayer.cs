

namespace UnityEngine.UI
{
    /// <summary>
    /// UI的层级
    /// </summary>
    public enum UILayer
    {
        Bottom,                     //最底层，全局点击响应
        ActorHP,                    //玩家的血条，名称
        Menu,                       //主界面
        Frame,                      //一级界面

        Dialog_Bottom,
        Dialog_Middle,
        Dialog_Top,                 //对话框

        Tip_Bottom,
        Tip_Middle,
        Tip_Top,                    //tips 提示

        Loading,                    //加载界面 显示在次上面
        Guide,                    //新手引导界面 显示在最上面

        Max,
    }
}
