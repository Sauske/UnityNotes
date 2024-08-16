//==================================================================================
/// UI Toggle List 控件
/// @neoyang
/// @2015.03.09
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class CUIToggleListScript : CUIListScript
    {
        //是否为多选
        public bool m_isMultiSelected = false;

        //选项状态
        //[HideInInspector]
        private int m_selected;
        private bool[] m_multiSelected;

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Initialize(CUIFormScript formScript)
        {
            if (m_isInitialized)
            {
                return;
            }

            if (m_isMultiSelected)
            {
                m_multiSelected = new bool[m_elementAmount];
                for (int i = 0; i < m_elementAmount; i++)
                {
                    m_multiSelected[i] = false;
                }
            }
            else
            {
                m_selected = -1;
            }

            base.Initialize(formScript);
        }

        //--------------------------------------------------
        /// 设置元素数量
        /// !!该函数会触发Element OnCreate
        /// @amount
        /// @elementsSize
        //--------------------------------------------------
        public override void SetElementAmount(int amount, List<Vector2> elementsSize)
        {
            //是否需要扩大选项数组
            if (m_isMultiSelected)
            {
                if (m_multiSelected == null || m_multiSelected.Length < amount)
                {
                    bool[] newMultiSelected = new bool[amount];

                    for (int i = 0; i < amount; i++)
                    {
                        if (m_multiSelected != null && i < m_multiSelected.Length)
                        {
                            newMultiSelected[i] = m_multiSelected[i];
                        }
                        else
                        {
                            newMultiSelected[i] = false;
                        }
                    }

                    m_multiSelected = newMultiSelected;
                }
            }

            base.SetElementAmount(amount, elementsSize);
        }

        //--------------------------------------------------
        /// 设置选中元素
        /// @index
        //--------------------------------------------------
        public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            if (m_isMultiSelected)
            {
                bool selected = m_multiSelected[index];
                selected = !selected;

                m_multiSelected[index] = selected;

                CUIListElementScript elementScript = GetElemenet(index);
                if (elementScript != null)
                {
                    elementScript.ChangeDisplay(selected);
                }

                //派发事件
                DispatchElementSelectChangedEvent();
            }
            else
            {
                if (index == m_selected)
                {                  
                    if (m_alwaysDispatchSelectedChangeEvent)
                    {
                        //派发事件
                        DispatchElementSelectChangedEvent();
                    }

                    return;
                }

                if (m_selected >= 0)
                {
                    CUIListElementScript elementScript = GetElemenet(m_selected);

                    if (elementScript != null)
                    {
                        elementScript.ChangeDisplay(false);
                    }
                }

                m_selected = index;

                if (m_selected >= 0)
                {
                    CUIListElementScript elementScript = GetElemenet(m_selected);

                    if (elementScript != null)
                    {
                        elementScript.ChangeDisplay(true);
                    }
                }

                //派发事件
                DispatchElementSelectChangedEvent();
            }
        }

        //--------------------------------------------------
        /// 返回单选选项状态
        //--------------------------------------------------
        public int GetSelected()
        {
            return m_selected;
        }

        //--------------------------------------------------
        /// 返回多选选项状态
        //--------------------------------------------------
        public bool[] GetMultiSelected()
        {
            return m_multiSelected;
        }

        //--------------------------------------------------
        /// 设置单选选项状态
        /// @selected
        /// !!执行此函数不会派发ElementSelectChanged事件
        //--------------------------------------------------
        public void SetSelected(int selected)
        {
            m_selected = selected;

            for (int i = 0; i < m_elementScripts.Count; i++)
            {
                m_elementScripts[i].ChangeDisplay(IsSelectedIndex(m_elementScripts[i].m_index));
            }
        }

        //--------------------------------------------------
        /// 设置多选选项状态
        /// @index
        /// @selected
        /// !!执行此函数不会派发ElementSelectChanged事件
        //--------------------------------------------------
        public void SetMultiSelected(int index, bool selected)
        {
            if (index < 0 || index >= m_elementAmount)
            {
                return;
            }

            m_multiSelected[index] = selected;

            for (int i = 0; i < m_elementScripts.Count; i++)
            {
                m_elementScripts[i].ChangeDisplay(IsSelectedIndex(m_elementScripts[i].m_index));
            }
        }

        //--------------------------------------------------
        /// index项element是否被选中
        /// @override
        //--------------------------------------------------
        public override bool IsSelectedIndex(int index)
        {
            return (m_isMultiSelected ? (m_multiSelected[index]) : (index == m_selected));
        }
    }
};