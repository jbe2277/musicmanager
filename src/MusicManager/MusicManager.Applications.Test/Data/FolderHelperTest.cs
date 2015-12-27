using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Domain;

namespace Test.MusicManager.Applications.Data
{
    [TestClass]
    public class FolderHelperTest
    {
        [TestMethod, TestCategory("GermanWindows")] // This Test Method runs only on a german Windows
        public void GetFolderFromLocalizedPath()
        {
            Assert.AreEqual(@"C:\Windows", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Windows\").GetResult().Path);

            Assert.AreEqual(@"C:\Program Files\Common Files", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Programme\Common Files").GetResult().Path);
            Assert.AreEqual(@"C:\Program Files\Common Files", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Program Files\Common Files").GetResult().Path);

            Assert.AreEqual(@"C:\Users", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Benutzer\").GetResult().Path);
            Assert.AreEqual(@"C:\Users", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Benutzer").GetResult().Path);
            Assert.AreEqual(@"C:\Users\Public", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:/Benutzer/Öffentlich/").GetResult().Path);
            Assert.AreEqual(@"C:\Users\Public", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Benutzer\Öffentlich").GetResult().Path);

            Assert.AreEqual(@"\\localhost\Users\Public\Music", FolderHelper.GetFolderFromLocalizedPathAsync(@"\\localhost\Users\Public\Music").GetResult().Path);
            Assert.AreEqual(@"\\localhost\Users\Public\Music", FolderHelper.GetFolderFromLocalizedPathAsync(@"\\localhost\Users\Öffentlich\Öffentliche Musik").GetResult().Path);
        }

        [TestMethod, TestCategory("GermanWindows")] // This Test Method runs only on a german Windows
        public void GetLocalizedDisplayPath()
        {
            Assert.AreEqual(@"C:\Windows", FolderHelper.GetDisplayPath(@"C:\Windows").GetResult());
            
            Assert.AreEqual(@"C:\Benutzer", FolderHelper.GetDisplayPath(@"C:\Users").GetResult());
            Assert.AreEqual(@"C:\Benutzer", FolderHelper.GetDisplayPath(@"C:\Users\").GetResult());
            
            Assert.AreEqual(@"C:\Benutzer\Öffentlich", FolderHelper.GetDisplayPath(@"C:/Users/Public").GetResult());

            Assert.AreEqual(@"\\localhost\Users\Öffentlich\Öffentliche Musik", FolderHelper.GetDisplayPath(@"\\localhost\Users\Public\Music").GetResult());
        }

        [TestMethod]
        public void GetPathSegmentsTest()
        {
            var pathSegments = FolderHelper.GetPathSegments(@"C:\Users\Public\Music");
            Assert.IsTrue(new[] { @"C:\", "Users", "Public", "Music" }.SequenceEqual(pathSegments));

            pathSegments = FolderHelper.GetPathSegments(@"C:\Users\Public\Music\");
            Assert.IsTrue(new[] { @"C:\", "Users", "Public", "Music" }.SequenceEqual(pathSegments));

            pathSegments = FolderHelper.GetPathSegments(@"C:");
            Assert.IsTrue(new[] { @"C:" }.SequenceEqual(pathSegments));

            pathSegments = FolderHelper.GetPathSegments(@"\\localhost\Users\Public\Music");
            Assert.IsTrue(new[] { @"\\localhost\Users", "Public", "Music" }.SequenceEqual(pathSegments));
        }
    }
}
