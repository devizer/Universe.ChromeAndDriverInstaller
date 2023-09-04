// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using GrabChromiumLinks;
using GrabChromiumLinks.External;
using Universe.ChromeAndDriverInstaller;

public class Program
{
    private const string MAX_PARSED_PAGES = "MAX_PARSED_PAGES";

    private static ThreadLocal<HtmlLinksParser> LinksParser = new ThreadLocal<HtmlLinksParser>();

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

        List<SourceRow> allRows = new List<SourceRow>();
        foreach (var metaFile in metaFiles)
        {
            Console.WriteLine($"Parsing {metaFile.Key}");
            string json = StaticRawJsonsParser.ReadFile(metaFile.Key);
            var parsedNewRows = StaticRawJsonsParser.ParseFile(json, metaFile.Plaform);
            allRows.AddRange(parsedNewRows);
        }


        int total = allRows.Count, current = 0;
        Console.WriteLine($"Total Html Pages: {total}");
        Stopwatch sw = Stopwatch.StartNew();
        var maxParsedPages = GetMaxParsedPages();
        var orderedSourceRows = allRows.OrderByDescending(x => x.RawVersion.TryParseVersion()).ToList();
        // foreach (SourceRow sourceRow in orderedSourceRows)
        int numThreads = Math.Min(3, Environment.ProcessorCount);
        Console.WriteLine($"Parsing Threads: {numThreads}");
        ParallelOptions po = new ParallelOptions() { MaxDegreeOfParallelism = numThreads};
        Parallel.ForEach(orderedSourceRows, po, sourceRow =>
        {
            string totalElapsed = "";
            var theCurrent = Interlocked.Increment(ref current);
            if (!maxParsedPages.HasValue || theCurrent <= maxParsedPages.Value)
            {
                long msec = sw.ElapsedMilliseconds;
                if (msec > 0)
                    totalElapsed =
                        $"total {FormatMilliseconds(msec)}, elapsed {FormatMilliseconds((long)((total - theCurrent) * 1.0d / (double)theCurrent * (double)msec))}";

                if (sourceRow.V8Version == null)
                {
                    sourceRow.V8Version = CachedV8Versions.GetCachedV8Version(sourceRow.RawVersion);
                    Console.WriteLine($"V8 Version of {sourceRow.RawVersion} is '{sourceRow.V8Version}'");
                }

                var progresHuman = $"{theCurrent}/{total} {totalElapsed} v{sourceRow.RawVersion} for {sourceRow.Platform} {sourceRow.HtmlLink}";
                Console.WriteLine($"→→ {progresHuman}");
                HtmlLinksParser linksParser = GetLinksParser();

                var links = linksParser.ParseLinks(sourceRow.HtmlLink, (driver, actualLinks) =>
                {
                    Thread.Sleep(1);
                    return IsValidLinks(actualLinks);
                });

                if (links == null) throw new Exception($"TIMEOUT for {sourceRow.HtmlLink}");

                // chrom[ium]?
                var filteredLinks = links.Where(x => x.Name.IndexOf(".zip", StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                sourceRow.DownloadLinks.AddRange(filteredLinks);

                var humanLinks = string.Join(Environment.NewLine, filteredLinks.Select(x => $"  {x}"));
                Console.WriteLine($"Completed v{sourceRow.RawVersion} for {sourceRow.Platform}" + Environment.NewLine + humanLinks);
            }
        });

        Console.WriteLine("FINISH. Serialize Results");
        foreach (var htmlLinksParser in LinksParsers)
            htmlLinksParser.Dispose();

        var utf8 = new UTF8Encoding(false);
        var fileNameSuffix = (GetMaxParsedPages().HasValue ? $" (max {GetMaxParsedPages()} pages)" : "");
        // Full (as is)
        string resultJsonFull = ChromiumMetadataJsonWriter.Serialize(allRows);
        File.WriteAllText($"chromium-and-drivers-full{fileNameSuffix}.json", resultJsonFull, utf8);
        // Only Chrome and Driver (public)
        string resultJson = ChromiumMetadataJsonWriter.Serialize(allRows, true);
        File.WriteAllText($"chromium-and-drivers{fileNameSuffix}.json", resultJson, utf8);
    }

    static int? GetMaxParsedPages()
    {
        if (Int32.TryParse(Environment.GetEnvironmentVariable(MAX_PARSED_PAGES), out var maxParsedPages))
            return maxParsedPages > 0 ? maxParsedPages : null;

        return null;
    }

    static bool IsValidLinks(List<DownloadLink> links)
    {
        return links.Any(x => x.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
    }

    static string FormatMilliseconds(long milliseconds)
    {
        return new DateTime().AddMilliseconds(milliseconds).ToString("HH:mm:ss");
    }

    private static ConcurrentBag<HtmlLinksParser> LinksParsers = new ConcurrentBag<HtmlLinksParser>();
    static HtmlLinksParser GetLinksParser()
    {
        if (LinksParser.Value == null)
        {
            var next = LinksParser.Value = new HtmlLinksParser();
            LinksParsers.Add(next);
        }

        return LinksParser.Value;
    }

}


