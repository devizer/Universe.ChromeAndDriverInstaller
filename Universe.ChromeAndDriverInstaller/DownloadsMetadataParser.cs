using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Universe.ChromeAndDriverInstaller
{
    public static class DownloadsMetadataParser
    {
        public static List<ChromeOrDriverEntry> Parse(string rawJson)
        {
            List<ChromeOrDriverEntry> ret = new List<ChromeOrDriverEntry>();
            JObject jsonRoot = JObject.Parse(rawJson);
            JObject jsonMilestones = jsonRoot["milestones"] as JObject;
            if (jsonMilestones == null) return ret;

            foreach (KeyValuePair<string, JToken> jsonMilestonePair in jsonMilestones)
            {
                string key = jsonMilestonePair.Key;
                JObject jsonMilestone = jsonMilestonePair.Value as JObject;
                if (jsonMilestone == null) continue;

                string rawMilestone = jsonMilestone["milestone"]?.ToString();
                string rawVersion = jsonMilestone["version"]?.ToString();
                JObject jsonDownloads = jsonMilestone["downloads"] as JObject;
                if (jsonDownloads == null) continue;
                foreach (KeyValuePair<string, JToken> typePair in jsonDownloads)
                {
                    var typeKey = typePair.Key;
                    JArray linksArray = typePair.Value as JArray;
                    ChromeOrDriverType type = ParseChromeOrDriverType(typeKey);
                    if (type == ChromeOrDriverType.Unknown || linksArray == null) continue;
                    foreach (JToken linkToken in linksArray)
                    {
                        JObject jsonLink = linkToken as JObject;
                        if (jsonLink == null) continue;
                        string rawPlatform = jsonLink["platform"]?.ToString();
                        string url = jsonLink["url"]?.ToString();
                        var platform = ParseChromeAndDriverPlatform(rawPlatform);
                        if (platform == ChromeAndDriverPlatform.Unknown || string.IsNullOrEmpty(url)) continue;

                        ret.Add(new ChromeOrDriverEntry()
                        {
                            Platform = platform,
                            RawMilestone = rawMilestone,
                            RawVersion = rawVersion,
                            Type = type,
                            Url = url,
                        });

                    }
                }

            }

            return ret;
        }

        public static ChromeOrDriverType ParseChromeOrDriverType(string raw)
        {
            if ("chromedriver".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeOrDriverType.Driver;
            if ("chrome".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeOrDriverType.Chrome;
            return ChromeOrDriverType.Unknown;
        }

        public static ChromeAndDriverPlatform ParseChromeAndDriverPlatform(string raw)
        {
            if ("win64".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.Win64;
            if ("win32".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.Win32;
            if ("linux64".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.Linux64;

            if ("mac-arm64".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.MacArm64;
            if ("mac64_m1".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.MacArm64;
            if ("mac-x64".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.MacX64;
            if ("mac64".Equals(raw, StringComparison.OrdinalIgnoreCase)) return ChromeAndDriverPlatform.MacX64;

            return ChromeAndDriverPlatform.Unknown;
        }
    }
}