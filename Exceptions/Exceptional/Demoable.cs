using System;
using System.Diagnostics;

namespace Exceptional
{
    abstract class Demoable
    {
        protected abstract string Description { get; }

        protected virtual bool IsVisualStudioConfigurationRequired => false;

        protected abstract void RunDemo();

        internal void Execute()
        {
            if (IsVisualStudioConfigurationRequired)
            {
                Debugger.Break(); // configure you VS now
            }

            Console.WriteLine("---------------");
            Console.WriteLine(Description);

            try
            {
                Debugger.Break();

                RunDemo();
            }
            catch
            {
            }

            Console.WriteLine("---------------");
        }
    }
}