using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ResUpdater
{
    public enum Loc
    {
        Stream,
        Persistent,
        Latest,
    }

    public enum UpdateState
    {
        CheckVersion,
        CheckMd5,
        DownloadComfirm,
        DownloadRes,
        Succeed,
        Failed,
    }

    public enum UpdateErrorType
    {
        NetWorkErr,
        ReadVersionErr,
        ReadMd5Err,
        DownloadVersionErr,
        DownloadMd5Err,
        NoSpace,
    }
    public interface Reporter
	{
        void CheckVersionDone(UpdateState nextState, UpdateVersion localVersion, UpdateVersion latestVersion);

        void CheckMd5Done(UpdateState nextState, ResUpdaterInfo info = null, Action downloadAction = null);

        void DownloadOneResComplete(string err, string fn, BundleMeta info);

        void DownloadResDone(UpdateState nextState, int errCount);

        void OnError(UpdateErrorType type, string err = "", Loc loc = Loc.Persistent);
    }
}
