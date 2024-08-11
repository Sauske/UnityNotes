using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace UMI.Net
{
    public class RUDP_API
    {

#if UNITY_IPHONE || UNITY_XBOX360

    // On iOS and Xbox 360 plugins are statically linked into
    // the executable, so we have to use __Internal as the
    // library name

    private const string plugin_name = "__Internal";
#else
        // Other platforms load plugins dynamically, so pass the name
        // of the plugin's dynamic library.
        private const string plugin_name = "RUDPExport";

#endif
        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "CreateSocket")]
        public static extern void CreateSocket(int clientPort);

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "CloseSocket")]
        public static extern void CloseSocket();

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Connect")]
        public static extern int Connect([MarshalAs(UnmanagedType.LPArray)] byte[] ip, int svrPort, int clientPort);

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ConnectWithIp")]
        public static extern int ConnectWithIp(int ip, int svrPort, int clientPort);

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DisConnect")]
        public static extern void DisConnect();

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Receive")]
        public static extern int Receive([MarshalAs(UnmanagedType.LPArray)] byte[] buff);

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Send")]
        public static extern int Send([MarshalAs(UnmanagedType.LPArray)] byte[] buff, int buffSize);

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Readable")]
        public static extern bool Readable();

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetConnectState")]
        public static extern int GetConnectState();

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetLogPath")]
        public static extern void SetLogPath([MarshalAs(UnmanagedType.LPArray)] byte[] szPath, int nPathLen);

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FinalizeLog")]
        public static extern void FinalizeLog();

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Process")]
        public static extern void Process();

        [DllImport(plugin_name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "EndSend")]
        public static extern void EndSend();

        public static void CreateRUDPConnection(string ip, int svrPort, int clientPort)
        {
            byte[] ipBuff = System.Text.Encoding.ASCII.GetBytes(ip);
            Connect(ipBuff, svrPort, clientPort);
        }
    }
}
