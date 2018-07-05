using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

namespace ResUpdater
{
    /// <summary>
    /// 下载最新的bundleinfo，检查文件md5，筛选需要下载的资源
    /// </summary>
    public class CheckMd5State : AbstractCheckState
	{
        public readonly string[] CheckBundleInfos;

        public Dictionary<string, BundleMeta> StreamInfo = new Dictionary<string, BundleMeta>();

        public Dictionary<string, BundleMeta> PersistentInfo = new Dictionary<string, BundleMeta>();

        public Dictionary<string, BundleMeta> LatestInfo = new Dictionary<string, BundleMeta>();

        private int downloadCount = 0;

        public readonly string AssetBundleUrl;
        public readonly string AdditionBundleUrl;

        public CheckMd5State(ResUpdater updater) : base(updater)
        {
            CheckBundleInfos = Updater.UpdateType == UpdateType.Res ? Updater.Path.ResCheckBundleInfos : Updater.Path.AppCheckBundleInfos;

            AssetBundleUrl = "/" + ResUpdaterPath.ExportBundlePath + "/" + Updater.Path.ProjectPlatform + "/";

            AdditionBundleUrl = AssetBundleUrl + ResUpdaterPath.AdditionBundlePath;
        }

        public void Start()
        {
            checkFnCount = 0;
            downloadCount = 0;

            for(int i = 0;i < CheckBundleInfos.Length;i++)
            {
                //streaming
                Updater.Coroutine(StartRead(Loc.Stream, CheckBundleInfos[i]));

                //persitent
                string path = Application.persistentDataPath + CheckBundleInfos[i];
                if(File.Exists(path))
                {
                    Updater.Coroutine(StartRead(Loc.Persistent, CheckBundleInfos[i]));
                }

                //lastet
                Updater.StartDownload(CheckBundleInfos[i], CheckBundleInfos[i] + ResUpdaterPath.LastestName);
            }
        }

        protected override void OnDownloadError(string error)
        {
            Updater.Reporter.OnError(UpdateErrorType.DownloadMd5Err, error);
            Check();
        }

        /// <summary>
        /// www下载完成
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="www"></param>
        /// <param name="fileName"></param>
        protected override void OnWWW(Loc loc, WWW www, string fileName)
        {
            Dictionary<string, BundleMeta> infoDic;

            switch(loc)
            {
                case Loc.Stream:
                    infoDic = StreamInfo;
                    break;
                case Loc.Persistent:
                    infoDic = PersistentInfo;
                    break;
                default:
                    infoDic = LatestInfo;
                    downloadCount++;
                    break;
            }

            if(string.IsNullOrEmpty(www.error))
            {
                Updater.Reporter.OnError(UpdateErrorType.ReadMd5Err, www.error, loc);
            }
            else
            {
                try
                {
                    var bundle = JsonUtility.FromJson<BundleInfo>(www.text);
                    for(int i = 0;i < bundle.metaList.Count;i++)
                    {
                        var meta = bundle.metaList[i];
                        infoDic.Add(GetResRealityName(meta.name, meta), meta);
                    }
                }
                catch(Exception ex)
                {
                    Updater.Reporter.OnError(UpdateErrorType.ReadMd5Err, ex.Message, loc);
                }
            }

            Check();
        }     

        private int checkFnCount = 0;

