using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Universe.NUnitTests;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class GetNixChromeVersionRegression : NUnitTestsBase
    {
        [Test]
        public void Test30Seconds()
        {
            string chromeVersion = null;
            if (CrossInfo.ThePlatform == CrossInfo.Platform.Windows) return;
            Stopwatch sw = Stopwatch.StartNew();
            int n = 0;
            List<Exception> errors = new List<Exception>();
            while (sw.Elapsed.TotalSeconds < 30)
            {
                n++;
                try
                {
                    chromeVersion = CurrentChromeVersionClient.GetNixChromeVersion();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                    Console.WriteLine($"Total Errors {errors.Count} of {n}{Environment.NewLine}{e}");
                }
            }

            Console.WriteLine($"CurrentChromeVersionClient.GetNixChromeVersion() is finished. Total Errors {errors.Count} of {n}. Duration is {sw.Elapsed.TotalSeconds:n1} seconds. Version is {chromeVersion}");

            if (errors.Count > 0)
                throw new Exception($"Total Errors {errors.Count} of {n}", errors.First());
        }
    }
}
