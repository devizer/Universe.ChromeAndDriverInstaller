using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Universe.ChromeAndDriverInstaller.StaticallyCached;
using Universe.ChromeAndDriverInstaller.StaticallyCached.DownloadsMetadata;
using Universe.NUnitTests;
using Universe.Shared;

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

            var entries = DownloadsMetadataParser.Parse(rawJson);
            foreach (ChromeOrDriverEntry entry in entries)
            {
                Console.WriteLine(entry);
            }
        }

        [Test]
        public void ParseSmarty()
        {
            var client = new SmartChromeAndDriverMetadataClient();
            var entries = client.Read();
            Console.WriteLine($"Entries: {entries.Count}");
        }

        [Test, Explicit]
        public void DownloadSmarty()
        {
            var client = new SmartChromeAndDriverMetadataClient();
            var entries = client.Read();
            Console.WriteLine($"Entries: {entries.Count}");

            var dir = Env.ChromeDownloadDir;
            if (string.IsNullOrEmpty(dir)) return;

            List<string> links = new List<string>();
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
                links.Add(entry.Uri.ToString());
            }

            File.WriteAllLines("links.tmp", links);
            int n = 0;
            TryAndRetry.Exec(() => Directory.CreateDirectory(dir));
            foreach (var link in links)
            {
                string localName = Path.Combine(
                    dir,
                    Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(link))) + "-" + Path.GetFileName(link)
                );

                Console.WriteLine($"{++n} of {links.Count}: {localName}");
                new WebDownloader().DownloadFile(link, localName);
            }

        }


    }
}