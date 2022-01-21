using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

[TestClass]
public class ManagerStatusServiceTest : ApplicationsTest
{
    [TestMethod]
    public void BasicManagerStatusServiceTest()
    {
        var service = new ManagerStatusService();
        Assert.IsFalse(service.UpdatingFilesList);
        Assert.AreEqual(-1, service.TotalFilesCount);

        service.StartUpdatingFilesList();
        Assert.IsTrue(service.UpdatingFilesList);
        Assert.AreEqual(-1, service.TotalFilesCount);

        service.FinishUpdatingFilesList(42);
        Assert.IsFalse(service.UpdatingFilesList);
        Assert.AreEqual(42, service.TotalFilesCount);

        service.StartUpdatingFilesList();
        Assert.IsTrue(service.UpdatingFilesList);
        Assert.AreEqual(-1, service.TotalFilesCount);
    }
}
