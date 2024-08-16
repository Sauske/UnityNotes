using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class UIBasePage : MonoBehaviour
    {
        public delegate UIBasePage PageCreator();

      //  private static int s_sequence = 0;

        #region Fields
        public int PageId { get; set; }


        public string AssetPath { get; set; }

        public bool IsTop { get; set; }

        public bool IsClosed { get; private set; }

        public bool IsOpened { get; private set; }

        public bool IsShowed { get; private set; }

        public bool IsHide { get; private set; }


        protected object mParam = null;
        
        public object Param
        { get { return mParam; } }

        protected CUIFormScript mForm = null;

        Dictionary<string, CUIFormScript> mAttachedForm = new Dictionary<string, CUIFormScript>();

        public CUIFormScript Form
        { get { return mForm; } }


     //   private enFormPriority mLastTopBarPriority = enFormPriority.Priority0;
        #endregion

        /// <summary>
        /// 挂在一个Form到界面上
        /// </summary>
        /// <param name="formPath"></param>
        /// <returns></returns>
        public CUIFormScript AttachForm(string formPath)
        {
            CUIFormScript form;
            if (mAttachedForm.TryGetValue(formPath, out form))
                return form;

            form = CUIManager.GetInstance().OpenForm(formPath, false);
            if (form == null) return null;

            mAttachedForm.Add(formPath, form);

            return form;
        }

        public CUIFormScript GetAttachedForm(string formPath)
        {
            CUIFormScript form;
            if (mAttachedForm.TryGetValue(formPath, out form))
                return form;

            return form;
        }

        public void DetachForm(string formPath)
        {
            CUIFormScript form;
            if (mAttachedForm.TryGetValue(formPath, out form))
            {
                CUIManager.GetInstance().CloseForm(form);
                mAttachedForm.Remove(formPath);
            }
        }

        public void ShowForm(string formPath)
        {
            if (mForm.m_formPath == formPath)
            {
                mForm.SetActive(true);
                return;
            }

            CUIFormScript form = GetAttachedForm(formPath);
            if (form != null)
                form.SetActive(true);
        }

        public void HideForm(string formPath)
        {
            if(mForm.m_formPath == formPath)
            {
                mForm.SetActive(false);
                return;
            }

            CUIFormScript form = GetAttachedForm(formPath);
            if (form != null)
                form.SetActive(false);
        }

        public bool Open(object param)
        {
            mParam = param;

            if(mForm == null)
            {
                PreInitialize();

                Initialize();

                PostInitialize();
            }

            if (mForm == null) return false;

            //! Move To Top Level
            mForm.transform.SetAsLastSibling();

            FillContent();

            IsOpened = true;
            IsShowed = true;
            IsHide = false;
            IsClosed = false;

            //mLastTopBarPriority = Assets.Scripts.GameSystem.CLobbySystem.GetInstance().GetTopBarPriority();
            //if (mForm != null && mForm.m_ShowTopBar)
            //{
            //    Assets.Scripts.GameSystem.CLobbySystem.GetInstance().SetTopBarPriority(mLastTopBarPriority + 1);
            //}
            //else
            //{

            //    Assets.Scripts.GameSystem.CLobbySystem.GetInstance().SetTopBarPriority(enFormPriority.Priority0);
            //}
            return true;
        }

        public bool Show()
        {
            if (mForm == null) return false;

            if (IsOpened && !IsClosed && IsHide && !IsShowed)
            {
                IsShowed = true;
                IsHide = false;

                mForm.SetActive(true);

                foreach (var pair in mAttachedForm)
                {
                    pair.Value.SetActive(true);
                }

                return true;
            }

            return false;
        }

        public bool Hide()
        {
            if (mForm == null) return false;

            if(IsOpened && !IsClosed && IsShowed && !IsHide)
            {
                IsHide = true;
                IsShowed = false;

                mForm.SetActive(false);

                foreach(var pair in mAttachedForm)
                {
                    pair.Value.SetActive(false);
                }

                return true;
            }
            return false;
        }

        protected virtual void PreInitialize()
        {

        }

        protected virtual void PostInitialize()
        {
            RegisterEvents();
        }

        protected virtual void FillContent()
        {

        }

        /// <summary>
        /// 此接口外部不要调用，这个接口是UIPageManager直接管理的
        /// </summary>
        public virtual void Close()
        {
          //  Assets.Scripts.GameSystem.CLobbySystem.GetInstance().SetTopBarPriority(mLastTopBarPriority);

            mParam = null;
            IsOpened = false;
            IsClosed = true;
            IsShowed = false;
            IsHide = false;

            UnRegisterEvents();

            foreach(var v in mAttachedForm)
            {
                CUIManager.GetInstance().CloseForm(v.Key);
            }
            mAttachedForm.Clear();


            if (mForm != null)
            {
                CUIManager.GetInstance().CloseForm(mForm);
                mForm = null;
            }

            AssetPath = null;
        }


        protected virtual void RegisterEvents()
        {

        }

        protected virtual void UnRegisterEvents()
        {

        }

        private bool Initialize()
        {
            if (string.IsNullOrEmpty(AssetPath)) return false;

            mForm = CUIManager.GetInstance().OpenForm(AssetPath, false);
            mForm.BelongPage = this;

            return mForm!=null;
        }
    }
}
