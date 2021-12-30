using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.ViewModels;

[TestClass]
public class ManagerViewModelTest : ApplicationsTest
{
    [TestMethod]
    public void ClearSearchCommandTest()
    {
        var viewModel = Container.GetExportedValue<ManagerViewModel>();
        viewModel.SearchFilter.UserSearchFilter = "test";
        viewModel.ClearSearchCommand.Execute(null);
        Assert.AreEqual("", viewModel.SearchFilter.UserSearchFilter);
    }
}
