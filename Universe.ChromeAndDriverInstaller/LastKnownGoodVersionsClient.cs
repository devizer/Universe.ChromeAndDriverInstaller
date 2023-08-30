using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Universe.Shared;

namespace Universe.ChromeAndDriverInstaller
{
    public class LastKnownGoodVersionsClient
    {
        public static readonly string EndpointV1 = 
            "https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions.json";

        
        public ChromeLastKnownGoodVersions ReadVersions()
        {
            var jsonBytes = new WebDownloader().DownloadContent(EndpointV1, 3);
            UTF8Encoding utf8 = new UTF8Encoding(false);
            var jsonString = utf8.GetString(jsonBytes);
            JObject json = JObject.Parse(jsonString);

            JObject jsonChannels = json["channels"] as JObject;
            if (jsonChannels == null) 
                throw new InvalidOperationException($"Missing channels object for {EndpointV1}");

            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (var pair in jsonChannels)
            {
                JObject jsonChannel = pair.Value as JObject;
                if (jsonChannel == null) continue;
                var channel = jsonChannel["channel"]?.ToString();
                var rawVersion = jsonChannel["version"]?.ToString();
                if (!string.IsNullOrEmpty(channel) && !string.IsNullOrEmpty(rawVersion))
                    ret[channel] = rawVersion;
            }

            return new ChromeLastKnownGoodVersions(ret);
        }
    }

    public class ChromeLastKnownGoodVersions
    {
        // Key: Stable|Beta|Dev|Canary
        public readonly Dictionary<string, string> Dictionary;

        public ChromeLastKnownGoodVersions(Dictionary<string, string> asDictionary)
        {
            Dictionary = asDictionary;
        }

        public Version TryGetStableVersion() => TryGetVersion("Stable");
        public Version TryGetBetaVersion() => TryGetVersion("Beta");

        public Version TryGetVersion(string channel)
        {
            if (true != Dictionary?.TryGetValue(channel, out var rawVersion)) rawVersion = null;
            if (rawVersion != null)
            {
                try
                {
                    return new Version(rawVersion);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}