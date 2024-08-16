using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class RGBCurve : ScriptableObject
{
    public AnimationCurve R = new AnimationCurve();
    public AnimationCurve G = new AnimationCurve();
    public AnimationCurve B = new AnimationCurve();

    public static float MaxTime(AnimationCurve curve)
    {
        if (curve == null || curve.length == 0)
            return 0f;

        int len = curve.length;
        Keyframe k = curve[len - 1];

        return k.time;
    }

    public float length
    {
        get
        {
            float t0 = MaxTime(R);
            float t1 = MaxTime(G);
            float t2 = MaxTime(B);

            return Mathf.Max(Mathf.Max(t0, t1), t2);
        }
    }

    public Vector3 Eval(float t)
    {
        Vector3 v=new Vector3();

        v.x = R.Evaluate(t);
        v.y = G.Evaluate(t);
        v.z = B.Evaluate(t);

        return v;
    }
}