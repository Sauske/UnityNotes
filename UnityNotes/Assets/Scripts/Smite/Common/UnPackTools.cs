using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;


public sealed class UnPackTools
{
    private const int cPackHeadLen = 25;


    [System.Flags]
    private enum EPackType
    {
        None = 0,
        Compress = 1,
        Encrypt = 1 << 1,
    }

    private static readonly string cKey = "vz!@#cvpSDOI1230Vvc0fVB03kj2nkj;";
    private static readonly string cIV = "aDXvlkJhvI*zHFJN";

    public static byte[] UnPackData(byte[] oriData, out bool isPackData, out bool crcSuccess)
    {
        isPackData = false;
        crcSuccess = true;
        bool compress;
        bool encrypt;
        uint fileCrc;
        int compressDataLen;
        if (IsPackData(oriData, out compress, out encrypt, out fileCrc, out compressDataLen))
        {
            isPackData = true;


            byte[] decryptData = null;
            if (encrypt)
            {
                decryptData = Decrypt(oriData, cPackHeadLen, compressDataLen);
            }
            else
            {
                decryptData = new byte[oriData.Length - cPackHeadLen];
                int len = decryptData.Length;
                for (int i = 0; i < len; ++i)
                {
                    decryptData[i] = oriData[cPackHeadLen + i];
                }
            }

            byte[] decomData = decryptData;
            if (compress)
            {
                decomData = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(decryptData);
            }

            return decomData;
        }
        return oriData;
    }

    public static bool IsPackData(byte[] data, out bool compress, out bool encrypt, out uint crc, out int compressDataLen)
    {
        compress = false;
        encrypt = false;
        crc = 0;
        compressDataLen = 0;
        if (data == null || data.Length < cPackHeadLen)
            return false;

        using (MemoryStream ms = new MemoryStream(data))
        using (BinaryReader br = new BinaryReader(ms))
        {
            byte type = br.ReadByte();
            compress = (type & (byte)EPackType.Compress) != 0;
            encrypt = (type & (byte)EPackType.Encrypt) != 0;
            crc = br.ReadUInt32();
            br.ReadBytes(16);//md5
            compressDataLen = br.ReadInt32();
            //Debug.LogError("datalen=" + compressDataLen.ToString());
        }
        return true;
    }

    public static byte[] Decrypt(byte[] encryptData, int startIdx, int oriDataLen)
    {
        System.Security.Cryptography.RijndaelManaged rijndael = new System.Security.Cryptography.RijndaelManaged();
        rijndael.Mode = System.Security.Cryptography.CipherMode.CFB;
        rijndael.Padding = System.Security.Cryptography.PaddingMode.Zeros;
        rijndael.Key = Encoding.ASCII.GetBytes(cKey);
        rijndael.IV = Encoding.ASCII.GetBytes(cIV);

        System.Security.Cryptography.ICryptoTransform transform = rijndael.CreateDecryptor();
        byte[] bytes = transform.TransformFinalBlock(encryptData, startIdx, encryptData.Length - startIdx);

        if (bytes.Length < oriDataLen)
            throw new System.Exception("impossable");
        if (bytes.Length > oriDataLen)
        {
            byte[] ret = new byte[oriDataLen];
            for (int i = 0; i < oriDataLen; ++i)
            {
                ret[i] = bytes[i];
            }
            return ret;
        }
        return bytes;
    }
}
