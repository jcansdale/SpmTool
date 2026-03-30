using NUnit.Framework;
using System.IO;
using System.Xml.XPath;

namespace SpmTool.Tests
{
    public class Dx8ToDx9MappingCharacterizationTests
    {
        [TestCase("40", "82")]
        [TestCase("41", "83")]
        [TestCase("50", "92")]
        public void DX8ToDX9_Servo_SourceId_Mapping(string dx8Value, string expectedDx9Value)
        {
            var dx8Spm = $@"<Servo>
sourceID= {dx8Value}
</Servo>";

            AssertMappedValue(dx8Spm, "/SPM/Servo/sourceID", expectedDx9Value);
        }

        [TestCase("16", "64")]
        [TestCase("17", "65")]
        [TestCase("32", "78")]
        public void DX8ToDX9_PMix_AnalogId_Mapping(string dx8Value, string expectedDx9Value)
        {
            var dx8Spm = $@"<P-Mix>
analogID= {dx8Value}
</P-Mix>";

            AssertMappedValue(dx8Spm, "/SPM/P-Mix/analogID", expectedDx9Value);
        }

        [TestCase("43", "85")]
        [TestCase("63", "107")]
        [TestCase("127", "145")]
        public void DX8ToDX9_PMix_ConditionId_Mapping(string dx8Value, string expectedDx9Value)
        {
            var dx8Spm = $@"<P-Mix>
conditionID= {dx8Value}
activePositions=%0000
</P-Mix>";

            AssertMappedValue(dx8Spm, "/SPM/P-Mix/conditionID", expectedDx9Value);
        }

        [TestCase("64", "108")]
        [TestCase("65", "109")]
        [TestCase("68", "112")]
        public void DX8ToDX9_PMix_TrimId_Mapping(string dx8Value, string expectedDx9Value)
        {
            var dx8Spm = $@"<P-Mix>
trimID= {dx8Value}
</P-Mix>";

            AssertMappedValue(dx8Spm, "/SPM/P-Mix/trimID", expectedDx9Value);
        }

        [TestCase("196", "36")]
        [TestCase("197", "37")]
        [TestCase("200", "52")]
        public void DX8ToDX9_PMix_OutChan_Mapping(string dx8Value, string expectedDx9Value)
        {
            var dx8Spm = $@"<P-Mix>
outChan= {dx8Value}
</P-Mix>";

            AssertMappedValue(dx8Spm, "/SPM/P-Mix/outChan", expectedDx9Value);
        }

        static void AssertMappedValue(string dx8Spm, string xpath, string expectedDx9Value)
        {
            var dx9Spm = SpmConvert.DX8To(dx8Spm);
            var reader = new StringReader(SpmToXml.Convert(dx9Spm));
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(xpath);

            Assert.That(node, Is.Not.Null, $"Expected node at '{xpath}'");
            Assert.That(node.Value, Is.EqualTo(expectedDx9Value));
        }
    }
}
