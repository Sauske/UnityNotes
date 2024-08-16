using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System;

namespace Framework
{


    public enum ImageAlphaTexLayout
    {
        None,
        Horizonatal,
        Vertical,
    }

    [AddComponentMenu("UI/Image2", 11)]
    public class Image2 : Image
    {
        [SerializeField]
        protected ImageAlphaTexLayout m_alphaTexLayout = ImageAlphaTexLayout.None;

        public ImageAlphaTexLayout alphaTexLayout
        {
            get
            {
                return m_alphaTexLayout;
            }
            set
            {
                if (m_alphaTexLayout != value)
                {
                    m_alphaTexLayout = value;
                    SetMaterialDirty();
                }
            }
        }

        public bool WriteTexcoordToNormal = false;

        static Vector2[] s_sizeScaling = new Vector2[]
    {
        new Vector2(1f, 1f),
        new Vector2(0.5f, 1f),
        new Vector2(1f, 0.5f),
    };

        private static readonly Vector2[] s_VertScratch = new Vector2[4];
        private static readonly Vector2[] s_UVScratch = new Vector2[4];
        private static readonly Vector2[] s_Xy = new Vector2[4];
        private static readonly Vector2[] s_Uv = new Vector2[4];

        private static List<Component> s_components = new List<Component>();

        private static DictionaryObjectView<Material, Material> s_materialList = new DictionaryObjectView<Material, Material>();

        private static Material s_defaultMaterial;
        public new static Material defaultMaterial
        {
            get
            {
                if (s_defaultMaterial == null)
                    s_defaultMaterial = Resources.Load("Shaders/UI/Default2", typeof(Material)) as Material;
                return s_defaultMaterial;
            }
        }

        public Material baseMaterial
        {
            get
            {
                if (m_Material == null || m_Material == defaultMaterial)
                {
                    return (alphaTexLayout == ImageAlphaTexLayout.None) ? Graphic.defaultGraphicMaterial : defaultMaterial;
                }
                else
                {
                    if (alphaTexLayout == ImageAlphaTexLayout.None)
                        return m_Material;

                    Material m = null;
                    if (!s_materialList.TryGetValue(m_Material, out m))
                    {
                        m = new Material(m_Material);
                        m.shaderKeywords = m_Material.shaderKeywords;
                        m.EnableKeyword("_ALPHATEX_ON");
                        s_materialList.Add(m_Material, m);
                    }
                    return m;
                }
            }
        }

        public override Material material
        {
            get
            {
                Material baseMat = baseMaterial;

                UpdateInternalState();

                if (true)//m_IncludeForMasking)
                {
                    if (m_MaskMaterial == null)
                    {
#if UNITY_4_7
                        m_MaskMaterial = StencilMaterial.Add(baseMat, (((int)1) << m_StencilValue) - 1);
#else

#endif
                        if (m_MaskMaterial != null)
                        {
                            m_MaskMaterial.shaderKeywords = baseMat.shaderKeywords;

                            return m_MaskMaterial;
                        }
                    }
                }
                return baseMat;
            }
            set
            {
                base.material = value;
            }
        }

        public void SetMaterialVector(string name, Vector4 factor)
        {
            if (m_Material == null)
                return;

            if (!m_Material.name.Contains("(Clone)"))
            {
                Material m = new Material(m_Material);
                m.name = m_Material.name + "(Clone)";
                m.CopyPropertiesFromMaterial(m_Material);
                m.shaderKeywords = m_Material.shaderKeywords;
                m.SetVector(name, factor);
                this.material = m;
            }
            else
            {
                m_Material.SetVector(name, factor);
                SetMaterialDirty();
            }
        }

        //是否初始化完成
      //  private bool m_initialized = false;

        private int GetStencilForGraphic()
        {
            int stencil = 0;
            Transform tr = transform.parent;
            s_components.Clear();
            while (tr != null)
            {
                tr.GetComponents(typeof(Mask), s_components);
                for (int i = 0; i < s_components.Count; i++)
                {
                    Mask mask = s_components[i] as Mask;
                    if ((mask != null) && mask.MaskEnabled())
                    {
                        stencil++;
                        stencil = Mathf.Clamp(stencil, 0, 8);
                        break;
                    }
                }
                tr = tr.parent;
            }
            s_components.Clear();
            return stencil;
        }

