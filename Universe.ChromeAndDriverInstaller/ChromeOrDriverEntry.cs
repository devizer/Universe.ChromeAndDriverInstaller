using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public static ChromeAndDriverPlatform ResolveCurrentPlatform(this ChromeAndDriverPlatform platform)
        {
            if (platform == ChromeAndDriverPlatform.Unknown)
            {
                if (CrossInfo.ThePlatform == CrossInfo.Platform.Windows) platform = Environment.Is64BitOperatingSystem ? ChromeAndDriverPlatform.Win64 : ChromeAndDriverPlatform.Win32;
                else if (CrossInfo.ThePlatform == CrossInfo.Platform.MacOSX)
                {
                    platform = ChromeAndDriverPlatform.MacX64; // TODO: Arm64

#if NETNETSTANDARD && false
                    if (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64)
                        platform = ChromeAndDriverPlatform.MacArm64;
#else
                    if (CrossInfo.IsMacOnArm) platform = ChromeAndDriverPlatform.MacArm64;
#endif
                }
                else platform = ChromeAndDriverPlatform.Linux64;
                // DebugConsole.WriteLine($"Resolved current platform {CrossInfo.ThePlatform} --> {platform}");
            }

            return platform;
        }

        public static ChromeOrDriverEntry FindByVersion(this IEnumerable<ChromeOrDriverEntry> all, int chromeMajor, ChromeOrDriverType type = ChromeOrDriverType.Driver, ChromeAndDriverPlatform platform = ChromeAndDriverPlatform.Unknown)
        {
            platform = ResolveCurrentPlatform(platform);
            /*
            ChromeAndDriverPlatform[] platforms = new[] { platform };
            if (platform == ChromeAndDriverPlatform.Win64)
                platforms = new[] { ChromeAndDriverPlatform.Win64, ChromeAndDriverPlatform.Win32 };

            foreach (var platformCandidate in platforms)
            {
                
            }
            */

            if (chromeMajor == 49 && type ==ChromeOrDriverType.Driver && Debugger.IsAttached) Debugger.Break();

            Func<ChromeOrDriverEntry, bool> isIt = entry => entry.Version.Major == chromeMajor;

            if (type == ChromeOrDriverType.Driver && chromeMajor <= 73)
            {
                LegacyChromedriver2xEntry[] drivers2x = LegacyChromedriver2xClient.FindChromeDriverVersion(chromeMajor);
                LegacyChromedriver2xEntry driver2x = drivers2x.FirstOrDefault();
                if (driver2x == null)
                {
                    DebugConsole.WriteLine($"WARNING! Unknown ChromeDriver version for Chrome v{chromeMajor}");
                    return null;
                }
                isIt = entry => entry.Version == driver2x.ChromeDriverVersion;
            }

            foreach (ChromeOrDriverEntry entry in all)
            {
                if (isIt(entry) && entry.Type == type && (entry.Platform == platform || (platform == ChromeAndDriverPlatform.Win64 /*&& type == ChromeOrDriverType.Driver*/ && entry.Platform == ChromeAndDriverPlatform.Win32) ))
                    return entry;
            }

            return null;
        }
    }
}