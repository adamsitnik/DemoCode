using BenchmarkDotNet;
using Benchmarks.Collections;

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
                    typeof(LengthForLoopBenchmarks)
                });

            competitionSwitch.Run(args);
        }
    }
}
