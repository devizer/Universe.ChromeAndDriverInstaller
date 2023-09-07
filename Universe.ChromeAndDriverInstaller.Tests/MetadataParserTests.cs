using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            SmartChromeAndDriverMetadataClient client = new SmartChromeAndDriverMetadataClient();
            List<ChromeOrDriverEntry> entries = client.Read();
            Console.WriteLine($"Entries: {entries.Count}");
            File.WriteAllLines("links.tmp", entries.Select(x => x.Uri.ToString()));

            string dir = Env.ChromeDownloadDir;
            if (string.IsNullOrEmpty(dir)) return;
            string zipsDir = Path.Combine(dir, "Zips");
            TryAndRetry.Exec(() => Directory.CreateDirectory(zipsDir));


            int n = 0;
            entries = entries.OrderByDescending(x => x.Type).ThenBy(x => x.Version).ToList();
            Stopwatch sw = Stopwatch.StartNew();
            foreach (ChromeOrDriverEntry entry in entries)
            {
                string link = entry.Uri.ToString();
                string localZipName = Path.Combine(
                    zipsDir,
                    entry.RawVersion + "-" + Path.GetFileName(link)
                );

                string localDirName = Path.Combine(
                    dir,
                    entry.RawVersion + "-" + Path.GetFileNameWithoutExtension(link)
                    );

                // Console.WriteLine($"localDirName: {localDirName}");
                // if (entry.Type != ChromeOrDriverType.Driver) continue;

                Console.WriteLine($"{++n} of {entries.Count}: {ToString(sw)} {localZipName}");
                new WebDownloader().DownloadFile(link, localZipName, retryCount: 3);

                bool isMacOnNonMac = (entry.Platform == ChromeAndDriverPlatform.MacArm64 || entry.Platform == ChromeAndDriverPlatform.MacX64) &&
                                     CrossInfo.ThePlatform != CrossInfo.Platform.MacOSX;

                if (isMacOnNonMac && entry.Type == ChromeOrDriverType.Chrome) continue;
                ChromeOrDriverExtractor.Extract(entry, localZipName, localDirName);
            }

        }

        string ToString(Stopwatch sw)
        {
            string format = "mm:ss.f";
            if (sw.Elapsed.TotalHours >= 1 ) format = "HH:mm:ss.f";
            return new DateTime().AddMilliseconds(sw.ElapsedMilliseconds).ToString(format);
        }


    }
}