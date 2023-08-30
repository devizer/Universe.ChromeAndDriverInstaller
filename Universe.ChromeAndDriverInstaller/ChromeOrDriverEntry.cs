using System;
using System.Collections.Generic;

namespace Universe.ChromeAndDriverInstaller
{
    public class ChromeOrDriverEntry
    {
        public string RawMilestone { get; set; }
        public string RawVersion { get; set; }
        public ChromeOrDriverType Type { get; set; }
        public ChromeAndDriverPlatform Platform { get; set; }
        public string Url { get; set; }

        public Uri Uri => TryAndRetry.Eval(() => new Uri(Url));
        public int Milestone => TryAndRetry.Eval(() => Int32.Parse(RawMilestone));
        public Version Version => TryAndRetry.Eval(() => new Version(RawVersion));

        public override string ToString()
        {
            return $"[{(Milestone == 0 ? RawMilestone : Milestone.ToString())}], {Type} on {Platform} v{(Version?.Major > 0 ? Version.ToString() : RawVersion)} '{(Uri == null ? Url : Uri.ToString())}'";
        }
    }

    public static class ChromeOrDriverEntryExtensions
    {
        public static List<ChromeOrDriverEntry> Normalize(this IEnumerable<ChromeOrDriverEntry> all)
        {
            Dictionary<string, object> hashSet = new Dictionary<string, object>();
            List<ChromeOrDriverEntry> ret = new List<ChromeOrDriverEntry>();

            foreach (var entry in all)
            {
                var key = $"{entry.RawMilestone}:{entry.RawVersion}:{entry.Type}:{entry.Platform}";
                if (!hashSet.ContainsKey(key))
                {
                    hashSet[key] = null;
                    ret.Add(entry);
                }
            }

            return ret;
        }

        public static ChromeOrDriverEntry FindByVersion(this IEnumerable<ChromeOrDriverEntry> all, int chromeMajor, ChromeOrDriverType type = ChromeOrDriverType.Driver, ChromeAndDriverPlatform platform = ChromeAndDriverPlatform.Unknown)
        {
            if (platform == ChromeAndDriverPlatform.Unknown)
            {
                if (CrossInfo.ThePlatform == CrossInfo.Platform.Windows) platform = ChromeAndDriverPlatform.Win32;
                else if (CrossInfo.ThePlatform == CrossInfo.Platform.MacOSX) platform = ChromeAndDriverPlatform.MacX64;
                else platform = ChromeAndDriverPlatform.Linux64;
                Console.WriteLine($"Detected platform {CrossInfo.ThePlatform} --> {platform}");
            }

            foreach (var entry in all)
            {
                if (entry.Version.Major == chromeMajor && entry.Type == type && entry.Platform == platform)
                    return entry;
            }

            return null;
        }
    }
}