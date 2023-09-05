using ValidateAndGrabHash.LinksValidator;
using ValidateAndGrabHash.StaticMetadata;

namespace ValidateAndGrabHash
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var root = LinksParser.ParseAsJObject();
            var links = LinksParser.FindLinks(root).ToList();

            var urls = links.Select(x => x.Value.ToString());
            Console.WriteLine($"Links: {links.Count}. Unique: {urls.Distinct().Count()}");

            List<ValidationResult> resultsList = CachedLinksValidator.ParallelValidate(urls);
            Dictionary<string, ValidationResult> resultsDictionary = resultsList.ToDictionary(x => x.Url, x => x);

        }
    }
}