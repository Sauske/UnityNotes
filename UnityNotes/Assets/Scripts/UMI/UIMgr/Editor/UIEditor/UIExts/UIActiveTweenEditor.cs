using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using UMI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UIActiveTween))]
    public class UIActiveTweenEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            UIActiveTween mTarget = target as UIActiveTween;

            mTarget.mTweenType = (UIActiveTween.ETweenType)EditorGUILayout.EnumPopup("移动类型：", mTarget.mTweenType);//枚举类型下拉框
            mTarget.mTweenTime = EditorGUILayout.FloatField("移动时间", mTarget.mTweenTime);
            if (mTarget.mTweenType == UIActiveTween.ETweenType.Move)
            {
                mTarget.mOutPos = EditorGUILayout.Vector3Field("移出位置", mTarget.mOutPos);
                mTarget.mInPos = EditorGUILayout.Vector3Field("移入位置", mTarget.mInPos);

            }
            else if (mTarget.mTweenType == UIActiveTween.ETweenType.Scale)
            {
                mTarget.mOutScale = EditorGUILayout.Vector3Field("移出缩放", mTarget.mOutScale);
                mTarget.mInScale = EditorGUILayout.Vector3Field("移入缩放", mTarget.mInScale);

            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button("移出"))
                {
                    mTarget.MoveOut();
                }
                if (GUILayout.Button("移入"))
                {
                    mTarget.MoveIn();
                }
            }
        }
    }
}
