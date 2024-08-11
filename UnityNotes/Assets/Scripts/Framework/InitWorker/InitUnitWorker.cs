using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UMI
{
    /// <summary>
    /// 初始化单元的处理器
    /// </summary>
    public class InitUnitWorker
    {
        private List<InitUnit> listUnit;        // 需要初始化的单元列表

        private bool initFinished;             // 是否初始化完成

        public bool InitFinished
        {
            get { return initFinished; }
        }
        
        public InitUnitWorker()
        {
            listUnit = new List<InitUnit>();
        }
        
        /// <summary>
        /// 进行初始化
        /// </summary>
        /// <param name="onAllFinish">所有都初始化完成回调</param>
        public async void DoInit(UnityAction onAllFinish)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            initFinished = false;
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < listUnit.Count; i++)
            {
                InitUnit unit = listUnit[i];
                float time1 = Time.realtimeSinceStartup;
                Debug.Log($"初始化开始：{unit.GetType()} start({time1})");
                taskList.Add(unit.Init());
                //await unit.Init();
                Debug.Log($"初始化完成：{unit.GetType()} cost({Time.realtimeSinceStartup - time1})");
            }
            await Task.WhenAll(taskList);
            initFinished = true;

            stopWatch.Stop();
            Debug.LogFormat("初始化总消耗: {0}ms", stopWatch.ElapsedMilliseconds);
            if (onAllFinish != null)
            {
                onAllFinish();
            }
        }

        /// <summary>
        /// 进行释放
        /// </summary>
        public void DoDispose()
        {
            initFinished = false;
            
            for (int i = 0; i < listUnit.Count; i++)
            {
                InitUnit unit = listUnit[i];
                Debug.Log($"释放开始：{unit.GetType()}");
                unit.Dispose();
                Debug.Log($"释放完成：{unit.GetType()}");
            }
        }

        /// <summary>
        /// 添加单元
        /// </summary>
        /// <param name="unit">单元实例</param>
        public void AddUnit(InitUnit unit)
        {
            listUnit.Add(unit);
        }
    }
}