        private void UpdateInternalState()
        {
            //if (m_ShouldRecalculate)
            //{
            //    m_StencilValue = GetStencilForGraphic();
            //    Transform tr = transform.parent;
            //    m_IncludeForMasking = false;
            //    s_components.Clear();
            //    while (maskable && (tr != null))
            //    {
            //        tr.GetComponents(typeof(Mask), s_components);
            //        if (s_components.Count > 0)
            //        {
            //            m_IncludeForMasking = true;
            //            break;
            //        }
            //        tr = tr.parent;
            //    }
            //    m_ShouldRecalculate = false;
            //    s_components.Clear();
            //}
        }

        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect, Vector2 sizeScaling)
        {
            Vector4 padding = !(overrideSprite == null) ? DataUtility.GetPadding(overrideSprite) : Vector4.zero;
            Vector2 size = !(overrideSprite == null) ? new Vector2(overrideSprite.rect.width * sizeScaling.x, overrideSprite.rect.height * sizeScaling.y) : Vector2.zero;
            Rect pixelAdjustedRect = GetPixelAdjustedRect();
            int x = Mathf.RoundToInt(size.x);
            int y = Mathf.RoundToInt(size.y);
            Vector4 dimension = new Vector4(padding.x / (float)x, padding.y / (float)y, ((float)x - padding.z) / (float)x, ((float)y - padding.w) / (float)y);
            if (shouldPreserveAspect && (double)size.sqrMagnitude > 0.0)
            {
                float aspect = size.x / size.y;
                float aspect2 = pixelAdjustedRect.width / pixelAdjustedRect.height;
                if ((double)aspect > (double)aspect2)
                {
                    float height = pixelAdjustedRect.height;
                    pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / aspect);
                    pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * rectTransform.pivot.y;
                }
                else
                {
                    float width = pixelAdjustedRect.width;
                    pixelAdjustedRect.width = pixelAdjustedRect.height * aspect;
                    pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * rectTransform.pivot.x;
                }
            }
            dimension = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * dimension.x, pixelAdjustedRect.y + pixelAdjustedRect.height * dimension.y, pixelAdjustedRect.x + pixelAdjustedRect.width * dimension.z, pixelAdjustedRect.y + pixelAdjustedRect.height * dimension.w);
            return dimension;
        }

        private void GenerateSimpleSprite(List<UIVertex> vbo, bool preserveAspect)
        {
            Vector2 sizeScaling = s_sizeScaling[(int)alphaTexLayout];

            UIVertex uiVertex = UIVertex.simpleVert;
            uiVertex.color = (Color32)color;
            Vector4 pos = GetDrawingDimensions(preserveAspect, sizeScaling);
            Vector4 uv = !(overrideSprite != null) ? Vector4.zero : DataUtility.GetOuterUV(overrideSprite);

            float y0 = uv.y;
            float y3 = uv.w;
            float y1 = (alphaTexLayout == ImageAlphaTexLayout.Vertical) ? (y0 + y3) * 0.5f : y3;
            float y2 = (alphaTexLayout == ImageAlphaTexLayout.Vertical) ? (y0 + y3) * 0.5f : y0;

            float x0 = uv.x;
            float x3 = uv.z;
            float x1 = (alphaTexLayout == ImageAlphaTexLayout.Horizonatal) ? (x0 + x3) * 0.5f : x3;
            float x2 = (alphaTexLayout == ImageAlphaTexLayout.Horizonatal) ? (x0 + x3) * 0.5f : x0;

            uiVertex.position = new Vector3(pos.x, pos.y);
            uiVertex.uv0 = new Vector2(x0, y0);
            uiVertex.uv1 = new Vector2(x2, y2);
            vbo.Add(uiVertex);

            uiVertex.position = new Vector3(pos.x, pos.w);
            uiVertex.uv0 = new Vector2(x0, y1);
            uiVertex.uv1 = new Vector2(x2, y3);
            vbo.Add(uiVertex);

            uiVertex.position = new Vector3(pos.z, pos.w);
            uiVertex.uv0 = new Vector2(x1, y1);
            uiVertex.uv1 = new Vector2(x3, y3);
            vbo.Add(uiVertex);

            uiVertex.position = new Vector3(pos.z, pos.y);
            uiVertex.uv0 = new Vector2(x1, y0);
            uiVertex.uv1 = new Vector2(x3, y2);
            vbo.Add(uiVertex);

            //1       2

            //0       3
        }

        private void GenerateSimpleSprite_Normal(List<UIVertex> vbo, bool preserveAspect)
        {
            Vector2 sizeScaling = s_sizeScaling[(int)alphaTexLayout];

            UIVertex uiVertex = UIVertex.simpleVert;
            uiVertex.color = (Color32)color;
            Vector4 pos = GetDrawingDimensions(preserveAspect, sizeScaling);
            Vector4 uv = !(overrideSprite != null) ? Vector4.zero : DataUtility.GetOuterUV(overrideSprite);

            float y0 = uv.y;
            float y3 = uv.w;
            float y1 = (alphaTexLayout == ImageAlphaTexLayout.Vertical) ? (y0 + y3) * 0.5f : y3;
            float y2 = (alphaTexLayout == ImageAlphaTexLayout.Vertical) ? (y0 + y3) * 0.5f : y0;

            float x0 = uv.x;
            float x3 = uv.z;
            float x1 = (alphaTexLayout == ImageAlphaTexLayout.Horizonatal) ? (x0 + x3) * 0.5f : x3;
            float x2 = (alphaTexLayout == ImageAlphaTexLayout.Horizonatal) ? (x0 + x3) * 0.5f : x0;

            uiVertex.position = new Vector3(pos.x, pos.y);
            uiVertex.uv0 = new Vector2(x0, y0);
            uiVertex.uv1 = new Vector2(x2, y2);
            uiVertex.normal = new Vector3(-1, -1, 0);
            vbo.Add(uiVertex);

            uiVertex.position = new Vector3(pos.x, pos.w);
            uiVertex.uv0 = new Vector2(x0, y1);
            uiVertex.uv1 = new Vector2(x2, y3);
            uiVertex.normal = new Vector3(-1, 1, 0);
            vbo.Add(uiVertex);

            uiVertex.position = new Vector3(pos.z, pos.w);
            uiVertex.uv0 = new Vector2(x1, y1);
            uiVertex.uv1 = new Vector2(x3, y3);
            uiVertex.normal = new Vector3(1, 1, 0);
            vbo.Add(uiVertex);

            uiVertex.position = new Vector3(pos.z, pos.y);
            uiVertex.uv0 = new Vector2(x1, y0);
            uiVertex.uv1 = new Vector2(x3, y2);
            uiVertex.normal = new Vector3(1, -1, 0);
            vbo.Add(uiVertex);
        }

        private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int index = 0; index <= 1; ++index)
            {
                float num1 = border[index] + border[index + 2];
                if ((double)rect.size[index] < (double)num1 && (double)num1 != 0.0)
                {
                    float num2 = rect.size[index] / num1;
                    border[index] *= num2;
                    border[index + 2] *= num2;
                }
            }
            return border;
        }

        private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax)
        {
            v.position = new Vector3(posMin.x, posMin.y, 0.0f);
            v.uv0 = new Vector2(uvMin.x, uvMin.y);
            vbo.Add(v);
            v.position = new Vector3(posMin.x, posMax.y, 0.0f);
            v.uv0 = new Vector2(uvMin.x, uvMax.y);
            vbo.Add(v);
            v.position = new Vector3(posMax.x, posMax.y, 0.0f);
            v.uv0 = new Vector2(uvMax.x, uvMax.y);
            vbo.Add(v);
            v.position = new Vector3(posMax.x, posMin.y, 0.0f);
            v.uv0 = new Vector2(uvMax.x, uvMin.y);
            vbo.Add(v);
        }

        private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax, Vector2 offset)
        {
            v.position = new Vector3(posMin.x, posMin.y, 0.0f);
            v.uv0 = new Vector2(uvMin.x, uvMin.y);
            //v.uv1 = v.uv0 + offset;
            vbo.Add(v);

            v.position = new Vector3(posMin.x, posMax.y, 0.0f);
            v.uv0 = new Vector2(uvMin.x, uvMax.y);
            //v.uv1 = v.uv0 + offset;
            vbo.Add(v);

            v.position = new Vector3(posMax.x, posMax.y, 0.0f);
            v.uv0 = new Vector2(uvMax.x, uvMax.y);
            //v.uv1 = v.uv0 + offset;
            vbo.Add(v);

            v.position = new Vector3(posMax.x, posMin.y, 0.0f);
            v.uv0 = new Vector2(uvMax.x, uvMin.y);
           // v.uv1 = v.uv0 + offset;
            vbo.Add(v);
        }

        private static Vector4 GetOuterUV(Sprite sprite, ImageAlphaTexLayout layout, out Vector2 offset)
        {
            Vector4 outerUV = DataUtility.GetOuterUV(sprite);

            offset = Vector2.zero;

            switch (layout)
            {
                case ImageAlphaTexLayout.Horizonatal:
                    {
                        offset.x = (outerUV.z - outerUV.x) * 0.5f;
                        outerUV.z = (outerUV.z + outerUV.x) * 0.5f;
                    }
                    break;

                case ImageAlphaTexLayout.Vertical:
                    {
                        offset.y = (outerUV.w - outerUV.y) * 0.5f;
                        outerUV.w = (outerUV.w + outerUV.y) * 0.5f;
                    }
                    break;
            }

            return outerUV;
        }

        private static Vector4 GetInnerUV(Sprite sprite, Vector2 sizeScaling)
        {
            Texture tex = sprite.texture;
            if (tex == null)
            {
                return new Vector4(0f, 0f, sizeScaling.x, sizeScaling.y);
            }
            else
            {
                Rect pos = sprite.textureRect;
                pos.width *= sizeScaling.x;
                pos.height *= sizeScaling.y;

                float fx = 1.0f / (float)tex.width;
                float fy = 1.0f / (float)tex.height;

                Vector4 padding = DataUtility.GetPadding(sprite);
                Vector4 border = sprite.border;

                float x = pos.x + padding.x;
                float y = pos.y + padding.y;

                Vector4 v = new Vector4();

                v.x = x + border.x;
                v.y = y + border.y;
                v.z = pos.x + pos.width - border.z;
                v.w = pos.y + pos.height - border.w;

                v.x *= fx;
                v.y *= fy;
                v.z *= fx;
                v.w *= fy;

                return v;
            }
        }

        private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
        {
            if ((double)fill < 1.0 / 1000.0)
                return false;
            if ((corner & 1) == 1)
                invert = !invert;
            if (!invert && (double)fill > 0.999000012874603)
                return true;
            float num = Mathf.Clamp01(fill);
            if (invert)
                num = 1f - num;
            float f = num * 1.570796f;
            float cos = Mathf.Cos(f);
            float sin = Mathf.Sin(f);
            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
        {
            int index1 = corner;
            int index2 = (corner + 1) % 4;
            int index3 = (corner + 2) % 4;
            int index4 = (corner + 3) % 4;
            if ((corner & 1) == 1)
            {
                if ((double)sin > (double)cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[index2].x = Mathf.Lerp(xy[index1].x, xy[index3].x, cos);
                        xy[index3].x = xy[index2].x;
                    }
                }
                else if ((double)cos > (double)sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[index3].y = Mathf.Lerp(xy[index1].y, xy[index3].y, sin);
                        xy[index4].y = xy[index3].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (!invert)
                    xy[index4].x = Mathf.Lerp(xy[index1].x, xy[index3].x, cos);
                else
                    xy[index2].y = Mathf.Lerp(xy[index1].y, xy[index3].y, sin);
            }
            else
            {
                if ((double)cos > (double)sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[index2].y = Mathf.Lerp(xy[index1].y, xy[index3].y, sin);
                        xy[index3].y = xy[index2].y;
                    }
                }
                else if ((double)sin > (double)cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[index3].x = Mathf.Lerp(xy[index1].x, xy[index3].x, cos);
                        xy[index4].x = xy[index3].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (invert)
                    xy[index4].y = Mathf.Lerp(xy[index1].y, xy[index3].y, sin);
                else
                    xy[index2].x = Mathf.Lerp(xy[index1].x, xy[index3].x, cos);
            }
        }

        private void GenerateSlicedSprite(List<UIVertex> vbo)
        {
            if (!hasBorder)
            {
                GenerateSimpleSprite(vbo, false);
            }
            else
            {
                Vector4 outerUV;
                Vector4 innerUV;
                Vector4 padding;
                Vector4 border;
                Vector2 offset = Vector2.zero;
                if (overrideSprite != null)
                {
                    outerUV = GetOuterUV(overrideSprite, alphaTexLayout, out offset);
                    innerUV = GetInnerUV(overrideSprite, s_sizeScaling[(int)alphaTexLayout]);
                    padding = DataUtility.GetPadding(overrideSprite);
                    border = overrideSprite.border;
                }
                else
                {
                    outerUV = Vector4.zero;
                    innerUV = Vector4.zero;
                    padding = Vector4.zero;
                    border = Vector4.zero;
                }
                Rect pixelAdjustedRect = GetPixelAdjustedRect();
                Vector4 adjustedBorders = GetAdjustedBorders(border / pixelsPerUnit, pixelAdjustedRect);
                Vector4 vector4_5 = padding / pixelsPerUnit;

                s_VertScratch[0] = new Vector2(vector4_5.x, vector4_5.y);
                s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector4_5.z, pixelAdjustedRect.height - vector4_5.w);
                s_VertScratch[1].x = adjustedBorders.x;
                s_VertScratch[1].y = adjustedBorders.y;
                s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
                s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
                for (int i = 0; i < 4; ++i)
                {
                    s_VertScratch[i].x += pixelAdjustedRect.x;
                    s_VertScratch[i].y += pixelAdjustedRect.y;
                }

                s_UVScratch[0] = new Vector2(outerUV.x, outerUV.y);
                s_UVScratch[1] = new Vector2(innerUV.x, innerUV.y);
                s_UVScratch[2] = new Vector2(innerUV.z, innerUV.w);
                s_UVScratch[3] = new Vector2(outerUV.z, outerUV.w);

                UIVertex v = UIVertex.simpleVert;
                v.color = (Color32)color;

                for (int index1 = 0; index1 < 3; ++index1)
                {
                    int index2 = index1 + 1;
                    for (int index3 = 0; index3 < 3; ++index3)
                    {
                        if (fillCenter || index1 != 1 || index3 != 1)
                        {
                            int index4 = index3 + 1;
                            AddQuad(vbo, v,
                                new Vector2(s_VertScratch[index1].x, s_VertScratch[index3].y),
                                new Vector2(s_VertScratch[index2].x, s_VertScratch[index4].y),
                                new Vector2(s_UVScratch[index1].x, s_UVScratch[index3].y),
                                new Vector2(s_UVScratch[index2].x, s_UVScratch[index4].y),
                                offset);
                        }
                    }
                }
            }
        }

        private void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
        {
            if ((double)fillAmount < 1.0 / 1000.0)
                return;

            Vector4 drawingDimensions = GetDrawingDimensions(preserveAspect, s_sizeScaling[(int)alphaTexLayout]);

            Vector2 offset = Vector2.zero;
            Vector4 outerUV = Vector4.zero;
            if (overrideSprite != null)
                outerUV = GetOuterUV(overrideSprite, alphaTexLayout, out offset);

            UIVertex uiVertex = UIVertex.simpleVert;
            uiVertex.color = (Color32)color;

            float u0 = outerUV.x;
            float v0 = outerUV.y;
            float u1 = outerUV.z;
            float v1 = outerUV.w;

            if (fillMethod == Image.FillMethod.Horizontal || fillMethod == Image.FillMethod.Vertical)
            {
                if (this.fillMethod == Image.FillMethod.Horizontal)
                {
                    float u = (u1 - u0) * fillAmount;
                    if (fillOrigin == 1)
                    {
                        drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * fillAmount;
                        u0 = u1 - u;
                    }
                    else
                    {
                        drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * fillAmount;
                        u1 = u0 + u;
                    }
                }
                else if (this.fillMethod == Image.FillMethod.Vertical)
                {
                    float v = (v1 - v0) * fillAmount;
                    if (fillOrigin == 1)
                    {
                        drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * fillAmount;
                        v0 = v1 - v;
                    }
                    else
                    {
                        drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * fillAmount;
                        v1 = v0 + v;
                    }
                }
            }
            s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
            s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
            s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
            s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
            s_Uv[0] = new Vector2(u0, v0);
            s_Uv[1] = new Vector2(u0, v1);
            s_Uv[2] = new Vector2(u1, v1);
            s_Uv[3] = new Vector2(u1, v0);
            if ((double)fillAmount < 1.0)
            {
                if (this.fillMethod == Image.FillMethod.Radial90)
                {
                    if (!RadialCut(s_Xy, s_Uv, fillAmount, fillClockwise, fillOrigin))
                        return;
                    for (int index = 0; index < 4; ++index)
                    {
                        uiVertex.position = (Vector3)s_Xy[index];
                        uiVertex.uv0 = s_Uv[index];
                       // uiVertex.uv1 = uiVertex.uv0 + offset;
                        vbo.Add(uiVertex);
                    }
                    return;
                }
                if (this.fillMethod == Image.FillMethod.Radial180)
                {
                    for (int index1 = 0; index1 < 2; ++index1)
                    {
                        int num5 = fillOrigin <= 1 ? 0 : 1;
                        float t1;
                        float t2;
                        float t3;
                        float t4;
                        if (fillOrigin == 0 || fillOrigin == 2)
                        {
                            t1 = 0.0f;
                            t2 = 1f;
                            if (index1 == num5)
                            {
                                t3 = 0.0f;
                                t4 = 0.5f;
                            }
                            else
                            {
                                t3 = 0.5f;
                                t4 = 1f;
                            }
                        }
                        else
                        {
                            t3 = 0.0f;
                            t4 = 1f;
                            if (index1 == num5)
                            {
                                t1 = 0.5f;
                                t2 = 1f;
                            }
                            else
                            {
                                t1 = 0.0f;
                                t2 = 0.5f;
                            }
                        }
                        s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t3);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t4);
                        s_Xy[3].x = s_Xy[2].x;
                        s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t1);
                        s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t2);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;
                        s_Uv[0].x = Mathf.Lerp(u0, u1, t3);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(u0, u1, t4);
                        s_Uv[3].x = s_Uv[2].x;
                        s_Uv[0].y = Mathf.Lerp(v0, v1, t1);
                        s_Uv[1].y = Mathf.Lerp(v0, v1, t2);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;
                        float num6 = !fillClockwise ? fillAmount * 2f - (float)(1 - index1) : this.fillAmount * 2f - (float)index1;
                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num6), fillClockwise, (index1 + fillOrigin + 3) % 4))
                        {
                            for (int index2 = 0; index2 < 4; ++index2)
                            {
                                uiVertex.position = (Vector3)s_Xy[index2];
                                uiVertex.uv0 = s_Uv[index2];
                                //uiVertex.uv1 = uiVertex.uv0 + offset;
                                vbo.Add(uiVertex);
                            }
                        }
                    }
                    return;
                }
                if (this.fillMethod == Image.FillMethod.Radial360)
                {
                    for (int index1 = 0; index1 < 4; ++index1)
                    {
                        float t1;
                        float t2;
                        if (index1 < 2)
                        {
                            t1 = 0.0f;
                            t2 = 0.5f;
                        }
                        else
                        {
                            t1 = 0.5f;
                            t2 = 1f;
                        }
                        float t3;
                        float t4;
                        if (index1 == 0 || index1 == 3)
                        {
                            t3 = 0.0f;
                            t4 = 0.5f;
                        }
                        else
                        {
                            t3 = 0.5f;
                            t4 = 1f;
                        }
                        s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t1);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t2);
                        s_Xy[3].x = s_Xy[2].x;
                        s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t3);
                        s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t4);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;
                        s_Uv[0].x = Mathf.Lerp(u0, u1, t1);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(u0, u1, t2);
                        s_Uv[3].x = s_Uv[2].x;
                        s_Uv[0].y = Mathf.Lerp(v0, v1, t3);
                        s_Uv[1].y = Mathf.Lerp(v0, v1, t4);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;
                        float num5 = !fillClockwise ? fillAmount * 4f - (float)(3 - (index1 + fillOrigin) % 4) : fillAmount * 4f - (float)((index1 + fillOrigin) % 4);
                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num5), fillClockwise, (index1 + 2) % 4))
                        {
                            for (int index2 = 0; index2 < 4; ++index2)
                            {
                                uiVertex.position = (Vector3)s_Xy[index2];
                                uiVertex.uv0 = s_Uv[index2];
                               // uiVertex.uv1 = uiVertex.uv0 + offset;
                                vbo.Add(uiVertex);
                            }
                        }
                    }
                    return;
                }
            }
            for (int index = 0; index < 4; ++index)
            {
                uiVertex.position = (Vector3)s_Xy[index];
                uiVertex.uv0 = s_Uv[index];
                //uiVertex.uv1 = uiVertex.uv0 + offset;
                vbo.Add(uiVertex);
            }
        }

        private void GenerateTiledSprite(List<UIVertex> vbo)
        {
            Vector4 outerUV;
            Vector4 innerUV;
            Vector4 border;
            Vector2 size;
            Vector2 uvOffset;
            if (overrideSprite != null)
            {
                //outerUV = DataUtility.GetOuterUV(overrideSprite);
                //innerUV = DataUtility.GetInnerUV(overrideSprite);
                Vector2 sizeScaling = s_sizeScaling[(int)alphaTexLayout];
                outerUV = GetOuterUV(overrideSprite, alphaTexLayout, out uvOffset);
                innerUV = GetInnerUV(overrideSprite, sizeScaling);
                border = overrideSprite.border;
                size = overrideSprite.rect.size;
                size.x *= sizeScaling.x;
                size.y *= sizeScaling.y;
            }
            else
            {
                outerUV = Vector4.zero;
                innerUV = Vector4.zero;
                border = Vector4.zero;
                size = Vector2.one * 100f;
                uvOffset = Vector2.zero;
            }
            Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
            float num1 = (size.x - border.x - border.z) / this.pixelsPerUnit;
            float num2 = (size.y - border.y - border.w) / this.pixelsPerUnit;
            border = this.GetAdjustedBorders(border / this.pixelsPerUnit, pixelAdjustedRect);
            Vector2 uvMin = new Vector2(innerUV.x, innerUV.y);
            Vector2 vector2_2 = new Vector2(innerUV.z, innerUV.w);
            UIVertex v = UIVertex.simpleVert;
            v.color = (Color32)color;
            float x1 = border.x;
            float x2 = pixelAdjustedRect.width - border.z;
            float y1 = border.y;
            float y2 = pixelAdjustedRect.height - border.w;
            if ((double)x2 - (double)x1 > (double)num1 * 100.0 || (double)y2 - (double)y1 > (double)num2 * 100.0)
            {
                num1 = (float)(((double)x2 - (double)x1) / 100.0);
                num2 = (float)(((double)y2 - (double)y1) / 100.0);
            }
            Vector2 uvMax = vector2_2;
            if (fillCenter)
            {
                float y3 = y1;
                while ((double)y3 < (double)y2)
                {
                    float y4 = y3 + num2;
                    if ((double)y4 > (double)y2)
                    {
                        uvMax.y = uvMin.y + (float)(((double)vector2_2.y - (double)uvMin.y) * ((double)y2 - (double)y3) / ((double)y4 - (double)y3));
                        y4 = y2;
                    }
                    uvMax.x = vector2_2.x;
                    float x3 = x1;
                    while ((double)x3 < (double)x2)
                    {
                        float x4 = x3 + num1;
                        if ((double)x4 > (double)x2)
                        {
                            uvMax.x = uvMin.x + (float)(((double)vector2_2.x - (double)uvMin.x) * ((double)x2 - (double)x3) / ((double)x4 - (double)x3));
                            x4 = x2;
                        }
                        AddQuad(vbo, v, new Vector2(x3, y3) + pixelAdjustedRect.position, new Vector2(x4, y4) + pixelAdjustedRect.position, uvMin, uvMax, uvOffset);
                        x3 += num1;
                    }
                    y3 += num2;
                }
            }
            if (!this.hasBorder)
                return;
            Vector2 vector2_3 = vector2_2;
            float y5 = y1;
            while ((double)y5 < (double)y2)
            {
                float y3 = y5 + num2;
                if ((double)y3 > (double)y2)
                {
                    vector2_3.y = uvMin.y + (float)(((double)vector2_2.y - (double)uvMin.y) * ((double)y2 - (double)y5) / ((double)y3 - (double)y5));
                    y3 = y2;
                }
                AddQuad(vbo, v, new Vector2(0.0f, y5) + pixelAdjustedRect.position, new Vector2(x1, y3) + pixelAdjustedRect.position, new Vector2(outerUV.x, uvMin.y), new Vector2(uvMin.x, vector2_3.y), uvOffset);
                AddQuad(vbo, v, new Vector2(x2, y5) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y3) + pixelAdjustedRect.position, new Vector2(vector2_2.x, uvMin.y), new Vector2(outerUV.z, vector2_3.y), uvOffset);
                y5 += num2;
            }
            vector2_3 = vector2_2;
            float x5 = x1;
            while ((double)x5 < (double)x2)
            {
                float x3 = x5 + num1;
                if ((double)x3 > (double)x2)
                {
                    vector2_3.x = uvMin.x + (float)(((double)vector2_2.x - (double)uvMin.x) * ((double)x2 - (double)x5) / ((double)x3 - (double)x5));
                    x3 = x2;
                }
                AddQuad(vbo, v, new Vector2(x5, 0.0f) + pixelAdjustedRect.position, new Vector2(x3, y1) + pixelAdjustedRect.position, new Vector2(uvMin.x, outerUV.y), new Vector2(vector2_3.x, uvMin.y), uvOffset);
                AddQuad(vbo, v, new Vector2(x5, y2) + pixelAdjustedRect.position, new Vector2(x3, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(uvMin.x, vector2_2.y), new Vector2(vector2_3.x, outerUV.w), uvOffset);
                x5 += num1;
            }
            AddQuad(vbo, v, new Vector2(0.0f, 0.0f) + pixelAdjustedRect.position, new Vector2(x1, y1) + pixelAdjustedRect.position, new Vector2(outerUV.x, outerUV.y), new Vector2(uvMin.x, uvMin.y), uvOffset);
            AddQuad(vbo, v, new Vector2(x2, 0.0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y1) + pixelAdjustedRect.position, new Vector2(vector2_2.x, outerUV.y), new Vector2(outerUV.z, uvMin.y), uvOffset);
            AddQuad(vbo, v, new Vector2(0.0f, y2) + pixelAdjustedRect.position, new Vector2(x1, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(outerUV.x, vector2_2.y), new Vector2(uvMin.x, outerUV.w), uvOffset);
            AddQuad(vbo, v, new Vector2(x2, y2) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, new Vector2(vector2_2.x, vector2_2.y), new Vector2(outerUV.z, outerUV.w), uvOffset);
        }

        public override void SetNativeSize()
        {
            if (!overrideSprite)
                return;
            Vector2 sizeScaling = s_sizeScaling[(int)alphaTexLayout];
            float x = overrideSprite.rect.width * sizeScaling.x / pixelsPerUnit;
            float y = overrideSprite.rect.height * sizeScaling.y / pixelsPerUnit;
            rectTransform.anchorMax = rectTransform.anchorMin;
            rectTransform.sizeDelta = new Vector2(x, y);
            SetAllDirty();
        }

        [Obsolete("This class writed in unity4.7.2")]
        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            if (overrideSprite == null || (alphaTexLayout == ImageAlphaTexLayout.None && !WriteTexcoordToNormal))
            {
                base.OnFillVBO(vbo);
            }
            else
            {
                switch (type)
                {
                    case Image.Type.Simple:
                        {
                            if (WriteTexcoordToNormal)
                                GenerateSimpleSprite_Normal(vbo, preserveAspect);
                            else
                                GenerateSimpleSprite(vbo, preserveAspect);
                        }
                        break;
                    case Image.Type.Sliced:
                        GenerateSlicedSprite(vbo);
                        break;
                    case Image.Type.Tiled:
                        GenerateTiledSprite(vbo);
                        break;
                    case Image.Type.Filled:
                        GenerateFilledSprite(vbo, preserveAspect);
                        break;
                    default:
                        DebugHelper.Assert(false);
                        break;
                }
            }
        }

        public override float preferredWidth
        {
            get
            {
                float width = base.preferredWidth;

                if (alphaTexLayout == ImageAlphaTexLayout.Horizonatal)
                {
                    width *= 0.5f;
                }

                return width;
            }
        }

        public override float preferredHeight
        {
            get
            {
                float height = base.preferredHeight;

                if (alphaTexLayout == ImageAlphaTexLayout.Vertical)
                {
                    height *= 0.5f;
                }

                return height;
            }
        }

        protected override void OnDestroy()
        {
            sprite = null;
            overrideSprite = null;
        }

        /*
            //--------------------------------------------------
            /// 重写OnEnable函数
            /// @CacheCanvas尽量只做一次
            //--------------------------------------------------
            protected override void OnEnable()
            {
                if (m_initialized)
                {
                    SetAllDirty();
                }
                else
                {
                    base.OnEnable();
                    m_initialized = true;
                }
            }
        */

        //--------------------------------------------------
        /// 重写OnCanvasHierarchyChanged函数
        //--------------------------------------------------
#if UNITY_4_6_8 || UNITY_4_6_9
    protected override void OnCanvasHierarchyChanged()
    {
        //here need do nothing...
    }
#endif
    };
}