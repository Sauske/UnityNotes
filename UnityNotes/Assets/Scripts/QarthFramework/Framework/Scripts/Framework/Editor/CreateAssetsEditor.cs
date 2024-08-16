using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Qarth.Editor
{
    //创建sprite字体
    public class CreateFont : EditorWindow
    {
        [MenuItem("Tools/创建字体(sprite)")]
        public static void Open()
        {
            GetWindow<CreateFont>("创建字体");
        }

        private Texture2D tex;
        private string fontName;
        private string fontPath;

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("字体图片：");
            tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("字体名称：");
            fontName = EditorGUILayout.TextField(fontName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(string.IsNullOrEmpty(fontPath) ? "选择路径" : fontPath))
            {
                fontPath = EditorUtility.OpenFolderPanel("字体路径", Application.dataPath, "");
                if (string.IsNullOrEmpty(fontPath))
                {
                    Debug.Log("取消选择路径");
                }
                else
                {
                    fontPath = fontPath.Replace(Application.dataPath, "") + "/";
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("创建"))
            {
                Create();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void Create()
        {
            if (tex == null)
            {
                Debug.LogWarning("创建失败，图片为空！");
                return;
            }

            if (string.IsNullOrEmpty(fontPath))
            {
                Debug.LogWarning("字体路径为空！");
                return;
            }

            if (fontName == null)
            {
                Debug.LogWarning("创建失败，字体名称为空！");
                return;
            }
            else
            {
                if (File.Exists(Application.dataPath + fontPath + fontName + ".fontsettings"))
                {
                    Debug.LogError("创建失败，已存在同名字体文件");
                    return;
                }

                if (File.Exists(Application.dataPath + fontPath + fontName + ".mat"))
                {
                    Debug.LogError("创建失败，已存在同名字体材质文件");
                    return;
                }
            }

            string selectionPath = AssetDatabase.GetAssetPath(tex);
            if (selectionPath.Contains("/Resources/"))
            {
                string selectionExt = Path.GetExtension(selectionPath);
                if (selectionExt.Length == 0)
                {
                    Debug.LogError("创建失败！");
                    return;
                }

                string fontPathName = fontPath + fontName + ".fontsettings";
                string matPathName = fontPath + fontName + ".mat";
                float lineSpace = 0.1f;
                //string loadPath = selectionPath.Remove(selectionPath.Length - selectionExt.Length).Replace("Assets/Resources/", "");
                string loadPath = selectionPath.Replace(selectionExt, "")
                    .Substring(selectionPath.IndexOf("/Resources/") + "/Resources/".Length);
                Sprite[] sprites = Resources.LoadAll<Sprite>(loadPath);
                if (sprites.Length > 0)
                {
                    Material mat = new Material(Shader.Find("GUI/Text Shader"));
                    mat.SetTexture("_MainTex", tex);
                    Font m_myFont = new Font();
                    m_myFont.material = mat;
                    CharacterInfo[] characterInfo = new CharacterInfo[sprites.Length];
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        if (sprites[i].rect.height > lineSpace)
                        {
                            lineSpace = sprites[i].rect.height;
                        }
                    }

                    for (int i = 0; i < sprites.Length; i++)
                    {
                        Sprite spr = sprites[i];
                        CharacterInfo info = new CharacterInfo();
                        try
                        {
                            info.index = System.Convert.ToInt32(spr.name);
                        }
                        catch
                        {
                            Debug.LogError("创建失败，Sprite名称错误！");
                            return;
                        }

                        Rect rect = spr.rect;
                        float pivot = spr.pivot.y / rect.height - 0.5f;
                        if (pivot > 0)
                        {
                            pivot = -lineSpace / 2 - spr.pivot.y;
                        }
                        else if (pivot < 0)
                        {
                            pivot = -lineSpace / 2 + rect.height - spr.pivot.y;
                        }
                        else
                        {
                            pivot = -lineSpace / 2;
                        }

                        int offsetY = (int)(pivot + (lineSpace - rect.height) / 2);
                        info.uvBottomLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y) / tex.height);
                        info.uvBottomRight = new Vector2((float)(rect.x + rect.width) / tex.width,
                            (float)(rect.y) / tex.height);
                        info.uvTopLeft = new Vector2((float)rect.x / tex.width,
                            (float)(rect.y + rect.height) / tex.height);
                        info.uvTopRight = new Vector2((float)(rect.x + rect.width) / tex.width,
                            (float)(rect.y + rect.height) / tex.height);
                        info.minX = 0;
                        info.minY = -(int)rect.height - offsetY;
                        info.maxX = (int)rect.width;
                        info.maxY = -offsetY;
                        info.advance = (int)rect.width;
                        characterInfo[i] = info;
                    }

                    AssetDatabase.CreateAsset(mat, "Assets" + matPathName);
                    AssetDatabase.CreateAsset(m_myFont, "Assets" + fontPathName);
                    m_myFont.characterInfo = characterInfo;
                    EditorUtility.SetDirty(m_myFont);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(); //刷新资源
                    Debug.Log("创建字体成功");
                }
                else
                {
                    Debug.LogError("图集错误！");
                }
            }
            else
            {
                Debug.LogError("创建失败,选择的图片不在Resources文件夹内！");
            }
        }
    }

    //创建脚本
    public class ScriptCreatorEditor : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// 将要创建资源时会调用这个函数
        /// </summary>
        static void OnWillCreateAsset(string path)
        {
            //导入资源的路径，不知道具体是什么的时候建议输出查看
            Debug.Log(path);
            AssetDatabase.DeleteAsset(path);

            path = path.Replace(".meta", "");

            string[] splitArgs = path.Split('.');
            //创建的是脚本文件
            if (splitArgs[splitArgs.Length - 1].Equals("cs"))
            {
                string nameSapce = "";
                
                if (path.StartsWith("Assets/Framework"))
                {
                    nameSapce = "Qarth";
                }
                else if(path.StartsWith("Assets/GameScripts"))
                {
                    nameSapce = "MainGame";
                }
                
                string[] newSplitArgs = path.Split('/');
                bool isEditor = false;
                foreach (var item in newSplitArgs)
                {
                    if (item.Equals("Editor"))
                    {
                        if (!string.IsNullOrEmpty(nameSapce))
                        {
                            nameSapce += ".Editor";
                        }
                        break;
                    }
                }
                
                //6是Assets
                ParseAndChangeScript(path.Substring(6, path.Length - 6),nameSapce);
            }
        }

        private static void ParseAndChangeScript(string path,string namespaceName)
        {
            string str = File.ReadAllText(Application.dataPath + path);
            if (string.IsNullOrEmpty(str))
            {
                Debug.Log("读取出错了，Application.dataPath=" + Application.dataPath + "  path=" + path);
                return;
            }

            string newStr = "";
            //增加命名空间
            if (!str.Contains("namespace"))
            {
                if (!string.IsNullOrEmpty(namespaceName))
                {
                    int length = str.IndexOf("public");
                    newStr += str.Substring(0, length);
                    string extraStr = "";
                    string[] extraStrs = str.Substring(length, str.Length - length).Replace("\r\n", "\n").Split('\n');
                    foreach (var item in extraStrs)
                    {
                        extraStr += "\t" + item + "\r\n";
                    }

                    newStr += "\r\nnamespace " + namespaceName + "\r\n{\r\n" + extraStr + "}";
                    //newStr = newStr.Replace("\n", "\r\n");
                    //newStr = newStr.Replace('\r', ' ');
                }
                else
                {
                    newStr = str;
                }

                File.WriteAllText(Application.dataPath + path, newStr);
                AssetDatabase.SaveAssets();
            }
        }
    }

    // 创建 Text、Image 的时候默认不选中 raycastTarget 等
    public class UICreatHelper
    {
        /// <summary>
        /// 第一次创建UI元素时，没有canvas、EventSystem所有要生成，Canvas作为父节点
        /// 之后再空的位置上建UI元素会自动添加到Canvas下
        /// 在非UI树下的GameObject上新建UI元素也会 自动添加到Canvas下（默认在UI树下）
        /// 添加到指定的UI元素下
        /// </summary>
        [MenuItem("GameObject/UI/Image")]
        static void CreatImage()
        {
            var canvasObj = SecurityCheck();
            GameObject go = GetImage();
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            if (!Selection.activeTransform) // 在根目录创建的， 自动移动到 Canvas下
            {
                // Debug.Log("没有选择对象");
                go.transform.SetParent(canvasObj.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }
            else // (Selection.activeTransform)
            {
                if (!Selection.activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    go.transform.SetParent(canvasObj.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                }
            }
        }

        private static GameObject GetImage()
        {
            GameObject go = new GameObject("Img", typeof(Image));
            go.GetComponent<Image>().raycastTarget = false;
            go.transform.SetParent(Selection.activeTransform);
            Selection.activeGameObject = go;
            return go;
        }

        [MenuItem("GameObject/UI/Text")]
        static void CreatText()
        {
            var canvasObj = SecurityCheck();
            GameObject go = GetText();
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            if (!Selection.activeTransform) // 在根目录创建的， 自动移动到 Canvas下
            {
                // Debug.Log("没有选择对象");
                go.transform.SetParent(canvasObj.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }
            else // (Selection.activeTransform)
            {
                if (!Selection.activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    go.transform.SetParent(canvasObj.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                }
            }
        }

        private static GameObject GetText()
        {
            GameObject go = new GameObject("Text", typeof(Text));
            var text = go.GetComponent<Text>();
            text.raycastTarget = false;
            text.text = "text";
            text.alignment = TextAnchor.MiddleCenter;
            text.supportRichText = false;
            go.transform.SetParent(Selection.activeTransform);
            Selection.activeGameObject = go;

            //go.AddComponent<Outline>();   // 默认添加 附加组件
            return go;
        }

        [MenuItem("GameObject/UI/Button")]
        static void CreatButton()
        {
            var canvasObj = SecurityCheck();
            GameObject go = GetButton();
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            if (!Selection.activeTransform) // 在根目录创建的， 自动移动到 Canvas下
            {
                // Debug.Log("没有选择对象");
                go.transform.SetParent(canvasObj.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }
            else // (Selection.activeTransform)
            {
                if (!Selection.activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下
                {
                    go.transform.SetParent(canvasObj.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                }
            }
        }

        private static GameObject GetButton()
        {
            GameObject go = new GameObject("Btn", typeof(Button));
            go.transform.SetParent(Selection.activeTransform);
            Selection.activeGameObject = go;
            go.AddComponent<Image>();

            go.AddComponent<ButtonSoundCompo>(); // 默认添加 附加组件
            return go;
        }


        // 如果第一次创建UI元素 可能没有 Canvas、EventSystem对象！
        private static GameObject SecurityCheck()
        {
            GameObject canvas;
            var cc = Object.FindObjectOfType<Canvas>();
            if (!cc)
            {
                canvas = new GameObject("Canvas", typeof(Canvas));
            }
            else
            {
                canvas = cc.gameObject;
            }

            if (!Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
            {
                new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
            }

            return canvas;
        }
    }
}