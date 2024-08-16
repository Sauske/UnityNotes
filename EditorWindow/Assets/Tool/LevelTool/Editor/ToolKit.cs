
using System.IO;

using UnityEngine;

namespace UMI
{

    public class ToolKit
    {
        public static bool SaveFile(byte[] bytes, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            try
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                fs.Close();
                fs.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        public static bool SaveFile(string value, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            try
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(value);
                fs.Close();
                fs.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        public static byte[] LoadByFile(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                int numBytesToRead = (int)fs.Length;
                byte[] bytes = reader.ReadBytes(numBytesToRead);

                reader.Close();
                fs.Close();
                return bytes;
            }
            catch (FileNotFoundException ioEx)
            {
                Debug.LogError(ioEx.Message);
            }
            return null;
        }
    }
}