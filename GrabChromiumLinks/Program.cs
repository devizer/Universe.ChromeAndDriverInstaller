// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using GrabChromiumLinks;
using GrabChromiumLinks.External;
using Universe.ChromeAndDriverInstaller;

public class Program
{
    public static void Main()
    {
        var metaFiles = new[]
        {
            new { Key = "Linux", Plaform = ChromeAndDriverPlatform.Linux32 },
            new { Key = "Linux_x64", Plaform = ChromeAndDriverPlatform.Linux64 },
            new { Key = "Mac", Plaform = ChromeAndDriverPlatform.MacX64 },
            new { Key = "Mac_Arm", Plaform = ChromeAndDriverPlatform.MacArm64 },
            new { Key = "Win", Plaform = ChromeAndDriverPlatform.Win32 },
            new { Key = "Win_x64", Plaform = ChromeAndDriverPlatform.Win64 },
        };

        List<StaticRawJsonFile> parsedFiles = new List<StaticRawJsonFile>();
        foreach (var metaFile in metaFiles)
        {
            Console.WriteLine($"Parsing {metaFile.Key}");
            string json = StaticRawJsonsParser.ReadFile(metaFile.Key);
            StaticRawJsonFile parsed = StaticRawJsonsParser.ParseFile(json, metaFile.Plaform);
            parsedFiles.Add(parsed);
        }

        bool IsValidLinks(List<DownloadLink> links)
        {
            return links.Any(x => x.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
        }

        string FormatMilliseconds(long milliseconds)
        {
            return new DateTime().AddMilliseconds(milliseconds).ToString("HH:mm:ss");
        }

        HtmlLinksParser linksParser = new HtmlLinksParser();
        int total = parsedFiles.SelectMany(x => x.Rows).Count(), current = 0;
        Console.WriteLine($"Total Html Pages: {total}");
        Stopwatch sw = Stopwatch.StartNew();
        var maxParsedPages = GetMaxParsedPages();
        foreach (SourceRow sourceRow in parsedFiles.SelectMany(x => x.Rows).OrderByDescending(x => x.RawVersion.TryParseVersion()))
        {
            string totalElapsed = "";
            ++current;
            if (maxParsedPages.HasValue && current > maxParsedPages.Value) break;
            long msec = sw.ElapsedMilliseconds;
            if (msec > 0) totalElapsed = $"total {FormatMilliseconds(msec)}, elapsed {FormatMilliseconds((long)((total - current) * 1.0d / current) * msec)}";
            Console.WriteLine($"{current}/{total} {totalElapsed} v{sourceRow.RawVersion} for {sourceRow.Platform} {sourceRow.HtmlLink}");
            var links = linksParser.ParseLinks(sourceRow.HtmlLink, (driver, actualLinks) =>
            {
                Thread.Sleep(1);
                return IsValidLinks(actualLinks);
            });

            var filteredLinks = links.Where(x => x.Name.IndexOf("chrom", StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            foreach (var link in filteredLinks)
            {
                Console.WriteLine($"  {link.Name}: {link.Url}");
                sourceRow.DownloadLinks.Add(link);
            }
        }

        string resultJson = ChromiumMetadataJsonWriter.Serialize(parsedFiles.ToArray());
        File.WriteAllText("chromium-and-drivers.json", resultJson, new UTF8Encoding(false));
    }

    static int? GetMaxParsedPages()
    {
        if (Int32.TryParse(Environment.GetEnvironmentVariable("MAX_PARSED_PAGES"), out var maxParsedPages))
            return maxParsedPages > 0 ? maxParsedPages : null;

        return null;
    }
}


