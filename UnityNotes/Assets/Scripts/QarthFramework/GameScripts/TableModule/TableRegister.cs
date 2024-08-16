using System.Collections;
using System.Collections.Generic;
using MainGame;
using Qarth;
using UnityEngine;

namespace MainGame
{
    public class TableRegister
    {
        public static void RegisterTable()
        {
            //预加载表格
            TableConfig.preLoadTableArray = new TDTableMetaData[]
            {
                TDLanguageTable.GetLanguageMetaData()
            };

            TableConfig.delayLoadTableArray = new TDTableMetaData[]
            {
                TDArenaConfigTable.metaData
            };
        }
    }
}