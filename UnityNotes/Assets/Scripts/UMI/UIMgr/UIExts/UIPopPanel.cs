using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    /// <summary>
    /// 未点击到指定区域时，隐藏对应的显示
    /// </summary>
    public class UIPopPanel : MonoBehaviour
    {
        public Action mHideFun;
        public List<GameObject> mExtentObj = new List<GameObject>();

        private Vector2 mClickScreenPos = Vector2.zero;

        public void SetHideFun(Action hideFun)
        {
            mHideFun = hideFun;
        }

        private void Update()
        {
            if (GetClickPos(ref mClickScreenPos))
            {
                if (UIObjContainPos(this.gameObject, mClickScreenPos))
                {
                    return;
                }

                foreach (var obj in mExtentObj)
                {
                    if (UIObjContainPos(obj, mClickScreenPos))
                    {
                        return;
                    }
                }

                HidePopPanel();
            }
        }

        private void HidePopPanel()
        {
            if (mHideFun != null)
            {
                mHideFun.Invoke();
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        private bool GetClickPos(ref Vector2 screenPos)
        {
            screenPos = Vector2.zero;
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                //移动端
                if (Application.platform == RuntimePlatform.Android ||
                            Application.platform == RuntimePlatform.IPhonePlayer)
                {

                    screenPos = Input.GetTouch(0).position;//Input.mousePosition;
                }
                //其它平台
                else
                {
                    screenPos = Input.mousePosition;
                }

                return true;
            }

            return false;
        }

        public bool UIObjContainPos(GameObject obj, Vector2 screenPos)
        {
            if (obj == null)
            {
                return false;
            }

            RectTransform rectTra = obj.GetComponent<RectTransform>();
            if (rectTra != null)
            {
                if (UMI.UIMgr.Instance.UICamera != null)
                {
                    return RectTransformUtility.RectangleContainsScreenPoint(rectTra, screenPos, UMI.UIMgr.Instance.UICamera);
                }
                return RectTransformUtility.RectangleContainsScreenPoint(rectTra, screenPos);
            }

            return false;

        }

    }
}
