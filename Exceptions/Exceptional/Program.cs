using System;
using System.Runtime.CompilerServices;

namespace Exceptional
{
    class Program
    {
        static void Main(string[] args)
        {
            CanThrowNonExceptionType();

            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }

        private static void CanThrowNonExceptionType()
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
