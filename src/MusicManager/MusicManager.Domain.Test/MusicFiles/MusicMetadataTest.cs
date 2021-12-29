using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.MusicManager.Domain.UnitTesting;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles
{
    [TestClass]
    public class MusicMetadataTest : DomainTest
    {
        [TestMethod]
        public void ApplyValuesFromTest()
        {
            var metadata1 = new MusicMetadata(TimeSpan.FromSeconds(1), 64000);
            var metadata2 = new MusicMetadata(TimeSpan.FromSeconds(1), 64000);
            Assert.IsTrue(metadata1.IsSupported);
            Assert.IsTrue(metadata2.IsSupported);
            
            metadata2.Artists = new[] { "Artist1", "Artist2" };
            metadata2.Title = "Title";
            metadata2.Rating = 80;
            metadata2.Album = "Album";
            metadata2.TrackNumber = 3;
            metadata2.Year = 2000;
            metadata2.Genre = new[] { "Genre1", "Genre2" };
            metadata2.AlbumArtist = "AlbumArtist";
            metadata2.Publisher = "Publisher";
            metadata2.Subtitle = "Subtitle";
            metadata2.Composers = new[] { "Composer1", "Composer2" };
            metadata2.Conductors = new[] { "Conductor1", "Conductor2" };

            metadata1.ApplyValuesFrom(metadata2);

            TestHelper.AssertHaveEqualPropertyValues(metadata2, metadata1, p => p.Name != nameof(MusicMetadata.Parent));
        }

        [TestMethod]
        public void UnsupportedMetadataTest()
        {
            var metadata = MusicMetadata.CreateUnsupported(TimeSpan.FromMinutes(3), 1024);
            Assert.IsFalse(metadata.IsSupported);
            Assert.AreEqual(TimeSpan.FromMinutes(3), metadata.Duration);
            Assert.AreEqual(1024, metadata.Bitrate);
        }
    }
}
