using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace UMI
{
    /// <summary>
    /// UI拖拽组件
    /// </summary>
    public class UIDragObj : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void DragObjHandle(Vector2 screenPoint);
        public DragObjHandle onPointDownFun;
        public DragObjHandle onPointUpFun;

        public DragObjHandle onBeginDragFun;
        public DragObjHandle onDragFun;
        public DragObjHandle onEndDragFun;

        public DragObjHandle onPointEnterFun;
        public DragObjHandle onPointExitFun;

        public Action onClick;

        private bool mBDraging = false;
        private bool mBPointEnter = false;
        private bool mBPointDown = false;


        public void OnPointerDown(PointerEventData eventData)
        {
            mBPointDown = true;
            onPointDownFun?.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointUpFun?.Invoke(eventData.position);
            if (!mBPointDown)
            {
                return;
            }

            mBPointDown = false;

            if (!mBDraging)
            {
                onClick?.Invoke();
            }

            // if (!mBDraging || (mBPointEnter && onDragFun == null))
            // {
            //     onClick?.Invoke();
            // }

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            mBDraging = true;
            onBeginDragFun?.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDragFun?.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mBDraging = false;
            onEndDragFun?.Invoke(eventData.position);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            mBPointEnter = true;
            onPointEnterFun?.Invoke(eventData.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mBPointEnter = false;
            onPointExitFun?.Invoke(eventData.position);
        }

    }
}
