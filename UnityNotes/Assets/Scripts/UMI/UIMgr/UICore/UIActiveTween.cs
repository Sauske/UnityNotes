using System;

using DG.Tweening;

using UMI;

namespace UnityEngine.UI
{
    /// <summary>
    /// UI显示动画
    /// </summary>
    public class UIActiveTween : MonoBehaviour
    {
        public enum ETweenType
        {
            None,
            Move,
            Scale,
        }

        public ETweenType mTweenType = ETweenType.Move;
        public float mTweenTime = 0.2f;
        public Vector3 mOutPos;
        public Vector3 mInPos = Vector3.zero;

        public Vector3 mOutScale = Vector3.zero;
        public Vector3 mInScale = Vector3.one;

        protected Action mTweenInOver;
        protected Action mTweenOutOver;
        protected Tween mAniTween;

        public virtual void SetTweenOver(Action tweenInOverCall, Action tweenOutOverCall)
        {
            mTweenInOver = tweenInOverCall;
            mTweenOutOver = tweenOutOverCall;

            if (mTweenType == ETweenType.Move)
            {
                this.transform.SetGoLocalPos(mOutPos);
            }
            else if (mTweenType == ETweenType.Scale)
            {
                this.transform.SetGoLocalScale(mOutScale);
            }
        }

        public virtual void MoveIn()
        {
            if (mAniTween != null)
            {
                mAniTween.Kill();
                mAniTween = null;
            }

            if (mTweenType == ETweenType.Move)
            {
                // mAniTween = this.transform.DoLocalMovePos(mOutPos, mInPos, mTweenTime, OnTweenInOver);
            }
            else if (mTweenType == ETweenType.Scale)
            {
                // mAniTween = this.transform.DoLocalScale(mOutScale, mInScale, mTweenTime, OnTweenInOver);
            }
            else
            {
                OnTweenInOver();
            }
        }

        public virtual void MoveOut()
        {
            if (mAniTween != null)
            {
                mAniTween.Kill();
                mAniTween = null;
            }

            if (mTweenType == ETweenType.Move)
            {
                //  mAniTween = this.transform.DoLocalMovePos(mOutPos, mTweenTime, OnTweenOutOver);
            }
            else if (mTweenType == ETweenType.Scale)
            {
                // mAniTween = this.transform.DoLocalScale(mOutScale, mTweenTime, OnTweenOutOver);
            }
            else
            {
                OnTweenOutOver();
            }
        }

        protected virtual void OnTweenInOver()
        {
            mTweenInOver?.Invoke();
        }

        protected virtual void OnTweenOutOver()
        {
            mTweenOutOver?.Invoke();
        }
    }
}
