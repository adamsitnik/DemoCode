using System.Linq;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Collections
{
    /// <summary>
    /// the idea behind this benchmark is to test how reducing the amount of arithmetic operations will affect perf
    /// x86 jit does produce such asm:
    /// 
    /// if (*((void**)firstPointer + 3) != *((void**)secondPointer + 3)) break;
    /// 007F1792  mov         eax,dword ptr [ebp-10h]  
    /// 007F1795  mov         edx,3  
    /// 007F179A  mov         ecx,4  
    /// 007F179F  imul        edx,ecx  
    /// 007F17A2  mov         eax,dword ptr [eax+edx]  
    /// 007F17A5  mov         edx,dword ptr [ebp-14h]  
    /// 007F17A8  mov         ecx,3  
    /// 007F17AD  mov         ebx,4  
    /// 007F17B2  imul        ecx,ebx  
    /// 007F17B5  cmp         eax,dword ptr [edx+ecx]  
    /// 007F17B8  jne         007F17F0  
    /// 
    /// and we want to test it against more simple one
    /// 
    /// if (*(int*)(firstPointer + 12) != *(int*)(secondPointer + 12)) break;
    /// 012D426F  mov         eax,dword ptr [ebp-0Ch]  
    /// 012D4272  mov         eax,dword ptr [eax+0Ch]  
    /// 012D4275  mov         edx,dword ptr [ebp-10h]  
    /// 012D4278  cmp         eax,dword ptr [edx+0Ch]  
    /// 012D427B  jne         012D429B  
    /// </summary>
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    public class MemCmpHybridsBenchmarks
    {
        //[Params(0, 1, 10, 100, 1000, 100000, 1000000)]
        public int ItemsCount = 1000;

        // we compare array with it's own copy just to measure the pessimistic scenario - full range check required
        private byte[] Values, SameValues;

        [Setup]
        public void SetupData()
        {
            Values = Enumerable.Range(0, ItemsCount).Select(number => (byte)number).ToArray();
            SameValues = Values.ToArray();
        }

        [Benchmark]
        public bool InitialHybrid()
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

        [Benchmark]
        public bool HybridWithLessArithmeticOperations()
        {
            unsafe
            {
                fixed (byte* pinnedFirst = Values, pinnedSecond = SameValues)
                {
                    byte* firstPointer = pinnedFirst;
                    byte* secondPointer = pinnedSecond;

                    const int step = 20;
                    byte* firstPointerLimit = firstPointer + (Values.Length - step);

                    if (step < Values.Length)
                    {
                        while (firstPointer < firstPointerLimit)
                        {
                            if (*(int*)(firstPointer + 0) != *(int*)(secondPointer + 0)) break;
                            if (*(int*)(firstPointer + 4) != *(int*)(secondPointer + 4)) break;
                            if (*(int*)(firstPointer + 8) != *(int*)(secondPointer + 8)) break;
                            if (*(int*)(firstPointer + 12) != *(int*)(secondPointer + 12)) break;
                            if (*(int*)(firstPointer + 16) != *(int*)(secondPointer + 16)) break;

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
                        if (*firstPointer != *secondPointer) return false;

                        ++firstPointer;
                        ++secondPointer;
                    }

                    return firstPointer == firstPointerLimit;
                }
            }
        }
    }
}