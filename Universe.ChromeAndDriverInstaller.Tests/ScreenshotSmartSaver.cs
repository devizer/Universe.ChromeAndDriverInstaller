using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    internal class ScreenshotSmartSaver
    {
        // Workaround
        public static void SaveAsPng(Screenshot screenshot, string fileName)
        {
            var asArray = screenshot.AsByteArray;
            var eightBytes = string.Join(" ", asArray.Take(8));
            var isPng = eightBytes == "137 80 78 71 13 10 26 10";
            if (isPng)
            {
                File.WriteAllBytes(fileName, asArray);
            }
            else
            {
                // [Selenium] System.PlatformNotSupportedException : System.Drawing.Common is not supported on this platform.
                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
            }
        }
    }
}
