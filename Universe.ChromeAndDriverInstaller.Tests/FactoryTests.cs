using System;
using System.Linq;
using NUnit.Framework;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class FactoryTests
    {
        [Test] // Chrome should be preinstalled
        public void A_DownloadCurrentDriver()
        {
            ChromeOrDriverResult result = ChromeOrDriverFactory.DownloadAndExtract(null, ChromeOrDriverType.Driver);
            Console.WriteLine($"RESULT{Environment.NewLine}──────");
            Console.WriteLine($"{result.ToString(true)}");
        }

        [Test] // Chrome should be preinstalled
        public void B_DownloadCurrentChrome()
        {
            ChromeOrDriverResult result = ChromeOrDriverFactory.DownloadAndExtract(null, ChromeOrDriverType.Chrome);
            Console.WriteLine($"RESULT{Environment.NewLine}──────");
            Console.WriteLine($"{result.ToString(true)}");
        }


        [Test, Explicit]
        public void C_DownloadAll()
        {
            var client = new SmartChromeAndDriverMetadataClient();
            var entries = client.Read();

            foreach (var entry in entries.Where(x => x.Milestone > 2))
            {
                bool isMacOnNonMac = (entry.Platform == ChromeAndDriverPlatform.MacArm64 || entry.Platform == ChromeAndDriverPlatform.MacX64)
                                     && CrossInfo.ThePlatform != CrossInfo.Platform.MacOSX;

                if (isMacOnNonMac) continue;

                Console.WriteLine($"•D&E│ {entry}");
                ChromeOrDriverResult result = ChromeOrDriverFactory.DownloadAndExtract(entry.Version.Major, entry.Type, entry.Platform);
                Console.WriteLine($"    │ {result}");
            }
        }
    }
}
