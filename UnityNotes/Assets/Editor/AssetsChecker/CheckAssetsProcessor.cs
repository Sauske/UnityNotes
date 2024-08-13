using GameEditor.LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class CheckAssetsProcessor
{
    const string URP_Lit_GUID = "933532a4fcc9baf4fa0491de14d08ed7";
    const string Game_Lit_GUID = "b171e2c06b0ffc34db10ba0500fa052c";
    const string URP_Lit_Mat_GUID = "31321ba15b8f8eb4c954353edc038b1d";
    const string Game_Lit_Mat_GUID = "4bd558bda241a6f4396d101d63d83f54";
    private static JsonData result;
    public static bool fix = true;


    [MenuItem("Tools/检查美术资产")]
    public static void Check()
    {
        result = new JsonData();
        result.SetJsonType(JsonType.Object);
        var material_guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        var prefab_guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        var scene_guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        var model_guids = AssetDatabase.FindAssets("t:model", new[] { "Assets" });
        var texture_guids = AssetDatabase.FindAssets("t:texture", new[] { "Assets" });

        CheckModel(model_guids);
        CheckMaterial(material_guids);
        CheckPrefab(prefab_guids);
        CheckScene(scene_guids);
        CheckTexture(texture_guids);
        SaveResult();

        if (fix)
        {
            //Fix_use_urp_lit_material();
            //Fix_shader_workflow_specular();
            //Fix_use_urp_lit_shader_mat();
        }
    }

    private static void CheckMaterial(string[] guids)
    {
        //var urp_lit_path = AssetDatabase.GUIDToAssetPath(URP_Lit_GUID);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            {
                var allAs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var asset in allAs)
                {
                    if (asset is Material mtA && mtA.shader.name.Equals("Universal Render Pipeline/Lit"))
                    {

                        if (path.Contains(".fbx", System.StringComparison.OrdinalIgnoreCase) || path.Contains(".obj", System.StringComparison.OrdinalIgnoreCase))
                            AddArray("use_urp_lit_shader_model", path);
                        else if(path.EndsWith(".mat"))
                            AddArray("use_urp_lit_shader_mat", path);
                        else
                            AddArray("use_urp_lit_shader", path);
                    }
                }
            }

            var mt = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mt.HasFloat("_WorkflowMode") && mt.GetFloat("_WorkflowMode") == 0)
                AddArray("shader_workflow_specular", path);
        }
    }

    private static void AddArray(string key, string value)
    {
        JsonData data = null;
        if (!result.Contains(key))
        {
            data = new JsonData();
            result[key] = data;
            switch (key)
            {
                case "shader_workflow_specular":
                    data.Add("#----------------------材质工作流为Specular----------------------#");
                    break;
                case "use_urp_lit_shader":
                    data.Add("#----------------------资源使用的shader为urp自带的lit shader，请改为项目中的shader----------------------#");
                    break;
                case "use_urp_lit_shader_mat":
                    data.Add("#----------------------材质使用的shader为urp自带的lit shader，请改为项目中的shader----------------------#");
                    break;
                case "use_urp_lit_shader_model":
                    data.Add("#----------------------模型导入默认材质，请使用项目的shader手动创建材质球并赋值----------------------#");
                    break;
                case "model_import_assign_mt_none":
                    data.Add("#----------------------导入的模型未设置材质----------------------#");
                    break;
                case "use_urp_lit_material":
                    data.Add("#----------------------使用了URP自带的Lit材质，请使用项目shader创建的材质----------------------#");
                    break;
                case "model_import_mt_mode_error":
                    data.Add("#----------------------模型导入材质选项不正确，请改为Import via MaterialDescription选项----------------------#");
                    break;
                case "texture_android_setting_error":
                    data.Add("#----------------------Android平台纹理格式设置不正确----------------------#");
                    break;
                case "texture_iOS_setting_error":
                    data.Add("#----------------------iOS平台纹理格式设置不正确----------------------#");
                    break;
            }
        }

        data = result[key];
        data.Add(value);
    }

    private static void CheckPrefab(string[] guids)
    {
        //var mtPath = AssetDatabase.GUIDToAssetPath(URP_Lit_Mat_GUID);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var rends = go.GetComponentsInChildren<MeshRenderer>();
            foreach (var rend in rends)
            {
                foreach (var mt in rend.sharedMaterials)
                {
                    var mPath = AssetDatabase.GetAssetPath(mt);
                    var id = AssetDatabase.AssetPathToGUID(mPath);
                    if (id.Equals(URP_Lit_Mat_GUID))
                        AddArray("use_urp_lit_material", path);
                }
            }
        }
    }

    private static void CheckScene(string[] guids)
    {
        //var mtPath = AssetDatabase.GUIDToAssetPath(URP_Lit_Mat_GUID);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (File.ReadAllText(path).Contains(URP_Lit_Mat_GUID))
                AddArray("use_urp_lit_material", path);
        }
    }

    private static void CheckModel(string[] guids)
    {
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter ti = AssetImporter.GetAtPath(path) as ModelImporter;
            if (ti == null)
                continue;

            if (ti.materialImportMode != ModelImporterMaterialImportMode.ImportViaMaterialDescription)
            {
                AddArray("model_import_mt_mode_error", path);
            }
            else
            {
                var mt = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mt)
                    AddArray("model_import_assign_mt_none", path);
            }
        }
    }

    private static void CheckTexture(string[] guids)
    {
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null)
                return;

            var android = ti.GetPlatformTextureSettings("Android");
            var iOS = ti.GetPlatformTextureSettings("iPhone");
            if (android.format != TextureImporterFormat.AutomaticCompressed && !android.format.ToString().Contains("astc", System.StringComparison.OrdinalIgnoreCase))
                AddArray("texture_android_setting_error", path);

            if (iOS.format != TextureImporterFormat.AutomaticCompressed && !iOS.format.ToString().Contains("astc", System.StringComparison.OrdinalIgnoreCase))
                AddArray("texture_iOS_setting_error", path);

            //android.format = format;
            //android.maxTextureSize = maxSize;
            //android.crunchedCompression = true;
            //android.compressionQuality = (int)TextureCompressionQuality.Normal;
            //android.overridden = true;

            ////TextureImporterPlatformSettings iOS = new TextureImporterPlatformSettings();
            ////iOS.name = "iPhone";
            //iOS.format = format;
            //iOS.maxTextureSize = maxSize;
            //iOS.crunchedCompression = true;
            //iOS.compressionQuality = (int)TextureCompressionQuality.Normal;
            //iOS.overridden = true;

            //ti.SetPlatformTextureSettings(android);
            //ti.SetPlatformTextureSettings(iOS);
            //ti.SaveAndReimport();
            ////AssetDatabase.Refresh();

            ////if ((an.format != TextureImporterFormat.AutomaticCompressed && an.format != format) || (ip.format != TextureImporterFormat.AutomaticCompressed && ip.format != format))
            ////{
            ////    an.format = format;
            ////    ip.format = format;
            ////    ti.SetPlatformTextureSettings(an);
            ////    ti.SetPlatformTextureSettings(ip);
            ////    ti.SaveAndReimport();
            ////    AssetDatabase.Refresh();
            ////}
        }
    }

    const string ShaderLogFilePath = "Assets/Editor/ShaderBuildLog.txt";
    public static HashSet<string> keywords = new();
    public static int variantCount;

    public static void OnPreprocessBundleBuild()
    {
        keywords.Clear();
        variantCount = 0;
        if (System.IO.File.Exists(ShaderLogFilePath))
            System.IO.File.Delete(ShaderLogFilePath);
    }

    public static void OnPostprocessBundleBuild()
    {
        if (System.IO.File.Exists(ShaderLogFilePath))
        {
            var kws = keywords.ToList();
            kws.Sort();
            var sb = new StringBuilder();
            var couStr = $"variant count:{variantCount}";
            sb.AppendLine();
            sb.AppendLine(couStr);
            sb.AppendLine($"kwy word count:{kws.Count}");
            sb.AppendLine($"key word list:\n\t{string.Join("\n\t", kws)}");
            sb.AppendLine();
            var tmpStr = System.IO.File.ReadAllText(ShaderLogFilePath);
            sb.AppendLine(tmpStr);
            System.IO.File.WriteAllText(ShaderLogFilePath, sb.ToString());
            //System.Diagnostics.Process.Start("C:\\Users\\weiqy\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe", ShaderLogFilePath);
            //AssetDatabase.Refresh();
        }
    }

    private static void Fix_use_urp_lit_material()
    {
        //AssetDatabase.StartAssetEditing();
        var data = result["use_urp_lit_material"];
        if (data == null)
            return;

        for (int i = 1; i < data.Count; i++)
        {
            var path = data[i].ToString();
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if (ai != null)
            {
                var content = File.ReadAllText(path).Replace(URP_Lit_Mat_GUID, Game_Lit_Mat_GUID);
                File.WriteAllText(path, content);
                ai.SaveAndReimport();
            }
        }
        //AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    private static void Fix_shader_workflow_specular()
    {
        var data = result["shader_workflow_specular"];
        if (data == null)
            return;

        for (int i = 1; i < data.Count; i++)
        {
            var path = data[i].ToString();
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if (ai != null)
            {
                var mt = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mt.HasFloat("_WorkflowMode") && mt.GetFloat("_WorkflowMode") == 0)
                {
                    mt.SetFloat("_WorkflowMode", 1);
                    ai.SaveAndReimport();
                }
            }
        }

        AssetDatabase.Refresh();
    }

    private static void Fix_use_urp_lit_shader_mat()
    {
        var data = result["use_urp_lit_shader_mat"];
        if (data == null)
            return;

        for (int i = 1; i < data.Count; i++)
        {
            var path = data[i].ToString();
            AssetImporter ai = AssetImporter.GetAtPath(path);
            if (ai != null)
            {
                var mt = AssetDatabase.LoadAssetAtPath<Material>(path);
                var content = File.ReadAllText(path).Replace($"fileID: 4800000, guid: {URP_Lit_GUID}", $"fileID: -6465566751694194690, guid: {Game_Lit_GUID}");
                File.WriteAllText(path, content);
                ai.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }

        AssetDatabase.Refresh();
    }

    private static void SaveResult()
    {
        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        var ss = reg.Replace(result.ToJson(), delegate (Match m) { return ((char)System.Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        System.IO.File.WriteAllText("Assets/Editor/CheckAssetsLog.json", ss);
        AssetDatabase.Refresh();
    }
}
