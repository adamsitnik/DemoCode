using HardwareIntrinsics.RayTracer;

namespace ProfilerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var sut = new SoA();

            for (int i = 0; i < 500; i++)
            {
                sut.Render();
            }
        }
    }
}
