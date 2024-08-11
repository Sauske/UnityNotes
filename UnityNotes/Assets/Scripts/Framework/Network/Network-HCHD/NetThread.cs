using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace UMI.Net
{
    public class NetThread
    {
        private Thread m_thread;
        private bool isOver;
        private System.Object m_mutex;

        public void Run()
        {
            m_thread.Start(this);
        }

        protected static void ThreadProc(object obj)
        {
            NetThread me = (NetThread)obj;
            me.Main();
        }

        protected virtual void Main() { }

        public void WaitTermination()
        {
            m_thread.Join();
        }

        public void SetOver()
        {
            lock (m_mutex)
            {
                isOver = true;
            }
        }

        protected bool IsOver()
        {
            lock (m_mutex)
            {
                return isOver;
            }
        }

        public NetThread()
        {
            m_thread = new Thread(ThreadProc);
            m_thread.Priority = ThreadPriority.AboveNormal;
            isOver = false;
            m_mutex = new System.Object();
        }
    }
}
