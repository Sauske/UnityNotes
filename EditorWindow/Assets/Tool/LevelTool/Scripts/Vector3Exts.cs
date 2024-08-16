using UnityEngine;
//using Protocol;

namespace UMI
{
    public static class Vector3Exts
    {
        public static ClientV3 ToClientV3(this Vector3 v3)
        {
            return new ClientV3()
            {
                X = v3.x.ToInt(),
                Y = v3.y.ToInt(),
                Z = v3.z.ToInt()
            };
        }

        public static int ToInt(this float val)
        {
            return (int)(val * 1000);
        }

        public static Vector3 ToVector3(this ClientV3 v3)
        {
            return new Vector3()
            {
                x = v3.X * 0.001f,
                y = v3.Y * 0.001f,
                z = v3.Z * 0.001f
            };
        }
    }
}