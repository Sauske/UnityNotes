using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UMI
{
    public class UGUITools
    {
        public static float GetterReturn0()
        {
            return 0f;
        }

        public static float GetterReturn1()
        {
            return 1f;
        }

        public static float GetterReturnNegative1()
        {
            return -1f;
        }

        #region Btn Click

        public static void AddBtnClick(Button btn, UnityAction clickFun)
        {
            if (btn != null && clickFun != null)
            {
                btn.onClick.AddListener(clickFun);
            }
        }

        public static void AddButtonClick(GameObject go, UnityAction clickFun)
        {
            if (go == null) return;
            Button btn = go.GetComponent<Button>();
            AddBtnClick(btn, clickFun);
        }

        public static void RemoveBtnClick(Button btn, UnityAction clickFun)
        {
            if (btn != null && clickFun != null)
            {
                btn.onClick.RemoveListener(clickFun);
            }
        }

        public static void RemoveBtnClick(GameObject go, UnityAction clickFun)
        {
            if (go == null) return;
            Button btn = go.GetComponent<Button>();
            RemoveBtnClick(btn, clickFun);
        }

        public static void RemoveBtnAllClick(Button btn)
        {
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        public static void RemoveBtnAllClick(GameObject go)
        {
            if (go == null) return;
            Button btn = go.GetComponent<Button>();
            RemoveBtnAllClick(btn);
        }


        #endregion

        #region Btn ClickPos

        public delegate void OnBtnClickPos(int pos);
        public delegate void OnBtnGoClick(GameObject go);
        public delegate void onBtnClickGoPos(GameObject go, int pos);


        public static void AddBtnClickPos(Button btn, OnBtnClickPos posClickFun, int pos)
        {
            if (btn != null && posClickFun != null)
            {
                AddBtnClick(btn, delegate () { posClickFun(pos); });
            }
        }

        public static void AddBtnClickPos(GameObject go, OnBtnClickPos posClickFun, int pos)
        {
            if (go == null) return;
            Button btn = go.GetComponent<Button>();
            AddBtnClickPos(btn, posClickFun, pos);
        }

        public static void AddBtnClickGoPos(GameObject go, onBtnClickGoPos posClickFun, int nPos)
        {
            if (go == null || posClickFun == null) return;
            Button btn = go.GetComponent<Button>();

            if (btn == null) return;
            AddBtnClick(btn, delegate () { posClickFun(go, nPos); });
        }

        public static void AddBtnClickGo(GameObject go, OnBtnGoClick func)
        {
            if (go == null || func == null) return;
            Button btn = go.GetComponent<Button>();
            if (btn == null) return;
            AddBtnClick(btn, delegate () { func(go); });
        }

        #endregion



        #region Set Text Info

        public static void SetTextInfo(TMP_Text tmpText, string str)
        {
            if (tmpText != null)
            {
                tmpText.text = str;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmpText"></param>
        /// <param name="textId">文本表里的ID</param>
        public static void SetTextInfo(TMP_Text tmpText, int textId)
        {
            if (tmpText != null)
            {
                UILocalizationText uiText = tmpText.GetComponent<UILocalizationText>();
                if (uiText == null)
                {
                    string str = "ConfigMgr.Instance";// ConfigMgr.Instance.GetCommonString(textId);
                    tmpText.text = str;
                }
                else
                {
                    uiText.TextId = textId;
                    uiText.ChangeText();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmpText"></param>
        /// <param name="textId">文本表里的ID</param>
        /// <param name="args"></param>
        public static void SetTextInfo(TMP_Text tmpText, int textId, params object[] args)
        {
            if (tmpText != null)
            {
                string str = "ConfigMgr.Instance";// ConfigMgr.Instance.GetCommonString(textId, args);
                tmpText.text = str;
            }
        }

        public static void SetTextInfo(Text text, string str)
        {
            if (text != null)
            {
                text.text = str;
            }
        }

        #endregion

    }
}
