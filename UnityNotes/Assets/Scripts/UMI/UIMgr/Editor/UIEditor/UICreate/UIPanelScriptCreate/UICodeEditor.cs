using System.IO;
using UnityEditor;

using UnityEngine;

namespace UMI.Editor
{
    public class UICodeEditor
    {
        private const string UICreateItemNameBase = "GameObject/UMI 生成 UI 脚本/";
        private const string UIAssetItemNameBase = "Assets/UMI 生成 UI 脚本/";

        private const string UIRemoveMissingScritps = "移除丢失脚本";

        private const string UICreateScriptItemName = "生成 UI 脚本";
        private const string UIDeleteItemName = "删除 UI 资源 和 脚本";
        private const string UIDeleteWithOutResItemName = "删除 UI 对应的所有脚本";
        private const string UIRecreateScriptItemName = "重新生成 UI 脚本 (删除原有的 View 和 item)";
        private const string UIRecreateScriptItemAllName = "重新生成 UI 脚本 (删除原有的Ctrl View 和 item)";


        public static string AutoUIPrefabBasePath = "Assets/Prefabs/UI";

        /// <summary>
        /// 生成UI脚本
        /// </summary>
        [MenuItem(UICreateItemNameBase + UIRemoveMissingScritps)]
        [MenuItem(UIAssetItemNameBase + UIRemoveMissingScritps)]
        public static void RemoveMissingScripts()
        {
            RemoveMissingScripts(Selection.activeGameObject.transform);
        }

        private static void RemoveMissingScripts(Transform objTra)
        {
            if (objTra == null)
            {
                return;
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(objTra.gameObject);
            int childCount = objTra.transform.childCount;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    RemoveMissingScripts(objTra.GetChild(i));
                }
            }
        }


        /// <summary>
        /// 生成UI脚本
        /// </summary>
        [MenuItem(UICreateItemNameBase + UICreateScriptItemName)]
        [MenuItem(UIAssetItemNameBase + UICreateScriptItemName)]
        public static void CreateSelectUICode()
        {
            string selectAssetPath = GetSelectPath();

            if (!string.IsNullOrEmpty(selectAssetPath))
            {
                UICodeCreater.ClearDelDict();
                UICodeCreater.CreateUIPrefabCode(selectAssetPath);

                SetSelectAAName(selectAssetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("生成 UI 脚本 完成");
        }

        /// <summary>
        /// 删除 UI 对应的所有脚本
        /// </summary>
        [MenuItem(UICreateItemNameBase + UIDeleteWithOutResItemName)]
        [MenuItem(UIAssetItemNameBase + UIDeleteWithOutResItemName)]
        public static void DeleteUIScripts()
        {
            if (EditorUtility.DisplayDialog("提示", "确定删除脚本吗？", "确定", "取消"))
            {
                string selectAssetPath = GetSelectPath();

                if (!string.IsNullOrEmpty(selectAssetPath))
                {
                    UICodeCreater.ClearDelDict();
                    UICodeCreater.DeleteUIPrefabCode(selectAssetPath, false);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("删除 UI 脚本 完成");
            }
        }

        /// <summary>
        /// 删除 UI 资源 和 脚本
        /// </summary>
        [MenuItem(UICreateItemNameBase + UIDeleteItemName)]
        [MenuItem(UIAssetItemNameBase + UIDeleteItemName)]
        public static void DeleteUIResAndScripts()
        {
            if (EditorUtility.DisplayDialog("提示", "确定删除资源和脚本吗？", "确定", "取消"))
            {
                string selectAssetPath = GetSelectPath();

                if (!string.IsNullOrEmpty(selectAssetPath))
                {
                    UICodeCreater.ClearDelDict();
                    UICodeCreater.DeleteUIPrefabCode(selectAssetPath, true);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("[==={0}===]删除 UI 脚本 和 资源 完成");
            }

        }

        /// <summary>
        /// 重新生成 UI 脚本 (删除原有的 View 和 item)
        /// </summary>
        [MenuItem(UICreateItemNameBase + UIRecreateScriptItemName)]
        [MenuItem(UIAssetItemNameBase + UIRecreateScriptItemName)]
        public static void ReCreateSelectUICode()
        {
            string selectAssetPath = GetSelectPath();

            if (!string.IsNullOrEmpty(selectAssetPath))
            {
                UICodeCreater.ClearDelDict();
                UICodeCreater.DeleteUIViewCode(selectAssetPath);
                UICodeCreater.CreateUIPrefabCode(selectAssetPath);

                SetSelectAAName(selectAssetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("生成 UI 脚本 完成");
        }

        /// <summary>
        /// 重新生成 UI 脚本 (删除原有的 Ctrl View 和 item)
        /// </summary>
        [MenuItem(UICreateItemNameBase + UIRecreateScriptItemAllName)]
        [MenuItem(UIAssetItemNameBase + UIRecreateScriptItemAllName)]
        public static void ReCreateAllSelectUICode()
        {
            string selectAssetPath = GetSelectPath();

            if (!string.IsNullOrEmpty(selectAssetPath))
            {
                UICodeCreater.ClearDelDict();

                UICodeCreater.DeleteUIPrefabCode(selectAssetPath, false);
                // UICodeCreater.DeleteUIViewCode(selectAssetPath);
                UICodeCreater.CreateUIPrefabCode(selectAssetPath);

                SetSelectAAName(selectAssetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("生成 UI 脚本 完成");
        }


        public static void SetSelectAAName(string selectAssetPath)
        {
            if (string.IsNullOrEmpty(selectAssetPath))
            {
                return;
            }

            //AASettingInfo setting = AAEditor.FindAddressableInfo();
            //if (Directory.Exists(selectAssetPath))
            //{
            //    AAGroup.QuickSetFolderGroupAAName(setting, new DirectoryInfo(selectAssetPath));
            //}
            //else
            //{
            //    AAGroup.QuickSetFolderGroupAAName(setting, new FileInfo(selectAssetPath));
            //}
        }

        public static string GetSelectPath()
        {
            string selectAssetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

            if (string.IsNullOrEmpty(selectAssetPath))
            {
                if (Selection.activeGameObject != null)
                {
                    selectAssetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject));
                }
            }

            if (!selectAssetPath.StartsWith(AutoUIPrefabBasePath))
            {
                selectAssetPath = string.Empty;
            }

            return selectAssetPath;
        }
    }
}
