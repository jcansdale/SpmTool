using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmTool.Tests
{
    public class SpmUtilitiesTests
    {
        [Test]
        public void IsAirplane()
        {
            string spmText =
@"<Acro>Wing=StandardTail=Normal</Acro>";

            bool isAirplane = SpmUtilities.IsAirplane(spmText);

            Assert.That(isAirplane, Is.EqualTo(true));
        }

        [Test]
        public void IsHelicopter()
        {
            string spmText =
@"<Heli>Swash=Swash_1_Normal</Heli>";

            bool isAirplane = SpmUtilities.IsHelicopter(spmText);

            Assert.That(isAirplane, Is.EqualTo(true));
        }

        [Test]
        public void GetModelName()
        {
            string expectedModelName = "__ModelName__";
            string spmText =@"<Spektrum>Name=""" + expectedModelName + @"""</Spektrum>";

            string modelName = SpmUtilities.GetModelName(spmText);

            Assert.That(modelName, Is.EqualTo(expectedModelName));
        }

        [Test]
        public void GetGenerator()
        {
            string expectedGenerator = "DX666";
            string spmText =
@"<Spektrum>Generator=""" + expectedGenerator + @"""</Spektrum>";

            string generator = SpmUtilities.GetGenerator(spmText);

            Assert.That(generator, Is.EqualTo(expectedGenerator));
        }

        [Test]
        public void GetModelNumberFromFilename_NumberPrefix()
        {
            int expectedModelNumber = 66;
            string path = expectedModelNumber + "MODEL.SPM";

            int modelNumber = SpmUtilities.GetModelNumberFromFilename(path);

            Assert.That(modelNumber, Is.EqualTo(expectedModelNumber));
        }

        [Test]
        public void GetModelNumberFromFilename_NoNumberPrefix()
        {
            string path = "MODEL.SPM";

            int modelNumber = SpmUtilities.GetModelNumberFromFilename(path);

            Assert.That(modelNumber, Is.EqualTo(-1));
        }

        [TestCase("001~CYSL.SPM", "01CYSL.SPM")]
        [TestCase("002~1 Gofly.SPM", "01Gofly.SPM")]
        public void GetDX8Filename(string fileName, string expectedFileName)
        {
            string dx8FileName = SpmUtilities.GetDX8Filename(fileName);

            Assert.That(dx8FileName, Is.EqualTo(expectedFileName));
        }
    }
}
