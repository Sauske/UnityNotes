using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResUpdater
{
    /// <summary>
    /// 下载器基类
    /// </summary>
    public abstract class DownloaderBase
	{
        protected class DownloadInfo
        {
            public string fn;
            public int tryCnt;
            public string url;
            public int TimeOut;

            public DownloadInfo(string fn,string url,int timeout,int tryCnt)
            {
                this.fn = fn;
                this.tryCnt = tryCnt;
                this.url = url;
                this.TimeOut = timeout;
            }
        }

        protected readonly string[] hosts; //前面网络更好，带宽更贵，所有meta从前往后试，data从后往前试；只试一轮
        protected readonly int maxProcessing; //同时download的文件数
        protected readonly string outputPath; //下载路径
        protected DownloadDoneFunc downloadDone;
        protected readonly StartCoroutineFunc coroutine;

        protected DownloaderBase(string[] hosts,int maxProcessing,string outputPath,DownloadDoneFunc downloadDone,StartCoroutineFunc coroutine)
        {
            this.hosts = hosts;
            this.maxProcessing = maxProcessing;
            this.outputPath = outputPath;
            this.downloadDone = downloadDone;
            this.coroutine = coroutine;
        }

        public abstract void StartDownload(string url, string fn, int timeout, int tryCnt);

        public abstract void Dispose();
    }
}
