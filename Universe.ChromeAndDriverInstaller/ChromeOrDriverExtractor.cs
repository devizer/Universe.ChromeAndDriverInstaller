﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Universe.ChromeAndDriverInstaller
{
    public static class ChromeOrDriverExtractor
    {
        // return executable full path;
        public static string Extract(ChromeOrDriverEntry entry, string zipFile, string targetDir)
        {
            // ZipArchive 
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }

            TryAndRetry.Exec(() => Directory.CreateDirectory(targetDir));
            // ZipFile Is not supported by Net 4.0
            ZipFile.ExtractToDirectory(zipFile, targetDir);
            string[] candidates = new[] { "chromedriver.exe", "chromedriver" };
            if (entry.Type == ChromeOrDriverType.Driver)
                candidates = new[] { "chromedriver.exe", "chromedriver" };
            else if (entry.Type == ChromeOrDriverType.Chrome)
                candidates = new[] { "chrome.exe", "chrome", "Google Chrome for Testing" };

            var allFiles = EnumFiles(targetDir);
            var foundList = allFiles.Where(x => candidates.Any(c => c.Equals(Path.GetFileName(x), StringComparison.OrdinalIgnoreCase)));
            var found = foundList.FirstOrDefault();
            if (found == null)
            {
                throw new InvalidOperationException($"Can't find any candidate [{string.Join(", ", candidates)}] in archive '{zipFile}'");
            }

            return found;
        }

        static IEnumerable<string> EnumFiles(string path)
        {
            var thisDir = new DirectoryInfo(path);
            foreach (var file in thisDir.GetFiles())
            {
                yield return file.FullName;
            }

            DirectoryInfo[] dirs = thisDir.GetDirectories();
            foreach (var dir in dirs)
            {
                var subFiles = EnumFiles(dir.FullName);
                foreach (var subFile in subFiles)
                {
                    yield return subFile;
                }
            }
        }
    }
}
