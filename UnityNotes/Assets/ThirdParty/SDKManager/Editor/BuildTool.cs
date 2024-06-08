using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class PlatformInfo {
    public PLATFORMTYPE platFormType = PLATFORMTYPE.None;
    public string iconName = "";
    public string appName = "";
    public string companyName = "";
    public string bundleID = "";
    public string androidPath = "";

    public PlatformInfo() {
    }

    public PlatformInfo(PLATFORMTYPE platFormType, string iconName, string companyName, string appName, string bundleID, string androidPath) {
        this.platFormType = platFormType;
        this.iconName = iconName;
        this.appName = appName;
        this.companyName = companyName;
        this.bundleID = bundleID;
        this.androidPath = androidPath;
    }
}

public class BuildTool : EditorWindow {

    Dictionary<string, PlatformInfo> mPlatformInfos = new Dictionary<string, PlatformInfo>();
    private string titlePlatform = "Third Platform Settings : ";
    private PLATFORMTYPE mPlatformType = PLATFORMTYPE.None;
    public string iconPath = "Textures/GameIcon/";
    private Texture2D appIcon = null;
    private string appName = "应用名称";
    private string companyName = "公司名称";
    private string androidSDKPath = "";
    private string streamingAssetsPath = "";
    public string localAndroidSDKPath = Application.dataPath + "/Plugins/Android";
    public string localStreamingAssetsPath = Application.dataPath + "/StreamingAssets"; //移动MM需要这个
    private bool needStreamingAssets = false;

    #region Out Look Parameters
    private GUIStyle titleStyle = new GUIStyle();
    private GUIStyleState titleState = new GUIStyleState();
    private static float totalWidth = 650;
    private float btnWidth = 100;
    private float labelWidth = 100;
    private float fieldWidth = 150;
    private float border = 4;

    private Vector2 scrollViewVector2 = Vector2.zero;
    #endregion

    #region Bundle Parameters
    private string titleBuileSetting = "Build Settings : ";
    private string bundleIdentifier = "";
    private string bundleVersion = "";
    private int bundleVersionCode = 1;
    private bool needToChangeKeyInfo = false;
    private string keystorePath = "";
    private string keystorePassword = "";
    private string keyName = "";
    private string keyPassword = "";
    #endregion


    [MenuItem("AutoBuilder/BuildTool")]
    static void DrawWindow() {
        BuildTool window = (BuildTool)EditorWindow.GetWindow(typeof(BuildTool));
    }

    void OnEnable() {
        InitStylePara();
        InitBuildPara();
        InitPlatformInfos();

    }

