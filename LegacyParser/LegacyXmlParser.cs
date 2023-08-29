using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace LegacyParser
{
    public class LegacyXmlParser
    {
        public static void Parse(XmlDocument doc)
        {
            var rawNodes = doc.DocumentElement.SelectNodes("Contents");
            List<string> urls = new List<string>();
            foreach (XmlElement node in rawNodes)
            {
                var key = node["Key"]?.InnerText;
                if (string.IsNullOrEmpty(key)) continue;
                if (!key.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) continue;

                var url = $"https://chromedriver.storage.googleapis.com/{key}";
                urls.Add(url);

                var fileOnly = Path.GetFileName(key);
                var fileWithoutExtension = Path.GetFileNameWithoutExtension(fileOnly);
                var driverVerRaw = Path.GetDirectoryName(key);
                Version driverVer = new Version(driverVerRaw);

                // Console.WriteLine($"{driverVer} on {ParsePlatform(fileWithoutExtension)}: " + key);
            }

            urls = urls.OrderBy(x => ParseVersion(x)).ToList();
            foreach (var url in urls)
            {
                var fileOnly = Path.GetFileName(url);
                var fileWithoutExtension = Path.GetFileNameWithoutExtension(fileOnly);
                var driverVerRaw = Path.GetFileName(Path.GetDirectoryName(url));
                Version driverVer = new Version(driverVerRaw);

                Console.WriteLine($"{driverVer} on {ParsePlatform(fileWithoutExtension)}: " + url);
            }

            File.WriteAllLines("LegacyChromeDriver.urls", urls);
        }

        static Version ParseVersion(string url)
        {
            var fileOnly = Path.GetFileName(url);
            var fileWithoutExtension = Path.GetFileNameWithoutExtension(fileOnly);
            var driverVerRaw = Path.GetFileName(Path.GetDirectoryName(url));
            Version driverVer = new Version(driverVerRaw);
            return driverVer;
        }

        // ChromeAndDriverPlatform.
        static string ParsePlatform(string fileWithoutExtension)
        {
            var suffix = fileWithoutExtension.Split('_').Last();
            string key = suffix.Substring(0, 1).ToUpper() + suffix.Substring(1);
            if (key == "M1") return "MacArm64";
            if (key == "Mac64") return "MacX64";
            if (key == "Arm64") return "MacArm64";
            return $"" + key;
        }

        static void DownloadFile(string url, string file)
        {

        }
    }
}