        /// <summary>
        /// 检查下载标记
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public override bool CheckDownloadFn(string fn)
        {
            if (CheckFnComplate) return false;

            for(int i = 0;i < CheckBundleInfos.Length;i++)
            {
                var checkBundleInfo = CheckBundleInfos[i] + ResUpdaterPath.LastestName;
                if (fn != checkBundleInfo) continue;

                checkFnCount++;
                if(CheckBundleInfos.Length == checkFnCount)
                {
                    CheckFnComplate = true;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 所有文件md5加载完毕之后检查
        /// </summary>
        private void Check()
        {
            if(downloadCount == CheckBundleInfos.Length)
            {
                DoCheckResource(LatestInfo,true);
            }
        }

        /// <summary>
        /// 资源检查
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isTargetLatest"></param>
        private void DoCheckResource(Dictionary<string,BundleMeta> target,bool isTargetLatest)
        {
            var downloadList = new Dictionary<string, BundleMeta>();
            var needDelFiles = new List<FileInfo>();

            foreach(var kv in target)
            {
                var fn = kv.Key;
                var info = kv.Value;

                BundleMeta infoInStream;
                bool inStream = StreamInfo.TryGetValue(fn, out infoInStream) && infoInStream.Equals(info);

                //当前stream里面有这个文件，并且size相同
                if(inStream)
                {
                    Res.resourcesInStreamWhenNotUseStreamVersion.Add(fn);
                }
                else
                {
                    FileInfo fi = new FileInfo(Application.persistentDataPath + fn);
                    if(fi.Exists)
                    {
                        //当文件存在，大小不同的时候，删除重新下载
                        if(fi.Length != info.size)
                        {
                            downloadList.Add(fn, info);
                            needDelFiles.Add(fi);
                        }
                        else if(isTargetLatest)
                        {
                            BundleMeta infoInPersistent;
                            bool inPersistent = PersistentInfo.TryGetValue(fn, out infoInPersistent) && infoInPersistent.Equals(info);

                            //last版本，但是persitent里不存在或者info不同，删除重新下载
                            if(!inPersistent && GetMd5(fi) != info.md5)
                            {
                                downloadList.Add(fn, info);
                                needDelFiles.Add(fi);
                            }
                        }
                    }
                    else
                    {
                        downloadList.Add(fn, info);
                    }
                }
            }

            //不需要下载
            if(downloadList.Count == 0)
            {
                Updater.Reporter.CheckMd5Done(UpdateState.Succeed);
                Updater.DownloadRes.ReplaceLatestToCurrent();
                Debug.LogFormat("check md5 finish,without download");
            }
            else
            {
                long length = downloadList.Sum(bundleMeta => bundleMeta.Value.size);
                ResUpdaterInfo resUpdaterInfo = ResUpdaterUtil.GetUpdaterInfo(length);
                resUpdaterInfo.UpdateCount = downloadList.Count;

                Updater.Reporter.CheckMd5Done(UpdateState.DownloadRes, resUpdaterInfo, () =>
                  {
                      Debug.LogFormat("check md5 finish,need download file count:{0}", downloadList.Count);

                      //删除不需要的文件
                      DeleteUnUseFiles(needDelFiles, target);
                      Updater.DownloadRes.Start(downloadList);
                  });
            }
        }

        /// <summary>
        /// 删除新版本不需要的文件
        /// </summary>
        /// <param name="needDelFiles"></param>
        /// <param name="target"></param>
        private void DeleteUnUseFiles(List<FileInfo> needDelFiles,Dictionary<string,BundleMeta> target)
        {
            //删除需要替换的文件
            for(var i = 0;i < needDelFiles.Count;i++)
            {
                needDelFiles[i].Delete();
            }
            needDelFiles.Clear();

            //删除当前persistent里面有，但是target里面没有的
            foreach(var bundleMeta in PersistentInfo)
            {
                if(target.ContainsKey(bundleMeta.Key))
                {
                    FileInfo fi = new FileInfo(Application.persistentDataPath + bundleMeta.Key);
                    if(fi.Exists)
                    {
                        fi.Delete();
                        Debug.Log("delete new version unuse file:" + bundleMeta.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 获得资源的全称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        private string GetResRealityName(string name,BundleMeta meta)
        {
            var url = meta.bIsAdditionBundle ? AdditionBundleUrl + name : AssetBundleUrl + name;
            return url;
        }

        /// <summary>
        /// 获取文件md5
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static String GetMd5(FileInfo file)
        {
            if (file == null || !file.Exists) return string.Empty;

            using (var stream = file.OpenRead())
            {
                var algorithm = MD5.Create();
                var hashBytes = algorithm.ComputeHash(stream);
                var replace = BitConverter.ToString(hashBytes).Replace("-", "");
                return replace;
            }
        }
    }
}
