using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.ViewModels;

[TestClass]
public class ManagerViewModelTest : ApplicationsTest
{
    [TestMethod]
    public void ClearSearchCommandTest()
    {
        var viewModel = Get<ManagerViewModel>();
        viewModel.SearchFilter.UserSearchFilter = "test";
        viewModel.ClearSearchCommand.Execute(null);
        Assert.AreEqual("", viewModel.SearchFilter.UserSearchFilter);
    }
}
