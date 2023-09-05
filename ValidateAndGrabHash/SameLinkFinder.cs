using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ValidateAndGrabHash.StaticMetadata;

namespace ValidateAndGrabHash
{
    internal class SameLinkFinder
    {
        public static string Process()
        {
            var root = SourceChromiumLinksParser.ParseAsJObject();
            List<JProperty> links = SourceChromiumLinksParser.FindLinks(root).ToList();

            StringBuilder ret = new StringBuilder();
            var groupsBuUrl = links.ToLookup(x => x.Value.ToString());
            foreach (var group in groupsBuUrl)
            {
                var first = group.First();
                var versions = group.Select(x => (x.Parent.Parent?.Parent?.Parent as JProperty)?.Name + "-"+ (x.Parent.Parent as JProperty)?.Name + "-" + x.Name).OrderByDescending(x => TryParseVersion(x)).ToList();
                if (versions.Count > 1)
                {
                    var reportLine = $"[{string.Join(", ", versions)}]: '{group.Key}'";
                    ret.AppendLine(reportLine);
                }
            }

            File.WriteAllText("same-links.txt", ret.ToString());
            Console.WriteLine($"SAME LINKS: {Environment.NewLine}{ret}");
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
}
