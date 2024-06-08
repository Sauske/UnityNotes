using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResUpdater
{
    public class WwwDownloader : DownloaderBase
	{
        private readonly Queue<DownloadInfo> downloadQueue = new Queue<DownloadInfo>();

        private static int jobProcessingCount;

        private int mainHostIndex = 0;

        public WwwDownloader(string[] hosts,int maxProcessingCount,string outputPath,DownloadDoneFunc func,StartCoroutineFunc coroutine):base(hosts,maxProcessingCount,outputPath,func
            ,coroutine)
        {

        }

        public override void StartDownload(string url, string fn, int timeout, int tryCnt)
        {
            DownloadInfo di = new DownloadInfo(fn, url, timeout, tryCnt);
            downloadQueue.Enqueue(di);
            CheckDownloadProcessing();
        }

        public override void Dispose()
        {
            downloadQueue.Clear();
        }

        private void CheckDownloadProcessing()
        {
            if (downloadQueue.Count == 0) return;

            if(jobProcessingCount < maxProcessing)
            {
                coroutine(ProcessJob());
            }
        }

        private IEnumerator ProcessJob()
        {
            jobProcessingCount++;

            DownloadInfo job = downloadQueue.Dequeue();

            yield return null;

            if(job != null)
            {
                string downloadError = "";
                var hostIndex = mainHostIndex;

                for(int i = 0;i < job.tryCnt;i++)
                {
                    WWW www = new WWW(hosts[hostIndex] + job.url);

                    var isTimeout = false;

                    if(job.TimeOut != 0)
                    {
                        float timer = 0;

                        while(!www.isDone)
                        {
                            if(timer > job.TimeOut)
                            {
                                isTimeout = true;
                                break;
                            }

                            timer += Time.deltaTime;
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return www;
                    }

                    // 如果超时 把地址放到列表尾部 并且再次尝试
                    if (isTimeout || !string.IsNullOrEmpty(www.error))
                    {
                        downloadError = "timeout";
                        if (!string.IsNullOrEmpty(www.error))
                        {
                            downloadError = www.error;
                            Debug.Log("Download Error :" + www.url + " : " + www.error);
                        }

                        if(hosts.Length - 1 > hostIndex)
                        {
                            hostIndex++;
                        }
                        else
                        {
                            hostIndex = 0;
                        }
                        www.Dispose();
                        continue;
                    }

                    mainHostIndex = hostIndex;
                    byte[] buffer = www.bytes;
                    string filePath = outputPath + "/" + job.fn;
                    var dirPath = Path.GetDirectoryName(filePath);
                    if(!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

#if !UNITY_WEBPLAYER
                    File.WriteAllBytes(filePath, buffer);
#endif

#if UNITY_IPHONE
                    //Apple will reject the app if this is backed up
                    UnityEngine.iOS.Device.SetNoBackupFlag(filePath);
#endif
                    downloadError = "";
                    www.Dispose();
                    break;
                }
                downloadDone(downloadError, job.fn);      
            }

            jobProcessingCount--;
            CheckDownloadProcessing();
        }
    }
}
