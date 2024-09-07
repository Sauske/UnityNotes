using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UMI
{
    /// <summary>
    /// 一次只能移动一页，且内容Item大小与视图大小等宽
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    class UIAutoAdsorptionView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [Header("是否自动翻页")]
        public bool bAutoScroll = false;
        [Header("自动翻页页面停留时间")]
        public float pageStayTime = 1f;
        [Header("翻页时页面滚动时间")]
        public float pageScrollTime = 0.5f;

        private bool mIsDrag = false;
        private Vector2 mBeginDragPos;
        private RectTransform mRect;

        //总页数
        [SerializeField]
        private int mTotalPage = 0;
        private int mCurPageIndex = 0;
        private Type mPageViewType;
        private Action<Transform> mOnInitItemDataCb;
        private Action<int> mOnPageChangedCb;

        private RectTransform mCurPage;
        private RectTransform mNextPage;
        private Vector3 mNextPageBeginPos;

        private void Awake()
        {
            mRect = GetComponent<RectTransform>();

            int childCount = transform.childCount;
            if (childCount < 2)
            {
                Debug.LogErrorFormat("{0} Must Need 2 child", this.GetType().Name);
                return;
            }

            mCurPage = transform.GetChild(0).GetComponent<RectTransform>();
            mNextPage = transform.GetChild(1).GetComponent<RectTransform>();

            Init();//初始化
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //mCurPageIndex = 0;
            ResetCurPagePos();
        }

        private void ResetCurPagePos()
        {
            mCurPage.localPosition = Vector3.zero;
            mNextPage.localPosition = mCurPage.localPosition + new Vector3(mCurPage.sizeDelta.x, 0);
        }

        public void InitView(int totalPage, Action<Transform> OnInitItemData)
        {
            mTotalPage = totalPage;
            mOnInitItemDataCb = OnInitItemData;
        }


        /// <summary>
        /// 拖拽开始
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            mIsDrag = true;
            UIMgr.Instance.ScreenPointToLocalPointInRectangle(mRect, eventData.position, out mBeginDragPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!mIsDrag)
            {
                return;
            }

            Vector2 curPos;
            UIMgr.Instance.ScreenPointToLocalPointInRectangle(mRect, eventData.position, out curPos);
            float moveX = (curPos - mBeginDragPos).x;

            //往右移动
            if (moveX >= 0)
            {
                //初始化位置
                if (mNextPage.localPosition.x >= mCurPage.localPosition.x + mCurPage.sizeDelta.x / 2)
                {
                    mNextPage.localPosition = mCurPage.localPosition - new Vector3(mCurPage.sizeDelta.x, 0);
                }
                float toX = moveX * 3 + mNextPage.localPosition.x;
                toX = Mathf.Clamp(toX, mCurPage.localPosition.x - mCurPage.sizeDelta.x, mCurPage.localPosition.x);

                //在右边
                mNextPage.localPosition = new Vector3(toX, mNextPage.localPosition.y);
            }
            else
            {
                //初始化位置
                if (mNextPage.localPosition.x <= mCurPage.localPosition.x - mCurPage.sizeDelta.x / 2)
                {
                    mNextPage.localPosition = mCurPage.localPosition + new Vector3(mCurPage.sizeDelta.x, 0);
                }
                float toX = moveX * 3 + mNextPage.localPosition.x;
                toX = Mathf.Clamp(toX, mCurPage.localPosition.x, mCurPage.localPosition.x + mCurPage.sizeDelta.x);

                //在右边
                mNextPage.localPosition = new Vector3(toX, mNextPage.localPosition.y);
            }
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            mIsDrag = false;
            var endPos = eventData.position;
        }


        private void MoveToNearPage()
        {

        }

        private void MoveToPage(int index)
        {
            if (mTotalPage <= index || index < 0)
            {
                return;
            }

        }


        /// <summary>
        /// 当前选中页发生变化
        /// </summary>
        /// <param name="page"></param>
        private void OnPageChanged(int page)
        {
            mOnPageChangedCb?.Invoke(page);
        }

    }
}
