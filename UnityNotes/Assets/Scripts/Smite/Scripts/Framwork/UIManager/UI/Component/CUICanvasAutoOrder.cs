using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    //自动给form下的嵌套canvas设置sortorder

    public class CUICanvasAutoOrder : CUIComponent
    {
        public bool bAutoOrder = true;
        public int iOrderValue = 0;
        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
            Canvas canvas = GetComponent<Canvas>();
            Canvas formCanvas = formScript.GetComponent<Canvas>();
            if (canvas == null || formCanvas == null) return;

            canvas.worldCamera = formCanvas.worldCamera;
            canvas.renderMode = formCanvas.renderMode;

            if (bAutoOrder)
            {
                canvas.sortingOrder = m_belongedFormScript.GetSortingOrder() + iOrderValue;
            }
            else
            {
                canvas.sortingOrder = iOrderValue;
            }
        }
    }
}
