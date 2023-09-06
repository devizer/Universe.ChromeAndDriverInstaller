namespace Universe.ChromeAndDriverInstaller
{
    public class ChromiumCDNEntry : ChromeOrDriverEntry
    {
        // stable/beta/pre-release/archive
        public string RawStatus { get; set; }
        public string V8Version { get; set; }
        public string SHA1 { get; set; }

        public ChromeOrDriveVersionStatus Status => DownloadsMetadataParser.ParseVersionStatus(RawStatus);

        public override string ToString()
        {
            return $"[{(Milestone == 0 ? RawMilestone : Milestone.ToString())}], {(Type == ChromeOrDriverType.Chrome ? "Chromium" : Type.ToString())} on {Platform} v{(Version?.Major > 0 ? Version.ToString() : RawVersion)}{(RawStatus == null ? "" : $":{RawStatus}")} '{(Uri == null ? Url : Uri.ToString())}' sha1={(SHA1 == null ? "null" : SHA1)}";
        }

    }
}