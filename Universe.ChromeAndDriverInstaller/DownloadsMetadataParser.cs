using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Universe.ChromeAndDriverInstaller
{
    public static class DownloadsMetadataParser
    {
        public static List<ChromeOrDriverEntry> Parse(string rawJson)
        {
            JObject jsonRoot = JObject.Parse(rawJson);
            JObject jsonMilestones = jsonRoot["milestones"] as JObject;
            List<ChromeOrDriverEntry> ret = new List<ChromeOrDriverEntry>();
            foreach (KeyValuePair<string, JToken> jsonMilestonePair in jsonMilestones)
            {
                string key = jsonMilestonePair.Key;
                JObject jsonMilestone = jsonMilestonePair.Value as JObject;
                Console.WriteLine($"milestone json key = {key}");
            }

            return ret;
        }
    }
}