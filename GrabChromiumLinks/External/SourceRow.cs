using Universe.ChromeAndDriverInstaller;

namespace GrabChromiumLinks.External;

public class SourceRow
{
    public string RawVersion { get; set; }
    public string HtmlLink { get; set; }
    public ChromeAndDriverPlatform Platform { get; set; }

    public List<DownloadLink> DownloadLinks { get; set; } = new List<DownloadLink>();
    public string V8Version { get; set; }

    public Version Version => RawVersion.TryParseVersion();
}

public class DownloadLink
{
    public string Name;
    public string Url;

    public override string ToString()
    {
        return $"{Name}: {Url}";
    }
}
