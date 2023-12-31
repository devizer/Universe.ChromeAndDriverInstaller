﻿using System;
using System.Collections.ObjectModel;
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
    [Explicit]
    public class DriverTestsMatrix : NUnitTestsBase
    {
        [Test]
        [TestCaseSource(typeof(DriverTestSmartCaseSource), nameof(DriverTestSmartCaseSource.Famous))]
        public void A_Open_What_Is_My_Browser(DriverTestSmartCase testCase)
        {
            OpenWhatIsMyBrowser(testCase, false);
        }

        // [Selenium] System.PlatformNotSupportedException : System.Drawing.Common is not supported on this platform.
        [Test /*, RequiredOs(Os.Windows)*/]
        [TestCaseSource(typeof(DriverTestSmartCaseSource), nameof(DriverTestSmartCaseSource.Famous))]
        public void B_Screenshot_What_Is_My_Browser(DriverTestSmartCase testCase)
        {
            OpenWhatIsMyBrowser(testCase, true);
        }

        public void OpenWhatIsMyBrowser(DriverTestSmartCase testCase, bool needScreenshot)
        {
            ChromeOrDriverResult localDriver = ChromeOrDriverFactory.DownloadAndExtract(testCase.ChromeDriverMetadata);

            var localChrome = ChromeOrDriverFactory.DownloadAndExtract(testCase.ChromeMetadata);

            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(localDriver.ExecutableFullPath), Path.GetFileName(localDriver.ExecutableFullPath));
            var artifactFolder = Environment.GetEnvironmentVariable("SYSTEM_ARTIFACTSDIRECTORY");
            if (!string.IsNullOrEmpty(artifactFolder))
            {
                var logFile = Path.Combine(artifactFolder, $"chrome-driver {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.log");
                DebugConsole.WriteLine($"LOG TO: [{logFile}]");
                driverService.LogPath = logFile;
                driverService.EnableAppendLog = true;
                driverService.EnableVerboseLogging = false;
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
            }

            // options.AddArguments("force-device-scale-factor=2.0");
            // options.AddArguments("high-dpi-support=2.0");

            using (driverService)
            using (var chromeDriver = new ChromeDriver(driverService, options))
            {
                chromeDriver.Navigate().GoToUrl($"https://www.whatismybrowser.com/");
                Console.WriteLine("SUCCESS: Navigate().GoToUrl()");
                if (localChrome.Metadata.Version.Major > 49)
                {
                    chromeDriver.Manage().Window.Size = new Size(1400, 1000);
                    chromeDriver.Manage().Window.Size = new Size(1400, 6000);
                    Console.WriteLine("SUCCESS: Set Manage().Window.Size");
                }
                else
                {
                    Console.WriteLine("WARNING! Window.Size is available for v63+");
                }

                if (localChrome.Metadata.Version.Major >= 76)
                {
                    string docTitle = chromeDriver.Title;
                    Console.WriteLine($"SUCCESS: Got Document Title '{docTitle}'");
                }
                else
                {
                    Console.WriteLine("WARNING! Window.Size is available for v76+");
                }



                if (localChrome.Metadata.Version.Major >= 76)
                {
                    var executeScriptResult = chromeDriver.ExecuteScript("return document.title");
                    // Dictionary means error
                    Console.WriteLine($"SUCCESS: ExecuteScript() returns {executeScriptResult?.GetType()}, {executeScriptResult}");
                    string titleLegacy = (string)executeScriptResult;
                    Console.WriteLine($"Got Document Legacy Title: '{titleLegacy}'");
                }


                if (localChrome.Metadata.Version.Major >= 49)
                {
                    ReadOnlyCollection<IWebElement> aLinks = chromeDriver.FindElements(By.TagName("a"));
                    var texts = aLinks.Select(x => $"{x.Text}").ToList();
                    Console.WriteLine($"SUCCESS: FindElements(By.TagName(\"a\"): {texts.Count}");
                    var hrefs = aLinks.Select(x => $"{x.GetDomAttribute("href")}").ToList();
                    Console.WriteLine($"SUCCESS: GetDomAttribute(\"href\") of each link: {hrefs.Count}");
                }


                Console.WriteLine($"chromeDriver.Capabilities Type: '{chromeDriver.Capabilities.GetType()}'");
                Console.WriteLine(chromeDriver.Capabilities.GetCapability("PlatformName"));


                if (needScreenshot)
                {
                    if (localChrome.Metadata.Version.Major >= 76)
                    {
                        var dir = Path.GetFullPath(Environment.CurrentDirectory);
                        var fileName = Path.Combine(dir, "what-is-my-browser");
                        Console.WriteLine($"Saving Screenshot as [{fileName}]");
                        Screenshot screenshot = ((ITakesScreenshot)chromeDriver).GetScreenshot();
                        var rawScreenshot = screenshot.AsByteArray;
                        Console.WriteLine($"First {GetFirstEightBytes(rawScreenshot)}");
                        File.WriteAllBytes(fileName + ".bin", rawScreenshot);
                        // screenshot.SaveAsFile(fileName + ".bmp", ScreenshotImageFormat.Bmp);
                        var fileNameSuffix = $"v{testCase.ChromeMetadata.Version.Major}";
                        ScreenshotSmartSaver.SaveAsPng(screenshot, $"{fileName}-{fileNameSuffix}.png");
                    }
                    else
                    {
                        Console.WriteLine("WARNING! Screenshots are available for v76+");
                    }
                }
            }
        }

        // PNG:    137 80 78 71 13 10 26 10
        // Actual: 137 80 78 71 13 10 26 10
        static string GetFirstEightBytes(byte[] arr)
        {
            if (arr == null) return "null";
            return string.Join(" ", arr.Take(8));
        }
    }


}
