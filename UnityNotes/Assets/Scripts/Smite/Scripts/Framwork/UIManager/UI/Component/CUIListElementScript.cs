//==================================================================================
/// UI List 元素控件
/// @neoyang
/// @2015.03.05
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Framework
{
    //自定义rect
    public struct stRect
    {
        //宽高
        public int m_width;
        public int m_height;

        //坐标
        public int m_top;
        public int m_bottom;
        public int m_left;
        public int m_right;
        public Vector2 m_center;
    };

    //枢轴点位置类型
    public enum enPivotType
    {
        Centre = 0,
        LeftTop = 1,
    };

    public class CUIListElementScript : CUIComponent
    {
        //被选中时候的前景框
        public GameObject m_selectFrontObj;

        //被选中时的背景Sprite
        public Sprite m_selectedSprite;

        //原始背景Sprite
        [HideInInspector]
        public Sprite m_defaultSprite;

        //原始背景的color值
        [HideInInspector]
        public Color m_defaultColor;

        //原始的文本color值
        [HideInInspector]
        public Color m_defaultTextColor;

        //文本对象
        public Text m_textObj;

        //选中后的文本color值
        public Color m_selectTextColor = new Color(1, 1, 1, 1);

        [HideInInspector]
        public ImageAlphaTexLayout m_defaultLayout;
        public ImageAlphaTexLayout m_selectedLayout;

        //默认尺寸
        [HideInInspector]
        public Vector2 m_defaultSize;

        //索引
        [HideInInspector]
        public int m_index;

        //索引
        [HideInInspector]
        public enPivotType m_pivotType = enPivotType.LeftTop;

        //用于显示的Image
        private Image m_image;

        //在Content上面的区域
        public stRect m_rect;

        //使用SetActive()还是CanvasGroup来显示或隐藏list element
        public bool m_useSetActiveForDisplay = true;

        //是否自动加上UIEventScript
        public bool m_autoAddUIEventScript = true;

        //事件
        [HideInInspector]
        public enUIEventID m_onEnableEventID;
        public stUIEventParams m_onEnableEventParams;

        // scotzeng++
        public delegate void OnSelectedDelegate();
        public OnSelectedDelegate onSelected = null;
        // scotzeng--

        //Canvas Group
        private CanvasGroup m_canvasGroup;

        private string m_dataTag;

        //--------------------------------------
        /// 初始化
        //--------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            m_image = gameObject.GetComponent<Image>();
            if (m_image != null)
            {
                m_defaultSprite = m_image.sprite;
                m_defaultColor = m_image.color;

                if (m_image is Image2)
                {
                    Image2 image2 = m_image as Image2;
                    m_defaultLayout = image2.alphaTexLayout;
                }
            }

            //如果Element不包含EventScript，增加一个以便于接收选中事件
            if (m_autoAddUIEventScript)
            {
                CUIEventScript eventScript = gameObject.GetComponent<CUIEventScript>();
                if (eventScript == null)
                {
                    eventScript = gameObject.AddComponent<CUIEventScript>();
                    eventScript.Initialize(formScript);
                }
            }

            //如果Element不包含CanvasGroup，增加一个以便于隐藏/显示
            if (!m_useSetActiveForDisplay)
            {
                m_canvasGroup = gameObject.GetComponent<CanvasGroup>();
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            m_defaultSize = GetDefaultSize();

            //初始化RectTransform
            InitRectTransform();

            if(m_textObj != null)
            {
                m_defaultTextColor = m_textObj.color;
            }
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_selectFrontObj = null;
            m_selectedSprite = null;
            m_defaultSprite = null;
            m_textObj = null;
            m_image = null;
            onSelected = null;
            m_canvasGroup = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// 获取元素默认尺寸
        //--------------------------------------------------
        protected virtual Vector2 GetDefaultSize()
        {
            return (new Vector2(((RectTransform)this.gameObject.transform).rect.width, ((RectTransform)this.gameObject.transform).rect.height));
        }

        public void SetDataTag(string dataTag)
        {
            m_dataTag = dataTag;
        }

        public string GetDataTag()
        {
            return m_dataTag;
        }

        //--------------------------------------------------
        /// Enable元素
        /// @belongedList
        /// @index
        /// @name
        /// @rect
        /// @selected
        //--------------------------------------------------
        public void Enable(CUIListScript belongedList, int index, string name, ref stRect rect, bool selected)
        {
            m_belongedListScript = belongedList;
            m_index = index;

            gameObject.name = name + "_" + index.ToString();

            if (m_useSetActiveForDisplay)
            {
                gameObject.CustomSetActive(true);
            }
            else
            {
                m_canvasGroup.alpha = 1f;
                m_canvasGroup.blocksRaycasts = true;
            }

            //递归设置从属List
            SetComponentBelongedList(gameObject);

            //设置位置属性
            SetRect(ref rect);

            //设置选中/非选中外观
            ChangeDisplay(selected);

            //派发Refresh事件
            DispatchOnEnableEvent();
        }

        //--------------------------------------------------
        /// 禁用元素
        //--------------------------------------------------
        public void Disable()
        {
            if (m_useSetActiveForDisplay)
            {
                gameObject.CustomSetActive(false);
            }
            else
            {
                m_canvasGroup.alpha = 0f;
                m_canvasGroup.blocksRaycasts = false;
            }
        }

        //--------------------------------------------------
        /// 被选中时回调
        /// *此函数会被注册给element及其所有子元素
        //--------------------------------------------------
        public void OnSelected(BaseEventData baseEventData)
        {
            m_belongedListScript.SelectElement(m_index);
        }

        //--------------------------------------------------
        /// 改变显示(选中/非选中)
        //--------------------------------------------------
        public virtual void ChangeDisplay(bool selected)
        {
            //处理背景选择图案
            if (m_image != null && m_selectedSprite != null)
            {
                if (selected)
                {
                    m_image.sprite = m_selectedSprite;
                    m_image.color = new Color(m_defaultColor.r, m_defaultColor.g, m_defaultColor.b, 255.0f);
                }
                else
                {
                    m_image.sprite = m_defaultSprite;
                    m_image.color = m_defaultColor;
                }

                if (m_image is Image2)
                {
                    Image2 image2 = m_image as Image2;
                    image2.alphaTexLayout = selected ? m_selectedLayout : m_defaultLayout;
                }
            }

            if (m_selectFrontObj != null)
            {
                m_selectFrontObj.CustomSetActive(selected);
            }

            if (m_textObj != null)
            {
                m_textObj.color = selected ? m_selectTextColor : m_defaultTextColor;
            }
        }

        //--------------------------------------------------
        /// 遍历并设置从属List
        /// @gameObject
        //--------------------------------------------------
        public void SetComponentBelongedList(GameObject gameObject)
        {
            //为UIComponent设置所属List
            CUIComponent[] componentScripts = gameObject.GetComponents<CUIComponent>();

            if (componentScripts != null && componentScripts.Length > 0)
            {
                for (int i = 0; i < componentScripts.Length; i++)
                {
                    componentScripts[i].SetBelongedList(m_belongedListScript, m_index);
                }                    
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                SetComponentBelongedList(gameObject.transform.GetChild(i).gameObject);
            }
        }

        //--------------------------------------------------
        /// 设置在Content上的位置
        //--------------------------------------------------
        public void SetRect(ref stRect rect)
        {
            m_rect = rect;

            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(m_rect.m_width, m_rect.m_height);

            if (m_pivotType == enPivotType.Centre)
            {
                rectTransform.anchoredPosition = rect.m_center;
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(rect.m_left, rect.m_top);
            }            
        }

        //--------------------------------------------------
        /// 设置OnCreate事件
        //--------------------------------------------------
        public void SetOnEnableEvent(enUIEventID eventID)
        {
            m_onEnableEventID = eventID;
        }

        //--------------------------------------------------
        /// 设置OnCreate事件
        //--------------------------------------------------
        public void SetOnEnableEvent(enUIEventID eventID, stUIEventParams eventParams)
        {
            m_onEnableEventID = eventID;
            m_onEnableEventParams = eventParams;
        }

        //--------------------------------------------------
        /// 初始化Rect
        /// @index
        //--------------------------------------------------
        private void InitRectTransform()
        {
            //设置锚点为Top-Left，枢轴点根据type来设置
            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = (m_pivotType == enPivotType.Centre) ? (new Vector2(0.5f, 0.5f)) : (new Vector2(0, 1));
            rectTransform.sizeDelta = m_defaultSize;

            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

        //--------------------------------------------------
        /// 派发Element OnCreate
        //--------------------------------------------------
        protected void DispatchOnEnableEvent()
        {
            //DebugHelper.ConsoleLog("Dispatch Element On Create Event, index = " + m_index);

            if (m_onEnableEventID != enUIEventID.None)
            {
                CUIEvent uiEvent = CUIEventManager.GetInstance().GetUIEvent();

                uiEvent.m_eventID = m_onEnableEventID;
                uiEvent.m_eventParams = m_onEnableEventParams;
                uiEvent.m_srcFormScript = m_belongedFormScript;
                uiEvent.m_srcWidgetBelongedListScript = m_belongedListScript;
                uiEvent.m_srcWidgetIndexInBelongedList = m_indexInlist;
                uiEvent.m_srcWidget = gameObject;
                uiEvent.m_srcWidgetScript = this;
                uiEvent.m_pointerEventData = null;

                DispatchUIEvent(uiEvent);
            }
        }
    };
};