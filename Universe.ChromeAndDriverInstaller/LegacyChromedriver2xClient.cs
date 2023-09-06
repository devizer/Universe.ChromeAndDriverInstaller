using System;
using System.Linq;
using Universe.ChromeAndDriverInstaller.StaticallyCached.LegacyChromeDriver;

namespace Universe.ChromeAndDriverInstaller;

public static class LegacyChromedriver2xClient
{
    private static Lazy<LegacyChromedriver2xEntry[]> _Entries = new Lazy<LegacyChromedriver2xEntry[]>(LegacyChromedriver2xParser.Parse);

    public static LegacyChromedriver2xEntry[] Entries = _Entries.Value;

    public static LegacyChromedriver2xEntry[] FindChromeDriverVersion(int chromeVersion)
    {
        return LegacyChromedriver2xClient.Entries
            .Where(x => x.MinChromeVersion <= chromeVersion && x.MaxChromeVersion >= chromeVersion)
            .ToArray();
    }

}