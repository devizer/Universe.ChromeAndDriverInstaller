using System;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class OnlineMetadataTests : NUnitTestsBase
    {
        [Test]
        public void DownloadOnline()
        {
            var rawJson = OnlineDownloadsMetadataClient.GetOnline();
            Console.WriteLine($"json.Length: {rawJson.Length}");
            Console.WriteLine($"json: {rawJson}");
        }
    }
}