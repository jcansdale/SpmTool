namespace SpmTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    public class XmlToSpm
    {
        public static string Convert(string xml)
        {
            var reader = new StringReader(xml);
            XPathDocument doc = new XPathDocument(reader);

            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings();
            transform.Load(getXmlReader("SpmTool.XmlToSpm.xsl"), settings, null);

            var writer = new StringWriter();
            var args = new XsltArgumentList();
            transform.Transform(doc, args, writer);
            return writer.ToString();
        }

        static XmlReader getXmlReader(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            return XmlReader.Create(stream);
        }
    }
}
