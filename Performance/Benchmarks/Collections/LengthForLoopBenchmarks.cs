using System.Runtime.CompilerServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Collections
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class LengthForLoopBenchmarks
    {
        private const int NumbersCount = 1000;

        [Benchmark]
        public int PublicReadOnlyField()
        {
            int sum = 0;
            var source = new LengthAsPublicReadOnlyField(NumbersCount);
            for (int i = 0; i < source.Length; i++)
            {
                sum += i;
            }
            return sum;
        }

        [Benchmark]
        public int PublicReadOnlyProperty()
        {
            int sum = 0;
            var source = new LengthAsPublicReadOnlyProperty(NumbersCount);
            for (int i = 0; i < source.Length; i++)
            {
                sum += i;
            }
            return sum;
        }

        [Benchmark]
        public int PublicReadOnlyInlineableProperty()
        {
            int sum = 0;
            var source = new LengthAsPublicInlineableReadOnlyProperty(NumbersCount);
            for (int i = 0; i < source.Length; i++)
            {
                sum += i;
            }
            return sum;
        }

        [Benchmark]
        public int PublicGetPropertyWithPrivateSet()
        {
            int sum = 0;
            var source = new LengthAsPublicGetPropertyWithPrivateSet(NumbersCount);
            for (int i = 0; i < source.Length; i++)
            {
                sum += i;
            }
            return sum;
        }

        private struct LengthAsPublicReadOnlyField
        {
            public readonly int Length;

            public LengthAsPublicReadOnlyField(int length)
            {
                Length = length;
            }
        }

        private struct LengthAsPublicReadOnlyProperty
        {
            private readonly int _length;

            public int Length
            {
                get { return _length; }
            }

            public LengthAsPublicReadOnlyProperty(int length)
            {
                _length = length;
            }
        }

        private struct LengthAsPublicInlineableReadOnlyProperty
        {
            private readonly int _length;

            public int Length
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return _length; }
            }

            public LengthAsPublicInlineableReadOnlyProperty(int length)
            {
                _length = length;
            }
        }

        private struct LengthAsPublicGetPropertyWithPrivateSet
        {
            public int Length { get; private set; }

            public LengthAsPublicGetPropertyWithPrivateSet(int length) : this()
            {
                Length = length;
            }
        }
    }
}