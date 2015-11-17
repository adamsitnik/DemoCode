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
            var competitionSwitch = new BenchmarkCompetitionSwitch(
                new[]
                {
                    typeof(EnumeratorsBenchmarks),
                    typeof(BoundariesCheckBenchmarks),
                    typeof(LengthForLoopBenchmarks),
                    typeof(SequentialEqualityBenchmarks),
                    typeof(ModuloOperatorBenchmarks),
                    typeof(IsValueTypeBenchmarks)
                });

            competitionSwitch.Run(args);
        }
    }
}
