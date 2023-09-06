using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using NormalizeFinalJson.External;
using Universe.ChromeAndDriverInstaller;

namespace NormalizeFinalJson;

static class DuplicatesRemover
{
    public static string Process()
    {
        LastKnownGoodVersionsClient knownVersionClient = new LastKnownGoodVersionsClient();
        var knownVersions = knownVersionClient.ReadVersions();
        var stableVersion = knownVersions.TryGetStableVersion();

        var root = SourceChromiumLinksParser.ParseAsJObject();
        List<JProperty> links = SourceChromiumLinksParser.FindLinks(root).ToList();

        StringBuilder ret = new StringBuilder();
        var groupsBuUrl = links.ToLookup(x => x.Value.ToString());
        foreach (var group in groupsBuUrl)
        {
            var first = group.First();
            var versions = group.Select(x => (x.Parent.Parent?.Parent?.Parent?.Parent?.Parent as JProperty)?.Name + "-" + (x.Parent.Parent.Parent.Parent as JProperty)?.Name + "-" + (x.Parent.Parent as JProperty)?.Name).OrderByDescending(x => TryParseVersion(x)).ToList();
            if (versions.Count > 1)
            {
                var reportLine = $"[{string.Join(", ", versions)}]: '{group.Key}'";
                ret.AppendLine(reportLine);
            }

            // Removing [2...]
            foreach (string version in versions.Skip(1))
            {
                var pureVersion = version.Split('-').First();
                root.Remove(pureVersion);
            }
        }

        // Add Status
        foreach (var jVersion in root.Children().OfType<JProperty>())
        {
            var jVersionValue = (jVersion.Value as JObject);
            var version = TryParseVersion(jVersion.Name.ToString());
            string status = null;
            if (version.Major > knownVersions.TryGetStableVersion().Major) status = "pre-release";
            if (version == stableVersion) status = "stable";
            else if (version.Major == knownVersions.TryGetBetaVersion().Major) status = "beta";
            if (version.Major < knownVersions.TryGetStableVersion().Major) status = "archive";
            if (status == null && version.Major == knownVersions.TryGetStableVersion().Major) status = "stable";
            if (!string.IsNullOrEmpty(status))
            {
                var childs = new List<JProperty>(jVersionValue.Children<JProperty>());
                jVersionValue.RemoveAll();
                jVersionValue["status"] = status;
                foreach (var child in childs) jVersionValue.Add(child);
            }
            // jVersionValue.Remove("")
        }

        File.WriteAllText("same-links.txt", ret.ToString());
        Console.WriteLine($"SAME LINKS: {Environment.NewLine}{ret}");

        File.WriteAllText("chromium-and-drivers-with-hash (final).json", root.ToString());

        return ret.ToString();

    }

    public static Version TryParseVersion(string rawVersion)
    {
        Version? ret;
        if (Version.TryParse(rawVersion, out ret))
            return ret;

        return null;
    }

}