using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Common
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class IsValueTypeBenchmarks
    {
        private SomeGeneric<int> someGeneric;

        [Benchmark]
        public bool ReturnConstForComparison()
        {
            return true;
        }

        [Benchmark]
        public bool NoOptimizations()
        {
            return GetSomeTypeThatIsNotKnownDuringCompilation().IsValueType;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private Type GetSomeTypeThatIsNotKnownDuringCompilation()
        {
            return typeof(object);
        }

        [Benchmark]
        public bool CanInlineConstResultOfType()
        {
            return typeof(int).IsValueType;
        }

        [Benchmark]
        public bool GenericOfStruct_GetType_IsValueType()
        {
            return someGeneric.CallTypeOfIsValueType();
        }

        [Benchmark]
        public bool GenericOfStruct_CompareDefaultValueWithNull()
        {
            return someGeneric.CompareDefaultValueWithNull();
        }

        [Benchmark]
        public bool HelperClassWithCachedValue()
        {
            return Helper<int>.IsValueType;
        }

        private struct SomeGeneric<T>
        {
            internal bool CallTypeOfIsValueType()
            {
                return typeof(T).IsValueType;
            }

            internal bool CompareDefaultValueWithNull()
            {
                return default(T) == null;
            }
        }

        private class Helper<T>
        {
            internal static readonly bool IsValueType = typeof(T).IsValueType;
        }
    }
}