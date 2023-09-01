namespace Universe.ChromeAndDriverInstaller
{
    public class ChromeOrDriverResult
    {
        public ChromeOrDriverEntry Metadata { get; set; }
        public string ExecutableFullPath { get; set; }
        public string ArchiveFullPath { get; set; }
        public string ExtractedFullPath { get; set; }

        public override string ToString()
        {
            return $"{Metadata}, Executable: {ExecutableFullPath}, ZIP: {ArchiveFullPath}, Extracted To: {ExtractedFullPath}";
        }
    }

}
