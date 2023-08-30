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
}