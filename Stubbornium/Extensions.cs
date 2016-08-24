using System;
using System.Threading;

namespace Stubbornium
{
    internal static class Extensions
    {
        public static void DefaultWait()
        {
            Thread.Sleep(WaitTime.Short.ToTimeSpan());
        }

        public static void Repeat(Action seleniumAction, Action wait = null)
        {
            Repeat(() =>
            {
                seleniumAction();
                return true;
            }, wait);
        }

        public static bool WaitFor(Func<bool> seleniumAction, Action wait = null)
        {
            if (wait == null)
            {
                wait = DefaultWait;               
            }

            int attemptNo = 0;
            bool result = false;
            while (!result)
            {
                try
                {
                    result = seleniumAction();
                }
                catch (Exception)
                {
                    result = false;
                }

                if (result)
                    return true;

                if (attemptNo >= 10)
                    return false;

                attemptNo++;
                try
                {
                    wait();
                }
                catch (Exception)
                {
                    // Ignore wait errors - just try to perform the core action again
                }
            }

            return result;
        }

        public static T Repeat<T>(Func<T> action, Action wait = null, int maxRetries = 10)
        {
            if (wait == null)
            {
                wait = DefaultWait;
            }

            int attemptNo = 0;
            while (true)
            {
                try
                {
                    return action();
                }
                catch (Exception)
                {
                    if (attemptNo >= maxRetries)
                        throw;

                    attemptNo++;
                    try
                    {
                        wait();
                    }
                    catch (Exception)
                    {
                        // Ignore wait errors - just try to perform the core action again
                    }
                }
            }
        }      
    }
}