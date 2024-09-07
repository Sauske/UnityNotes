using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UMI
{
    /// <summary>
    /// 长按背景，进入可编辑状态
    /// </summary>
    public class ButtonLongPress : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Tooltip("按下多久后触发长按")]
        public float duration = 0.2f;
        [Tooltip("长按事件触发间隔，0只触发一次")]
        public float repeatStep = 0;
        [Tooltip("只触发下按事件")]
        public bool isSinglePress = false;
        public delegate void listenerDelegate(GameObject go, BaseEventData data);

        public listenerDelegate onClick;
        public listenerDelegate onLongClick;


        private bool isDown = false;
        private bool isLongPress = false;
        private bool isLongPressTriggered = false;
        private float touchTime;
        private float repeatTime;

        private void Update()
        {
            if (isDown)
            {
                touchTime += Time.deltaTime;
                if (touchTime > duration)
                {
                    isLongPress = true;
                    if (!isLongPressTriggered)
                    {
                        isLongPressTriggered = true;
                        LongPress(null);
                    }
                    if (repeatStep > 0 && repeatTime > repeatStep)
                    {
                        repeatTime = repeatTime - repeatStep;
                        //长按
                        LongPress(null);
                    }
                    repeatTime += Time.deltaTime;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData) //按下执行
        {
            isLongPressTriggered = false;
            repeatTime = 0;
            touchTime = 0;
            isDown = true;
            if (isSinglePress)
            {
                OnPress(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData) //抬起执行
        {
            isDown = false;
        }

        public void OnPointerExit(PointerEventData eventData) //离开执行
        {
            isDown = false;
        }

        protected override void OnDisable()
        {
            isDown = false;
        }

        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
                isDown = false;
        }

        //长按方法执行了，就不执行点击，，，反之亦然...
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLongPress)
            {
                isLongPress = false;
                return;
            }
            OnClick(eventData);
        }

        /// <summary>
        /// 长按 
        /// </summary>
        private void LongPress(PointerEventData eventData)  //  todo...方法中实现，要做的事情
        {
            if (onLongClick != null)
                onLongClick(gameObject, eventData);

        }


        /// <summary>
        /// 点击
        /// </summary>
        private void OnClick(PointerEventData eventData)
        {
            if (onClick != null)
                onClick(gameObject, eventData);
        }

        private void OnPress(PointerEventData eventData)
        {

        }

        public void Release()
        {
            onClick = null;
            onLongClick = null;

        }

        protected override void OnDestroy()
        {
            Release();
            base.OnDestroy();
        }
    }
}
