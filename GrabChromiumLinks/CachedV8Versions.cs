using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrabChromiumLinks.Links;
using Universe.ChromeAndDriverInstaller;

namespace GrabChromiumLinks
{
    internal class CachedV8Versions
    {
        private static ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();
        
        public static string GetCachedV8Version(string version)
        {
            if (!Cache.TryGetValue(version, out var v8Version))
            {
                try
                {
                    v8Version = DependenciesClient.GetChromiumDependenciesMetadata(version)?.V8Version;
                    Cache[version] = v8Version;
                }
                catch(Exception ex)
                {
                    DebugConsole.WriteLine($"WARNING! V8 Version is unavailable. {ex.GetLegacyExceptionDigest()}");
                }
            }

            return v8Version;
        }
    }
}
