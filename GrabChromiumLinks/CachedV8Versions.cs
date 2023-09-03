﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                v8Version = DependenciesClient.GetChromiumDependenciesMetadata(version)?.V8Version;
                Cache[version] = v8Version;
            }

            return v8Version;
        }
    }
}
