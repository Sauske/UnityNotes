using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UMI
{
    /// <summary>
    /// 滑动手势，弹出手指后，获取方向
    /// </summary>
    public class UISwipeGeusture : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private RectTransform mRect;
        private Vector2 mBeginPos;

        private Action<Vector2> mOnEnd;

        void Awake()
        {
            mRect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 回调是一个方向向量
        /// </summary>
        /// <param name="onEnd"></param>
        public void Init(Action<Vector2> onEnd)
        {
            mOnEnd = onEnd;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            UIMgr.Instance.ScreenPointToLocalPointInRectangle(mRect, eventData.position, out mBeginPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            //UIMgr.Instance.ScreenPointToLocalPointInRectangle(mRect, eventData.position, out mBeginPos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 toPos;
            UIMgr.Instance.ScreenPointToLocalPointInRectangle(mRect, eventData.position, out toPos);
            mOnEnd?.Invoke(toPos - mBeginPos);
        }
    }
}
