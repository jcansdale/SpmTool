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
            XPathDocument doc = loadSpmDocument(dx9Spm);

            string modelName = null;
            var nameNode = doc.CreateNavigator().SelectSingleNode("/SPM/Spektrum/Name");
            if (nameNode != null)
            {
                modelName = nameNode.Value;
                modelName = getShortName(modelName);
            }

            var args = new XsltArgumentList();
            if (modelName != null)
            {
                args.AddParam("modelName", "", modelName);
            }

            var dx8Xml = transformToString(doc, "SpmTool.DX9toDX8.xslt", args);
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
            XPathDocument doc = loadSpmDocument(dx8Spm);

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

            string dx9Spm = transformToString(doc, "SpmTool.DX8toDX9.xsl", args);
            return normalizeSpm(dx9Spm);
        }

        public static string FilterDX8(string dx8Spm)
        {
            XPathDocument doc = loadSpmDocument(dx8Spm);
            var dx8Xml = transformToString(doc, "SpmTool.FilterDX8.xslt", new XsltArgumentList());
            return XmlToSpm.Convert(dx8Xml);
        }

        static XPathDocument loadSpmDocument(string spm)
        {
            string xml = SpmToXml.Convert(spm);
            var reader = new StringReader(xml);
            return new XPathDocument(reader);
        }

        static string transformToString(XPathDocument doc, string resourceName, XsltArgumentList args)
        {
            XslCompiledTransform transform = findTransform(resourceName);
            var writer = new StringWriter();
            transform.Transform(doc, args, writer);
            return writer.ToString();
        }

        static string normalizeSpm(string spm)
        {
            return XmlToSpm.Convert(SpmToXml.Convert(spm));
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
