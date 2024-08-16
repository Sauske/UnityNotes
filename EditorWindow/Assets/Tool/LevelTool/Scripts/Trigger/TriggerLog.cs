using System.Collections.Generic;
using UnityEngine;

namespace UMI
{
    public class TriggerLog : TriggerAreaRuntime
    {
        protected override void OnTriggerIn(List<Transform> res)
        {
            foreach (var item in res) Debug.Log("进入 " + item.name);
        }

        protected override void OnTriggerOut(List<Transform> res)
        {
            foreach (var item in res) Debug.Log("退出 " + item.name);
        }
    }
}