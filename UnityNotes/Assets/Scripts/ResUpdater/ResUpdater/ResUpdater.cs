using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ResUpdater
{
    public delegate Coroutine StartCoroutineFunc(IEnumerator routine);

    /// <summary>
    /// 资源更新
    /// 检查版本 --> 检查md5 --> (资源下载) --> 进入游戏
    /// </summary>
    public class ResUpdater
    {
        //下载器
        private DownloaderBase downloader;

        //进度更新
        internal readonly Reporter Reporter;

        //版本检查
        public CheckVersionState CheckVersion;

        //Md5检查，下载内容检查
        public CheckMd5State CheckMd5;

        //更新类型
        public readonly UpdateType UpdateType;

        //下载资源处理
        public DownloadResState DownloadRes;

        //更新路径
        public readonly ResUpdaterPath Path;

        public StartCoroutineFunc Coroutine;

        public ResUpdater(string[] hosts,int thread,Reporter reporter,string projectPlatform,UpdateType type,StartCoroutineFunc coroutine)
        {
            this.UpdateType = type;
            this.Coroutine = coroutine;
            this.Path = new ResUpdaterPath(projectPlatform);
            this.Reporter = reporter;
            this.downloader = new WwwDownloader(hosts, thread, Application.persistentDataPath, DownloadDone, coroutine);
        }

        public void Start()
        {
            CheckVersion = new CheckVersionState(this);
            CheckMd5 = new CheckMd5State(this);
            DownloadRes = new DownloadResState(this);

            CheckVersion.Start();
        }

        internal void StartDownload(string url,string fn,int timeout = 0)
        {
            try
            {
                downloader.StartDownload(url, fn, timeout, 2);
            }
            catch(IOException ex)
            {
                Reporter.OnError(UpdateErrorType.NoSpace, ex.Message);
            }
        }

        /// <summary>
        /// 文件下载完成
        /// </summary>
        /// <param name="err"></param>
        /// <param name="fn"></param>
        private void DownloadDone(string err,string fn)
        {
            if(CheckVersion.CheckDownloadFn(fn))
            {
                CheckVersion.OnDownloadCompleted(err, fn);
            }
            else if(CheckMd5.CheckDownloadFn(fn))
            {
                CheckMd5.OnDownloadCompleted(err, fn);
            }
            else
            {
                DownloadRes.OnDownloadCompleted(err, fn);
            }
        }

        public void Dispose()
        {
            if(downloader != null)
            {
                downloader.Dispose();
            }
        }
    }
}
