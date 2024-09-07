using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UMI
{
    public class OverrideUIEditor
    {
        public const string FontPath = "Assets/Resources/TMPFonts/方正粗圆_GBK.ttf";
        public const string TMPFontPath = "Assets/Resources/TMPFonts/方正粗圆_GBK_TMP.asset";
        public const string Common_btn_blue = "Assets/Prefabs/UI/UICommon/__Textures/common_btn_blue.png";
        public const string Common_btn_red = "Assets/Prefabs/UI/UICommon/__Textures/common_btn_red.png";

        [MenuItem("GameObject/UI/Default Canvas")]
        private static void OnCreateUICanvas()
        {
            GameObject obj = new GameObject("Canvas", typeof(Canvas));
            var canvas = obj.GetComponent<Canvas>();
            var canvasScaler = canvas.gameObject.GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(UIMgr.UI_SCREEN_WIDTH, UIMgr.UI_SCREEN_HEIGHT);
            var graphic = canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        //[MenuItem("GameObject/UI/Text")]
        private static void OnCreateUIText()
        {
            var text = CreateComponent<Text>("Text");

            Font font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);

            text.transform.localPosition = Vector3.zero;
            text.transform.localScale = Vector3.one;
            text.transform.localRotation = Quaternion.identity;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 50);

            text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            text.raycastTarget = false;
            text.supportRichText = false;
            text.font = font;
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = "New Text";
            text.color = Color.black;
        }

        [MenuItem("GameObject/UI/Text_TMP")]
        private static void OnCreateUITMPText()
        {
            var text = CreateComponent<TextMeshProUGUI>("Text_TMP");

            TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TMPFontPath);

            text.transform.localPosition = Vector3.zero;
            text.transform.localScale = Vector3.one;
            text.transform.localRotation = Quaternion.identity;

            text.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 50);

            text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            text.raycastTarget = false;
            text.font = tmpFont;
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.text = "New Text";
            text.color = Color.black;
        }

        [MenuItem("GameObject/UI/My Image")]
        private static void OnCreateUIImage()
        {
            var image = CreateComponent<Image>("Image");
            image.raycastTarget = false;
            image.maskable = false;

            image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            image.transform.localPosition = Vector3.zero;
            image.transform.localScale = Vector3.one;
            image.transform.localRotation = Quaternion.identity;
        }

        [MenuItem("GameObject/UI/My Raw Image")]
        private static void OnCreateRawImage()
        {
            var rawImage = CreateComponent<RawImage>("RawImage");
            rawImage.raycastTarget = false;
            rawImage.maskable = false;

            rawImage.transform.localPosition = Vector3.zero;
            rawImage.transform.localScale = Vector3.one;
            rawImage.transform.localRotation = Quaternion.identity;

            rawImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        [MenuItem("GameObject/UI/Button")]
        private static void OnCreateUIButton()
        {
            var button = CreateComponent<Image>("Button");
            button.gameObject.AddComponent<Button>();

            var text = CreateComponent<TextMeshProUGUI>("TextBtn");
            text.transform.SetParent(button.transform);

            if (text != null)
            {
                TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TMPFontPath);

                text.transform.localPosition = Vector3.zero;
                text.transform.localScale = Vector3.one;
                text.transform.localRotation = Quaternion.identity;

                text.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                text.GetComponent<RectTransform>().anchorMax = Vector2.one;
                text.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                text.GetComponent<RectTransform>().offsetMax = Vector2.zero;

                text.raycastTarget = false;
                text.font = tmpFont;
                text.fontSize = 20;
                text.alignment = TextAlignmentOptions.Center;
                text.text = "Btn Text";
                text.color = Color.black;
            }

            //button.maskable = false;
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(Common_btn_blue);
            button.GetComponent<Image>().sprite = sp;

            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 50);
        }


        [MenuItem("GameObject/UI/Button_commonBlue")]
        private static void OnCreateUIButton_commonBlue()
        {
            var button = CreateComponent<Image>("Button");
            button.gameObject.AddComponent<Button>();

            var text = CreateComponent<TextMeshProUGUI>("TextBtnBlue");
            text.transform.SetParent(button.transform);

            if (text != null)
            {
                TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TMPFontPath);

                text.transform.localPosition = Vector3.zero;
                text.transform.localScale = Vector3.one;
                text.transform.localRotation = Quaternion.identity;

                text.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                text.GetComponent<RectTransform>().anchorMax = Vector2.one;
                text.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                text.GetComponent<RectTransform>().offsetMax = Vector2.zero;

                text.raycastTarget = false;
                text.font = tmpFont;
                text.fontSize = 20;
                text.alignment = TextAlignmentOptions.Center;
                text.text = "Btn Text";
                text.color = Color.black;
            }

            button.maskable = false;
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(Common_btn_blue);
            button.GetComponent<Image>().sprite = sp;

            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 50);
        }



        [MenuItem("GameObject/UI/Button_common_red")]
        private static void OnCreateUIButton_CommonRed()
        {
            var button = CreateComponent<Image>("Button");
            button.gameObject.AddComponent<Button>();

            var text = CreateComponent<TextMeshProUGUI>("TextBtnRed");
            text.transform.SetParent(button.transform);

            if (text != null)
            {
                TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TMPFontPath);

                text.transform.localPosition = Vector3.zero;
                text.transform.localScale = Vector3.one;
                text.transform.localRotation = Quaternion.identity;

                text.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                text.GetComponent<RectTransform>().anchorMax = Vector2.one;
                text.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                text.GetComponent<RectTransform>().offsetMax = Vector2.zero;

                text.raycastTarget = false;
                text.font = tmpFont;
                text.fontSize = 20;
                text.alignment = TextAlignmentOptions.Center;
                text.text = "Btn Text";
                // text.color = Color.black;
            }

            button.maskable = false;
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(Common_btn_red);
            button.GetComponent<Image>().sprite = sp;

            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 50);
        }



        /// <summary>
        /// 创建ui组件
        /// </summary>
        /// <param name="defaultName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T CreateComponent<T>(string defaultName) where T : Behaviour
        {
            Transform parentTra = CreateUIParent();

            GameObject go = new GameObject(defaultName, typeof(T));
            go.transform.SetParent(parentTra);
            Selection.activeGameObject = go;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            go.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            go.layer = LayerMask.NameToLayer("UI");
            return go.GetComponent<T>();
        }

        private static Transform CreateUIParent()
        {
            Transform parentTra = Selection.activeTransform;
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (Selection.activeTransform == null)
            {
                if (canvas != null)
                {
                    parentTra = canvas.transform;
                }
                else
                {
                    parentTra = new GameObject("Canvas", typeof(Canvas)).transform;
                }
                canvas = parentTra.GetComponent<Canvas>();
            }
            else if (!Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject obj = new GameObject("Canvas", typeof(Canvas));
                obj.SetGoParent(Selection.activeTransform);
                parentTra = obj.transform;
                canvas = obj.GetComponent<Canvas>();
            }
            else
            {
                canvas = Selection.activeTransform.GetComponentInParent<Canvas>();
            }

            var canvasScaler = canvas.gameObject.GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(UIMgr.UI_SCREEN_WIDTH, UIMgr.UI_SCREEN_HEIGHT);

            canvas.gameObject.SetGoAndChildrenlayer(LayerMask.NameToLayer("UI"), true);

            return parentTra;
        }
    }
}
