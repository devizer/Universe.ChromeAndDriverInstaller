using System;
using System.Diagnostics;

namespace Universe.ChromeAndDriverInstaller
{
    public static class DebugConsole
    {
        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}