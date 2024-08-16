using System;
using System.Collections;
using System.Collections.Generic;
using Qarth;
using UnityEditor;
using UnityEngine;


namespace MainGame
{
	public class AssetTableUtils
	{
		//编辑器内运行时生成assettable
		public static AssetDataTable BuildEditorDataTable()
		{
			Debug.Log("Start BuildAssetDataTable!");
			var resDatas = new AssetDataTable();
			AddABInfo2ResDatas(resDatas);
			return resDatas;
		}

		public static void AddABInfo2ResDatas(AssetDataTable assetBundleConfigFile)
		{
#if UNITY_EDITOR
			AssetDatabase.RemoveUnusedAssetBundleNames();

			var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
			foreach (var abName in assetBundleNames)
			{
				var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);
				AssetDataPackage group;
				string md5 = abName.GetHashCode().ToString();
				long buildTime = DateTime.Now.Ticks;
				var abIndex = assetBundleConfigFile.AddAssetBundleName(abName, depends, md5,abName.Length,buildTime,out @group);
				if (abIndex < 0)
				{
					continue;
				}
                
				string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
				foreach (var cell in assets)
				{
					if (cell.EndsWith(".unity"))
					{
						group.AddAssetData(new AssetData(AssetPath2Name(cell), eResType.kABScene, abIndex));
					}
					else
					{
						group.AddAssetData(new AssetData(AssetPath2Name(cell), eResType.kABAsset, abIndex));
					}
				}
			}
			//assetBundleConfigFile.Dump();
#endif   
		}
		
		private static string AssetPath2Name(string assetPath)
		{
			int startIndex = assetPath.LastIndexOf("/") + 1;
			int endIndex = assetPath.LastIndexOf(".");

			if (endIndex > 0)
			{
				int length = endIndex - startIndex;
				return assetPath.Substring(startIndex, length).ToLower();
			}

			return assetPath.Substring(startIndex).ToLower();
		}
		
		public static string[] GetAssetPathsFromAssetBundleAndAssetName(string abRAssetName, string assetName)
		{
#if UNITY_EDITOR
			return AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abRAssetName, assetName);
#else
            return null;
#endif
		}
	}
	
}