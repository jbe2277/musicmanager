using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles;

[TestClass]
public class MusicFileTest : DomainTest
{
    private readonly AssertUnobservedExceptions assertUnobservedExceptions = new(); 

    protected override void OnInitialize()
    {
 	    base.OnInitialize();
        assertUnobservedExceptions.Initialize();
        ServiceLocator.RegisterInstance<IChangeTrackerService>(new MockChangeTrackerService());
    }

    protected override void OnCleanup()
    {
        assertUnobservedExceptions.Cleanup();
        base.OnCleanup();
    }

    [TestMethod]
    public async Task ImplicitLoadMetadata()
    {
        var metadata = new MusicMetadata(TimeSpan.Zero, 0);
        var musicFile = new MusicFile(async fileNme => 
        {
            await Task.Yield();
            return metadata;
        }, "testfile.mp3");

        Assert.AreEqual("testfile.mp3", musicFile.FileName);
        Assert.IsFalse(musicFile.SharedMusicFiles.Any());
        Assert.IsFalse(musicFile.IsMetadataLoaded);

        Assert.IsNull(musicFile.Metadata);
        await Task.Delay(5);
        Assert.AreEqual(metadata, musicFile.Metadata);
        Assert.AreEqual(metadata, musicFile.GetMetadataAsync().Result);
        Assert.IsTrue(musicFile.IsMetadataLoaded);
        Assert.AreEqual(musicFile, musicFile.Metadata!.Parent);

        Assert.IsFalse(musicFile.Metadata.HasChanges);
        musicFile.Metadata.Rating = 33;
        Assert.IsTrue(musicFile.Metadata.HasChanges);
    }
        
    [TestMethod]
    public void ImplicitLoadMetadataWithException()
    {
        var musicFile = new MusicFile(fileName => { throw new InvalidOperationException("Test Exception"); }, "");

        Assert.IsNull(musicFile.Metadata);
        Assert.IsFalse(musicFile.IsMetadataLoaded);
        Assert.IsInstanceOfType<InvalidOperationException>(musicFile.LoadError);
    }

    [TestMethod]
    public void ImplicitLoadMetadataWithNull()
    {
        var musicFile = new MusicFile(fileName => Task.FromResult((MusicMetadata?)null), "");

        Assert.IsNull(musicFile.Metadata);
        Assert.IsFalse(musicFile.IsMetadataLoaded);
        Assert.IsInstanceOfType<InvalidOperationException>(musicFile.LoadError);
    }
}
