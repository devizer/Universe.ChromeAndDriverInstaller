using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Stream = System.IO.Stream;

namespace ValidateAndGrabHash.StaticMetadata
{
    internal class LinksParser
    {
        public static JObject ParseAsJObject()
        {
            using(Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(LinksParser).Namespace + ".chromium-and-drivers.json"))
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
