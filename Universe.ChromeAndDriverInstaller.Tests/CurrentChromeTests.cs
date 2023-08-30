using System;
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