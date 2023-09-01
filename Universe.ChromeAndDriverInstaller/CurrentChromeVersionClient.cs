using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            return TryAndRetry.Eval(() => GetNixChromeVersion_Impl(), null, 3);
        }

        private static string GetNixChromeVersion_Impl()
        {
            // TODO: Chrome for Enterprise?
            string exe = null;
            if (CrossInfo.ThePlatform == CrossInfo.Platform.MacOSX)
            {
                var candidates = new[]
                {
                    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                    "/Applications/Google Chrome for Testing.app/Contents/MacOS/Google Chrome for Testing",
                };

                foreach (var candidate in candidates)
                {
                    if (File.Exists(candidate)) { exe = candidate; break; }
                }

                if (exe == null)
                {
                    // No Any Chrome Found
                    return null;
                }
            }
            else exe = "google-chrome"; // Linux


            ExecProcessHelper.ExecResult result;
            try
            {
                result = ExecProcessHelper.HiddenExec(exe, "--version");
            }
            catch (Win32Exception w32ex)
            {
                // Linux & Mac: NativeError 2. Error -2147467259. HResult -2147467259
                if (w32ex.NativeErrorCode == 2) return null;
                throw new InvalidOperationException($"Missing chrome. NativeError {w32ex.NativeErrorCode}. Error {w32ex.ErrorCode}. HResult {w32ex.HResult}");
            }

            result.DemandGenericSuccess("Query Google Chrome version (google-chrome --version)", false);
            return ParseVersionByChromeOutput(result.OutputText);
        }

        public static string ParseVersionByChromeOutput(string output)
        {
            string firstLine = output.Split(new[] { '\r', '\n' }).FirstOrDefault(x => x.Trim().Length > 0);
            string[] words = firstLine?.Split(' ').Where(x => x.Length > 0).ToArray();
            if (words == null)
                throw new InvalidOperationException($"Can't obtain chrome version. Output is '{output}'");

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
