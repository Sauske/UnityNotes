using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIGraphBaseScript))]
    public class CUIGraphEditor : Editor
    {
     //   private CUIGraphBaseScript m_line; 
        protected virtual void OnEnable()
        {
          //  m_line = target as CUIGraphBaseScript;
            EditorApplication.update -= UpdateObj;
            EditorApplication.update += UpdateObj;
        }

        void OnDisable()
        {
            //EditorApplication.update -= Update3DGameObjects;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        //--------------------------------------------------
        /// 查找FormScript
        /// @transform
        //--------------------------------------------------
        private static CUIFormScript GetFormScript(Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            CUIFormScript formScript = transform.gameObject.GetComponent<CUIFormScript>();
            if (formScript != null)
            {
                return formScript;
            }

            return GetFormScript(transform.parent);
        }

        //--------------------------------------------------
        /// 画线
        //--------------------------------------------------
        public void UpdateObj()
        {
        }
    };
};