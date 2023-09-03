using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrabChromiumLinks.External;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Universe;
using Universe.ChromeAndDriverInstaller;

namespace GrabChromiumLinks
{
    internal class HtmlLinksParser : IDisposable
    {
        private ChromeDriverService DriverService;
        private ChromeDriver Driver;
        private object SyncOpenBrowser = new object();

        public List<DownloadLink> ParseLinks(string htmlUrl, Func<ChromeDriver, List<DownloadLink>, bool> isReady)
        {
            var (driverService, chromeDriver) = OpenBrowser();


            chromeDriver.Navigate().GoToUrl(htmlUrl);
            // Thread.Sleep(777);

            Stopwatch sw = Stopwatch.StartNew();
            int retries = 0;
            int consequentlySuccess = 0;
            while (true)
            {
                retries++;
                List<DownloadLink> ret = new List<DownloadLink>();
                var linkList = chromeDriver.FindElements(By.TagName("a"));
                foreach (IWebElement? link in linkList)
                {
                    var href = link.GetDomAttribute("href");
                    var text = link.Text;
                    ret.Add(new DownloadLink() { Name = text, Url = href });
                }

                if (isReady(chromeDriver, ret))
                {
                    consequentlySuccess++;
                    if (consequentlySuccess >= 2)
                    {
                        Console.WriteLine($"Success number {consequentlySuccess} (total {retries}) for {htmlUrl}");
                        return ret;
                    }
                    else
                    {
                        Console.WriteLine($"First success number {consequentlySuccess} (total {retries}) for {htmlUrl}");
                    }

                    continue;
                }
                else if (sw.Elapsed.TotalSeconds > 30)
                {
                    Console.WriteLine($"WARNING! TIMEOUT FOR {htmlUrl}");
                    return null;
                }

                // consequentlySuccess = 0;
            }
        }


        private (ChromeDriverService, ChromeDriver) OpenBrowser()
        {
            if (Driver == null || DriverService == null)
                lock(SyncOpenBrowser)
                    if (Driver == null || DriverService == null)
                    {
                        (DriverService, Driver) = OpenBrowser_Impl();
                    }

            return (DriverService, Driver);
        }
        private (ChromeDriverService, ChromeDriver) OpenBrowser_Impl()
        {
            var knownVersions = new LastKnownGoodVersionsClient().ReadVersions();
            var stableVersion = knownVersions.TryGetStableVersion();
            Console.WriteLine($"Chrome Stable Version: {stableVersion}");

            Console.WriteLine($"Downloading and extracting driver");
            var localDriver = ChromeOrDriverFactory.DownloadAndExtract(stableVersion.Major, ChromeOrDriverType.Driver);

            Console.WriteLine($"Downloading and extracting chrome");
            var localChrome = ChromeOrDriverFactory.DownloadAndExtract(stableVersion.Major, ChromeOrDriverType.Chrome);

            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(localDriver.ExecutableFullPath),
                Path.GetFileName(localDriver.ExecutableFullPath));
            var artifactFolder = Environment.GetEnvironmentVariable("SYSTEM_ARTIFACTSDIRECTORY");
            if (false && !string.IsNullOrEmpty(artifactFolder))
            {
                var logFile = Path.Combine(artifactFolder, $"chrome-driver {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.log");
                DebugConsole.WriteLine($"LOG TO: [{logFile}]");
                driverService.LogPath = logFile;
                driverService.EnableAppendLog = true;
                driverService.EnableVerboseLogging = true;
            }


            // C:\Apps\Chromium-116.0.5841.1-32bit\chrome.exe
            // TODO: https://stackoverflow.com/a/50642913
            // --disable-extensions, --disable-gpu, --disable-dev-shm-usage, --no-sandbox
            ChromeOptions options = new ChromeOptions()
            {
                BinaryLocation = localChrome.ExecutableFullPath,
                // BinaryLocation = @"C:\Apps\Chromium-116.0.5841.1-32bit\chrome.exe",
                AcceptInsecureCertificates = true,
            };
            if (!TinyCrossInfo.IsWindows)
            {
                // options.AddArgument("--disable-extensions");
                options.AddArgument("--disable-gpu");
                // options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--headless");
                // options.AddArgument("user-data-dir=W:\\Temp\\SeleniumProfile");
            }

            options.AddArguments("force-device-scale-factor=2.0");
            options.AddArguments("high-dpi-support=2.0");

            ChromeDriver chromeDriver = new ChromeDriver(driverService, options);

            var docTitle = chromeDriver.Title;
            Console.WriteLine($"Got Document Title: '{docTitle}'");
            return (driverService, chromeDriver);
        }


        public void Dispose()
        {
            lock (SyncOpenBrowser)
            {
                if (Driver != null)
                {
                    Driver.Dispose();
                    Driver = null;
                }

                if (DriverService != null)
                {
                    DriverService.Dispose();
                    DriverService = null;
                }
            }
        }
    }
}
