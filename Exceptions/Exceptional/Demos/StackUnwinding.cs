using System;
using System.Diagnostics;

namespace Exceptional.Demos
{
    class StackUnwinding : Demoable
    {
        protected override string Description => "Stack Unwinding";

        protected override string VisualStudioConfigurationRequired => "make sure that your Visual Studio is set up to break only on uncaught exceptions! (CTRL + ALT + E)";

        protected override void RunDemo()
        {
            try
            {
                PutSomeLocalVariablesOnStackAndThrow();
            }
            catch (Exception anyException) when(JustTakeALook(anyException))
            {
                // stack unwinding happens here! 
                // you can't see PutSomeLocalVariablesOnStackAndThrow on call stack neither check the value of somethingImportant

                throw; 
            }
        }

        void PutSomeLocalVariablesOnStackAndThrow()
        {
            var somethingImportant = "you should know this, because it it the reason of failure";

            if (!somethingImportant.Contains("just Make sure that compiler does not remove this local variable"))
            {
                throw new Exception("something bad has happened, no more details");
            }
        }

        bool JustTakeALook(Exception anyException)
        {
            // take a look at the call stack window
            // you can see PutSomeLocalVariablesOnStackAndThrow on stack and the value of somethingImportant
            Debugger.Break(); 

            return true;
        }
    }
}