using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class FindByVersionTests : NUnitTestsBase
    {
        [Test]
        [TestCase(113, ChromeAndDriverPlatform.Win32)]
        [TestCase(77, ChromeAndDriverPlatform.Win32)]
        [TestCase(116, ChromeAndDriverPlatform.Win32)]
        [TestCase(113, ChromeAndDriverPlatform.Linux64)]
        [TestCase(77, ChromeAndDriverPlatform.Linux64)]
        [TestCase(116, ChromeAndDriverPlatform.Linux64)]
        [TestCase(113, ChromeAndDriverPlatform.MacX64)]
        [TestCase(77, ChromeAndDriverPlatform.MacX64)]
        [TestCase(116, ChromeAndDriverPlatform.MacX64)]
        [TestCase(113, ChromeAndDriverPlatform.Unknown)]
        [TestCase(77, ChromeAndDriverPlatform.Unknown)]
        [TestCase(116, ChromeAndDriverPlatform.Unknown)]
        public void CheckUpMetadataByVersion(int majorVersion, ChromeAndDriverPlatform platform)
        {
            var metadataClient = new SmartChromeAndDriverMetadataClient();
            var entries = metadataClient.Read();
            var driverEntry = entries?.Normalize().FindByVersion(majorVersion, platform: platform);
            if (driverEntry == null)
                Console.WriteLine("ChromeDriver not found");
            else
                Console.WriteLine($"FOUND: {driverEntry}");
        }

    }
}