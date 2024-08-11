using System;
using System.Text;
using UnityEngine;
using System.Reflection;


namespace UnityWebSocket
{
    public class ResEventArgs : EventArgs
    {
        private byte[] _rawData;
        IncommingBase incommingBase;
        public IncommingBase Deserialize { get { return incommingBase; } }
        private int proto;
        public int Proto { get { return proto; } }

        internal ResEventArgs(Opcode opcode, byte[] rawData)
        {
           // DebugHelper.LogFormat("收到消息,字节长度:{0}", rawData.Length);

            _rawData = rawData;

            incommingBase = MsgPacketUtil.GetIncomming(_rawData);

            if (incommingBase != null)
            {
                proto = incommingBase.proto;

                if (proto != 10011)
                    Debug.Log(incommingBase.ToString());
            }
        }
    }
}