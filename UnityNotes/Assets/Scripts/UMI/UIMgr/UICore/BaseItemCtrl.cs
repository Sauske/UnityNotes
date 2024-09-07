using UnityEngine;
using UnityEngine.UI;

namespace UMI
{
    public abstract class BaseItemCtrl : MonoEventListener
    {
        protected bool mInitedView = false;
        protected abstract void CreateView();


        protected virtual void OnEnable()
        {
            CreateView();
            OnShow();
        }

        protected override void OnDisable()
        {
            OnHide();
            base.OnDisable();
        }

        protected virtual void AddBtnListeners()
        {

        }

        protected virtual void OnShow()
        {

        }

        protected virtual void OnHide()
        {

        }

        /// <summary>
        /// 克隆界面下的 Item 
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual T CloneItem<T>(T obj) where T : BaseItemCtrl
        {
            if (obj == null)
            {
                return null;
            }

            return null;// ResMgr.Instance.InstantiateObj<T>(obj, obj.transform.parent);
        }
    }
}