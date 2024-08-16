using UnityEngine;
using UnityEngine.UI;

/// <summary>
///是否接受射线检测的image
/// </summary>
namespace Framework
{
    public class ImageBlockRaycast : Image
    {
        //blocksRaycasts = false 不接受射线检测
        [SerializeField]
        public bool blocksRaycasts = false;

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (blocksRaycasts)
            {
                return base.IsRaycastLocationValid(screenPoint, eventCamera);
            }
            else
            {
                return false;
            }
        }
    }
}