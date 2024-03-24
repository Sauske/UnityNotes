using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    public static class RTTParam
    {
        public static void SetLayerRecursively(this GameObject go,int layer)
        {
            if(go != null)
            {
                go.layer = layer;
            }
        }

        public static Transform FindChildRecursively(this Transform tran,string name)
        {
            if(tran != null)
            {
                return tran.Find(name);
            }
            return new GameObject().transform;
        }
    }

    public class PlayerAnimName
    {
        public const string Stand = "Stand";
    }

    public class PhysicsLayer
    {
        public const int RTT = 10;
    }
}
