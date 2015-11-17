using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarks.Math
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class ModuloOperatorBenchmarks
    {
        private int a = 100, b = 200, c = 300, d = 400, e = 100, f = 200, g = 300, h = 400;

        [Benchmark]
        [OperationsPerInvoke(4)]
        public void BuiltInOperator()
        {
            a = a % 2;
            b = b % 4;
            c = c % 8;
            d = d % 16;
        }

        [Benchmark]
        [OperationsPerInvoke(4)]
        public void Alternative()
        {
            e = e & 0x00000001;
            f = f & 0x00000003;
            g = g & 0x00000007;
            h = h & 0x0000000F;
        }
    }
}