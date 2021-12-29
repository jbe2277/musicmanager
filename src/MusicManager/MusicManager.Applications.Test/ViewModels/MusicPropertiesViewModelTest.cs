using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Services;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.ViewModels
{
    [TestClass]
    public class MusicPropertiesViewModelTest : ApplicationsTest
    {
        public void CopyFileNameToClipboardTest()
        {
            var clipboardService = Container.GetExportedValue<MockClipboardService>();
            var viewModel = Container.GetExportedValue<MusicPropertiesViewModel>();
            
            string? clipboardText = null;
            clipboardService.SetTextAction = txt => clipboardText = txt;

            viewModel.CopyFileNameCommand.Execute(null);
            Assert.IsNull(clipboardText);

            var musicFile = MockMusicFile.CreateEmpty(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3");
            viewModel.MusicFile = musicFile;

            viewModel.CopyFileNameCommand.Execute(null);
            Assert.AreEqual("Culture Beat - Serenity", clipboardText);
        }
        
        [TestMethod]
        public void AutoFillFromFileNameTest()
        {
            var viewModel = Container.GetExportedValue<MusicPropertiesViewModel>();

            Assert.IsFalse(viewModel.AutoFillFromFileNameCommand.CanExecute(null));

            var musicFile = MockMusicFile.CreateEmpty(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3");            
            viewModel.MusicFile = musicFile;

            Assert.IsTrue(viewModel.AutoFillFromFileNameCommand.CanExecute(null));
            viewModel.AutoFillFromFileNameCommand.Execute(null);
            Assert.AreEqual("Culture Beat", musicFile.Metadata!.Artists[0]);
            Assert.AreEqual("Serenity", musicFile.Metadata.Title);
            Assert.IsFalse(viewModel.AutoFillFromFileNameCommand.CanExecute(null));

            musicFile = MockMusicFile.CreateEmpty(@"\\server\share\Culture Beat Serenity.wma");
            viewModel.MusicFile = musicFile;
            viewModel.AutoFillFromFileNameCommand.Execute(null);
            Assert.IsFalse(musicFile.Metadata!.Artists.Any());
            Assert.AreEqual("Culture Beat Serenity", musicFile.Metadata.Title);

            musicFile = MockMusicFile.CreateEmpty(@"C:\Culture Beat - Serenity - Epilog.wma");
            viewModel.MusicFile = musicFile;
            viewModel.AutoFillFromFileNameCommand.Execute(null);
            Assert.AreEqual("Culture Beat", musicFile.Metadata!.Artists[0]);
            Assert.AreEqual("Serenity - Epilog", musicFile.Metadata.Title);

            // CanExecute returns false when metadata are not supported.
            musicFile = new MockMusicFile(MusicMetadata.CreateUnsupported(TimeSpan.FromSeconds(55), 1411000), @"C:\Test.wav");
            viewModel.MusicFile = musicFile;
            Assert.IsFalse(viewModel.AutoFillFromFileNameCommand.CanExecute(null));
        }

        [TestMethod]
        public void UpdateAutoFillFromFileNameTestCommand()
        {
            var viewModel = Container.GetExportedValue<MusicPropertiesViewModel>();
            Assert.IsFalse(viewModel.AutoFillFromFileNameCommand.CanExecute(null));

            var musicFile = MockMusicFile.CreateEmpty(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3");
            viewModel.MusicFile = musicFile;
            Assert.IsTrue(viewModel.AutoFillFromFileNameCommand.CanExecute(null));

            AssertHelper.CanExecuteChangedEvent(viewModel.AutoFillFromFileNameCommand, () => musicFile.Metadata!.Title = "Serenity");
            Assert.IsFalse(viewModel.AutoFillFromFileNameCommand.CanExecute(null));

            AssertHelper.CanExecuteChangedEvent(viewModel.AutoFillFromFileNameCommand, () => musicFile.Metadata!.Title = "");
            Assert.IsTrue(viewModel.AutoFillFromFileNameCommand.CanExecute(null));

            AssertHelper.CanExecuteChangedEvent(viewModel.AutoFillFromFileNameCommand, () => musicFile.Metadata!.Artists = new[] { "Culture Beat" });
            Assert.IsFalse(viewModel.AutoFillFromFileNameCommand.CanExecute(null));
        }
    }
}
