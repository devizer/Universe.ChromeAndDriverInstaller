﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class DriverTests : NUnitTestsBase
    {
        [Test]
        public void A_Open_What_Is_My_Browser()
        {
            OpenWhatIsMyBrowser(false);
        }

        // [Selenium] System.PlatformNotSupportedException : System.Drawing.Common is not supported on this platform.
        [Test /*, RequiredOs(Os.Windows)*/]
        public void B_Screenshot_What_Is_My_Browser()
        {
            OpenWhatIsMyBrowser(true);
        }

        public void OpenWhatIsMyBrowser(bool needScreenshot)
        {
            var knownVersions = new LastKnownGoodVersionsClient().ReadVersions();
            var stableVersion = knownVersions.TryGetStableVersion();
            Console.WriteLine($"Chrome Stable Version: {stableVersion}");

            Console.WriteLine($"Downloading and extracting driver");
            var localDriver = ChromeOrDriverFactory.DownloadAndExtract(stableVersion.Major, ChromeOrDriverType.Driver);

            Console.WriteLine($"Downloading and extracting chrome");
            var localChrome = ChromeOrDriverFactory.DownloadAndExtract(stableVersion.Major, ChromeOrDriverType.Chrome);

            ChromeDriverService svc = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(localDriver.ExecutableFullPath), Path.GetFileName(localDriver.ExecutableFullPath));
            var artifactFolder = Environment.GetEnvironmentVariable("$SYSTEM_ARTIFACTSDIRECTORY");
            if (!string.IsNullOrEmpty(artifactFolder))
            {
                svc.LogPath = Path.Combine(artifactFolder, $"chrome-driver {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.log");
                svc.EnableAppendLog = true;
                svc.EnableVerboseLogging = true;
            }


            // TODO: https://stackoverflow.com/a/50642913
            // --disable-extensions, --disable-gpu, --disable-dev-shm-usage, --no-sandbox
            ChromeOptions options = new ChromeOptions()
            {
                BinaryLocation = localChrome.ExecutableFullPath,
            };
            if (!TinyCrossInfo.IsWindows)
            {
                // options.AddArgument("--disable-extensions");
                options.AddArgument("--disable-gpu");
                // options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--headless");
            }

            using (svc)
            using (var chromeDriver = new ChromeDriver(svc, options))
            {
                chromeDriver.Navigate().GoToUrl($"https://www.whatismybrowser.com/");
                chromeDriver.Manage().Window.Size = new Size(1400, 1000);
                chromeDriver.Manage().Window.Size = new Size(1400, 3000);

                var docTitle = chromeDriver.Title;
                Console.WriteLine($"Got Document Title: '{docTitle}'");

                if (needScreenshot)
                {
                    var dir = Path.GetFullPath(Environment.CurrentDirectory);
                    var fileName = Path.Combine(dir, "what-is-my-browser");
                    Console.WriteLine($"Saving Screenshot as [{fileName}]");
                    Screenshot screenshot = ((ITakesScreenshot)chromeDriver).GetScreenshot();
                    File.WriteAllBytes(fileName + ".bin", screenshot.AsByteArray); 
                    screenshot.SaveAsFile(fileName + ".bmp", ScreenshotImageFormat.Bmp);
                }
            }

        }
    }
}
