using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using GrabChromiumLinks.External;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Universe.ChromeAndDriverInstaller;

namespace GrabChromiumLinks;

public class ChromiumMetadataJsonWriter
{
    public static string Serialize(IEnumerable<SourceRow> allRows, bool onlyBrowserAndDriver = false)
    {
        JObject ret = new JObject();

        var uniqueVersions = allRows.ToDistinct(x => x.RawVersion);

        HashSet<string> unknownV8Set = new HashSet<string>();

        foreach (var versionAndList in uniqueVersions.OrderByDescending(x => x.Key.TryParseVersion()))
        {
            var version = versionAndList.Key;
            var listByVersion = versionAndList.Value;

            var platformsByVersion = listByVersion.ToDistinct(x => x.Platform);

            JObject jPlatforms = JObject.FromObject(new { });
            string v8Ver = platformsByVersion.Values.SelectMany(x => x).FirstOrDefault(x => x.V8Version != null && x.RawVersion == version)?.V8Version;
            if (v8Ver != null)
                jPlatforms.Add("v8version", v8Ver);
            else
                unknownV8Set.Add(version);

            foreach (var platformByVersion in platformsByVersion.Keys.OrderBy(x => x.ToString()))
            {
                // JObject jPlatformValue = JObject.FromObject(new { });
                var links = platformsByVersion[platformByVersion];
                if (links.Count > 1) throw new InvalidOperationException("links.Count > 1");
                
                // JObject jPlatformValue = JObject.FromObject(new {html = links.First().HtmlLink});
                var downloadLinks = links.First().DownloadLinks;
                if (onlyBrowserAndDriver) downloadLinks = PatchForBrowserAndDriverOnly(downloadLinks);
                JObject jPlatformValue = JObject.FromObject(downloadLinks.AsJObject());
                jPlatforms.Add(platformByVersion.ToString(), jPlatformValue);
                jPlatforms[platformByVersion.ToString()] = jPlatformValue;
            }
            var humanPlatformList = string.Join(", ", platformsByVersion.Select(x => x.Key.ToString()));
            ret.Add(version, jPlatforms);
        }

        if (unknownV8Set.Count > 0)
            DebugConsole.WriteLine($"WARNING: Unknown V8 version for the list: {string.Join(", ", unknownV8Set.OrderBy(x => x.TryParseVersion()))}");

        return JsonExtensions.Format(ret, false);
    }

    private static List<DownloadLink> PatchForBrowserAndDriverOnly(List<DownloadLink> downloadLinks)
    {
        var ret = new List<DownloadLink>();
        foreach (var link in downloadLinks)
        {
            var isChromeDriver = link.Name.IndexOf("chromedriver", StringComparison.OrdinalIgnoreCase) >= 0;
            var isBrowser = link.Name.IndexOf("chrome", StringComparison.OrdinalIgnoreCase) >= 0;
            var isSyms = link.Name.IndexOf("-syms", StringComparison.OrdinalIgnoreCase) >= 0;
            var isAndroid = link.Name.IndexOf("-android", StringComparison.OrdinalIgnoreCase) >= 0;
            if (!isSyms && !isAndroid)
            {
                if (isBrowser && !isChromeDriver)
                    ret.Add(new DownloadLink() { Name = "chrome", Url = link.Url });

                if (isChromeDriver)
                    ret.Add(new DownloadLink() { Name = "chromedriver", Url = link.Url });
            }
        }
        return ret;
    }


    [JsonProperty("version")]
    public string RawVersion;
    [JsonProperty("platform")]
    public ChromeAndDriverPlatform Platform;
        


}