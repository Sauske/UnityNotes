using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;

namespace ResUpdater
{
    /// <summary>
    /// 下载所有md5state 筛选后的资源并且记录下载状态
    /// 下载完成后覆盖下载的bundleinfo为当前
    /// </summary>
    public class DownloadResState
	{
        private readonly ResUpdater Updater;

        public Dictionary<string, BundleMeta> DownloadList { get; private set; }

        public int OkCount { get; private set; }

        public int ErrCount { get; private set; }

        private readonly Stopwatch watch = new Stopwatch();

        private readonly string persistentDataPath = Application.persistentDataPath;

        public DownloadResState(ResUpdater updater)
        {
            this.Updater = updater;
        }


        public void Start(Dictionary<string,BundleMeta> needDownloads)
        {
            watch.Start();
            DownloadList = needDownloads;
            foreach(var kv in DownloadList)
            {
                Updater.StartDownload(kv.Key, kv.Key);
            }
        }

        internal void OnDownloadCompleted(string err,string fn)
        {
            Updater.Reporter.DownloadOneResComplete(err, fn, DownloadList[fn]);
            if (string.IsNullOrEmpty(err))
                OkCount++;
            else
                ErrCount++;

            if ((OkCount+ErrCount)==DownloadList.Count)
            {
                if(ErrCount == 0)
                {
                    watch.Stop();
                    ReplaceLatestToCurrent();
                    Updater.Reporter.DownloadResDone(UpdateState.Succeed, 0);
                }
                else
                {
                    Updater.Reporter.DownloadResDone(UpdateState.Failed, ErrCount);
                }

                Updater.Dispose();
            }
        }

        /// <summary>
        /// 替换最新的配置为本地配置
        /// </summary>
        public void ReplaceLatestToCurrent()
        {
            // 替换bundleinfo
            for (int i = 0;i < Updater.CheckMd5.CheckBundleInfos.Length;i++)
            {
                var bundleName = Updater.CheckMd5.CheckBundleInfos[i];
                RenameFileName(bundleName + ResUpdaterPath.LastestName, bundleName);
            }

            for(int i = 0;i < Updater.Path.VersionInfos.Length;i++)
            {
                if(Updater.UpdateType == UpdateType.Res && i== 0)
                {
                    continue;
                }

                var versionPath = Updater.Path.VersionInfos[i];
                RenameFileName(versionPath + ResUpdaterPath.LastestName, versionPath);
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="targetFileName"></param>
        private void RenameFileName(string sourceFileName,string targetFileName)
        {
            var sourcePath = persistentDataPath + sourceFileName;
            var targetPath = persistentDataPath + targetFileName;

            if(File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }

            File.Move(sourcePath, targetPath);
        }
    }
}
