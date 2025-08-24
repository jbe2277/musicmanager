using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class PathToFileNameConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var converter = PathToFileNameConverter.Default;
        Assert.AreEqual(@"Test - File", converter.Convert(@"C:\Users\Test\Music\Test - File.mp3", null, null, null));
        Assert.AreEqual(@"Test - File.mp3", converter.Convert(@"C:\Users\Test\Music\Test - File.mp3", null, "WithExtension", null));
    }
}
