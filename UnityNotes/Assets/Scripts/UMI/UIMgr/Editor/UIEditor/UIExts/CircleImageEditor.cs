using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(CircleImage))]
    public class CircleImageEditor : ImageEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CircleImage mTarget = target as CircleImage;
            bool bCircle = EditorGUILayout.Toggle("Circle", mTarget.bCircle);
            if (mTarget.bCircle != bCircle)
            {
                mTarget.bCircle = bCircle;
                mTarget.OnRebuildRequested();
            }
        }
    }
}


