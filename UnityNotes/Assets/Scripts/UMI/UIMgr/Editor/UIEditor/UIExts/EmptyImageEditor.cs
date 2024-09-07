using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(EmptyImage))]
    public class EmptyImageEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            EmptyImage mTarget = target as EmptyImage;
            mTarget.raycastTarget = EditorGUILayout.Toggle("raycastTarget", mTarget.raycastTarget);
        }
    }
}
