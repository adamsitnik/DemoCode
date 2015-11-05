using System;

namespace Exceptional
{
    class ExceptionLogger
    {
        readonly Action<string> _log = Console.WriteLine;

        internal void Execute(Action command)
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
            _log($"Exception occured, details: {ex.Message}");

            return false;
        }
    }
}