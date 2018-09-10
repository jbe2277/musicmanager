using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;
using Test.MusicManager.Domain.UnitTesting;
using System.Waf.UnitTesting;

namespace Test.MusicManager.Domain.Transcoding
{
    [TestClass]
    public class TranscodingManagerTest : DomainTest
    {
        [TestMethod]
        public void AddRemoveTranscodeItemsTest()
        {
            var musicFile1 = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(33), 320), "TestFile1.wma");
            var musicFile2 = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(33), 320), "TestFile2.wma");
            var item1 = new TranscodeItem(musicFile1, "TestFile1.mp3");
            var item2 = new TranscodeItem(musicFile1, "TestFile2.mp3");

            var manager = new TranscodingManager();
            Assert.IsFalse(manager.TranscodeItems.Any());

            manager.AddTranscodeItem(item1);
            manager.AddTranscodeItem(item2);

            AssertHelper.SequenceEqual(new[] { item1, item2 }, manager.TranscodeItems);

            manager.RemoveTranscodeItem(item1);

            Assert.AreEqual(item2, manager.TranscodeItems.Single());
        }
    }
}
