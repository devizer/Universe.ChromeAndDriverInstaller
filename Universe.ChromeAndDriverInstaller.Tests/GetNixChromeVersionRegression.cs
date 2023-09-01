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
            if (CrossInfo.ThePlatform != CrossInfo.Platform.Linux) return;
            Stopwatch sw = Stopwatch.StartNew();
            int n = 0;
            while (sw.Elapsed.TotalSeconds < 30)
            {
                string chromeVersion = CurrentChromeVersionClient.GetNixChromeVersion();
                n++;
                Assert.IsTrue(!string.IsNullOrEmpty(chromeVersion), $"{n}: missing chrome version");
            }

            Console.WriteLine($"CurrentChromeVersionClient.GetNixChromeVersion() is successful {n} times during {sw.Elapsed.TotalSeconds:n1} seconds");
        }
    }
}
