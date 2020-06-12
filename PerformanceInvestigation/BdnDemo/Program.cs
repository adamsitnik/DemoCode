using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using HardwareIntrinsics.RayTracer;

namespace BdnDemo
{
    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }

    public class Benchmarks
    {
        SoA _soa = new SoA();

        [Benchmark]
        public void Render() => _soa.Render();
    }
}
