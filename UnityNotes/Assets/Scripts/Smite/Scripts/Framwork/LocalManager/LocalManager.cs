//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace Framework
{

    public class LocalManager : Singleton<LocalManager>
    {
        public const string CACHE_MAIL_FILE_PATH = "mail";

        public override void Init()
        {
            base.Init();

        }


        public override void UnInit()
        {
            base.UnInit();

        }

        public class CGlobalData
        {
            public string latestServerIP;
            public string latestServerName;

            public static CGlobalData LoadFromFile()
            {
                string strPath = string.Format("{0}/global_data.xml", Application.persistentDataPath);
                CGlobalData data = (CGlobalData)DeSerializerObject(strPath, typeof(CGlobalData));
                return data;
            }

            public static void Save(CGlobalData data)
            {
                string strPath = string.Format("{0}/global_data.xml", Application.persistentDataPath);
                SerializerObject(strPath, data);
            }
        }

        public static void SerializerObject(string path, object obj)
        {
            if (File.Exists(path))
            { // remove exist file to fix unexcept text
                File.Delete(path);
            }

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (Stream streamFile = new FileStream(path, FileMode.OpenOrCreate))
            {
                if (streamFile == null)
                {
                    Debug.LogError("OpenFile Erro");
                    return;
                }

                try
                {
                    string strDirectory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(strDirectory))
                    {
                        Directory.CreateDirectory(strDirectory);
                    }

                    XmlSerializer xs = new XmlSerializer(obj.GetType());
                    TextWriter writer = new StreamWriter(streamFile, System.Text.Encoding.UTF8);
                    xs.Serialize(writer, obj);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("DeSerializerObject Erro:" + ex.ToString());
                }
            }
        }

        public static object DeSerializerObject(string path, Type type)
        {
            object obj = null;
            if (!File.Exists(path))
            {
                return null;
            }

            using (Stream streamFile = new FileStream(path, FileMode.Open))
            {
                if (streamFile == null)
                {
                    Debug.LogError("OpenFile Erro");
                    return obj;
                }

                try
                {
                    if (streamFile != null)
                    {
                        XmlSerializer xs = new XmlSerializer(type);
                        obj = xs.Deserialize(streamFile);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("SerializerObject Erro:" + ex.ToString());
                }
            }
            return obj;
        }


        public static object DeSerializerObjectFromAssert(string path, Type type)
        {
            object objRet = null;
            TextAsset textFile = (TextAsset)Resources.Load(path);
            if (textFile == null)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream(textFile.bytes))
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    objRet = xs.Deserialize(stream);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Deserialize Error:" + ex.ToString());
                }
            }
            Resources.UnloadAsset(textFile);
            return objRet;
        }

        public static object DeSerializerObjectFromBuff(byte[] buff, Type type)
        {
            object objRet = null;
            using (MemoryStream stream = new MemoryStream(buff))
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    objRet = xs.Deserialize(stream);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Deserialize Error:" + ex.ToString());
                }
            }

            return objRet;
        }



        /// <summary>
        /// 从本地读取缓存的读取的邮件
        /// </summary>
        public void ReadReadMailsFromCache()
        {
            if (File.Exists(CACHE_MAIL_FILE_PATH))
            {
                byte[] data = CFileManager.ReadFile(CACHE_MAIL_FILE_PATH);

                Stream stream = new MemoryStream(data);
                int mailCount = BinUtil.ReadInteger(stream);
                for (int idx = 0; idx < mailCount; idx++)
                {
                    Mail mail = new Mail();
                    mail.Deserialize(stream);
                }
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }

        /// <summary>
        /// 保存读取的邮件到缓存中去
        /// </summary>
        public void SaveReadMails2Cache()
        {
            if (File.Exists(CACHE_MAIL_FILE_PATH))
                File.Delete(CACHE_MAIL_FILE_PATH);

            string dir = CFileManager.GetFullDirectory(CACHE_MAIL_FILE_PATH);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Stream stream = new FileStream(CACHE_MAIL_FILE_PATH, FileMode.Create);
            List<Mail> temp = new List<Mail>();

            int count = temp.Count;
            BinUtil.WriteInteger(ref stream, count);
            for (int idx = 0; idx < count; idx++)
            {
                Mail ml = temp[idx];
                ml.Serialize(ref stream);
            }

            stream.Flush();
            stream.Close();
            stream.Dispose();
            stream = null;
        }
    }

    public class Mail
    {
        private uint mId;
        public uint Id { get { return mId; } }

        private bool mIsAutoDelete;
        public bool IsAutoDelete { get { return mIsAutoDelete; } }

        private string mFrom; //邮件发送方名字
        public string From { get { return mFrom; } }

        private string mSubject; //邮件标题
        public string Subject { get { return mSubject; } }


        private ulong mSendTime;        //邮件发送时间
        public ulong SendTime { get { return mSendTime; } }


        private string mContent;
        public string Content { get { return mContent; } }

        private ListView<int> mAccessList = null;
        public ListView<int> AccessList { get { return mAccessList; } }

        private bool mIsAccess = false;
        public bool IsAccess { get { return mIsAccess; } }

        private string mMailHyperLink;
        public string MailHyperlink { get { return mMailHyperLink; } }

        public Mail() { }

        public void Serialize(ref Stream stream)
        {
            BinUtil.WriteBoolean(ref stream,mIsAutoDelete);
            BinUtil.WriteUInteger(ref stream, mId);
            BinUtil.WriteString(ref stream, mFrom);
            BinUtil.WriteString(ref stream, mSubject);
            BinUtil.WriteUInteger(ref stream, (uint)mSendTime);
            BinUtil.WriteString(ref stream, mContent);
            BinUtil.WriteString(ref stream, mMailHyperLink);
        }

        public void Deserialize(Stream stream)
        {
            mIsAutoDelete = BinUtil.ReadBoolean(stream);
            mId = BinUtil.ReadUInteger(stream);
            mFrom = BinUtil.ReadString(stream);
            mSubject = BinUtil.ReadString(stream);
            mSendTime = BinUtil.ReadUInteger(stream);
            mContent = BinUtil.ReadString(stream);
            mMailHyperLink = BinUtil.ReadString(stream);
        }
    }
}