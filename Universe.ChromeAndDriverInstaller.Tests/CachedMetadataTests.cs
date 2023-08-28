using System;
using System.Linq;
using NUnit.Framework;
using Universe.ChromeAndDriverInstaller.StaticallyCached;
using Universe.ChromeAndDriverInstaller.StaticallyCached.DownloadsMetadata;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class CachedMetadataTests : NUnitTestsBase
    {
        [Test]
        public void CachedMetadataExists()
        {
            var names = EmbeddedResourcesHelper.FindEmbeddedResources(DownloadsMetadataAnchor.Prefix, DownloadsMetadataAnchor.Extension).ToList();
            Console.WriteLine($"Cached Downloads Metadata Count: {names.Count}");
            Assert.GreaterOrEqual(names.Count, 1);
        }
    }
}
