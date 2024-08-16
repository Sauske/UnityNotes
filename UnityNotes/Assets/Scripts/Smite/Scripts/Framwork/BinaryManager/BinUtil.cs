using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Framework
{
    public class BinUtil
    {

        public static void WriteBoolean(ref Stream input, bool val)
        {
            byte b = (byte)(val ? 1 : 0);
            input.WriteByte(b);
        }

        public static bool ReadBoolean(Stream input)
        {
            return input.ReadByte() != 0;
        }

        public static void WriteUInteger(ref Stream input, uint val)
        {
            byte[] b = BitConverter.GetBytes(val);
            Array.Reverse(b);
            input.Write(b, 0, b.Length);
        }

        public static void WriteInteger(ref Stream input, int val)
        {
            byte[] b = BitConverter.GetBytes(val);
            Array.Reverse(b);
            input.Write(b, 0, b.Length);
        }

        public static uint ReadUInteger(Stream input)
        {
            return (uint)((input.ReadByte() << 24) + (input.ReadByte() << 16) + (input.ReadByte() << 8) + input.ReadByte());
        }

        public static int ReadInteger(Stream input)
        {
            return (input.ReadByte() << 24) + (input.ReadByte() << 16) + (input.ReadByte() << 8) + input.ReadByte();
        }

        public static void WriteFloat(ref Stream input, float val)
        {
            byte[] b = BitConverter.GetBytes(val);
            Array.Reverse(b);
            input.Write(b, 0, b.Length);
        }

        public static float ReadFloat(Stream input)
        {
            byte[] buffer = new byte[4];
            buffer[3] = (byte)input.ReadByte();
            buffer[2] = (byte)input.ReadByte();
            buffer[1] = (byte)input.ReadByte();
            buffer[0] = (byte)input.ReadByte();
            return BitConverter.ToSingle(buffer, 0);
        }

        public static void WriteString(ref Stream input, String str)
        {
            if (str == null)
            {
                WriteVariant(ref input, 0);
                return;
            }

            byte[] b = Encoding.UTF8.GetBytes(str);
            int count = b.Length;
            WriteVariant(ref input, count + 1);
            input.Write(b, 0, b.Length);
        }

        public static string ReadString(Stream input)
        {
            int byteCount = ReadVariant(input);
            switch (byteCount)
            {
                case 0:
                    return null;
                case 1:
                    return "";
            }
            byteCount--;
            byte[] buffer = new byte[byteCount];
            ReadFully(input, buffer, 0, byteCount);
            return Encoding.UTF8.GetString(buffer, 0, byteCount);
        }

        public static void WriteFloatArray(ref Stream stream, float[] values)
        {
            WriteVariant(ref stream, values.Length);
            for (int i = 0; i < values.Length; i++)
                WriteFloat(ref stream,values[i]);
        }


        public static float[] ReadFloatArray(Stream input)
        {
            int cnt = ReadVariant(input);
            float[] data = new float[cnt];
            for (int i = 0; i < cnt; i++)
                data[i] = ReadFloat(input);
            return data;
        }

        public static void WriteIntegerArray(ref Stream stream, int[] values)
        {
            WriteVariant(ref stream, values.Length);
            for (int i = 0; i < values.Length; i++)
                WriteInteger(ref stream,values[i]);
        }

        public static int[] ReadIntegerArray(Stream input)
        {
            int cnt = ReadVariant(input);
            int[] data = new int[cnt];
            for (int i = 0; i < cnt; i++)
                data[i] = ReadInteger(input);
            return data;
        }

        public static void WriteIntegerArrayArray(Stream stream, int[][] values)
        {
            WriteVariant(ref stream, values.Length);
            for (int i = 0; i < values.Length; i++)
                WriteIntegerArray(ref stream,values[i]);
        }

        public static int[][] ReadIntegerArrayArray(Stream input)
        {
            int cnt = ReadVariant(input);
            int[][] data = new int[cnt][];
            for (int i = 0; i < cnt; i++)
                data[i] = ReadIntegerArray(input);
            return data;
        }

        /// <summary>
        /// 读取变长数据【读取的字节是和WriteVariant相对应的，一个数值7位表示】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int ReadVariant(Stream input)
        {
            int b = input.ReadByte();
            int result = b & 0x7F;
            if ((b & 0x80) != 0)
            {
                b = input.ReadByte();
                result |= (b & 0x7F) << 7;
                if ((b & 0x80) != 0)
                {
                    b = input.ReadByte();
                    result |= (b & 0x7F) << 14;
                    if ((b & 0x80) != 0)
                    {
                        b = input.ReadByte();
                        result |= (b & 0x7F) << 21;
                        if ((b & 0x80) != 0) result |= (input.ReadByte() & 0x7F) << 28;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 写变长类型
        /// </summary>
        /// <param name="val"></param>
        public static void WriteVariant(ref Stream stream, int val)
        {
            if (stream == null)
                return;

            if (val <= (Math.Pow(2, 7) - 1))
            {
                stream.WriteByte((byte)val);
            }
            else if (val <= (Math.Pow(2, 14) - 1))
            {
                stream.WriteByte((byte)((val & 0x7F) | 0x80));
                stream.WriteByte((byte)(val >> 7));
            }
            else if (val <= (Math.Pow(2, 21) - 1))
            {
                stream.WriteByte((byte)((val & 0x7F) | 0x80));
                stream.WriteByte((byte)(((val >> 7) & 0x7F) | 0x80));
                stream.WriteByte((byte)((val >> 14)));
            }
            else if (val <= (Math.Pow(2, 28) - 1))
            {
                stream.WriteByte((byte)((val & 0x7F) | 0x80));
                stream.WriteByte((byte)(((val >> 7) & 0x7F) | 0x80));
                stream.WriteByte((byte)(((val >> 14) & 0x7F) | 0x80));
                stream.WriteByte((byte)((val >> 21)));
            }
            else
            {
                stream.WriteByte((byte)((val & 0x7F) | 0x80));
                stream.WriteByte((byte)(((val >> 7) & 0x7F) | 0x80));
                stream.WriteByte((byte)(((val >> 14) & 0x7F) | 0x80));
                stream.WriteByte((byte)(((val >> 21) & 0x7F) | 0x80));
                stream.WriteByte((byte)((val >> 28)));
            }
        }


        public static void WriteVector2(ref Stream input, Vector2 v)
        {
            WriteFloat(ref input, v.x);
            WriteFloat(ref input, v.y);
        }

        public static void WriteVector3(ref Stream input, Vector3 v)
        {
            WriteFloat(ref input, v.x);
            WriteFloat(ref input, v.y);
            WriteFloat(ref input, v.z);
        }


        static void ReadFully(Stream input, byte[] buffer, int offset, int length)
        {
            while (length > 0)
            {
                int count = input.Read(buffer, offset, length);
                if (count <= 0) throw new EndOfStreamException();
                offset += count;
                length -= count;
            }
        }
    }
}
