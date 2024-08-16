using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Qarth
{
    public enum AnchorType
    {
        /// <summary>
        /// 右上角
        /// </summary>
        TopRight,
        /// <summary>
        /// 左上角
        /// </summary>
        TopLeft,
        /// <summary>
        /// 四周对齐
        /// </summary>
        Stretch,
        /// <summary>
        /// 顶部对齐
        /// </summary>
        StretchTop
    }
    
    public static class RectranformEX
    {
        private static Vector2 pivotTopRight = new Vector2(1, 1);
        private static Vector2 pivotTop = new Vector2(0.5f, 1);
        private static Vector2 pivotCenter = new Vector2(0.5f, 0.5f);
        private static Vector2 AnchorPos = new Vector2(0, 0);

        public static void SetAnchor(this RectTransform rect, AnchorType anchor)
        {
            if (rect == null)
                return;
            var size = rect.sizeDelta;
            //left,right对应x,top,bottom对应Y
            switch (anchor)
            {
                case AnchorType.TopRight:
                    rect.pivot = pivotCenter;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, size.x);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, size.y);
                    rect.anchoredPosition = new Vector2(-size.x * .5f, -size.y * .5f);
                    break;
                case AnchorType.TopLeft:
                    rect.pivot = pivotCenter;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, size.x);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, size.y);
                    rect.anchoredPosition = new Vector2(size.x * .5f, -size.y * .5f);
                    break;
                case AnchorType.Stretch:
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    break;
                case AnchorType.StretchTop:
                    rect.pivot = pivotTop;
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
                    rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = Vector2.one;
                    break;
                default:
                    Debug.Log("未知的锚点类型");
                    break;
            }
        }
    }
}