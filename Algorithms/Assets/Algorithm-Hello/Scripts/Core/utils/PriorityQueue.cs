
using System.Collections.Generic;

namespace hello_algo.utils
{
    public class PriorityQueue<T, K> : Queue<T>
    {
        public PriorityQueue() { }

        public PriorityQueue(List<T> _list)
        {

        }

        public PriorityQueue(T[] _arr)
        {

        }

        public PriorityQueue(PriorityQueue<T, K> queue)
        {

        }

        public void Enqueue(T Tval,K Kval)
        {

        }

        public bool TryDequeue(out T Tval,K Kval)
        {
            Tval = default(T);
            return false;
        }
    }
}
