using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace UMI.Editor
{
    public class UGUIComponentEditor
    {

        /// <summary>
        /// 添加Button按钮声音脚本
        /// </summary>
        [MenuItem("Assets/UMI 生成 UI 脚本/添加 按钮 声音 脚本")]
        public static void MenuAddButtonAudio()
        {
            string selectAssetPath = UICodeEditor.GetSelectPath();

            if (string.IsNullOrEmpty(selectAssetPath))
            {
                Debug.LogError("请选择UI路径。");
                return;
            }
            AddButtonAudioScripte(selectAssetPath);

            UICodeEditor.SetSelectAAName(selectAssetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("添加按钮声音");
        }


        public static void AddButtonAudioScripte(string path)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            for (int idx = 0; idx < guids.Length; idx++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[idx]);
                Debug.Log(assetPath);

                GameObject orgAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (orgAsset != null)
                {
                    GameObject go = GameObject.Instantiate(orgAsset);

                    UnityEngine.UI.Button[] buttons = go.GetComponentsInChildren<UnityEngine.UI.Button>();
                    for (int subidx = 0; subidx < buttons.Length; subidx++)
                    {
                        Debug.Log(buttons[subidx].name);

                        ButtonAudioComponent compent = buttons[subidx].gameObject.GetComponent<ButtonAudioComponent>();
                        if (compent == null)
                        {
                            buttons[subidx].gameObject.AddComponent<ButtonAudioComponent>();
                        }
                    }
                    bool bSuccess;
                    PrefabUtility.SaveAsPrefabAsset(go, assetPath, out bSuccess);

                    GameObject.Destroy(go);
                    if (!bSuccess)
                    {
                        Debug.LogError("保存预制失败。");
                    }
                }
            }
        }

        /// <summary>
        /// 添加Button按钮声音脚本
        /// </summary>
        [MenuItem("Assets/UMI 生成 UI 脚本/检查带TextMesh_Input的对象")]
        public static void CheckInputText()
        {
            string selectAssetPath = UICodeEditor.GetSelectPath();

            if (string.IsNullOrEmpty(selectAssetPath))
            {
                Debug.LogError("请选择UI路径。");
                return;
            }
            CheckInputScripte(selectAssetPath);

            UICodeEditor.SetSelectAAName(selectAssetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("添加按钮声音");
        }

        private static void CheckInputScripte(string path)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            for (int idx = 0; idx < guids.Length; idx++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[idx]);
                Debug.Log(assetPath);

                GameObject orgAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (orgAsset != null)
                {
                    GameObject go = GameObject.Instantiate(orgAsset);
                                      
                    TMP_InputField[] inputs = orgAsset.GetComponentsInChildren<TMP_InputField>();

                    bool bChange = false;

                    for (int subidx = 0; subidx < inputs.Length; subidx++)
                    {
                        Debug.Log(inputs[subidx].name);

                        InputFieldComponent compent = inputs[subidx].gameObject.GetComponent<InputFieldComponent>();
                        if (compent == null)
                        {
                            inputs[subidx].gameObject.AddComponent<InputFieldComponent>();
                            bChange = true;
                        }
                    }                   

                    if (bChange)
                    {
                        bool bSuccess;
                        PrefabUtility.SaveAsPrefabAsset(go, assetPath, out bSuccess);

                        if (!bSuccess)
                        {
                            Debug.LogError("保存预制失败。");
                        }
                    }
                    GameObject.DestroyImmediate(go);                 
                }
            }
        }

        /// <summary>
        /// 添加Button按钮声音脚本
        /// </summary>
        [MenuItem("Assets/UMI 生成 UI 脚本/检查带 TextMeshProUGUI 的对象")]
        public static void CheckTextMeshProUGUI()
        {
            string selectAssetPath = UICodeEditor.GetSelectPath();

            if (string.IsNullOrEmpty(selectAssetPath))
            {
                Debug.LogError("请选择UI路径。");
                return;
            }
            CheckTextScripte(selectAssetPath);

            UICodeEditor.SetSelectAAName(selectAssetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("添加按钮声音");
        }

        private static void CheckTextScripte(string path)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            for (int idx = 0; idx < guids.Length; idx++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[idx]);
                Debug.Log(assetPath);

                GameObject orgAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (orgAsset != null)
                {
                    GameObject go = GameObject.Instantiate(orgAsset);

                    TextMeshProUGUI[] meshTexts = orgAsset.GetComponentsInChildren<TextMeshProUGUI>();         
                    bool bChange = false;

                    for (int subidx = 0; subidx < meshTexts.Length; subidx++)
                    {
                        //Debug.Log(meshTexts[subidx].name);

                        //UILocalizationText compent = meshTexts[subidx].gameObject.GetComponent<UILocalizationText>();
                        //if (compent == null)
                        //{
                        //    meshTexts[subidx].gameObject.AddComponent<UILocalizationText>();
                        //    bChange = true;
                        //}
                    }

                    if (bChange)
                    {
                        bool bSuccess;
                        PrefabUtility.SaveAsPrefabAsset(go, assetPath, out bSuccess);

                        if (!bSuccess)
                        {
                            Debug.LogError("保存预制失败。");
                        }
                    }

                    Text[] texts = orgAsset.GetComponentsInChildren<Text>();
                    for (int subidx = 0; subidx < texts.Length; subidx++)
                    {
                        Debug.LogErrorFormat("预制体：{0}，路径：{1}", orgAsset.name, texts[subidx].name);
                    }

                    GameObject.DestroyImmediate(go);
                }
            }
        }
    }
}
