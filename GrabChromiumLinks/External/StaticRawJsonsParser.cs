using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using Universe.ChromeAndDriverInstaller;

namespace GrabChromiumLinks.External
{
    public static class StaticRawJsonsParser
    {
        public static string ReadFile(string key)
        {
            var fullName = typeof(StaticRawJsonsParser).Namespace +  $".version-position-link-{key}.json";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName))
            {
                if (stream == null) throw new InvalidOperationException($"Embedded file note found: {fullName} for {key}");
                using (StreamReader rdr = new StreamReader(stream, new UTF8Encoding(false)))
                {
                    return rdr.ReadToEnd();
                }

            }
        }

        public static StaticRawJsonFile ParseFile(string jsonString, ChromeAndDriverPlatform platform)
        {
            StaticRawJsonFile ret = new StaticRawJsonFile()
            {
                Platform = platform,
            };

            JObject jsonRoot = JObject.Parse(jsonString);
            foreach (var pair in jsonRoot)
            {
                SourceRow row = new SourceRow()
                {
                    HtmlLink = pair.Value?.ToString(),
                    RawVersion = pair.Key,
                    Platform = platform,
                };

                if (!string.IsNullOrEmpty(row.HtmlLink) && !string.IsNullOrEmpty(row.RawVersion))
                {
                    ret.Rows.Add(row);
                }
            }

            return ret;
        }

    }
}
