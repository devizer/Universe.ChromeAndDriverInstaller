using System;
using System.Collections.Generic;

namespace Universe.ChromeAndDriverInstaller
{
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