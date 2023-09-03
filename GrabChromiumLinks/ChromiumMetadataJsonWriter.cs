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
    public static string Serialize(StaticRawJsonFile[] jsonFiles)
    {
        JObject ret = new JObject();

        var allRows = jsonFiles.SelectMany(x => x.Rows);
        var uniqueVersions = allRows.ToDistinct(x => x.RawVersion);

        foreach (var versionAndList in uniqueVersions.OrderByDescending(x => x.Key.TryParseVersion()))
        {
            var version = versionAndList.Key;
            var listByVersion = versionAndList.Value;

            var platformsByVersion = listByVersion.ToDistinct(x => x.Platform);

            JObject jPlatforms = JObject.FromObject(new { });
            string v8Ver = platformsByVersion.Values.SelectMany(x => x).FirstOrDefault(x => x.V8Version != null)?.V8Version;
            if (v8Ver != null) jPlatforms.Add("v8version", v8Ver);
            foreach (var platformByVersion in platformsByVersion.Keys.OrderBy(x => x.ToString()))
            {
                // JObject jPlatformValue = JObject.FromObject(new { });
                var links = platformsByVersion[platformByVersion];
                if (links.Count > 1) throw new InvalidOperationException("links.Count > 1");
                
                // JObject jPlatformValue = JObject.FromObject(new {html = links.First().HtmlLink});
                JObject jPlatformValue = JObject.FromObject(links.First().DownloadLinks.AsJObject());
                jPlatforms.Add(platformByVersion.ToString(), jPlatformValue);
            }
            var humanPlatformList = string.Join(", ", platformsByVersion.Select(x => x.Key.ToString()));
            ret.Add(version, jPlatforms);
        }

        return ret.ToString();
    }

        
        
    [JsonProperty("version")]
    public string RawVersion;
    [JsonProperty("platform")]
    public ChromeAndDriverPlatform Platform;
        


}