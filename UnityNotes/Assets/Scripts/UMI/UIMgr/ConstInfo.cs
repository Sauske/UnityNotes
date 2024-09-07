

namespace UMI
{

    /// <summary>
    /// 放一些AOT 需要的常量
    /// </summary>
    public class ConstInfo
    {
        public const string Prefab_Suffix = ".prefab";
        public const string UIAltas_Suffix = ".spriteatlas";
        public const string Mat_Suffix = ".mat";
        public const string Shader_Suffix = ".shader";

        public static string UIRootAAName = "UI/UIRoot" + Prefab_Suffix;        //UI的根目录
        public static string UIMaskAAName = "UI/UIMask" + Prefab_Suffix;        //UI统一遮罩

        public static string UICtrlStart = "UI";




        public const long PageNumStartIndex = 0;
        public const long PageSize = 100;

    }
}
