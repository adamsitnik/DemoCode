using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Collections
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class SequentialEqualityBenchmarks
    {
        [Params(0, 1, 10, 100, 1000, 100000, 1000000)]
        public int ItemsCount = 0;

        // each benchmarks has it's own data just to avoid better results for Benchmarks that got the CPU cached warmed up by previous benchmarks
        // we compare array with it's own copy just to measure the pessimistic scenario - full range check required
        private byte[] ObjectEqualsData, ObjectEqualsCopy,
            ByteEqualsData, ByteEqualsDataCopy,
            EqualsOperatorData, EqualsOperatorDataCopy,
            DefaultComparerLoopData, DefaultComparerLoopDataCopy,
            EnumerableSequenceEqualsData, EnumerableSequenceEqualsDataCopy,
            MemCmpData, MemCmpDataCopy,
            UnsafeData, UnsafeDataCopy;

        [Setup]
        public void SetupData()
        {
            ObjectEqualsData = Enumerable.Range(0, ItemsCount).Select(number => (byte)number).ToArray();
            ObjectEqualsCopy = ObjectEqualsData.ToArray();
            ByteEqualsData = ObjectEqualsData.ToArray();
            ByteEqualsDataCopy = ObjectEqualsData.ToArray();
            EqualsOperatorData = ObjectEqualsData.ToArray();
            EqualsOperatorDataCopy = ObjectEqualsData.ToArray();
            DefaultComparerLoopData = ObjectEqualsData.ToArray();
            DefaultComparerLoopDataCopy = ObjectEqualsData.ToArray();
            EnumerableSequenceEqualsData = ObjectEqualsData.ToArray();
            EnumerableSequenceEqualsDataCopy = ObjectEqualsData.ToArray();
            MemCmpData = ObjectEqualsData.ToArray();
            MemCmpDataCopy = ObjectEqualsData.ToArray();
            UnsafeData = ObjectEqualsData.ToArray();
            UnsafeDataCopy = ObjectEqualsData.ToArray();
        }

        [Benchmark]
        public bool ObjectEquals()
        {
            for (int i = 0; i < ObjectEqualsData.Length; i++)
            {
                if (!object.Equals(ObjectEqualsData[i], ObjectEqualsCopy[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool BytesEqual()
        {
            for (int i = 0; i < ByteEqualsData.Length; i++)
            {
                if (!ByteEqualsData[i].Equals(ByteEqualsDataCopy[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool EqualsOperator()
        {
            for (int i = 0; i < EqualsOperatorData.Length; i++)
            {
                if (!EqualsOperatorData[i].Equals(EqualsOperatorDataCopy[i]))
                {
                    return false;
                }
            }
            return true;
        }


        [Benchmark]
        public bool DefaultComparer()
        {
            var nonBoxingComparer = EqualityComparer<byte>.Default;
            for (int i = 0; i < ByteEqualsData.Length; i++)
            {
                if (!nonBoxingComparer.Equals(DefaultComparerLoopData[i], DefaultComparerLoopDataCopy[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool EnumerableSequenceEquals()
        {
            return Enumerable.SequenceEqual(EnumerableSequenceEqualsData, EnumerableSequenceEqualsDataCopy);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int memcmp(byte* first, byte* second, int count);

        [Benchmark]
        public bool MemCmp()
        {
            unsafe
            {
                fixed (byte* first = MemCmpData, second = MemCmpDataCopy)
                {
                    return memcmp(first, second, MemCmpData.Length) == 0;
                }
            }
        }

        [Benchmark]
        public bool Unsafe()
        {
            return UnsafeCompare(UnsafeData, UnsafeDataCopy);
        }

        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        private static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;

            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2))
                        return false;
                if ((l & 4) != 0)
                {
                    if (*((int*)x1) != *((int*)x2)) return false;
                    x1 += 4;
                    x2 += 4;
                }
                if ((l & 2) != 0)
                {
                    if (*((short*)x1) != *((short*)x2)) return false;
                    x1 += 2;
                    x2 += 2;
                }
                if ((l & 1) != 0)
                    if (*((byte*)x1) != *((byte*)x2))
                        return false;

                return true;
            }
        }
    }
}