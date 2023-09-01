using System;
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

        [Test]
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

            // TODO: https://stackoverflow.com/a/50642913
            // --disable-extensions, --disable-gpu, --disable-dev-shm-usage, --no-sandbox
            ChromeOptions options = new ChromeOptions()
            {
                BinaryLocation = localChrome.ExecutableFullPath,
            };
            if (!TinyCrossInfo.IsWindows)
            {
                options.AddArgument("--disable-extensions");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--headless");
            }

            using (var chromeDriver = new ChromeDriver(svc, options))
            {
                chromeDriver.Navigate().GoToUrl($"https://www.whatismybrowser.com/");
                chromeDriver.Manage().Window.Size = new Size(1400, 1000);
                chromeDriver.Manage().Window.Size = new Size(1400, 3000);

                if (needScreenshot)
                {
                    var dir = Path.GetFullPath(Environment.CurrentDirectory);
                    var fileName = Path.Combine(dir, "whatismybrowser.png");
                    Console.WriteLine($"Saving Screenshot as [{fileName}]");
                    Screenshot screenshot = ((ITakesScreenshot)chromeDriver).GetScreenshot();
                    screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
                }
            }

        }
    }
}
