//==================================================================================
/// CUIListScript Inspector
/// @neoyang
/// @2015.03.11
//==================================================================================

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(CUIListScript))]
    public class CUIListEditor : Editor
    {
        protected string[] m_eventOptions;
        protected CUIListScript m_listScript;

        protected virtual void OnEnable()
        {
            m_eventOptions = CUIEditorUtility.GetEnumOptions(typeof(enUIEventID));
            m_listScript = target as CUIListScript;
        }

        void OnDisable() 
        {
 
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_listScript.m_useExternalElement = EditorGUILayout.Toggle("Use External Element", m_listScript.m_useExternalElement);

            if (m_listScript.m_useExternalElement)
            {
                GameObject externalElement = (GameObject)CUIEditorUtility.GetAssetInResources(m_listScript.m_externalElementPrefabPath, typeof(GameObject));
                GameObject newExternalElement = (GameObject)EditorGUILayout.ObjectField("External Element Prefab:", externalElement, typeof(GameObject), true);

                if (newExternalElement != externalElement)
                {
                    externalElement = newExternalElement;
                    m_listScript.m_externalElementPrefabPath = CUIEditorUtility.GetAssetPathInResources(externalElement, ".prefab");
                }
            }

            m_listScript.m_autoAdjustScrollAreaSize = EditorGUILayout.Toggle("Auto Adjust Scroll Area Size", m_listScript.m_autoAdjustScrollAreaSize);

            if (m_listScript.m_autoAdjustScrollAreaSize)
            {
                m_listScript.m_scrollRectAreaMaxSize = EditorGUILayout.Vector2Field("Max List Size:", m_listScript.m_scrollRectAreaMaxSize);
            }

            CUIEditorUtility.EditorEventID("On Select Changed", ref m_listScript.m_listSelectChangedEventID, m_eventOptions);
            CUIEditorUtility.EditorEventID("On Scroll Changed", ref m_listScript.m_listScrollChangedEventID, m_eventOptions);

            if (!m_listScript.m_autoCenteredElements)
            {
                m_listScript.m_autoCenteredBothSides = false;
            }
        }
    };
};
