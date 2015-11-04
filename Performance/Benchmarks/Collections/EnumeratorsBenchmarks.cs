using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Collections
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class EnumeratorsBenchmarks
    {
        private const int NumbersCount = 1000;
        // each benchmarks has it's own data just to avoid better results for Benchmarks that got the CPU cached warmed up by previous benchmarks
        private static readonly int[] NumbersForSingleStructEnumeratorCalledDirectly = Enumerable.Range(1, NumbersCount).ToArray();
        private static readonly int[] NumbersForSingleStructEnumeratorCalledViaInterface = Enumerable.Range(1, NumbersCount).ToArray();
        private static readonly int[] NumbersForSingleClassEnumeratorCalledDirectly = Enumerable.Range(1, NumbersCount).ToArray();
        private static readonly int[] NumbersForSingleClassEnumeratorCalledViaInterface = Enumerable.Range(1, NumbersCount).ToArray();
        private static readonly int[] NumbersForDedicatedEnumeratorCalledDirectly = Enumerable.Range(1, NumbersCount).ToArray();
        private static readonly int[] NumbersForDedicatedClassEnumeratorCalledViaInterface = Enumerable.Range(1, NumbersCount).ToArray();

        [Benchmark]
        public int SingleStructEnumeratorCalledDirectly()
        {
            int sum = 0;
            SingleStructEnumerator<int> enumerator = new SingleStructEnumerator<int>(NumbersForSingleStructEnumeratorCalledDirectly);
            while (enumerator.MoveNext())
            {
                sum += enumerator.Current;
            }
            return sum;
        }

        [Benchmark]
        public int SingleStructEnumeratorCalledViaInterface()
        {
            int sum = 0;
            using (IEnumerator<int> enumerator = new SingleStructEnumerator<int>(NumbersForSingleStructEnumeratorCalledViaInterface))
            {
                while (enumerator.MoveNext())
                {
                    sum += enumerator.Current;
                }
            }
            return sum;
        }

        [Benchmark]
        public int SingleClassEnumeratorCalledDirectly()
        {
            int sum = 0;
            SingleClassEnumerator<int> enumerator = new SingleClassEnumerator<int>(NumbersForSingleClassEnumeratorCalledDirectly);
            while (enumerator.MoveNext())
            {
                sum += enumerator.Current;
            }
            return sum;
        }

        [Benchmark]
        public int SingleClassEnumeratorCalledViaInterface()
        {
            int sum = 0;
            using (IEnumerator<int> enumerator = new SingleClassEnumerator<int>(NumbersForSingleClassEnumeratorCalledViaInterface))
            {
                while (enumerator.MoveNext())
                {
                    sum += enumerator.Current;
                }
            }
            return sum;
        }

        [Benchmark]
        public int DedicatedStructEnumeratorCalledDirectly()
        {
            int sum = 0;
            DedicatedStructEnumerator<int> enumerator = new DedicatedStructEnumerator<int>(NumbersForDedicatedEnumeratorCalledDirectly);
            while (enumerator.MoveNext())
            {
                sum += enumerator.Current;
            }
            return sum;
        }

        [Benchmark]
        public int DedicatedClassEnumeratorCalledViaInterface()
        {
            int sum = 0;
            using (IEnumerator<int> enumerator = new DedicatedClassEnumerator<int>(NumbersForDedicatedClassEnumeratorCalledViaInterface))
            {
                while (enumerator.MoveNext())
                {
                    sum += enumerator.Current;
                }
            }
            return sum;
        }

        public struct SingleStructEnumerator<T> : IEnumerator<T>
        {
            private readonly T[] _wrappedArray;
            private int _index;

            public SingleStructEnumerator(T[] wrappedArray) : this() { _wrappedArray = wrappedArray; }

            public bool MoveNext() { return ++_index < _wrappedArray.Length; }
            public T Current { get { return _wrappedArray[_index]; } }
            object IEnumerator.Current { get { return Current; } }
            public void Reset() { }
            public void Dispose() { }
        }

        public class SingleClassEnumerator<T> : IEnumerator<T>
        {
            private readonly T[] _wrappedArray;
            private int _index;

            public SingleClassEnumerator(T[] wrappedArray) { _wrappedArray = wrappedArray; }

            public bool MoveNext() { return ++_index < _wrappedArray.Length; }
            public T Current { get { return _wrappedArray[_index]; } }
            object IEnumerator.Current { get { return Current; } }
            public void Reset() { }
            public void Dispose() { }
        }

        public struct DedicatedStructEnumerator<T>
        {
            private readonly T[] _wrappedArray;
            private int _index;

            public DedicatedStructEnumerator(T[] wrappedArray) : this() { _wrappedArray = wrappedArray; }

            public T Current { get { return _wrappedArray[_index]; } }
            public bool MoveNext() { return ++_index < _wrappedArray.Length; }
        }

        public class DedicatedClassEnumerator<T> : IEnumerator<T>
        {
            private readonly T[] _wrappedArray;
            private int _index;

            public DedicatedClassEnumerator(T[] wrappedArray) { _wrappedArray = wrappedArray; }

            public bool MoveNext() { return ++_index < _wrappedArray.Length; }
            public T Current { get { return _wrappedArray[_index]; } }
            object IEnumerator.Current { get { return Current; } }
            public void Reset() { }
            public void Dispose() { }
        }
    }
}
