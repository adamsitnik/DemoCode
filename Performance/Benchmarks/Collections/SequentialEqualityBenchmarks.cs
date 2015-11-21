using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        // we compare array with it's own copy just to measure the pessimistic scenario - full range check required
        private byte[] Values, SameValues;

        [Setup]
        public void SetupData()
        {
            Values = Enumerable.Range(0, ItemsCount).Select(number => (byte)number).ToArray();
            SameValues = Values.ToArray();
        }

        [Benchmark]
        public bool ObjectEquals()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (!object.Equals(Values[i], SameValues[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool BytesEqual()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (!Values[i].Equals(SameValues[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool IEqutableGenericConstraint()
        {
            return CompareEqutable(Values, SameValues);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool CompareEqutable<T>(T[] left, T[] right)
            where T : IEquatable<T>
        {
            for (int i = 0; i < left.Length; i++)
            {
                if (!left[i].Equals(right[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool IEqutableGenericStructConstraint()
        {
            return CompareEqutableStructs(Values, SameValues);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool CompareEqutableStructs<T>(T[] left, T[] right)
            where T : struct, IEquatable<T>
        {
            for (int i = 0; i < left.Length; i++)
            {
                if (!left[i].Equals(right[i]))
                {
                    return false;
                }
            }
            return true;
        }


        [Benchmark]
        public bool EqualsOperator()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] != SameValues[i])
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
            for (int i = 0; i < Values.Length; i++)
            {
                if (!nonBoxingComparer.Equals(Values[i], SameValues[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Benchmark]
        public bool EnumerableSequenceEquals()
        {
            return Enumerable.SequenceEqual(Values, SameValues);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int memcmp(byte* first, byte* second, int count);

        [Benchmark]
        public bool MemCmp()
        {
            unsafe
            {
                fixed (byte* first = Values, second = SameValues)
                {
                    return memcmp(first, second, Values.Length) == 0;
                }
            }
        }

        [Benchmark]
        public bool Unsafe()
        {
            return UnsafeCompare(Values, SameValues);
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

        [Benchmark]
        public bool LikeStringEqualsHelper()
        {
            unsafe
            {
                fixed (byte* pinnedFirst = Values, pinnedSecond = SameValues)
                {
                    byte* firstPointer = pinnedFirst;
                    byte* secondPointer = pinnedSecond;

                    long bytesCount = Values.Length;

                    while (bytesCount >= 20)
                    {
                        if (*(int*)firstPointer != *(int*)secondPointer)
                            break;
                        if (*(int*)(firstPointer + 4) != *(int*)(secondPointer + 4))
                            break;
                        if (*(int*)(firstPointer + 8) != *(int*)(secondPointer + 8))
                            break;
                        if (*(int*)(firstPointer + 12) != *(int*)(secondPointer + 12))
                            break;
                        if (*(int*)(firstPointer + 16) != *(int*)(secondPointer + 16))
                            break;
                        firstPointer += 20;
                        secondPointer += 20;
                        bytesCount -= 20;
                    }
                    while (bytesCount > 0)
                    {
                        if (*firstPointer != *secondPointer)
                            break;
                        ++firstPointer;
                        ++secondPointer;
                        --bytesCount;
                    }

                    return bytesCount == 0;
                }
            }
        }

        /// <summary>
        /// this method = idea from String.EqualsHelper + hints from Vladimir Sadov + some improvements by Adam Sitnik
        /// </summary>
        [Benchmark]
        public bool OptimizedHybrid()
        {
            unsafe
            {
                fixed (byte* pinnedFirst = Values, pinnedSecond = SameValues)
                {
                    byte* firstPointer = pinnedFirst;
                    byte* secondPointer = pinnedSecond;

                    int step = sizeof(void*) * 5;
                    byte* firstPointerLimit = firstPointer + (Values.Length - step);

                    if (step < Values.Length)
                    {
                        while (firstPointer < firstPointerLimit)
                        {
                            if (*((void**)firstPointer + 0) != *((void**)secondPointer + 0)) break;
                            if (*((void**)firstPointer + 1) != *((void**)secondPointer + 1)) break;
                            if (*((void**)firstPointer + 2) != *((void**)secondPointer + 2)) break;
                            if (*((void**)firstPointer + 3) != *((void**)secondPointer + 3)) break;
                            if (*((void**)firstPointer + 4) != *((void**)secondPointer + 4)) break;

                            firstPointer += step;
                            secondPointer += step;
                        }
                        if (firstPointer < firstPointerLimit) // the upper loop ended with break;
                        {
                            return false;
                        }
                    }
                    firstPointerLimit += step;
                    while (firstPointer < firstPointerLimit)
                    {
                        if (*firstPointer != *secondPointer) break;
                        
                        ++firstPointer;
                        ++secondPointer;
                    }

                    return firstPointer == firstPointerLimit;
                }
            }
        }
    }
}