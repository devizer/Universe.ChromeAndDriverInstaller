using System;
using System.Collections.Generic;
using Universe.ChromeAndDriverInstaller.StaticallyCached.LegacyChromeDriver;

namespace Universe.ChromeAndDriverInstaller
{

    public class LegacyChromedriver2xEntry
    {
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