using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class ChromiumCDNTests : NUnitTestsBase
    {
        [Test]
        public void TestParsingEntries()
        {
            var entries = ChromiumCDN.Entries;
            Assert.IsTrue(entries.Count > 0, "ChromiumCDN.Entries is empty");
            Console.WriteLine($"ChromiumCDN.Entries count is {entries.Count}");

            foreach (var chromiumCdnEntry in entries)
            {
                Console.WriteLine(chromiumCdnEntry);
            }
        }
    }
}