using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LegacyParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var doc = LoadRawXml();
            LegacyXmlParser.Parse(doc);
        }

        static XmlDocument LoadRawXml()
        {
            string content = File.ReadAllText("Legacy Formatted.xml");
            content = content.Replace("xmlns='http://doc.s3.amazonaws.com/2006-03-01'", " ");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            return doc;
        }
    }
}
