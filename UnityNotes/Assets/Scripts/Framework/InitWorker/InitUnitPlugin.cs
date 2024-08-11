using UnityEngine;
using System.Threading.Tasks;

namespace UMI
{
    /// <summary>
    /// 插件的初始化
    /// </summary>
    public class InitUnitPlugin : InitUnit
    {
        protected override async Task OnInit()
        {
            await base.OnInit();
        }
    }
}