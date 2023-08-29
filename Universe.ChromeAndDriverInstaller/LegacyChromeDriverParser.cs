using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Universe.ChromeAndDriverInstaller.StaticallyCached;
using Universe.ChromeAndDriverInstaller.StaticallyCached.DownloadsMetadata;

namespace Universe.ChromeAndDriverInstaller
{
    public static class LegacyChromeDriverParser
    {
        public static List<ChromeOrDriverEntry> Parse()
        {
            var ret = new List<ChromeOrDriverEntry>();
            var foundList = EmbeddedResourcesHelper.FindEmbeddedResources(typeof(DownloadsMetadataAnchor).Namespace + ".LegacyChromeDriver", ".urls");
            var found = foundList.FirstOrDefault();
            if (found == null)
                throw new InvalidOperationException("Can't find LegacyChromeDriver.urls embedded resource");

            var raw = EmbeddedResourcesHelper.ReadEmbeddedResource(found);
            var lines = raw.Split(new[] { '\r', '\n' }).Where(x => !string.IsNullOrEmpty(x));
            foreach (var url in lines)
            {
                if (url.EndsWith("_linux32.zip", StringComparison.OrdinalIgnoreCase)) continue;
                if (url.EndsWith("_mac32.zip", StringComparison.OrdinalIgnoreCase)) continue;
                if (url.IndexOf("_debug", StringComparison.OrdinalIgnoreCase) >= 0) continue;

                var fileOnly = Path.GetFileName(url);
                var fileWithoutExtension = Path.GetFileNameWithoutExtension(fileOnly);
                var driverVerRaw = Path.GetFileName(Path.GetDirectoryName(url));
                Version driverVer = new Version(driverVerRaw);

                ret.Add(new ChromeOrDriverEntry()
                {
                    Type = ChromeOrDriverType.Driver,
                    Platform = ParsePlatform(fileWithoutExtension),
                    RawMilestone = driverVer.Major.ToString(),
                    RawVersion = driverVerRaw,
                    Url = $"https://chromedriver.storage.googleapis.com/{url}",
                });
            }

            return ret;
        }


        static ChromeAndDriverPlatform ParsePlatform(string fileWithoutExtension)
        {
            var p = ParsePlatformRaw(fileWithoutExtension);
            return (ChromeAndDriverPlatform)Enum.Parse(typeof(ChromeAndDriverPlatform), p, false);
        }

        static string ParsePlatformRaw(string fileWithoutExtension)
        {
            var suffix = fileWithoutExtension.Split('_').Last();
            string key = suffix.Substring(0, 1).ToUpper() + suffix.Substring(1);
            if (key == "M1") return "MacArm64";
            if (key == "Mac64") return "MacX64";
            if (key == "Arm64") return "MacArm64";
            return $"" + key;
        }

    }
}
