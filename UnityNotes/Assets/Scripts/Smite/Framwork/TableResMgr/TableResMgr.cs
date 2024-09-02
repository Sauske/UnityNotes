using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ProtoBuf;
using Res_Table;

namespace Framework
{
    public class TableResMgr : Singleton<TableResMgr>
    {
        public const int IdxHint = 0;

        private Hashtable mResTable = new Hashtable();

        public Dictionary<int, CTableRecordBase> mTablePathDic = new Dictionary<int, CTableRecordBase>()
    {
        {
            IdxHint,
            new TableRecord<ResHintsInfo,ResHintsList>
            {
                mTableIndex = IdxHint,
                mTablePath = "TableRes/Hints",
            }
         },
    };

        public override void Init()
        {
            //  LoadBinary(IdxHint);
        }


        public void LoadBinary(int nKey)
        {
            if (!mResTable.Contains(nKey))
            {
                CTableRecordBase recordBase = mTablePathDic[nKey];
                if (recordBase != null)
                {
                    CRecordTable recordTable = recordBase.Load();
                    mResTable.Add(nKey, recordTable);
                }
            }
        }

        public T GetRecordKey<T>(int tableIndex, uint dataId) where T : IExtensible
        {
            return GetRecordKey<T>(tableIndex, (int)dataId);
        }

        public T GetRecordKey<T>(int tableIndex, int dataId) where T : IExtensible
        {
            CRecordTable recordTable = (CRecordTable)mResTable[tableIndex];
            if (recordTable == null)
            {
                recordTable = mTablePathDic[tableIndex].Load();
                mResTable.Add(tableIndex, recordTable);
            }
            if (recordTable != null)
            {
                return (T)recordTable.GetItemWithKey(dataId);
            }
            return default(T);
        }

        public T GetRecordIndex<T>(int tableIndex, uint dataIndex) where T : IExtensible
        {
            return GetRecordIndex<T>(tableIndex, (int)dataIndex);
        }

        public T GetRecordIndex<T>(int tableIndex, int dataIndex) where T : IExtensible
        {
            CRecordTable recordTable = (CRecordTable)mResTable[tableIndex];
            if (recordTable == null)
            {
                recordTable = mTablePathDic[tableIndex].Load();
                mResTable.Add(tableIndex, recordTable);
            }
            if (recordTable != null)
            {
                return (T)recordTable.GetItemWithIndex(dataIndex);
            }
            return default(T);
        }

        public CRecordTable GetTable(int tableIndex)
        {
            if (mResTable[tableIndex] == null)
            {
                CRecordTable recordTable = mTablePathDic[tableIndex].Load();
                if (recordTable == null)
                {
                    Debug.LogError("TableResMgr.GetTable is Error,index: " + tableIndex);
                    return null;
                }
                mResTable.Add(tableIndex, recordTable);

                return recordTable;
            }
            return (CRecordTable)mResTable[tableIndex];
        }

    }



    public class CTableRecordBase
    {
        public virtual CRecordTable Load() { return null; }
    }


    public class TableRecord<TItem, TList> : CTableRecordBase
    {
        public int mTableIndex;
        public string mTablePath;

        public CRecordTable mRecordTable;

        public override CRecordTable Load()
        {
            byte[] buff = (Resources.Load(mTablePath) as TextAsset).bytes;
            if (buff != null)
            {
                mRecordTable = new CRecordTable();
                mRecordTable.Load<TItem, TList>(buff);
            }
            return mRecordTable;
        }
    }


    public class CRecordTable
    {
        private Dictionary<int, IExtensible> m_ItemMap = new Dictionary<int, IExtensible>();
        private List<IExtensible> m_ItemList = new List<IExtensible>();

        public int Count { get { return m_ItemList.Count; } }

        public IExtensible GetItemWithKey(int nKey)
        {
            IExtensible t;
            m_ItemMap.TryGetValue(nKey, out t);
            return t;
        }

        public IExtensible GetItemWithIndex(int nIndex)
        {
            if (nIndex < m_ItemList.Count)
            {
                return m_ItemList[nIndex];
            }
            return default(IExtensible);
        }

        public void Load<TItem, TList>(byte[] buffer)
        {
            try
            {
                using (Stream stream = new MemoryStream(buffer))
                {
                    IList list = (IList)Serializer.Deserialize<TList>(stream);
                    if (list == null)
                    {
                        Debug.LogError("Load<TItem,TList> " + typeof(TList) + " is Error");
                        return;
                    }

                    PropertyInfo listProperty = list.GetType().GetProperty("list");
                    if (listProperty == null)
                    {
                        Debug.LogError("Load<TItem,TList>the PropertyInfo list isn`t list" + typeof(TList) + " Error.");
                        return;
                    }

                    List<TItem> itemList = (List<TItem>)listProperty.GetGetMethod().Invoke(list, null);
                    if (itemList != null)
                    {
                        for (int i = 0; i < itemList.Count; i++)
                        {
                            PropertyInfo itemProp = itemList[i].GetType().GetProperty("id");
                            int id = System.Convert.ToInt32(itemProp.GetGetMethod().Invoke(itemList[i], null));
                            if (m_ItemMap.ContainsKey(id))
                            {
                                Debug.LogError("Load<TItem,TList>the PropertyInfo id cann`t find" + typeof(TItem) + " Error.");
                                continue;
                            }
                            m_ItemList.Add(itemList[i] as IExtensible);
                            m_ItemMap.Add(id, itemList[i] as IExtensible);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadError type is: " + typeof(TItem).ToString() + ex.Message);
            }
        }
    }
}