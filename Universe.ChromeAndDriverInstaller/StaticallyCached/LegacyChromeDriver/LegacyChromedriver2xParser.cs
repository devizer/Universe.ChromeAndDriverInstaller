using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Universe.ChromeAndDriverInstaller.StaticallyCached.LegacyChromeDriver
{
    public static class LegacyChromedriver2xParser
    {
        public static LegacyChromedriver2xEntry[] Parse()
        {
            List<LegacyChromedriver2xEntry> ret = new List<LegacyChromedriver2xEntry>();
            var fullName = typeof(LegacyChromedriver2xParser).Namespace + "." + "legacy-chromedriver-2x-versions.json";
            var rawJson = EmbeddedResourcesHelper.ReadEmbeddedResource(fullName);
            JObject json = JObject.Parse(rawJson);
            foreach (JProperty jVersion in json.Properties())
            {
                var ver = new Version(jVersion.Name);
                var versionValue = jVersion.Value as JObject;
                var rawFrom = versionValue?["from"]?.Value<string>();
                var rawTo = versionValue?["to"].Value<string>();
                var rawDate = versionValue?["date"].Value<string>();
                Int32.TryParse(rawFrom, out var minChromeVersion);
                Int32.TryParse(rawTo, out var maxChromeVersion);
                DateTime.TryParseExact(rawDate, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var date);
                LegacyChromedriver2xEntry entry = new LegacyChromedriver2xEntry()
                {
                    MinChromeVersion = minChromeVersion,
                    MaxChromeVersion = maxChromeVersion,
                    Date = date,
                    ChromeDriverVersion = ver,
                };

                if (entry.MinChromeVersion > 0 
                    && entry.MaxChromeVersion > 0
                    && entry.ChromeDriverVersion.Major > 0
                    && entry.Date.Year > 1900)
                    ret.Add(entry);
            }

            return ret.ToArray();
        }

    }
}
