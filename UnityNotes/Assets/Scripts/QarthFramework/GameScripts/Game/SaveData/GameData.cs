using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;

namespace MainGame
{
    //游戏存档数据结构，需要的存档数据可以在下面列出
    //注意：不支持字典序列化 建议自定义数据结构
    public class GameData : IDataClass
    {
        public string userName;
        
        public override void InitWithDefaultData()
        {
            userName = "xixi";
        }

        public override void RefreshDataByDay()
        {
            
        }

        public override void OnDataLoadFinish()
        {
            //注意：如果是后面新加的数据，原始的存档数据没有，需要手动初始化
        }
        
        public void SetUserName(string name)
        {
           userName = name;
           SetDataDirty();
        }
    }
}

