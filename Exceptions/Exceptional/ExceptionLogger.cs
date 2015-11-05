using System;

namespace Exceptional
{
    class ExceptionLogger : Demoable
    {
        protected override string Description => "Filtering exception with when keyword";

        protected override void RunDemo()
        {
            Execute(() => { throw new Exception("Just log and forget"); });
        }

        void Execute(Action command)
        {
            try
            {
                command.Invoke();
            }
            catch (Exception ex) when (LogOnly(ex))
            {
                // unreachable
            }
        }

        bool LogOnly(Exception ex)
        {
            Console.WriteLine($"Exception occured, details: {ex.Message}");

            return false;
        }
    }
}