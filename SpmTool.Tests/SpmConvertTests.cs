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
    public class SpmConvertTests
    {
        // TODO: <Digital> and <Analog>. Show up in early SPM version files?

        // All those tweaks will come straight across. Only place I know of differences are the values in Flap System and Throttle Cut.
        // http://www.rcgroups.com/forums/showpost.php?p=27112656&postcount=23859

        [Test]
        public void DX8To()
        {
            string expectedGenerator = "DX18";
            var dx8Spm = @"<Spektrum>Generator=""DX8""VCode="" 3.00""</Spektrum>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm, expectedGenerator);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var generatorNode = navigator.SelectSingleNode("/SPM/Spektrum/Generator[@Type='String']");
            Assert.That(generatorNode.Value, Is.EqualTo(expectedGenerator));
            var vcodeNode = navigator.SelectSingleNode("/SPM/Spektrum/VCode[@Type='String']");
            Assert.That(vcodeNode, Is.Null);    // Don't specify a VCode.
        }

        [Test]
        public void DX8ToDX9()
        {
            var dx8Spm = @"<Spektrum>Generator=""DX8""VCode="" 3.00""</Spektrum>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var generatorNode = navigator.SelectSingleNode("/SPM/Spektrum/Generator[@Type='String']");
            Assert.That(generatorNode.Value, Is.EqualTo(@"DX9"));
            var vcodeNode = navigator.SelectSingleNode("/SPM/Spektrum/VCode[@Type='String']");
            Assert.That(vcodeNode, Is.Null);    // Don't specify a VCode.
        }

        [Test]
        public void DX8ToDX9_MasterVolume()
        {
            string expectedMasterVolume = "20";
            var dx8Spm = @"<Spektrum>Generator=""DX8""VCode="" 3.00""</Spektrum>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm, masterVolume: expectedMasterVolume);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var masterVolumeNode = navigator.SelectSingleNode("/SPM/Voice/masterVolume");
            Assert.That(masterVolumeNode, Is.Not.Null);
            Assert.That(masterVolumeNode.Value, Is.EqualTo(expectedMasterVolume));
        }

        [Test]
        public void DX8ToDX9_ModelNumber()
        {
            string expectedModelName = "__TEST__";
            var dx8Spm =@"<Spektrum>Name=""Acro""</Spektrum>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm, modelName: expectedModelName);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Spektrum/Name[@Type='String']");
            Assert.That(node.Value, Is.EqualTo(expectedModelName));
        }

        [TestCase("Common", "%00000000")]
        [TestCase("FMode", "%0000003F")]
        public void DX8ToDX9_Config(string trimType, string expectTrimType)
        {
            var dx8Spm =
string.Format(@"<Config>FrameRate=AutoTrimType={0}trimMode=Normal</Config>", trimType);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var trimModeNode = navigator.SelectSingleNode("/SPM/Config/trimMode");
            Assert.That(trimModeNode.Value, Is.EqualTo("Normal"));
            var trimTypeNode = navigator.SelectSingleNode("/SPM/Config/TrimType");
            Assert.That(trimTypeNode.Value, Is.EqualTo(expectTrimType));
        }

        // Sailplane support

        [TestCase("/SPM/Spektrum/PosMaxSail", "5")]
        public void DX8ToDX9_Spektrum_Sail(string name, string value)
        {
            var dx8Spm =
@"<Spektrum>Generator=""DX8""VCode="" 3.00""; Originator=""HH101XBRcz9pUHB8kBAOaZhH0regbM""PosIndex= 5PosMaxSail= 5Type=SailcurveIndex= 7Name=""Sail""</Spektrum>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var posMaxSail = navigator.SelectSingleNode(name);
            Assert.That(posMaxSail, Is.Not.Null);
            Assert.That(posMaxSail.Value, Is.EqualTo(value));
        }

        [TestCase("/SPM/Sail/Wing", "Standard")]
        [TestCase("/SPM/Sail/Tail", "Normal")]
        [TestCase("/SPM/Sail/Motor", "None")]
        public void DX8ToDX9_Sail(string name, string value)
        {
            var dx8Spm =
@"<Sail>
Wing=Standard
Tail=Normal
Motor=None
</Sail>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo(value));
        }

        [TestCase("None", "None", null)]
        [TestCase("SpoilStk", "Unsupported", "64")] // Thr. Stick
        [TestCase("Gear", "Unsupported", "82")]     // Gear->Switch A
        [TestCase("FModeSw", "Unsupported", "83")]  // F Mode->Switch B
        [TestCase("EleDR", "Unsupported", "84")]    // Elev D/R->Switch C
        [TestCase("Flap", "Unsupported", "85")]     // Flap->Switch D
        [TestCase("Aux2", "Unsupported", "86")]     // AUX 2->Switch E
        [TestCase("AilDR", "Unsupported", "87")]    // Ail D/R -> Switch F
        [TestCase("RudDR", "Unsupported", "88")]    // Rud D/R->Switch G
        [TestCase("Mix", "Unsupported", "89")]      // Mix/Hold->Switch H
        [TestCase("Trainer", "Unsupported", "92")]  // Trainer->Switch I ?
        public void DX8ToDX9_Sail_Motor(string motor, string expectedMotor, string expectedSubTypeC)
        {
            var dx8Spm = string.Format(
@"<Sail>
Motor={0}
</Sail>", motor);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var motorNode = navigator.SelectSingleNode("/SPM/Sail/Motor");
            Assert.That(motorNode, Is.Not.Null);
            Assert.That(motorNode.Value, Is.EqualTo(expectedMotor));
            var subTypeCNode = navigator.SelectSingleNode("/SPM/Sail/subTypeC");
            if (expectedSubTypeC != null)
            {
                Assert.That(subTypeCNode, Is.Not.Null);
                Assert.That(subTypeCNode.Value, Is.EqualTo(expectedSubTypeC));
            }
            else
            {
                Assert.That(subTypeCNode, Is.Null);
            }
        }

        [TestCase("Unsupported", "68", "Unsupported", "112")]  // 68-> 112 - LTrimD
        [TestCase("Unsupported", "69", "Unsupported", "113")]  // 69-> 113 - RTrimD
        public void DX8ToDX9_Sail_Motor_subTypeC(string motor, string subTypeC, string expectedMotor, string expectedSubTypeC)
        {
            var dx8Spm = string.Format(
@"<Sail>
Motor={0}
subTypeC={1}
</Sail>", motor, subTypeC);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var motorNode = navigator.SelectSingleNode("/SPM/Sail/Motor");
            Assert.That(motorNode, Is.Not.Null);
            Assert.That(motorNode.Value, Is.EqualTo(expectedMotor));
            var subTypeCNode = navigator.SelectSingleNode("/SPM/Sail/subTypeC");
            if (expectedSubTypeC != null)
            {
                Assert.That(subTypeCNode, Is.Not.Null);
                Assert.That(subTypeCNode.Value, Is.EqualTo(expectedSubTypeC));
            }
            else
            {
                Assert.That(subTypeCNode, Is.Null);
            }
        }

        [TestCase("None", "64", "0")] // Default throttle stick
        [TestCase("Gear", "82", "82")] // Gear->Switch A
        [TestCase("SpoilStk", "64", "145")] // Throttle stick, Flight Mode
        public void DX8ToDX9_ThroCurve_Sail(string motor, string expectedAnalogID, string expectedConditionID)
        {
            var dx8Spm = string.Format(
@"<Sail>
Motor={0}
</Sail>

<ThroCurve>analogID= 16
</ThroCurve>", motor);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var analogIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/analogID");
            Assert.That(analogIDNode, Is.Not.Null);
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID));

            var conditionIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/conditionID");
            Assert.That(conditionIDNode, Is.Not.Null);
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID));
        }

        [TestCase("Gear", "%0000", "0", "0", "0")]
        [TestCase("Gear", "%0001", "1", "0", "0")]
        [TestCase("Gear", "%0002", "0", "1", "0")]
        [TestCase("Gear", "%0003", "1", "1", "0")]
        [TestCase("Gear", "%0004", "0", "0", "1")]
        [TestCase("Gear", "%0005", "1", "0", "1")]
        [TestCase("Gear", "%0006", "0", "1", "1")]
        [TestCase("Gear", "%0007", "1", "1", "1")]
        public void DX8ToDX9_ThroCurve_assignedCurve_Sail(string motor, string activePositions, params string[] ac)
        {
            var dx8Spm = string.Format(
@"<Sail>Wing=StandardTail=NormalMotor={0}</Sail>

<RAE-Mix>
activePositions={1}
</RAE-Mix>

<ThroCurve>analogID= 16
conditionID= 0
assignedCurve: 0 1 2 3 4

[Curvedata]*Index= 1X: -1023 -511 0 511 1023 0 0Y: 0 0 0 0 0 0 0[/Curvedata]</ThroCurve>", motor, activePositions);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            for (int count = 0; count < ac.Length; count++)
            {
                var acNode = navigator.SelectSingleNode(string.Format("/SPM/ThroCurve/assignedCurve/Element[{0}]", count + 1));
                Assert.That(acNode, Is.Not.Null, "Element " + (count + 1));
                Assert.That(acNode.Value, Is.EqualTo(ac[count]), "Element " + (count + 1));
            }
        }

        [TestCase("SpoilStk", "%0000", "0", "0", "0", "0")]
        [TestCase("SpoilStk", "%0001", "1", "0", "0", "0")]
        [TestCase("SpoilStk", "%0002", "0", "1", "0", "0")]
        [TestCase("SpoilStk", "%0003", "1", "1", "0", "0")]
        [TestCase("SpoilStk", "%0004", "0", "0", "1", "0")]
        [TestCase("SpoilStk", "%0005", "1", "0", "1", "0")]
        [TestCase("SpoilStk", "%0006", "0", "1", "1", "0")]
        [TestCase("SpoilStk", "%0007", "1", "1", "1", "0")]
        [TestCase("SpoilStk", "%0008", "0", "0", "0", "1")]
        [TestCase("SpoilStk", "%0009", "1", "0", "0", "1")]
        [TestCase("SpoilStk", "%000A", "0", "1", "0", "1")]
        [TestCase("SpoilStk", "%000B", "1", "1", "0", "1")]
        [TestCase("SpoilStk", "%000C", "0", "0", "1", "1")]
        [TestCase("SpoilStk", "%000D", "1", "0", "1", "1")]
        [TestCase("SpoilStk", "%000E", "0", "1", "1", "1")]
        [TestCase("SpoilStk", "%000F", "1", "1", "1", "1")]
        public void DX8ToDX9_ThroCurve_assignedCurve_Sail_SpoilStk(string motor, string activePositions, params string[] ac)
        {
            var dx8Spm = string.Format(
@"<Sail>Wing=StandardTail=NormalMotor={0}</Sail>

<RAE-Mix>
activePositions={1}
</RAE-Mix>

<ThroCurve>analogID= 16
conditionID= 0
assignedCurve: 0 1 2 3 4

[Curvedata]*Index= 1X: -1023 -511 0 511 1023 0 0Y: 0 0 0 0 0 0 0[/Curvedata]</ThroCurve>", motor, activePositions);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            for (int count = 0; count < ac.Length; count++)
            {
                var acNode = navigator.SelectSingleNode(string.Format("/SPM/ThroCurve/assignedCurve/Element[{0}]", count + 1));
                Assert.That(acNode, Is.Not.Null, "Element " + (count + 1));
                Assert.That(acNode.Value, Is.EqualTo(ac[count]), "Element " + (count + 1));
            }
        }

        [TestCase("/SPM/CamberPreset/conditionID", "145")]
        //[TestCase("/SPM/CamberPreset/activePositions", "%0000")]
        //[TestCase("/SPM/CamberPreset/mixName", "Camber Presets")]
        [TestCase("/SPM/CamberPreset/efItem/Index", "1")]
        //[TestCase("/SPM/CamberPreset/efItem/offset", "0")]
        [TestCase("/SPM/CamberPreset/efItem/flapLeft", "8000")]
        [TestCase("/SPM/CamberPreset/efItem/flapRight", "-8000")]
        [TestCase("/SPM/CamberPreset/efItem/flonLeft", "-16000")]
        [TestCase("/SPM/CamberPreset/efItem/flonRight", "16000")]
        //[TestCase("/SPM/CamberPreset/efItem/tipLeft", "0")]
        //[TestCase("/SPM/CamberPreset/efItem/tipRight", "0")]
        [TestCase("/SPM/CamberPreset/efItem/elevator", "12000")]
        [TestCase("/SPM/CamberPreset/efItem/speed", "32676")]
        //[TestCase("/SPM/CamberPreset/efItem/analogID", "0")]
        public void DX8ToDX9_CamberPreset_Sail(string name, string value)
        {
            var dx8Spm =
@"<Sail>Wing=Ail_2_Flap_2</Sail><CamberPreset>conditionID= 127[cpItem]*Index= 1flap= 800flon= 1600elevator= 1200speed= 32676[/cpItem]
</CamberPreset>";

            /*
            <CamberPreset>            conditionID= 145            activePositions=%0000            mixName="Camber Presets"            [efItem]            *Index= 1            offset= 0            flapLeft= 8000            flapRight= -8000            flonLeft= -16000            flonRight= 16000            tipLeft= 0            tipRight= 0            elevator= 12000            speed= 32676            analogID= 0            [/efItem]            </CamberPreset>
            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("Ail_2_Flap_2", "800", "-8000")]
        [TestCase("Ail_2_Flap_1", "800", "8000")]
        public void DX8ToDX9_CamberPreset_flapRight_Sail(string wing, string flap, string expectedFlapRight)
        {
            var dx8Spm = string.Format(
@"<Sail>Wing={0}</Sail><CamberPreset>[cpItem]flap= {1}[/cpItem]
</CamberPreset>", wing, flap);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/CamberPreset/efItem/flapRight");
            Assert.That(node.Value, Is.EqualTo(expectedFlapRight));
        }

        [TestCase("/SPM/CamberMix/conditionID", "145")]
        //[TestCase("/SPM/CamberMix/activePositions", "%0000")]
        //[TestCase("/SPM/CamberMix/mixName", "Camber System")]
        [TestCase("/SPM/CamberMix/efItem/Index", "2")]
        [TestCase("/SPM/CamberMix/efItem/offset", "1023")]
        [TestCase("/SPM/CamberMix/efItem/flapLeft", "250")]
        [TestCase("/SPM/CamberMix/efItem/flapRight", "500")]
        [TestCase("/SPM/CamberMix/efItem/flonLeft", "-750")]
        [TestCase("/SPM/CamberMix/efItem/flonRight", "-1000")]
        //[TestCase("/SPM/CamberMix/efItem/tipLeft", "0")]
        //[TestCase("/SPM/CamberMix/efItem/tipRight", "0")]
        //[TestCase("/SPM/CamberMix/efItem/elevator", "0")]
        //[TestCase("/SPM/CamberMix/efItem/speed", "0")]
        [TestCase("/SPM/CamberMix/efItem/analogID", "76")] // Spoiler
        public void DX8ToDX9_CamberMix_Sail(string name, string value)
        {
            var dx8Spm =
@"<CamberMix>conditionID= 0[csItem]*Index= 2offset= -1023flapUp= 255flapDown= 511flonUp= 767flonDown= 1023analogID= 16[/csItem]
</CamberMix>";

            /*
            <CamberMix>            conditionID= 145            activePositions=%0000            mixName="Camber System"            [efItem]            *Index= 2            offset= 1023            flapLeft= 250            flapRight= 500            flonLeft= -750            flonRight= -1000            tipLeft= 0            tipRight= 0            elevator= 0            speed= 32736            analogID= 76            [/efItem]
            </CamberMix>            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("Ail_2_Flap_1", "/SPM/CamberMix/efItem/flapLeft", "2500")]
        [TestCase("Ail_2_Flap_1", "/SPM/CamberMix/efItem/flapRight", "5000")]
        [TestCase("Ail_2_Flap_2", "/SPM/CamberMix/efItem/flapLeft", "250")]
        [TestCase("Ail_2_Flap_2", "/SPM/CamberMix/efItem/flapRight", "500")]
        public void DX8ToDX9_CamberMix_flaps_Sail(string wing, string name, string value)
        {
            var dx8Spm = string.Format(
@"<Sail>Wing={0}</Sail><CamberMix>[csItem]flapUp= 255flapDown= 511[/csItem]
</CamberMix>", wing);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Value, Is.EqualTo(value));
        }

        [TestCase("0", "0")]   // Inhibit
        [TestCase("16", "76")] // Spoiler Stick
        [TestCase("21", "21 NOT SUPPORTED")] // RT Knob
        public void DX8ToDX9_CamberMix_analogID_Sail(string analogID, string expectedAnalogID)
        {
            var dx8Spm = string.Format(
@"<CamberMix>[csItem]analogID= {0}[/csItem]
</CamberMix>", analogID);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/CamberMix/efItem/analogID");
            Assert.That(node.Value, Is.EqualTo(expectedAnalogID));
        }

        [TestCase("0", "0")]
        [TestCase("10", "10")]
        [TestCase("255", "250")]
        [TestCase("511", "500")]
        [TestCase("1023", "1000")]
        public void DX8ToDX9_CamberMix_Sail_Percentage(string binaryPercentage, string normalPercentage)
        {
            var dx8Spm =
string.Format(@"<CamberMix>[csItem]flapUp= {0}[/csItem]
</CamberMix>", binaryPercentage);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var percentage = navigator.SelectSingleNode("/SPM/CamberMix/efItem/flapLeft").Value;
            Assert.That(percentage, Is.EqualTo(normalPercentage));
        }

        [TestCase("/SPM/EF-Mix/conditionID", "145")]
        // [TestCase("/SPM/EF-Mix/activePositions", "%0002")] // Is this required?
        [TestCase("/SPM/EF-Mix/efItem/Index", "0")]
        [TestCase("/SPM/EF-Mix/efItem/offset", "-51")]
        [TestCase("/SPM/EF-Mix/efItem/flapLeft", "-40")]  // NOTE: Double check this is correct (with reversed RFL)
        [TestCase("/SPM/EF-Mix/efItem/flapRight", "-30")]
        // NOTE: flonLeft/flonRight don't appear in GUI.
        [TestCase("/SPM/EF-Mix/efItem/flonLeft", "-20")]
        [TestCase("/SPM/EF-Mix/efItem/flonRight", "-10")]
        public void DX8ToDX9_EF_Mix_Sail(string name, string value)
        {
            var dx8Spm =
@"<EF-Mix>
conditionID= 127

[efItem]
*Index= 0
offset= -51
flapUp= -40
flapDown= -30
flonUp= -20
flonDown= -10
[/efItem]
</EF-Mix>";

            /*
            <EF-Mix>
            conditionID= 145
            activePositions=%0002
            mixName="ELE > FLP"

            [efItem]
            *Index= 0
            offset= -51
            flapLeft= -40
            flapRight= -30
            flonLeft= -20
            flonRight= -10
            tipLeft= 0
            tipRight= 0
            elevator= 0
            speed= 32736
            analogID= 0
            [/efItem]
            </EF-Mix>
            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/AR-Mix-S/conditionID", "145")]
        // [TestCase("/SPM/AR-Mix-S/mixName", "AIL > RUD")]
        [TestCase("/SPM/AR-Mix-S/arafItem/Index", "0")]
        [TestCase("/SPM/AR-Mix-S/arafItem/left1", "255")]
        [TestCase("/SPM/AR-Mix-S/arafItem/right1", "511")]
        public void DX8ToDX9_AR_Mix_Sail(string name, string value)
        {
            var dx8Spm =
@"<AR-Mix-S>
conditionID= 127

[arafItem]
*Index= 0
left= 255
right= 511
[/arafItem]
</AR-Mix-S>";

/*
<AR-Mix-S>conditionID= 145mixName="AIL > RUD"[arafItem]*Index= 0left= 0right= 0left1= 255right1= 511left2= 0right2= 0[/arafItem]
*/
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/AF-Mix-S/conditionID", "107")]
        //[TestCase("/SPM/AF-Mix-S/mixName", "AIL > FLP")]
        [TestCase("/SPM/AF-Mix-S/arafItem/Index", "1")]
        [TestCase("/SPM/AF-Mix-S/arafItem/left", "-613")]
        [TestCase("/SPM/AF-Mix-S/arafItem/right", "-409")]
        public void DX8ToDX9_AF_Mix_Sail(string name, string value)
        {
            var dx8Spm =
@"<AF-Mix-S>conditionID= 63[arafItem]*Index= 1left= -613right= -409[/arafItem]</AF-Mix-S>";

/*
<AF-Mix-S>conditionID= 107mixName="AIL > FLP"[arafItem]*Index= 1left= -613right= -409left1= 0right1= 0left2= 0right2= 0[/arafItem]</AF-Mix-S>*/
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/FlpEleMix/analogID", "198")]
        [TestCase("/SPM/FlpEleMix/conditionID", "145")]
        [TestCase("/SPM/FlpEleMix/trimID", "0")]
        [TestCase("/SPM/FlpEleMix/activeMask", "%0000")]
        [TestCase("/SPM/FlpEleMix/delay", "0")]
        // [TestCase("/SPM/FlpEleMix/mixName", "FLP > ELE")]
        [TestCase("/SPM/FlpEleMix/assignedCurve/Element[1]", "0")]
        [TestCase("/SPM/FlpEleMix/assignedCurve/Element[5]", "4")]
        [TestCase("/SPM/FlpEleMix/Curvedata/Index", "1")]
        [TestCase("/SPM/FlpEleMix/Curvedata/points", "5")]
        [TestCase("/SPM/FlpEleMix/Curvedata/Expo", "Disabled")]
        [TestCase("/SPM/FlpEleMix/Curvedata/curved", "Enabled")]
        [TestCase("/SPM/FlpEleMix/Curvedata/X/Element[1]", "-1023")]
        [TestCase("/SPM/FlpEleMix/Curvedata/X/Element[5]", "1023")]
        [TestCase("/SPM/FlpEleMix/Curvedata/Y/Element[1]", "327")]
        [TestCase("/SPM/FlpEleMix/Curvedata/Y/Element[5]", "0")]
        public void DX8ToDX9_FlpEleMix_Sail(string name, string value)
        {
            var dx8Spm =
@"<FlpEleMix>analogID= 16conditionID= 127trimID= 0activeMask=%0000delay= 0assignedCurve: 0 1 2 3 4[Curvedata]*Index= 1points= 5Expo=DisabledtrimActive=Disabledcurved=EnabledX: -1023 -511 0 511 1023 0 0Y: 327 327 306 204 0 0 0[/Curvedata]</FlpEleMix>";

/*
<FlpEleMix>analogID= 198conditionID= 0trimID= 0activeMask=%0000delay= 0mixName="FLP > ELE"assignedCurve: 0 1 2 3 4 4 4 4 4 4[Curvedata]*Index= 1points= 5Expo=DisabledtrimActive=Disabledcurved=EnabledX: -1023 -511 0 511 1023 0 0Y: 327 327 306 204 0 0 0[/Curvedata]
</FlpEleMix>*/
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Differential", null)]
        [TestCase("/SPM/Diff-Ail/conditionID", "145")]
        [TestCase("/SPM/Diff-Ail/rate/Element[1]", "818")]
        [TestCase("/SPM/Diff-Ail/rate/Element[2]", "409")]
        [TestCase("/SPM/Diff-Ail/rate/Element[3]", "460")]
        [TestCase("/SPM/Diff-Ail/rate/Element[4]", "255")]
        [TestCase("/SPM/Diff-Flap/conditionID", "145")]
        [TestCase("/SPM/Diff-Flap/rate/Element[1]", "0")] // NOTE: Can't seem to change this in the GUI.
        public void DX8ToDX9_Differential_Sail(string name, string value)
        {
            var dx8Spm =
@"<Spektrum>Type=Sail</Spektrum>

<Sail>
</Sail>

<Differential>conditionID= 127ailRate: 818 409 460 255 0flapRate: 0 0 0 0 0</Differential>";

/*
<Diff-Ail>conditionID= 145rate: 818 409 460 255 0 0 0 0 0 0</Diff-Ail><Diff-Flap>conditionID= 145rate: 0 0 0 0 0 0 0 0 0 0</Diff-Flap><Diff-Tip>conditionID= 145rate: 0 0 0 0 0 0 0 0 0 0</Diff-Tip><Diff-Rud>conditionID= 0rate: 0 0 0 0 0 0 0 0 0 0</Diff-Rud>
*/

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("DX8", "Normal", "0", "MOT", "0")]
        [TestCase("DX8", "Ail_2_Flap_1", "0", "MOT", "6")]
        [TestCase("DX8", "Ail_2_Flap_1", "4", "FLP", "4")]
        [TestCase("DX8", "Ail_2_Flap_1", "5", "LAL", "0")]
        [TestCase("DX8", "Ail_2_Flap_2", "0", "MOT", "6")]
        [TestCase("DX8", "Ail_2_Flap_2", "4", "LFL", "5")]
        [TestCase("DX8", "Ail_2_Flap_2", "5", "LAL", "0")]
        [TestCase("DX8", "Ail_2_Flap_2", "6", "RFL", "4")]
        public void DX8ToDX9_Servo_Sail(string generator, string wing, string index, string name, string expectedVSource)
        {
            var dx8Spm =
string.Format(@"<Spektrum>Generator=""{0}""Type=Sail</Spektrum><Sail>Wing={1}</Sail><Servo>*Index= {2}name={3}</Servo>", generator, wing, index, name);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Servo/vSource");
            Assert.That(node, Is.Not.Null, "Check that 'vSource' has been added");
            Assert.That(node.Value, Is.EqualTo(expectedVSource), "Check 'vSource' value");
        }

        [TestCase("Sail", "6", "Normal", "RFL", "Reverse")]
        [TestCase("Sail", "6", "Reverse", "RFL", "Normal")]
        [TestCase("Sail", "4", "Normal", "LFL", "Normal")]
        [TestCase("Acro", "6", "Normal", "RFL", "Normal")]
        public void DX8ToDX9_Servo_direction_Sail(string type, string index, string direction, string name, string expectedDirection)
        {
            var dx8Spm =
string.Format(@"<Spektrum>Type={0}</Spektrum><{0}></{0}><Servo>*Index= {1}direction={2}name={3}</Servo>", type, index, direction, name);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Servo/direction");
            Assert.That(node.Value, Is.EqualTo(expectedDirection), "Check 'direction' value");
        }

        [TestCase("Sail", "0", "Launch")]
        [TestCase("Sail", "1", "Cruise")]
        [TestCase("Sail", "2", "Thermal")]
        [TestCase("Sail", "3", "Speed")]
        [TestCase("Sail", "4", "Land")] // Moved from "2" on DX9
        [TestCase("Acro", "0", null)]
        public void DX8ToDX9_FMode_Names_Sail(string type, string index, string expectedDisplay)
        {
            var dx8Spm =
string.Format(
@"<Spektrum>Type={0}</Spektrum><{0}></{0}><FMode></FMode>", type, index);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var node = navigator.SelectSingleNode(string.Format("/SPM/FMode_Names/fmName[Index='{0}']/display", index));
            if (expectedDisplay != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(expectedDisplay), "Check 'display' value");
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Warning/Vibrate", "Enabled")]
        [TestCase("/SPM/Warning/Spoiler", "Under")]
        [TestCase("/SPM/Warning/Thresh", "818")]
        [TestCase("/SPM/Warning/FltMode", "%0001")]
        [TestCase("/SPM/Warning/Motor", "%0000")]
        public void DX8ToDX9_Warning_Sail(string name, string value)
        {
            var dx8Spm =
@"<Spektrum>
Type=Sail
</Spektrum>

<Sail>
</Sail>

<Warning>
Vibrate=Enabled
Spoiler=Under
Thresh= 818
FltMode=%0001
Motor=%0000
</Warning>";

/*
<Warning>
Vibrate=Enabled
Spoiler=Under
Thresh= 818
FltMode=%0000
Gear=%0000
Motor=%0000
chanAchan= 0
chanAtype=Disabled
chanBchan= 0
chanBtype=Disabled
</Warning>
*/
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("None", "%0000", "%0000", "%0000")]
        [TestCase("Aux2", "%0001", "%0001", "%0001")]
        [TestCase("Aux2", "%0002", "%0000", "%0000")]
        [TestCase("Aux2", "%0002", "%0001", "%0002")]
        [TestCase("SpoilStk", "%0001", "%0001", "%0000")] // Doesn't warn when DX8 uses SpoilStk
        public void DX8ToDX9_Warning_Motor_Sail(string motor, string activePositions, string warningMotor, string expectedMotor)
        {
            var dx8Spm =
string.Format(
@"<Spektrum>Type=Sail</Spektrum><Sail>Motor={0}</Sail><RAE-Mix>
activePositions={1}
</RAE-Mix><Warning>
Motor={2}
</Warning>", motor, activePositions, warningMotor);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var node = navigator.SelectSingleNode("/SPM/Warning/Motor");
            if (expectedMotor != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(expectedMotor), "Check 'motor' value");
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        // Helicopter support

        [Test]
        public void DX8ToDX9_Heli()
        {
            var dx8Spm =
@"<Heli>
Swash=Swash_1_Normal
</Heli>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var swashNode = navigator.SelectSingleNode("/SPM/Heli/Swash");
            Assert.That(swashNode.Value, Is.EqualTo("Swash_1_Normal"));
        }

        // NOTE: Doesn't seem to store Tone! (DX8/DX9)

        //DX8 FltMode Stunt1:%20 Stunt2:%40 Hold(On):%80
        //DX9 FltMode Stunt1:%04 Stunt2:%08 Stunt3:%10 Hold Hold(Off):%0001 Hold(On):%0002
        [TestCase("%0000", "%0000", "%0000")] // Stunt1:Inh Stunt2:Inh Hold:Inh
        [TestCase("%00E0", "%000C", "%0002")] // Stunt1:Act Stunt2:Act Hold:Act
        [TestCase("%0080", "%0000", "%0002")] // Hold:Act
        [TestCase("%0020", "%0004", "%0000")] // Stunt1:Act
        [TestCase("%0040", "%0008", "%0000")] // Stunt2:Act
        public void DX8ToDX9_Warning_FltMode_Hold(string fltMode, string expectFltMode, string expectHold)
        {
            var dx8Spm =
string.Format(@"<Heli>
</Heli><Warning>FltMode={0}</Warning>", fltMode);
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var holdNode = navigator.SelectSingleNode("/SPM/Warning/Hold");
            Assert.That(holdNode.Value, Is.EqualTo(expectHold), "Check Hold");
            var fltModeNode = navigator.SelectSingleNode("/SPM/Warning/FltMode");
            Assert.That(fltModeNode.Value, Is.EqualTo(expectFltMode), "Check FltMode");
        }

        [TestCase("%0000", "%0000")] // Flaps: Inhibit
        [TestCase("%0001", "%0002")] // Flaps: Mid
        [TestCase("%0002", "%0004")] // Flaps: Land
        [TestCase("%0003", "%0006")] // Flaps: Mid+Land
        [TestCase("%0004", "%0005")] // Flaps: Norm+Land
        public void DX8ToDX9_Warning_Flaps(string flaps, string expectFlaps)
        {
            var dx8Spm =
string.Format(@"<Warning>Flaps={0}</Warning>", flaps);
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var flapsNode = navigator.SelectSingleNode("/SPM/Warning/Flaps");
            Assert.That(flapsNode.Value, Is.EqualTo(expectFlaps), "Check Flaps");
        }

        [TestCase("%0000", "%0000")] // Gear: Inhibit
        [TestCase("%0001", "%0001")] // Gear: Pos 0
        [TestCase("%0002", "%0002")] // Gear: Pos 1
        public void DX8ToDX9_Warning_Gear(string gear, string expectGear)
        {
            var dx8Spm =
string.Format(@"<Warning>Gear={0}</Warning>", gear);
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var gearNode = navigator.SelectSingleNode("/SPM/Warning/Gear");
            Assert.That(gearNode.Value, Is.EqualTo(expectGear), "Check Gear");
        }

        [Test]
        public void DX8ToDX9_Warning_Acro()
        {
            var dx8Spm =
@"<Acro>
</Acro><Warning>FltMode=%0000</Warning>";
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var holdNode = navigator.SelectSingleNode("/SPM/Warning/Hold");
            Assert.That(holdNode, Is.Null, "Check no Hold for Acro model");
        }

        [Test]
        public void DX8ToDX9_Heli_ThroCurve()
        {
            // DX9 will add:
            // mixName="Throttle"
            // [Curvedata]
            // curved=Enabled
            string expectedAnalogID = "64"; // 16-> 64  - Thr. Stick
            string expectedConditionID = "0"; // DX8 and DX9 are fixed as FMode
            string expectedTrimID = "108"; // 64-> 108 - THR Trim (ThroCurve/trimID)
            var dx8Spm =
@"<ThroCurve>analogID= 16conditionID= 0trimID= 64activeMask=%0000delay= 0assignedCurve: 0 1 2 3 1[Curvedata]*Index= 0points= 5Expo=DisabledtrimActive=DisabledX: -1023 -511 0 511 1023 0 0Y: -1023 -1023 -1023 -1023 -1023 0 0[/Curvedata]
</ThroCurve>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var trimIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/trimID");
            Assert.That(trimIDNode.Value, Is.EqualTo(expectedTrimID), "Check that 'trimID' has been converted");
        }

        [Test]
        public void DX8ToDX9_PitchCurve()
        {
            // DX9 will add:
            // mixName="Pitch"
            // [Curvedata]
            // curved=Enabled
            string expectedAnalogID = "64"; // 16-> 64  - Thr. Stick
            string expectedConditionID = "0"; // DX8 and DX9 are fixed as FMode
            string expectedTrimID = "0";
            var dx8Spm =
@"<PitchCurve>
analogID= 16
conditionID= 0
trimID= 0
activeMask=%0000
delay= 0
assignedCurve: 0 1 2 3 4

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
X: -1023 -511 0 511 1023 0 0
Y: -1023 -511 0 511 1023 0 0
[/Curvedata]
</PitchCurve>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/PitchCurve/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDNode = navigator.SelectSingleNode("/SPM/PitchCurve/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var trimIDNode = navigator.SelectSingleNode("/SPM/PitchCurve/trimID");
            Assert.That(trimIDNode.Value, Is.EqualTo(expectedTrimID), "Check that 'trimID' has been converted");
        }

        [Test]
        public void DX8ToDX9_RevoCurve()
        {
            // DX9 will add:
            // mixName="Tail"
            // [Curvedata]
            // curved=Enabled
            string expectedAnalogID = "64"; // 16-> 64  - Thr. Stick
            string expectedConditionID = "145"; // DX8 is fixed as FMode
            string expectedTrimID = "0";
            var dx8Spm =
@"<RevoCurve>analogID= 16conditionID= 0trimID= 0activeMask=%0000delay= 100assignedCurve: 0 1 2 3 4[Curvedata]*Index= 0points= 5Expo=DisabledtrimActive=DisabledX: -1023 -511 0 511 1023 0 0Y: 0 0 0 0 0 0 0[/Curvedata]
</RevoCurve>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/RevoCurve/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDNode = navigator.SelectSingleNode("/SPM/RevoCurve/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var trimIDNode = navigator.SelectSingleNode("/SPM/RevoCurve/trimID");
            Assert.That(trimIDNode.Value, Is.EqualTo(expectedTrimID), "Check that 'trimID' has been converted");
        }

        [Test]
        public void DX8ToDX9_FMode_Heli()
        {
            // NOTE: 'switch_b' moves to 'switch_c'
            // NOTE: Hold is defined using 'activePositions' not 'fmtable'.
            string expectedSwitchA = "83"; // 41-> 83  - F Mode->Switch B
            string expectedSwitchB = "0";
            string expectedSwitchC = "82"; // 40-> 82  - Gear->Switch A
            string expectedActivePositions = "%0006";
            string expectedFmtable = "1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4";
            var dx8Spm =
@"<Heli>Swash=Swash_1_Normal</Heli><FMode>switch_a= 41switch_b= 40switch_c= 0size= 9data: 1 0 0 2 0 0 3 0 0</FMode>";

/*
<FMode>
switch_a= 83
switch_b= 0
switch_c= 82
size= 18
fmtable: 1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4
activePositions=%0006
</FMode>
*/

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var switchANode = navigator.SelectSingleNode("/SPM/FMode/switch_a");
            Assert.That(switchANode.Value, Is.EqualTo(expectedSwitchA), "Check that 'switch_a' has been converted");
            var switchBNode = navigator.SelectSingleNode("/SPM/FMode/switch_b");
            Assert.That(switchBNode.Value, Is.EqualTo(expectedSwitchB), "Check that 'switch_b' is 0");
            var switchCNode = navigator.SelectSingleNode("/SPM/FMode/switch_c");
            Assert.That(switchCNode.Value, Is.EqualTo(expectedSwitchC), "Check that 'switch_c' has been changed to DX8 hold switch");
            var dataNode = navigator.SelectSingleNode("/SPM/FMode/data");
            Assert.That(dataNode, Is.Null, "Check that 'data' has been removed");
            var activePositionsNode = navigator.SelectSingleNode("/SPM/FMode/activePositions");
            Assert.That(activePositionsNode.Value, Is.EqualTo(expectedActivePositions), "Check that 'activePositions' has been created");
            StringAssert.Contains("fmtable: " + expectedFmtable, dx9Spm);
        }

        [Test]
        public void DX8ToDX9_Gyro()
        {
            string expectedConditionID = "145"; // 127->145 - Flight Mode
            var dx8Spm =
@"<Gyro>sourceID= 0conditionID= 127trimID= 0fpct: 0 0 0 0 0tailHold=DisabledoutChan= 8</Gyro>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Gyro/conditionID");
            Assert.That(node.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Governor()
        {
            string expectedConditionID = "145"; // 127->145 - Flight Mode
            var dx8Spm =
@"<Governor>sourceID= 0conditionID= 127trimID= 0fpct: 0 0 0 0 0outChan= 7</Governor>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Governor/conditionID");
            Assert.That(node.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        [Test]
        public void DX8ToDX9_RAEMix()
        {
            var dx8Spm =
@"<RAE-Mix>
analogID= 98
conditionID= 63
percentAileron= 66
percentElevator= 77
activePositions=%0002
</RAE-Mix>";
/*
<RAE-Mix>
analogID= 130
conditionID= 107
percentAileron= 5
percentElevator= 6
percentAileronR= 5
percentElevatorR= -6
percentAileronFP= 50
percentElevatorFP= 60
percentAileronRFP= 50
percentElevatorRFP= -60
mixName="RUD > AIL/ELE"
activePositions=%0002
</RAE-Mix>
 */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogID = navigator.SelectSingleNode("/SPM/RAE-Mix/analogID").Value;
            Assert.That(analogID, Is.EqualTo("130"), "Check that 'analogID' has been converted"); // RUD
            var conditionID = navigator.SelectSingleNode("/SPM/RAE-Mix/conditionID").Value;
            Assert.That(conditionID, Is.EqualTo("107"), "Check that 'conditionID' has been converted"); // On
            var percentAileron = navigator.SelectSingleNode("/SPM/RAE-Mix/percentAileron").Value;
            Assert.That(percentAileron, Is.EqualTo("66"), "Check that 'percentAileron' has been copied");
            var percentAileronR = navigator.SelectSingleNode("/SPM/RAE-Mix/percentAileronR").Value;
            Assert.That(percentAileronR, Is.EqualTo("66"), "Check that 'percentAileronR' has been copied from 'percentAileron'");
            var percentElevator = navigator.SelectSingleNode("/SPM/RAE-Mix/percentElevator").Value;
            Assert.That(percentElevator, Is.EqualTo("77"), "Check that 'percentElevator' has been copied");
            var percentElevatorR = navigator.SelectSingleNode("/SPM/RAE-Mix/percentElevatorR").Value;
            Assert.That(percentElevatorR, Is.EqualTo("-77"), "Check that 'percentElevatorR' has been copied from 'percentElevator'");
            var activePositions = navigator.SelectSingleNode("/SPM/RAE-Mix/activePositions").Value;
            Assert.That(activePositions, Is.EqualTo("%0002"), "Check that 'activePositions' has been copied");
        }

        [Test]
        public void DX8ToDX9_CMix()
        {
            string expectedConditionID = "145"; // DX8 defaults to using FMode
            string expectedActivePositions = "%0000";
            var dx8Spm =
@"<C-Mix>
rateHighAilThr= 0
rateLowAilThr= 0
rateHighEleThr= 0
rateLowEleThr= 0
rateHighRudThr= 0
rateLowRudThr= 0
activePositions=%0000
</C-Mix>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/C-Mix");
            Assert.That(node, Is.Not.Null, "Check that 'C-Mix' has been copied");
            var conditionIDnode = navigator.SelectSingleNode("/SPM/C-Mix/conditionID");
            Assert.That(conditionIDnode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been added");
            var activePositionsNode = navigator.SelectSingleNode("/SPM/C-Mix/activePositions");
            Assert.That(activePositionsNode.Value, Is.EqualTo(expectedActivePositions), "Check that 'activePositions' has been copied");
        }

        [Test]
        public void DX8ToDX9_SMix()
        {
            string expectedConditionID = "145"; // DX8 defaults to using FMode
            var dx8Spm =
@"<S-Mix>
rateHighAilEle= 0
rateLowAilEle= 0
rateHighEleAil= 0
rateLowEleAil= 0
activePositions=%0000
</S-Mix>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/S-Mix");
            Assert.That(node, Is.Not.Null, "Check that 'S-Mix' has been copied");
            var conditionIDnode = navigator.SelectSingleNode("/SPM/S-Mix/conditionID");
            Assert.That(conditionIDnode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been added");
        }

        [Test]
        public void DX8ToDX9_SwashPlate()
        {
            var dx8Spm =
@"<SwashPlate>
rateAileron= 60
rateElevator= 60
ratePitch= 60
E-Ring=Disabled
Expo=Disabled
rateExpo= 30
</SwashPlate>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/SwashPlate");
            Assert.That(node, Is.Not.Null, "Check that 'SwashPlate' has been copied");
        }

        // Airplane support

        [Test]
        public void DX8ToDX9_Acro()
        {
            var dx8Spm =
@"<Acro>Wing=StandardTail=Normal</Acro>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var wingNode = navigator.SelectSingleNode("/SPM/Acro/Wing");
            Assert.That(wingNode.Value, Is.EqualTo("Standard"));
            var tailNode = navigator.SelectSingleNode("/SPM/Acro/Tail");
            Assert.That(tailNode.Value, Is.EqualTo("Normal"));
        }

        // Found in Spektrum 6ch_Glider.SPM sample.
        [Test]
        public void DX8ToDX9_Servo_RUD()
        {
            string expectedSourceID = "35"; // 239->35  - RUD (same as RUD?)
            var dx8Spm =
@"<Servo>*Index= 3sourceID= 239speed= 32736direction=NormalsubTrim= 0travelLow= -100travelHigh= 100name=RUD</Servo>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Servo/sourceID");
            Assert.That(node.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Servo_GYR()
        {
            string expectedSourceID = "200"; // 244->200  - Gyro

            var dx8Spm =
@"<Servo>
*Index= 4
sourceID= 244
name=GYR
</Servo>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/Servo/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Servo_GOV()
        {
            string expectedSourceID = "201"; // 245->201  - Governor
            var dx8Spm =
@"<Servo>*Index= 6sourceID= 245speed= 32736direction=NormalsubTrim= 0travelLow= -100travelHigh= 100name=GOV</Servo>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Servo/sourceID");
            Assert.That(node.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been converted");
        }

        [TestCase("DX8", "Dual_Ele", "6", "LEL", "8")]      // LEL servo moved from #6 on DX8 to #8 on DX9
        [TestCase("DX8", "Dual_Rud_Ele", "6", "LEL", "8")]  // LEL servo moved from #6 on DX8 to #8 on DX9
        [TestCase("DX7S", "Dual_Rud", "6", "LRU", "7")]     // LRU servo moved from #6 on DX7s to #7 on DX9
        [TestCase("DX7S", "Dual_Ele", "6", "LEL", "8")]     // LEL servo moved from #6 on DX7s to #8 on DX9
        // TODO: Add some negative tests
        public void DX8ToDX9_Servo_Tail(string generator, string tail, string index, string name, string expectedVSource)
        {
            var dx8Spm =
string.Format(@"<Spektrum>Generator=""{0}""</Spektrum><Acro>Wing=Dual_AilTail={1}</Acro><Servo>*Index= {2}sourceID= 194name={3}</Servo>", generator, tail, index, name);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Servo/vSource");
            Assert.That(node, Is.Not.Null, "Check that 'vSource' has been added");
            Assert.That(node.Value, Is.EqualTo(expectedVSource), "Check 'vSource' value");

            string xpath = string.Format("/SPM/Servo[Index/text()='{0}']", expectedVSource);
            var servoNode = navigator.SelectSingleNode(xpath);
            Assert.That(servoNode, Is.Not.Null, "Check that Servo has been added");
            Assert.That(servoNode.SelectSingleNode("vSource").Value, Is.EqualTo("74"), "Check that servo is inhibited");
            Assert.That(servoNode.SelectSingleNode("name").Value, Is.EqualTo("INH"), "Check that servo name is INH");
        }

        [Test]
        public void DX8ToDX9_Servo_SpeedDown()
        {
            string expectedSpeed = "666";

            var dx8Spm =
string.Format(@"<Servo>
*Index= 0
speed= {0}
</Servo>", expectedSpeed);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var speedNode = navigator.SelectSingleNode("/SPM/Servo/speed");
            Assert.That(speedNode.Value, Is.EqualTo(expectedSpeed), "Check that 'speed' has been converted");
            var speedDownNode = navigator.SelectSingleNode("/SPM/Servo/speedDown");
            Assert.That(speedDownNode.Value, Is.EqualTo(expectedSpeed), "Check that 'speedDown' has been copied from 'speed'");
        }

        [TestCase("DX8", "X1.00", "Reverse", "666", "-666")]
        [TestCase("DX8", " 1.00", "Reverse", "666", "-666")]

        [TestCase("DX8", " 2.04", "Reverse", "666", "-666")]
        [TestCase("DX8", " 2.04", "Normal", "666", "666")]
        [TestCase("DX8", " 2.05", "Reverse", "666", "666")]
        [TestCase("DX8", " 2.05", "Normal", "666", "666")]

        [TestCase("DX7S", " 1.01", "Reverse", "666", "-666")]
        [TestCase("DX7S", " 1.01", "Normal", "666", "666")]
        [TestCase("DX7S", " 1.02", "Reverse", "666", "666")]
        [TestCase("DX7S", " 1.02", "Normal", "666", "666")]
        public void DX8ToDX9_Servo_ConvertSubTrim(string generator, string vcode, string direction, string subTrim, string expectedSubTrim)
        {
            var dx8Spm =
string.Format(@"<Spektrum>Generator=""{0}""VCode=""{1}""</Spektrum>

<Servo>
*Index= 0
direction={2}
subTrim= {3}
</Servo>", generator, vcode, direction, subTrim);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var subTrimNode = navigator.SelectSingleNode("/SPM/Servo/subTrim");
            Assert.That(subTrimNode.Value, Is.EqualTo(expectedSubTrim), "Check 'subTrim' value");
        }

        [TestCase("DX8", "X1.00", "Reverse", "-100", "90", "-90", "100")]
        [TestCase("DX8", " 1.00", "Reverse", "-100", "90", "-90", "100")]

        [TestCase("DX8", " 2.04", "Reverse", "-100", "90", "-90", "100")]
        [TestCase("DX8", " 2.04", "Normal", "-100", "90", "-100", "90")]
        [TestCase("DX8", " 2.05", "Reverse", "-100", "90", "-100", "90")]
        [TestCase("DX8", " 2.05", "Normal", "-100", "90", "-100", "90")]

        [TestCase("DX7S", " 1.01", "Reverse", "-100", "90", "-90", "100")]
        [TestCase("DX7S", " 1.01", "Normal", "-100", "90", "-100", "90")]
        [TestCase("DX7S", " 1.02", "Reverse", "-100", "90", "-100", "90")]
        [TestCase("DX7S", " 1.02", "Normal", "-100", "90", "-100", "90")]
        public void DX8ToDX9_Servo_ConvertTravel(string generator, string vcode, string reverse, string travelLow, string travelHigh,
            string expectedTravelLow, string expectedTravelHigh)
        {
            var dx8Spm =
string.Format(@"<Spektrum>Generator=""{0}""VCode=""{1}""</Spektrum>

<Servo>
*Index= 0
direction= {2}
travelLow= {3}
travelHigh= {4}
</Servo>", generator, vcode, reverse, travelLow, travelHigh);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var travelLowNode = navigator.SelectSingleNode("/SPM/Servo/travelLow");
            Assert.That(travelLowNode.Value, Is.EqualTo(expectedTravelLow), "Check 'travelLow'");
            var travelHighNode = navigator.SelectSingleNode("/SPM/Servo/travelHigh");
            Assert.That(travelHighNode.Value, Is.EqualTo(expectedTravelHigh), "Check 'travelHigh'");
        }

        [Test]
        public void DX8ToDX9_Servo()
        {
            var dx8Spm =
@"<Servo>
*Index= 0
sourceID= 192
speed= 32736
direction=Normal
subTrim= 0
travelLow= -100
travelHigh= 100
name=THR
</Servo>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Servo/sourceID");
            Assert.That(node.Value, Is.EqualTo("32"), "Check that 'sourceID' has been converted");
        }

        [Test]
        public void DX8ToDX9_DR_Expo()
        {
            string expectedAnalogID = "65"; // 17-> 65  - Ail. Stick
            string expectedConditionID = "87"; // 45-> 87  - Ail D/R->Switch F
            var dx8Spm =
@"<DR_Expo>
*Index= 0
analogID= 17
conditionID= 45
activePositions=%0000
drHigh: 100 100 100 100 100
drLow: 100 100 100 100 100
expoHigh: 0 0 0 0 0
expoLow: 0 0 0 0 0
</DR_Expo>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/DR_Expo/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDnode = navigator.SelectSingleNode("/SPM/DR_Expo/conditionID");
            Assert.That(conditionIDnode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        [Test]
        public void DX8ToDX9_P_Mix()
        {
            string expectedAnalogID = "129"; // 97-> 129 - ELE (analogID)
            string expectedConditionID = "145"; // 127->145 - Flight Mode
            string expectedOutChan = "37"; // 197->37  - AX1 (outChan)
            var dx8Spm =
@"<P-Mix>*Index= 0analogID= 97conditionID= 127trimID= 0activePositions=%0002outChan= 197[Curvedata]*Index= 0points= 3Expo=DisabledtrimActive=DisabledX: -1023 0 1023 0 0 0 0Y: 511 0 -511 0 0 0 0[/Curvedata]</P-Mix>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/P-Mix/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDnode = navigator.SelectSingleNode("/SPM/P-Mix/conditionID");
            Assert.That(conditionIDnode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var outChanNode = navigator.SelectSingleNode("/SPM/P-Mix/outChan");
            Assert.That(outChanNode.Value, Is.EqualTo(expectedOutChan), "Check that 'outChan' has been converted");
        }

        [TestCase("Heli", "0", "%000F", "145")] // Flight Mode
        [TestCase("Heli", "0", "%0000", "0")]   // Inh
        [TestCase("Heli", "40", "%0001", "82")] // Gear->Switch A
        [TestCase("Acro", "0", "%000F", "0")]   // Inh
        [TestCase("Acro", "40", "%0001", "82")] // Gear->Switch A
        public void DX8ToDX9_P_Mix_Heli(string type, string conditionID, string activePositions, string expectedConditionID)
        {
            var dx8Spm = string.Format(
@"<Spektrum>Type={0}</Spektrum>

<P-Mix>
*Index= 0
conditionID= {1}
activePositions={2}
</P-Mix>", type, conditionID, activePositions);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var conditionIDnode = navigator.SelectSingleNode("/SPM/P-Mix/conditionID");
            Assert.That(conditionIDnode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        [TestCase("DX8", "Acro", "Standard", "Dual_Rud_Ele", "198", "40")]  // DX8 198(LEL)->40
        [TestCase("DX8", "Acro", "Standard", "Dual_Ele", "198", "40")]      // DX8 198(LEL)->40
        [TestCase("DX8", "Acro", "Standard", "Dual_Rud", "199", "39")]      // DX8 198(LRU)->39
        [TestCase("DX8", "Acro", "Standard", "Normal", "198", "38")]        // DX8 198(AX2)->38
        [TestCase("DX7S", "Acro", "Standard", "Dual_Ele", "198", "40")]     // DX7s 198(LEL)->40
        [TestCase("DX7S", "Acro", "Standard", "Dual_Rud", "198", "39")]     // DX7s 198(LRU)->39
        [TestCase("DX7S", "Acro", "Standard", "Normal", "198", "38")]       // DX7s 198(AX2)->38

        [TestCase("DX8", "Acro", "Standard", "Dual_Rud_Ele", "7", "9")]     // DX8 7(AX2)->9(AX4)
        [TestCase("DX8", "Acro", "Standard", "Dual_Ele", "7", "9")]         // DX8 7(AX2)->9(AX4)
        [TestCase("DX8", "Acro", "Standard", "Dual_Rud", "8", "8")]         // DX8 8(AX3)->8(AX3)
        [TestCase("DX8", "Acro", "Standard", "Normal", "7", "7")]           // DX8 7(AX2)->7(AX2)
        [TestCase("DX7S", "Acro", "Standard", "Dual_Ele", "7", "9")]        // DX7s 7(AX2)->9(AX4)
        [TestCase("DX7S", "Acro", "Standard", "Dual_Rud", "7", "8")]        // DX7s 7(AX2)->8(AX3)
        [TestCase("DX7S", "Acro", "Standard", "Normal", "7", "7")]          // DX7s 7(AX2)->7(AX2)

        [TestCase("DX8", "Acro", "Ail_2_Flap_1", "Normal", "192", "32")]    // DX8 192(THR)->32(THR)
        [TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "192", "38")]    // DX8 192(THR)->38(AX2) - MOT
        [TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "192", "38")]    // DX8 192(THR)->38(AX2) - MOT
        [TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "16", "7")]      // DX8  16(THR)-> 7(AX2) - MOT
        [TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "16", "7")]      // DX8  16(THR)-> 7(AX2) - MOT
        [TestCase("DX8", "Sail", "Standard", "Normal", "192", "32")]        // DX8 192(THR)->32(THR)
        [TestCase("DX8", "Acro", "Ail_2_Flap_2", "Normal", "196", "36")]    // DX8 196(GER)->36(GER)
        [TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "196", "37")]    // DX8 196(GER)->37(AX1) - LFL
        //[TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "33", "6")]      // DX8  33(GER)-> 6(AX1) - LFL
        [TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "196", "36")]    // DX8 196(GER)->36(GER)
        [TestCase("DX8", "Acro", "Ail_2_Flap_1", "Normal", "197", "37")]    // DX8 197(AX1)->37(AX1)
        [TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "197", "32")]    // DX8 197(AX1)->32(THR) - LAL
        [TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "197", "32")]    // DX8 197(AX1)->32(THR) - LAL
        //[TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "6", "64")]      // DX8   6(AX1)->64(THR) - LAL
        //[TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "6", "64")]      // DX8   6(AX1)->64(THR) - LAL
        [TestCase("DX8", "Sail", "Standard", "Normal", "197", "37")]        // DX8 197(AX1)->37(AX1)
        [TestCase("DX8", "Acro", "Ail_2_Flap_2", "Normal", "198", "38")]    // DX8 198(AX2)->38(AX2)
        [TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "198", "36")]    // DX8 198(AX2)->32(GER) - RFL
        [TestCase("DX8", "Sail", "Ail_2_Flap_2", "Normal", "7", "79")]      // DX8   7(AX2)->79(GER) - RFL / GER (analogID)
        [TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "7", "AX2_NOT_AVAILABLE")] // DX8   7(AX2)->AX2_NOT_AVAILABLE_ON_DX9
        [TestCase("DX8", "Sail", "Ail_2_Flap_1", "Normal", "198", "38")]    // DX8 198(AX2)->38(AX2)
        public void DX8ToDX9_P_Mix_Remap(string generator, string type, string wing, string tail, string outChan, string expectedOutChan)
        {
            var dx8Spm =
string.Format(@"<Spektrum>Generator=""{0}""Type={1}</Spektrum><{1}>Wing={2}Tail={3}</{1}><P-Mix>*Index= 1analogID= 16conditionID= 63trimID= 0activePositions=%00FFoutChan= {4}</P-Mix>", generator, type, wing, tail, outChan);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var outChanNode = navigator.SelectSingleNode("/SPM/P-Mix/outChan");
            Assert.That(outChanNode.Value, Is.EqualTo(expectedOutChan), "Check that 'outChan' has been converted");
        }

        [Test]
        public void DX8ToDX9_ThroCut()
        {
            string expectedConditionID = "89"; // 47-> 89  - Mix/Hold->Switch H
            var dx8Spm =
@"<ThroCut>conditionID= 47percent= 306rampSpeed= 32736activePositions=%FFFE</ThroCut>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/ThroCut/conditionID");
            Assert.That(node.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        [Test]
        public void DX8ToDX9_ThroCurve()
        {
            // DX9 will add:
            // mixName="Throttle"
            string expectedAnalogID = "64"; // 16-> 64  - Thr. Stick
            string expectedConditionID = "85"; // 43-> 85  - Flap->Switch D
            string expectedTrimID = "108"; // 64-> 108 - Throttle Trim (ThroCurve/trimID)
            var dx8Spm =
@"<ThroCurve>
analogID= 16
conditionID= 43
trimID= 64
activeMask=%0000
delay= 0
assignedCurve: 0 1 2 3 1

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
X: -1023 -511 0 511 1023 0 0
Y: -1023 -511 0 511 1023 0 0
[/Curvedata]
</ThroCurve>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var trimIDNode = navigator.SelectSingleNode("/SPM/ThroCurve/trimID");
            Assert.That(trimIDNode.Value, Is.EqualTo(expectedTrimID), "Check that 'trimID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Timer()
        {
            string expectedStartID = "1"; // 1->  1   - THR Servo Out (Timer/StartID)
            var dx8Spm =
@"<Timer>*Index= 0Mode=DownMinutes= 4Seconds= 0oneTime=DisabledAudio=EnabledVibrate=DisabledStartID= 1Event= 2Thresh= -511activePositions=%FFFE</Timer>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Timer/StartID");
            Assert.That(node.Value, Is.EqualTo(expectedStartID), "Check that 'StartID' has been converted");
            var activePositionsNode = navigator.SelectSingleNode("/SPM/Timer/activePositions");
            Assert.That(activePositionsNode, Is.Null, "Check that 'activePositions' is gone"); // Spotted in DX7S 1.03. Not compatible with DX8.
            var audioNode = navigator.SelectSingleNode("/SPM/Timer/Audio");
            Assert.That(audioNode.Value, Is.EqualTo("Enabled"), "Check that 'Audio' has been copied");
            var vibrateNode = navigator.SelectSingleNode("/SPM/Timer/Vibrate");
            Assert.That(vibrateNode.Value, Is.EqualTo("Disabled"), "Check that 'Vibrate' has been copied");
        }

        // NOTE: Not supported in DX9 1.0 firmware.
        /*
        [TestCase("Enabled", "%00F4")]
        [TestCase("Disabled", "%0080")]
        public void DX8ToDX9_Timer_Audio(string audio, string expectedAudioX)
        {
            var dx8Spm = string.Format(
@"<Timer>*Index= 0Audio={0}</Timer>", audio);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Timer/audioX");
            Assert.That(node.Value, Is.EqualTo(expectedAudioX), "Check that 'Audio' has been converted to 'audioX'");
        }

        [TestCase("Enabled", "%0020")]
        [TestCase("Disabled", "%0000")]
        public void DX8ToDX9_Timer_Vibrate(string vibrate, string expectedVibeX)
        {
            var dx8Spm = string.Format(
@"<Timer>*Index= 0Vibrate={0}</Timer>", vibrate);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Timer/vibeX");
            Assert.That(node.Value, Is.EqualTo(expectedVibeX), "Check that 'Vibrate' has been converted to 'vibeX'");
        }
*/

        [TestCase("/SPM/RAE-Mix/analogID", true)]
        [TestCase("/SPM/RAE-Mix/conditionID", true)]
        [TestCase("/SPM/RAE-Mix/percentAileron", true)]
        [TestCase("/SPM/RAE-Mix/percentElevator", true)]
        [TestCase("/SPM/RAE-Mix/percentAileronR", false)]
        [TestCase("/SPM/RAE-Mix/percentElevatorR", false)]
        [TestCase("/SPM/RAE-Mix/percentAileronFP", false)]
        [TestCase("/SPM/RAE-Mix/percentElevatorFP", false)]
        [TestCase("/SPM/RAE-Mix/percentAileronRFP", false)]
        [TestCase("/SPM/RAE-Mix/percentElevatorRFP", false)]
        [TestCase("/SPM/RAE-Mix/mixName", false)]
        [TestCase("/SPM/RAE-Mix/activePositions", true)]
        public void DX9To_RAEMix(string element, bool keep)
        {
            var dx9Spm =
@"<RAE-Mix>
analogID= 130
conditionID= 107
percentAileron= 5
percentElevator= 6
percentAileronR= 5
percentElevatorR= -6
percentAileronFP= 50
percentElevatorFP= 60
percentAileronRFP= 50
percentElevatorRFP= -60
mixName=""RUD > AIL/ELE""
activePositions=%0002
</RAE-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }
        
        [Test]
        public void DX8ToDX9_EF_Mix()
        {
            // TODO: mixName="ELE > FLP"
            string expectedAnalogID = "129"; // 97-> 129 - ELE (analogID)
            string expectedConditionID = "145"; // 127->145 - Flight Mode
            string expectedOutChan = "52"; // 200->52  - LFL (EF-Mix/outChan)
            var dx8Spm =
@"<EF-Mix>
analogID= 97
conditionID= 127
trimID= 0
activePositions=%0002
outChan= 200

[Curvedata]
*Index= 0
points= 3
Expo=Disabled
trimActive=Disabled
X: -1023 0 1023 0 0 0 0
Y: 0 0 0 0 0 0 0
[/Curvedata]
</EF-Mix>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/EF-Mix/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDNode = navigator.SelectSingleNode("/SPM/EF-Mix/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var outChanNode = navigator.SelectSingleNode("/SPM/EF-Mix/outChan");
            Assert.That(outChanNode.Value, Is.EqualTo(expectedOutChan), "Check that 'outChan' has been converted");
        }

        [Test]
        public void DX8ToDX9_AR_Mix()
        {
            // DX9 will add:
            // mixName="AIL > RUD"
            string expectedAnalogID = "128"; // 96-> 128 - AIL (analogID)
            string expectedConditionID = "145"; // 127->145 - Flight Mode
            string expectedOutChan = "35"; // 195->35 - RUD
            var dx8Spm =
            // Guessing conditionID and activePositions.
@"<AR-Mix>
analogID= 96
conditionID= 127
trimID= 0
activePositions=%0002
outChan= 195

[Curvedata]
*Index= 0
points= 3
Expo=Disabled
trimActive=Disabled
X: -1023 0 1023 0 0 0 0
Y: -511 0 -511 0 0 0 0[/Curvedata]
</AR-Mix>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/AR-Mix/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedAnalogID), "Check that 'analogID' has been converted");
            var conditionIDNode = navigator.SelectSingleNode("/SPM/AR-Mix/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
            var outChanNode = navigator.SelectSingleNode("/SPM/AR-Mix/outChan");
            Assert.That(outChanNode.Value, Is.EqualTo(expectedOutChan), "Check that 'outChan' has been converted");
            var curvedataXNode = navigator.SelectSingleNode("/SPM/AR-Mix/Curvedata/X/Element");
            Assert.That(curvedataXNode.Value, Is.EqualTo("-1023"), "Check that 'X' has not been inverted");
            var curvedataYNode = navigator.SelectSingleNode("/SPM/AR-Mix/Curvedata/Y/Element");
            Assert.That(curvedataYNode.Value, Is.EqualTo("511"), "Check that 'Y' has been inverted");
        }

        [Test]
        public void DX8ToDX9_FlapSystem_conditionID()
        {
            string expectedConditionID = "85"; // 43-> 85  - Flap->Switch D
            var dx8Spm =
@"<FlapSystem>
analogID= 0
conditionID= 43
trimID= 0
speed= 32736
flapTarget: 0 0 0 0 0
elevatorTarget: 0 0 0 0 0

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
X: -1023 -511 0 511 1023 0 0
Y: -1023 -511 0 511 1023 0 0
[/Curvedata]
</FlapSystem>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var conditionIDNode = navigator.SelectSingleNode("/SPM/FlapSystem/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        // 0, 68, 69, 75, 
        [TestCase("0", "0")]    // Inhibit
        [TestCase("68", "112")] // 68-> 112 - LTrimD
        [TestCase("69", "113")] // 69-> 113 - RTrimD
        [TestCase("75", "0")]   // 75-> 0   - Knob: FlpTrm (not supported on DX9?)
        public void DX8ToDX9_FlapSystem_trimID(string trimID, string expectedTrimID)
        {
            var dx8Spm = string.Format(
@"<FlapSystem>
trimID= {0}
</FlapSystem>", trimID);

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/FlapSystem/trimID");
            Assert.That(node.Value, Is.EqualTo(expectedTrimID), "Check that 'trimID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Differential()
        {
            string expectedConditionID = "86"; // 44-> 86  - AUX 2->Switch E
            var dx8Spm =
@"<Differential>
conditionID= 44
rate: 0 0 0 0 0
</Differential>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var conditionIDNode = navigator.SelectSingleNode("/SPM/Differential/conditionID");
            Assert.That(conditionIDNode.Value, Is.EqualTo(expectedConditionID), "Check that 'conditionID' has been converted");
        }

        [Test]
        public void DX8ToDX9_FMode_Acro()
        {
            string expectedSwitchA = "83"; // 41-> 83  - F Mode->Switch B
            string expectedSwitchB = "82"; // 40-> 82  - Gear->Switch A
            string expectedSwitchC = "0";
            var dx8Spm =
@"<Acro>Wing=StandardTail=Normal</Acro><FMode>switch_a= 41switch_b= 40switch_c= 0size= 9data: 0 1 2 1 3 3 2 3 3</FMode>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var switchANode = navigator.SelectSingleNode("/SPM/FMode/switch_a");
            Assert.That(switchANode.Value, Is.EqualTo(expectedSwitchA), "Check that 'switch_a' has been converted");
            var switchBNode = navigator.SelectSingleNode("/SPM/FMode/switch_b");
            Assert.That(switchBNode.Value, Is.EqualTo(expectedSwitchB), "Check that 'switch_b' has been converted");
            var switchCNode = navigator.SelectSingleNode("/SPM/FMode/switch_c");
            Assert.That(switchCNode.Value, Is.EqualTo(expectedSwitchC), "Check that 'switch_c' has been converted");
            var sizeNode = navigator.SelectSingleNode("/SPM/FMode/size");
            Assert.That(sizeNode, Is.Not.Null, "Check that 'size' has been converted");
            Assert.That(sizeNode.Value, Is.EqualTo("18"));
            StringAssert.Contains("fmtable: 0 1 2 0 0 0 1 3 3 0 0 0 2 3 3 0 0 0", dx9Spm);
        }

        [Test]
        public void DX8ToDX9_Special()
        {
            string expectedSourceID = "85"; // 43-> 85  - Flap->Switch D
            var dx8Spm =
@"<Special>
*Index= 0
sourceID= 43
</Special>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/Special/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been converted");
        }

        [Test]
        public void DX8ToDX9_SoftSw()
        {
            string expectedSourceID = "8"; // 8->  8   - AX3 Servo Out
            var dx8Spm =
@"<SoftSw>
*Index= 1
sourceID= 8
</SoftSw>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/SoftSw/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been converted");
        }

        [Test]
        public void DX8ToDX9_SoftSw_FlpTrm()
        {
            string expectedSourceID = "0"; // 70-> 0   - FlpTrm (not supported on DX9?)
            var dx8Spm =
@"<SoftSw>*Index= 4sourceID= 70</SoftSw>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/SoftSw/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been inhibited");
        }

        [Test]
        public void DX8ToDX9_SoftSw_Flaps()
        {
            string expectedSourceID = "0"; // 242->0   - Flaps (not supported on DX9?)
            var dx8Spm =
@"<SoftSw>
*Index= 10
sourceID= 242
</SoftSw>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/SoftSw/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been inhibited");
        }

        [Test]
        public void DX8ToDX9_TrimID()
        {
            // When is sourceID ever not 0?
            string expectedSourceID = "0"; // Inhibit
            var dx8Spm =
@"<TrimID>
*Index= 0
sourceID= 0
</TrimID>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/TrimID/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID), "Check that 'sourceID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Telemetry_StartID()
        {
            string expectedStartID = "82"; // Gear->Switch A
            var dx8Spm =
@"<Telemetry>
StartID= 40
</Telemetry>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var startIDNode = navigator.SelectSingleNode("/SPM/Telemetry/StartID");
            Assert.That(startIDNode.Value, Is.EqualTo(expectedStartID), "Check that 'StartID' has been converted");
        }

        [Test]
        public void DX8ToDX9_Telemetry_FlightLog_sdEnabled()
        {
            var dx8Spm =
@"<Telemetry>
[FlightLog]
F= 0
H= 0
A= 0
B= 0
R= 0
L= 0
FrameAlarm=Disabled
HoldAlarm=Disabled
minRx= 43
maxRx= 81
RxVoltAlarm=Disabled
[/FlightLog]
</Telemetry>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var flightLogNode = navigator.SelectSingleNode("/SPM/Telemetry/FlightLog");
            Assert.That(flightLogNode, Is.Not.Null, "Check that 'FlightLog' exists");
            var sdEnabledNode = flightLogNode.SelectSingleNode("sdEnabled");
            Assert.That(sdEnabledNode, Is.Null, "Check that 'sdEnabled' hasn't been added"); // Not compatible with DX18
            //Assert.That(sdEnabledNode, Is.Not.Null, "Check that 'sdEnabled' exists");
            //Assert.That(sdEnabledNode.Value, Is.EqualTo("1"), "Check that 'sdEnabled' is '1' (Enabled)");
        }

        [Test]
        public void DX8ToDX9_Telemetry_Module()
        {
            var dx8Spm =
@"<Telemetry>
[Module]
*Device=Tachometer
[/Module]
</Telemetry>";

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var moduleNode = navigator.SelectSingleNode("/SPM/Telemetry/Module");
            Assert.That(moduleNode, Is.Not.Null, "Check that 'Module' exists");
            var deviceNode = moduleNode.SelectSingleNode("Device");
            Assert.That(deviceNode, Is.Not.Null, "Check that 'Device' exists");
            Assert.That(deviceNode.Value, Is.EqualTo("Tachometer"));
            StringAssert.Contains("*Device=", dx9Spm);
        }

        [Test]
        public void DX8ToDX9_Trainer_Active()
        {
            var dx8Spm =
@"<Trainer>
Active: Disabled Enabled Disabled Enabled Disabled Enabled Disabled Enabled
</Trainer>";

            /*
            <Trainer>
            mixOrNormal=%0000
            mixRatio: 0 100 0 100 0 100 0 100
            </Trainer>
             */

            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var mixOrNormalNode = navigator.SelectSingleNode("/SPM/Trainer/mixOrNormal");
            Assert.That(mixOrNormalNode, Is.Not.Null, "Check 'mixOrNormal' has been created");
            Assert.That(mixOrNormalNode.Value, Is.EqualTo("%0000"));
            var activeNode = navigator.SelectSingleNode("/SPM/Trainer/Active");
            Assert.That(activeNode, Is.Null, "Check 'Active' has been removed");
            var mixRatioNode = navigator.SelectSingleNode("/SPM/Trainer/mixRatio");
            Assert.That(mixRatioNode, Is.Not.Null, "Check 'mixRatio' has been created");
            StringAssert.Contains("mixRatio: 0 100 0 100 0 100 0 100", dx9Spm);
        }

        [TestCase("/SPM/Trainer/Type", "Normal")]
        [TestCase("/SPM/Trainer/mixOrNormal", "%0000")]
        [TestCase("/SPM/Trainer/conditionID", "92")] // Trainer->Switch I
        [TestCase("/SPM/Trainer/MOverride", "Disabled")]
        [TestCase("/SPM/Trainer/activePositions", "254")]
        [TestCase("/SPM/Trainer/Active", null)]
        public void DX8ToDX9_Trainer_Normal(string name, string expectedValue)
        {
            var dx8Spm =
@"<Trainer>
Type=Normal
Active: Disabled Enabled Disabled Enabled Disabled Enabled Disabled Enabled
</Trainer>";

            /*
            <Trainer>
            Type=Normal
            mixOrNormal=%0000
            mixRatio: 0 100 0 100 0 100 0 100
            conditionID= 92
            MOverride=Disabled
            activePositions= 254
            </Trainer>
            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (expectedValue != null)
            {
                Assert.That(node, Is.Not.Null, string.Format("Check '{0}' exists", name));
                Assert.That(node.Value, Is.EqualTo(expectedValue));
            }
            else
            {
                Assert.That(node, Is.Null, string.Format("Check '{0}' doesn't exist", name));
            }
        }

        [TestCase("/SPM/Trainer/Type", "PMaster")]
        [TestCase("/SPM/Trainer/conditionID", "92")] // Trainer->Switch I
        [TestCase("/SPM/Trainer/MOverride", "Disabled")]
        [TestCase("/SPM/Trainer/activePositions", "254")]
        [TestCase("/SPM/Trainer/mixOrNormal", null)]
        [TestCase("/SPM/Trainer/mixRatio", null)]
        public void DX8ToDX9_Trainer_PMaster(string name, string expectedValue)
        {
            var dx8Spm =
@"<Trainer>
Type=PMaster
</Trainer>";

            /*
            <Trainer>
            Type=PMaster
            conditionID= 92
            MOverride=Disabled
            activePositions= 254
            </Trainer>
            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (expectedValue != null)
            {
                Assert.That(node, Is.Not.Null, string.Format("Check '{0}' exists", name));
                Assert.That(node.Value, Is.EqualTo(expectedValue));
            }
            else
            {
                Assert.That(node, Is.Null, string.Format("Check '{0}' doesn't exist", name));
            }
        }

        [TestCase("/SPM/Trainer/Type", "PSlave")]
        [TestCase("/SPM/Trainer/conditionID", "92")] // Trainer->Switch I
        [TestCase("/SPM/Trainer/MOverride", "Disabled")]
        [TestCase("/SPM/Trainer/activePositions", "254")]
        [TestCase("/SPM/Trainer/mixOrNormal", null)]
        [TestCase("/SPM/Trainer/mixRatio", null)]
        public void DX8ToDX9_Trainer_PSlave(string name, string expectedValue)
        {
            var dx8Spm =
@"<Trainer>
Type=PSlave
</Trainer>";

            /*
            <Trainer>
            Type=PSlave
            conditionID= 92
            MOverride=Disabled
            activePositions= 254
            </Trainer>
            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (expectedValue != null)
            {
                Assert.That(node, Is.Not.Null, string.Format("Check '{0}' exists", name));
                Assert.That(node.Value, Is.EqualTo(expectedValue));
            }
            else
            {
                Assert.That(node, Is.Null, string.Format("Check '{0}' doesn't exist", name));
            }
        }

        [TestCase("/SPM/Trainer/Type", "Disabled")]
        [TestCase("/SPM/Trainer/conditionID", "92")] // Trainer->Switch I
        [TestCase("/SPM/Trainer/MOverride", "Disabled")]
        [TestCase("/SPM/Trainer/activePositions", "254")]
        [TestCase("/SPM/Trainer/mixOrNormal", null)]
        [TestCase("/SPM/Trainer/mixRatio", null)]
        public void DX8ToDX9_Trainer_Disabled(string name, string expectedValue)
        {
            var dx8Spm =
@"<Trainer>
Type=Disabled
</Trainer>";

            /*
            <Trainer>
            Type=Disabled
            conditionID= 92
            MOverride=Disabled
            activePositions= 254
            </Trainer>
            */
            var dx9Spm = SpmConvert.DX8To(dx8Spm);

            var xml = SpmToXml.Convert(dx9Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (expectedValue != null)
            {
                Assert.That(node, Is.Not.Null, string.Format("Check '{0}' exists", name));
                Assert.That(node.Value, Is.EqualTo(expectedValue));
            }
            else
            {
                Assert.That(node, Is.Null, string.Format("Check '{0}' doesn't exist", name));
            }
        }

        // ==============

        // TODO: Convert name....    "10: Blar" -> "Blar", File: 10BLAR

        [TestCase("TrimID", 8)]
        [TestCase("P-Mix", 6)]
        [TestCase("Servo", 8)]
        [TestCase("Timer", 1)]
        public void DX9To_MaxIndex(string elementName, int elementNumber)
        {
            var dx9Spm = string.Format(
@"<{0}>
*Index= {1}
</{0}>

<{0}>
*Index= {2}
</{0}>", elementName, elementNumber - 1, elementNumber);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var nodeA = navigator.SelectSingleNode(string.Format("/SPM/{0}[Index={1}]", elementName, elementNumber - 1));
            Assert.That(nodeA, Is.Not.Null);
            var nodeB = navigator.SelectSingleNode(string.Format("/SPM/{0}[Index={1}]", elementName, elementNumber));
            Assert.That(nodeB, Is.Null);
        }

        [Test]
        public void DX9To_Spektrum_Generator()
        {
            string expectedGenerator = "DX8";
            var dx9Spm = @"<Spektrum>
Generator=""DX9""
VCode="" 1.03""
</Spektrum>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var generatorNode = navigator.SelectSingleNode("/SPM/Spektrum/Generator[@Type='String']");
            Assert.That(generatorNode.Value, Is.EqualTo(expectedGenerator));
            var vcodeNode = navigator.SelectSingleNode("/SPM/Spektrum/VCode[@Type='String']");
            Assert.That(vcodeNode, Is.Null);    // Don't specify version
        }

        [TestCase("Acro", "Acro")]
        [TestCase("X: Acro", "X: Acro")]
        [TestCase("0123456789", "0123456789")]
        [TestCase("1: Acro", "Acro")]
        [TestCase("0123456789X", "0123456789")]
        public void DX9To_Spektrum_Name(string name, string expectedName)
        {
            var dx9Spm = string.Format(
@"<Spektrum>
Name=""{0}""
</Spektrum>", name);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            string dx8Name = navigator.SelectSingleNode("/SPM/Spektrum/Name").Value;
            Assert.That(dx8Name, Is.EqualTo(expectedName));
        }

        [TestCase("/SPM/Spektrum/Generator", true)]
        [TestCase("/SPM/Spektrum/VCode", false)]
        [TestCase("/SPM/Spektrum/Originator", false)] // NOTE: Should comment out
        [TestCase("/SPM/Spektrum/mmNum", false)]
        [TestCase("/SPM/Spektrum/bCode", false)]
        [TestCase("/SPM/Spektrum/PosIndex", true)]
        [TestCase("/SPM/Spektrum/Type", true)]
        [TestCase("/SPM/Spektrum/curveIndex", true)]
        [TestCase("/SPM/Spektrum/enabXPLUS", false)]
        [TestCase("/SPM/Spektrum/Name", true)]
        public void DX9To_Spektrum(string element, bool keep)
        {
            var dx9Spm =
@"<Spektrum>
Generator=""DX9""
VCode="" 1.03""
Originator=""HS309XBwg4v74CHkcIOIrSLaVDKKkj""
mmNum=30
bCode=0
PosIndex= 5
Type=Acro
curveIndex= 7
enabXPLUS=Disabled
Name=""31: Acro""
</Spektrum>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Acro/Wing", true)]
        [TestCase("/SPM/Acro/Tail", true)]
        public void DX9To_Acro(string element, bool keep)
        {
            var dx9Spm =
@"<Acro>
Wing=Standard
Tail=Normal
</Acro>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Servo/Index", true)]
        [TestCase("/SPM/Servo/sourceID", true)]
        [TestCase("/SPM/Servo/speed", true)]
        [TestCase("/SPM/Servo/conditionID", false)]
        [TestCase("/SPM/Servo/activePositions", false)]
        [TestCase("/SPM/Servo/speedDown", false)]
        [TestCase("/SPM/Servo/absLimitHigh", false)]
        [TestCase("/SPM/Servo/absLimitLow", false)]
        [TestCase("/SPM/Servo/direction", true)]
        [TestCase("/SPM/Servo/subTrim", true)]
        [TestCase("/SPM/Servo/travelLow", true)]
        [TestCase("/SPM/Servo/travelHigh", true)]
        [TestCase("/SPM/Servo/name", true)]
        [TestCase("/SPM/Servo/vSource", false)]
        [TestCase("/SPM/Servo/Curvedata", false)]
        public void DX9To_Servo(string element, bool keep)
        {
            var dx9Spm =
@"<Servo>
*Index= 0
sourceID= 32
speed= 32736
conditionID= 107
activePositions=%0002
speedDown= 32736
absLimitHigh= 2047
absLimitLow= 0
direction=Normal
subTrim= 0
travelLow= -100
travelHigh= 100
name=THR
vSource= 0

[Curvedata]
*Index= 0
points= 7
Expo=Disabled
trimActive=Disabled
X: 0 341 682 1023 1364 1705 2047
Y: 0 341 682 1023 1364 1705 2047
[/Curvedata]
</Servo>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        // Servo> 0,                21, 40, 41, 43, 44, 47, 50, 69,     192, 193, 194, 195, 199, 200, 238, 239,      244, 245, 
        // All>   0, 1, 5, 6, 7, 8, 21, 40, 41, 43, 44, 47, 50, 69, 70, 192, 193, 194, 195, 199, 200, 238, 239, 242, 244, 245, 
        [TestCase("0", "0")]     // Inhibit
        [TestCase("69", "21")]   // R Knob
        [TestCase("82", "40")]   // Gear->Switch A
        [TestCase("83", "41")]   // F Mode->Switch B 
        [TestCase("85", "43")]   // Flap->Switch D
        [TestCase("86", "44")]   // AUX 2->Switch E
        [TestCase("89", "47")]   // Mix/Hold->Switch H
        [TestCase("92", "50")]   // Trainer->Switch I
        [TestCase("113", "69")]  // RTrimD
        [TestCase("32", "192")]  // THR
        [TestCase("33", "193")]  // AIL 
        [TestCase("34", "194")]  // ELE 
        [TestCase("35", "195")]  // RUD
        [TestCase("39", "199")]  // FLP/LFL/GER/AX3
        [TestCase("52", "200")]  // FLP/LFL
        [TestCase("200", "244")] // Gyro
        [TestCase("201", "245")] // Governor 
        public void DX9To_Servo_sourceID(string sourceID, string expectedSourceID)
        {
            var dx9Spm = string.Format(
@"<Servo>
*Index= 0
sourceID= {0}
</Servo>", sourceID);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var sourceIDNode = navigator.SelectSingleNode("/SPM/Servo/sourceID");
            Assert.That(sourceIDNode.Value, Is.EqualTo(expectedSourceID));
        }

        [TestCase("/SPM/DR_Expo/Index", true)]
        [TestCase("/SPM/DR_Expo/analogID", true)]
        [TestCase("/SPM/DR_Expo/conditionID", true)]
        [TestCase("/SPM/DR_Expo/activePositions", true)]
        [TestCase("/SPM/DR_Expo/drHigh", true)]
        [TestCase("/SPM/DR_Expo/drLow", true)]
        [TestCase("/SPM/DR_Expo/drHigh1", false)]
        [TestCase("/SPM/DR_Expo/drLow1", false)]
        [TestCase("/SPM/DR_Expo/drHigh2", false)]
        [TestCase("/SPM/DR_Expo/drLow2", false)]
        [TestCase("/SPM/DR_Expo/expoHigh", true)]
        [TestCase("/SPM/DR_Expo/assignedCurve", false)]
        [TestCase("/SPM/DR_Expo/expoLow", true)]
        public void DX9To_DR_Expo(string element, bool keep)
        {
            var dx9Spm =
@"<DR_Expo>
*Index= 0
analogID= 65
conditionID= 107
activePositions=%0000
drHigh: 100 100 100 100 100 0 0 0 0 0
drLow: 100 100 100 100 100 0 0 0 0 0
drHigh1: 100 100 100 100 100 0 0 0 0 0
drLow1: 100 100 100 100 100 0 0 0 0 0
drHigh2: 100 100 100 100 100 0 0 0 0 0
drLow2: 100 100 100 100 100 0 0 0 0 0
expoHigh: 0 0 0 0 0 0 0 0 0 0
assignedCurve: 0 1 2 3 4 4 4 4 4 4
expoLow: 0 0 0 0 0 0 0 0 0 0
</DR_Expo>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("drHigh", 5)]
        [TestCase("drLow", 5)]
        [TestCase("expoHigh", 5)]
        [TestCase("expoLow", 5)]
        public void DX9To_DR_Expo(string arrayName, int length)
        {
            var dx9Spm =
@"<DR_Expo>
drHigh: 100 100 100 100 100 0 0 0 0 0
drLow: 100 100 100 100 100 0 0 0 0 0
expoHigh: 0 0 0 0 0 0 0 0 0 0
expoLow: 0 0 0 0 0 0 0 0 0 0
</DR_Expo>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node1 = navigator.SelectSingleNode(string.Format("/SPM/DR_Expo/{0}/Element[{1}]", arrayName, length));
            Assert.That(node1, Is.Not.Null);
            var node6 = navigator.SelectSingleNode(string.Format("/SPM/DR_Expo/{0}/Element[{1}]", arrayName, length + 1));
            Assert.That(node6, Is.Null);
        }

        [TestCase("0", "0")]     // Inhibit
        [TestCase("65", "17")]   // Ail. Stick
        [TestCase("66", "18")]   // Ele. Stick
        [TestCase("67", "19")]   // Rud. Stick
        public void DX9To_DR_Expo_analogID(string sourceID, string expectedSourceID)
        {
            var dx9Spm = string.Format(
@"<DR_Expo>
analogID= {0}
</DR_Expo>", sourceID);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/DR_Expo/analogID");
            Assert.That(node.Value, Is.EqualTo(expectedSourceID));
        }

        // 0, 42, 43, 45, 46, 63, 127, 
        [TestCase("0", "0")]     // Inhibit
        [TestCase("84", "42")]   // Elev D/R->Switch C
        [TestCase("85", "43")]   // Flap->Switch D
        [TestCase("87", "45")]   // Ail D/R->Switch F
        [TestCase("88", "46")]   // Rud D/R->Switch G
        [TestCase("107", "63")]  // On
        [TestCase("145", "127")] // Flight Mode 
        public void DX9To_DR_Expo_conditionID(string sourceID, string expectedSourceID)
        {
            var dx9Spm = string.Format(
@"<DR_Expo>
conditionID= {0}
</DR_Expo>", sourceID);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/DR_Expo/conditionID");
            Assert.That(node.Value, Is.EqualTo(expectedSourceID));
        }

        [TestCase("/SPM/P-Mix/Index", true)]
        [TestCase("/SPM/P-Mix/analogID", true)]
        [TestCase("/SPM/P-Mix/conditionID", true)]
        [TestCase("/SPM/P-Mix/trimID", true)]
        [TestCase("/SPM/P-Mix/activePositions", true)]
        [TestCase("/SPM/P-Mix/outChan", true)]
        [TestCase("/SPM/P-Mix/Curvedata", true)]
        public void DX9To_P_Mix(string element, bool keep)
        {
            var dx9Spm =
@"<P-Mix>
*Index= 0
analogID= 0
conditionID= 0
trimID= 0
activePositions=%0000
outChan= 0

[Curvedata]
*Index= 0
points= 3
Expo=Disabled
trimActive=Disabled
X: -1023 0 1023 0 0 0 0
Y: 0 0 0 0 0 0 0
[/Curvedata]
</P-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        // 0, 6, 7, 8, 16, 21, 32, 33, 96, 97, 98, 
        [TestCase("0", "0")]     // Inhibit
        [TestCase("6", "6")]     // AX1
        [TestCase("7", "7")]     // AX2
        [TestCase("8", "8")]     // AX3
        [TestCase("64", "16")]   // Thr. Stick
        [TestCase("69", "21")]   // R Knob
        [TestCase("78", "32")]   // FLP
        [TestCase("79", "33")]   // GER
        [TestCase("128", "96")]  // AIL
        [TestCase("129", "97")]  // ELE
        [TestCase("130", "98")]  // RUD
        public void DX9To_PMix_analogID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<P-Mix>
*Index= 0
analogID= {0}
</P-Mix>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/P-Mix/analogID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        [TestCase("Normal", "9", "9")]       // AX4
        [TestCase("Dual_Ele", "9", "7")]     // AX4->AX2
        [TestCase("Dual_Rud_Ele", "9", "7")] // AX4->AX2
        public void DX9To_PMix_Tail_analogID(string tail, string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<Acro>
Tail={0}
</Acro>

<P-Mix>
*Index= 0
analogID= {1}
</P-Mix>", tail, id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogIDNode = navigator.SelectSingleNode("/SPM/P-Mix/analogID");
            Assert.That(analogIDNode.Value, Is.EqualTo(expectedID));
        }

        [TestCase("Normal", "38", "198")]       // AX2
        [TestCase("Dual_Ele", "40", "198")]     // LEL
        [TestCase("Dual_Rud_Ele", "40", "198")] // LEL
        public void DX9To_PMix_Tail_outChan(string tail, string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<Acro>
Tail={0}
</Acro>

<P-Mix>
*Index= 0
outChan= {1}
</P-Mix>", tail, id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var outChanNode = navigator.SelectSingleNode("/SPM/P-Mix/outChan");
            Assert.That(outChanNode.Value, Is.EqualTo(expectedID));
        }

        // 0, 40, 42, 45, 46, 47, 63, 127, 
        [TestCase("0", "0")]     // Inhibit
        [TestCase("82", "40")]   // Gear->Switch A
        [TestCase("84", "42")]   // Elev D/R->Switch C 
        [TestCase("87", "45")]   // Ail D/R->Switch F
        [TestCase("88", "46")]   // Rud D/R->Switch G
        [TestCase("89", "47")]   // Mix/Hold->Switch H
        [TestCase("107", "63")]  // On
        [TestCase("145", "127")] // Flight Mode
        public void DX9To_PMix_conditionID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<P-Mix>
*Index= 0
conditionID= {0}
</P-Mix>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/P-Mix/conditionID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        // 0, 192, 193, 194, 195, 196, 197, 198, 199, 
        [TestCase("0", "0")]     // Inhibit
        [TestCase("32", "192")]  // THR
        [TestCase("33", "193")]  // AIL 
        [TestCase("34", "194")]  // ELE 
        [TestCase("35", "195")]  // RUD 
        [TestCase("36", "196")]  // GER 
        [TestCase("37", "197")]  // AX1 
        [TestCase("38", "198")]  // AX2 
        [TestCase("39", "199")]  // AX3/FLP/LFL/GER
        public void DX9To_PMix_outChan(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<P-Mix>
*Index= 0
outChan= {0}
</P-Mix>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/P-Mix/outChan");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        // 0, 65, 66, 67, 68, 
        [TestCase("0", "0")]     // Inhibit
        [TestCase("109", "65")]  // AIL Trim
        [TestCase("110", "66")]  // ELE Trim
        [TestCase("111", "67")]  // RUD Trim
        [TestCase("112", "68")]  // LTrimD
        [TestCase("113", "69")]  // RTrimD
        public void DX9To_PMix_trimID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<P-Mix>
*Index= 0
trimID= {0}
</P-Mix>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/P-Mix/trimID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        // TODO:
        //198->40  - LEL when DX8/DX7s Dual_Rud_Ele or Dual_Ele
        //198->39  - LRU when DX7s Dual_Rud

        [TestCase("/SPM/FMode/switch_a", true)]
        [TestCase("/SPM/FMode/switch_b", true)]
        [TestCase("/SPM/FMode/switch_c", true)]
        [TestCase("/SPM/FMode/size", true)]
        [TestCase("/SPM/FMode/fmtable", false)]
        [TestCase("/SPM/FMode/data", true)]
        [TestCase("/SPM/FMode/activePositions", false)]
        public void DX9To_FMode(string element, bool keep)
        {
            var dx9Spm =
@"<FMode>switch_a= 83switch_b= 84switch_c= 0size= 18fmtable: 0 0 0 0 0 0 1 3 4 0 0 0 2 2 2 0 0 0activePositions=%0002</FMode>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        // NOTE: switch_c isn't used on DX8.
        [Test]
        public void DX9To_FMode()
        {
            var dx9Spm =
@"<FMode>switch_a= 83switch_b= 84switch_c= 0size= 18fmtable: 0 0 0 0 0 0 1 3 4 0 0 0 2 2 2 0 0 0</FMode>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var size = navigator.SelectSingleNode("/SPM/FMode/size").Value;
            Assert.That(size, Is.EqualTo("9"));
            var switch_a = navigator.SelectSingleNode("/SPM/FMode/switch_a").Value;
            Assert.That(switch_a, Is.EqualTo("41")); // F Mode->Switch B
            var switch_b = navigator.SelectSingleNode("/SPM/FMode/switch_b").Value;
            Assert.That(switch_b, Is.EqualTo("42")); // Elev D/R->Switch C
            StringAssert.Contains("data: 0 0 0 1 3 4 2 2 2", dx8Spm);
        }

        // 0, 1, 5, 6, 7, 8, 70, 242, 244,
        [TestCase("0", "0")]     // Inhibit
        [TestCase("1", "1")]     // THR Servo Out
        [TestCase("5", "5")]     // GER Servo Out
        [TestCase("6", "6")]     // AX1 Servo Out
        [TestCase("7", "7")]     // AX2 Servo Out
        [TestCase("8", "8")]     // AX3 Servo Out
        // [TestCase("", "70")]  // FlpTrm (not supported on DX9?)
        // [TestCase("", "242")] // Flaps (not supported on DX9?)
        [TestCase("200", "244")] // Gyro
        public void DX9To_SoftSw_sourceID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<SoftSw>
*Index= 0
sourceID= {0}
</SoftSw>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/SoftSw/sourceID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        // 40, 43
        [TestCase("82", "40")]   // Gear->Switch A
        [TestCase("85", "43")]   // Flap->Switch D
        public void DX9To_Special_sourceID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<Special>
*Index= 0
sourceID= {0}
</Special>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Special/sourceID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        // 0, 21, 69
        [TestCase("0", "0")]    // Inhibit
        [TestCase("69", "21")]  // R Knob
        [TestCase("113", "69")] // RTrimD
        public void DX9To_TrimID_sourceID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<TrimID>
*Index= 0
sourceID= {0}
</TrimID>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/TrimID/sourceID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        [TestCase("/SPM/Trim/Index", true)]
        [TestCase("/SPM/Trim/Pos0", true)]
        [TestCase("/SPM/Trim/Pos1", true)]
        [TestCase("/SPM/Trim/Pos2", true)]
        [TestCase("/SPM/Trim/trimClicks", true)]
        [TestCase("/SPM/Trim/maxTrimClicks", true)]
        [TestCase("/SPM/Trim/trimStepSize", true)]
        [TestCase("/SPM/Trim/trimRepeat", true)]
        [TestCase("/SPM/Trim/trimNextRepeat", true)]
        public void DX9To_Trim(string element, bool keep)
        {
            var dx9Spm =
@"<Trim>
*Index= 0
Pos0= 0
Pos1= 767
Pos2= -767
trimClicks: 0 0 0 0 0 0 0 0 0 0
maxTrimClicks= 50
trimStepSize= 6
trimRepeat= 1
trimNextRepeat= 21
</Trim>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("trimClicks", 5)]
        public void DX9To_Trim(string arrayName, int length)
        {
            var dx9Spm =
@"<Trim>
*Index= 0
trimClicks: 0 0 0 0 0 0 0 0 0 0
</Trim>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var nodeA = navigator.SelectSingleNode(string.Format("/SPM/Trim/{0}/Element[{1}]", arrayName, length));
            Assert.That(nodeA, Is.Not.Null);
            var nodeB = navigator.SelectSingleNode(string.Format("/SPM/Trim/{0}/Element[{1}]", arrayName, length + 1));
            Assert.That(nodeB, Is.Null);
        }

        [TestCase("/SPM/ThroCut/conditionID", true)]
        [TestCase("/SPM/ThroCut/percent", true)]
        [TestCase("/SPM/ThroCut/rampSpeed", true)]
        [TestCase("/SPM/ThroCut/activePositions", true)]
        public void DX9To_ThroCut(string element, bool keep)
        {
            var dx9Spm =
@"<ThroCut>
conditionID= 0
percent= -6
rampSpeed= 32736
activePositions=%FFFE
</ThroCut>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        // 0, 40, 47, 50, 69
        [TestCase("0", "0")]    // Inhibit
        [TestCase("82", "40")]  // Gear->Switch A
        [TestCase("89", "47")]  // Mix/Hold->Switch H
        [TestCase("92", "50")]  // Trainer->Switch I
        [TestCase("113", "69")] // RTrimD
        public void DX9To_ThroCut_conditionID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<ThroCut>
conditionID= {0}
</ThroCut>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/ThroCut/conditionID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        [TestCase("/SPM/ThroCurve/analogID", true)]
        [TestCase("/SPM/ThroCurve/conditionID", true)]
        [TestCase("/SPM/ThroCurve/trimID", true)]
        [TestCase("/SPM/ThroCurve/activeMask", true)]
        [TestCase("/SPM/ThroCurve/delay", true)]
        [TestCase("/SPM/ThroCurve/mixName", false)]
        [TestCase("/SPM/ThroCurve/assignedCurve", true)]
        [TestCase("/SPM/ThroCurve/Curvedata", true)]
        [TestCase("/SPM/ThroCurve/Curvedata/Index", true)]
        [TestCase("/SPM/ThroCurve/Curvedata/points", true)]
        [TestCase("/SPM/ThroCurve/Curvedata/Expo", true)]
        [TestCase("/SPM/ThroCurve/Curvedata/trimActive", true)]
        [TestCase("/SPM/ThroCurve/Curvedata/curved", false)]
        [TestCase("/SPM/ThroCurve/Curvedata/X", true)]
        [TestCase("/SPM/ThroCurve/Curvedata/Y", true)]
        public void DX9To_ThroCurve(string element, bool keep)
        {
            var dx9Spm =
@"<ThroCurve>
analogID= 64
conditionID= 0
trimID= 108
activeMask=%0000
delay= 0
mixName=""Throttle""
assignedCurve: 0 1 2 3 1 0 0 0 0 0

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
curved=Enabled
X: -1023 -511 0 511 1023 0 0
Y: -1023 -511 0 511 1023 0 0
[/Curvedata]
</ThroCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [Test]
        public void DX9To_ThroCurve()
        {
            var dx9Spm =
@"<ThroCurve>
analogID= 64
conditionID= 145
trimID= 108
</ThroCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var analogID = navigator.SelectSingleNode("/SPM/ThroCurve/analogID").Value;
            Assert.That(analogID, Is.EqualTo("16")); // Thr. Stick
            var conditionID = navigator.SelectSingleNode("/SPM/ThroCurve/conditionID").Value;
            Assert.That(conditionID, Is.EqualTo("127")); // Flight Mode
            var trimID = navigator.SelectSingleNode("/SPM/ThroCurve/trimID").Value;
            Assert.That(trimID, Is.EqualTo("64")); // THR Trim
        }

        [TestCase("assignedCurve", 5)]
        public void DX9To_ThroCurve(string arrayName, int length)
        {
            var dx9Spm =
@"<ThroCurve>
assignedCurve: 0 1 2 3 1 0 0 0 0 0
</ThroCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var nodeA = navigator.SelectSingleNode(string.Format("/SPM/ThroCurve/{0}/Element[{1}]", arrayName, length));
            Assert.That(nodeA, Is.Not.Null);
            var nodeB = navigator.SelectSingleNode(string.Format("/SPM/ThroCurve/{0}/Element[{1}]", arrayName, length + 1));
            Assert.That(nodeB, Is.Null);
        }

        // TODO: Check other fields
        [Test]
        public void DX9To_EF_Mix()
        {
            var dx9Spm =
@"<EF-Mix>
analogID= 129
outChan= 52
mixName=""ELE > FLP""
</EF-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var analogID = navigator.SelectSingleNode("/SPM/EF-Mix/analogID").Value;
            Assert.That(analogID, Is.EqualTo("97")); // 97-> 129 - ELE
            var outChan = navigator.SelectSingleNode("/SPM/EF-Mix/outChan").Value;
            Assert.That(outChan, Is.EqualTo("200")); // 200->52  - FLP/LFL
            var mixNameNode = navigator.SelectSingleNode("/SPM/EF-Mix/mixName");
            Assert.That(mixNameNode, Is.Null);
        }

        // TODO: Check other fields
        [Test]
        public void DX9To_AR_Mix()
        {
            var dx9Spm =
@"<AR-Mix>
analogID= 128
outChan= 35
mixName=""AIL > RUD""
</AR-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var analogID = navigator.SelectSingleNode("/SPM/AR-Mix/analogID").Value;
            Assert.That(analogID, Is.EqualTo("96")); // 96-> 128 - AIL
            var outChan = navigator.SelectSingleNode("/SPM/AR-Mix/outChan").Value;
            Assert.That(outChan, Is.EqualTo("195")); // 195->35  - RUD
            var mixNameNode = navigator.SelectSingleNode("/SPM/AR-Mix/mixName");
            Assert.That(mixNameNode, Is.Null);
        }

        [Test]
        public void DX9To_AR_Mix_Invert()
        {
            var dx9Spm =
@"<AR-Mix>
[Curvedata]
X: -1023 0 1023 0 0 0 0
Y: -1023 0 1023 0 0 0 0
[/Curvedata]
</AR-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var x1 = navigator.SelectSingleNode("/SPM/AR-Mix/Curvedata/X/Element[1]").Value;
            Assert.That(x1, Is.EqualTo("-1023"), "Check X not inverted");
            var x3 = navigator.SelectSingleNode("/SPM/AR-Mix/Curvedata/X/Element[3]").Value;
            Assert.That(x3, Is.EqualTo("1023"), "Check X not inverted");

            var y1 = navigator.SelectSingleNode("/SPM/AR-Mix/Curvedata/Y/Element[1]").Value;
            Assert.That(y1, Is.EqualTo("1023"), "Check Y is inverted");
            var y3 = navigator.SelectSingleNode("/SPM/AR-Mix/Curvedata/Y/Element[3]").Value;
            Assert.That(y3, Is.EqualTo("-1023"), "Check Y is inverted");
        }

        [TestCase("/SPM/FlapSystem/analogID", true)]
        [TestCase("/SPM/FlapSystem/conditionID", true)]
        [TestCase("/SPM/FlapSystem/trimID", true)]
        [TestCase("/SPM/FlapSystem/speed", true)]
        [TestCase("/SPM/FlapSystem/flapTarget", true)]
        [TestCase("/SPM/FlapSystem/elevatorTarget", true)]
        [TestCase("/SPM/FlapSystem/Curvedata", true)]
        public void DX9To_FlapSystem(string element, bool keep)
        {
            var dx9Spm =
@"<FlapSystem>
analogID= 0
conditionID= 0
trimID= 0
speed= 32736
flapTarget: 0 0 0 0 0
elevatorTarget: 0 0 0 0 0

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
X: -1023 -511 0 511 1023 0 0
Y: -1023 -511 0 511 1023 0 0
[/Curvedata]
</FlapSystem>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Differential/conditionID", true)]
        [TestCase("/SPM/Differential/rate", true)]
        public void DX9To_Differential(string element, bool keep)
        {
            var dx9Spm =
@"<Differential>
conditionID= 0
rate: 0 0 0 0 0
</Differential>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("%00000000", "Common")] // No FMode trims.
        [TestCase("%0000003F", "FMode")]  // This is what DX8->DX9 converts to
        [TestCase("%00000001", "FMode")]  // Default to FMode?
        public void DX9To_Config_TrimType(string dx9TrimType, string expectedTrimType)
        {
            var dx9Spm = string.Format(
@"<Config>
TrimType={0}
</Config>", dx9TrimType);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var trimType = navigator.SelectSingleNode("/SPM/Config/TrimType").Value;
            Assert.That(trimType, Is.EqualTo(expectedTrimType));
        }

        [TestCase("/SPM/Config/FrameRate", false)]
        [TestCase("/SPM/Config/FrameRateX", false)]
        [TestCase("/SPM/Config/TrimType", true)]
        [TestCase("/SPM/Config/trimMode", true)]
        [TestCase("/SPM/Config/thrTrimRev", false)]
        public void DX9To_Config(string element, bool keep)
        {
            var dx9Spm =
@"<Config>
;FrameRate=ms22
FrameRateX=%0001
TrimType=%00000000
trimMode=Normal
thrTrimRev=%00
</Config>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("1", "1")]    // THR Servo Out
        [TestCase("64", "16")]  // Thr. Stick
        [TestCase("82", "40")]  // Gear->Switch A
        [TestCase("92", "50")]  // Trainer->Switch I
        [TestCase("112", "68")] // LTrimD
        [TestCase("113", "69")] // RTrimD
        public void DX9To_Timer_StartID(string id, string expectedID)
        {
            var dx9Spm = string.Format(
@"<Timer>
*Index= 0
StartID= {0}
</Timer>", id);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Timer/StartID");
            Assert.That(node.Value, Is.EqualTo(expectedID));
        }

        [TestCase("/SPM/Timer/Index", true)]
        [TestCase("/SPM/Timer/Mode", true)]
        [TestCase("/SPM/Timer/Minutes", true)]
        [TestCase("/SPM/Timer/Seconds", true)]
        [TestCase("/SPM/Timer/oneTime", true)]
        [TestCase("/SPM/Timer/audioX", false)]
        [TestCase("/SPM/Timer/vibeX", false)]
        [TestCase("/SPM/Timer/voiceX", false)]
        [TestCase("/SPM/Timer/voxEv1", false)]
        [TestCase("/SPM/Timer/StartID", true)]
        [TestCase("/SPM/Timer/Event", true)]
        [TestCase("/SPM/Timer/Thresh", true)]
        [TestCase("/SPM/Timer/activePositions", false)] // Not compatible with DX8. Spotted in DX7S 1.03
        [TestCase("/SPM/Timer/Audio", true)]
        [TestCase("/SPM/Timer/Vibrate", true)]
        public void DX9To_Timer(string element, bool keep)
        {
            var dx9Spm =
@"<Timer>
*Index= 0
Mode=Down
Minutes= 5
Seconds= 0

oneTime=Disabled
audioX=%00F4
vibeX=%0000
audioX=%00F4
voxEv1=%0055
StartID= 64
Event= 2
Thresh= -511
activePositions=%00FE
</Timer>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Telemetry/lcdMode", true)]
        [TestCase("/SPM/Telemetry/Screens", true)]
        [TestCase("/SPM/Telemetry/StartID", true)]
        [TestCase("/SPM/Telemetry/Event", true)]
        [TestCase("/SPM/Telemetry/Thresh", true)]
        [TestCase("/SPM/Telemetry/oneTime", true)]
        // Too many changes in DX9 FlightLog|Module to support.
        [TestCase("/SPM/Telemetry/FlightLog", false)]
        [TestCase("/SPM/Telemetry/Module", false)]
        //[TestCase("/SPM/Telemetry/FlightLog", true)]
        //[TestCase("/SPM/Telemetry/FlightLog/__ALL__", true)]
        //[TestCase("/SPM/Telemetry/FlightLog/sdEnabled", false)]
        //[TestCase("/SPM/Telemetry/Module", true)]
        //[TestCase("/SPM/Telemetry/Module/Device", true)]
        //[TestCase("/SPM/Telemetry/Module/__ALL__", true)]
        public void DX9To_Telemetry(string element, bool keep)
        {
            var dx9Spm =
@"<Telemetry>
lcdMode= 2
Screens=%FFFF
StartID= 0
Event= 4
Thresh= -511
oneTime=Disabled

[FlightLog]
__ALL__= 0
sdEnabled= 1
[/FlightLog]

[Module]
*Device=Tachometer
__ALL__= 0
[/Module]
</Telemetry>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [Test]
        public void DX9To_Trainer_Active()
        {
            var dx9Spm =
@"<Trainer>
mixOrNormal=%0000
mixRatio:  0  100  0  100  0  100  0  100
</Trainer>";

            /*
            <Trainer>
            Active: Disabled Enabled Disabled Enabled Disabled Enabled Disabled Enabled
            </Trainer>
             */

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var mixOrNormalNode = navigator.SelectSingleNode("/SPM/Trainer/mixOrNormal");
            Assert.That(mixOrNormalNode, Is.Null, "Check 'mixOrNormal' has been removed");
            var mixRatioNode = navigator.SelectSingleNode("/SPM/Trainer/mixRatio");
            Assert.That(mixRatioNode, Is.Null, "Check 'mixRatio' has been removed");
            var activeNode = navigator.SelectSingleNode("/SPM/Trainer/Active");
            Assert.That(activeNode, Is.Not.Null, "Check 'Active' has created");
            StringAssert.Contains("Active: Disabled Enabled Disabled Enabled Disabled Enabled Disabled Enabled", dx8Spm);
        }

        [TestCase("/SPM/Trainer/Type", "Disabled")]
        [TestCase("/SPM/Trainer/conditionID", null)]
        [TestCase("/SPM/Trainer/MOverride", null)]
        [TestCase("/SPM/Trainer/activePositions", null)]
        [TestCase("/SPM/Trainer/mixOrNormal", null)]
        [TestCase("/SPM/Trainer/mixRatio", null)]
        public void DX9To_Trainer_Type_Disabled(string element, string expectedValue)
        {
            var dx9Spm =
@"<Trainer>
Type=Disabled
conditionID= 92
MOverride=Disabled
activePositions= 254
</Trainer>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (expectedValue != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(expectedValue));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("%00F4", "Enabled")]
        [TestCase("%0080", "Disabled")]
        [TestCase("%0000", "Disabled")]
        [TestCase("%00FF", "Enabled")] // default
        [TestCase("%0001", "Enabled")] // default
        public void DX9To_Timer_audioX(string audioX, string expectedAudio)
        {
            var dx9Spm = string.Format(
@"<Timer>*Index= 0audioX={0}</Timer>", audioX);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Timer/Audio");
            Assert.That(node.Value, Is.EqualTo(expectedAudio), "Check that 'audioX' has been converted to 'Audio'");
        }

        [TestCase("%0020", "Enabled")]
        [TestCase("%0000", "Disabled")]
        [TestCase("%00FF", "Enabled")] // default
        [TestCase("%0001", "Enabled")] // default
        public void DX9To_Timer_vibeX(string vibeX, string expectedVibrate)
        {
            var dx9Spm = string.Format(
@"<Timer>*Index= 0vibeX={0}</Timer>", vibeX);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode("/SPM/Timer/Vibrate");
            Assert.That(node.Value, Is.EqualTo(expectedVibrate), "Check that 'vibeX' has been converted to 'Vibrate'");
        }

        [TestCase("/SPM/Warning/Vibrate", true)]
        [TestCase("/SPM/Warning/Throttle", true)]
        [TestCase("/SPM/Warning/Thresh", true)]
        [TestCase("/SPM/Warning/Gear", true)]
        [TestCase("/SPM/Warning/FltMode", true)]
        [TestCase("/SPM/Warning/Flaps", true)]
        [TestCase("/SPM/Warning/chanAchan", false)]
        [TestCase("/SPM/Warning/chanAtype", false)]
        [TestCase("/SPM/Warning/chanBchan", false)]
        [TestCase("/SPM/Warning/chanBtype", false)]
        public void DX9To_Warning(string element, bool keep)
        {
            var dx9Spm =
@"<Warning>
Vibrate=Enabled
Throttle=Over
Thresh= -819
FltMode=%0000
Gear=%0000
Flaps=%0000
chanAchan= 0
chanAtype=Disabled
chanBchan= 0
chanBtype=Disabled
</Warning>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("%0000", "%0000")] // Flaps: Inhibit
        [TestCase("%0002", "%0001")] // Flaps: Mid
        [TestCase("%0004", "%0002")] // Flaps: Land
        [TestCase("%0006", "%0003")] // Flaps: Mid+Land
        [TestCase("%0005", "%0004")] // Flaps: Norm+Land
        public void DX9To_Warning_Flaps(string flaps, string expectFlaps)
        {
            var dx9Spm =
string.Format(@"<Warning>Flaps={0}</Warning>", flaps);
            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var flapsNode = navigator.SelectSingleNode("/SPM/Warning/Flaps");
            Assert.That(flapsNode.Value, Is.EqualTo(expectFlaps), "Check Flaps");
        }

        [TestCase("%0000", "%0000")] // Gear: Inhibit
        [TestCase("%0001", "%0001")] // Gear: Pos 0
        [TestCase("%0002", "%0002")] // Gear: Pos 1
        public void DX9To_Warning_Gear(string gear, string expectGear)
        {
            var dx9Spm =
string.Format(@"<Warning>Gear={0}</Warning>", gear);
            var dx8Spm = SpmConvert.DX8To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var gearNode = navigator.SelectSingleNode("/SPM/Warning/Gear");
            Assert.That(gearNode.Value, Is.EqualTo(expectGear), "Check Gear");
        }

        //DX8 FltMode Stunt1:%20 Stunt2:%40
        //DX9 FltMode Stunt1:%04 Stunt2:%08 Stunt3:%10
        [TestCase("%0000", "%0000")] // Stunt1:Act
        [TestCase("%0004", "%0020")] // Stunt1:Act
        [TestCase("%0008", "%0040")] // Stunt2:Act
        [TestCase("%000C", "%0060")] // Stunt1:Act,Stunt2:Act
        [TestCase("%0010", "%0000")] // Stunt1:Act,Stunt3:Act
        [TestCase("%0014", "%0020")] // Stunt1:Act,Stunt3:Act
        [TestCase("%0018", "%0040")] // Stunt2:Act,Stunt3:Act
        [TestCase("%001C", "%0060")] // Stunt1:Act,Stunt2:Act,Stunt3:Act
        public void DX9To_Warning_Acro_FltMode_Hold(string fltMode, string expectFltMode)
        {
            var dx9Spm =
string.Format(@"<Spektrum>Type=Acro</Spektrum><Acro></Acro><Warning>FltMode={0}</Warning>", fltMode);
            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var fltModeNode = navigator.SelectSingleNode("/SPM/Warning/FltMode");
            Assert.That(fltModeNode.Value, Is.EqualTo(expectFltMode), "Check FltMode");
        }

        // ============ HELI =============

        [TestCase("/SPM/Heli/Swash", true)]
        public void DX9To_Heli(string element, bool keep)
        {
            var dx9Spm =
@"<Heli>Swash=Swash_1_Normal</Heli>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [Test]
        public void DX9To_Heli_FMode()
        {
            var dx9Spm =
@"<Spektrum>Type=Heli</Spektrum><Heli>Swash=Swash_1_Normal</Heli><FMode>switch_a= 83switch_b= 0switch_c= 89size= 18</FMode>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            var size = navigator.SelectSingleNode("/SPM/FMode/size").Value;
            Assert.That(size, Is.EqualTo("9"));
            var switch_a = navigator.SelectSingleNode("/SPM/FMode/switch_a").Value;
            Assert.That(switch_a, Is.EqualTo("41")); // F Mode->Switch B
            var switch_b = navigator.SelectSingleNode("/SPM/FMode/switch_b").Value;
            Assert.That(switch_b, Is.EqualTo("47")); // Hold
        }

        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0000", "1 1 1 2 2 2 3 3 3")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0001", "0 1 1 0 2 2 0 3 3")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0002", "1 0 1 2 0 2 3 0 3")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0003", "0 0 1 0 0 2 0 0 3")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0004", "1 1 0 2 2 0 3 3 0")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0005", "0 1 0 0 2 0 0 3 0")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0006", "1 0 0 2 0 0 3 0 0")]
        [TestCase("1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4", "%0007", "0 0 0 0 0 0 0 0 0")]
        [TestCase("3 0 0 0 0 0 2 0 0 0 0 0 1 0 0 0 0 0", "%0000", "3 3 3 2 2 2 1 1 1")]
        public void DX9To_Heli_FMode_data(string fmtable, string activePositions, string expectedData)
        {
            var dx9Spm = string.Format(
@"<Spektrum>Type=Heli</Spektrum><FMode>fmtable: {0}activePositions={1}</FMode>", fmtable, activePositions);

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();

            StringAssert.Contains("data: " + expectedData, dx8Spm);
        }

        // TODO: Pitch curve on DX9 Acro?

        [TestCase("/SPM/PitchCurve/analogID", true)]
        [TestCase("/SPM/PitchCurve/conditionID", true)]
        [TestCase("/SPM/PitchCurve/trimID", true)]
        [TestCase("/SPM/PitchCurve/activeMask", true)]
        [TestCase("/SPM/PitchCurve/delay", true)]
        [TestCase("/SPM/PitchCurve/mixName", false)]
        [TestCase("/SPM/PitchCurve/assignedCurve", true)]
        [TestCase("/SPM/PitchCurve/Curvedata", true)]
        [TestCase("/SPM/PitchCurve/Curvedata/Index", true)]
        [TestCase("/SPM/PitchCurve/Curvedata/points", true)]
        [TestCase("/SPM/PitchCurve/Curvedata/Expo", true)]
        [TestCase("/SPM/PitchCurve/Curvedata/trimActive", true)]
        [TestCase("/SPM/PitchCurve/Curvedata/curved", false)]
        [TestCase("/SPM/PitchCurve/Curvedata/X", true)]
        [TestCase("/SPM/PitchCurve/Curvedata/Y", true)]
        public void DX9To_PitchCurve(string element, bool keep)
        {
            var dx9Spm =
@"<PitchCurve>
analogID= 64
conditionID= 145
trimID= 0
activeMask=%0000
delay= 0
mixName=""Pitch""
assignedCurve: 0 1 2 3 4 0 0 0 0 0

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
curved=Enabled
X: -1023 -511 0 511 1023 0 0
Y: -1023 -511 0 511 1023 0 0
[/Curvedata]
</PitchCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("assignedCurve", 5)]
        public void DX9To_PitchCurve(string arrayName, int length)
        {
            var dx9Spm =
@"<PitchCurve>
assignedCurve: 0 1 2 3 4 0 0 0 0 0
</PitchCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var nodeA = navigator.SelectSingleNode(string.Format("/SPM/PitchCurve/{0}/Element[{1}]", arrayName, length));
            Assert.That(nodeA, Is.Not.Null);
            var nodeB = navigator.SelectSingleNode(string.Format("/SPM/PitchCurve/{0}/Element[{1}]", arrayName, length + 1));
            Assert.That(nodeB, Is.Null);
        }

        [TestCase("/SPM/RevoCurve/analogID", true)]
        [TestCase("/SPM/RevoCurve/conditionID", true)]
        [TestCase("/SPM/RevoCurve/trimID", true)]
        [TestCase("/SPM/RevoCurve/activeMask", true)]
        [TestCase("/SPM/RevoCurve/delay", true)]
        [TestCase("/SPM/RevoCurve/mixName", false)]
        [TestCase("/SPM/RevoCurve/assignedCurve", true)]
        [TestCase("/SPM/RevoCurve/Curvedata", true)]
        [TestCase("/SPM/RevoCurve/Curvedata/Index", true)]
        [TestCase("/SPM/RevoCurve/Curvedata/points", true)]
        [TestCase("/SPM/RevoCurve/Curvedata/Expo", true)]
        [TestCase("/SPM/RevoCurve/Curvedata/trimActive", true)]
        [TestCase("/SPM/RevoCurve/Curvedata/curved", false)]
        [TestCase("/SPM/RevoCurve/Curvedata/X", true)]
        [TestCase("/SPM/RevoCurve/Curvedata/Y", true)]
        public void DX9To_RevoCurve(string element, bool keep)
        {
            var dx9Spm =
@"<RevoCurve>
analogID= 64
conditionID= 0
trimID= 0
activeMask=%0000
delay= 100
mixName=""Tail""
assignedCurve: 0 1 2 3 4 0 0 0 0 0

[Curvedata]
*Index= 0
points= 5
Expo=Disabled
trimActive=Disabled
curved=Enabled
X: -1023 -511 0 511 1023 0 0
Y: 0 0 0 0 0 0 0
[/Curvedata]
</RevoCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("assignedCurve", 5)]
        public void DX9To_RevoCurve(string arrayName, int length)
        {
            var dx9Spm =
@"<RevoCurve>
assignedCurve: 0 1 2 3 4 0 0 0 0 0
</RevoCurve>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var nodeA = navigator.SelectSingleNode(string.Format("/SPM/RevoCurve/{0}/Element[{1}]", arrayName, length));
            Assert.That(nodeA, Is.Not.Null);
            var nodeB = navigator.SelectSingleNode(string.Format("/SPM/RevoCurve/{0}/Element[{1}]", arrayName, length + 1));
            Assert.That(nodeB, Is.Null);
        }

        [TestCase("/SPM/Gyro/sourceID", true)]
        [TestCase("/SPM/Gyro/conditionID", true)]
        [TestCase("/SPM/Gyro/trimID", true)]
        [TestCase("/SPM/Gyro/fpct", true)]
        [TestCase("/SPM/Gyro/tailHold", true)]
        [TestCase("/SPM/Gyro/outChan", true)]
        public void DX9To_Gyro(string element, bool keep)
        {
            var dx9Spm =
@"<Gyro>
sourceID= 0
conditionID= 0
trimID= 0
fpct: 0 0 0 0 0
tailHold=Disabled
outChan= 5
</Gyro>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Governor/sourceID", true)]
        [TestCase("/SPM/Governor/conditionID", true)]
        [TestCase("/SPM/Governor/trimID", true)]
        [TestCase("/SPM/Governor/fpct", true)]
        [TestCase("/SPM/Governor/outChan", true)]
        public void DX9To_Governor(string element, bool keep)
        {
            var dx9Spm =
@"<Governor>
sourceID= 0
conditionID= 0
trimID= 0
fpct: 0 0 0 0 0
outChan= 7
</Governor>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/C-Mix/rateHighAilThr", true)]
        [TestCase("/SPM/C-Mix/rateLowAilThr", true)]
        [TestCase("/SPM/C-Mix/rateHighEleThr", true)]
        [TestCase("/SPM/C-Mix/rateLowEleThr", true)]
        [TestCase("/SPM/C-Mix/rateHighRudThr", true)]
        [TestCase("/SPM/C-Mix/rateLowRudThr", true)]
        [TestCase("/SPM/C-Mix/conditionID", false)]
        [TestCase("/SPM/C-Mix/mixName", false)]
        [TestCase("/SPM/C-Mix/activePositions", true)]
        public void DX9To_C_Mix(string element, bool keep)
        {
            var dx9Spm =
@"<C-Mix>
rateHighAilThr= 0
rateLowAilThr= 0
rateHighEleThr= 0
rateLowEleThr= 0
rateHighRudThr= 0
rateLowRudThr= 0
conditionID= 0
mixName=""CYCLIC > THR""
activePositions=%0000
</C-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/S-Mix/rateHighAilEle", true)]
        [TestCase("/SPM/S-Mix/rateLowAilEle", true)]
        [TestCase("/SPM/S-Mix/rateHighEleAil", true)]
        [TestCase("/SPM/S-Mix/rateLowEleAil", true)]
        [TestCase("/SPM/S-Mix/conditionID", false)]
        [TestCase("/SPM/S-Mix/mixName", false)]
        [TestCase("/SPM/S-Mix/activePositions", true)]
        public void DX9To_S_Mix(string element, bool keep)
        {
            var dx9Spm =
@"<S-Mix>
rateHighAilEle= 0
rateLowAilEle= 0
rateHighEleAil= 0
rateLowEleAil= 0
conditionID= 0
mixName=""Swashplate""
activePositions=%0000
</S-Mix>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/SwashPlate/rateAileron", true)]
        [TestCase("/SPM/SwashPlate/rateElevator", true)]
        [TestCase("/SPM/SwashPlate/ratePitch", true)]
        [TestCase("/SPM/SwashPlate/E-Ring", true)]
        [TestCase("/SPM/SwashPlate/Expo", true)]
        [TestCase("/SPM/SwashPlate/rateExpo", true)]
        [TestCase("/SPM/SwashPlate/eleComp", false)]
        public void DX9To_SwashPlate(string element, bool keep)
        {
            var dx9Spm =
@"<SwashPlate>
rateAileron= 60
rateElevator= 60
ratePitch= 60
E-Ring=Disabled
Expo=Disabled
rateExpo= 30
eleComp= 0
</SwashPlate>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Warning/Vibrate", true)]
        [TestCase("/SPM/Warning/Throttle", true)]
        [TestCase("/SPM/Warning/Thresh", true)]
        [TestCase("/SPM/Warning/FltMode", true)]
        [TestCase("/SPM/Warning/Gear", false)]
        [TestCase("/SPM/Warning/Hold", false)]
        [TestCase("/SPM/Warning/Gyro", true)]
        [TestCase("/SPM/Warning/Governor", false)]
        [TestCase("/SPM/Warning/chanAchan", false)]
        [TestCase("/SPM/Warning/chanAtype", false)]
        [TestCase("/SPM/Warning/chanBchan", false)]
        [TestCase("/SPM/Warning/chanBtype", false)]
        public void DX9To_Warning_Heli(string element, bool keep)
        {
            var dx9Spm =
@"<Spektrum>Type=Heli</Spektrum><Heli>Swash=Swash_1_Normal</Heli>

<Warning>
Vibrate=Enabled
Throttle=Over
Thresh= -819
FltMode=%001C
Gear=%0000
Hold=%0002
Gyro=%0000
Governor=%0000
chanAchan= 0
chanAtype=Disabled
chanBchan= 0
chanBtype=Disabled
</Warning>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        //DX8 FltMode Stunt1:%20 Stunt2:%40 Hold(On):%80
        //DX9 FltMode Stunt1:%04 Stunt2:%08 Stunt3:%10 Hold Hold(Off):%0001 Hold(On):%0002
        [TestCase("%0000", "%0000", "%0000")] // Stunt1:Inh Stunt2:Inh Hold:Inh
        [TestCase("%0004", "%0000", "%0020")] // Stunt1:Act
        [TestCase("%0008", "%0000", "%0040")] // Stunt2:Act
        [TestCase("%000C", "%0000", "%0060")] // Stunt1:Act,Stunt2:Act
        [TestCase("%0010", "%0000", "%0000")] // Stunt3:Act
        [TestCase("%0014", "%0000", "%0020")] // Stunt1:Act,Stunt3:Act
        [TestCase("%0018", "%0000", "%0040")] // Stunt2:Act,Stunt3:Act
        [TestCase("%001C", "%0000", "%0060")] // Stunt1:Act,Stunt2:Act,Stunt3:Act
        [TestCase("%0000", "%0002", "%0080")] // Hold:Act
        [TestCase("%000C", "%0002", "%00E0")] // Stunt1:Act Stunt2:Act Hold:Act
        [TestCase("%0010", "%0002", "%0080")] // Stunt3:Act,Hold:Act
        [TestCase("%001C", "%0002", "%00E0")] // Stunt1:Act,Stunt2:Act,Stunt3:Act,Hold:Act
        // DX8 can only warn when Hold is active.
        [TestCase("%0000", "%0001", "%0000")] // Hold:0
        [TestCase("%0004", "%0001", "%0020")] // Stunt1:Act, Hold:0
        public void DX9To_Warning_Heli_FltMode_Hold(string fltMode, string hold, string expectFltMode)
        {
            var dx9Spm =
string.Format(@"<Spektrum>Type=Heli</Spektrum><Heli>Swash=Swash_1_Normal</Heli><Warning>FltMode={0}Hold={1}</Warning>", fltMode, hold);
            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var fltModeNode = navigator.SelectSingleNode("/SPM/Warning/FltMode");
            Assert.That(fltModeNode.Value, Is.EqualTo(expectFltMode), "Check FltMode");
        }

        // Sailplane support

        [TestCase("/SPM/Spektrum/Generator", true)]
        [TestCase("/SPM/Spektrum/VCode", false)]
        [TestCase("/SPM/Spektrum/Originator", false)]
        [TestCase("/SPM/Spektrum/mmNum", false)]
        [TestCase("/SPM/Spektrum/bCode", false)]
        [TestCase("/SPM/Spektrum/PosIndex", true)]
        [TestCase("/SPM/Spektrum/PosMaxSail", true)]
        [TestCase("/SPM/Spektrum/Type", true)]
        [TestCase("/SPM/Spektrum/curveIndex", true)]
        [TestCase("/SPM/Spektrum/enabXPLUS", false)]
        [TestCase("/SPM/Spektrum/Name", true)]
        public void DX9To_Spektrum_Sail(string element, bool keep)
        {
            var dx9Spm =
@"<Spektrum>Generator=""DX9""VCode="" 1.03""Originator=""HS309XBwg4v74CHkcIOIrSLaVDKKkj""mmNum=3bCode=0PosIndex= 5PosMaxSail= 10Type=SailcurveIndex= 7enabXPLUS=DisabledName=""Sail""</Spektrum>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/Sail/Wing", true)]
        [TestCase("/SPM/Sail/Tail", true)]
        [TestCase("/SPM/Sail/Motor", true)]
        public void DX9To_Sail(string element, bool keep)
        {
            var dx9Spm =
@"<Sail>
Wing=Standard
Tail=Normal
Motor=None
</Sail>";

            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(element);
            if (keep)
            {
                Assert.That(node, Is.Not.Null);
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

        [TestCase("/SPM/EF-Mix/conditionID", "127")]
        // NOTE: Is it OK to copy activePositions?
        [TestCase("/SPM/EF-Mix/activePositions", "%0002")]
        [TestCase("/SPM/EF-Mix/mixName", null)]
        [TestCase("/SPM/EF-Mix/efItem/Index", "0")]
        [TestCase("/SPM/EF-Mix/efItem/offset", "-51")]
        [TestCase("/SPM/EF-Mix/efItem/flapUp", "-40")]
        [TestCase("/SPM/EF-Mix/efItem/flapDown", "-30")]
        // NOTE: flonUp/flonDown don't appear in GUI
        [TestCase("/SPM/EF-Mix/efItem/flonUp", "-20")]
        [TestCase("/SPM/EF-Mix/efItem/flonDown", "-10")]
        [TestCase("/SPM/EF-Mix/efItem/tipLeft", null)]
        [TestCase("/SPM/EF-Mix/efItem/tipRight", null)]
        [TestCase("/SPM/EF-Mix/efItem/elevator", null)]
        [TestCase("/SPM/EF-Mix/efItem/speed", null)]
        [TestCase("/SPM/EF-Mix/efItem/analogID", null)]
        public void DX9To_EF_Mix_Sail(string name, string value)
        {
            var dx9Spm =
@"<EF-Mix>
conditionID= 145
activePositions=%0002
mixName=""ELE > FLP""

[efItem]
*Index= 0
offset= -51
flapLeft= -40
flapRight= -30
flonLeft= -20
flonRight= -10
tipLeft= 0
tipRight= 0
elevator= 0
speed= 32736
analogID= 0
[/efItem]
</EF-Mix>";

/*
<EF-Mix>
conditionID= 127

[efItem]
*Index= 0
offset= -51
flapUp= -40
flapDown= -30
flonUp= -20
flonDown= -10
[/efItem]
</EF-Mix>
*/
            var dx8Spm = SpmConvert.DX9To(dx9Spm);

            var xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            var doc = new XPathDocument(reader);
            var navigator = doc.CreateNavigator();
            var node = navigator.SelectSingleNode(name);
            if (value != null)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.Value, Is.EqualTo(value));
            }
            else
            {
                Assert.That(node, Is.Null);
            }
        }

    }
}
