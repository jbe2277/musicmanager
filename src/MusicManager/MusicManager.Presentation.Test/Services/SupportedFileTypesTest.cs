using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Presentation.Services;

namespace Test.MusicManager.Presentation.Services;

[TestClass]
public class SupportedFileTypesTest
{
    [TestMethod]
    public void FileTypesTest()
    {
        Assert.IsTrue(SupportedFileTypes.MusicFileExtensions.Contains(".mp3"));
        Assert.IsTrue(SupportedFileTypes.PlaylistFileExtensions.Contains(".m3u"));

        AssertHelper.ExpectedException<NotSupportedException>(() => SupportedFileTypes.GetReadMetadata(".foo"));
        AssertHelper.ExpectedException<NotSupportedException>(() => SupportedFileTypes.GetSaveMetadata(".foo"));
    }
}
