using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class UIBasePart : MonoBehaviour
    {
        public string FormPath { get; set; }
        public UIBasePage UIPage { get; set; }

        public CUIFormScript UIForm { get; private set; }

        public virtual void Open()
        {
            if (UIPage == null || FormPath == null)
            {
                return;
            }

            UIForm = UIPage.AttachForm(FormPath);
            RegisterEvents();
        }

        public virtual void Show()
        {
            if (UIPage == null || FormPath == null)
            {
                return ;
            }

            UIPage.ShowForm(FormPath);
        }

        public virtual void Hide()
        {
            if (UIPage == null || FormPath == null)
            {
                return ;
            }

            UIPage.HideForm(FormPath);
        }

        public virtual void Close()
        {
            if (UIPage == null || FormPath == null)
            {
                return;
            }

            UnRegisterEvents();
            UIPage.DetachForm(FormPath);

            GameObject.DestroyImmediate(this);
        }

        protected virtual void RegisterEvents()
        {

        }

        protected virtual void UnRegisterEvents()
        {

        }
    }
}
