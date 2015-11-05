using System;
using System.Diagnostics;

namespace Exceptional.Demos
{
    class ExceptionInFinallyBlock : Demoable
    {
        protected override string Description => "What happens if a finally block throws an exception?";

        protected override void RunDemo()
        {
            try
            {
                try
                {
                    throw new Exception("Initial");
                }
                finally
                {
                    throw new Exception("thrown in finally block");
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();

                Console.WriteLine(ex.Message);
            }
        }
    }
}