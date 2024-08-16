using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ProtoBuf;

public static class Tools
{

    public static void ProtoBufTest()
    {
       
    }

    public static void CustomActive(this UnityEngine.GameObject go, bool active)
    {
        if (go != null && go.activeSelf != active)
        {
            go.SetActive(active);
        }
    }
}
