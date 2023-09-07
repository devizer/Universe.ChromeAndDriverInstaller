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

            // Online https://googlechromelabs.github.io/chrome-for-testing/latest-versions-per-milestone-with-downloads.json
            var jsonOnline = TryAndRetry.Eval(() => OnlineDownloadsMetadataClient.GetOnline(), null, 3);
            if (jsonOnline != null)
            {
                ret.AddRange(DownloadsMetadataParser.Parse(jsonOnline));
            }

            // Cached copy of https://googlechromelabs.github.io/chrome-for-testing/latest-versions-per-milestone-with-downloads.json
            var names = EmbeddedResourcesHelper.FindEmbeddedResources(DownloadsMetadataAnchor.Prefix, DownloadsMetadataAnchor.Extension).ToList();
            foreach (var fullName in names)
            {
                var rawJson = EmbeddedResourcesHelper.ReadEmbeddedResource(fullName);
                var entries = DownloadsMetadataParser.Parse(rawJson);
                const int maxVersion = 116; // TODO: Move to file
                ret.AddRange(entries.Where(x => x.Version.Major <= maxVersion));
            }

            // chromedrive CDN at https://chromedriver.storage.googleapis.com/
            var allChromeDriverLegacyEntries = LegacyChromeDriverParser.Parse();
            ret.AddRange(allChromeDriverLegacyEntries);

            /*
            // Drivers 2.x
            foreach (LegacyChromedriver2xEntry driver2x in LegacyChromedriver2xClient.Entries)
            {
                for (int major = driver2x.MinChromeVersion; major <= driver2x.MaxChromeVersion; major++)
                {
                    var driverVersion = driver2x.ChromeDriverVersion;
                    var tempDriverEntries = allChromeDriverLegacyEntries.Where(x => x.RawVersion == driverVersion.ToString()).ToList();
                    foreach (var entry in tempDriverEntries)
                    {
                        ret.Add(new ChromeOrDriverEntry()
                        {
                            RawVersion = 
                        });
                    }
                }
            }
            */

            // Google API CDN: https://www.googleapis.com/download/storage
            ret.AddRange(ChromiumCDN.Entries.Where(x => x.Status != ChromeOrDriveVersionStatus.PreRelease && x.Status != ChromeOrDriveVersionStatus.Beta));

            return ret.Normalize();
        }
    }
}