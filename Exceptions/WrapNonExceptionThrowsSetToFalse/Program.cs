using System;
using System.Runtime.CompilerServices;
using Exceptional;

namespace WrapNonExceptionThrowsSetToFalse
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
                Console.WriteLine(
                    "It is not going to get here with current settings (AssemblyInfo -> [assembly: RuntimeCompatibility(WrapNonExceptionThrows = false)]");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Not today");
            }
            catch
            {
                Console.WriteLine("When wrapping is disabled, this is the only way to catch non-CLS-compilant exceptions");
            }
        }
    }
}
