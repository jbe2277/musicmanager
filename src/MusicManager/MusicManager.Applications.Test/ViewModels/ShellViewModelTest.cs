using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.ViewModels
{
    [TestClass]
    public class ShellViewModelTest : ApplicationsTest
    {
        [TestMethod]
        public void SelectViewsTest()
        {
            var musicPropertiesView = new object();
            var playlistView = new object();
            var transcodingListView = new object();

            var shellService = Container.GetExportedValue<IShellService>();
            shellService.MusicPropertiesView = musicPropertiesView;
            shellService.PlaylistView = playlistView;
            shellService.TranscodingListView = new Lazy<object>(() => transcodingListView);
            
            var viewModel = Container.GetExportedValue<ShellViewModel>();
            Assert.IsNull(viewModel.DetailsView);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsMusicPropertiesViewVisible, () => viewModel.IsMusicPropertiesViewVisible = true);
            Assert.AreEqual(musicPropertiesView, viewModel.DetailsView);
            Assert.IsTrue(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsFalse(viewModel.IsPlaylistViewVisible);
            Assert.IsFalse(viewModel.IsTranscodingListViewVisible);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsMusicPropertiesViewVisible, () => viewModel.IsPlaylistViewVisible = true);
            Assert.AreEqual(playlistView, viewModel.DetailsView);
            Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsTrue(viewModel.IsPlaylistViewVisible);
            Assert.IsFalse(viewModel.IsTranscodingListViewVisible);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsPlaylistViewVisible, () => viewModel.IsTranscodingListViewVisible = true);
            Assert.AreEqual(transcodingListView, viewModel.DetailsView);
            Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
            Assert.IsFalse(viewModel.IsPlaylistViewVisible);
            Assert.IsTrue(viewModel.IsTranscodingListViewVisible);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsTranscodingListViewVisible, () => viewModel.IsMusicPropertiesViewVisible = true);
            Assert.AreEqual(musicPropertiesView, viewModel.DetailsView);
        }
        
        [TestMethod]
        public void ErrorMessagesTest()
        {
            var viewModel = Container.GetExportedValue<ShellViewModel>();
            Assert.IsFalse(viewModel.Errors.Any());
            
            viewModel.ShowError(null, "test 1");
            Assert.AreEqual("test 1", viewModel.Errors.Single().Item2);

            viewModel.ShowError(null, "test 2");
            Assert.AreEqual("test 2", viewModel.Errors[^1].Item2);
            
            viewModel.CloseErrorCommand.Execute(null);
            Assert.AreEqual("test 1", viewModel.Errors.Single().Item2);

            viewModel.CloseErrorCommand.Execute(null);
            Assert.IsFalse(viewModel.Errors.Any());
        }
        
        [TestMethod]
        public void RestoreWindowLocationAndSize()
        {
            SetSettingsValues(20, 10, 400, 300, true);

            var shellView = Container.GetExportedValue<MockShellView>();
            shellView.VirtualScreenWidth = 1000;
            shellView.VirtualScreenHeight = 700;

            Container.GetExportedValue<ShellViewModel>();

            Assert.AreEqual(20, shellView.Left);
            Assert.AreEqual(10, shellView.Top);
            Assert.AreEqual(400, shellView.Width);
            Assert.AreEqual(300, shellView.Height);
            Assert.IsTrue(shellView.IsMaximized);

            shellView.Left = 25;
            shellView.Top = 15;
            shellView.Width = 450;
            shellView.Height = 350;
            shellView.IsMaximized = false;

            shellView.Close();
            AssertSettingsValues(25, 15, 450, 350, false);
        }

        [TestMethod]
        public void RestoreWindowLocationAndSizeSpecial()
        {
            var shellView = Container.GetExportedValue<MockShellView>();
            shellView.VirtualScreenWidth = 1000;
            shellView.VirtualScreenHeight = 700;

            shellView.SetNAForLocationAndSize();

            SetSettingsValues();
            var shellService = Container.GetExportedValue<IShellService>();
            new ShellViewModel(shellView, shellService, null!).Close();
            AssertSettingsValues(double.NaN, double.NaN, double.NaN, double.NaN, false);

            // Height is 0 => don't apply the Settings values
            SetSettingsValues(0, 0, 1, 0);
            new ShellViewModel(shellView, shellService, null!).Close();
            AssertSettingsValues(double.NaN, double.NaN, double.NaN, double.NaN, false);

            // Left = 100 + Width = 901 > VirtualScreenWidth = 1000 => don't apply the Settings values
            SetSettingsValues(100, 100, 901, 100);
            new ShellViewModel(shellView, shellService, null!).Close();
            AssertSettingsValues(double.NaN, double.NaN, double.NaN, double.NaN, false);

            // Top = 100 + Height = 601 > VirtualScreenWidth = 600 => don't apply the Settings values
            SetSettingsValues(100, 100, 100, 601);
            new ShellViewModel(shellView, shellService, null!).Close();
            AssertSettingsValues(double.NaN, double.NaN, double.NaN, double.NaN, false);

            // Use the limit values => apply the Settings values
            SetSettingsValues(0, 0, 1000, 700);
            new ShellViewModel(shellView, shellService, null!).Close();
            AssertSettingsValues(0, 0, 1000, 700, false);
        }


        private void SetSettingsValues(double left = 0, double top = 0, double width = 0, double height = 0, bool isMaximized = false)
        {
            var settings = Container.GetExportedValue<IShellService>().Settings;
            settings.Left = left;
            settings.Top = top;
            settings.Width = width;
            settings.Height = height;
            settings.IsMaximized = isMaximized;
        }

        private void AssertSettingsValues(double left, double top, double width, double height, bool isMaximized)
        {
            var settings = Container.GetExportedValue<IShellService>().Settings;
            Assert.AreEqual(left, settings.Left);
            Assert.AreEqual(top, settings.Top);
            Assert.AreEqual(width, settings.Width);
            Assert.AreEqual(height, settings.Height);
            Assert.AreEqual(isMaximized, settings.IsMaximized);
        }
    }
}
