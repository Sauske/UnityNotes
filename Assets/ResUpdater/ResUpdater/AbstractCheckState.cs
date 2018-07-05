using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResUpdater
{
    public abstract class AbstractCheckState
	{
        protected readonly ResUpdater Updater;

        protected bool CheckFnComplate = false;

        protected AbstractCheckState(ResUpdater updater)
        {
            Updater = updater;
        }

        protected abstract void OnDownloadError(string error);

        protected abstract void OnWWW(Loc loc, WWW www, string fileName);

        internal virtual IEnumerator StartRead(Loc loc,string fileName)
        {
            var file = fileName.Replace(Updater.Path.AssetBundlePath, "");
            string url;

            switch(loc)
            {
                case Loc.Stream:
                    url = Updater.Path.StreamingAssetPathForWWW + "/" + file;
                    break;
                case Loc.Persistent:
                    url = Updater.Path.PersitentPathForWWW + "/" + file;
                    break;
                default:
                    url = Updater.Path.PersitentPathForWWW + "/" + file;
                    break;
            }

            WWW www = new WWW(url);
            yield return www;
            OnWWW(loc, www, fileName);
        }

        internal void OnDownloadCompleted(string error,string fn)
        {
            if (string.IsNullOrEmpty(error))
                Updater.Coroutine(StartRead(Loc.Latest, fn));
            else
                OnDownloadError(error);
        }

        public abstract bool CheckDownloadFn(string fn);
    }
}
