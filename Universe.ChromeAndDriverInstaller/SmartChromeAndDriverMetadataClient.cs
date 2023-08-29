using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Universe.ChromeAndDriverInstaller.StaticallyCached;
using Universe.ChromeAndDriverInstaller.StaticallyCached.DownloadsMetadata;

namespace Universe.ChromeAndDriverInstaller
{
    public class SmartChromeAndDriverMetadataClient
    {
        private static Lazy<List<ChromeOrDriverEntry>> _Entries = new Lazy<List<ChromeOrDriverEntry>>(() =>
        {
            return new SmartChromeAndDriverMetadataClient().Read();
        }, LazyThreadSafetyMode.ExecutionAndPublication);


        public List<ChromeOrDriverEntry> Read()
        {
            List<ChromeOrDriverEntry> ret = new List<ChromeOrDriverEntry>();
            var jsonOnline = TryAndRetry.Eval(() => OnlineDownloadsMetadataClient.GetOnline(), null, 3);
            if (jsonOnline != null)
            {
                ret.AddRange(DownloadsMetadataParser.Parse(jsonOnline));
            }

            var names = EmbeddedResourcesHelper.FindEmbeddedResources(DownloadsMetadataAnchor.Prefix, DownloadsMetadataAnchor.Extension).ToList();
            foreach (var fullName in names)
            {
                var rawJson = EmbeddedResourcesHelper.ReadEmbeddedResource(fullName);
                var entries = DownloadsMetadataParser.Parse(rawJson);
                ret.AddRange(entries);
            }

            ret.AddRange(LegacyChromeDriverParser.Parse());

            return ret.Normalize();
        }
    }
}