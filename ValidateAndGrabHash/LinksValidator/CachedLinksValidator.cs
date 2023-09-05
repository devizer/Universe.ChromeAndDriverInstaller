using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Universe.ChromeAndDriverInstaller;
using Universe.Shared;

namespace ValidateAndGrabHash.LinksValidator
{
    internal static class CachedLinksValidator
    {
        private static ConcurrentDictionary<string, ValidationResult> Cache = new();
        private const string MAX_PARSED_PAGES = "MAX_PARSED_PAGES";

        public static List<ValidationResult> ParallelValidate(IEnumerable<string> urlList)
        {
            
            ParallelOptions po = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount + 1 };
            ConcurrentBag<ValidationResult> ret = new ConcurrentBag<ValidationResult>();
            var uniqueUrls = urlList.Distinct().OrderBy(x => x).ToList();
            var maxParsedPages = GetMaxParsedPages();
            int total = uniqueUrls.Count, current = 0;
            Stopwatch sw = Stopwatch.StartNew();
            Parallel.ForEach(uniqueUrls, po, url =>
            {
                string totalElapsed = "";
                var theCurrent = Interlocked.Increment(ref current);
                if (!maxParsedPages.HasValue || theCurrent <= maxParsedPages.Value)
                {
                    long msec = sw.ElapsedMilliseconds;
                    if (msec > 0)
                        totalElapsed =
                            $"total {FormatMilliseconds(msec)}, elapsed {FormatMilliseconds((long)((total - theCurrent) * 1.0d / (double)theCurrent * (double)msec))}";

                    var progresHuman = $"{theCurrent}/{total} {totalElapsed}: {url}";
                    Console.WriteLine($"→→ {progresHuman}");

                    ret.Add(Validate(url));
                }
            });

            return new List<ValidationResult>(ret);
        }

        public static ValidationResult Validate(string url)
        {
            if (Cache.TryGetValue(url, out var ret))
                return ret;

            var dirTemp = Env.ChromeDownloadDir;
            if (dirTemp == null)
                dirTemp = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Downloads");
            else
                dirTemp = Path.Combine(dirTemp, "_");

            var localFile = Path.Combine(dirTemp, EscapeFileName(Path.GetFileName(url)));

            try
            {
                Console.WriteLine($"Local File: {localFile}");
                TryAndRetry.Exec(() => Directory.CreateDirectory(Path.GetDirectoryName(localFile)));
                Stopwatch start1File = Stopwatch.StartNew();
                new WebDownloader().DownloadFile(url, localFile, retryCount: 3);
                var msec1File = start1File.ElapsedMilliseconds;
                long size1File = File.Exists(localFile) ? new FileInfo(localFile).Length : 0;
                if (msec1File > 0 && size1File > 0)
                {
                    Console.WriteLine($"Download speed for {size1File / 1024d:n0}K file: {size1File / 1024d / (msec1File / 1000d):n0} KB/s");
                }

                SHA1 sha1 = SHA1.Create();
                byte[] hash;
                using (FileStream fs = new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    hash = sha1.ComputeHash(fs);

                ret = new ValidationResult
                {
                    ErrorInfo = null,
                    LocalFullPath = localFile,
                    SHA1 = string.Join("", hash.Select(x => x.ToString("X2"))),
                    Url = url
                };
            }
            catch (Exception ex)
            {
                ret = new ValidationResult
                {
                    ErrorInfo = ex.GetLegacyExceptionDigest(),
                    LocalFullPath = localFile,
                    SHA1 = null,
                    Url = url
                };
                Console.WriteLine($"Validation failed. {ret.ErrorInfo}");
            }
            finally
            {
                var delEx = TryAndRetry.Exec(() => File.Delete(localFile));
                if (delEx != null)
                    Console.WriteLine($"WARNING! Unable to delete '{localFile}': {delEx.GetLegacyExceptionDigest()}");
            }

            Cache[url] = ret;
            return ret;
        }

        private static string EscapeFileName(string fileName)
        {
            return fileName?.Replace("?", "-");
        }

        static int? GetMaxParsedPages()
        {
            if (Int32.TryParse(Environment.GetEnvironmentVariable(MAX_PARSED_PAGES), out var maxParsedPages))
                return maxParsedPages > 0 ? maxParsedPages : null;

            return null;
        }

        static string FormatMilliseconds(long milliseconds)
        {
            return new DateTime().AddMilliseconds(milliseconds).ToString("HH:mm:ss");
        }
    }


    public class ValidationResult
    {
        public string Url { get; set; }
        public string LocalFullPath { get; set; }
        public string SHA1 { get; set; }
        public string ErrorInfo { get; set; } // Null
    }
}

    
