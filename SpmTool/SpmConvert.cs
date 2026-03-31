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

    public class SpmConvert
    {
        public static string DX9To(string dx9Spm)
        {
            string xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            XPathDocument doc = new XPathDocument(reader);

            string modelName = null;
            var nameNode = doc.CreateNavigator().SelectSingleNode("/SPM/Spektrum/Name");
            if (nameNode != null)
            {
                modelName = nameNode.Value;
                modelName = getShortName(modelName);
            }

            XslCompiledTransform transform = findTransform("SpmTool.DX9toDX8.xslt");
            var writer = new StringWriter();
            var args = new XsltArgumentList();
            if (modelName != null)
            {
                args.AddParam("modelName", "", modelName);
            }

            transform.Transform(doc, args, writer);

            var dx8Xml = writer.ToString();
            var dx8Spm = XmlToSpm.Convert(dx8Xml);
            return dx8Spm;
        }

        static string getShortName(string name)
        {
            name = getNameWithoutNumber(name);
            if (name.Length > 10)
            {
                name = name.Substring(0, 10);
            }

            return name;
        }

        static string getNameWithoutNumber(string name)
        {
            int index = name.IndexOf(": ");
            if (index == -1) return name;

            string num = name.Substring(0, index);
            int result;
            if (!int.TryParse(num, out result)) return name;

            return name.Substring(index + 2);
        }

        public static string DX8To(string dx8Spm, string generator = "DX9", string modelName = null, string masterVolume = null)
        {
            string xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            XPathDocument doc = new XPathDocument(reader);

            XslCompiledTransform transform = findTransform("SpmTool.DX8toDX9.xslt");

            var writer = new StringWriter();
            var args = new XsltArgumentList();
            if (modelName != null)
            {
                args.AddParam("modelName", "", modelName);
            }

            if (masterVolume != null)
            {
                args.AddParam("masterVolume", "", masterVolume);
            }

            args.AddParam("generator", "", generator);

            transform.Transform(doc, args, writer);

            var dx9Xml = writer.ToString();
            return XmlToSpm.Convert(dx9Xml);
        }

        public static string FilterDX8(string dx8Spm)
        {
            string xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            XPathDocument doc = new XPathDocument(reader);

            XslCompiledTransform transform = findTransform("SpmTool.FilterDX8.xslt");
            var writer = new StringWriter();
            var args = new XsltArgumentList();

            transform.Transform(doc, args, writer);

            var dx8Xml = writer.ToString();
            return XmlToSpm.Convert(dx8Xml);
        }

        static XslCompiledTransform findTransform(string resourceName)
        {
            if (transforms.ContainsKey(resourceName))
            {
                return transforms[resourceName];
            }

            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings();
            settings.EnableScript = true;
            transform.Load(getXmlReader(resourceName), settings, null);
            transforms[resourceName] = transform;
            return transform;
        }

        static Dictionary<string, XslCompiledTransform> transforms = new Dictionary<string, XslCompiledTransform>();

        static XmlReader getXmlReader(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            return XmlReader.Create(stream);
        }
    }
}
