using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;

namespace MainGame
{
	//游戏存档数据清单
	public class SaveDataTable
	{
		public DataClassHandler<GameData> gameDataHandler;
		public DataClassHandler<OtherData> otherDataHandler;

		public SaveDataTable()
		{
			gameDataHandler = new DataClassHandler<GameData>();
			//是否开启计时自动存档，保证数据不丢失
			gameDataHandler.EnableAutoSave();
			
			otherDataHandler = new DataClassHandler<OtherData>();
		}
		
	}
	
}