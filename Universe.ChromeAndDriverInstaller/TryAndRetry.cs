using System;
using System.Threading;

namespace Universe.ChromeAndDriverInstaller
{
    public static class TryAndRetry
    {
        public static T Eval<T>(Func<T> func, T valueOnFail, int retryCount)
        {
            for (int i = 1; i <= retryCount; i++)
            {
                try
                {
                    return func();
                }
                catch
                {
                    Thread.Sleep(i);
                }
            }
            return valueOnFail;
        }

        public static T Eval<T>(Func<T> func, T valueOnFail)
        {
            try
            {
                return func();
            }
            catch
            {
                return valueOnFail;
            }
        }
        public static T Eval<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch
            {
                return default(T);
            }
        }
        public static Exception Exec(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch(Exception ex)
            {
                return ex;
            }
        }
    }
}
