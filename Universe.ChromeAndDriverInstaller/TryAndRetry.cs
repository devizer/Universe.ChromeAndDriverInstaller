using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Universe.ChromeAndDriverInstaller
{
    public static class TryAndRetry
    {
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
    }
}
