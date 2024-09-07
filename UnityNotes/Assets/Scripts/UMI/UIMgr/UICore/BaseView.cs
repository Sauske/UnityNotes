using UnityEngine;

namespace UMI
{
    public class BaseView
    {
        protected GameObject mViewObj;
        protected RectTransform mViewRect;

        public virtual GameObject ViewObj { get => mViewObj; }

        public virtual RectTransform ViewRect
        {
            get
            {
                if (mViewRect == null)
                {
                    mViewRect = mViewObj.GetComponent<RectTransform>();
                }
                return mViewRect;
            }
        }

        public BaseView(GameObject viewObj)
        {
            //mViewObj = viewObj;
            InitViewObj(viewObj);
        }

        public virtual void InitViewObj(GameObject viewObj)
        {
            mViewObj = viewObj;
        }

        public virtual void SetGoActive(bool bActive)
        {
            ViewObj.SetGoActive(bActive);
        }
    }
}
