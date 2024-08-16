using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEngine;


namespace MainGame
{
	public class OtherData : IDataClass
	{
		public int age;
		
		public override void InitWithDefaultData()
		{
			age = 30;
		}

		public override void RefreshDataByDay()
		{
			
		}

		public override void OnDataLoadFinish()
		{
			
		}

		public void SetAge(int age)
		{
			this.age = age;
			SetDataDirty();
		}
	}
	
}