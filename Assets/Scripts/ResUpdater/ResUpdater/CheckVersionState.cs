using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResUpdater
{
    /// <summary>
    /// 版本检查，当服务器有更新版本的时候启动md5检查
    /// </summary>
    public class CheckVersionState:AbstractCheckState
	{
        
        public UpdaterTypeVersion ResVersion { get; private set; }

        public UpdaterTypeVersion AppVersion { get; private set; }
               

        public CheckVersionState(ResUpdater updater):base(updater)
        {

        }

        public void Start()
        {
            ResVersion = new UpdaterTypeVersion();
            AppVersion = new UpdaterTypeVersion();

            Res.useStreamBundleInfo = false;
            Res.resourcesInStreamWhenNotUseStreamVersion.Clear();

            //获取所有的版本信息
            for(int i = 0;i < Updater.Path.VersionInfos.Length;i++)
            {
                var verinfoPath = Updater.Path.VersionInfos[i];
                Updater.Coroutine(StartRead(Loc.Stream, verinfoPath));
                string path = Application.persistentDataPath + verinfoPath;

                //是否存在persitent版本信息
                if(File.Exists(path))
                {
                    Updater.Coroutine(StartRead(Loc.Persistent, verinfoPath));
                }
                else
                {
                    var version = i == (int)UpdateType.App ? AppVersion : ResVersion;
                    version.SetVersion(Loc.Persistent, new UpdateVersion());
                }

                Updater.StartDownload(verinfoPath, verinfoPath + ResUpdaterPath.LastestName, 5);
            }
        }      

        protected override void OnDownloadError(string error)
        {
            Updater.Reporter.OnError(UpdateErrorType.DownloadVersionErr, error);
            ResVersion.SetVersion(Loc.Latest,new UpdateVersion());
            AppVersion.SetVersion(Loc.Latest, new UpdateVersion());

            Check();
        }

        protected override void OnWWW(Loc loc, WWW www, string fileName)
        {
            UpdateVersion tempVersion = new UpdateVersion();
            if(www.error != null)
            {
                Updater.Reporter.OnError(UpdateErrorType.ReadVersionErr, www.error, loc);
            }
            else
            {
                try
                {
                    tempVersion = GetUpdateVersion(www.text);
                }
                catch(Exception ex)
                {
                    Updater.Reporter.OnError(UpdateErrorType.ReadVersionErr, ex.Message, loc);
                }
            }

            //App版本
            if(fileName.Contains(ResUpdaterPath.ResVersionInfoName))
            {
                ResVersion.SetVersion(loc, tempVersion);
            }
            else
            {
                AppVersion.SetVersion(loc, tempVersion);
            }

            Check();
        }

        /// <summary>
        /// 所有版本号版本读取完毕检查
        /// </summary>
        private void Check()
        {
            if(AppVersion.IsLoadFinish() && ResVersion.IsLoadFinish())
            {
                Debug.LogFormat("AppVersion:Steam:{0} ==== Persistent:{1} ==== Server:{2}",
                   AppVersion.StreamVersion.FinalVersion, AppVersion.PersistentVersion.FinalVersion,
                   AppVersion.LatestVersion.FinalVersion);

                Debug.LogFormat("ResVersion:Steam:{0} ==== Persistent:{1} ==== Server:{2}",
                    ResVersion.StreamVersion.FinalVersion, ResVersion.PersistentVersion.FinalVersion,
                    ResVersion.LatestVersion.FinalVersion);

                bool isNeedUpdate = false;

                if(Updater.UpdateType == UpdateType.App)
                {
                    if(AppVersion.IsNeedUpdate() || ResVersion.IsNeedUpdate())
                    {
                        isNeedUpdate = true;
                    }
                }

                if(Updater.UpdateType == UpdateType.Res)
                {
                    isNeedUpdate = ResVersion.IsNeedUpdate();
                }

                //需要更新进行md5检查
                if(isNeedUpdate)
                {
                    Updater.Reporter.CheckVersionDone(UpdateState.CheckMd5, AppVersion.LocalVersion, AppVersion.LatestVersion);
                    Updater.CheckMd5.Start();
                }
                else
                {
                    Updater.Reporter.CheckVersionDone(UpdateState.Succeed, AppVersion.LocalVersion, AppVersion.LatestVersion);
                }
            }
        }

        private int checkFnCount = 0;

        /// <summary>
        /// 检查下载的标记
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public override bool CheckDownloadFn(string fn)
        {
            if (CheckFnComplate) return false;

            for(int i = 0; i < Updater.Path.VersionInfos.Length;i++)
            {
                var checkVersionPath = Updater.Path.VersionInfos[i] + ResUpdaterPath.LastestName;
                if (fn != checkVersionPath) continue;

                checkFnCount++;
                if(Updater.Path.VersionInfos.Length == checkFnCount)
                {
                    CheckFnComplate = true;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得版本号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public UpdateVersion GetUpdateVersion(string str)
        {
            var vers = str.Split('.');
            var version = new UpdateVersion(str);

            return version;
        }
    }
}
