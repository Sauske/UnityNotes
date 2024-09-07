//using UnityEditor;
//using UnityEngine;

//namespace UMI.Editor
//{
//    [CustomEditor(typeof(UICCAnimationDotweenConfig), true)]
//    public class UAnimationDotweenConfigEditor : UnityEditor.Editor
//    {
//        private UICCAnimationDotweenConfig mTarget;

//        private void OnEnable()
//        {
//            mTarget = target as UICCAnimationDotweenConfig;
//        }

//        public override void OnInspectorGUI()
//        {
//            //关闭原有属性绘制，自定义绘制
//            //base.OnInspectorGUI();

//            UICCAnimationDotweenConfig script = target as UICCAnimationDotweenConfig;

//            script.mIsMove = EditorGUILayout.Toggle("是否需要移动", script.mIsMove);
//            if (script.mIsMove)
//            {
//                script.mIsSetPosBeforeDelay = EditorGUILayout.Toggle("是否在延迟前设置初始位置", script.mIsSetPosBeforeDelay);

//                script.mDelayMove = (float)EditorGUILayout.DoubleField("播放延迟(单位：秒)", script.mDelayMove);
//                script.mDurationMove = (float)EditorGUILayout.DoubleField("移动时间(单位：秒)", script.mDurationMove);

//                EditorGUILayout.BeginHorizontal();
//                string Tooltip = "AnimationCurve 为自定义曲线，其他为 DoTween 曲线";
//                script.mTypeEaseMove = (UICCAnimationDotweenConfig.TypeEase)EditorGUILayout.EnumPopup(new GUIContent("缓动函数曲线", Tooltip), script.mTypeEaseMove);
//                EditorGUILayout.EndHorizontal();

//                EditorGUILayout.BeginHorizontal();
//                if (script.mTypeEaseMove == UICCAnimationDotweenConfig.TypeEase.AnimationCurve)
//                {
//                    script.mAnimationCurveMove = EditorGUILayout.CurveField("自定义缓动函数曲线", script.mAnimationCurveMove);
//                }
//                EditorGUILayout.EndHorizontal();

//                script.mPosOffset = EditorGUILayout.Vector2Field("始末位移差 Anchored Pos", script.mPosOffset);

//                GUILayout.Space(20);
//            }

//            script.mIsScale = EditorGUILayout.Toggle("是否需要缩放", script.mIsScale);
//            if (script.mIsScale)
//            {
//                script.mIsSetScaleBeforeDelay = EditorGUILayout.Toggle("是否在延迟前设置初始大小", script.mIsSetScaleBeforeDelay);

//                script.mDelayScale = (float)EditorGUILayout.FloatField("播放延迟(单位：秒)", script.mDelayScale);
//                script.mDurationScale = (float)EditorGUILayout.FloatField("缩放时间(单位：秒)", script.mDurationScale);

//                EditorGUILayout.BeginHorizontal();
//                string Tooltip = "AnimationCurve 为自定义曲线，其他为 DoTween 曲线";
//                script.mTypeEaseScale = (UICCAnimationDotweenConfig.TypeEase)EditorGUILayout.EnumPopup(new GUIContent("缓动函数曲线", Tooltip), script.mTypeEaseScale);
//                EditorGUILayout.EndHorizontal();

//                EditorGUILayout.BeginHorizontal();
//                if (script.mTypeEaseScale == UICCAnimationDotweenConfig.TypeEase.AnimationCurve)
//                {
//                    script.mAnimationCurveScale = EditorGUILayout.CurveField("自定义缓动函数曲线", script.mAnimationCurveScale);
//                }

//                EditorGUILayout.EndHorizontal();

//                script.mScaleSrc = EditorGUILayout.Vector3Field("初始大小 LocalScale", script.mScaleSrc);
//                script.mScaleTgt = EditorGUILayout.Vector3Field("最终大小 LocalScale", script.mScaleTgt);

//                GUILayout.Space(20);
//            }

//            script.mIsFade = EditorGUILayout.Toggle("是否需要渐变", script.mIsFade);
//            if (script.mIsFade)
//            {
//                script.mIsSetColorBeforeDelay = EditorGUILayout.Toggle("是否在延迟前设置初始颜色", script.mIsSetColorBeforeDelay);

//                script.mDelayFade = (float)EditorGUILayout.DoubleField("播放延迟(单位：秒)", script.mDelayFade);
//                script.mDurationFade = (float)EditorGUILayout.DoubleField("渐变时间(单位：秒)", script.mDurationFade);

//                EditorGUILayout.BeginHorizontal();
//                string Tooltip = "AnimationCurve 为自定义曲线，其他为 DoTween 曲线";
//                script.mTypeEaseFade = (UICCAnimationDotweenConfig.TypeEase)EditorGUILayout.EnumPopup(new GUIContent("缓动函数曲线", Tooltip), script.mTypeEaseFade);
//                EditorGUILayout.EndHorizontal();

//                EditorGUILayout.BeginHorizontal();
//                if (script.mTypeEaseFade == UICCAnimationDotweenConfig.TypeEase.AnimationCurve)
//                {
//                    script.mAnimationCurveFade = EditorGUILayout.CurveField("自定义缓动函数曲线", script.mAnimationCurveFade);
//                }
//                EditorGUILayout.EndHorizontal();

//                script.mColorSrc = EditorGUILayout.ColorField("初始颜色", script.mColorSrc);
//                script.mColorTgt = EditorGUILayout.ColorField("最终颜色", script.mColorTgt);

//                GUILayout.Space(20);
//            }

//            script.mIsRotate = EditorGUILayout.Toggle("是否需要旋转", script.mIsRotate);
//            if (script.mIsRotate)
//            {
//                script.mIsSetRotationBeforeDelay = EditorGUILayout.Toggle("是否在延迟前设置初始角度", script.mIsSetRotationBeforeDelay);

//                script.mDelayRotate = (float)EditorGUILayout.DoubleField("播放延迟(单位：秒)", script.mDelayRotate);
//                script.mDurationRotate = (float)EditorGUILayout.DoubleField("旋转时间(单位：秒)", script.mDurationRotate);

//                EditorGUILayout.BeginHorizontal();
//                string Tooltip = "AnimationCurve 为自定义曲线，其他为 DoTween 曲线";
//                script.mTypeEaseRotate = (UICCAnimationDotweenConfig.TypeEase)EditorGUILayout.EnumPopup(new GUIContent("缓动函数曲线", Tooltip), script.mTypeEaseRotate);
//                EditorGUILayout.EndHorizontal();

//                EditorGUILayout.BeginHorizontal();
//                if (script.mTypeEaseRotate == UICCAnimationDotweenConfig.TypeEase.AnimationCurve)
//                {
//                    script.mAnimationCurveRotate = EditorGUILayout.CurveField("自定义缓动函数曲线", script.mAnimationCurveRotate);
//                }
//                EditorGUILayout.EndHorizontal();

//                script.mEulerOffset = EditorGUILayout.Vector3Field("始末角度差", script.mEulerOffset);
//            }

//            if (GUI.changed)
//            {
//                EditorUtility.SetDirty(mTarget);
//            }
//        }
//    }
//}

