using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Universe.ChromeAndDriverInstaller.StaticallyCached;

namespace Universe.ChromeAndDriverInstaller
{
    public static class ChromiumCDN
    {

        public static List<ChromiumCDNEntry> Entries => _Entries.Value;

        private static Lazy<List<ChromiumCDNEntry>> _Entries => new Lazy<List<ChromiumCDNEntry>>(ReadEntries);

        static string GetJsonString()
        {
            var fullName = typeof(EmbeddedResourcesHelper).Namespace + "." + "chromium-and-chromedriver-with-sha1-hash.json";
            var ret = EmbeddedResourcesHelper.ReadEmbeddedResource(fullName);
            return ret;
        }

        public static List<ChromiumCDNEntry> ReadEntries()
        {
            List<ChromiumCDNEntry> ret = new List<ChromiumCDNEntry>();
            JObject root = JObject.Parse(GetJsonString());

            foreach (var pair in root)
            {
                var rawVersion = pair.Key;
                JObject jVersionValue = (JObject)pair.Value;
                var rawStatus = jVersionValue?["status"]?.Value<string>();
                var rawV8Version = jVersionValue?["v8version"]?.Value<string>();
                foreach (var jPlatform in jVersionValue.Properties())
                {
                    var rawPlatform = jPlatform.Name;
                    var platform = DownloadsMetadataParser.ParseChromeAndDriverPlatform(rawPlatform);
                    if (!(jPlatform.Value is JObject jPlatformValue)) continue;

                    foreach (var jDownloadType in jPlatformValue.Properties())
                    {
                        var rawType = jDownloadType.Name;
                        ChromeOrDriverType type = DownloadsMetadataParser.ParseChromeOrDriverType(rawType);
                        JObject jDownloadValue = (JObject) jDownloadType.Value;
                        string url = jDownloadValue?["url"].Value<string>();
                        string sha1 = jDownloadValue?["sha1"].Value<string>();

                        ChromiumCDNEntry row = new ChromiumCDNEntry
                        {
                            RawVersion = rawVersion,
                            Platform = platform,
                            Type = type,
                            RawMilestone = new Version(rawVersion).Major.ToString(),
                            SHA1 = sha1,
                            Url = url,
                            RawStatus = rawStatus,
                            V8Version = rawV8Version,
                        };

                        bool isValid =
                            !string.IsNullOrEmpty(row.RawVersion)
                            && row.Platform != ChromeAndDriverPlatform.Unknown
                            && row.Type != ChromeOrDriverType.Unknown
                            && !string.IsNullOrEmpty(row.Url)
                            && !string.IsNullOrEmpty(row.RawStatus);

                        if (isValid) ret.Add(row);
                    }
                }
            }

            return ret;
        }

    }
}