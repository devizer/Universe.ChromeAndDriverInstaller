using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Universe.Shared;

namespace Universe.ChromeAndDriverInstaller
{
    public class DependenciesClient
    {
        public static readonly string EndpointV1 = "https://omahaproxy.appspot.com/deps.json?version={0}";
        
        public static ChromiumDependenciesMetadata GetChromiumDependenciesMetadata(string version)
        {
            var url = string.Format(EndpointV1, version);
            var jsonString = new WebDownloader().DownloadContent(url, 3);
            JObject json = JObject.Parse(new UTF8Encoding(false).GetString(jsonString));

            ChromiumDependenciesMetadata ret = new ChromiumDependenciesMetadata()
            {
                Version = version,
                V8Version = json["v8_version"]?.ToString(),
                SkiaCommit = json["skia_commit"]?.ToString(),
            };

            bool isValid = !string.IsNullOrEmpty(ret.Version)
                           && !string.IsNullOrEmpty(ret.SkiaCommit)
                           && !string.IsNullOrEmpty(ret.V8Version);

            return isValid ? ret : null;
        }
    }

    public class ChromiumDependenciesMetadata
    {
        public string Version { get; set; }
        public string V8Version { get; set; }
        public string SkiaCommit { get; set; }
    }
}
