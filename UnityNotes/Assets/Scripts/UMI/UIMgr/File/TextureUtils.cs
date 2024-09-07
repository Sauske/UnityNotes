using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UMI
{
    /// <summary>
    /// 图片扩展工具类
    /// </summary>
    public static class TextureUtils
    {
        private static Texture2D s_watermarkLogo;               // 水印logo
        private static Texture2D s_watermarkFontTexture;        // 水印的字体的纹理图，
        private static Font s_watermarkFont;                    // 水印的字体
        
        /// <summary>
        /// 给图片添加水印
        /// </summary>
        /// <param name="image">要添加水印的图片</param>
        /// <returns>加了水印后的新贴图</returns>
        public static Texture2D AddWatermarkToImage(Texture2D image)
        {
            if (s_watermarkLogo == null)
            {
                s_watermarkLogo = Resources.Load<Texture2D>("watermark");
                s_watermarkFontTexture = Resources.Load<Texture2D>("watermarkFontTexture");
                s_watermarkFont = Resources.Load<Font>("watermarkFont");
            }

            return AddWatermarkToImage(image, s_watermarkLogo, s_watermarkFont, s_watermarkFontTexture);
        }

        /// <summary>
        /// 添加水印到图片
        /// </summary>
        /// <param name="image">原图片</param>
        /// <param name="logo">水印logo图</param>
        /// <param name="font">水印字体，字体的Character一定要选择CustomSet，选择Dynamic会获取不字体贴图异常，搞了我几个小时才发现</param>
        /// <param name="fontTex">水印字体的纹理图</param>
        /// <returns>加了水印后的新贴图</returns>
        private static Texture2D AddWatermarkToImage(Texture2D image, Texture2D logo, Font font, Texture2D fontTex)
        {
            Texture2D newTexture = new Texture2D(image.width, image.height);
            Color[] colors = image.GetPixels();

            // 插入logo，起始点(0,0)在左下角
            int logoStartPixelX = image.width - logo.width - 180;
            int logoStartPixelY = 90;
            AddWatermarkToImage(colors, newTexture.width, logo, logoStartPixelX, logoStartPixelY);
            
            // 插入时间
            string timeContent = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            Texture2D timeTex = TextToTexture2D(font, fontTex, timeContent, 400, 80, 10, 10, 1,
                26, 26, Color.red);
            int timeStartPixelX = image.width - logo.width - 300;
            int timeStartPixelY = 65;
            AddWatermarkToImage(colors, newTexture.width, timeTex, timeStartPixelX, timeStartPixelY);

            newTexture.SetPixels(0, 0, image.width, image.height, colors);
            newTexture.Apply();

            return newTexture;
        }

        /// <summary>
        /// 添加水印到图片
        /// </summary>
        /// <param name="colors">原图片的像素集合</param>
        /// <param name="imageWidth">原图片宽度</param>
        /// <param name="watermark">水印图</param>
        /// <param name="startX">水印添加起始坐标x</param>
        /// <param name="startY">水印添加起始坐标y</param>
        private static void AddWatermarkToImage(Color[] colors,  int imageWidth,  Texture2D watermark,int startX, int startY)
        {
            for (int i = 0; i < watermark.width; i++)
            {
                for (int j = 0; j < watermark.height; j++)
                {
                    Color cLogo = watermark.GetPixel(i, j);

                    // 颜色混合
                    if (cLogo.a != 0)
                    {
                        colors[imageWidth * (startY + j) + i + startX] = cLogo;
                    }
                }
            }
        }
        
        /// <summary>
        /// 文本内容转换成Texture2D
        /// </summary>
        /// <param name="font">字体</param>
        /// <param name="fontTex">字体的纹理贴图，一定要有这个，不然android平台获取字体的贴图没有透明度，暂时不知道怎么解决，只能用这个方式处理</param>
        /// <param name="text">Text的文本</param>
        /// <param name="textureWidth">图片的宽</param>
        /// <param name="textureHeight">图片的高（最终会以字体的排版而裁剪掉多余的空白部分）</param>
        /// <param name="drawOffsetX">要渲染的文字的x坐标偏移</param>
        /// <param name="drawOffsetY">要渲染的文字的y坐标偏移</param>
        /// <param name="textGap">每个字之间的间隔</param>
        /// <param name="spaceGap">空白符的间隔</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="textColor">文字颜色</param>
        /// <returns>字体纹理</returns>
        public static Texture2D TextToTexture2D(Font font, Texture2D fontTex, string text,
            int textureWidth, int textureHeight,
            int drawOffsetX, int drawOffsetY,
            int textGap, int spaceGap, int fontSize,
            Color textColor)
        {

            // 创建返回的Texture
            var textTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, true);
            Color[] emptyColor = new Color[textureWidth * textureHeight];
            textTexture.SetPixels(emptyColor);

            // 字体贴图不可读，需要创建一个新的可读的
            // var fontTexture = (Texture2D) font.material.mainTexture;
            // var readableFontTexture = new Texture2D(fontTexture.width, fontTexture.height, fontTexture.format,
                // fontTexture.mipmapCount, true);
            // Graphics.CopyTexture(fontTexture, readableFontTexture);

            var readableFontTexture = fontTex; 

            // Debug.Log($"============= {fontTexture.format}");

            // return readableFontTexture;

            // 调整偏移量
            var originalDrawOffsetX = drawOffsetX; // 记录一下，换行用
            drawOffsetY = textureHeight - drawOffsetY - fontSize; // 从上方开始画

            // 逐个字符绘制
            foreach (var charitem in text.ToCharArray())
            {
                if ('\u3000' == charitem || ' ' == charitem)
                {
                    drawOffsetX += spaceGap;
                    continue;
                }

                if ('\n' == charitem)
                {
                    // 换行
                    drawOffsetX = originalDrawOffsetX;
                    drawOffsetY -= fontSize;

                    continue;
                }

                // 判断是否是中文
                bool isChinese = false;
                if (charitem >= 0x4e00 && charitem <= 0x9fbb)
                {
                    isChinese = true;
                }

                if (drawOffsetX >= textTexture.width - fontSize)
                {
                    // 换行
                    drawOffsetX = originalDrawOffsetX;
                    drawOffsetY -= fontSize;
                }

                int charWidth = 35, charHeight = 35; // 字符宽高
                Color[] charColor; // 字符颜色，数组内颜色的顺序为从左至右，从下至上

                font.GetCharacterInfo(charitem, out CharacterInfo info);

                if (info.uvTopLeft.y < info.uvBottomRight.y) // 处理被垂直翻转的字符
                {
                    charWidth = info.glyphWidth;
                    charHeight = info.glyphHeight;
                    
                    charColor = readableFontTexture.GetPixels(
                        (int) (readableFontTexture.width * info.uvTopLeft.x),
                        (int) (readableFontTexture.height * info.uvTopLeft.y),
                        charWidth, charHeight);
                    
                    int yOffset = 0;
                    if ('-' == charitem)
                    {
                        // -减号，不特殊处理会变成_
                        yOffset = fontSize / 2 - charHeight / 2;
                    }
                    
                    for (int j = 0; j < charHeight; j++)
                    {
                        for (int i = 0; i < charWidth; i++)
                        {
                            if (charColor[j * charWidth + i].a != 0)
                            {
                                // 从上往下画，把字符颠倒过来
                                textTexture.SetPixel(
                                    drawOffsetX + i,
                                    drawOffsetY + charHeight - j +
                                    (isChinese ? ((int) ((fontSize - charHeight) / 2f)) : 0) + yOffset,
                                    textColor);
                            }
                        }
                    }

                    drawOffsetX += charWidth + textGap;
                }
                else // 处理被顺时针旋转90度的字符
                {
                    charWidth = info.glyphHeight;
                    charHeight = info.glyphWidth;
                    charColor = readableFontTexture.GetPixels(
                        (int) (readableFontTexture.width * info.uvBottomRight.x),
                        (int) (readableFontTexture.height * info.uvBottomRight.y),
                        charWidth, charHeight);
                    
                    int yOffset = 0;
                    if ('-' == charitem)
                    {
                        // -减号，不特殊处理会变成_
                        yOffset = fontSize / 2 - charWidth / 2;
                    }
                    
                    for (int j = 0; j < charHeight; j++)
                    {
                        for (int i = 0; i < charWidth; i++)
                        {
                            if (charColor[j * charWidth + i].a != 0)
                            {

                                // 旋转
                                textTexture.SetPixel(
                                    drawOffsetX + charHeight - j,
                                    drawOffsetY + i + (isChinese ? ((int) ((fontSize - charWidth) / 2f)) : 0) + yOffset,
                                    textColor);
                            }
                        }
                    }

                    drawOffsetX += charHeight + textGap;
                }
            }

            var realTextureHeight = textureHeight - drawOffsetY;
            textTexture.Apply();
            var finalTexture = new Texture2D(textureWidth, realTextureHeight, TextureFormat.ARGB32, true);
            Graphics.CopyTexture(textTexture, 0, 0, 0, drawOffsetY, textureWidth, realTextureHeight, finalTexture, 0, 0,
                0, 0);
            Object.Destroy(textTexture);
            // Object.Destroy(readableFontTexture);
            return finalTexture;
        }

        
        /// <summary>
        /// 存储图片到临时目录
        /// </summary>
        /// <param name="texture2D">要存储的图片实例</param>
        /// <param name="fileName">图片存储名称</param>
        /// <returns>文件路径</returns>
        public static string SaveImageToTempDirectory(Texture2D texture2D, string fileName)
        {
            var bytes = texture2D.EncodeToJPG();

            string dirName = Application.persistentDataPath + "/temp";
            string savePath = Path.Combine(dirName, fileName );
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            Directory.CreateDirectory(dirName);
            File.WriteAllBytes(savePath, bytes);

            return savePath;
        }

        public static Texture2D LoadTextureFromLocalPath(string path)
        {
            byte[] dataFile = FileUtils.ReadFile(path);
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(dataFile);
            return texture2D;
        }
    }
}