using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UMI
{
    public class UITools
    {
        /// <summary>
        /// 创建UI GameObject
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject child = new GameObject(name);
            if (parent != null)
            {
                child.transform.SetParent(parent.transform, false);
                child.SetGoAndChildrenlayer(parent.layer);
            }

            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            //

            return child;
        }


        /// <summary>
        /// 立即更新界面布局
        /// </summary>
        /// <param name="target"></param>
        /// <param name="horizontal"></param>
        public static void ForceUpdateContentSizeFitter(GameObject target, bool horizontal = true)
        {
            if (target)
            {
                ContentSizeFitter fitter = target.GetComponent<ContentSizeFitter>();
                if (fitter)
                {
                    if (horizontal)
                    {
                        fitter.SetLayoutHorizontal();
                    }
                    else
                    {
                        fitter.SetLayoutVertical();
                    }
                }
                RectTransform trans = target.transform as RectTransform;
                if (trans)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
                }
            }
        }

    }
}
