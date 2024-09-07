using System.Security.Cryptography;
using UMI;


namespace UMI
{
    public class UMIEventMgr : Singleton<UMIEventMgr>
    {
        private static UMIEventDispatch mView;
        public static UMIEventDispatch View { get { return mView; } }

        private static UMIEventDispatch mLogic;
        public static UMIEventDispatch Logic { get { return mLogic; } }

        private static UMIEventDispatch mNet;
        public static UMIEventDispatch Net { get { return mNet; } }

        protected override void OnInit()
        {
            mView = new UMIEventDispatch();
            mLogic = new UMIEventDispatch();
            mNet = new UMIEventDispatch();
        }
    }
}