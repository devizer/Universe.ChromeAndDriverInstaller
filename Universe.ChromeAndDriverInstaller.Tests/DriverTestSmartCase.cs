using System;
using System.Collections.Generic;
using System.Threading;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class DriverTestSmartCase
    {
        public static DriverTestSmartCase CreatePreinstalledCase() => new DriverTestSmartCase() { UsePreinstalledChrome = true };
        public static DriverTestSmartCase CreateStableCase() => new DriverTestSmartCase() { MajorVersionDownload = null };
        public static DriverTestSmartCase CreateSpecificVersionCase(int majorVersion) => new DriverTestSmartCase() { MajorVersionDownload = majorVersion };


        private static Lazy<string> _StableVersion = new Lazy<string>(GetStableVersion, LazyThreadSafetyMode.ExecutionAndPublication);
        private static Lazy<string> _PreinstalledVersion = new Lazy<string>(CurrentChromeVersionClient.GetRawVersion, LazyThreadSafetyMode.ExecutionAndPublication);

        private static Lazy<List<ChromeOrDriverEntry>> _Entries = new Lazy<List<ChromeOrDriverEntry>>(() =>
        {
            return new SmartChromeAndDriverMetadataClient().Read();
        }, LazyThreadSafetyMode.ExecutionAndPublication);



        public bool UsePreinstalledChrome { get; private set; }
        public int? MajorVersionDownload { get; private set; }

        private string _AsString;

        public ChromeOrDriverEntry ChromeEntry => throw new NotImplementedException();
        public ChromeOrDriverEntry ChromeDriverEntry => throw new NotImplementedException();

        public override string ToString()
        {
            return _AsString;
        }



        private void Populate()
        {
            if (_AsString != null) return;
            if (UsePreinstalledChrome)
            {
                string preinstalledRawVersion = _PreinstalledVersion.Value;
                if (preinstalledRawVersion != null)
                {
                    _AsString = $"Preinstalled Chrome {preinstalledRawVersion}";
                }
                else
                {
                    MajorVersionDownload = new Version(_StableVersion.Value).Major;
                    _AsString = $"Stable Version {_StableVersion.Value} (missing pre-installed chrome)";
                }
            }
            else
            {
                if (MajorVersionDownload.HasValue)
                {
                    _AsString = $"Specific Chrome {_Entries.Value.FindByVersion(MajorVersionDownload.Value, ChromeOrDriverType.Chrome)}";
                }
                else
                {
                    int majorVersionDownload = new Version(_StableVersion.Value).Major;
                    _AsString = $"Stable Chrome {_Entries.Value.FindByVersion(majorVersionDownload, ChromeOrDriverType.Chrome)}";
                }
            }
        }

        static string GetStableVersion()
        {
            return new LastKnownGoodVersionsClient().ReadVersions()?.TryGetStableVersion().ToString();
        }

    }
}