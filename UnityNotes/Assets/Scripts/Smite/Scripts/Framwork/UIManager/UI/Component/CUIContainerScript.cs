//==================================================================================
/// UI组件容器类
/// @对容器元素的创建/缓存/销毁等进行管理
/// @一个容器类只能管理一种控件
/// @neoyang
/// @2015.03.10
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CUIContainerScript : CUIComponent
    {
        //初始准备元素数量
        public int m_prepareElementAmount = 0;

        //元素模版
        private GameObject m_elementTemplate;
        private string m_elementName;

        //Container最大容纳元素数量
        private const int c_elementMaxAmount = 200;
        private int m_usedElementAmount = 0;

        //使用/非使用容器元素GameObject队列
        private GameObject[] m_usedElements = new GameObject[c_elementMaxAmount];
        private List<GameObject> m_unusedElements = new List<GameObject>();
        
        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            base.Initialize(formScript);

            //获取元素模版
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject element = gameObject.transform.GetChild(i).gameObject;

                if (m_elementTemplate == null)
                {
                    m_elementTemplate = element;
                    m_elementName = element.name;
                    m_elementTemplate.name = m_elementName + "_Template";

                    //保证元素模版为active
                    if (m_elementTemplate.activeSelf)
                    {
                        m_elementTemplate.SetActive(false);
                    }
                }

                element.SetActive(false);
            }

            //准备元素
            if (m_prepareElementAmount > 0)
            {
                for (int i = 0; i < m_prepareElementAmount; i++)
                {
                    //创建Element实例并初始化
                    GameObject element = Instantiate(m_elementTemplate);
                    element.gameObject.name = m_elementName;

                    InitializeComponent(element.gameObject);

                    //隐藏Element并添加到Unused列表
                    if (element.activeSelf)
                    {
                        element.SetActive(false);
                    }

                    if (element.transform.parent != this.gameObject.transform)
                    {
                        element.transform.SetParent(this.gameObject.transform, true);
                        element.transform.localScale = Vector3.one;
                    }

                    m_unusedElements.Add(element);
                }
            }
        }

        //--------------------------------------------------
        /// 销毁
        //--------------------------------------------------
        protected override void OnDestroy()
        {
            m_elementTemplate = null;
            m_usedElements = null;
            m_unusedElements.Clear();
            m_unusedElements = null;

            base.OnDestroy();
        }

        //--------------------------------------------------
        /// 获取容器元素
        /// @返回分配的容器元素的序列号
        //--------------------------------------------------
        public int GetElement()
        {
            if (m_elementTemplate == null || m_usedElementAmount >= c_elementMaxAmount)
            {
                return -1;
            }

            GameObject element = null;

            //优先尝试从unused列表中获取,如果没有则从模版创建副本
            if (m_unusedElements.Count > 0)
            {
                element = m_unusedElements[0];
                m_unusedElements.RemoveAt(0);
            }
            else
            {                
                element = Instantiate(m_elementTemplate);
                element.name = m_elementName;

                InitializeComponent(element.gameObject);
            }

            element.SetActive(true);

            //插入到used数组中去
            for (int i = 0; i < c_elementMaxAmount; i++)
            {
                if (m_usedElements[i] == null)
                {
                    m_usedElements[i] = element;
                    m_usedElementAmount++;

                    return i;
                }
            }

            return -1;
        }

        //--------------------------------------------------
        /// 根据序列号返回容器元素
        /// @sequence
        //--------------------------------------------------
        public GameObject GetElement(int sequence)
        {
            if (sequence < 0 || sequence >= c_elementMaxAmount)
            {
                return null;
            }

            return ((m_usedElements[sequence] == null) ? null : m_usedElements[sequence].gameObject);
        }

        //--------------------------------------------------
        /// 回收容器元素
        /// @sequence
        //--------------------------------------------------
        public void RecycleElement(int sequence)
        {
            if (m_elementTemplate == null || sequence < 0 || sequence >= c_elementMaxAmount)
            {
                return;
            }

            //将element从used列表移除
            GameObject element = m_usedElements[sequence];
            m_usedElements[sequence] = null;

            //隐藏element并添加到unused列表
            if (element != null)
            {
                element.SetActive(false);

                if (element.transform.parent != this.gameObject.transform)
                {
                    element.transform.SetParent(this.gameObject.transform, true);
                    element.transform.localScale = Vector3.one;
                }               

                m_unusedElements.Add(element);

                m_usedElementAmount--;
            }
        }

        //--------------------------------------------------
        /// 回收容器元素
        /// @element
        //--------------------------------------------------
        public void RecycleElement(GameObject elementObject)
        {
            if (m_elementTemplate == null || elementObject == null)
            {
                return;
            }

            GameObject element = elementObject;

            //将element从used列表移除
            for (int i = 0; i < c_elementMaxAmount; i++)
            {
                if (m_usedElements[i] == element)
                {
                    m_usedElements[i] = null;
                    m_usedElementAmount--;
                    break;
                }
            }

            //隐藏element并添加到unused列表
            element.SetActive(false);

            if (element.transform.parent != this.gameObject.transform)
            {
                element.transform.SetParent(this.gameObject.transform, true);
                element.transform.localScale = Vector3.one;
            }

            m_unusedElements.Add(element);
        }

        //--------------------------------------------------
        /// 回收所有元素
        //--------------------------------------------------
        public void RecycleAllElement()
        {
            if (m_elementTemplate == null || m_usedElementAmount <= 0)
            {
                return;
            }

            //将element从used列表移除
            for (int i = 0; i < c_elementMaxAmount; i++)
            {
                if (m_usedElements[i] != null)
                {
                    //隐藏element并添加到unused列表
                    m_usedElements[i].SetActive(false);

                    if (m_usedElements[i].transform.parent != this.gameObject.transform)
                    {
                        m_usedElements[i].transform.SetParent(this.gameObject.transform, true);
                        m_usedElements[i].transform.localScale = Vector3.one;
                    }

                    m_unusedElements.Add(m_usedElements[i]);

                    m_usedElements[i] = null;
                    m_usedElementAmount--;
                }
            }
        }
    };
};