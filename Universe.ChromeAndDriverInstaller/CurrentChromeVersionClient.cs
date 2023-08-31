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

            // TODO: Exception if missing chrome 
            return GetNixChromeVersion();
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

        public static string GetNixChromeVersion()
        {
            var exe = CrossInfo.ThePlatform == CrossInfo.Platform.MacOSX
                ? "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
                : "google-chrome";

            var result = ExecProcessHelper.HiddenExec(exe + "-missing", "--version");
            result.DemandGenericSuccess("Query Google Chrome version (google-chrome --version)", false);
            return ParseVersionByChromeOutput(result.OutputText);
        }

        public static string ParseVersionByChromeOutput(string output)
        {
            string firstLine = output.Split(new[] { '\r', '\n' }).FirstOrDefault(x => x.Trim().Length > 0);
            string[] words = firstLine?.Split(' ').Where(x => x.Length > 0).ToArray();
            if (words == null)
                throw new InvalidOperationException("Can't obtain chrome version");

            for (int i = 0; i < words.Length; i++)
            {
                var ret = TryParseVersion(words[i]);
                if (ret != null) return ret.ToString();
            }

            return null;
        }

        public static Version TryParseVersion(string word)
        {
            StringBuilder raw = new StringBuilder();
            foreach (var ch in word)
            {
                if ((ch >= '0' && ch <= '9') || ch == '.')
                    raw.Append(ch);

                else break;
            }

            var candidate = raw.ToString();
            if (candidate.Count(x => x == '.') >= 2)
            {
                try
                {
                    return new Version(candidate);
                }
                catch {}

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
