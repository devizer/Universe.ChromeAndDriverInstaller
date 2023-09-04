using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LegacyParser
{
    internal class LegacyDriverNotesParser
    {
        public static void Parse()
        {
            var vvv = new Version(1, 12);
            var filesRaw = new DirectoryInfo("LegacyDriverNotes").GetFiles("*-notes.txt");
            var files = filesRaw.Select(x => new Row()
            {
                ReadmeFileVersion = new Version(x.Name.Split('-').FirstOrDefault()),
                FileName = x.Name,
            }).ToList();

            files = files.OrderByDescending(x => x.ReadmeFileVersion).ToList();

            File.WriteAllText("LegacyDriverNotes-files.json", JsonConvert.SerializeObject(files, Formatting.Indented, new VersionConverter()));


            List<Row> rows = new List<Row>();
            foreach (var file in files)
            {
                var fileLines = File.ReadAllLines(Path.Combine("LegacyDriverNotes", file.FileName));
                for (int i = 0; i < fileLines.Length - 1; i++)
                {
                    var newRow = TryParseMeta(fileLines[i], fileLines[i + 1]);
                    if (newRow == null) continue;
                    bool isAlready = rows.Any(x =>
                        x.DriverVersion == newRow.DriverVersion && x.MaxChromeVersion == newRow.MaxChromeVersion &&
                        x.MinChromeVersion == newRow.MinChromeVersion);

                    if (isAlready) continue;

                    rows.Add(new Row()
                    {
                        FileName = file.FileName,
                        DriverVersion = newRow.DriverVersion,
                        MaxChromeVersion = newRow.MaxChromeVersion,
                        MinChromeVersion = newRow.MinChromeVersion,
                        ReadmeFileVersion = file.ReadmeFileVersion,
                        DriverDate = newRow.DriverDate,
                    });
                }
            }

            File.WriteAllText("LegacyDriverNotes-final.json", JsonConvert.SerializeObject(rows, Formatting.Indented, new VersionConverter()));

            var prettyJson = rows.Select(x =>
                $"\t{{ \"{x.DriverVersion}\": {{ \"from\": {x.MinChromeVersion}, \"to\": {x.MaxChromeVersion}, \"date\": \"{x.DriverDate.ToString("yyyy-MM-dd")}\" }} }}");

            var prettyJson2 = $"[{Environment.NewLine}{string.Join($",{Environment.NewLine}", prettyJson)}{Environment.NewLine}]";
            File.WriteAllText("legacy-driver-versions.json", prettyJson2);


        }

        static ParsedMeta TryParseMeta(string line1, string line2)
        {
            var word1 = line1.Split(' ');
            var word2 = line2.Split(' ');
            if (word1.Length < 3) return null;
            if (!word1[0].EndsWith("--ChromeDriver", StringComparison.OrdinalIgnoreCase)) return null;
            if (!word1[0].StartsWith("---", StringComparison.OrdinalIgnoreCase)) return null;
            if (!word1[1].StartsWith("v")) return null;

            var driverVersion = TryVersionAsVersion(word1[1].Substring(1));


            var hasDate = word1[2].Length > 12 && word1[2][0] == '(' && word1[2].EndsWith("--");
            if (!hasDate) return null;

            DateTime.TryParse(word1[2].Substring(1, 10), out var driverDate);

            if (word2.Length < 3) return null;

            var hasChromeVersions = word2[0].StartsWith("Support") && word2[1].StartsWith("Chrom") && word2[2].StartsWith("v");
            if (!hasChromeVersions) return null;

            var rawMinMax = word2[2].Substring(1).Split('-');
            if (rawMinMax.Length < 2) return null;
            if (!int.TryParse(rawMinMax[0], out var minChromeVersion)) return null;
            if (!int.TryParse(rawMinMax[1], out var maxChromeVersion)) return null;

            return new ParsedMeta()
            {
                DriverDate = driverDate,
                DriverVersion = driverVersion,
                MaxChromeVersion = maxChromeVersion,
                MinChromeVersion = minChromeVersion,
            };
        }

        static Version TryVersionAsVersion(string arg)
        {
            if (Version.TryParse(arg, out var result)) return result;

            return null;
        }


        class ParsedMeta
        {
            public Version DriverVersion;
            public DateTime DriverDate;
            public int MinChromeVersion;
            public int MaxChromeVersion;
        }

        class Row
        {
            public Version ReadmeFileVersion;
            public string FileName;
            public Version DriverVersion;
            public int MinChromeVersion;
            public int MaxChromeVersion;
            public DateTime DriverDate;
        }


    }
}
