using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class PathToDirectoryNameConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var converter = PathToDirectoryNameConverter.Default;
        Assert.AreEqual(@"C:\Users\Test\Music", converter.Convert(@"C:\Users\Test\Music\Test - File.mp3", null, null, null));
    }
}
