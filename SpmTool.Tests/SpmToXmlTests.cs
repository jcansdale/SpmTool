using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace SpmTool.Tests
{
    public class SpmToXmlTests
    {
        [Test]
        public void Convert_String()
        {
            var text =
@"<Spektrum>Generator=""DX8""</Spektrum>";

            var xml = SpmToXml.Convert(text);

            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Spektrum/Generator");
            string type = node.GetAttribute("Type", "");
            Assert.That(type, Is.EqualTo("String"));
            Assert.That(node.Value, Is.EqualTo("DX8"));
        }

        [Test]
        public void Convert_Index()
        {
            var text =
@"<Special>
*Index= 0
</Special>";

            var xml = SpmToXml.Convert(text);

            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Special/Index");
            string type = node.GetAttribute("Type", "");
            Assert.That(type, Is.EqualTo("Index"));
            Assert.That(node.Value, Is.EqualTo("0"));
        }

        [Test]
        public void Convert_Object()
        {
            var text =
@"<PitchCurve>
[Curvedata]
[/Curvedata]
</PitchCurve>";

            var xml = SpmToXml.Convert(text);

            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/PitchCurve/Curvedata");
            string type = node.GetAttribute("Type", "");
            Assert.That(type, Is.EqualTo("Object"));
        }

        [Test]
        public void Convert_Array()
        {
            var text =
@"<PitchCurve>
assignedCurve: 0 1 2 3 4
</PitchCurve>";

            var xml = SpmToXml.Convert(text);

            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/PitchCurve/assignedCurve");
            string type = node.GetAttribute("Type", "");
            Assert.That(type, Is.EqualTo("Array"));
            var element = navigator.SelectSingleNode("/SPM/PitchCurve/assignedCurve/Element");
            Assert.That(element.Value, Is.EqualTo("0"));
        }

        [Test]
        public void Convert_KeyValue()
        {
            var text =
@"<Spektrum>
PosIndex=666
</Spektrum>";

            var xml = SpmToXml.Convert(text);

            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Spektrum/PosIndex");
            string type = node.GetAttribute("Type", "");
            Assert.That(type, Is.Empty);
            Assert.That(node.Value, Is.EqualTo("666"));
        }
    }
}
