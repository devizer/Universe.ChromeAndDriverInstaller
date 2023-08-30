using System;
using System.IO;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class CurrentChromeTests : NUnitTestsBase
    {
        [Test]
        public void TestWindowsChromePath()
        {
            if (!TinyCrossInfo.IsWindows) return;
            Console.WriteLine(CurrentChromeVersionClient.GetWindowsChromePath());
        }

        [Test]
        public void TestWindowsRawVersion()
        {
            if (!TinyCrossInfo.IsWindows) return;
            Console.WriteLine(CurrentChromeVersionClient.GetWindowsChromeVersion());
        }

        [Test]
        public void MacOsLab()
        {
            if (CrossInfo.ThePlatform != CrossInfo.Platform.MacOSX) return;
            const string bin = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            Console.WriteLine($"BIN: {bin}");
            Console.WriteLine($"BIN EXISTS: {File.Exists(bin)}");
            Console.WriteLine($"BIN DIR EXISTS: {Directory.Exists(Path.GetDirectoryName(bin))}");
        }


        [Test]
        public void TestMajorVersion()
        {
            if (CrossInfo.ThePlatform == CrossInfo.Platform.MacOSX) return;
            Console.WriteLine($"Major Chrome Version: [{CurrentChromeVersionClient.TryGetMajorVersion()}]");
        }

        [Test]
        public void TestRawVersion()
        {
            if (CrossInfo.ThePlatform == CrossInfo.Platform.MacOSX) return;
            Console.WriteLine($"Chrome Version: [{CurrentChromeVersionClient.GetRawVersion()}]");
        }

    }
}