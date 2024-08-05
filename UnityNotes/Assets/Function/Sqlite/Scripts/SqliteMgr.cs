using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System;
using System.Text;

namespace UMI
{
    public class SqliteMgr : Singleton<SqliteMgr>
    {
        const string DB_NAME = "UMISqlite.db";

        private SQLiteConnection _connection;

        public override void OnInitialize()
        {
            base.OnInitialize();

            CreateSqlite(DB_NAME);
            _connection.CreateTable<IMMsgData>();
        }

        public void Dispose()
        {
            if(_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
            _connection = null;
        }


        public void Insert(IMMsgData data)
        {
            try
            {
                _connection.Insert(data);
            }
            catch(Exception e)
            {
                SqliteLog.Error("Insert Data Error: +" + e.Message);
            }
        }

        public void InsertList(List<IMMsgData> list)
        {
            _connection.InsertAll(list);
        }

        public IEnumerable<IMMsgData> GetMsgList(long userId)
        {
            return _connection.Table<IMMsgData>().Where(x => x.chatId == userId);
        }


        /// <summary>
        /// 创建本地数据库
        /// </summary>
        /// <param name="DatabaseName"></param>
        public void CreateSqlite(string DatabaseName)
        {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            SqliteLog.Debug("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            SqliteLog.Debug("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            SqliteLog.Debug("Final PATH: " + dbPath);
        }


        #region 后期根据UserId分表

        string[] fields = { "Id", "chatId", "senderId", "receiverId", "content", "createTime", "MsgDataType", "MsgTalkType" };
        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="urseId"></param>
        public void CreateTable(long urseId)
        {
            //"CREATE TABLE "{urseId}"("Id" bigint primary key not null ,"chatId" bigint ,"senderId" bigint ,
            //"receiverId" bigint ,"content" varchar ,"createTime" bigint ,"MsgDataType" integer ,"MsgTalkType" integer ); ";
            string[] values = { "bigint primary key not null", "bigint", "bigint", "bigint", "varchar", "bigint", "integer", "integer" };

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"create table if not exists \"R{urseId}\"(");
            for (int idx = 0; idx < fields.Length; idx++)
            {
                strBuilder.Append($"\"{fields[idx]}\"");
                strBuilder.Append(" ");
                strBuilder.Append(values[idx]);
                if(idx< 7)
                    strBuilder.Append(",");
            }
            strBuilder.Append(");");
            //Log.Error(strBuilder.ToString());
            try
            {
                _connection.Execute(strBuilder.ToString());
            }
            catch (Exception e)
            {
                SqliteLog.Error("CreateTable Error: +" + e.Message);
            }
        }

        public void InsertMsg(IMMsgData data)
        {
            if (data.Id == 0) return;

            CreateTable(data.chatId);

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"insert into \"R{data.chatId}\"(");
            for (int idx = 0; idx < fields.Length; idx++)
            {
                strBuilder.Append($"{fields[idx]}");
                if (idx < 7)
                    strBuilder.Append(",");
            }
            strBuilder.Append(")");
            strBuilder.Append(" values (");
            strBuilder.Append(data.Id + ",");
            strBuilder.Append(data.chatId + ",");
            strBuilder.Append(data.senderId + ",");
            strBuilder.Append(data.receiverId + ",");
            strBuilder.Append("\"" + data.content + "\",");
            strBuilder.Append(data.createTime + ",");
            strBuilder.Append(data.MsgDataType + ",");
            strBuilder.Append(data.MsgTalkType);
            strBuilder.Append(");");
           // Log.Error(strBuilder.ToString());
            try
            {
                _connection.Execute(strBuilder.ToString());
            }
            catch (Exception e)
            {
                SqliteLog.Error("InsertMsg Error: +" + e.Message);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<IMMsgData> SelectMsgByUserId(long userId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"select * from R{userId};");
            try
            {
                List<IMMsgData> list = _connection.Query<IMMsgData>(strBuilder.ToString());
                return list;
            }
            catch (Exception e)
            {
                SqliteLog.Error("InsertMsg Error: +" + e.Message);
            }
            return null;
        }
        #endregion

    }
}
