using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace Universe.ChromeAndDriverInstaller.Tests
{
    public class DriverTestSmartCase
    {
        public static DriverTestSmartCase CreatePreinstalledCase() => new DriverTestSmartCase() { UsePreinstalledChrome = true }.Populate();
        public static DriverTestSmartCase CreateStableCase() => new DriverTestSmartCase() { MajorVersionDownload = null }.Populate();
        public static DriverTestSmartCase CreateSpecificVersionCase(int majorVersion) => new DriverTestSmartCase() { MajorVersionDownload = majorVersion }.Populate();


        private static Lazy<string> _StableVersion = new Lazy<string>(GetStableVersion, LazyThreadSafetyMode.ExecutionAndPublication);
        private static Lazy<string> _PreinstalledVersion = new Lazy<string>(CurrentChromeVersionClient.GetRawVersion, LazyThreadSafetyMode.ExecutionAndPublication);

        private static Lazy<List<ChromeOrDriverEntry>> _Entries = new Lazy<List<ChromeOrDriverEntry>>(() =>
        {
            return new SmartChromeAndDriverMetadataClient().Read().Normalize();
        }, LazyThreadSafetyMode.ExecutionAndPublication);



        public bool UsePreinstalledChrome { get; private set; }
        public int? MajorVersionDownload { get; private set; }

        private string _AsString;

        private ChromeOrDriverEntry _ChromeMetadata;
        public ChromeOrDriverEntry ChromeMetadata => _ChromeMetadata == null ? throw new VersionNotFoundException() : _ChromeMetadata;

        private ChromeOrDriverEntry _ChromeDriverMetadata;
        public ChromeOrDriverEntry ChromeDriverMetadata => _ChromeDriverMetadata == null ? throw new VersionNotFoundException() : _ChromeDriverMetadata;

        public override string ToString()
        {
            return _AsString;
        }

        private DriverTestSmartCase Populate()
        {
            if (_AsString != null) return this;

            if (MajorVersionDownload == 49 && Debugger.IsAttached) Debugger.Break();

            int? actualMajor = null;
            if (UsePreinstalledChrome)
            {
                string preinstalledRawVersion = _PreinstalledVersion.Value;
                if (preinstalledRawVersion != null)
                {
                    _AsString = $"Preinstalled Chrome {preinstalledRawVersion}";
                    actualMajor = new Version(preinstalledRawVersion).Major;
                }
                else
                {
                    MajorVersionDownload = new Version(_StableVersion.Value).Major;
                    _AsString = $"Stable Version {_StableVersion.Value} (missing pre-installed chrome)";
                    actualMajor = MajorVersionDownload;
                }
            }
            else
            {
                if (MajorVersionDownload.HasValue)
                {
                    var found = _Entries.Value.FindByVersion(MajorVersionDownload.Value, ChromeOrDriverType.Chrome);
                    _AsString = $"Specific Chrome {(found == null ? $"Not Found v{MajorVersionDownload.Value}" : found.ToString())}";
                    actualMajor = MajorVersionDownload.Value;
                }
                else
                {
                    int majorVersionDownload = new Version(_StableVersion.Value).Major;
                    actualMajor = majorVersionDownload;
                    _AsString = $"Stable Chrome {_Entries.Value.FindByVersion(majorVersionDownload, ChromeOrDriverType.Chrome)}";
                }
            }

            if (actualMajor.HasValue)
            {
                _ChromeMetadata = _Entries.Value.FindByVersion(actualMajor.Value, ChromeOrDriverType.Chrome);
                // if (MajorVersionDownload == 71 && Debugger.IsAttached) Debugger.Break();
                _ChromeDriverMetadata = _Entries.Value.FindByVersion(actualMajor.Value, ChromeOrDriverType.Driver);
            }

            return this;
        }

        static string GetStableVersion()
        {
            return new LastKnownGoodVersionsClient().ReadVersions()?.TryGetStableVersion().ToString();
        }
    }

    public class DriverTestSmartCaseSource
    {
        public static List<DriverTestSmartCase> Famous => new List<DriverTestSmartCase>()
        {
            DriverTestSmartCase.CreateStableCase(),
            DriverTestSmartCase.CreatePreinstalledCase(),
            DriverTestSmartCase.CreateSpecificVersionCase(109),
            DriverTestSmartCase.CreateSpecificVersionCase(113),
            DriverTestSmartCase.CreateSpecificVersionCase(93),
            DriverTestSmartCase.CreateSpecificVersionCase(92),
            DriverTestSmartCase.CreateSpecificVersionCase(86),
            DriverTestSmartCase.CreateSpecificVersionCase(76),
            DriverTestSmartCase.CreateSpecificVersionCase(74),
            DriverTestSmartCase.CreateSpecificVersionCase(71),
            DriverTestSmartCase.CreateSpecificVersionCase(63),
            DriverTestSmartCase.CreateSpecificVersionCase(49),
        };
    }
}