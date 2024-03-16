using UnityEngine;

namespace SFramework
{
    public static class Logger
    {
        public static void Log(string msg)
        {
            Debug.Log(msg);
        }

        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }
    }
}
