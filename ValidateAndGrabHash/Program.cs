using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValidateAndGrabHash.LinksValidator;
using ValidateAndGrabHash.StaticMetadata;

namespace ValidateAndGrabHash
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SameLinkFinder.Process();
            var root = SourceChromiumLinksParser.ParseAsJObject();
            var links = SourceChromiumLinksParser.FindLinks(root).ToList();

            var urls = links.Select(x => x.Value.ToString());
            Console.WriteLine($"Links: {links.Count}. Unique: {urls.Distinct().Count()}");

            List<ValidationResult> resultsList = CachedLinksValidator.ParallelValidate(urls);
            Dictionary<string, ValidationResult> resultsDictionary = resultsList.ToDictionary(x => x.Url, x => x);

            // Replace JProperty by JObject with to properties: url and sha1
            foreach (JProperty link in links)
            {
                var url = link.Value.ToString();
                resultsDictionary.TryGetValue(url, out var validationResult);
                JObject newValue = JObject.FromObject(new {url = url});
                if (validationResult?.SHA1 != null) newValue["sha1"] = validationResult?.SHA1;
                if (!string.IsNullOrEmpty(validationResult?.ErrorInfo)) newValue["error"] = validationResult?.ErrorInfo;

                link.Value = newValue;
            }

            File.WriteAllText("chromium-and-drivers-with-hash.json", root.ToString(Formatting.Indented));
        }
    }
}