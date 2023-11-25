using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Presentation.Converters;

namespace Test.MusicManager.Presentation.Converters;

[TestClass]
public class MusicPropertiesEnabledConverterTest
{
    [TestMethod]
    public void ConvertTest()
    {
        var musicFile = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(20), 0), "");
        var unsupportedFile = new MockMusicFile(MusicMetadata.CreateUnsupported(TimeSpan.FromSeconds(20), 0), "");
            
        var converter = new MusicPropertiesEnabledConverter();
        Assert.AreEqual(true, converter.Convert([ musicFile, true ], null, null, null));
        Assert.AreEqual(false, converter.Convert([ unsupportedFile, true ], null, null, null));
            
        AssertHelper.ExpectedException<NotSupportedException>(() => converter.ConvertBack(null, null, null, null));
    }
}
