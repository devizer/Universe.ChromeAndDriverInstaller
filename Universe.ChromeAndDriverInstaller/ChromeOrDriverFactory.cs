using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Universe.Shared;

namespace Universe.ChromeAndDriverInstaller
{
    public class ChromeOrDriverFactory
    {
        public static string TempFolder { get; set; }

        static readonly object SyncTempFolder = new object();
        static Dictionary<string, ChromeOrDriverResult> Cache = new Dictionary<string, ChromeOrDriverResult>(StringComparer.OrdinalIgnoreCase);
        static int Counter = 0;
        static readonly object SyncCache = new object();

        public static ChromeOrDriverResult DownloadAndExtract(int? majorVersion, ChromeOrDriverType type)
        {
            return DownloadAndExtract(majorVersion, type, ChromeAndDriverPlatform.Unknown);
        }

        public static ChromeOrDriverResult DownloadAndExtract(int? majorVersion, ChromeOrDriverType type, ChromeAndDriverPlatform platform)
        {
            if (majorVersion == null)
            {
                majorVersion = CurrentChromeVersionClient.TryGetMajorVersion();
                DebugConsole.WriteLine($"Local chrome detected: v{majorVersion}. Downloading {type}");
            }

            if (platform == ChromeAndDriverPlatform.Unknown)
            {
                platform = platform.ResolveCurrentPlatform();
                // DebugConsole.WriteLine($"Resolved current platform {CrossInfo.ThePlatform} --> {platform}");
            }

            if (majorVersion.HasValue)
            {
                var metadataClient = new SmartChromeAndDriverMetadataClient();
                var entries = metadataClient.Read();
                var metadataEntry = entries?.Normalize().FindByVersion(majorVersion.Value, type, platform);
                if (metadataEntry != null)
                {
                    // cache key
                    var uniqueName = $"{metadataEntry.Type}-{metadataEntry.RawVersion}-{metadataEntry.Platform}".ToLower();

                    ChromeOrDriverResult cached;
                    lock (SyncCache) Cache.TryGetValue(uniqueName, out cached);
                    if (cached != null) return cached;

                    string tempDir = GetTempFolder();
                    var counter = Interlocked.Increment(ref Counter);
                    tempDir = Path.Combine(tempDir, counter.ToString("0"));

                    var zipFileName = Path.Combine(tempDir, "Zips", uniqueName + ".zip");
                    var extractDir = Path.Combine(tempDir, uniqueName);
                    DebugConsole.WriteLine($"Extracting {metadataEntry} into [{extractDir}]");

                    var doneAnchorFile = Path.Combine(extractDir, uniqueName + ".done");
                    string exeFullPath = null;
                    if ((exeFullPath = TryReuse(doneAnchorFile)) == null) 
                    {
                        TryAndRetry.Exec(() => Directory.CreateDirectory(Path.GetDirectoryName(zipFileName)));
                        TryAndRetry.Exec(() => Directory.CreateDirectory(extractDir));

                        DebugConsole.WriteLine($"Downloading {metadataEntry}");
                        new WebDownloader().DownloadFile(metadataEntry.Url, zipFileName, retryCount: 3);
                        exeFullPath = ChromeOrDriverExtractor.Extract(metadataEntry, zipFileName, extractDir);
                        DebugConsole.WriteLine($"{metadataEntry}: {Environment.NewLine}{exeFullPath}");
                        using (FileStream fs = new FileStream(doneAnchorFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        using (StreamWriter wr = new StreamWriter(fs, new UTF8Encoding(false)))
                        {
                            wr.Write(exeFullPath);
                        }
                    }
                    else {
                        DebugConsole.WriteLine($"Reusing existing binary [{exeFullPath}]");
                    }

                    var ret = new ChromeOrDriverResult()
                    {
                        Metadata = metadataEntry,
                        ArchiveFullPath = zipFileName,
                        ExtractedFullPath = extractDir,
                        ExecutableFullPath = exeFullPath,
                    };

                    lock(SyncCache) Cache[uniqueName] = ret;
                    return ret;
                }
            }

            return null;
        }

        static string TryReuse(string doneAnchorFile)
        {
            if (File.Exists(doneAnchorFile))
            {
                using (FileStream fs = new FileStream(doneAnchorFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using(StreamReader rdr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    var firstLine = rdr.ReadLine();
                    if (firstLine != null && File.Exists(firstLine)) return firstLine;
                }
            }

            return null;
        }

        static string GetTempFolder()
        {
            if (TempFolder == null)
                lock(SyncTempFolder)
                    if (TempFolder == null)
                    {
                        string tempDir;
                        if (TinyCrossInfo.IsWindows)
                            tempDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Chrome-And-Drivers");
                        else
                            tempDir = "/tmp/chrome-and-drivers";

                        TempFolder = tempDir;
                    }

            return TempFolder;
        }
    }
}