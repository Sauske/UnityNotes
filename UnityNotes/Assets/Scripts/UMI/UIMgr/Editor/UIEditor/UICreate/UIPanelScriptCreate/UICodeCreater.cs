using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UMI.Editor
{
    public class UICodeCreater
    {
        private static Dictionary<string, string> mDelFileDict = new Dictionary<string, string>();

        public static void ClearDelDict()
        {
            mDelFileDict.Clear();
        }

        /// <summary>
        /// 生成单个UI对应的脚本
        /// </summary>
        /// <param name="uiResPath"></param>
        public static void CreateUIPrefabCode(string uiResPath)
        {
            uiResPath = FileUtils.GetUnityAssetPathFromFullPath(uiResPath);
            if (!uiResPath.StartsWith(UICodeCreaterConfig.UIPrefabBasePath))
            {
                return;
            }

            if (Directory.Exists(uiResPath))
            {
                DirectoryInfo folderInfo = new DirectoryInfo(uiResPath);
                FileSystemInfo[] fileInfos = folderInfo.GetFileSystemInfos();
                foreach (var info in fileInfos)
                {
                    string fullName = FileUtils.GetUnityAssetPathFromFullPath(info.FullName);
                    if (info is DirectoryInfo || info.Extension == UICodeCreaterConfig.PrefabExtName)
                    {
                        CreateUIPrefabCode(fullName);
                    }
                }
            }
            else if (Path.GetExtension(uiResPath) == UICodeCreaterConfig.PrefabExtName)
            {
                CreateUICode(uiResPath);
            }

        }

        /// <summary>
        /// 删除 UI 和其 对应的脚本
        /// </summary>
        /// <param name="uiResPath"></param>
        public static void DeleteUIPrefabCode(string uiResPath, bool delUIRes = true)
        {
            uiResPath = FileUtils.GetUnityAssetPathFromFullPath(uiResPath);
            if (!uiResPath.StartsWith(UICodeCreaterConfig.UIPrefabBasePath))
            {
                return;
            }

            if (Directory.Exists(uiResPath))
            {
                DirectoryInfo folderInfo = new DirectoryInfo(uiResPath);
                FileSystemInfo[] fileInfos = folderInfo.GetFileSystemInfos();
                foreach (var info in fileInfos)
                {
                    string fullName = FileUtils.GetUnityAssetPathFromFullPath(info.FullName);
                    if (info is DirectoryInfo || info.Extension == UICodeCreaterConfig.PrefabExtName)
                    {
                        DeleteUIPrefabCode(fullName, delUIRes);
                    }
                }
            }
            else if (Path.GetExtension(uiResPath) == UICodeCreaterConfig.PrefabExtName)
            {
                DelUIResAll(uiResPath, delUIRes);
            }

        }

        /// <summary>
        /// 删除 UI 的 View代码 用于重新生成
        /// </summary>
        /// <param name="uiResPath"></param>
        public static void DeleteUIViewCode(string uiResPath)
        {
            uiResPath = FileUtils.GetUnityAssetPathFromFullPath(uiResPath);
            if (!uiResPath.StartsWith(UICodeCreaterConfig.UIPrefabBasePath))
            {
                return;
            }

            if (Directory.Exists(uiResPath))
            {
                DirectoryInfo folderInfo = new DirectoryInfo(uiResPath);
                FileSystemInfo[] fileInfos = folderInfo.GetFileSystemInfos();
                foreach (var info in fileInfos)
                {
                    string fullName = FileUtils.GetUnityAssetPathFromFullPath(info.FullName);
                    if (info is DirectoryInfo || info.Extension == UICodeCreaterConfig.PrefabExtName)
                    {
                        DeleteUIViewCode(fullName);
                    }
                }

            }
            else if (Path.GetExtension(uiResPath) == UICodeCreaterConfig.PrefabExtName)
            {
                DelUIView(uiResPath);
            }
        }

        /// <summary>
        /// 生成UIPrefab 的脚本
        /// </summary>
        /// <param name="uiPrefabPath"></param>
        private static void CreateUICode(string uiPrefabPath)
        {
            uiPrefabPath = FileUtils.GetUnityAssetPathFromFullPath(uiPrefabPath);
            if (!uiPrefabPath.StartsWith(UICodeCreaterConfig.UIPrefabBasePath)
                || Path.GetDirectoryName(uiPrefabPath) == UICodeCreaterConfig.UIPrefabBasePath)
            {
                return;
            }

            string prefabName = Path.GetFileNameWithoutExtension(uiPrefabPath);
            string folderName = GetFolderName(uiPrefabPath);
            string codePath = Path.Combine(UICodeCreaterConfig.UICodeBasePath, folderName);

            string uiViewName;
            string uiCtrlName;
            GetCtrlAndViewName(prefabName, out uiCtrlName, out uiViewName);

            string uiViewPath = string.Empty;
            string uiCtrlPath = string.Empty;
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiViewName, ref uiViewPath);
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiCtrlName, ref uiCtrlPath);
            if (string.IsNullOrEmpty(uiViewPath))
            {
                uiViewPath = Path.Combine(codePath, UICodeCreaterConfig.ViewFolderName, uiViewName);
            }

            if (string.IsNullOrEmpty(uiCtrlPath))
            {
                uiCtrlPath = Path.Combine(codePath, UICodeCreaterConfig.CtrlFolderName, uiCtrlName);
            }

            FileUtils.CreateFolder(codePath);
            FileUtils.CreateFolder(Path.Combine(codePath, UICodeCreaterConfig.ViewFolderName));
            FileUtils.CreateFolder(Path.Combine(codePath, UICodeCreaterConfig.CtrlFolderName));

            FileUtils.CreateFolder(Path.GetDirectoryName(uiViewPath));
            FileUtils.CreateFolder(Path.GetDirectoryName(uiCtrlPath));

            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(uiPrefabPath);
            //GameObject insObj = GameObject.Instantiate(obj);
            //insObj.name = obj.name;
            bool bChanged = false;
            bool succ = false;

            if (Path.GetFileNameWithoutExtension(uiPrefabPath).ToLower().Contains(UICodeCreaterConfig.SubItemName)
                || Path.GetDirectoryName(uiPrefabPath).ToLower().Contains("itemorpanel"))
            {
                CreateSubItemOrPanelCode(obj, codePath, ref bChanged);
            }
            else
            {
                CreateFrameUICode(obj, codePath, ref bChanged);
            }

            if (bChanged)
            {
                PrefabUtility.SavePrefabAsset(obj, out succ);
                //PrefabUtility.SaveAsPrefabAsset(obj, uiPrefabPath, out succ);
            }
            //GameObject.DestroyImmediate(insObj);
        }

        /// <summary>
        /// 删除UI对应的view脚本 
        /// </summary>
        /// <param name="uiPrefabPath"></param>
        private static void DelUIView(string uiPrefabPath)
        {
            uiPrefabPath = FileUtils.GetUnityAssetPathFromFullPath(uiPrefabPath);
            if (!uiPrefabPath.StartsWith(UICodeCreaterConfig.UIPrefabBasePath))
            {
                return;
            }

            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(uiPrefabPath);
            if (obj == null)
            {
                return;
            }

            BaseItemCtrl[] itemCtrls = obj.GetComponentsInChildren<BaseItemCtrl>();
            List<string> viewNameLsts = new List<string>();
            foreach (var item in itemCtrls)
            {
                viewNameLsts.Add(item.GetType().Name.Substring(0, item.GetType().Name.Length - 4)
                    + UICodeCreaterConfig.ViewFolderName);
            }

            string uiCtrlName;
            string uiViewName;
            GetCtrlAndViewName(obj.name, out uiCtrlName, out uiViewName);
            viewNameLsts.Add(uiViewName);

            List<string> delPathList = new List<string>();

            foreach (var name in viewNameLsts)
            {
                FindSubViewList(name, delPathList);
            }

            foreach (var name in delPathList)
            {
                //Debug.LogError(name);
                mDelFileDict[Path.GetFileNameWithoutExtension(name)] = name;
                FileUtils.DelFile(name);
                // FileUtils.DelFile(name + UICodeCreaterConfig.MetaExtName);
            }

        }

        /// <summary>
        /// 删除UI对应的所有脚本
        /// </summary>
        /// <param name="uiPrefabPath"></param>
        private static void DelUIResAll(string uiPrefabPath, bool delUIRes = true)
        {
            uiPrefabPath = FileUtils.GetUnityAssetPathFromFullPath(uiPrefabPath);
            if (!uiPrefabPath.StartsWith(UICodeCreaterConfig.UIPrefabBasePath))
            {
                return;
            }

            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(uiPrefabPath);
            if (obj == null)
            {
                return;
            }

            BaseItemCtrl[] itemCtrls = obj.GetComponentsInChildren<BaseItemCtrl>();
            List<string> viewNameLsts = new List<string>();

            List<string> delPathList = new List<string>();
            List<string> uiTypeLst = new List<string>();

            foreach (var item in itemCtrls)
            {
                viewNameLsts.Add(item.GetType().Name.Substring(0, item.GetType().Name.Length - 4)
                    + UICodeCreaterConfig.ViewFolderName);

                uiTypeLst.Add(item.GetType().Name.Substring(0, item.GetType().Name.Length - 4));
                string ctrlPath = string.Empty;
                FindUICodePath(UICodeCreaterConfig.UICodeBasePath, item.GetType().Name, ref ctrlPath);
                if (!string.IsNullOrEmpty(ctrlPath))
                {
                    delPathList.Add(ctrlPath);
                }
            }

            string uiCtrlName;
            string uiViewName;
            GetCtrlAndViewName(obj.name, out uiCtrlName, out uiViewName);
            viewNameLsts.Add(uiViewName);

            uiTypeLst.Add(uiViewName.Substring(0, uiViewName.Length - 4));

            foreach (var name in viewNameLsts)
            {
                FindSubViewAndCtrlList(name, delPathList);
            }

            int index = uiPrefabPath.Substring(UICodeCreaterConfig.UIPrefabBasePath.Length + 1).IndexOf("/");
            string uiTypeFolder = uiPrefabPath.Substring(UICodeCreaterConfig.UIPrefabBasePath.Length + 1).Substring(0, index);

            foreach (var name in delPathList)
            {
                if (name.Contains(uiTypeFolder))
                {
                    //Debug.LogError(name);
                    FileUtils.DelFile(name);
                    FileUtils.DelFile(name + UICodeCreaterConfig.MetaExtName);

                }
            }

            RemoveUINameClass(uiTypeLst);

            if (delUIRes)
            {
                FileUtils.DelFile(uiPrefabPath);
                FileUtils.DelFile(uiPrefabPath + UICodeCreaterConfig.MetaExtName);
            }
        }



        /// <summary>
        /// 查找UI View 里面的 使用的View Ctrl
        /// </summary>
        /// <param name="uiViewName"></param>
        /// <param name="viewPathList"></param>
        private static void FindSubViewList(string uiViewName, List<string> viewPathList)
        {
            string viewCodePath = string.Empty;
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiViewName, ref viewCodePath);
            if (!viewPathList.Contains(viewCodePath))
            {
                viewPathList.Add(viewCodePath);
            }

            if (!string.IsNullOrEmpty(viewCodePath))
            {
                List<string> strLst = FileUtils.readAllLine(viewCodePath);
                int i = 0;
                while (i < strLst.Count)
                {
                    string str = strLst[i].Trim().Replace("{", "").Replace("}", "").Replace(";", "").Replace("\n", "");
                    if (str.Contains("private") && str.Contains(UICodeCreaterConfig.CtrlFolderName))
                    {
                        List<string> keyLst = str.Split(" ").ToList();
                        if (keyLst.Count == 3)
                        {
                            string subViewName = keyLst[1].Substring(0, keyLst[1].Length - 4) + UICodeCreaterConfig.ViewFolderName;

                            FindSubViewList(subViewName, viewPathList);
                        }
                    }
                    i++;
                }
            }
        }


        /// <summary>
        /// 查找UI View 里面的 使用的View Ctrl
        /// </summary>
        /// <param name="uiViewName"></param>
        /// <param name="viewPathList"></param>
        private static void FindSubViewAndCtrlList(string uiViewName, List<string> viewPathList)
        {
            if (string.IsNullOrEmpty(uiViewName))
            {
                return;
            }

            string viewCodePath = string.Empty;
            string ctrlCodePath = string.Empty;
            string uiCtrlName = uiViewName.Substring(0, uiViewName.Length - 4) + UICodeCreaterConfig.CtrlFolderName;
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiViewName, ref viewCodePath);
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiCtrlName, ref ctrlCodePath);

            if (!viewPathList.Contains(viewCodePath))
            {
                viewPathList.Add(viewCodePath);
            }
            if (!viewPathList.Contains(ctrlCodePath))
            {
                viewPathList.Add(ctrlCodePath);
            }

            if (!string.IsNullOrEmpty(viewCodePath))
            {
                List<string> strLst = FileUtils.readAllLine(viewCodePath);
                int i = 0;
                while (i < strLst.Count)
                {
                    string str = strLst[i].Trim().Replace("{", "").Replace("}", "").Replace(";", "").Replace("\n", "");
                    if (str.Contains("private") && str.Contains(UICodeCreaterConfig.CtrlFolderName))
                    {
                        List<string> keyLst = str.Split(" ").ToList();
                        if (keyLst.Count == 3)
                        {
                            string subViewName = keyLst[1].Substring(0, keyLst[1].Length - 4) + UICodeCreaterConfig.ViewFolderName;

                            FindSubViewAndCtrlList(subViewName, viewPathList);
                        }
                    }
                    i++;
                }
            }
        }






        #region CreateCode

        /// <summary>
        /// 创建UI框架的界面代码
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uiCodeFolder"></param>
        /// <param name="bChanged"></param>
        private static void CreateFrameUICode(GameObject obj, string uiCodeFolder, ref bool bChanged)
        {
            List<string> uiObjPathLst = new List<string>();
            GetAllUseChildPath(obj, string.Empty, uiCodeFolder, uiObjPathLst, ref bChanged);
            uiCodeFolder = FileUtils.GetUnityAssetPathFromFullPath(uiCodeFolder);

            string uiCtrlName = string.Empty;
            string uiViewName = string.Empty;
            GetCtrlAndViewName(obj.name, out uiCtrlName, out uiViewName);

            string uiCtrlCodePath = string.Empty;
            string uiViewCodePath = string.Empty;

            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiCtrlName, ref uiCtrlCodePath);
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, uiViewName, ref uiViewCodePath);

            List<EditorPropInfo> basePropLst = new List<EditorPropInfo>();
            if (!string.IsNullOrEmpty(uiViewCodePath))
            {
                ViewCodeToPropInfo(uiViewCodePath, basePropLst);
            }
            else
            {
                if (mDelFileDict.ContainsKey(uiViewName))
                {
                    uiViewCodePath = mDelFileDict[uiViewName];
                }
                else
                {
                    uiViewCodePath = Path.Combine(uiCodeFolder, UICodeCreaterConfig.ViewFolderName,
                        uiViewName + UICodeCreaterConfig.CSExtName);
                }
            }

            if (string.IsNullOrEmpty(uiCtrlCodePath))
            {
                if (mDelFileDict.ContainsKey(uiCtrlName))
                {
                    uiCtrlCodePath = mDelFileDict[uiCtrlName];
                }
                else
                {
                    uiCtrlCodePath = Path.Combine(uiCodeFolder, UICodeCreaterConfig.CtrlFolderName,
                        uiCtrlName + UICodeCreaterConfig.CSExtName);
                }
            }
            else
            {
                string ctrlPath = FileUtils.GetUnityAssetPathFromFullPath(uiCtrlCodePath);
                if (!ctrlPath.Contains(uiCodeFolder))
                {
                    Debug.LogError(uiCtrlCodePath);
                    EditorUtility.DisplayDialog("警告", $"{obj.name}已经存在对应的UI,不能使用相同的命名", "确定");
                    return;
                }
            }

            List<EditorPropInfo> newPropLst = new List<EditorPropInfo>();
            GetObjListProps(obj, string.Empty, uiObjPathLst, newPropLst);

            for (int i = 0; i < newPropLst.Count; i++)
            {
                if (!basePropLst.Contains(newPropLst[i]))
                {
                    basePropLst.Add(newPropLst[i]);
                }
            }

            string addBtnClickFunStr = string.Empty;
            string btnClickFunStr = string.Empty;

            foreach (var prop in basePropLst)
            {
                if (prop.typeName == typeof(UnityEngine.UI.Button).Name)
                {
                    string funName = $"On{prop.propPublicGetName}Click";

                    //UGUITools.AddBtnClick(mUIView.{prop.propPublicGetName},{funName});
                    addBtnClickFunStr += $"\t\t\tUGUITools.AddBtnClick(mUIView.{prop.propPublicGetName}, {funName});\r\n";

                    string funStr = "\t\tprivate void " + funName + "()\r\n\t\t{\r\n\r\n\t\t}\r\n\r\n";
                    btnClickFunStr += funStr;
                }
            }

            string propStr = string.Empty;
            EditorPropInfoToStr(basePropLst, ref propStr);

            string uiTypeName = obj.name;
            if (!uiTypeName.ToUpper().StartsWith(ConstInfo.UICtrlStart))
            {
                uiTypeName = ConstInfo.UICtrlStart + uiTypeName;
            }

            string viewCodeData = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.ViewTemplateName));
            viewCodeData = viewCodeData.Replace(UICodeCreaterConfig.Temp_View_NAME, uiViewName)
                            .Replace(UICodeCreaterConfig.Temp_View_UI_Props, propStr);

            FileUtils.WriteFile(uiViewCodePath, viewCodeData);
            if (!FileUtils.FileExists(uiCtrlCodePath))
            {
                string ctrlCodeData = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.CtrlTemplateName));
                ctrlCodeData = ctrlCodeData.Replace(UICodeCreaterConfig.Temp_UINameType, uiTypeName)
                                .Replace(UICodeCreaterConfig.Temp_UINameCtrl, uiCtrlName)
                                .Replace(UICodeCreaterConfig.Temp_View_NAME, uiViewName)
                                .Replace(UICodeCreaterConfig.Temp_Add_Btn_Click_Motheds,addBtnClickFunStr)
                                .Replace(UICodeCreaterConfig.Temp_Btn_Click_Motheds,btnClickFunStr);

                FileUtils.WriteFile(uiCtrlCodePath, ctrlCodeData);
            }

            Debug.Log($"生成一级界面 {uiViewName} ");

            AddUINameClass(uiTypeName);
        }

        /// <summary>
        /// 根据资源对象创建对应的二级界面或item代码
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uiCodeFolder"></param>
        /// <param name="bChanged"></param>
        private static void CreateSubItemOrPanelCode(GameObject obj, string uiCodeFolder, ref bool bChanged)
        {
            List<string> uiObjPathLst = new List<string>();
            GetAllUseChildPath(obj, string.Empty, uiCodeFolder, uiObjPathLst, ref bChanged);

            string subCtrlName = string.Empty;
            string subViewName = string.Empty;
            GetCtrlAndViewName(obj.name, out subCtrlName, out subViewName, true);

            Behaviour[] uiBehaviours = obj.GetComponents<Behaviour>();
            foreach (var uiInfo in uiBehaviours)
            {
                if (uiInfo != null)
                {
                    string tempTypeName = uiInfo.GetType().Name;
                    if (uiInfo is BaseItemCtrl)//(tempTypeName.EndsWith(CodeConfig.CtrlEnd))
                    {
                        subCtrlName = tempTypeName;
                        break;
                    }
                }
            }

            subViewName = subCtrlName.Substring(0, subCtrlName.Length - 4) + UICodeCreaterConfig.ViewFolderName;

            string subCtrlCodePath = string.Empty;
            string subViewCodePath = string.Empty;

            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, subCtrlName, ref subCtrlCodePath);
            FindUICodePath(UICodeCreaterConfig.UICodeBasePath, subViewName, ref subViewCodePath);

            List<EditorPropInfo> basePropLst = new List<EditorPropInfo>();
            if (!string.IsNullOrEmpty(subViewCodePath))
            {
                ViewCodeToPropInfo(subViewCodePath, basePropLst);
            }
            else
            {
                if (mDelFileDict.ContainsKey(subViewName))
                {
                    subViewCodePath = mDelFileDict[subViewName];
                }
                else
                {
                    subViewCodePath = Path.Combine(uiCodeFolder, UICodeCreaterConfig.ViewFolderName,
                        subViewName + UICodeCreaterConfig.CSExtName);
                }
            }

            if (string.IsNullOrEmpty(subCtrlCodePath))
            {
                if (mDelFileDict.ContainsKey(subCtrlName))
                {
                    subCtrlCodePath = mDelFileDict[subCtrlName];
                }
                else
                {
                    subCtrlCodePath = Path.Combine(uiCodeFolder, UICodeCreaterConfig.CtrlFolderName,
                        subCtrlName + UICodeCreaterConfig.CSExtName);
                }
            }

            List<EditorPropInfo> newPropLst = new List<EditorPropInfo>();
            GetObjListProps(obj, string.Empty, uiObjPathLst, newPropLst);

            for (int i = 0; i < newPropLst.Count; i++)
            {
                if (!basePropLst.Contains(newPropLst[i]))
                {
                    basePropLst.Add(newPropLst[i]);
                }
            }

            string addBtnClickFunStr = string.Empty;
            string btnClickFunStr = string.Empty;

            foreach (var prop in basePropLst)
            {
                if (prop.typeName == typeof(UnityEngine.UI.Button).Name)
                {
                    string funName = $"On{prop.propPublicGetName}Click";

                    //UGUITools.AddBtnClick(mUIView.{prop.propPublicGetName},{funName});
                    addBtnClickFunStr += $"\t\t\tUGUITools.AddBtnClick(ItemView.{prop.propPublicGetName}, {funName});\r\n";

                    string funStr = "\t\tprivate void " + funName + "()\r\n\t\t{\r\n\r\n\t\t}\r\n\r\n";
                    btnClickFunStr += funStr;
                }
            }

            // Debug.LogError(addBtnClickFunStr);
            // Debug.LogError(btnClickFunStr);

            string propStr = string.Empty;
            EditorPropInfoToStr(basePropLst, ref propStr);

            string viewCodeData = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.ViewTemplateName));
            viewCodeData = viewCodeData.Replace(UICodeCreaterConfig.Temp_View_NAME, subViewName)
                            .Replace(UICodeCreaterConfig.Temp_View_UI_Props, propStr);

            FileUtils.WriteFile(subViewCodePath, viewCodeData);
            if (!FileUtils.FileExists(subCtrlCodePath))
            {
                string ctrlCodeData = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.CtrlItemTemplateName));
                ctrlCodeData = ctrlCodeData.Replace(UICodeCreaterConfig.Temp_View_NAME, subViewName)
                                .Replace(UICodeCreaterConfig.Temp_UINameCtrl, subCtrlName)
                                .Replace(UICodeCreaterConfig.Temp_Add_Btn_Click_Motheds,addBtnClickFunStr)
                                .Replace(UICodeCreaterConfig.Temp_Btn_Click_Motheds,btnClickFunStr);

                FileUtils.WriteFile(subCtrlCodePath, ctrlCodeData);
            }

            if (subViewCodePath.Contains(uiCodeFolder))
            {
                Debug.Log($"生成二级界面 {subViewName}");
            }
            else
            {
                Debug.Log($"复用二级界面 {subViewName}" + "  原始路径 " + FileUtils.GetUnityAssetPathFromFullPath(subViewCodePath));
            }
        }

        #endregion



        #region 遍历节点

        /// <summary>
        /// 获取所有可用的节点
        /// </summary>
        /// <param name="obj"> gameObject 对象 </param>
        /// <param name="goBasePath"> 基础路径 </param>
        /// <param name="codeFolder"></param>
        /// <param name="propPathList"></param>
        /// <param name="bChanged"></param>
        private static void GetAllUseChildPath(GameObject obj, string goBasePath, string codeFolder,
            List<string> propPathList, ref bool bChanged)
        {
            if (obj == null)
            {
                return;
            }

            int childCount = obj.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject childObj = obj.transform.GetChild(i).gameObject;
                if (childObj.GetComponent<TMP_SubMeshUI>() != null)
                {
                    continue;
                }
                string childName = childObj.name;
                if (childName.StartsWith(UICodeCreaterConfig.IgnoreObjStart))
                {
                    continue;
                }

                if(childObj.GetComponent<UnityEngine.UI.Button>() != null)
                {
                    if(childObj.GetComponent<ButtonAudioComponent>() == null)
                    {
                        childObj.AddComponent<ButtonAudioComponent>();
                    }
                }

                if (childName.Contains(" ") || childName.Contains("(") || childName.Contains(")"))
                {
                    childName = childName.Replace(" ", "").Replace("(", "_").Replace(")", "_");
                    childObj.name = childName;
                    bChanged = true;
                }

                string objPath = string.IsNullOrEmpty(goBasePath) ? childName : goBasePath + "/" + childName;

                if (!propPathList.Contains(objPath))
                {
                    propPathList.Add(objPath);
                }
                else
                {
                    childName += propPathList.Count;
                    objPath = string.IsNullOrEmpty(goBasePath) ? childName : goBasePath + "/" + childName;
                    childObj.name = childName;
                    propPathList.Add(objPath);
                    bChanged = true;
                }

                if (childObj.transform.childCount > 0)
                {
                    if (childName.ToLower().StartsWith(UICodeCreaterConfig.SubItemName))
                    {
                        GetAllUseChildPath(childObj, objPath, codeFolder, propPathList, ref bChanged);
                    }
                    else if (childName.ToLower().StartsWith(UICodeCreaterConfig.SubPanelName))
                    {
                        GetAllUseChildPath(childObj, objPath, codeFolder, propPathList, ref bChanged);
                    }
                    else if (!childName.ToLower().Contains(UICodeCreaterConfig.SubItemName)
                        && !childName.ToLower().Contains(UICodeCreaterConfig.SubPanelName))
                    {
                        GetAllUseChildPath(childObj, objPath, codeFolder, propPathList, ref bChanged);
                    }
                }

                if (childName.ToLower().StartsWith(UICodeCreaterConfig.SubItemName)
                    || childName.ToLower().StartsWith(UICodeCreaterConfig.SubPanelName))
                {
                    continue;
                }

                if ((childName.ToLower().Contains(UICodeCreaterConfig.SubItemName)
                    || childName.ToLower().Contains(UICodeCreaterConfig.SubPanelName)))
                {
                    CreateSubItemOrPanelCode(childObj, codeFolder, ref bChanged);
                }
            }
        }

        /// <summary>
        /// 获取对应节点的 EditorPropInfo
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="viewCodeFolder"></param>
        /// <param name="uiObjPathLst"></param>
        /// <param name="propLst"></param>
        private static void GetObjListProps(GameObject obj, string viewCodeFolder, List<string> uiObjPathLst,
            List<EditorPropInfo> propLst)
        {
            List<string> propNameLst = new List<string>();

            Behaviour[] goBehaviours = obj.GetComponents<Behaviour>();
            if (goBehaviours != null && goBehaviours.Length > 0)
            {
                foreach (var uiInfo in goBehaviours)
                {
                    if (uiInfo is BaseItemCtrl)
                    {
                        continue;
                    }

                    string typeName = uiInfo.GetType().Name;
                    if (uiInfo is TMP_Text)
                    {
                        typeName = typeof(TMP_Text).Name;
                    }

                    string objName = "Self";
                    string propName = string.Empty;
                    string propGetName = string.Empty;

                    GetObjPropName(objName, typeName, propNameLst, ref propName, ref propGetName);
                    AddProp(typeName, propName, propGetName, "", propLst);
                }
            }



            foreach (var uiPath in uiObjPathLst)
            {
                int goAddCount = 0;
                GameObject propObj = obj.FindObj(uiPath);
                string objName = propObj.name;
                string typeName = string.Empty;
                string propName = string.Empty;
                string propGetName = string.Empty;

                if (objName.ToLower().Contains(UICodeCreaterConfig.SubItemName)
                    && !objName.ToLower().StartsWith(UICodeCreaterConfig.SubItemName))
                {
                    int index = objName.ToLower().LastIndexOf(UICodeCreaterConfig.SubItemName) + UICodeCreaterConfig.SubItemName.Length;
                    typeName = objName.Substring(0, index);
                }
                else if (objName.ToLower().Contains(UICodeCreaterConfig.SubPanelName)
                    && !objName.ToLower().StartsWith(UICodeCreaterConfig.SubPanelName))
                {
                    int index = objName.ToLower().LastIndexOf(UICodeCreaterConfig.SubPanelName) + UICodeCreaterConfig.SubPanelName.Length;
                    typeName = objName.Substring(0, index);
                }

                if (!string.IsNullOrEmpty(typeName))
                {
                    if (typeName.ToLower().StartsWith(UICodeCreaterConfig.UICodeStart))
                    {
                        typeName = typeName.Substring(0, 3).ToUpper() + typeName.Substring(3);
                    }
                    else
                    {
                        typeName = UICodeCreaterConfig.UICodeStart.ToUpper() + typeName.Substring(0, 1).ToUpper()
                         + typeName.Substring(1);
                    }

                    typeName += UICodeCreaterConfig.CtrlFolderName;
                }

                BaseItemCtrl itemCtrl = propObj.GetComponent<BaseItemCtrl>();
                if (itemCtrl != null)
                {
                    typeName = itemCtrl.GetType().Name;
                }

                if (!string.IsNullOrEmpty(typeName))
                {
                    GetObjPropName(objName, typeName, propNameLst, ref propName, ref propGetName);
                    AddProp(typeName, propName, propGetName, uiPath, propLst);
                    continue;
                }

                if (objName.ToLower().StartsWith(UICodeCreaterConfig.RectStart.ToLower()))
                {
                    goAddCount++;
                    typeName = typeof(RectTransform).Name;
                    GetObjPropName(objName, typeName, propNameLst, ref propName, ref propGetName);
                    AddProp(typeName, propName, propGetName, uiPath, propLst);
                }

                Behaviour[] uiBehaviours = propObj.GetComponents<Behaviour>();
                foreach (var uiInfo in uiBehaviours)
                {
                    typeName = uiInfo.GetType().Name;
                    if (uiInfo is TMP_Text)
                    {
                        typeName = typeof(TMP_Text).Name;
                    }

                    GetObjPropName(objName, typeName, propNameLst, ref propName, ref propGetName);
                    goAddCount++;
                    AddProp(typeName, propName, propGetName, uiPath, propLst);
                }

                if (objName.ToLower().StartsWith(UICodeCreaterConfig.GoStart.ToLower()) || goAddCount == 0)
                {
                    goAddCount++;
                    typeName = typeof(GameObject).Name;
                    GetObjPropName(objName, typeName, propNameLst, ref propName, ref propGetName);
                    AddProp(typeName, propName, propGetName, uiPath, propLst);
                }
            }

        }

        private static void AddProp(string typeName, string propName, string propGetName,
            string uiPath, List<EditorPropInfo> propLst)
        {
            EditorPropInfo prop = new EditorPropInfo();

            prop.typeName = typeName;
            prop.propPrivateName = propName;
            prop.propPublicGetName = propGetName;
            prop.objPath = uiPath;
            if (!propLst.Contains(prop))
            {
                propLst.Add(prop);
            }
        }

        /// <summary>
        /// 获取对应的命名
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="objTypeName"></param>
        /// <param name="propNameLst"></param>
        /// <param name="privatePropName"></param>
        /// <param name="getPropName"></param>
        private static void GetObjPropName(string objName, string objTypeName, List<string> propNameLst,
            ref string privatePropName, ref string getPropName)
        {
            string firstUpName = objName[0].ToString().ToUpper() + objName.Substring(1);
            if (objTypeName == typeof(GameObject).Name)
            {
                if (objName.ToLower().StartsWith("go"))
                {
                    privatePropName = "m" + firstUpName;
                }
                else
                {
                    privatePropName = "mGo" + firstUpName;
                }
                privatePropName = GetFinalName(privatePropName, propNameLst);
            }
            else if (objTypeName == typeof(RectTransform).Name)
            {
                if (objName.ToLower().StartsWith("rect"))
                {
                    privatePropName = "m" + firstUpName;
                }
                else
                {
                    privatePropName = "mRect" + firstUpName;
                }
                privatePropName = GetFinalName(privatePropName, propNameLst);
            }
            else if (objTypeName.Contains(UICodeCreaterConfig.CtrlFolderName))
            {
                if (objTypeName.ToLower().Contains(UICodeCreaterConfig.SubPanelName.ToLower()))
                {
                    privatePropName = "mPanel_" + firstUpName;
                }
                else
                {
                    privatePropName = "mItem_" + firstUpName;
                }
                privatePropName = GetFinalName(privatePropName, propNameLst);
            }
            else
            {
                string firstUpTypeName = objTypeName[0].ToString().ToUpper() + objTypeName.Substring(1);
                if (objTypeName == typeof(TMP_Text).Name)
                {
                    firstUpTypeName = "Text";
                }
                else if (objTypeName == typeof(UnityEngine.UI.Button).Name)
                {
                    firstUpTypeName = "Btn";
                }
                else if (objTypeName == typeof(UnityEngine.UI.Image).Name)
                {
                    firstUpTypeName = "Img";
                }

                if (objName.ToLower().StartsWith(firstUpTypeName.ToLower()))
                {
                    privatePropName = "m" + firstUpName;
                }
                else
                {
                    privatePropName = "m" + firstUpTypeName + firstUpName;
                }
                privatePropName = GetFinalName(privatePropName, propNameLst);
            }
            getPropName = privatePropName.Substring(1);
        }

        /// <summary>
        /// 获取属性的最终名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameLst"></param>
        /// <returns></returns>
        private static string GetFinalName(string name, List<string> nameLst)
        {
            if (nameLst.Contains(name))
            {
                name += "_" + nameLst.Count;
            }
            nameLst.Add(name);
            return name;
        }


        #endregion


        #region 读取模板文件 PropInfo  转 String

        private static void EditorPropInfoToStr(List<EditorPropInfo> propLst, ref string propStr)
        {
            string getPropTemp = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.GetPropTemplateName));

            foreach (var prop in propLst)
            {
                propStr += GetPropStr(getPropTemp, prop.typeName, prop.propPublicGetName, prop.propPrivateName, prop.objPath);
            }
        }


        /// <summary>
        /// 设置属性的获取方式
        /// </summary>
        /// <param name="baseStr"></param>
        /// <param name="typeName"></param>
        /// <param name="getPropName"></param>
        /// <param name="mPropPrivateName"></param>
        /// <param name="objPath"></param>
        /// <returns></returns>
        private static string GetPropStr(string baseStr, string typeName, string getPropName,
            string mPropPrivateName, string objPath)
        {
            string result = baseStr.Replace("typeName", typeName).Replace("getPropName", getPropName)
                    .Replace("mPropPrivateName", mPropPrivateName).Replace("objPath", objPath);

            if (string.IsNullOrEmpty(objPath))
            {
                result = result.Replace("\"\"", "");
                result = result.Replace("FindObjAndGetOrAddComponent", "GetComponent");
            }
            else if (typeName == typeof(GameObject).Name)
            {
                result = result.Replace("<GameObject>", "");
                result = result.Replace("FindObjAndGetOrAddComponent", "FindObj");
            }

            return result;
        }

        #endregion


        #region 读取CS 文件

        /// <summary>
        /// 读取 View 脚本
        /// </summary>
        /// <param name="codePath"></param>
        /// <param name="propLst"></param>
        private static void ViewCodeToPropInfo(string codePath, List<EditorPropInfo> propLst)
        {
            List<string> strLst = FileUtils.readAllLine(codePath);
            List<EditorPropInfo> tempPropLst = new List<EditorPropInfo>();

            int lineNum = 0;
            while (lineNum < strLst.Count)
            {
                string str = strLst[lineNum].Trim().Replace("{", "").Replace("}", "").Replace(";", "").Replace("\n", "");
                List<string> keyLst = str.Split(" ").ToList();
                lineNum++;
                if (str.Contains("class") || str.Contains("base"))
                {
                    continue;
                }

                if (keyLst.Count == 3)
                {
                    if (keyLst[0] == "private" && keyLst[2] != "mViewRect" && keyLst[2] != "mRect")
                    {
                        EditorPropInfo prop = new EditorPropInfo();
                        prop.typeName = keyLst[1];
                        prop.propPrivateName = keyLst[2];
                        tempPropLst.Add(prop);
                    }
                    else if (keyLst[0] == "public" && keyLst[2] != "ViewRect" && keyLst[2] != "RectTra")
                    {
                        foreach (var prop in tempPropLst)
                        {
                            if (prop.typeName == keyLst[1])
                            {
                                if (prop.propPrivateName == "m" + keyLst[2])
                                {
                                    prop.propPublicGetName = keyLst[2];
                                    break;
                                }
                            }
                        }
                    }

                    if (str.Contains("\""))
                    {
                        str = str.Replace(" ", "");
                        string propName = str.Substring(0, str.IndexOf("="));
                        foreach (var tempProp in tempPropLst)
                        {
                            if (tempProp.propPrivateName == propName)
                            {
                                tempProp.objPath = str.Split("\"").ToList()[1];
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var prop in tempPropLst)
            {
                if (!propLst.Contains(prop))
                {
                    propLst.Add(prop);
                }
            }
        }

        #endregion


        #region 辅助工具


        /// <summary>
        /// 读取最后生成的文件所在的文件夹名称
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns></returns>
        private static string GetFolderName(string prefabPath)
        {
            // string folderPath = Path.GetDirectoryName(prefabPath);
            // string folderName = Path.GetFileName(folderPath);
            // while (folderName.ToLower().Contains("prefab"))
            // {
            //     folderPath = Path.GetDirectoryName(folderPath);
            //     folderName = Path.GetFileName(folderPath);
            // }

            string folderPath = FileUtils.GetUnityAssetPathFromFullPath(prefabPath);
            folderPath = folderPath.Replace("Assets/Prefabs/UI/", "");
            string folderName = folderPath;
            if (folderPath.IndexOf('/') == -1)
            {
                folderName = folderPath;
            }
            else
            {
                folderName = folderPath.Substring(0, folderPath.IndexOf('/'));
            }

            if (!folderName.ToUpper().StartsWith(ConstInfo.UICtrlStart) && !folderName.ToLower().EndsWith(ConstInfo.UICtrlStart))
            {
                folderName = ConstInfo.UICtrlStart + folderName;
            }

            return folderName;
        }

        /// <summary>
        /// 生成对应的脚本名称
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="ctrlName"></param>
        /// <param name="viewName"></param>
        private static void GetCtrlAndViewName(string objName, out string ctrlName, out string viewName, bool bSubObj = false)
        {
            ctrlName = string.Empty;
            viewName = string.Empty;

            if (string.IsNullOrEmpty(objName))
            {
                return;
            }

            objName = Path.GetFileNameWithoutExtension(objName);
            ctrlName = objName;

            if (bSubObj)
            {
                if (objName.ToLower().Contains(UICodeCreaterConfig.SubItemName))
                {
                    int index = objName.ToLower().LastIndexOf(UICodeCreaterConfig.SubItemName)
                        + UICodeCreaterConfig.SubItemName.Length;
                    objName = objName.Substring(0, index);
                }
                else if (objName.ToLower().Contains(UICodeCreaterConfig.SubPanelName))
                {
                    int index = objName.ToLower().LastIndexOf(UICodeCreaterConfig.SubPanelName)
                        + UICodeCreaterConfig.SubPanelName.Length;
                    objName = objName.Substring(0, index);
                }
            }

            if (!ctrlName.ToLower().StartsWith(UICodeCreaterConfig.UICodeStart))
            {
                ctrlName = UICodeCreaterConfig.UICodeStart.ToUpper() + objName.Substring(0, 1).ToUpper() + objName.Substring(1);
            }
            else
            {
                ctrlName = objName.Substring(0, 3).ToUpper() + objName.Substring(3);
            }
            if (!ctrlName.EndsWith(UICodeCreaterConfig.CtrlFolderName))
            {
                ctrlName += UICodeCreaterConfig.CtrlFolderName;
            }

            viewName = ctrlName.Substring(0, ctrlName.Length - 4) + UICodeCreaterConfig.ViewFolderName;
        }

        /// <summary>
        /// 查找UI脚本的位置
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="codeName"></param>
        /// <param name="resultPath"></param>
        private static void FindUICodePath(string basePath, string codeName, ref string resultPath)
        {
            DirectoryInfo folderInfo = new DirectoryInfo(basePath);
            FileSystemInfo[] fileInfos = folderInfo.GetFileSystemInfos();
            foreach (var info in fileInfos)
            {
                if (info is DirectoryInfo)
                {
                    FindUICodePath(info.FullName, codeName, ref resultPath);
                }
                else
                {
                    if (Path.GetFileNameWithoutExtension(info.FullName) == codeName)
                    {
                        resultPath = info.FullName;
                        return;
                    }
                }
            }
        }


        #endregion


        #region 修改UITypeName

        /// <summary>
        /// 添加UI静态类
        /// </summary>
        /// <param name="uiName"></param>
        private static void AddUINameClass(string uiName)
        {
            string codePath = Path.Combine(UICodeCreaterConfig.UICodeBasePath, "UIConst/UITypeName.cs");

            List<string> strLst = FileUtils.readAllLine(codePath);
            List<string> uiNameLst = new List<string>();
            int i = 0;
            while (i < strLst.Count)
            {
                if (strLst[i].Contains("public static string ")
                    || strLst[i].Contains("public const string "))
                {
                    string uiStaticName = strLst[i].Split("\"")[1];
                    if (!uiNameLst.Contains(uiStaticName))
                    {
                        uiNameLst.Add(uiStaticName);
                    }
                }
                i++;
            }

            if (!uiNameLst.Contains(uiName))
            {
                uiNameLst.Add(uiName);
            }

            string nameCode = string.Empty;
            foreach (var name in uiNameLst)
            {
                nameCode += $"\t\tpublic const string {name} = \"{name}\";\n\n";
            }

            string ctrlCodeData = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.TxtUINames));
            ctrlCodeData = ctrlCodeData.Replace(UICodeCreaterConfig.Temp_UI_Names, nameCode).Replace("UIClassName", UICodeCreaterConfig.UITypeClassName);
            FileUtils.WriteFile(codePath, ctrlCodeData);

        }

        /// <summary>
        /// 移除 UI 
        /// </summary>
        /// <param name="uiName"></param>
        private static void RemoveUINameClass(List<string> uiTypeList)
        {
            string codePath = Path.Combine(UICodeCreaterConfig.UICodeBasePath, "UIConst/UITypeName.cs");

            List<string> strLst = FileUtils.readAllLine(codePath);
            List<string> uiNameLst = new List<string>();
            int i = 0;
            while (i < strLst.Count)
            {
                if (strLst[i].Contains("public static string ") ||
                    strLst[i].Contains("public const string "))
                {
                    string uiStaticName = strLst[i].Split("\"")[1];
                    if (!uiNameLst.Contains(uiStaticName) && !uiTypeList.Contains(uiStaticName))
                    {
                        uiNameLst.Add(uiStaticName);
                    }
                }
                i++;
            }

            string nameCode = string.Empty;
            foreach (var name in uiNameLst)
            {
                nameCode += $"\t\tpublic const string {name} = \"{name}\";\n\n";
            }

            string ctrlCodeData = FileUtils.ReadFileStr(Path.Combine(UICodeCreaterConfig.UICodeTemplatePath, UICodeCreaterConfig.TxtUINames));
            ctrlCodeData = ctrlCodeData.Replace(UICodeCreaterConfig.Temp_UI_Names, nameCode).Replace("UIClassName", UICodeCreaterConfig.UITypeClassName);
            FileUtils.WriteFile(codePath, ctrlCodeData);

        }

        #endregion

        /// <summary>
        /// 自动生成代码时，对应的单个属性的详细信息
        /// </summary>
        public class EditorPropInfo
        {
            /// <summary>
            /// 类型的名称
            /// </summary>
            public string typeName;

            /// <summary>
            /// 私有属性的名称
            /// </summary>
            public string propPrivateName;

            /// <summary>
            /// 外部访问的属性名
            /// </summary>
            public string propPublicGetName;

            /// <summary>
            /// 属性对象路径
            /// </summary>
            public string objPath = "";

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                EditorPropInfo propInfo = obj as EditorPropInfo;
                if (propInfo == null) return false;

                if (string.IsNullOrEmpty(propInfo.objPath))
                {
                    return propInfo.typeName.Equals(typeName) && string.IsNullOrEmpty(objPath);
                }

                return propInfo.typeName.Equals(typeName) && propInfo.objPath.Equals(objPath);
            }

            public override int GetHashCode()
            {
                return typeName.GetHashCode() ^ objPath.GetHashCode();
            }
        }

    }


}
