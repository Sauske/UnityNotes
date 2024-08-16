using System;
using ProtoBuf;
using System.IO;

namespace Framework
{
    [RGameMessageHandlerClassAttribute]
    public class RGameLobbyMsgHandler
    {
        public static void SendMsg(ProtoID cmdId,IExtensible msg)
        {
         //   NetPkg pkg = NetPkg.CreateNetReqPkg((int)cmdId, msg);
        //    RGameNetworkModule.GetInstance().SendLobbyMsg(ref pkg);
        }
    }
}
