namespace UMI
{
    public enum TriggerShape
    {
        Box2D = 0,
        Box = 1,
        Sphere,      //球形
        Rectangle,   //长方形
    }

    public enum TriggerCondition
    {
        In = 0,
        Out = 1,
        KeepTime = 2
    }

    public enum TriggerEffect
    {
        /// <summary>
        /// 用来测试，触发后输出log
        /// </summary>
        Default_Log = 0,
    }
}
