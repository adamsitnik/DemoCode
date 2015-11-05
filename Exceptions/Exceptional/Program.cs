using System;
using System.Runtime.CompilerServices;

namespace Exceptional
{
    class Program
    {
        static void Main(string[] args)
        {
            //CanThrowNonExceptionType();
            StackUnwinding();
            //new ExceptionLogger().Execute(() => { throw new Exception("Just log and forget"); });

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

        private static void StackUnwinding()
        {
            new StackUnwinding().Present();
        }
    }
}
