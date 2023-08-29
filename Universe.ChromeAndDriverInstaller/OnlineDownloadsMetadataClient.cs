using System;
using System.Collections.Generic;
using System.Text;
using Universe.Shared;

namespace Universe.ChromeAndDriverInstaller
{
    public static class OnlineDownloadsMetadataClient
    {
        private static UTF8Encoding Utf8 = new UTF8Encoding(false);
        
        public static string GetOnline()
        {
            WebDownloader wd = new WebDownloader();
            var bytes = wd.DownloadContent(ChromeAndDriverAxioms.DownloadsMetadataEndpoint, 3);
            return Utf8.GetString(bytes);
        }
    }
}
