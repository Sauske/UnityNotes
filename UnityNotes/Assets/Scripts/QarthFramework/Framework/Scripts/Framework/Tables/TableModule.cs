using System;
using System.Collections;
using System.Collections.Generic;
namespace Qarth
{
    public class TableModule
    {
        // 表格读取进度
        private static float       m_TableReadProgress;
        private static bool        m_IsLoading = false;
    
        public static float tableReadProgress
        {
            get { return m_TableReadProgress; }
        }
    
        public static bool isLoading
        {
            get { return m_IsLoading; }
        }
    
        //预加载表格资源
        
        public static IEnumerator PreLoadTable(Action onLoadFinish)
        {
            TableReadThreadWork readWork = CreateTableReadJobs(TableConfig.preLoadTableArray);
    
            readWork.Start();
            while (readWork.IsDone == false)
            {
                yield return 0;
            }
    
            if (onLoadFinish != null)
            {
                onLoadFinish();
            }
            yield return 0;
        }
        
        public static IEnumerator DelayLoadTable(Action onLoadFinish)
        {
            TableReadThreadWork readWork = CreateTableReadJobs(TableConfig.delayLoadTableArray);
    
            readWork.Start();
            while (readWork.IsDone == false)
            {
                yield return 0;
            }
    
            if (onLoadFinish != null)
            {
                onLoadFinish();
            }
            yield return 0;
        }
        
        public static IEnumerator LoadTable(TDTableMetaData[] dataArray, Action onLoadFinish)
        {
            m_IsLoading = true;
            TableReadThreadWork readWork = CreateTableReadJobs(dataArray);
            readWork.Start();
            while (readWork.IsDone == false)
            {
                m_TableReadProgress = readWork.finishedCount * 1.0f / readWork.readMaxCount * 1.0f;
                yield return 0;
            }
    
            m_IsLoading = false;
    
            if (onLoadFinish != null)
            {
                onLoadFinish();
            }
            yield return 0;
        }
        
        private static TableReadThreadWork CreateTableReadJobs(TDTableMetaData[] tableArrayA, TDTableMetaData[] tableArrayB = null)
        {
            TableReadThreadWork readWork = new TableReadThreadWork();
            if (tableArrayA != null)
            {
                for (int i = 0; i < tableArrayA.Length; ++i)
                {
                    readWork.AddJob(tableArrayA[i].tableName, tableArrayA[i].onParse);
                }
            }
    
            if (tableArrayB != null)
            {
                for (int i = 0; i < tableArrayB.Length; ++i)
                {
                    readWork.AddJob(tableArrayB[i].tableName, tableArrayA[i].onParse);
                }
            }
    
            return readWork;
        }
    }
}

