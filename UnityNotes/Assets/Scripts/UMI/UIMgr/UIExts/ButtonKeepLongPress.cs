using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace UMI
{
    /// <summary>
    /// 持续长按，长按时移出和移入按钮。
    /// 按下-->长按开始-->(移出|移出)-->长按结束
    /// </summary>
    public class ButtonKeepLongPress : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler, IPointerEnterHandler
    {
        [Tooltip("按下多久后触发长按")]
        public float duration = 0.2f;

        public Action onLongPressBegin;
        public Action onLongPressEnd;
        public Action onLongPressCancel;
        public Action onLongPressExit;
        public Action onLongPressEnter;


        private bool isDown = false;
        private bool isLongPress = false;
        private bool isMoveOutside = false;
        private float touchBeginTime;

        private void Update()
        {
            if (!isDown )
            {
                return;
            }

            if (isLongPress)
            {
                return;
            }

            if (Time.time - touchBeginTime > duration)
            {
                isLongPress = true;
                onLongPressBegin?.Invoke();
            }
            
        }

        public void OnPointerDown(PointerEventData eventData) //按下执行
        {
            isLongPress = false;
            isMoveOutside = false;
            touchBeginTime = Time.time;
            isDown = true;
        }

        public void OnPointerUp(PointerEventData eventData) //抬起执行
        {
            isDown = false;
            if (isLongPress)
            {
                if (isMoveOutside)
                {
                    onLongPressCancel?.Invoke();
                }
                else
                {
                    onLongPressEnd?.Invoke();
                }
            }

            isLongPress = false;
            isMoveOutside = false;
        }

        public void OnPointerExit(PointerEventData eventData) //离开执行
        {
            isMoveOutside = true;
            if (isLongPress)
            {
                onLongPressExit?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMoveOutside = false;
            if (isLongPress)
            {
                onLongPressEnter?.Invoke();
            }
        }

        protected override void OnDisable()
        {
            isDown = false;
        }

        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                isDown = false;
                isLongPress = false;
                //onLongPressCancel?.Invoke();
            }
        }

        //长按方法执行了，就不执行点击，，，反之亦然...
        public void OnPointerClick(PointerEventData eventData)
        {
           
        }

        public void Release()
        {
            onLongPressBegin = null;
            onLongPressCancel = null;
            onLongPressEnd = null;
            onLongPressEnter = null;
            onLongPressExit = null;
        }

        protected override void OnDestroy()
        {
            Release();
            base.OnDestroy();
        }

        public bool Cancel()
        {
            isDown = false;
            isLongPress = false;
            bool isOut = isMoveOutside;
            isMoveOutside = false;
            touchBeginTime = 0f;
            return isOut;
        }
    }
}
