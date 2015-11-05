using System;

namespace Exceptional.Demos
{
    class ExceptionHierarchyAndOrderMatters : Demoable
    {
        protected override string Description => "Exception's Hierarchy & Order matters in catch block";

        protected override void RunDemo()
        {
            try
            {
                throw new SpecificException("which catch handler is going to be invoked?");
            }
            catch (GeneralException)
            {
                // ??
            }
            catch (SpecificException)
            {
                // ??
            }
            catch (Exception)
            {
                // ??
            }
        }

        private class GeneralException : Exception { }

        private class SpecificException : Exception
        {
            public SpecificException(string message) : base(message)
            {
            }
        }
    }
}