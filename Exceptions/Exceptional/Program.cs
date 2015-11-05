using System;

namespace Exceptional
{
    class Program
    {
        static void Main(string[] args)
        {
            Demoable[] demos = { new ThrowingAnything(), new ExceptionLogger(), new StackUnwinding() };

            foreach (var demo in demos)
            {
                demo.Execute();
            }

            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }
    }
}
