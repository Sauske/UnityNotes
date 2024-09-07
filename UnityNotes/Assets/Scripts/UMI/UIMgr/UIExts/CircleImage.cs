using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{

    public class CircleImage : Image
    {
        [HideInInspector]
        public int segmements = 100;
        [HideInInspector]
        public float fillPercent = 1;


        [HideInInspector]
        public bool bCircle = true;

        private PolygonCollider2D polygonCollider2D;

        public PolygonCollider2D PolygonCollider2D
        {
            get
            {
                if (polygonCollider2D == null)
                {
                    polygonCollider2D = GetComponent<PolygonCollider2D>();
                    // if (polygonCollider2D == null)
                    // {
                    //     polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
                    // }
                }
                return polygonCollider2D;
            }
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (bCircle)
            {
                OnCirclePopulateMesh(vh);
            }
            else
            {
                base.OnPopulateMesh(vh);
            }
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (PolygonCollider2D == null)
            {
                return base.IsRaycastLocationValid(screenPoint, eventCamera);
            }

            Vector3 point;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out point);
            return PolygonCollider2D.OverlapPoint(point);
        }

        protected virtual void OnCirclePopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Vector2[] uvs = GetUVS(); //SpriteUtility.GetSpriteUVs(overrideSprite, false);
            float minX = 10000, minY = 10000, maxX = -10000, maxY = -10000;
            for (int i = 0; i < uvs.Length; i++)
            {
                Vector2 uv = uvs[i];
                minX = Mathf.Min(minX, uv.x);
                maxX = Mathf.Max(maxX, uv.x);
                minY = Mathf.Min(minY, uv.y);
                maxY = Mathf.Max(maxY, uv.y);
            }
            float uvCenterX = (minX + maxX) * 0.5f;
            float uvCenterY = (minY + maxY) * 0.5f;
            float uvScaleX = (maxX - minX) / rectTransform.rect.width;
            float uvScaleY = (maxY - minY) / rectTransform.rect.height;

            float radian = 2 * Mathf.PI / segmements;
            float widthRadian = rectTransform.rect.width * 0.5f;
            float heightRadian = rectTransform.rect.height * 0.5f;
            UIVertex origin = new UIVertex();

            origin.color = color;
            origin.position = Vector2.zero - new Vector2((rectTransform.pivot.x - 0.5f) * rectTransform.rect.width, (rectTransform.pivot.y - 0.5f) * rectTransform.rect.height);
            origin.uv0 = new Vector2(uvCenterX, uvCenterY);
            vh.AddVert(origin);


            int realSegmement = (int)(segmements * fillPercent);
            int vertexCount = realSegmement + 1;
            float currRadian = 0;

            for (int i = 0; i < vertexCount; i++)
            {
                float x = Mathf.Cos(currRadian) * widthRadian;
                float y = Mathf.Sin(currRadian) * heightRadian;
                UIVertex vertexTemp = new UIVertex();
                vertexTemp.color = color;
                var uv_x = x * uvScaleX + uvCenterX;
                var uv_y = y * uvScaleY + uvCenterY;
                vertexTemp.position = new Vector2(origin.position.x + x, origin.position.y + y);
                vertexTemp.uv0 = new Vector2(uv_x, uv_y);
                vh.AddVert(vertexTemp);
                currRadian += radian;
            }


            for (int i = 0; i < vertexCount; i++)
            {
                vh.AddTriangle(i, 0, i + 1);
            }

        }

        private Vector2[] GetUVS()
        {
            if (overrideSprite == null)
            {
                return new Vector2[]{
                    new Vector2(0,1),
                    new Vector2(1,0),
                    new Vector2(1,1),
                    new Vector2(0,0),
                };
            }
            else
            {
                return overrideSprite.uv;
            }
        }

    }

}