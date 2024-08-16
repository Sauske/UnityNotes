//==================================================================================
/// UI组件基类
/// @neoyang
/// @2015.03.05
//==================================================================================

using UnityEngine;
using System.Collections;

namespace Framework
{
    //--------------------------------------------------
    /// UI组件
    //--------------------------------------------------
    public class CUIComponent : MonoBehaviour
    {
        //从属的Form
        [HideInInspector]
        public CUIFormScript m_belongedFormScript;

        //从属的List控件(当作为List元素时有效)
        [HideInInspector]
        public CUIListScript m_belongedListScript;

        //在从属的List控件中的索引(当作为List元素时有效)
        [HideInInspector]
        public int m_indexInlist;

        //自维护UI控件列表
        public GameObject[] m_widgets = new GameObject[0];

        //是否已经初始化完成
        protected bool m_isInitialized = false;

        //--------------------------------------
        /// 初始化
        /// @formScript
        //--------------------------------------
        public virtual void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            m_belongedFormScript = formScript;

            //将this添加到从属form的UI组件列表中去
            if (m_belongedFormScript != null)
            {
                m_belongedFormScript.AddUIComponent(this);

                //初始化sortingOrder
                SetSortingOrder(m_belongedFormScript.GetSortingOrder());
            }
            
            m_isInitialized = true;
        }

        //--------------------------------------
        /// OnDestroy
        //--------------------------------------
        protected virtual void OnDestroy()
        {
            m_belongedFormScript = null;
            m_belongedListScript = null;
            m_widgets = null;
        }

        //--------------------------------------
        /// 关闭
        //--------------------------------------
        public virtual void Close()
        {
            
        }

        //--------------------------------------
        /// 隐藏
        //--------------------------------------
        public virtual void Hide()
        {

        }

        //--------------------------------------
        /// 重新显示
        //--------------------------------------
        public virtual void Appear()
        {

        }

        //--------------------------------------
        /// 设置SortingOrder
        /// @sortingOrder
        //--------------------------------------
        public virtual void SetSortingOrder(int sortingOrder)
        {

        }

        //--------------------------------------
        /// 设置所属List控件脚本及索引
        /// @belongedListScript
        /// @index
        //--------------------------------------
        public void SetBelongedList(CUIListScript belongedListScript, int index)
        {
            m_belongedListScript = belongedListScript;
            m_indexInlist = index;
        }

        //--------------------------------------------------
        /// 返回子控件
        /// @index
        //--------------------------------------------------
        public GameObject GetWidget(int index)
        {
            if (index < 0 || index >= m_widgets.Length)
            {
                return null;
            }

            return m_widgets[index];
        }

        //--------------------------------------
        /// 遍历获取UI组件
        /// @go在非active状态下依然有效
        //--------------------------------------
        protected T GetComponentInChildren<T>(GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();

            if (t != null)
            {
                return t;
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                t = GetComponentInChildren<T>(go.transform.GetChild(i).gameObject);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

        //--------------------------------------
        /// 复制GameObject
        /// @gameObject
        //--------------------------------------
        protected GameObject Instantiate(GameObject srcGameObject)
        {
            GameObject dstGameObject = GameObject.Instantiate(srcGameObject) as GameObject;

            dstGameObject.transform.SetParent(srcGameObject.transform.parent);

            RectTransform srcRectTransform = srcGameObject.transform as RectTransform;
            RectTransform dstRectTransform = dstGameObject.transform as RectTransform;

            if (srcRectTransform != null && dstRectTransform != null)
            {
                dstRectTransform.pivot = srcRectTransform.pivot;
                dstRectTransform.anchorMin = srcRectTransform.anchorMin;
                dstRectTransform.anchorMax = srcRectTransform.anchorMax;
                dstRectTransform.offsetMin = srcRectTransform.offsetMin;
                dstRectTransform.offsetMax = srcRectTransform.offsetMax;

                dstRectTransform.localPosition = srcRectTransform.localPosition;
                dstRectTransform.localRotation = srcRectTransform.localRotation;
                dstRectTransform.localScale = srcRectTransform.localScale;
            }

            return dstGameObject;
        }

        //--------------------------------------
        /// 派发UI事件
        //--------------------------------------
        protected void DispatchUIEvent(CUIEvent uiEvent)
        {
            if (CUIEventManager.GetInstance() != null)
            {
                CUIEventManager.GetInstance().DispatchUIEvent(uiEvent);
            }
        }

        //--------------------------------------
        /// 遍历初始化UI组件
        /// @root
        //--------------------------------------
        protected void InitializeComponent(GameObject root)
        {
            CUIComponent[] uiComponents = root.GetComponents<CUIComponent>();

            if (uiComponents != null && uiComponents.Length > 0)
            {
                for (int i = 0; i < uiComponents.Length; i++)
                {
                    uiComponents[i].Initialize(m_belongedFormScript);
                }
            }

            for (int i = 0; i < root.transform.childCount; i++)
            {
                InitializeComponent(root.transform.GetChild(i).gameObject);
            }
        }
    };
};