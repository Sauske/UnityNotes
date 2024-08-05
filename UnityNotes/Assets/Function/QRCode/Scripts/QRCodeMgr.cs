using UnityEngine;
using ZXing;
using ZXing.Common;
using System;

namespace UMI
{
    public class QRCodeMgr : Singleton<QRCodeMgr>
    {
        public const int CODE_PIXEL = 256;
        public const string CODE_HEAD = "umi:";
        const float REFRESH_TIME = 0.5F; 

        BarcodeWriter barcodeWriter;         // 二维码绘制类

        private WebCamTexture webCamTextrue; // 摄像机映射纹理                                             
        private BarcodeReader barcodeReader; // 二维码识别类 库文件的对象（二维码信息保存的地方)
        private Color32[] data;              // 二维码图片信息以像素点颜色信息数组存放
        private Action<string>  action;      // 扫描后的回调 

        private Texture2D logoTexture;

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void Dispose()
        {
            interval = 0;
            isGoScanning = false;

            
            barcodeWriter = null;

            if (webCamTextrue != null)
            {
                webCamTextrue.Stop();                
            }
            webCamTextrue = null;

            barcodeReader = null;
            data = null;
            action = null;
        }

        public void OnUpdateEx(float fDeltaTime)
        {
            UpdateScanning();
        }

        #region 识别二维码
        bool isGoScanning;// 是否运行扫描
        float interval = 0;// 间隔时间

        private void UpdateScanning()
        {
            if (isGoScanning)
            {
                interval += Time.deltaTime;
                if (interval >= REFRESH_TIME)
                {
                    ScanQRCodeing();
                    interval = 0;
                }
            }
        }

        public void ScanQRCode(Action<string> _action)
        {
            this.action = _action;
            DeviceInit();
        }

        public void ReScanQRCode()
        {
            isGoScanning = true;
        }

        void DeviceInit()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                string deviceName = devices[0].name;
                webCamTextrue = new WebCamTexture(deviceName, 400, 400);
                webCamTextrue.Play();

               ////cameraTexture.texture = webCamTextrue;
               // UIMgr.Instance.ShowUI(UITypeName.UIQRCode, webCamTextrue);         

                barcodeReader = new BarcodeReader();

