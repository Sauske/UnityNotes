

namespace UnityEngine.UI
{
    /// <summary>
    /// 空白图片，不参与绘制，会拦截点击事件
    /// </summary>
    public class EmptyImage : Image
    {
        protected EmptyImage()
        {
            useLegacyMeshGeneration = false;
            raycastTarget = true;
            //maskable = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
