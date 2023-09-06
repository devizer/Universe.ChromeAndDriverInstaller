namespace Universe.ChromeAndDriverInstaller
{
    public class ChromiumCDNEntry : ChromeOrDriverEntry
    {
        // stable/beta/pre-release/archive
        public string Status { get; set; }
        public string V8Version { get; set; }
        public string SHA1 { get; set; }
    }
}