using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;

namespace ResUpdater
{
    public delegate void DownloadDoneFunc(string error, string fn);

    /// <summary>
    /// 文件下载器，负责文件下载
    /// </summary>
    public class HttpDownloader : DownloaderBase
	{
        const int BUFFER_SIZE = 4096;

        //下载队列
        private Queue<DownloadInfo> downloadQueue = new Queue<DownloadInfo>();

        private bool threadNeedRun = true;

        private Thread downloadThread;

        private byte[] buffer;

        public HttpDownloader(string[] hosts,int maxProcessing,string outputPath,DownloadDoneFunc downloadDone,StartCoroutineFunc coroutine):base(hosts,maxProcessing,outputPath,downloadDone,coroutine)
        {
            buffer = new byte[BUFFER_SIZE];
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fn"></param>
        /// <param name="timeout"></param>
        /// <param name="tryCount"></param>
        public override void StartDownload(string url, string fn, int timeout, int tryCnt)
        {
            var info = new DownloadInfo(fn, url, timeout, tryCnt);
            downloadQueue.Enqueue(info);
            ContinueDownload();
        }


        public override void Dispose()
        {
            buffer = null;
            threadNeedRun = false;
            downloadDone = null;
            downloadQueue.Clear();
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        private void ContinueDownload()
        {
            if(downloadThread == null || downloadThread.ThreadState != ThreadState.Running)
            {
                downloadThread = new Thread(DownloadProcess);
                downloadThread.Priority = ThreadPriority.Lowest;
                downloadThread.Start();
            }
        }

        /// <summary>
        /// 下载处理
        /// </summary>
        private void DownloadProcess()
        {
            while(threadNeedRun && downloadQueue.Count > 0)
            {
                var info = downloadQueue.Dequeue();
                try
                {
                    if (info != null)
                    {
                        //请求下载
                        info.tryCnt--;
                        var address = hosts[0] + info.url;

                        var request = Request(address);
                        var response = request.GetResponse();

                        if (response.ContentLength <= 0)
                        {
                            downloadDone("size error", info.fn);
                            continue;
                        }

                        //写入文件
                        var fileName = outputPath + "/" + info.fn;
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        var dirName = Path.GetDirectoryName(fileName);
                        if (string.IsNullOrEmpty(dirName) && Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }

                        using (var fs = File.Create(fileName))
                        {
                            var count = 0;
                            var responseStream = response.GetResponseStream();
                            do
                            {
                                count = responseStream.Read(buffer, 0, buffer.Length);
                                if (count == 0) break;

                                fs.Write(buffer, 0, count);
                                Thread.Sleep(1);
                            }
                            while (true);

                            responseStream.Close();
                            response.Close();
                            DownloadComplate("", info.fn);
                        }
                    }
                }
                catch(Exception ex)
                {
                    if(info != null)
                    {
                        DownloadComplate(ex.Message, info.fn);
                    }
                    else
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            downloadThread = null;
        }

        private void DownloadComplate(string error,string fn)
        {
            if(downloadDone != null)
            {
                downloadDone(error, fn);
            }
        }


        private HttpWebRequest Request(string url,string method = "GET")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 10000;
            request.Method = method;
            request.Proxy = null;
            request.UserAgent = "Mozilla/4.0";
            request.Headers.Add("Accept-Encoding", "gzip,deflate");
            request.ProtocolVersion = HttpVersion.Version11;
            return request;
        }
    }
}
