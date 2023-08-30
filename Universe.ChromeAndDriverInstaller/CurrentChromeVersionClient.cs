using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Universe.ChromeAndDriverInstaller
{
    public class CurrentChromeVersionClient
    {
        public static string GetRawVersion()
        {
            if (TinyCrossInfo.IsWindows)
            {
                return GetWindowsChromeVersion();
            }

            return null;
        }

        public static int? TryGetMajorVersion()
        {
            var raw = GetRawVersion();
            try
            {
                return new Version(raw).Major;
            }
            catch
            {
                try
                {
                    return int.Parse(raw.Split('.').First());
                }
                catch { }
            }

            return null;
        }

        public static string GetWindowsChromeVersion()
        {
            var chromePath = GetWindowsChromePath();
            if (!string.IsNullOrEmpty(chromePath) && File.Exists(chromePath))
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(chromePath);
                return fileVersionInfo.FileVersion;
            }

            return null;
        }

        public static string GetWindowsChromePath()
        {
            string chromePath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", null, null);
            return chromePath;
        }
    }
}
