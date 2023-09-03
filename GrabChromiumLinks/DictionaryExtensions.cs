using GrabChromiumLinks.External;

namespace GrabChromiumLinks;

public static class DictionaryExtensions
{


    public static Version TryParseVersion(this string rawVersion)
    {
        Version? ret;
        if (Version.TryParse(rawVersion, out ret))
            return ret;

        return null;
    }


    public static V GetOrAdd<K, V>(this IDictionary<K, V> dictionary, K key, Func<K, V> newValueFactory)
    {
        V value;
        if (!dictionary.TryGetValue(key, out value))
            dictionary[key] = value = newValueFactory(key);

        return value;
    }

    public static IDictionary<K, List<SourceRow>> ToDistinct<K>(this IEnumerable<SourceRow> sourceRows, Func<SourceRow, K> keyFactory)
    {
        Dictionary<K, List<SourceRow>> ret = new Dictionary<K, List<SourceRow>>();
        foreach (var sourceRow in sourceRows)
        {
            var key = keyFactory(sourceRow);
            ret.GetOrAdd(key, x => new List<SourceRow>()).Add(sourceRow);
        }

        return ret;
    }


}