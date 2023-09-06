using System;
using Universe.ChromeAndDriverInstaller.StaticallyCached.LegacyChromeDriver;

namespace Universe.ChromeAndDriverInstaller
{

    public class LegacyChromedriver2xEntry
    {
        private static Lazy<LegacyChromedriver2xEntry[]> _Entries = new Lazy<LegacyChromedriver2xEntry[]>(LegacyChromedriver2xParser.Parse);

        public static LegacyChromedriver2xEntry[] Entries = _Entries.Value;

        public Version ChromeDriverVersion { get; set; }
        public int MinChromeVersion { get; set; }
        public int MaxChromeVersion { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(ChromeDriverVersion)}: {ChromeDriverVersion}, ChromeVersion: {MinChromeVersion} ... {MaxChromeVersion}, {Date.ToString("yyyy-MM-dd")}";
        }
    }
}