                interval = 0;
                isGoScanning = true;
            }
            else
            {
               // FlyTipsMgr.Instance.ShowTips("没有摄像机。");
                action?.Invoke("");
            }
        }

        void ScanQRCodeing()
        {
            if (webCamTextrue == null) return;

            data = webCamTextrue.GetPixels32();
            if (barcodeReader != null)
            {
                Result result = barcodeReader.Decode(data, webCamTextrue.width, webCamTextrue.height);
                if (result != null)
                {
                    HandlerResult(result);
                }
            }
        }

        public void ScanQRCodeing(Texture2D texture)
        {
            if (texture == null) return;

            data = texture.GetPixels32();
            if (barcodeReader != null)
            {
                Result result = barcodeReader.Decode(data, texture.width, texture.height);
                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    HandlerResult(result);
                }
                else
                {
                    QRCodeLog.Debug("扫一扫本地图片失败。");
                }
            }
        }

        private void HandlerResult(Result result)
        {
            interval = 0;
            isGoScanning = false;

            if (result == null)
            {
                action?.Invoke("");
                return;
            }
            QRCodeLog.Debug("QR Code result:{0}", result.Text);

            action?.Invoke(result.Text);

           // UIMgr.Instance.HideUI(UITypeName.UIQRCode);
        }

        #endregion


        #region 二维码生成

        public Texture2D ShowQRCode(long uid)
        {
            string msg = string.Format($"{CODE_HEAD + uid}");
            QRCodeLog.Debug("QR Code: msg:{0}", msg);
            return ShowQRCode(msg);
        }

        public Texture2D ShowQRCode(string str)
        {
            return ShowQRCode(str, CODE_PIXEL, CODE_PIXEL);
        }
        /// <summary>
        /// 绘制指定字符串信息的二维码显示到指定区域
        /// </summary>
        /// <param name="str">要生产二维码的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns>返回绘制好的图片信息</returns>
        public Texture2D ShowQRCode(string str, int width, int height)  //  Texture2D t = ShowQRCode(formatStr, 256, 256);
        {
            // 实例化一个图片类
            Texture2D t = new Texture2D(width, height);
            Color32[] col32 = GeneQRCode(str, width, height);
            t.SetPixels32(col32);
            t.Apply();
            return t;
        }

        /// <summary>
        /// 将指定字符串信息转换成二维码图片信息
        /// </summary>
        /// <param name="formatStr">要生产二维码的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns>返回二维码图片的颜色数组信息</returns>
        Color32[] GeneQRCode(string formatStr, int width, int height)
        {
            ZXing.QrCode.QrCodeEncodingOptions options = new ZXing.QrCode.QrCodeEncodingOptions();
            options.CharacterSet = "UTF-8";
            options.Width = width;
            options.Height = height;
            //  设置二维码边缘留白宽度（值越大，六百宽度大，二维码就减小）
            options.Margin = 1;    
            // 实例化字符串绘制二维码工具
            barcodeWriter = new BarcodeWriter { Format = ZXing.BarcodeFormat.QR_CODE, Options = options };
            return barcodeWriter.Write(formatStr);
        }
        #endregion


        #region 生成带LOGO二维码
        public Texture2D ShowQRCodeWithLogo(long uid)
        {
            string msg = string.Format($"{CODE_HEAD + uid}");
            QRCodeLog.Debug("QR Code: msg:{0}", msg);
            return ShowQRCodeWithLogo(msg);
        }

        public Texture2D ShowQRCodeWithLogo(string str)
        {
            return ShowQRCodeWithLogo(str, CODE_PIXEL, CODE_PIXEL);
        }
        /// <summary>
        /// 绘制指定字符串信息的二维码显示到指定区域
        /// </summary>
        /// <param name="str">要生产二维码的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns>返回绘制好的图片信息</returns>
        public Texture2D ShowQRCodeWithLogo(string str, int width, int height)
        {
            return GeneQRCodeWithLogo(str, width, height);
        }

        /// <summary>
        /// 将指定字符串信息转换成二维码图片信息
        /// </summary>
        /// <param name="formatStr">要生产二维码的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns>返回二维码图片的颜色数组信息</returns>
        Texture2D GeneQRCodeWithLogo(string formatStr, int width, int height)
        {
            // 创建二维码写入器
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;

            // 设置二维码的参数
            EncodingOptions options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 1,
                //PureBarcode = false
            };
            writer.Options = options;

            // 生成二维码图像
            Color32[] qrCodeBitmap = writer.Write(formatStr);

            // 将Bitmap转换为Texture2D
            Texture2D qrCodeTexture = new Texture2D(width, height);
            qrCodeTexture.SetPixels32(qrCodeBitmap);
            qrCodeTexture.Apply();

            if (logoTexture == null)
            {
                logoTexture = Resources.Load<Texture2D>("logo");
            }
            if(logoTexture != null)
            {
                // 在二维码中心位置绘制Logo
                int logoWidth = logoTexture.width;
                int logoHeight = logoTexture.height;
                int logoX = (qrCodeTexture.width - logoWidth) / 2;
                int logoY = (qrCodeTexture.height - logoHeight) / 2;

                for (int x = 0; x < logoWidth; x++)
                {
                    for (int y = 0; y < logoHeight; y++)
                    {
                        Color color = logoTexture.GetPixel(x, y);
                        qrCodeTexture.SetPixel(logoX + x, logoY + y, color);
                    }
                }
                qrCodeTexture.Apply();
            }

            return qrCodeTexture;
        }
        #endregion
    }
}
