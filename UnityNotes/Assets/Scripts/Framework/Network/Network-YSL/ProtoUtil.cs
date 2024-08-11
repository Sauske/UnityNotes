using System.IO;
using System.Net;
using System;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityWebSocket
{
    public class OutgoingBase
    {
        public int len = 8;
        public ushort proto;

        public MsgPacket buffer = null;

        public byte[] GetBuffer()
        {
            fill();
            return buffer.GetBuffer();
        }

        public OutgoingBase(ushort proto)
        {
            this.proto = proto;
            buffer = new MsgPacket();

            buffer.WriteInt32(8);
            buffer.WriteUInit16(proto);
            buffer.WriteInit16(0);

            buffer.Position = 8;
        }

        public virtual void fill()
        {

        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            Type type = this.GetType();

            FieldInfo[] fields = type.GetFields();

            builder.Append("SendMsg:");
            builder.AppendFormat("Proto:{0} ", proto);

            for (int idx = 0; idx < fields.Length; idx++)
            {
                string fieldName = fields[idx].Name;
                if (fieldName == "len" || fieldName == "proto" || fieldName == "buffer") continue;
                var value = fields[idx].GetValue(this);
                if (value != null)
                {
                    if (fieldName == "cards")
                    {
                        ArrayList list = value as ArrayList;
                        if (list != null)
                        {
                            int count = list.Count;
                            builder.AppendFormat("{0} Count : {1} \n", fieldName, count);

                            for (int subIdx = 0; subIdx < list.Count; subIdx++)
                            {
                                builder.Append(list[subIdx].ToString());
                                builder.Append("\n");
                            }
                        }
                    }
                    else
                    {
                        builder.AppendFormat("{0} : {1} \n", fieldName, value);
                    }
                }
            }

            return builder.ToString();
        }
    }

    public class OutgoinSubData
    {

        public MsgPacket buffer;

        public OutgoinSubData()
        {
            buffer = new MsgPacket();
        }

        public virtual void fill()
        {

        }


        public override string ToString()
        {

            StringBuilder builder = new StringBuilder();
            Type type = this.GetType();

            FieldInfo[] fields = type.GetFields();

            for (int idx = 0; idx < fields.Length; idx++)
            {
                string fieldName = fields[idx].Name;
                if (fieldName == "buffer") continue;

                var value = fields[idx].GetValue(this);
                if (value != null)
                {
                    builder.AppendFormat("{0} : {1} ", fieldName, value);
                    builder.Append("\n");
                }
            }

            return builder.ToString();
        }
    }


    public class IncommingBase
    {
        public int proto = 0;
        public IncommingBase(int proto)
        {
            this.proto = proto;
        }

        public virtual void fill(MsgPacket mp)
        {

        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            Type type = this.GetType();

            FieldInfo[] fields = type.GetFields();

            builder.Append("ReceiveMsg:");
            builder.AppendFormat("Proto : {0} \n", proto);

            for (int idx = 0; idx < fields.Length; idx++)
            {
                string fieldName = fields[idx].Name;
                Type subType = fields[idx].FieldType;
                if (fieldName == "proto") continue;
                var value = fields[idx].GetValue(this);
                if (value != null)
                {
                    if (subType.Name == "ArrayList")
                    {
                        ArrayList list = value as ArrayList;
                        if (list != null)
                        {
                            int count = list.Count;
                            builder.AppendFormat("{0} Count : {1} \n", fieldName, count);

                            for (int subIdx = 0; subIdx < list.Count; subIdx++)
                            {
                                builder.Append(list[subIdx].ToString());
                                builder.Append("\n");
                            }
                        }
                    }
                    else
                    {
                        builder.AppendFormat("{0} : {1} \n", fieldName, value);
                    }
                }
            }

            return builder.ToString();
        }
    }

    public class IncommingSubData
    {
        public IncommingSubData()
        {
        }

        public virtual void fill(MsgPacket mp)
        {

        }


        public override string ToString()
        {

            StringBuilder builder = new StringBuilder();
            Type type = this.GetType();

            FieldInfo[] fields = type.GetFields();

            //for (int idx = 0; idx < fields.Length; idx++)
            //{
            //    string fieldName = fields[idx].Name;
            //    var value = fields[idx].GetValue(this);
            //    if (value != null)
            //    {
            //        builder.AppendFormat("{0} : {1} ", fieldName, value);
            //        builder.Append("\n");
            //    }
            //}
            for (int idx = 0; idx < fields.Length; idx++)
            {
                string fieldName = fields[idx].Name;
                Type subType = fields[idx].FieldType;
                if (fieldName == "proto") continue;
                var value = fields[idx].GetValue(this);
                if (value != null)
                {
                    if (subType.Name == "ArrayList")
                    {
                        ArrayList list = value as ArrayList;
                        if (list != null)
                        {
                            int count = list.Count;
                            builder.AppendFormat("{0} Count : {1} \n", fieldName, count);

                            for (int subIdx = 0; subIdx < list.Count; subIdx++)
                            {
                                builder.Append(list[subIdx].ToString());
                                builder.Append("\n");
                            }
                        }
                    }
                    else
                    {
                        builder.AppendFormat("{0} : {1} \n", fieldName, value);
                    }
                }
            }

            return builder.ToString();
        }
    }

    public class MsgPacketUtil
    {
        public static void write(MsgPacket packet, byte value)
        {
            packet.WriteByte(value);
        }

        public static void write(MsgPacket packet, short value)
        {
            packet.WriteInit16(value);
        }

        public static void write(MsgPacket packet, int value)
        {
            packet.WriteInt32(value);
        }

        public static void write(MsgPacket packet, string value)
        {
            packet.WriteString(value);
        }

        public static void write(MsgPacket packet, long value)
        {
            packet.WriteLong(value);
        }

        public static IncommingBase GetIncomming(byte[] buffs)
        {
            MsgPacket packet = new MsgPacket(buffs);

            uint len = packet.readUInt32();
            ushort proto = packet.readUShort();
            ushort zip = packet.readUShort();

            // DebugHelper.LogFormat("len:{0},proto:{1},zip:{2}", len, proto, zip);

            try
            {
                string protoName = string.Empty;
                Dictionary<int, string> dir = Commands.getInstance().commandMap;
                if (dir.TryGetValue(proto, out protoName))
                {
                    Type type = Type.GetType(protoName);

                    IncommingBase inBase = (IncommingBase)Activator.CreateInstance(type);
                    inBase.proto = proto;
                    inBase.fill(packet);
                    return inBase;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogFormat("初始化出错：{0},Proto:{1}", ex.Message, proto);
            }
            return null;
        }
    }


    public class MsgPacket
    {
        private MemoryStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;

        public long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        public MsgPacket(byte[] buffer = null)
        {
            if (buffer == null) stream = new MemoryStream();
            else stream = new MemoryStream(buffer);

            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);
        }

        public void Close()
        {
            stream.Close();
            reader.Close();
            writer.Close();
        }

        public long readInt64()
        {
            return IPAddress.HostToNetworkOrder(this.reader.ReadInt64());
        }

        public long readLong()
        {
            return IPAddress.HostToNetworkOrder(this.reader.ReadInt64());
        }

        public int readInt()
        {
            return IPAddress.HostToNetworkOrder(this.reader.ReadInt32());
        }

        public uint readUInt32()
        {
            return (UInt32)IPAddress.HostToNetworkOrder(reader.ReadInt32());
        }

        public short readShort()
        {
            return IPAddress.HostToNetworkOrder(this.reader.ReadInt16());
        }

        public ushort readUShort()
        {
            ushort value = this.reader.ReadUInt16();
            return (ushort)IPAddress.HostToNetworkOrder((short)value);
        }


        public byte readByte()
        {
            return reader.ReadByte();
        }

        public sbyte readSByte()
        {
            return reader.ReadSByte();
        }

        public float readFloat()
        {
            return reader.ReadSingle();
        }

        public double readDouble()
        {
            return reader.ReadDouble();
        }

        public string readString()
        {
            //  return reader.ReadString();

            ushort len = readUShort();
            if (len > 0)
            {
                byte[] buffs = new byte[len];
                buffs = reader.ReadBytes(len);
                return System.Text.Encoding.UTF8.GetString(buffs);
            }
            return string.Empty;
        }





        public long Seek(long offset)
        {
            return this.stream.Seek(offset, SeekOrigin.Begin);
        }



        public void WriteSByte(SByte value)
        {
            writer.Write(value);
        }

        public void WriteByte(Byte value)
        {
            writer.Write(value);
        }

        public void WriteInit16(short value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            writer.Write(bytes);
        }

        public int WriteUInit16(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
            writer.Write(bytes);
            return 2;
        }

        public int WriteInt32(int value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            writer.Write(bytes);
            return 4;
        }

        public int WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            writer.Write(bytes);
            return 4;
        }

        public int WriteDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            writer.Write(bytes);
            return 8;
        }

        public int WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            writer.Write(bytes);
            return 8;
        }

        //public int WriteUInt32(UInt32 value)
        //{
        //    byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        //    writer.Write(bytes);
        //    return 4;
        //}

        public int WriteString(string value)
        {
            if (value == null) return 0;

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            WriteInit16((short)bytes.Length);
            writer.Write(bytes);

            return bytes.Length + 2;
        }

        public void AddRange(MsgPacket buffer)
        {
            writer.Write(buffer.GetBuffer());
        }


        public byte[] GetBuffer()
        {
            return this.stream.ToArray();
        }

        public int Count
        {
            get
            {
                return (int)this.stream.Length;
            }
        }
    }
}

