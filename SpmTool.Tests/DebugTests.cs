using NUnit.Framework;
using SpmTool;
using System.IO;
using System.Xml.XPath;

namespace SpmTool.Tests
{
    public class DebugTests
    {
        [Test]
        public void Debug_SpmToXml()
        {
            var dx8Spm = @"<Spektrum>Generator=""DX8""VCode="" 3.00""</Spektrum>";
            TestContext.WriteLine("=== INPUT ===");
            TestContext.WriteLine(repr(dx8Spm));
            
            var xml = SpmToXml.Convert(dx8Spm);
            TestContext.WriteLine("=== XML FROM SpmToXml ===");
            TestContext.WriteLine(xml);
            
            var dx9Spm = SpmConvert.DX8To(dx8Spm);
            TestContext.WriteLine("=== DX9 SPM ===");
            TestContext.WriteLine(dx9Spm);
        }
        
        [Test]
        public void Debug_FMode_Names_Sail()
        {
            var dx8Spm = @"<Spektrum>
Type=Sail
</Spektrum>
<Sail>
</Sail>
<FMode>
</FMode>";
            var dx9Spm = SpmConvert.DX8To(dx8Spm);
            TestContext.WriteLine("=== DX9 SPM ===");
            TestContext.WriteLine(dx9Spm);
        }
        
        private string repr(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}
