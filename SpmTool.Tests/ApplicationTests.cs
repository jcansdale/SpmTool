using NUnit.Framework;
using System.IO;

namespace SpmTool.Tests
{
    public class ApplicationTests
    {
        string tempDir;

        [SetUp]
        public void SetUp()
        {
            tempDir = Path.Combine(Path.GetTempPath(), "SpmToolTests_" + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }

        static string DX8AirplaneSpm => @"<Spektrum>
Generator=""DX8""
VCode="" 3.00""
Name=""TestModel""
</Spektrum>
<Acro>
Wing=Standard
Tail=Normal
</Acro>";

        static string DX8HelicopterSpm => @"<Spektrum>
Generator=""DX8""
VCode="" 3.00""
Name=""HeliModel""
</Spektrum>
<Heli>
Swash=Swash_1_Normal
</Heli>";

        static string NonAirplaneOrHeliSpm => @"<Spektrum>
Generator=""DX8""
VCode="" 3.00""
Name=""Other""
</Spektrum>";

        [Test]
        public void SingleFile_ConvertsToDX9()
        {
            var spmFile = Path.Combine(tempDir, "model.spm");
            File.WriteAllText(spmFile, DX8AirplaneSpm);

            Application.Main(new[] { spmFile });

            var outputFile = Path.Combine(tempDir, "DX9", "model.spm");
            Assert.That(File.Exists(outputFile), Is.True, "Output file should exist");
            var output = File.ReadAllText(outputFile);
            Assert.That(output, Does.Contain("Generator=\"DX9\""));
            Assert.That(output, Does.Not.Contain("VCode"));
        }

        [Test]
        public void SingleFile_Helicopter_ConvertsToDX9()
        {
            var spmFile = Path.Combine(tempDir, "heli.spm");
            File.WriteAllText(spmFile, DX8HelicopterSpm);

            Application.Main(new[] { spmFile });

            var outputFile = Path.Combine(tempDir, "DX9", "heli.spm");
            Assert.That(File.Exists(outputFile), Is.True, "Output file should exist");
            var output = File.ReadAllText(outputFile);
            Assert.That(output, Does.Contain("Generator=\"DX9\""));
        }

        [Test]
        public void Directory_ConvertsAllSpmFiles()
        {
            File.WriteAllText(Path.Combine(tempDir, "model1.spm"), DX8AirplaneSpm);
            File.WriteAllText(Path.Combine(tempDir, "model2.spm"), DX8HelicopterSpm);
            File.WriteAllText(Path.Combine(tempDir, "readme.txt"), "not an spm file");

            Application.Main(new[] { tempDir });

            var dx9Dir = Path.Combine(tempDir, "DX9");
            Assert.That(File.Exists(Path.Combine(dx9Dir, "model1.spm")), Is.True);
            Assert.That(File.Exists(Path.Combine(dx9Dir, "model2.spm")), Is.True);
            Assert.That(File.Exists(Path.Combine(dx9Dir, "readme.txt")), Is.False,
                "Non-SPM files should not be converted");
        }

        [Test]
        public void SingleFile_NonAirplaneOrHeli_SkipsFile()
        {
            var spmFile = Path.Combine(tempDir, "other.spm");
            File.WriteAllText(spmFile, NonAirplaneOrHeliSpm);

            Application.Main(new[] { spmFile });

            var outputFile = Path.Combine(tempDir, "DX9", "other.spm");
            Assert.That(File.Exists(outputFile), Is.False,
                "Non-airplane/helicopter models should be skipped");
        }

        [Test]
        public void SingleFile_WithModelNumber_IncludesInName()
        {
            var spmFile = Path.Combine(tempDir, "03_model.spm");
            File.WriteAllText(spmFile, DX8AirplaneSpm);

            Application.Main(new[] { spmFile });

            var outputFile = Path.Combine(tempDir, "DX9", "03_model.spm");
            Assert.That(File.Exists(outputFile), Is.True);
            var output = File.ReadAllText(outputFile);
            Assert.That(output, Does.Contain("3: TestModel"));
        }
    }
}
