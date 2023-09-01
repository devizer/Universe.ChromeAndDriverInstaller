using System;

namespace Universe.ChromeAndDriverInstaller
{
    public static class DebugConsole
    {
        public static void WriteLine(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}