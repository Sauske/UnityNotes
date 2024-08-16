//==================================================================================
///Image管理器
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    
    public class ImageManager : Singleton<ImageManager>
    {
        private Dictionary<string, Sprite> mSpriteDic;

        public override void Init()
        {
            base.Init();

            mSpriteDic = new Dictionary<string, Sprite>();
        }


        public override void UnInit()
        {
            base.UnInit();

            mSpriteDic.Clear();
        }

        public Sprite GetCacheSprite(string url)
        {
            if (mSpriteDic.ContainsKey(url) && mSpriteDic[url] != null)
            {
                return mSpriteDic[url];
            }

          //  string path = Application.persistentDataPath + url + ".png";

            byte[] data = new byte[10];  //TODO:后面补全CFileManager

            if (data == null || data.Length <= 0)
                return GetCacheSprite("DefaultImage");//给一张默认的Sprite
            Texture2D tex = null;

            tex = new Texture2D(100, 100, TextureFormat.RGB24, false);
            tex.LoadImage(data);

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            mSpriteDic[url] = sprite;

            return sprite;
        }
    }
}