using System;

namespace Universe.ChromeAndDriverInstaller
{
    public class ChromeOrDriverEntry
    {
        public string RawMilestone { get; set; }
        public string RawVersion { get; set; }
        public ChromeOrDriverType Type { get; set; }
        public ChromeAndDriverPlatform Platform { get; set; }
        public string Url { get; set; }

        public Uri Uri => TryAndRetry.Eval(() => new Uri(Url));
        public int Milestone => TryAndRetry.Eval(() => Int32.Parse(RawMilestone));
        public Version Version => TryAndRetry.Eval(() => new Version(RawVersion));


    }
}