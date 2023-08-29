using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Universe.ChromeAndDriverInstaller.StaticallyCached
{
    public static class EmbeddedResourcesHelper
    {
        // Order matters
        public static List<string> FindEmbeddedResources(string prefix, string extension)
        {
            string[] allNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            List<string> ret = new List<string>();
            foreach (var name in allNames)
            {
                if (name.StartsWith(prefix) && name.EndsWith(extension)) ret.Add(name);
            }

            ret.Sort();
            ret.Reverse();
            return ret;
        }

        private static UTF8Encoding Utf8 = new UTF8Encoding(false);
        public static string ReadEmbeddedResource(string fullName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName))
            {
                using (var rdr = new StreamReader(stream, Utf8))
                {
                    return rdr.ReadToEnd();
                }
            }
        }
    }
}
