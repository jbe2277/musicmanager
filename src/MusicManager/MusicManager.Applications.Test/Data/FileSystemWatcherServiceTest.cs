using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data;

[TestClass]
public class FileSystemWatcherServiceTest : IntegrationTest
{
    private readonly List<string> fileNames = new();
    private string testWatcherDirectory = null!;
        
    protected override void OnInitialize()
    {
        base.OnInitialize();
        testWatcherDirectory = Environment.CurrentDirectory + @"\Files\Watcher";
        Directory.CreateDirectory(testWatcherDirectory);
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
        foreach (var fileName in fileNames)
        {
            try
            {
                File.Delete(Path.Combine(testWatcherDirectory, fileName));
            }
            catch (DirectoryNotFoundException) { }
            catch (FileNotFoundException) { }
        }

        try
        {
            Directory.Delete(testWatcherDirectory);
        }
        catch (DirectoryNotFoundException) { }
    }

    [TestMethod, TestCategory("IntegrationTest")]
    public void BasicFileSystemWatcherTest()
    {
        var service = Container.GetExportedValue<FileSystemWatcherService>();
        service.NotifyFilter = NotifyFilters.FileName;
        Assert.AreEqual(NotifyFilters.FileName, service.NotifyFilter);
        var createdEventOccurred = new TaskCompletionSource<FileSystemEventArgs>();
        var renamedEventOccurred = new TaskCompletionSource<FileSystemEventArgs>();
        var deletedEventOccurred = new TaskCompletionSource<FileSystemEventArgs>();
        service.Created += (sender, e) => createdEventOccurred.SetResult(e);
        service.Renamed += (sender, e) => renamedEventOccurred.SetResult(e);
        service.Deleted += (sender, e) => deletedEventOccurred.SetResult(e);

        service.Path = testWatcherDirectory;
        Assert.AreEqual(testWatcherDirectory, service.Path);
        service.EnableRaisingEvents = true;
        Assert.IsTrue(service.EnableRaisingEvents);

        var fileName1 = GetTestFilePath("File1.mp3");
        File.WriteAllText(fileName1, "Test File");
        createdEventOccurred.Task.Wait(Context);
        Assert.AreEqual(fileName1, createdEventOccurred.Task.Result.FullPath);

        var fileName2 = GetTestFilePath("File2.mp3");
        File.Move(fileName1, fileName2);
        renamedEventOccurred.Task.Wait(Context);
        Assert.AreEqual(fileName2, renamedEventOccurred.Task.Result.FullPath);

        File.Delete(fileName2);
        deletedEventOccurred.Task.Wait(Context);
        Assert.AreEqual(fileName2, deletedEventOccurred.Task.Result.FullPath);
    }

    private string GetTestFilePath(string fileName)
    {
        fileNames.Add(fileName);
        return Path.Combine(testWatcherDirectory, fileName);
    }
}
