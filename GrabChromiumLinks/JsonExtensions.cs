using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GrabChromiumLinks;

public static class JsonExtensions
{
    // public static string Format(JObject json, bool intended, )
    private static readonly DefaultContractResolver TheContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy
        {
            OverrideSpecifiedNames = false,
            ProcessDictionaryKeys = true,
        },
    };

    public static string Format(JObject jObject, bool Minify)
    {

        StringBuilder b = new StringBuilder();
        using StringWriter wr = new StringWriter(b);
        using JsonTextWriter jwr = new JsonTextWriter(wr);
        if (Minify)
            jwr.Formatting = Formatting.None;
        else
        {
            jwr.Formatting = Formatting.Indented;
            jwr.IndentChar = '\t';
            jwr.Indentation = 1;
        }


        JsonSerializer ser = new JsonSerializer()
        {
            Formatting = !Minify ? Formatting.Indented : Formatting.None,
        };

        ser.Serialize(jwr, jObject);
        jwr.Flush();
        string ret = b.ToString();
        return ret;

    }

}