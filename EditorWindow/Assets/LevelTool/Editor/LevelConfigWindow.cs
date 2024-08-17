using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace UMI
{
    public class LevelConfigWindow : EditorWindow
    {
        public static LevelConfigWindow Instance;

        private string[] options = new string[] { "空", "场景1", "场景2" };

        private int selectedIndex = 0;

        private const string config_scene_folder = "Assets/LevelTool/ConfigScene/";
        private const string config_scene_ext = "_config";

        static readonly string LEVEL_EXPORT_PATH = "Assets/LevelTool/DataBytes/Config/";

        [MenuItem("项目工具/关卡配置")]
        public static void OpenLevelConfigWindow()
        {
            Instance = GetWindow<LevelConfigWindow>("关卡配置");
            Instance.Show();
        }
        private SceneAsset sceneAsset;
        private SceneAsset lastSceneAsset;

        private UnityEngine.SceneManagement.Scene mConfigScene;        

        LevelConfigHelper helper = null;

        private void OnGUI()
        {

            selectedIndex = EditorGUI.Popup(new Rect(10, 10, position.width - 20, 20), "关卡：", selectedIndex, options);

            GUILayout.Space(30);
            GUILayout.BeginVertical();
            sceneAsset = EditorGUILayout.ObjectField("Scene", sceneAsset, typeof(SceneAsset), false) as SceneAsset;
            bool val = SceneChange();
            lastSceneAsset = sceneAsset;
            if (val && sceneAsset != null)
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                EditorSceneManager.OpenScene(scenePath);

                LoadOrCreateConfigScene(sceneAsset.name);
            }

            if (GUILayout.Button("刷新 config scene 下的配置"))
            {
               // var scene = EditorSceneManager.GetActiveScene();
                var scene = mConfigScene;
                if (scene.name.Contains(config_scene_ext))
                {
                    var gameObj = scene.GetRootGameObjects();
                    bool hasFind = false;

                    foreach (var obj in gameObj)
                    {
                        if (obj.GetComponent<LevelConfigHelper>())
                        {
                            hasFind = true;
                            helper = obj.GetComponent<LevelConfigHelper>();
                            helper.ConfigName = scene.name;
                            break;
                        }
                    }

                    if (hasFind == false)
                    {
                        GameObject tool = new GameObject("LevelConfigTool");
                        helper = tool.AddComponent<LevelConfigHelper>();
                        helper.ConfigName = scene.name;
                    }
                }
            }

            if (GUILayout.Button("保存 Config 场景"))
            {
                EditorSceneManager.SaveScene(mConfigScene, string.Format("{0}/{1}.unity", config_scene_folder, mConfigScene.name));
            }

            if (GUILayout.Button("读取配置"))
            {

            }

            if (GUILayout.Button("导出配置"))
            {
                SaveCongfig();
            }

            GUILayout.EndVertical();
        }

        private void LoadOrCreateConfigScene(string sceneName)
        {
            string path = config_scene_folder + sceneName + config_scene_ext + ".unity";
            string uid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(uid) == false)
            {
                //EditorSceneManager.LoadScene(path, UnityEngine.SceneManagement.LoadSceneMode.Additive);

                mConfigScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

                EditorSceneManager.SetActiveScene(mConfigScene);
            }
            else
            {
                mConfigScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

                string newSceneName = sceneName + config_scene_ext;
                mConfigScene.name = newSceneName;

                EditorSceneManager.SetActiveScene(mConfigScene);

                string scenePath = string.Format("{0}{1}.unity", config_scene_folder, newSceneName);
                EditorSceneManager.SaveScene(mConfigScene, scenePath, true);
            }
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;

            Instance = this;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        float mDownTime = 0;
        Vector3 mDownPosition;

        private void OnSceneGUI(SceneView sceneView)
        {

            if (!helper) return;

            if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                mDownPosition = Event.current.mousePosition;
                mDownTime = DateTime.Now.Ticks;
            }

            if (Event.current.button == 1 && Event.current.type == EventType.MouseUp && DateTime.Now.Ticks - mDownTime < 2000000)
            {
                Vector3 mousePosition = Event.current.mousePosition;

                //无视拖拽事件
                if ((mDownPosition - mousePosition).sqrMagnitude > 25)
                {
                    return;
                }

                mousePosition.y = sceneView.position.height - mousePosition.y;

                Vector3 targetPosition = sceneView.camera.transform.position + sceneView.camera.transform.forward * 30;

                Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);

                RaycastHit hitInfo;


                //命中了才弹出
                if (Physics.Raycast(ray, out hitInfo, 100, LayerMask.GetMask("Default")))
                {
                    targetPosition = hitInfo.point;

                    //抬高一点点避免深度冲突
                    targetPosition.y += 0.02f;

                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("鹅鸭杀/创建会议室交互点"), false, LevelConfigUtil.EYS_CreatePoint, new object[] { targetPosition, helper.PointRoot });
                          
                    menu.AddItem(new GUIContent("新增触发点/创建四点位区域触发器"), false, LevelConfigUtil.CreateGamePlayTrigger, new object[] { targetPosition, helper.GamePlayTriggerRoot });

                    // menu.AddItem(new GUIContent("创建出生区域"), false, LevelConfigUtil.CreateGamePlayTrigger, new object[] { targetPosition, helper.GamePlayTriggerRoot });

                    menu.AddItem(new GUIContent("新增触发点/创建规则触发区域"), false, LevelConfigUtil.CreateAreaTrigger, new object[] { targetPosition, helper.GamePlayTriggerRoot });

                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }


        }

        private bool SceneChange()
        {
            if (lastSceneAsset == null && sceneAsset == null)
            {
                return false;
            }

            if (lastSceneAsset == null && sceneAsset != null)
            {
                return true;
            }

            if (lastSceneAsset != null && sceneAsset == null)
            {
                return true;
            }

            return lastSceneAsset != sceneAsset;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SaveCongfig()
        {
            if (helper == null) return;

            EditorSceneManager.MarkSceneDirty(mConfigScene);

            EditorSceneManager.SaveScene(mConfigScene);

            switch (selectedIndex)
            {
                case 0:
                    LevelConfigUtil.ExportAction(helper, LEVEL_EXPORT_PATH);
                    break;
                case 1:
                    //LevelConfigUtil.EYS_Export(helper, LEVEL_EXPORT_PATH);
                    break;
                case 2:
                    LevelConfigUtil.Export(helper, LEVEL_EXPORT_PATH);
                    break;
                default:
                    throw new Exception("no defined");
            }
        }
    }
}


