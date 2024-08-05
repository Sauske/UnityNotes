using System.Collections.Generic;

namespace ResUpdater
{
    public static class Res
	{
        public static bool useStreamBundleInfo = false;
        public static readonly HashSet<string> resourcesInStreamWhenNotUseStreamVersion = new HashSet<string>();
        public static bool shellAreadyUpdated = false;
    }
}
