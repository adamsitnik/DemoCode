using System;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Collections
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class MemCpy
    {
        [Params(0, 1, 10, 100, 1000, 100000, 1000000)]
        public int ItemsCount = 0;

        private byte[] Source, Target;

        [Setup]
        public void SetupData()
        {
            Source = Enumerable.Range(0, ItemsCount).Select(number => (byte)number).ToArray();
            Target = new byte[ItemsCount];
        }

        [Benchmark]
        public byte[] CopyingBytesInLoop()
        {
            for (int i = 0; i < Source.Length; i++)
            {
                Target[i] = Source[i];
            }
            return Target;
        }

        [Benchmark]
        public byte[] ArrayCopy()
        {
            Array.Copy(Source, Target, Source.Length);

            return Target;
        }

        [Benchmark]
        public byte[] BufferBlockCopy()
        {
            Buffer.BlockCopy(Source, 0, Target, 0, Source.Length);

            return Target;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void memcpy(byte* first, byte* second, int count);

        [Benchmark]
        public byte[] ExternMemCpy()
        {
            unsafe
            {
                fixed (byte* first = Source, second = Target)
                {
                    memcpy(first, second, Source.Length);
                }
            }
            return Target;
        }

        [Benchmark]
        public byte[] OptimizedHybrid()
        {
            unsafe
            {
                fixed (byte* pinnedSource = Source, pinnedTarget = Target)
                {
                    byte* sourcePointer = pinnedSource;
                    byte* targetPointer = pinnedTarget;

                    int step = sizeof(void*) * 4;
                    byte* firstPointerLimit = sourcePointer + (Source.Length - step);

                    if (step < Source.Length)
                    {
                        while (sourcePointer < firstPointerLimit)
                        {
                            var element = *((void**)sourcePointer + 0);
                            var nextElement = *((void**)sourcePointer + 1);

                            *((void**)targetPointer + 0) = element;
                            *((void**)targetPointer + 1) = nextElement;

                            element = *((void**)sourcePointer + 2);
                            nextElement = *((void**)sourcePointer + 3);

                            *((void**)targetPointer + 2) = element;
                            *((void**)targetPointer + 3) = nextElement;

                            sourcePointer += step;
                            targetPointer += step;
                        }
                    }
                    firstPointerLimit += step;
                    while (sourcePointer < firstPointerLimit)
                    {
                        *targetPointer = *sourcePointer;

                        ++sourcePointer;
                        ++targetPointer;
                    }

                    return Target;
                }
            }
        }
    }
}