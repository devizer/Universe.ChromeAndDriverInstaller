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
            if (eightBytes == "137 80 78 71 13 10 26 10")
            {
                File.WriteAllBytes(fileName + ".bin", asArray);
            }
            else
            {
                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
            }
        }
    }
}
