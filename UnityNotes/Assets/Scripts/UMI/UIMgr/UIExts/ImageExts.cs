using System.Collections.Generic;

using UMI;

namespace UnityEngine.UI
{

    /// <summary>
    /// Image 的扩展，
    /// </summary>
    public static class ImageExts
    {
        private static Dictionary<Image, string> mCacheImgSprite = new Dictionary<Image, string>(20);

        /// <summary>
        /// 设置Image 的 Sprite
        /// </summary>
        /// <param name="img"></param>
        /// <param name="atlasAAName"></param>
        /// <param name="spriteName"></param>
        public static void SetImgSprite(this Image img, string atlasAAName, string spriteName,  bool setNativeSize = false)
        {
            if (img == null)
            {
                return;
            }

            if (mCacheImgSprite.ContainsKey(img))
            {
                mCacheImgSprite[img] = spriteName;
            }
            else
            {
                mCacheImgSprite.Add(img, spriteName);
            }

            //AltasMgr.Instance.SetImgSprite(img, atlasAAName, spriteName, OnLoadSpriteOver, setNativeSize);
        }

        /// <summary>
        /// 图集加载完成
        /// </summary>
        /// <param name="img"></param>
        /// <param name="spriteName"></param>
        /// <param name="imgSprite"></param>
        private static void OnLoadSpriteOver(Image img, string spriteName, Sprite imgSprite)
        {
            if (mCacheImgSprite.ContainsKey(img) && mCacheImgSprite[img] == spriteName)
            {
                if (img != null)
                {
                    img.sprite = imgSprite;
                }

                mCacheImgSprite.Remove(img);
            }
        }

        /// <summary>
        /// 加载本地图片，非图集的（UGUI）
        /// </summary>
        /// <param name="image"></param>
        /// <param name="localPath"></param>
       public static void SetSpriteByLocal(this Image image, string localPath){
            //ExternFileLoad.Instance.LoadTexure(localPath, (Texture2D tex, string path) =>
            //{
            //    Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            //    image.sprite = sp;
            //}, () =>
            //{
            //    Log.PrintError("加载本地图片错误:", localPath);
            //});
       }

        public static void AdaptiveUIImage(this Image mImage, bool mIsAdaptHeight = false, float mSize = 0.95f)
        {
            if (mImage.type != Image.Type.Simple)
                return;
            mImage.SetNativeSize();
            if (mImage.rectTransform.sizeDelta.x > mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x)
            {
                float w = mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x / mImage.rectTransform.sizeDelta.x;
                //Debug.Log ("图片宽:" + f + "%");
                mImage.rectTransform.sizeDelta = new Vector2(mImage.rectTransform.sizeDelta.x * w, mImage.rectTransform.sizeDelta.y * w) * mSize;
            }
            if (mIsAdaptHeight)
            {
                if (mImage.rectTransform.sizeDelta.y > mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y)
                {
                    float h = mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y / mImage.rectTransform.sizeDelta.y;
                    //Debug.Log ("图片宽:" + f + "%");
                    mImage.rectTransform.sizeDelta = new Vector2(mImage.rectTransform.sizeDelta.x * h, mImage.rectTransform.sizeDelta.y * h) * mSize;
                }
            }
        }

        public static void AdaptiveUIImage(this RawImage mImage, bool mIsAdaptHeight = false, float mSize = 0.95f)
        {
            // if (mImage.type != Image.Type.Simple)
            //     return;
            mImage.SetNativeSize();
            if (mImage.rectTransform.sizeDelta.x > mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x)
            {
                float w = mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x / mImage.rectTransform.sizeDelta.x;
                //Debug.Log ("图片宽:" + f + "%");
                mImage.rectTransform.sizeDelta = new Vector2(mImage.rectTransform.sizeDelta.x * w, mImage.rectTransform.sizeDelta.y * w) * mSize;
            }
            if (mIsAdaptHeight)
            {
                if (mImage.rectTransform.sizeDelta.y > mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y)
                {
                    float h = mImage.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.y / mImage.rectTransform.sizeDelta.y;
                    //Debug.Log ("图片宽:" + f + "%");
                    mImage.rectTransform.sizeDelta = new Vector2(mImage.rectTransform.sizeDelta.x * h, mImage.rectTransform.sizeDelta.y * h) * mSize;
                }
            }
        }
    }
}
