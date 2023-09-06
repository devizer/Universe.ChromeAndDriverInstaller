using System.Reflection;
using Newtonsoft.Json.Linq;
using Stream = System.IO.Stream;

namespace NormalizeFinalJson.External
{
    internal class SourceChromiumLinksParser
    {
        public static JObject ParseAsJObject()
        {
            using(Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(SourceChromiumLinksParser).Namespace + ".chromium-and-drivers-with-hash (duplacates).json"))
            using (StreamReader sr = new StreamReader(s))
            {
                var jObject = JObject.Parse(sr.ReadToEnd());
                return jObject;
            }
        }

        public static IEnumerable<JProperty> FindLinks(JObject root)
        {
            var tokens = new List<JToken>();
            EnumJObject(root, tokens);
            foreach (var property in tokens.OfType<JProperty>())
            {
                string? pv = property.Value?.ToString();
                if (pv?.StartsWith("https://") == true)
                {
                    yield return property;
                }
            }
        }

        static void EnumJObject(JToken next, List<JToken> tokens)
        {
            foreach (var child in next.Children())
            {
                tokens.Add(child);
                EnumJObject(child, tokens);
            }
        }
    }
}
