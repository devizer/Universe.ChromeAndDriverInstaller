using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_3
using System.Net.Http;
#endif
using System.Threading.Tasks;
using Universe.ChromeAndDriverInstaller;

namespace Universe.Shared
{
    public class WebDownloader
    {
        private const string _UserAgent = "chromedriver-downloader";

        public byte[] DownloadContent(string url, int retryCount)
        {
            Exception err = null;
            for (int i = 1; i <= retryCount; i++)
            {
                try
                {
                    return DownloadContent(url);
                }
                catch (Exception e)
                {
                    if (i < retryCount)
                        Console.WriteLine($"Warning. Retrying {i} of {retryCount} to download {url}");

                    if (i == retryCount) throw;
                }
            }

            throw new InvalidOperationException("Unable to download. Never goes here");
        }

        public byte[] DownloadContent(string url)
        {
            ConfigureCertificateValidation();

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_3
            return DownloadContentAsync(url).Result;
#else
            using (var wc = new System.Net.WebClient())
            {
                wc.Headers["User-Agent"] = _UserAgent;
                wc.Proxy = System.Net.WebRequest.DefaultWebProxy;
                return wc.DownloadData(new Uri(url));
            }
#endif
        }

        public void DownloadFile(string url, string toFile, int retryCount)
        {
            for (int i = 1; i <= retryCount; i++)
            {
                try
                {
                    DownloadFile(url, toFile);
                }
                catch (Exception e)
                {
                    TryAndRetry.Exec(() => File.Delete(toFile));
                    if (i<retryCount)
                        Console.WriteLine($"Warning. Retrying {i+1} of {retryCount} to download {url} as {toFile}");

                    if (i == retryCount) throw new InvalidOperationException($"Unable to download {url} as {toFile}", e);
                }
            }
        }

        public void DownloadFile(string url, string toFile)
        {
            ConfigureCertificateValidation();

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_3
            DownloadFileAsync(url, toFile).Wait();
#else
            
            using (var wc = new System.Net.WebClient())
            {
                wc.Headers["User-Agent"] = _UserAgent;
                wc.Proxy = System.Net.WebRequest.DefaultWebProxy;
                // wc.DownloadFile(new Uri(url), toFile);
                using (Stream net = wc.OpenRead(url))
                using (FileStream fs = new FileStream(toFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 64 * 1024))
                {
                    net.CopyTo(fs);
                }
            }
#endif
        }

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_3

        private async Task<byte[]> DownloadContentAsync(string url)
        {
            using (var client = new System.Net.Http.HttpClient(_ClientHandler.Value))
            {
                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        using (MemoryStream mem = new MemoryStream())
                        {
                            Stream stream = await result.Content.ReadAsStreamAsync();
                            await stream.CopyToAsync(mem);
                            return mem.ToArray();
                        }
                    }
                    else
                    {
                        throw new Exception($"{url} is not accessible. Status: {result.StatusCode}");
                    }
                }
            }
        }  
        
        private async Task DownloadFileAsync(string url, string toFile)
        {
            using (var client = new System.Net.Http.HttpClient(_ClientHandler.Value))
            {
                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        using (FileStream fs = new FileStream(toFile, FileMode.Create, FileAccess.Write,
                            FileShare.ReadWrite))
                        {
                            await result.Content.CopyToAsync(fs);
                        }
                    }
                    else
                    {
                        throw new Exception($"{url} is not accessible. Status: {result.StatusCode}");
                    }
                }
            }
        }
        
        private static Lazy<HttpClientHandler> _ClientHandler = new Lazy<HttpClientHandler>(() =>
            {
                var handler = new HttpClientHandler();
                handler.AllowAutoRedirect = true;
                handler.ServerCertificateCustomValidationCallback += (requestMessage, x509Certificate2, x509Chain, sslPolicyErrors) => true;
                return handler;
            }
        );

#endif

        private static bool IsCertificateValidationConfigured = false;
        static readonly object SyncCertificateValidation = new object();

        static void ConfigureCertificateValidation()
        {
            if (IsCertificateValidationConfigured) return;
            lock (SyncCertificateValidation)
            {
                if (IsCertificateValidationConfigured) return;
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_3
                // nothing can do here
#else
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };
#if NET45
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif
#endif
                IsCertificateValidationConfigured = true;
            }
        }
    }
}
