using UnityEngine;

namespace ResUpdater
{
    public class ResUpdaterPath
	{
        public const string ExportBundlePath = "AssetBundles";

        public const string BundleInfoName = "bundleinfo.byte";

        public const string AdditionBundleInfoName = "AdditionBundleInfo.byte";

        public const string DllBundleInfoName = "dllbundleinfo.byte";

        public const string VersionInfoName = "version";

        public const string ResVersionInfoName = "resversion";

        public const string AssetBundlePathInStream = "/AssetBundles";

        public const string LastestName = ".latest";

        public const string AdditionBundlePath = "AdditoionBundle/";

        public static readonly string VersionPath = BundleBuildPath + "/{0}/" + VersionInfoName;

        public static string BundleBuildPath { get { return "../.." + AssetBundlePathInStream; } }

        public static string AdditionBundleBuildPath { get { return BundleBuildPath + "/{0}/" + AdditionBundlePath; } }


        //>---------------------------------------------------------------------------------------
        //Paths for runtine
        public readonly string StreamingAssetPathForWWW;

        public readonly string ProjectPlatform;

        public readonly string PersistentDataPath = Application.persistentDataPath + "/AssetBundles/";

        //assetBundle 根目录
        public readonly string AssetBundlePath;

        //persitentPath
        public readonly string PersitentPathForWWW;

        //>---------------------------------------------------------------------------------------
        //版本信息路径
        public readonly string[] VersionInfos;

        //资源更新需要检查的类型
        public readonly string[] ResCheckBundleInfos;

        //代码需要检查的类型
        public readonly string[] AppCheckBundleInfos;

        //>---------------------------------------------------------------------------------------
        public ResUpdaterPath(string projectPlatform)
        {
            ProjectPlatform = projectPlatform;

            if(Application.isEditor)
            {
                StreamingAssetPathForWWW = "file://" + Application.dataPath + "/../../.." + AssetBundlePathInStream + "/" + ProjectPlatform;
            }
            else
            {
                switch(ProjectPlatform)
                {
                    case "StandaloneWindows":
                        StreamingAssetPathForWWW = "file://" + Application.streamingAssetsPath + AssetBundlePathInStream + "/" + ProjectPlatform;
                        break;
                    case "Android":
                        StreamingAssetPathForWWW = "jar:file://" + Application.dataPath + "!/assets" + AssetBundlePathInStream + "/" + ProjectPlatform;
                        break;
                    case "iOS":
                        StreamingAssetPathForWWW = "file://" + Application.streamingAssetsPath + AssetBundlePathInStream + "/" + ProjectPlatform;
                        break;
                    default:
                        ProjectPlatform = "Unknow";
                        break;
                }
            }

            AssetBundlePath = AssetBundlePathInStream + "/" + ProjectPlatform + "/";
            PersitentPathForWWW = "file:///" + Application.persistentDataPath + AssetBundlePath;

            VersionInfos = new[]
            {
                AssetBundlePath + VersionInfoName,
                AssetBundlePath + ResVersionInfoName,
            };

            ResCheckBundleInfos = new[]
            {
                AssetBundlePath + BundleInfoName,
            };

            AppCheckBundleInfos = new[]
            {
                AssetBundlePath + BundleInfoName,
                AssetBundlePath + AdditionBundlePath + AdditionBundleInfoName,
                AssetBundlePath + DllBundleInfoName,
            };
        }
    }
}
