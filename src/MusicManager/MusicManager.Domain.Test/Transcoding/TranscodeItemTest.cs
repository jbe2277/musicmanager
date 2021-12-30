using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Domain.MusicFiles;
using Test.MusicManager.Domain.UnitTesting;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Test.MusicManager.Domain.Transcoding;

[TestClass]
public class TranscodeItemTest : DomainTest
{
    [TestMethod]
    public void PropertiesTest()
    {
        var musicFile = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(33), 320), "TestFile.wma");
        var item = new TranscodeItem(musicFile, "TestFile.mp3");

        Assert.AreEqual(musicFile, item.Source);
        Assert.AreEqual("TestFile.mp3", item.DestinationFileName);

        Assert.AreEqual(0, item.Progress);
        AssertHelper.PropertyChangedEvent(item, x => x.Progress, () => item.Progress = 0.5);
        Assert.AreEqual(0.5, item.Progress);

        Assert.IsNull(item.Error);
        var exception = new InvalidOperationException("Test");
        AssertHelper.PropertyChangedEvent(item, x => x.Error, () => item.Error = exception);
        Assert.AreEqual(exception, item.Error);
    }

    [TestMethod]
    public void TranscodeStatusTest()
    {
        var musicFile = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(33), 320), "TestFile.wma");
        var item = new TranscodeItem(musicFile, "TestFile.mp3");

        Assert.AreEqual(TranscodeStatus.Pending, item.TranscodeStatus);

        AssertHelper.PropertyChangedEvent(item, x => x.TranscodeStatus, () => item.Progress = 0.01);
        Assert.AreEqual(TranscodeStatus.InProgress, item.TranscodeStatus);

        AssertHelper.PropertyChangedEvent(item, x => x.TranscodeStatus, () => item.Progress = 1);
        Assert.AreEqual(TranscodeStatus.Completed, item.TranscodeStatus);

        AssertHelper.PropertyChangedEvent(item, x => x.TranscodeStatus, () => item.Error = new InvalidOperationException());
        Assert.AreEqual(TranscodeStatus.Error, item.TranscodeStatus);
    }
}
