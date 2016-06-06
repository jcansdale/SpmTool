using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace SpmTool
{
    public class SpmUtilities
    {
        public static bool IsCorrupt(string spmText)
        {
            return spmText.Contains('\u0000');
        }

        public static bool IsAirplane(string spmText)
        {
            var xml = SpmToXml.Convert(spmText);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Acro");
            return node != null;
        }

        public static bool IsHelicopter(string spmText)
        {
            var xml = SpmToXml.Convert(spmText);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Heli");
            return node != null;
        }

        public static string GetModelName(string spmText)
        {
            var xml = SpmToXml.Convert(spmText);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Spektrum/Name[@Type='String']");
            if (node == null) throw new Exception("No 'Name' specified in SPM");
            return node.ToString();
        }

        public static string GetGenerator(string spmText)
        {
            var xml = SpmToXml.Convert(spmText);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Spektrum/Generator[@Type='String']");
            if (node == null) throw new Exception("No 'Generator' specified in SPM");
            return node.ToString();
        }

        public static int GetModelNumberFromFilename(string path)
        {
            string name = Path.GetFileName(path);
            string prefix = name.Substring(0, 2);
            int modelNumber;
            if (int.TryParse(prefix, out modelNumber))
            {
                return modelNumber;
            }

            return -1;
        }

        public static string GetDX8Filename(string fileName)
        {
            var match = Regex.Match(fileName, "([0-9]+)~(?<Number>[0-9]+) (?<Name>.*)");
            if (match.Success)
            {
                string number = match.Groups["Number"].Value;
                if (number.Length < 2) number = "0" + number;
                string name = match.Groups["Name"].Value;
                return number + name;
            }

            match = Regex.Match(fileName, "0(?<Number>[0-9]+)~(?<Name>.*)");
            if (match.Success)
            {
                string number = match.Groups["Number"].Value;
                string name = match.Groups["Name"].Value;
                return number + name;
            }

            return fileName;
        }
    }
}
