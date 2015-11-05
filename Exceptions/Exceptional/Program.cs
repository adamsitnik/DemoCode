using System;
using Exceptional.Demos;

namespace Exceptional
{
    class Program
    {
        static void Main(string[] args)
        {
            Demoable[] demos =
            {
                new ThrowingAnything(),
                new SilentExceptionLogingWithWhen(),
                new StackUnwinding(),
                new ExceptionHierarchyAndOrderMatters(),
                new ExceptionInFinallyBlock()
            };

            foreach (var demo in demos)
            {
                demo.Execute();
            }

            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }
    }
}
