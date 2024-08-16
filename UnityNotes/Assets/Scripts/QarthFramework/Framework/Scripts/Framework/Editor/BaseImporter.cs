using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Qarth.Editor
{
    public class BaseImporter : AssetPostprocessor
    {
        //导入音频前
        void OnPreprocessAudio()
        {
            var importer = assetImporter as AudioImporter;
            if (importer == null)
                return;
            importer.loadInBackground = true;
            AudioImporterSampleSettings setting = new AudioImporterSampleSettings();
            setting.compressionFormat = AudioCompressionFormat.Vorbis;
            setting.quality = 0.5f;
            setting.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            importer.preloadAudioData = false;

            importer.defaultSampleSettings = setting;
        }

        //导入模型前
        void OnPreprocessModel()
        {
            ModelImporter importer = assetImporter as ModelImporter;
            if (importer == null)
                return;

            if (importer.isReadable)
            {
                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }

        //导入动画前
        void OnPreprocessAnimation()
        {
            var importer = assetImporter as ModelImporter;
            if (importer != null)
            {
                //这些名字的动画设置为循环的
                List<string> loopList = new List<string>()
                    { "run", "stand", "riderun", "ridestand", "idle", "ridestand", "ready", "walk" };
                var clips = importer.clipAnimations;
                if (clips == null || clips.Length == 0)
                    clips = importer.defaultClipAnimations;
                if (clips != null)
                {
                    foreach (ModelImporterClipAnimation clipAnimation in clips)
                    {
                        if (loopList.Contains(clipAnimation.name.ToLower()))
                        {
                            clipAnimation.loopTime = true;
                        }
                    }
                }

                importer.clipAnimations = clips;

                //NOTE 如果需要其它的设置，在这儿扩充
            }
        }

        //导入图片前
        public void OnPreprocessTexture()
        {
            TextureImporter importer = this.assetImporter as TextureImporter;
            if (importer == null)
                return;
            if (importer.assetPath.Contains("UI"))
            {
                importer.textureType = TextureImporterType.Sprite;
            }
            
            var andSettings = new TextureImporterPlatformSettings();
            var iosSettings = new TextureImporterPlatformSettings();
            andSettings.name = "Android";
            iosSettings.name = "iPhone";
            andSettings.overridden = true;
            iosSettings.overridden = true;
            andSettings.compressionQuality = iosSettings.compressionQuality = 50;

            andSettings.format = TextureImporterFormat.ETC2_RGBA8;
            andSettings.androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit;
            iosSettings.format = TextureImporterFormat.ASTC_6x6;
            andSettings.maxTextureSize = iosSettings.maxTextureSize = 1024; //NOTE 如果有特殊需求可通过配置修改

            importer.SetPlatformTextureSettings(andSettings);
            importer.SetPlatformTextureSettings(iosSettings);
        }

    }
}