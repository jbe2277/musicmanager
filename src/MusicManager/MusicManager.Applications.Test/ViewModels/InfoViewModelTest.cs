using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.ViewModels;

[TestClass]
public class InfoViewModelTest : ApplicationsTest
{
    protected override void OnCleanup()
    {
        MockInfoView.ShowDialogAction = null;
        base.OnCleanup();
    }
        
    [TestMethod]
    public void BasicInfoViewModelTest()
    {
        var viewModel = Get<InfoViewModel>();
        var ownerWindow = new object();

        bool isShowDialogCalled = false;
        MockInfoView.ShowDialogAction = view =>
        {
            isShowDialogCalled = true;
            Assert.IsTrue(viewModel.ShowWebsiteCommand.CanExecute(null));
        };

        viewModel.ShowDialog(ownerWindow);
        Assert.IsTrue(isShowDialogCalled);
    }
}
