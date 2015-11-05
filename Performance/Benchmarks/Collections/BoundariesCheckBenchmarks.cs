using System;
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
        [Benchmark]
        public void SingleCheck()
        {
            for (int i = 0; i < 100; i++)
            {
                RequiresInRangeWithSingleComparison(i, 100);
            }
        }

        [Benchmark]
        public void DoubleCheck()
        {
            for (int i = 0; i < 100; i++)
            {
                RequiresInRangeWithTwoComparisons(i, 100);
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