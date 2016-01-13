using BenchmarkDotNet;
using Benchmarks.Collections;
using Benchmarks.Common;
using Benchmarks.Math;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var competitionSwitch = new BenchmarkSwitcher(
                new[]
                {
                    typeof(EnumeratorsBenchmarks),
                    typeof(BoundariesCheckBenchmarks),
                    typeof(LengthForLoopBenchmarks),
                    typeof(SequentialEqualityBenchmarks),
                    typeof(ModuloOperatorBenchmarks),
                    typeof(IsValueTypeBenchmarks),
                    typeof(MemCpy)
                });

            competitionSwitch.Run(args);
        }
    }
}
