using UnityEngine;

namespace UMI.FrameCommand
{

    public static class MMGame_Math
    {
        public static float Dot3(this Vector3 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot3(this Vector3 a, ref Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot3(this Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float Dot3(this Vector3 a, ref Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float DotXZ(this Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.z * b.z;
        }

        public static float DotXZ(this Vector3 a, ref Vector3 b)
        {
            return a.x * b.x + a.z * b.z;
        }

        public static Vector3 Mul(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Mul(this Vector3 a, ref Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Mul(this Vector3 a, Vector3 b, float f)
        {
            return new Vector3(a.x * b.x * f, a.y * b.y * f, a.z * b.z * f);
        }

        public static Vector3 Mul(this Vector3 a, ref Vector3 b, float f)
        {
            return new Vector3(a.x * b.x * f, a.y * b.y * f, a.z * b.z * f);
        }

        public static float XZSqrMagnitude(this Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;

            return dx * dx + dz * dz;
        }

        public static float XZSqrMagnitude(this Vector3 a, ref Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;

            return dx * dx + dz * dz;
        }

        public static Vector2 xz(this Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }

        public static string ToString2(this Vector3 a)
        {
            return string.Format("({0},{1},{2})", a.x, a.y, a.z);
        }

        public static Vector3 toVec3(this Vector4 a)
        {
            return new Vector3(a.x, a.y, a.z);
        }

        public static Vector4 toVec4(this Vector3 v, float a)
        {
            return new Vector4(v.x, v.y, v.z, a);
        }

        public static Vector3 RotateY(this Vector3 v, float angle)
        {
            float s = Mathf.Sin(angle);
            float c = Mathf.Cos(angle);

            Vector3 outV;
            outV.x = v.x * c + v.z * s;
            outV.z = -v.x * s + v.z * c;
            outV.y = v.y;
            return outV;
        }

        public static bool isMirror(Matrix4x4 m)
        {
            Vector3 x = m.GetColumn(0).toVec3();
            Vector3 y = m.GetColumn(1).toVec3();
            Vector3 z = m.GetColumn(2).toVec3();

            Vector3 zz = Vector3.Cross(x, y);

            z.Normalize();
            zz.Normalize();

            float f = z.Dot3(ref zz);

            if (f < 0)
            {
                return true;
            }

            return false;
        }

        public static void SetLayer(this GameObject go, string layerName, bool bFileSkillIndicator = false)
        {
            int layer = LayerMask.NameToLayer(layerName);

            SetLayerRecursively(go, layer, bFileSkillIndicator);
        }

        public static void SetLayer(this GameObject go, int layer, bool bFileSkillIndicator)
        {
            SetLayerRecursively(go, layer, bFileSkillIndicator);
        }

        public static void SetLayerRecursively(GameObject go, int layer, bool bFileSkillIndicator)
        {
            if (bFileSkillIndicator == true && go.CompareTag("SCI") == true)
                return;

            go.layer = layer;

            int count = go.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
            }
        }

        public static void SetGameObjVisible(this GameObject go, bool bVisible)
        {
            if (go.IsGameObjHidden() == !bVisible)
            {
                return;
            }

            if (bVisible)
            {
                go.SetLayer("Actor", "Particles", true);
            }
            else
            {
                go.SetLayer("Hide", true);
            }
        }

        public static bool IsGameObjHidden(this GameObject go)
        {
            string layerName = LayerMask.LayerToName(go.layer);

            return layerName == "Hide";
        }

        public static void SetVisibleSameAs(this GameObject go, GameObject tarGo)
        {
            if (IsGameObjHidden(tarGo))
            {
                go.SetGameObjVisible(false);
            }
            else
            {
                go.SetGameObjVisible(true);
            }
        }

        public static void SetLayer(this GameObject go, string layerName, string layerNameParticles, bool bFileSkillIndicator = false)
        {
            int layer = LayerMask.NameToLayer(layerName);
            int layerParticles = LayerMask.NameToLayer(layerNameParticles);

            SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
        }

        public static void SetLayer(this GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
        {
            SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
        }

        public static void SetLayerRecursively(GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
        {
            if (bFileSkillIndicator == true && go.CompareTag("SCI") == true)
                return;

            if (go.GetComponent<ParticleSystem>() != null)
                go.layer = layerParticles;
            else
                go.layer = layer;

            int count = go.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
            }
        }

        public static Renderer GetRendererInChildren(this GameObject go)
        {
            if (go.GetComponent<Renderer>())
                return go.GetComponent<Renderer>();

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform childTrans = go.transform.GetChild(i);
                if (childTrans && childTrans.gameObject)
                {
                    Renderer r = childTrans.gameObject.GetRendererInChildren();
                    if (r)
                        return r;
                }
            }

            return null;
        }

        public static SkinnedMeshRenderer GetSkinnedMeshRendererInChildren(this GameObject go)
        {
            var r = go.GetComponent<Renderer>() as SkinnedMeshRenderer;
            if (r)
                return r;

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform childTrans = go.transform.GetChild(i);
                if (childTrans && childTrans.gameObject)
                {
                    r = childTrans.gameObject.GetSkinnedMeshRendererInChildren();
                    if (r)
                        return r;
                }
            }

            return null;
        }

        public static MeshRenderer GetMeshRendererInChildren(this GameObject go)
        {
            var r = go.GetComponent<Renderer>() as MeshRenderer;
            if (r)
                return r;

            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform childTrans = go.transform.GetChild(i);
                if (childTrans && childTrans.gameObject)
                {
                    r = childTrans.gameObject.GetMeshRendererInChildren();
                    if (r)
                        return r;
                }
            }

            return null;
        }

        public static void SetOffsetX(this Camera camera, float offsetX)
        {
            float height = 2f * Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f) * camera.nearClipPlane;
            float width = height * camera.aspect;

            float centerX = -Mathf.Clamp(offsetX, -1f, 1f) * width;

            float right = (width + centerX) * 0.5f;
            float left = right - width;

            camera.SetPerspectiveOffCenter(left, right, -height * 0.5f, height * 0.5f, camera.nearClipPlane, camera.farClipPlane);
        }

        public static void SetPerspectiveOffCenter(this Camera camera, float left, float right, float bottom, float top, float near, float far)
        {
            float invW = 1f / (right - left);
            float invH = 1f / (top - bottom);
            float invL = 1f / (near - far);

            Matrix4x4 m = new Matrix4x4();

            m.m00 = 2f * near * invW;
            m.m10 = 0f;
            m.m20 = 0f;
            m.m30 = 0f;

            m.m01 = 0f;
            m.m11 = 2f * near * invH;
            m.m21 = 0f;
            m.m31 = 0f;

            m.m02 = (right + left) * invW;
            m.m12 = (top + bottom) * invH;
            m.m22 = far * invL;
            m.m32 = -1f;

            m.m03 = 0f;
            m.m13 = 0f;
            m.m23 = (2f * far * near) * invL;
            m.m33 = 0f;

            camera.projectionMatrix = m;
        }



        public static Vector2 Lerp(this Vector2 left, Vector2 right, float lerp)
        {
            return new Vector2(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp));
        }

        public static Vector3 Lerp(this Vector3 left, Vector3 right, float lerp)
        {
            return new Vector3(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp));
        }

        public static Vector4 Lerp(this Vector4 left, Vector4 right, float lerp)
        {
            return new Vector4(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp), Mathf.Lerp(left.w, right.w, lerp));
        }

        public static int RoundToInt(double x)
        {
            if (x >= 0.0)
            {
                return (int)(x + 0.5);
            }
            else
            {
                return (int)(x - 0.5);
            }
        }

        public static double Round(double x)
        {
            return (double)RoundToInt(x);
        }
    }

}
