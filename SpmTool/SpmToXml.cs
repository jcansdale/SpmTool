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

    public class SpmToXml
    {
        public static string Convert(string text)
        {
            using (var reader = new StringReader(text))
            {
                var writer = new StringWriter();
                writer.WriteLine("<SPM>");
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null || line == "*EOF*") break;
                    string element = toXmlElement(line);
                    writer.WriteLine(element);
                }
                writer.WriteLine("</SPM>");
                return writer.ToString();
            }
        }

        static string toXmlElement(string line)
        {
            if (line.StartsWith(";")) return "";

            if (line.StartsWith("["))
            {
                var content = line.Substring(1, line.Length - 2);
                if (content.StartsWith("/"))
                {
                    return "<" + content + ">";
                }

                return "<" + content + " Type='Object'>";
            }

            var split = line.Split('=');
            if (split.Length > 1)
            {
                var name = split[0];
                var str = split[1];

                if (name.StartsWith("*"))
                {
                    name = name.Substring(1);
                    str = str.TrimStart(' ');
                    return "<" + name + " Type='Index'>" + str + "</" + name + ">";
                }

                if (str.StartsWith(@"""") && str.EndsWith(@""""))
                {
                    str = str.Substring(1, str.Length - 2);
                    return "<" + name + " Type='String'>" + str + "</" + name + ">";
                }

                str = str.TrimStart(' ');
                return "<" + name + ">" + str + "</" + name + ">";

            }

            split = line.Split(':');
            if (split.Length > 1)
            {
                var name = split[0];
                var str = split[1];
                string[] elements = str.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var builder = new StringBuilder();
                builder.Append("<" + name + " Type='Array'>");
                foreach (var element in elements)
                {
                    builder.Append("<Element>" + element + "</Element>");
                }
                builder.Append("</" + name + ">");
                return builder.ToString();
            }

            return line;
        }
    }
}