    void OnGUI() {

        {
            // 服务器设置// 
            AddTitleLabel("Package Tool, by Aklan");
            AddTitleLabel("Settings");
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("OpenGlobalConst", GUILayout.Width(btnWidth * 2))) {
                OpenGlobalConstScript();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        {
            // 平台设置//
            AddTitleLabel(titlePlatform);
            EditorGUILayout.BeginHorizontal();
            LeftNameLabel("Third Platform");
            PLATFORMTYPE platform = (PLATFORMTYPE)EditorGUILayout.EnumPopup(mPlatformType, GUILayout.Width(fieldWidth));
            if(mPlatformType != platform) {
                mPlatformType = platform;
                ShowPlatformInfo(mPlatformType);
                needStreamingAssets = false;
            }

            appIcon = EditorGUILayout.ObjectField("Icon  ->", appIcon, typeof(Texture2D), GUILayout.Width(labelWidth + fieldWidth)) as Texture2D;
            EditorGUILayout.EndHorizontal();
            appName = EditorGUILayout.TextField("App Name", appName, GUILayout.Width(labelWidth + fieldWidth));
            companyName = EditorGUILayout.TextField("Company Name", companyName, GUILayout.Width(labelWidth + fieldWidth));
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            LeftNameLabel("AndroidSDKPath");
            LeftNameLabel(androidSDKPath, labelWidth + fieldWidth);
            if(GUILayout.Button("Select SDK", GUILayout.Width(btnWidth))) {
                GetAndroidSDKPath();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if(needStreamingAssets) {
                EditorGUILayout.BeginHorizontal();
                LeftNameLabel("StreamingAssetsPath");
                LeftNameLabel(androidSDKPath, labelWidth + fieldWidth);
                if(GUILayout.Button("Select SA", GUILayout.Width(btnWidth))) {
                    GetStreamingAssetsPath();
                }
                if(GUILayout.Button("Import SA", GUILayout.Width(btnWidth))) {
                    CopyStreamingAssetsPath();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        {
            // 包信息设置//
            AddTitleLabel(titleBuileSetting);
            EditorGUILayout.BeginHorizontal();
            LeftNameLabel("Bundle Identifier");
            bundleIdentifier = EditorGUILayout.TextField(bundleIdentifier, GUILayout.Width(fieldWidth));
            LeftNameLabel("Bundle Version");
            bundleVersion = EditorGUILayout.TextField(bundleVersion, GUILayout.Width(fieldWidth / 2));
            LeftNameLabel("Version Code");
            bundleVersionCode = EditorGUILayout.IntField(bundleVersionCode, GUILayout.Width(fieldWidth / 2));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            needToChangeKeyInfo = EditorGUILayout.BeginToggleGroup("Need To Change KeyInfo", needToChangeKeyInfo);
            LeftNameLabel("-- Keystore");
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Browse Keystore", GUILayout.Width(btnWidth * 2))) {
                UnityOpenFile();
            }
            EditorGUILayout.LabelField(keystorePath);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            LeftNameLabel("Keystore password", 20);
            keystorePassword = EditorGUILayout.TextField(keystorePassword, GUILayout.Width(fieldWidth - 20));
            EditorGUILayout.EndHorizontal();

            LeftNameLabel("-- Key");
            EditorGUILayout.BeginHorizontal();
            LeftNameLabel("Name");
            keyName = EditorGUILayout.TextField(keyName, GUILayout.Width(fieldWidth));
            LeftNameLabel("Password");
            keyPassword = EditorGUILayout.TextField(keyPassword, GUILayout.Width(fieldWidth));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndToggleGroup();
        }

        if(GUILayout.Button("Save Build Settings", GUILayout.Width(btnWidth * 2))) {
            SaveBuildValue();
        }

        EditorGUILayout.Space();
        if(GUILayout.Button("One Key Build Android", GUILayout.Width(btnWidth * 2))) {
            SaveBuildValue();
            CopyAndroidSDK();
            BuildToAndroid();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.TextArea("现版本注意事项及发布步骤：\n" +
                                  "发布前需手动更改 GlobalConst.cs 里面的平台参数\n" +
                                  "把SDK按渠道归类,比如'C:/Users/Aklan/Desktop/SDK/MI'为小米渠道,且不要放到Assets目录下\n" +
                                  "发布时：\n" +
                                  "1.选择平台\n" +
                                  "2.选择SDK目录,比如'C:/Users/Aklan/Desktop/SDK/MI'\n" +
                                  "3.点击一键发布按钮\n" +
                                  "注意：\n" +
                                  "1.需要手动在脚本中先添加各平台参数\n" +
                                  "（脚本 BuildTool.cs 函数 InitPlatformInfos 赋值）\n" +
                                  "2.平台的android目录不含中文时，代码能自动保存路径，该工具未关闭时,下次可以不用再选\n" +
                                  "3.生成结果在项目工程目录Builds/Android下,并按日期命名");
    }

    public void OpenGlobalConstScript() {
        string path = "Assets/Scripts/Common/GlobalConst.cs";
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(path);
        if(obj != null) {
            AssetDatabase.OpenAsset(obj);
        }
        else {
            Debug.Log("ERROR: Open Script File Failed!");
        }
    }

    private void LeftNameLabel(string name, float offsetWidth = 0) {
        EditorGUILayout.LabelField(name, GUILayout.Width(labelWidth + offsetWidth));
    }

    private void AddTitleLabel(string title) {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent(title), titleStyle);
        EditorGUILayout.Space();
    }

    private void UnityOpenFile() {
        keystorePath = EditorUtility.OpenFilePanel("Select Keystore", "", "keystore");
    }

    private void GetStreamingAssetsPath() {
        string path = EditorUtility.OpenFolderPanel("Select Android File", androidSDKPath, "Android");
        if(path != "") {
            streamingAssetsPath = path;
            SaveStreamingAssetsPath();
        }
    }

    private void CopyStreamingAssetsPath() {
        AssetsCopyDirectory(streamingAssetsPath, localStreamingAssetsPath);

    }

    private void AssetsCopyDirectory(string fromPath, string toPath) {
        FileUtil.DeleteFileOrDirectory(toPath);
        AssetDatabase.Refresh();
        if(Directory.Exists(fromPath)) {
            if(Directory.Exists(toPath)) {
                string[] files = Directory.GetFiles(fromPath + "/");
                foreach(string file in files) {
                    string name = file.Substring(file.LastIndexOf("/"));
                    FileUtil.CopyFileOrDirectory(file, toPath + "/" + name);
                }

                string[] dirs = Directory.GetDirectories(fromPath + "/");
                foreach(string dir in dirs) {
                    string name = dir.Substring(dir.LastIndexOf("/"));
                    FileUtil.CopyFileOrDirectory(dir, toPath + "/" + name);
                }
            }
            else {
                FileUtil.CopyFileOrDirectory(fromPath, toPath);
            }
        }
        AssetDatabase.Refresh();
    }

    private void GetAndroidSDKPath() {
        string path = EditorUtility.OpenFolderPanel("Select Android File", androidSDKPath, "Android");
        if(path != "")
            androidSDKPath = path;
        PlatformInfo info = GetPlatformInfo(mPlatformType);
        info.androidPath = androidSDKPath;
        SetLocalSDKPath(androidSDKPath);
    }

    private void CopyAndroidSDK() {
        AssetsCopyDirectory(androidSDKPath, localAndroidSDKPath);
    }

    private void InitStylePara() {
        titleState.textColor = Color.white;
        titleStyle.normal = titleState;
        titleStyle.fontStyle = FontStyle.Bold;
    }

    private void InitBuildPara() {
        bundleIdentifier = PlayerSettings.applicationIdentifier;
        bundleVersion = PlayerSettings.bundleVersion;
        bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
        keystorePath = PlayerSettings.Android.keystoreName;
        keystorePassword = "";
        keyName = PlayerSettings.Android.keyaliasName;
        keyPassword = "";
        appIcon = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown)[0];
    }

    private void InitPlatformInfos() {
        AddPlatformInfo(new PlatformInfo(PLATFORMTYPE.None, "ICON", "公司名称", "应用名称", bundleIdentifier, GetLocalSDKPath(PLATFORMTYPE.None)));
        AddPlatformInfo(new PlatformInfo(PLATFORMTYPE.Android_360, "ICON", "BuddyGame", "AklanTest_360", bundleIdentifier, GetLocalSDKPath(PLATFORMTYPE.Android_360)));
        AddPlatformInfo(new PlatformInfo(PLATFORMTYPE.Android_MI, "ICON_MI", "BuddyGame", "AklanTest_Mi", bundleIdentifier, GetLocalSDKPath(PLATFORMTYPE.Android_MI)));
        AddPlatformInfo(new PlatformInfo(PLATFORMTYPE.Android_UC, "ICON_UC", "BuddyGame", "AklanTest_UC", bundleIdentifier, GetLocalSDKPath(PLATFORMTYPE.Android_UC)));
    }

    private void AddPlatformInfo(PlatformInfo info) {
        if(!mPlatformInfos.ContainsKey(info.platFormType.ToString())) {
            mPlatformInfos.Add(info.platFormType.ToString(), info);
        }
    }

    private PlatformInfo GetPlatformInfo(PLATFORMTYPE platformtype) {
        PlatformInfo info = new PlatformInfo();
        if(mPlatformInfos.TryGetValue(platformtype.ToString(), out info)) {

        }
        else {
            Debug.LogError("ERROR: platform error.");
        }
        return info;
    }

    public void ShowPlatformInfo(PLATFORMTYPE platformtype) {
        PlatformInfo info = GetPlatformInfo(platformtype);
        appName = info.appName;
        companyName = info.companyName;
        bundleIdentifier = info.bundleID;
        androidSDKPath = info.androidPath;
        appIcon = Resources.Load(iconPath + info.iconName) as Texture2D;
    }

    public void SaveBuildValue() {
        PlayerSettings.applicationIdentifier = bundleIdentifier;
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = keystorePassword;
        PlayerSettings.Android.keyaliasName = keyName;
        PlayerSettings.Android.keyaliasPass = keyPassword;
        PlayerSettings.productName = appName;
        PlayerSettings.companyName = companyName;
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { appIcon });
    }

    public void BuildToAndroid() {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        string path = "Builds/Android/APK" + System.DateTime.Now.ToString("MM-dd") + ".apk";
        if(File.Exists(path)) {
            File.Delete(path);
        }
        BuildPipeline.BuildPlayer(AutoBuilder.GetScenePaths(), path, BuildTarget.Android, BuildOptions.None);
    }

    public void SetLocalSDKPath(string path) {
        string key = string.Format("LocalSDKPath{0}", mPlatformType);
        PlayerPrefs.SetString(key, path);
    }

    public string GetLocalSDKPath(PLATFORMTYPE platformtype) {
        string key = string.Format("LocalSDKPath{0}", platformtype);
        return PlayerPrefs.GetString(key);
    }

    private void SaveStreamingAssetsPath() {
        string key = "StreamingAssetsPath";
        PlayerPrefs.SetString(key, streamingAssetsPath);
    }

    private void LoadStreamingAssetsPath() {
        string key = "StreamingAssetsPath";
        streamingAssetsPath = PlayerPrefs.GetString(key);
    }
}
