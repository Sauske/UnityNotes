using UnityEngine;
using UnityEngine.UI;

/// <summary>
///�Ƿ�������߼���image
/// </summary>
namespace Framework
{
    public class ImageBlockRaycast : Image
    {
        //blocksRaycasts = false ���������߼��
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