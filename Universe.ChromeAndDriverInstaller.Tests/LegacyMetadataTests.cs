using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class LegacyMetadataTests : NUnitTestsBase
    {
        [Test]
        public void LegacyMetadataExists()
        {
            var entries = LegacyChromeDriverParser.Parse();
            Console.WriteLine($"Legacy Metadata Count: {entries.Count}");
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }
            Assert.GreaterOrEqual(entries.Count, 1);

        }
    }
}