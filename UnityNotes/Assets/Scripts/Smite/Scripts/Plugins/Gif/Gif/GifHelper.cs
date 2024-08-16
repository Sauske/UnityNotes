using UnityEngine;
using System.Collections;
using System.IO;

public class GifHelper
{
    static public Texture2D GifToTexture(Stream stream, int frameIndex)
    {
        Gif.Loader _loader = new Gif.Loader();
        if (!_loader.Load(stream))
            return null;

        int _width = _loader._logical_screen_desc.image_width;
        int _height = _loader._logical_screen_desc.image_height;

        int num = _width * _height;
        UnityEngine.Color[] _background = new UnityEngine.Color[num];
        Gif.Color[] color_table = _loader._global_color_table;

        #region clear with bgColor
        int bgIndex = _loader._logical_screen_desc.background_color;
        bool transflag = _loader._frames[0]._GCE_data.transparent_color_flag;
        UnityEngine.Color clear = UnityEngine.Color.clear;
        if ((!transflag && (color_table != null)) && (bgIndex < color_table.Length))
        {
            clear.r = ((float)color_table[bgIndex].r) / 255f;
            clear.g = ((float)color_table[bgIndex].g) / 255f;
            clear.b = ((float)color_table[bgIndex].b) / 255f;
            clear.a = 1f;
        }
        for (int i = 0; i < num; i++)
        {
            _background[i] = clear;
        }
        #endregion

        #region readFrame

        Gif.GifFrame frame = _loader._frames[frameIndex];
       // int x = frame._image.desc.image_left;
       // int y = frame._image.desc.image_top;
       // int blockWidth = frame._image.desc.image_width;
       // int blockHeight = frame._image.desc.image_height;
        if (frame._image.desc.local_color_table_flag)
        {
            color_table = frame._image.desc.local_color_table;
        }
        int length = frame._image.data.Length;
        bool flag2 = frame._GCE_data.transparent_color_flag;
        int trans_color = frame._GCE_data.transparent_color;
        for (int i = 0; i < length; i++)
        {
            int color_idx = frame._image.data[i];
            if (!flag2 || (trans_color != color_idx))
            {
                Gif.Color color = color_table[color_idx];
                _background[i].r = ((float)color.r) / 255f;
                _background[i].g = ((float)color.g) / 255f;
                _background[i].b = ((float)color.b) / 255f;
                _background[i].a = 1f;
            }
        }

        #endregion

        Texture2D _texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
        _texture.SetPixels(_background);
        _texture.Apply();

        return _texture;
    }

}
