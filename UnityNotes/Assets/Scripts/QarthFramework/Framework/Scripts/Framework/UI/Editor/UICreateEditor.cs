using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Qarth.Editor
{
    //创建UI组件后处理
    public class UICreateEditor : UnityEditor.Editor
    {
        private const string m_Menu = "GameObject/UI/";

        private static Vector2 m_NormalGUIElementSize = new Vector2(100, 100);
        private static Vector2 m_ThickGUIElementSize = new Vector2(160, 30);
        private static Vector2 m_ScrollRectSize = new Vector2(400, 100);

        [MenuItem(m_Menu + "NoGraphicButton(空UI用于点击)", false, 1001)]
        public static void AddNoGraphicButton(MenuCommand menuCommand)
        {
            var go = CreateUIElementRoot("NoGraphicButton", menuCommand, m_NormalGUIElementSize);
            var image = go.AddComponent<NoGraphicButton>();
            var button = go.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            image.raycastTarget = true;
        }

        [MenuItem(m_Menu + "ScrollRect(水平列表)", false, 1002)]
        static public void AddScrollRectHorizontal(MenuCommand menuCommand)
        {
            AddScrollRect(menuCommand, typeof(HorizontalLayoutGroup));
        }

        [MenuItem(m_Menu + "ScrollRect(垂直列表)", false, 1003)]
        static public void AddScrollRectVertical(MenuCommand menuCommand)
        {
            AddScrollRect(menuCommand, typeof(VerticalLayoutGroup));
        }

        [MenuItem(m_Menu + "ScrollRect(格子排列)", false, 1004)]
        static public void AddScrollRectGrid(MenuCommand menuCommand)
        {
            AddScrollRect(menuCommand, typeof(GridLayoutGroup));
        }

        static public void AddScrollRect<T>(MenuCommand menuCommand, T type)
        {
            var go = CreateUIElementRoot("ScrollRect", menuCommand, m_ScrollRectSize);
            var scrollRect = go.AddComponent<ScrollRect>();
            var rect = go.GetComponent<RectTransform>();
            go.AddComponent<NoGraphicButton>().raycastTarget = true;
            go.AddComponent<RectMask2D>();

            //Create Content
            GameObject content = new GameObject("Content");
            GameObjectUtility.SetParentAndAlign(content, go);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.sizeDelta = m_ScrollRectSize;
            SetPositionVisibleinSceneView(rect, contentRect);
            scrollRect.content = contentRect;

            //Create Child
            GameObject item = new GameObject("Image");
            var itemRect = item.AddComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(100, 100);
            GameObjectUtility.SetParentAndAlign(item, content);
            SetPositionVisibleinSceneView(rect, itemRect);
            item.AddComponent<Image>();

            if (type.GetHashCode() == typeof(HorizontalLayoutGroup).GetHashCode())
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
                var layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
                layoutGroup.spacing = 10;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
                content.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else if (type.GetHashCode() == typeof(VerticalLayoutGroup).GetHashCode())
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                var layoutGroup = content.AddComponent<VerticalLayoutGroup>();
                layoutGroup.spacing = 10;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
                content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                //顶对齐
                contentRect.SetAnchor(AnchorType.StretchTop);
            }
            else if (type.GetHashCode() == typeof(GridLayoutGroup).GetHashCode())
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                var layoutGroup = content.AddComponent<GridLayoutGroup>();
                layoutGroup.cellSize = new Vector3(100, 100);
                layoutGroup.spacing = new Vector2(10, 10);
            }
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform,
                new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera,
                out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) +
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) +
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) -
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) -
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null)
                return null;

            T comp = null;
            Transform t = go.transform;
            while (t != null && comp == null)
            {
                comp = t.GetComponent<T>();
                t = t.parent;
            }

            return comp;
        }
        
        // Helper methods
        static public GameObject CreateNewCanvas()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer("UI");
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            bool customScene = false;
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                root.transform.SetParent(prefabStage.prefabContentsRoot.transform, false);
                customScene = true;
            }

            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // If there is no event system add one...
            // No need to place event system in custom scene as these are temporary anyway.
            // It can be argued for or against placing it in the user scenes,
            // but let's not modify scene user is not currently looking at.
            if (!customScene)
                CreateEventSystem(false,null);
            return root;
        }
        
        private static void CreateEventSystem(bool select, GameObject parent)
        {
            StageHandle stage = parent == null ? StageUtility.GetCurrentStageHandle() : StageUtility.GetStageHandle(parent);
            var esys = stage.FindComponentOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                if (parent == null)
                    StageUtility.PlaceGameObjectInCurrentStage(eventSystem);
                else
                    GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }
        
        // Helper function that returns the selected root object.
        static public GameObject GetParentActiveCanvasInSelection(bool createIfMissing)
        {
            GameObject go = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if ots parents
            Canvas p = (go != null) ? FindInParents<Canvas>(go) : null;
            // Only use active objects
            if (p != null && p.gameObject.activeInHierarchy)
                go = p.gameObject;

            // No canvas in selection or its parents? Then use just any canvas.
            if (go == null)
            {
                Canvas canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
                if (canvas != null)
                    go = canvas.gameObject;
            }

            // No canvas present? Create a new one.
            if (createIfMissing && go == null)
                go = CreateNewCanvas();

            return go;
        }

        private static GameObject CreateUIElementRoot(string name, MenuCommand menuCommand, Vector2 size)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || FindInParents<Canvas>(parent) == null)
            {
                parent = GetParentActiveCanvasInSelection(true);
            }

            GameObject child = new GameObject(name);

            Undo.RegisterCreatedObjectUndo(child, "Create " + name);
            Undo.SetTransformParent(child.transform, parent.transform, "Parent " + child.name);
            GameObjectUtility.SetParentAndAlign(child, parent);

            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            if (parent != menuCommand.context) // not a context click, so center in sceneview
            {
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), rectTransform);
            }

            Selection.activeGameObject = child;
            return child;
        }

        public static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        public static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        public static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }
    }
}