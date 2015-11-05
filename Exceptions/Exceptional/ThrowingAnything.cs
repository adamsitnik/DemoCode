using System;
using System.Runtime.CompilerServices;

namespace Exceptional
{
    class ThrowingAnything : Demoable
    {
        protected override string Description => "Throwing anything";

        protected override void RunDemo()
        {
            try
            {
                Action<string> throwingMethod = ThrowAnythingMethodBuilder.BuildThrowingMethod<string>();

                throwingMethod.Invoke("I can throw the string");
            }
            catch (RuntimeWrappedException wrappedException)
            {
                Console.WriteLine(wrappedException.WrappedException);
            }
        }
    }
}