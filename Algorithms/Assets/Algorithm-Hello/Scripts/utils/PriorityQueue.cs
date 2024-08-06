using System;
using System.Collections.Generic;

namespace hello_algo.utils
{
    public class PriorityQueue<TElement, TPriority>:Queue<TElement>
    {
        private (TElement Element, TPriority Priority)[] _nodes;
        private readonly IComparer<TPriority>? _comparer;

        private int _size;
        private int _version;
        private const int Arity = 4;
        private const int Log2Arity = 2;

        public PriorityQueue()
        {
            _nodes = Array.Empty<(TElement, TPriority)>();
            _comparer = InitializeComparer(null);
        }

        public PriorityQueue(int initialCapacity): this(initialCapacity, comparer: null)
        {

        }

        public PriorityQueue(IComparer<TPriority>? comparer)
        {
            _nodes = Array.Empty<(TElement, TPriority)>();
            _comparer = InitializeComparer(comparer);
        }

        public PriorityQueue(int initialCapacity, IComparer<TPriority>? comparer)
        {
            //ArgumentOutOfRangeException.ThrowIfNegative(initialCapacity);

            _nodes = new (TElement, TPriority)[initialCapacity];
            _comparer = InitializeComparer(comparer);
        }

        public PriorityQueue(IEnumerable<(TElement Element, TPriority Priority)> items, IComparer<TPriority>? comparer)
        {
           // ArgumentNullException.ThrowIfNull(items);

            //_nodes = EnumerableHelpers.ToArray(items, out _size);
            _comparer = InitializeComparer(comparer);

            if (_size > 1)
            {
               // Heapify();
            }
        }

        public PriorityQueue(IEnumerable<(TElement Element, TPriority Priority)> items): this(items, comparer: null)
        {
        }

        public PriorityQueue(List<TElement> _list)
        {

        }

        public PriorityQueue(TElement[] _arr)
        {

        }

        public PriorityQueue(PriorityQueue<TElement, TPriority> queue)
        {

        }

        public void Enqueue(TElement Tval, TPriority Kval)
        {

        }

        public bool TryDequeue(out TElement Tval, TPriority Kval)
        {
            Tval = default(TElement);
            return false;
        }

        private static IComparer<TPriority>? InitializeComparer(IComparer<TPriority>? comparer)
        {
            if (typeof(TPriority).IsValueType)
            {
                if (comparer == Comparer<TPriority>.Default)
                {
                    // if the user manually specifies the default comparer,
                    // revert to using the optimized path.
                    return null;
                }

                return comparer;
            }
            else
            {
                // Currently the JIT doesn't optimize direct Comparer<T>.Default.Compare
                // calls for reference types, so we want to cache the comparer instance instead.
                // TODO https://github.com/dotnet/runtime/issues/10050: Update if this changes in the future.
                return comparer ?? Comparer<TPriority>.Default;
            }
        }
    }
}
