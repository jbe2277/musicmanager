using System.Waf.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Presentation.Services;

namespace Test.MusicManager.Presentation.Services;

[TestClass]
public class FileServiceTest
{
    [TestMethod]
    public void GetFolderFromLocalizedPath()
    {
        Assert.AreEqual(@"C:\Windows", FileService.GetFolderFromLocalizedPathAsync(@"C:\Windows\").GetResult().Path);
        Assert.AreEqual(@"C:\Program Files\Common Files", FileService.GetFolderFromLocalizedPathAsync(@"C:\Program Files\Common Files").GetResult().Path);

        var fileService = new FileService();
        if (@"C:\Benutzer" == fileService.GetDisplayPath(@"C:\Users").GetResult())  // German Windows
        {
            Assert.AreEqual(@"C:\Users", FileService.GetFolderFromLocalizedPathAsync(@"C:\Benutzer").GetResult().Path);
            Assert.AreEqual(@"C:\Users", FileService.GetFolderFromLocalizedPathAsync(@"C:\Benutzer\").GetResult().Path);
            Assert.AreEqual(@"C:\Users\Public", FileService.GetFolderFromLocalizedPathAsync(@"C:/Benutzer/Öffentlich/").GetResult().Path);
            Assert.AreEqual(@"C:\Users\Public", FileService.GetFolderFromLocalizedPathAsync(@"C:\Benutzer\Öffentlich").GetResult().Path);

            Assert.AreEqual(@"C:\Program Files\Common Files", FileService.GetFolderFromLocalizedPathAsync(@"C:\Programme\Common Files").GetResult().Path);
        }
    }

    [TestMethod]
    public void GetLocalizedDisplayPath()
    {
        var fileService = new FileService();
        Assert.AreEqual(@"C:\Windows", fileService.GetDisplayPath(@"C:\Windows").GetResult());

        if (@"C:\Benutzer" == fileService.GetDisplayPath(@"C:\Users").GetResult())  // German Windows
        {
            Assert.AreEqual(@"C:\Benutzer", fileService.GetDisplayPath(@"C:\Users\").GetResult());
            Assert.AreEqual(@"C:\Benutzer\Öffentlich", fileService.GetDisplayPath(@"C:/Users/Public").GetResult());
        }
    }

    [TestMethod]
    public void GetPathSegmentsTest()
    {
        var pathSegments = FileService.GetPathSegments(@"C:\Users\Public\Music");
        AssertHelper.SequenceEqual(new[] { @"C:\", "Users", "Public", "Music" }, pathSegments);

        pathSegments = FileService.GetPathSegments(@"C:\Users\Public\Music\");
        AssertHelper.SequenceEqual(new[] { @"C:\", "Users", "Public", "Music" }, pathSegments);

        pathSegments = FileService.GetPathSegments(@"C:");
        AssertHelper.SequenceEqual(new[] { @"C:" }, pathSegments);

        pathSegments = FileService.GetPathSegments(@"\\localhost\Users\Public\Music");
        AssertHelper.SequenceEqual(new[] { @"\\localhost\Users", "Public", "Music" }, pathSegments);
    }
}
