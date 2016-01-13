using System;
using System.Diagnostics;

namespace Exceptional
{
    abstract class Demoable
    {
        protected abstract string Description { get; }

        protected virtual string VisualStudioConfigurationRequired => string.Empty;

        private bool IsVisualStudioConfigurationRequired => !string.IsNullOrEmpty(VisualStudioConfigurationRequired);

        protected abstract void RunDemo();

        internal void Execute()
        {
            if (IsVisualStudioConfigurationRequired)
            {
                Console.WriteLine(VisualStudioConfigurationRequired);
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