using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Collections
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class BoundariesCheckBenchmarks
    {
        private const int NumbersCount = 1000;
        // each benchmarks has it's own data just to avoid better results for Benchmarks that got the CPU cached warmed up by previous benchmarks
        private static readonly int[] NumbersForSingleCheck = Enumerable.Range(1, NumbersCount).ToArray();
        private static readonly int[] NumbersForDoubleCheck = Enumerable.Range(1, NumbersCount).ToArray();

        [Benchmark]
        public int SingleCheck()
        {
            int sum = 0;
            var wrapper = new SingleCheckArrayWrapper<int>(NumbersForSingleCheck);
            for (int i = 0; i < wrapper.Length; i++)
            {
                sum += wrapper[i];
            }
            return sum;
        }

        [Benchmark]
        public int DoubleCheck()
        {
            int sum = 0;
            var wrapper = new DoubleCheckArrayWrapper<int>(NumbersForDoubleCheck);
            for (int i = 0; i < wrapper.Length; i++)
            {
                sum += wrapper[i];
            }
            return sum;
        }

        private struct SingleCheckArrayWrapper<T>
        {
            private readonly T[] _array;
            public readonly int Length;

            public SingleCheckArrayWrapper(T[] array)
            {
                _array = array;
                Length = array.Length;
            }

            public T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    RequiresInRangeWithSingleComparison(index, Length);
                    return _array[index];
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void RequiresInRangeWithSingleComparison(int start, int length)
            {
                unchecked
                {
                    if ((uint)start >= (uint)length)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private struct DoubleCheckArrayWrapper<T>
        {
            private readonly T[] _array;
            public readonly int Length;

            public DoubleCheckArrayWrapper(T[] array)
            {
                _array = array;
                Length = array.Length;
            }

            public T this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    RequiresInRangeWithTwoComparisons(index, Length);
                    return _array[index];
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void RequiresInRangeWithTwoComparisons(int start, int length)
            {
                if (!(start >= 0 && start < length))
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}