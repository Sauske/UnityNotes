using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResUpdater
{
    /// <summary>
    /// 更新类型
    /// </summary>
    public enum UpdateType
    {
        App,
        Res,
    }
    public class UpdaterTypeVersion
	{
        public UpdateVersion StreamVersion { get; private set; }

        public UpdateVersion PersistentVersion { get; private set; }

        public UpdateVersion LatestVersion { get; private set; }

        public UpdateVersion LocalVersion { get; private set; }

        public void SetVersion(Loc loc,UpdateVersion versoin)
        {
            switch(loc)
            {
                case Loc.Latest:
                    LatestVersion = versoin;
                    break;
                case Loc.Stream:
                    StreamVersion = versoin;
                    break;
                case Loc.Persistent:
                    PersistentVersion = versoin;
                    break;
            }
            
            if(StreamVersion != null && PersistentVersion != null && LatestVersion != null)
            {
                LocalVersion = UpdateVersion.New(StreamVersion, PersistentVersion);
            }
        }


        public bool IsLoadFinish()
        {
            return LocalVersion != null && LatestVersion != null;
        }

        /// <summary>
        /// 是否需要更新
        /// </summary>
        /// <returns></returns>
        public bool IsNeedUpdate()
        {
            // 最新版本不等于streaming，并且大于，需要更新
            return !LatestVersion.Equals(StreamVersion) && LatestVersion.IsMax(LocalVersion);
        }
    }

    public class UpdateVersion
    {
        //主版本号
        public int MajorVersion = 0;

        //次本版号
        public int MinorVersion = 0;

        //修订号
        public int PatchVersion = 0;

        //构建版本号
        public int BuildVersion = 0;

        /// <summary>
        /// 默认构造
        /// </summary>
        public UpdateVersion() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="versionString"></param>
        public UpdateVersion(string versionString)
        {
            var vers = versionString.Split('.');
            if(vers.Length == 4)
            {
                MajorVersion = int.Parse(vers[0]);
                MinorVersion = int.Parse(vers[1]);
                PatchVersion = int.Parse(vers[2]);
                BuildVersion = int.Parse(vers[3]);
            }
        }

        /// <summary>
        /// 是否为空版本
        /// </summary>
        /// <returns></returns>
        public bool IsNull()
        {
            return MajorVersion == 0 && MinorVersion == 0 && PatchVersion == 0 && BuildVersion == 0;
        }

        /// <summary>
        /// 完整版本号
        /// </summary>
        public string FinalVersion
        {
            get { return string.Format("{0}.{1}.{2}.{3}", MajorVersion, MinorVersion, PatchVersion, BuildVersion); }
        }

        /// <summary>
        /// 返回更新的版本
        /// </summary>
        /// <param name="version"></param>
        /// <param name="version1"></param>
        /// <returns></returns>
        public static UpdateVersion New(UpdateVersion versionLeft,UpdateVersion versionRight)
        {
            if(versionLeft.MajorVersion != versionRight.MajorVersion)
            {
                return versionLeft.MajorVersion > versionRight.MajorVersion ? versionLeft : versionRight;
            }
            else
            {
                if(versionLeft.MinorVersion != versionRight.MinorVersion)
                {
                    return versionLeft.MinorVersion > versionRight.MinorVersion ? versionLeft : versionRight;
                }
                else
                {
                    if(versionLeft.PatchVersion != versionRight.PatchVersion)
                    {
                        return versionLeft.PatchVersion > versionRight.PatchVersion ? versionLeft : versionRight;
                    }
                    else
                    {
                        if(versionLeft.BuildVersion != versionRight.BuildVersion)
                        {
                            return versionLeft.BuildVersion > versionRight.BuildVersion ? versionLeft : versionRight;
                        }
                        else
                        {
                            return versionLeft;
                        }
                    }
                }
            }
        }


        public bool Equals(UpdateVersion ver)
        {
            return ver.MajorVersion == MajorVersion 
                && ver.MinorVersion == MinorVersion 
                && ver.PatchVersion == PatchVersion 
                && ver.BuildVersion == BuildVersion;
        }


        public bool IsMax(UpdateVersion ver)
        {
            if (MajorVersion == ver.MajorVersion)
            {
                if (MinorVersion == ver.MinorVersion)
                {
                    if (PatchVersion == ver.PatchVersion)
                    {
                        if (BuildVersion == ver.BuildVersion)
                        {
                            return false;
                        }
                        else
                        {
                            return BuildVersion > ver.BuildVersion;
                        }
                    }
                    else
                    {
                        return PatchVersion > ver.PatchVersion;
                    }
                }
                else
                {
                    return MinorVersion > ver.MinorVersion;
                }
            }
            else
            {
                return MajorVersion > ver.MajorVersion;
            }
        }
    }
}
