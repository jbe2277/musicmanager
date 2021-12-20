using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.UnitTesting.Mocks;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.Controllers
{
    [TestClass]
    public class PlaylistControllerIntegrationTest : IntegrationTest
    {
        private ModuleController moduleController = null!;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            moduleController = Container.GetExportedValue<ModuleController>();
            moduleController.Initialize();
            moduleController.Run();
        }

        protected override void OnCleanup()
        {
            moduleController.Shutdown();
            base.OnCleanup();
        }
        
        [TestMethod]
        public void SaveAndLoadPlaylist()
        {
            string musicFileName1 = Environment.CurrentDirectory + @"\Files\TestMP3.mp3";
            string musicFileName2 = Environment.CurrentDirectory + @"\Files\TestWMA.wma";
            string playlistFileName = Environment.CurrentDirectory + @"\TestPlaylist.m3u";
            
            var shellService = Container.GetExportedValue<ShellService>();
            var view = shellService.PlaylistView!;
            var viewModel = ViewHelper.GetViewModel<PlaylistViewModel>((IView)view)!;
            viewModel.InsertFilesAction(0, new[] 
            { 
                musicFileName1,
                musicFileName2
            });
            Assert.AreEqual(2, viewModel.PlaylistManager.Items.Count);

            var fileDialogService = Container.GetExportedValue<MockFileDialogService>();
            fileDialogService.Result = new FileDialogResult(playlistFileName, new FileType("test", ".m3u"));
            viewModel.SaveListCommand.Execute(null);

            Assert.IsTrue(File.Exists(playlistFileName));

            viewModel.ClearListCommand.Execute(null);
            Assert.AreEqual(0, viewModel.PlaylistManager.Items.Count);

            viewModel.OpenListCommand.Execute(null);
            Assert.AreEqual(2, viewModel.PlaylistManager.Items.Count);
            Assert.AreEqual(musicFileName1, viewModel.PlaylistManager.Items[0].MusicFile.FileName, ignoreCase: true);
            Assert.AreEqual(musicFileName2, viewModel.PlaylistManager.Items[1].MusicFile.FileName, ignoreCase: true);

            File.Delete(playlistFileName);
        }
    }
}
