//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{

    public class RGameUtil
    {
        public static T Deserialize<T>(NetPkg pkg) where T : ProtoBuf.IExtensible
        {
            T iex = default(T);

            return iex;
        }


    }

    public enum ProtoID
    {
        SCID_RECOVERGAMEFRAP_RSP = 1,
        ProtoID_LoginReq,
        CSID_LobbyReconnect,
        CSID_GAMESVRPING,
    }

    public class CSPkg
    {

    }

    public interface IIdentifierAttribute<TIdentifier>
    {
        TIdentifier ID { get; }
        TIdentifier[] AdditionalIdList { get; }
    }


    public class RGameMessageHandlerClassAttribute:Attribute
    {

    }

    public class RGameMessageHandlerAttribute : Attribute,IIdentifierAttribute<uint>
    {
        public int MessageID;

        public uint ID { get { return (uint)MessageID; } }
        public uint[] AdditionalIdList { get { return null; } }

        public RGameMessageHandlerAttribute(int InMessageID)
        {
            MessageID = InMessageID;
        }
    }

    public class RGameRelayMessageHandlerClassAttribute : Attribute
    {

    }

    public class RGameRelayMessageHandlerAttribute : Attribute
    {
        public int MessageID;

        public uint ID { get { return (uint)MessageID; } }
        public uint[] AdditionalIdList { get { return null; } }

        public RGameRelayMessageHandlerAttribute(int InMessageID)
        {
            MessageID = InMessageID;
        }
    }

    public class RGameRelayMessageHandlerForCSPkgAttribute : Attribute
    {
         public int MessageID;

        public uint ID { get { return (uint)MessageID; } }
        public uint[] AdditionalIdList { get { return null; } }

        public RGameRelayMessageHandlerForCSPkgAttribute(int InMessageID)
        {
            MessageID = InMessageID;
        }
    }
}