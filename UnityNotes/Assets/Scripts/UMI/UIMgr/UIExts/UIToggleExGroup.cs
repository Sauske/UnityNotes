using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UMI
{
    [AddComponentMenu("UI/Extensions/UIToggleEx Group", 32)]
    [DisallowMultipleComponent]
    public class UIToggleExGroup : MonoBehaviour
    {
        [SerializeField] private bool m_AllowSwitchOff = false;
        public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }
        public bool autoSorting = true;
        public Vector2 startPos = new Vector2(0, 0);
        public Vector2 spacing = new Vector2(0, -50);
        private List<UIToggleEx> m_Toggles = new List<UIToggleEx>();
        private Dictionary<UIToggleEx, Action> mCallBackDic = new Dictionary<UIToggleEx, Action>();//回调函数字典

        public Dictionary<UIToggleEx, Action> TogCallBackDic{
            get{
                return  mCallBackDic;
            }
        }
        protected UIToggleExGroup()
        { }

        private bool ValidateToggleIsInGroup(UIToggleEx toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
            {
                Debug.LogError(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
                return false;
            }
            return true;
            //throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
        }

        public void NotifyToggleOn(UIToggleEx toggle)
        {
            if (ValidateToggleIsInGroup(toggle) == false)
            {
                return;
            }

            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                    {
                        m_Toggles[i].isOn = true;
                        continue;
                    }
                    

                m_Toggles[i].isOn = false;
            }
        }

        

        public void UnregisterToggle(UIToggleEx toggle)
        {
            if (m_Toggles.Contains(toggle))
                m_Toggles.Remove(toggle);
            
        }

        public void RegisterToggle(UIToggleEx toggle)
        {
            if (!m_Toggles.Contains(toggle))
                m_Toggles.Add(toggle);

            
        }

        /// <summary>
        /// 添加切换标签页回调函数
        /// </summary>
        /// <param name="index">标签页索引</param>
        /// <param name="callBack">回调函数</param>
        public void AddListener(int index, Action callBack){
            int count = m_Toggles.Count;
            if(count <= index)return;
            UIToggleEx togger = m_Toggles[index];
             if(!mCallBackDic.ContainsKey(togger)){
                mCallBackDic.Add(togger, callBack);
            }
        }
        
        public void Clear(){
            mCallBackDic.Clear();
            m_Toggles.Clear();
        }

        public bool AnyTogglesOn()
        {
            return m_Toggles.Find(x => x.isOn) != null;
        }

        public void RefreshTogglePosition()
        {
            if (autoSorting && startPos != null)
            {
                int index = 0;
                for (var i = 0; i < m_Toggles.Count; i++)
                {
                    if (m_Toggles[i].gameObject.activeSelf)
                    {
                        RectTransform rect = m_Toggles[i].GetComponent<RectTransform>();
                        rect.anchoredPosition = startPos + spacing * index;
                        ++index;
                    }
                }
            }
        }
        [ContextMenu("TestUIToggleExGroup")]
        public void TestUIToggleExGroup()
        {
            if (Application.isPlaying && m_Toggles.Count == 1)
            {
                var toggleTpl = m_Toggles[0];
                for (int i = 0; i < 3; i++)
                {
                    GameObject go = (GameObject)Instantiate(toggleTpl.gameObject);
                    go.transform.SetParent(toggleTpl.transform.parent);
                    go.transform.localScale = new Vector3(1, 1, 1);
                }
                RefreshTogglePosition();
            }
        }
    }
}
