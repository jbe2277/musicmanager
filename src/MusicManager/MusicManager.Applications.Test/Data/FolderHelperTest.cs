using System.Waf.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Domain;

namespace Test.MusicManager.Applications.Data
{
    [TestClass]
    public class FolderHelperTest
    {
        [TestMethod]
        public void GetFolderFromLocalizedPath()
        {
            Assert.AreEqual(@"C:\Windows", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Windows\").GetResult().Path);
            Assert.AreEqual(@"C:\Program Files\Common Files", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Program Files\Common Files").GetResult().Path);

            if (@"C:\Benutzer" == FolderHelper.GetDisplayPath(@"C:\Users").GetResult())  // German Windows
            {
                Assert.AreEqual(@"C:\Users", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Benutzer").GetResult().Path);
                Assert.AreEqual(@"C:\Users", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Benutzer\").GetResult().Path);
                Assert.AreEqual(@"C:\Users\Public", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:/Benutzer/Öffentlich/").GetResult().Path);
                Assert.AreEqual(@"C:\Users\Public", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Benutzer\Öffentlich").GetResult().Path);

                Assert.AreEqual(@"C:\Program Files\Common Files", FolderHelper.GetFolderFromLocalizedPathAsync(@"C:\Programme\Common Files").GetResult().Path);
            }
        }

        [TestMethod]
        public void GetLocalizedDisplayPath()
        {
            Assert.AreEqual(@"C:\Windows", FolderHelper.GetDisplayPath(@"C:\Windows").GetResult());

            if (@"C:\Benutzer" == FolderHelper.GetDisplayPath(@"C:\Users").GetResult())  // German Windows
            {
                Assert.AreEqual(@"C:\Benutzer", FolderHelper.GetDisplayPath(@"C:\Users\").GetResult());
                Assert.AreEqual(@"C:\Benutzer\Öffentlich", FolderHelper.GetDisplayPath(@"C:/Users/Public").GetResult());
            }
        }

        [TestMethod]
        public void GetPathSegmentsTest()
        {
            var pathSegments = FolderHelper.GetPathSegments(@"C:\Users\Public\Music");
            AssertHelper.SequenceEqual(new[] { @"C:\", "Users", "Public", "Music" }, pathSegments);

            pathSegments = FolderHelper.GetPathSegments(@"C:\Users\Public\Music\");
            AssertHelper.SequenceEqual(new[] { @"C:\", "Users", "Public", "Music" }, pathSegments);

            pathSegments = FolderHelper.GetPathSegments(@"C:");
            AssertHelper.SequenceEqual(new[] { @"C:" }, pathSegments);

            pathSegments = FolderHelper.GetPathSegments(@"\\localhost\Users\Public\Music");
            AssertHelper.SequenceEqual(new[] { @"\\localhost\Users", "Public", "Music" }, pathSegments);
        }
    }
}
