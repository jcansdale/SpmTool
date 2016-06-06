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
    public class XmlToSpmTests
    {
        [Test]
        public void ConvertToSpm_String()
        {
            var expectedSpm = @"MyString=""MyString""";
            var xml =
@"<SPM>
    <Spektrum>
        <MyString Type='String'>MyString</MyString>
    </Spektrum>
</SPM>";

            string spm = XmlToSpm.Convert(xml);

            StringAssert.Contains(expectedSpm, spm);
        }

        [Test]
        public void ConvertToSpm_Index()
        {
            var expectedSpm = @"*MyIndex=MyIndex";
            var xml =
@"<SPM>
    <Spektrum>
        <MyIndex Type='Index'>MyIndex</MyIndex>
    </Spektrum>
</SPM>";

            string spm = XmlToSpm.Convert(xml);

            StringAssert.Contains(expectedSpm, spm);
        }

        [Test]
        public void ConvertToSpm_Object()
        {
            var expectedSpm =
@"[Curvedata]
[/Curvedata]";
            var xml =
@"<SPM>
    <PitchCurve>
        <Curvedata Type='Object'></Curvedata>
    </PitchCurve>
</SPM>";

            string spm = XmlToSpm.Convert(xml);

            StringAssert.Contains(expectedSpm, spm);
        }

        [Test]
        public void ConvertToSpm_Array()
        {
            var expectedSpm = "assignedCurve: 0 1 2 3 4";
            var xml =
@"<SPM>
    <PitchCurve>
        <assignedCurve Type='Array'>
            <Element>0</Element>
            <Element>1</Element>
            <Element>2</Element>
            <Element>3</Element>
            <Element>4</Element>
        </assignedCurve>
    </PitchCurve>
</SPM>";

            string spm = XmlToSpm.Convert(xml);

            StringAssert.Contains(expectedSpm, spm);
        }

        [Test]
        public void ConvertToSpm_KeyValue()
        {
            var expectedSpm = "PosIndex= 666";
            var xml =
@"<SPM>
    <Spektrum>
        <PosIndex>666</PosIndex>
    </Spektrum>
</SPM>";

            string spm = XmlToSpm.Convert(xml);

            StringAssert.Contains(expectedSpm, spm);
        }

        [Test]
        public void ConvertToSpm_SourceID()
        {
            var expectedSpm = "sourceID= 192";
            var xml =
@"<SPM>
    <Servo>
        <sourceID>192</sourceID>
    </Servo>
</SPM>";

            string spm = XmlToSpm.Convert(xml);

            StringAssert.Contains(expectedSpm, spm);
        }
    }
}
