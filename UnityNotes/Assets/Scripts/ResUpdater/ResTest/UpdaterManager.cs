using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ResUpdater
{

    public class UpdaterServer
    {
        public string[] Hosts;
    }

    /// <summary>
    /// 更新管理器
    /// </summary>
    public class UpdaterManager:Reporter
	{

        private ResUpdater updater;

        //最大线程数
        private int maxThread = 12;

        //是否强制检查版本
        private bool isForceCheckVersion = false;

        //版本更新信息
        private ResUpdaterInfo updateInfo;

        //更新UI
        //private UIUpdateVersionRootRef updateUI = null;

        //提示框UI
        // private UIMessageBoxSimple messageBoxUI = null;

        //private List<AssetLoadAgent> resAgents = new List<AssetLoadAgent>();

        //已下载完成数
        private int downloadCount = 0;

        public Action<bool> updateCompleteAction = null;

        //更新列表
        public static string UpdateServerFilePath = ResUpdaterPath.AssetBundlePathInStream + "updateserverlist.txt";

        //>-----------------------------------------------------------------------------
        
        public void Start()
        {


            //// 启动更新流程,更新资源
            //updater = new ResUpdater(GameMain.config.updateHost, maxThread, this, ResourcesPath.ProjectPlatform, UpdateType.Res,
            //    CoroutineHelper.ins.StartCoroutine);
            //updater.Start();
        }


        /// <summary>
        /// 版本检查完成
        /// </summary>
        /// <param name="nextState"></param>
        /// <param name="localVersion"></param>
        /// <param name="latestVersion"></param>
        public void CheckVersionDone(UpdateState nextState,UpdateVersion localVersion,UpdateVersion latestVersion)
        {
            //if (nextState == UpdateState.Succeed || (nextState == UpdateState.Failed && !isForceCheckVersion))
            //{
            //    // 本地版本比服务器版本还高，bundleinfo都要使用本地的，重新加载
            //    // 开发阶段可能会有这个问题，还有就是读取服务器版本信息出错的时候
            //    if (localVersion.IsMax(latestVersion) && !latestVersion.IsNull())
            //    {
            //        Res.useStreamBundleInfo = true;

            //        Debug.LogWarning("local version greater than server version,reload stream bundleinfo!");
            //        FinishWork(true);
            //    }
            //    else
            //    {
            //        FinishWork(false);
            //    }
            //}
            //else
            //{
            //    UpdateCurrentState(nextState, localVersion.FinalVersion, latestVersion.FinalVersion);
            //}
        }

        /// <summary>
        /// 资源检查完毕
        /// </summary>
        /// <param name="nextState"></param>
        /// <param name="info"></param>
        /// <param name="downloadAction"></param>
        public void CheckMd5Done(UpdateState nextState,ResUpdaterInfo info = null,Action downloadAction = null)
        {
            //if (nextState == UpdateState.Succeed)
            //{
            //    FinishWork(false);
            //}

            //if (nextState == UpdateState.DownloadRes && info != null)
            //{
            //    UpdateCurrentState(nextState);

            //    updateInfo = info;
            //    downloadCount = 0;

            //    // 获得下载大小,提示更新
            //    var content = string.Format(Localization.Get("update_xiazaitishi"), info.Size, info.Format);
            //    messageBoxUI.SetInformation(content, "", downloadAction, false, UpdateSkipedOrFailed);
            //    messageBoxUI.gameObject.SetActive(true);
            //}
        }

        public void UpdateCurrentState(UpdateState state, string localVersion = "", string lastVersion = "")
        {
            //updateUI.SetStateLabel(GetStateStr(state));

            //// 下载资源
            //if (state == GreatWall.UpdateState.DownloadRes)
            //{
            //    updateUI.LabelProgress.enabled = true;
            //    updateUI.Slider.gameObject.SetActive(true);
            //    updateUI.Slider.value = 0;
            //}

            //// 版本获取完毕，检查md5
            //if (state == GreatWall.UpdateState.CheckMd5)
            //{
            //    updateUI.SetVersionLabel(string.Format(Localization.Get("update_banbenxinxi"), lastVersion, localVersion));
            //}

            //if (state == GreatWall.UpdateState.Failed)
            //{
            //    UpdateError(Localization.Get("update_gengxinshibai"));
            //}
        }

        /// <summary>
        /// 资源下载完成
        /// </summary>
        /// <param name="err"></param>
        /// <param name="fn"></param>
        /// <param name="info"></param>
        public void DownloadOneResComplete(string err,string fn,BundleMeta info)
        {
            //downloadCount++;

            //updateUI.LabelProgress.text = downloadCount + "/" + updateInfo.UpdateCount;
            //updateUI.Slider.value = (float)downloadCount / updateInfo.UpdateCount;
        }


        /// <summary>
        /// 资源下载完成回调
        /// </summary>
        /// <param name="nextState"></param>
        /// <param name="errCount"></param>
        public void DownloadResDone(UpdateState nextState, int errCount)
        {
            //UpdateCurrentState(nextState);

            //if (nextState == UpdateState.Succeed)
            //{
            //    FinishWork(true);
            //}
        }

        /// <summary>
        /// 加载失败回掉
        /// </summary>
        /// <param name="error"></param>
        public void UpdateError(string error)
        {
            //messageBoxUI.SetInformation(error, "", Start, false, UpdateSkipedOrFailed);
            //messageBoxUI.gameObject.SetActive(true);
        }


        /// <summary>
        /// 更新完成
        /// </summary>
        /// <param name="needReloadRes"></param>
        private void FinishWork(bool needReloadRes)
        {
            //CoroutineHelper.ins.QueueOnMainThread(() =>
            //{
            //    DestoryUI();

            //    if (UpdateCompleteAction != null)
            //    {
            //        UpdateCompleteAction(needReloadRes);
            //    }
            //});
        }

        /// <summary>
        /// 取消更新或更新失败
        /// </summary>
        private void UpdateSkipedOrFailed()
        {
            //// 比对版本号是否允许跨版本进入游戏
            //DestoryUI();

            //if (UpdateCompleteAction != null)
            //{
            //    UpdateCompleteAction(false);
            //}
        }

        private void DestoryUI()
        {
            //GameObject.Destroy(messageBoxUI.gameObject);
            //GameObject.Destroy(updateUI.gameObject);

            //for (int index = 0; index < resAgents.Count; index++)
            //{
            //    var assetLoadAgent = resAgents[index];
            //    assetLoadAgent.Release();
            //}
        }

        /// <summary>
        /// 获得状态文字
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string GetStateStr(UpdateState state)
        {
            var str = string.Empty;
            switch (state)
            {
                case UpdateState.CheckMd5:
                    str = Localization.Get("update_jianchawenjian");
                    break;
                case UpdateState.CheckVersion:
                    str = Localization.Get("update_jianchabanben");
                    break;
                case UpdateState.DownloadRes:
                    str = Localization.Get("update_xiazaiziyuan");
                    break;
                case UpdateState.Failed:
                    str = Localization.Get("update_gengxinshibai");
                    break;
                case UpdateState.Succeed:
                    str = Localization.Get("update_gengxinchenggong");
                    break;
            }
            return str;
        }

        /// <summary>
        /// 获取错误描述
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetErrorStr(UpdateErrorType type)
        {
            var msg = string.Empty;
            switch (type)
            {
                case UpdateErrorType.NetWorkErr:
                    msg = Localization.Get("update_wangluoshibai");
                    break;
                case UpdateErrorType.NoSpace:
                    msg = Localization.Get("update_kongjianbuzu");
                    break;
                case UpdateErrorType.ReadMd5Err:
                    msg = Localization.Get("update_jianchawenjianshibai");
                    break;
                case UpdateErrorType.ReadVersionErr:
                    msg = Localization.Get("update_jianchabanbenshibai");
                    break;
                case UpdateErrorType.DownloadMd5Err:
                    msg = Localization.Get("update_xiazaiwenjianshibai");
                    break;
                case UpdateErrorType.DownloadVersionErr:
                    msg = Localization.Get("update_xiazaibanbenshibai");
                    break;
            }

            return msg;
        }

        /// <summary>
        /// 加载错误回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="error"></param>
        /// <param name="loc"></param>
        public void OnError(UpdateErrorType type, string error = "", Loc loc = Loc.Persistent)
        {
            //CoroutineHelper.ins.QueueOnMainThread(() =>
            //{
            //    UpdateError(GetErrorStr(type));
            //});
        }
    }
}
