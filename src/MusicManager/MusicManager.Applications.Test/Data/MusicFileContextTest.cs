using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Domain.UnitTesting;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.Data
{
    [TestClass]
    public class MusicFileContextTest : IntegrationTest
    {
        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadMP3FileWithMetadata()
        {
            var fileName = TestHelper.GetTempFileName(".mp3");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestMP3.mp3", fileName, true);
            SaveAndLoadFileWithMetadataCore(fileName);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadWMAFileWithMetadata()
        {
            var fileName = TestHelper.GetTempFileName(".wma");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestWMA.wma", fileName, true);
            SaveAndLoadFileWithMetadataCore(fileName);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadM4AFileWithMetadata()
        {
            var fileName = TestHelper.GetTempFileName(".m4a");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestAAC.m4a", fileName, true);
            SaveAndLoadFileWithMetadataCore(fileName);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadMP4FileWithMetadata()
        {
            var fileName = TestHelper.GetTempFileName(".mp4");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestMP4.mp4", fileName, true);
            SaveAndLoadFileWithMetadataCore(fileName);
        }

        private void SaveAndLoadFileWithMetadataCore(string fileName, bool isFlac = false)
        {
            var ctx = Container.GetExportedValue<MusicFileContext>();
            var musicFile = ctx.Create(fileName);
            musicFile.GetMetadataAsync().Wait(Context);

            Assert.IsTrue(musicFile.Metadata!.IsSupported);
            Assert.IsTrue(musicFile.Metadata.Duration.Ticks > 0);
            Assert.IsTrue(musicFile.Metadata.Bitrate > 0);
            Assert.IsFalse(musicFile.Metadata.Artists.Any());
            Assert.AreEqual("", musicFile.Metadata.Title);

            SetMusicFileData(musicFile);

            ctx.SaveChangesAsync(musicFile).Wait(Context);

            // Simulate an application restart
            Cleanup();
            Initialize();

            var ctx2 = Container.GetExportedValue<MusicFileContext>();
            var musicFile2 = ctx2.Create(fileName);
            musicFile2.GetMetadataAsync().Wait(Context);

            Assert.AreNotEqual(musicFile, musicFile2);
            Assert.AreNotEqual(musicFile.Metadata, musicFile2.Metadata);

            var notSupportedByFlac = new[] { nameof(MusicMetadata.Rating), nameof(MusicMetadata.Bitrate), nameof(MusicMetadata.Publisher), nameof(MusicMetadata.Subtitle) };
            TestHelper.AssertHaveEqualPropertyValues(musicFile.Metadata, musicFile2.Metadata, p => p.Name != nameof(MusicMetadata.Parent) && (!isFlac || !notSupportedByFlac.Contains(p.Name)));
        }

        private static void SetMusicFileData(MusicFile musicFile)
        {
            musicFile.Metadata!.Artists = new[] { "Artist1", "Artist2" };
            musicFile.Metadata.Title = "Title";
            musicFile.Metadata.Rating = 75;
            musicFile.Metadata.Album = "Album";
            musicFile.Metadata.TrackNumber = 3;
            musicFile.Metadata.Year = 2000;
            musicFile.Metadata.Genre = new[] { "Genre1", "Genre2" };
            musicFile.Metadata.AlbumArtist = "AlbumArtist";
            musicFile.Metadata.Publisher = "Publisher";
            musicFile.Metadata.Subtitle = "Subtitle";
            musicFile.Metadata.Composers = new[] { "Composer1", "Composer2" };
            musicFile.Metadata.Conductors = new[] { "Conductor1", "Conductor2" };
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadWAVFileWithMetadata()
        {
            var fileName = TestHelper.GetTempFileName(".wav");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestWAV.wav", fileName, true);
            SaveAndLoadFileWithUnsupportedMetadataCore(fileName, true);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadFlacFileWithMetadata()
        {
            var fileName = TestHelper.GetTempFileName(".flac");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestFlac.flac", fileName, true);
            SaveAndLoadFileWithMetadataCore(fileName, true);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SaveAndLoadCorructFile()
        {
            var fileName = TestHelper.GetTempFileName(".wma");
            File.Copy(Environment.CurrentDirectory + @"\Files\Corrupt.wma", fileName, true);
            SaveAndLoadFileWithUnsupportedMetadataCore(fileName, false);
        }

        private void SaveAndLoadFileWithUnsupportedMetadataCore(string fileName, bool isAudioAvailable)
        {
            var ctx = Container.GetExportedValue<MusicFileContext>();
            var musicFile = ctx.Create(fileName);
            musicFile.GetMetadataAsync().Wait(Context);

            Assert.IsFalse(musicFile.Metadata!.IsSupported);
            if (isAudioAvailable)
            {
                Assert.IsTrue(musicFile.Metadata.Duration.Ticks > 0);
                Assert.IsTrue(musicFile.Metadata.Bitrate > 0);
            }
            else
            {
                Assert.AreEqual(0, musicFile.Metadata.Duration.Ticks);
                Assert.AreEqual(0, musicFile.Metadata.Bitrate);
            }
            Assert.IsFalse(musicFile.Metadata.Artists.Any());
            Assert.AreEqual("", musicFile.Metadata.Title);

            // Do not set any metadata properties because it is not supported.
            // Just ensure that the following code has no negative side-effects on unsupported metadata.

            ctx.SaveChangesAsync(musicFile).Wait(Context);

            // Simulate an application restart
            Cleanup();
            Initialize();
            
            var ctx2 = Container.GetExportedValue<MusicFileContext>();
            var musicFile2 = ctx2.Create(fileName);
            musicFile2.GetMetadataAsync().Wait(Context);

            Assert.AreNotEqual(musicFile, musicFile2);
            Assert.AreNotEqual(musicFile.Metadata, musicFile2.Metadata);
            TestHelper.AssertHaveEqualPropertyValues(musicFile.Metadata, musicFile2.Metadata, p => p.Name != nameof(MusicMetadata.Parent));
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SharedMusicFilesTest()
        {
            var fileName1 = TestHelper.GetTempFileName(".mp3");
            var fileName2 = TestHelper.GetTempFileName(".mp3");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestMP3.mp3", fileName1, true);
            File.Copy(Environment.CurrentDirectory + @"\Files\TestMP3.mp3", fileName2, true);

            var ctx = Container.GetExportedValue<MusicFileContext>();
            var musicFile1 = ctx.Create(fileName1);
            var musicFile2 = ctx.Create(fileName2);
            AssertHelper.ExpectedException<ArgumentException>(() => ctx.CreateFromMultiple(Array.Empty<MusicFile>()));
            var sharedMusicFile = ctx.CreateFromMultiple(new[] { musicFile1, musicFile2 });
            sharedMusicFile.GetMetadataAsync().Wait(Context);

            Assert.IsTrue(sharedMusicFile.Metadata!.IsSupported);
            Assert.IsFalse(sharedMusicFile.Metadata.HasChanges);
            Assert.IsFalse(sharedMusicFile.Metadata.Artists.Any());
            SetMusicFileData(sharedMusicFile);
            Assert.IsTrue(sharedMusicFile.Metadata.HasChanges);
            Assert.IsTrue(sharedMusicFile.Metadata.Artists.Any());

            Assert.IsFalse(musicFile1.Metadata!.Artists.Any());
            ctx.ApplyChanges(sharedMusicFile);
            Assert.IsTrue(musicFile2.Metadata!.Artists.Any());

            TestHelper.AssertHaveEqualPropertyValues(sharedMusicFile.Metadata, musicFile1.Metadata, p => p.Name != nameof(MusicMetadata.Parent));
            TestHelper.AssertHaveEqualPropertyValues(sharedMusicFile.Metadata, musicFile2.Metadata, p => p.Name != nameof(MusicMetadata.Parent));


            musicFile1.Metadata.Title = "Title1";
            musicFile2.Metadata.Title = "Title2";
            sharedMusicFile = ctx.CreateFromMultiple(new[] { musicFile1, musicFile2 });
            sharedMusicFile.GetMetadataAsync().Wait(Context);
            Assert.AreEqual("", sharedMusicFile.Metadata!.Title);
            
            ctx.ApplyChanges(sharedMusicFile); // Nothing happens because sharedMusicFile has no changes.
            
            sharedMusicFile.Metadata.Rating = 75;
            ctx.ApplyChanges(sharedMusicFile);
            Assert.AreEqual("Title2", musicFile2.Metadata.Title);
            Assert.AreEqual(75u, musicFile2.Metadata.Rating);

            sharedMusicFile.Metadata.Title = "New Title";
            ctx.ApplyChanges(sharedMusicFile);
            Assert.AreEqual("New Title", musicFile1.Metadata.Title);
            Assert.AreEqual("New Title", musicFile2.Metadata.Title);
        }
    }
}
