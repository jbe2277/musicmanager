using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Data;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.Controllers;

[TestClass]
public class TranscodingControllerTest : ApplicationsTest
{
    private ObservableCollection<MusicFile> musicFiles = null!;
    private MockMusicFileContext musicFileContext = null!;
    private SelectionService selectionService = null!;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        musicFileContext = Container.GetExportedValue<MockMusicFileContext>();
        musicFiles = new ObservableCollection<MusicFile>()
        {
            musicFileContext.Create(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.wav"),
            musicFileContext.Create(@"C:\Culture Beat - Serenity - Epilog.wma"),
        };
        selectionService = Container.GetExportedValue<SelectionService>();
        selectionService.Initialize(musicFiles);

        musicFiles[0].Metadata!.Album = "Serenity";
    }
        
    [TestMethod]
    public void ConvertToMp3Selected()
    {
        var controller = Container.GetExportedValue<TranscodingController>();
        controller.Initialize();

        var shellService = Container.GetExportedValue<ShellService>();
        var view = shellService.TranscodingListView!.Value;
        var viewModel = ViewHelper.GetViewModel<TranscodingListViewModel>((IView)view)!;
        var transcodingService = viewModel.TranscodingService;
        var transcodingManager = viewModel.TranscodingManager;
            
        // No music file selected
        Assert.IsFalse(viewModel.TranscodingManager.TranscodeItems.Any());
        Assert.IsTrue(transcodingService.ConvertToMp3AllCommand.CanExecute(null));
        Assert.IsFalse(transcodingService.ConvertToMp3SelectedCommand.CanExecute(null));

        // Select first music file
        AssertHelper.CanExecuteChangedEvent(transcodingService.ConvertToMp3SelectedCommand, () => selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles.First()));
        Assert.IsTrue(transcodingService.ConvertToMp3SelectedCommand.CanExecute(null));

        // Call the convert to MP3 command

        // -- mock ShowTranscodingListView call
        var showTranscodingListViewCalled = false;
        shellService.ShowTranscodingListViewAction = () => showTranscodingListViewCalled = true;

        // -- mock TranscodeAsync call
        bool transcodeCalled = false;
        Task transcodeDelayTask = Task.CompletedTask;
        Exception? transcodeError = null;
        var transcode = Container.GetExportedValue<MockTranscoder>();
        transcode.TranscodeAsyncAction = async (sourceFileName, destinationFileName, bitrate, cancellationToken, progress) => 
        {
            transcodeCalled = true;
            Assert.AreEqual(320000u, bitrate);
            progress.Report(50);
            await transcodeDelayTask;
            cancellationToken.ThrowIfCancellationRequested();
            if (transcodeError != null) throw transcodeError;
            progress.Report(100);
        };

        // -- mock SaveChangesAsync call of destination file
        var saveChangesCalls = new List<MusicFile>();
        musicFileContext.SaveChangesAsyncAction = (musicFile) =>
        {
            saveChangesCalls.Add(musicFile);
            return Task.CompletedTask;
        };

        transcodingService.ConvertToMp3SelectedCommand.Execute(null);
        Context.WaitFor(() => transcodingManager.TranscodeItems.Single().Progress == 1, TimeSpan.FromSeconds(1));

        Assert.IsTrue(showTranscodingListViewCalled);
        Assert.IsTrue(transcodeCalled);
        Assert.AreEqual(1d, transcodingManager.TranscodeItems.Single().Progress);
        Assert.IsNull(transcodingManager.TranscodeItems.Single().Error);
        Assert.AreEqual("Serenity", saveChangesCalls.Single().Metadata!.Album);

        // Simulate the FileWatcher and add the new file to the list
        musicFiles.Add(musicFileContext.Create(transcodingManager.TranscodeItems.Single().DestinationFileName));
        Assert.IsFalse(transcodingService.ConvertToMp3SelectedCommand.CanExecute(null));
            
        // Select second music file and convert to MP3 but cancel during conversion
        selectionService.SelectedMusicFiles.Clear();
        selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[1]);
        Assert.IsTrue(transcodingService.ConvertToMp3SelectedCommand.CanExecute(null));

        var transcodeDelayTaskSource = new TaskCompletionSource<object?>();
        transcodeDelayTask = transcodeDelayTaskSource.Task;
        transcodeCalled = false;
        saveChangesCalls.Clear();

        transcodingService.ConvertToMp3SelectedCommand.Execute(null);
        Assert.IsTrue(transcodingService.CancelAllCommand.CanExecute(null));
        Assert.IsFalse(transcodingService.CancelSelectedCommand.CanExecute(null));

        AssertHelper.CanExecuteChangedEvent(transcodingService.CancelSelectedCommand, () => viewModel.SelectedTranscodeItems.Add(transcodingManager.TranscodeItems[^1]));
        Assert.IsTrue(transcodingService.CancelSelectedCommand.CanExecute(null));

        transcodingService.CancelSelectedCommand.Execute(null);
        transcodeDelayTaskSource.SetResult(null);

        Context.Wait(TimeSpan.FromMilliseconds(25));

        // The manager contains just the TranscodeItem from the first conversion.
        Assert.AreEqual(1, transcodingManager.TranscodeItems.Count);
        Assert.IsFalse(saveChangesCalls.Any());

        // Convert again the second music file; simulate a conversion error
        transcodeError = new InvalidOperationException("Test");
        transcodingService.ConvertToMp3AllCommand.Execute(null);
        Assert.AreEqual(transcodeError, transcodingManager.TranscodeItems[^1].Error);

        controller.Shutdown();
    }
        
    [TestMethod]
    public void GetConvertBitrateTest()
    {
        Assert.AreEqual(128000u, TranscodingController.GetConvertBitrate(80000));
        Assert.AreEqual(128000u, TranscodingController.GetConvertBitrate(128000));

        Assert.AreEqual(192000u, TranscodingController.GetConvertBitrate(191000));
        Assert.AreEqual(192000u, TranscodingController.GetConvertBitrate(192000));

        Assert.AreEqual(256000u, TranscodingController.GetConvertBitrate(224000));
        Assert.AreEqual(256000u, TranscodingController.GetConvertBitrate(256000));

        Assert.AreEqual(320000u, TranscodingController.GetConvertBitrate(300000));
        Assert.AreEqual(320000u, TranscodingController.GetConvertBitrate(320000));
        Assert.AreEqual(320000u, TranscodingController.GetConvertBitrate(1411000));
    }
}
