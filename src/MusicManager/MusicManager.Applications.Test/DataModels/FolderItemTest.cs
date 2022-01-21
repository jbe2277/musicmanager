using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications.DataModels;

namespace Test.MusicManager.Applications.DataModels;

[TestClass]
public class FolderItemTest : ApplicationsTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        var folderItem = new FolderItem("a", "b");
        Assert.AreEqual("a", folderItem.Path);
        Assert.AreEqual("b", folderItem.DisplayName);
    }
}
