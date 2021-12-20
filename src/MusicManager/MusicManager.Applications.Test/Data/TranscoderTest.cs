using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Domain.UnitTesting;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data
{
    [TestClass]
    public class TranscoderTest : IntegrationTest
    {
        [TestMethod, TestCategory("IntegrationTest")]
        public void TranscodeFileTest()
        {
            var fileName = TestHelper.GetTempFileName(".wma");
            File.Copy(Environment.CurrentDirectory + @"\Files\TestWMA.wma", fileName, true);

            var ctx = Container.GetExportedValue<MusicFileContext>();
            var musicFile = ctx.Create(fileName);
            musicFile.GetMetadataAsync().Wait(Context);

            var transcoder = Container.GetExportedValue<Transcoder>();
            var destinationFileName = TestHelper.GetTempFileName(".mp3");
            int progressCount = 0;
            double progress = 0;
            transcoder.TranscodeAsync(musicFile.FileName!, destinationFileName, 320000, CancellationToken.None, new Progress<double>(p =>
            {
                progressCount++;
                progress = p;
            }
            )).Wait(Context);

            Assert.IsTrue(progressCount >= 1);
            Assert.AreEqual(100d, progress);

            var destinationMusicFile = ctx.Create(destinationFileName);
            destinationMusicFile.GetMetadataAsync().Wait(Context);

            Assert.AreNotEqual(musicFile, destinationMusicFile);
            Assert.AreNotEqual(musicFile.Metadata, destinationMusicFile.Metadata);
            Assert.AreEqual(musicFile.Metadata!.Duration.TotalMilliseconds, destinationMusicFile.Metadata!.Duration.TotalMilliseconds, 50);
            Assert.AreEqual(320000, destinationMusicFile.Metadata.Bitrate);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TranscodeCorruptFileTest()
        {
            var fileName = TestHelper.GetTempFileName(".wma");
            File.Copy(Environment.CurrentDirectory + @"\Files\Corrupt.wma", fileName, true);

            var ctx = Container.GetExportedValue<MusicFileContext>();
            var musicFile = ctx.Create(fileName);
            musicFile.GetMetadataAsync().Wait(Context);

            var transcoder = Container.GetExportedValue<Transcoder>();
            var destinationFileName = TestHelper.GetTempFileName(".mp3");
            int progressCount = 0;
            double progress = 0;
            AssertHelper.ExpectedException<InvalidOperationException>(() =>
            {
                transcoder.TranscodeAsync(musicFile.FileName!, destinationFileName, 320000, CancellationToken.None, new Progress<double>(p =>
                {
                    progressCount++;
                    progress = p;
                }
                )).Wait(Context);
            });
            
            Assert.AreEqual(0, progressCount);
            Assert.AreEqual(0d, progress);
            Assert.IsFalse(File.Exists(destinationFileName));

            Context.Wait(TimeSpan.FromSeconds(1));
        }
    }
}
