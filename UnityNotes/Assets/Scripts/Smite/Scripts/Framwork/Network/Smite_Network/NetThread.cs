//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;

namespace Framework
{    
    public class NetThread 
    {
        private Thread _thread;
        private bool _isOver;
        private System.Object _mutex;
        
        public void Run()
        {
            _thread.Start(this);
        }

        protected static void ThreadProc(object _object)
        {
            try
            {
                NetThread me = (NetThread)_object;
                me.Main();
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);

                NetThread me = (NetThread)_object;
                me.SetOver();
                me.EnqueueLog(ex.Message);

                me._thread.Join();
            }
        }

        protected virtual void Main() { }

        public void WaitTermination()
        {
            _thread.Join();
        }

        public void SetOver()
        {
            lock(_mutex)
            {
                _isOver = true;
            }
        }

        public bool IsOver()
        {
            lock(_mutex)
            {
                return _isOver;
            }
        }

        public NetThread()
        {
            _thread = new Thread(ThreadProc);
            _thread.Priority = ThreadPriority.AboveNormal;
            _isOver = false;
            _mutex = new System.Object();
        }

        public virtual void EnqueueLog(string log)
        {

        }
    }
}