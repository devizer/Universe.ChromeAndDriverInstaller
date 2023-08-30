using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class CurrentChromeTests : NUnitTestsBase
    {
        [Test]
        public void TestPath()
        {
            if (!TinyCrossInfo.IsWindows) return;
            Console.WriteLine(CurrentChromeVersionClient.GetWindowsChromePath());
        }

        [Test]
        public void TestRawVersion()
        {
            if (!TinyCrossInfo.IsWindows) return;
            Console.WriteLine(CurrentChromeVersionClient.GetWindowsChromeVersion());
        }
        [Test]

        public void TestMajorVersion()
        {
            Console.WriteLine($"Major Chrome Version: [{CurrentChromeVersionClient.TryGetMajorVersion()}]");
        }

    }
}