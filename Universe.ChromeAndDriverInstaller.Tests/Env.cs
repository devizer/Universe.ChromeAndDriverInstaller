using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    internal static class Env
    {
        public static string ChromeDownloadDir => Environment.GetEnvironmentVariable("CHROMEDRIVER_DOWNLOAD_DIR");
    }
}
