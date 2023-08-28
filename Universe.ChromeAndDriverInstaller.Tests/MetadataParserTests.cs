using System;
using System.Linq;
using NUnit.Framework;
using Universe.ChromeAndDriverInstaller.StaticallyCached;
using Universe.ChromeAndDriverInstaller.StaticallyCached.DownloadsMetadata;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class MetadataParserTests : NUnitTestsBase
    {
        [Test]
        public void ParseOnline()
        {
            var rawJson = OnlineDownloadsMetadataClient.GetOnline();
            Console.WriteLine($"json.Length: {rawJson.Length}");

            DownloadsMetadataParser.Parse(rawJson);
        }
        
        [Test]
        public void ParseCached()
        {
            var names = EmbeddedResourcesHelper.FindEmbeddedResources(DownloadsMetadataAnchor.Prefix, DownloadsMetadataAnchor.Extension).ToList();
            var fullName = names.First();

            var rawJson = EmbeddedResourcesHelper.ReadEmbeddedResource(fullName);
            Console.WriteLine($"json.Length: {rawJson.Length}");

            DownloadsMetadataParser.Parse(rawJson);
        }
    }
}