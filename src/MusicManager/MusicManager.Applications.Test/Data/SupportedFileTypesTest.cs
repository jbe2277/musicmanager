using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data
{
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
}
