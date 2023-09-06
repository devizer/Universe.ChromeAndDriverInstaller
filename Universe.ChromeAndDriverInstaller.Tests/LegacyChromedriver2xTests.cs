using System;
using System.Linq;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class LegacyChromedriver2xTests : NUnitTestsBase
    {
        [Test]
        public void ShowAll()
        {
            var entries = LegacyChromedriver2xClient.Entries;
            Console.WriteLine(string.Join(Environment.NewLine, entries.Select(x => x.ToString())));

        }

        [Test]
        [TestCase(73, "2.46")]
        [TestCase(51, "2.23")]
        public void TestLookup(int chromeVersion, string expectedDriverVersion)
        {
            var latest = LegacyChromedriver2xClient.FindChromeDriverVersion(chromeVersion).FirstOrDefault();
            Assert.IsNotNull(latest, "ChromeDriver version not found");
            Assert.AreEqual(expectedDriverVersion, latest.ChromeDriverVersion.ToString());

        }
    }
}