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
                var key = $"{entry.RawMilestone}:{entry.Type}:{entry.Platform}";
                if (!hashSet.ContainsKey(key))
                {
                    hashSet[key] = null;
                    ret.Add(entry);
                }
            }

            return ret;
        }
    }
}