using System;
using System.Threading.Tasks;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Domain.MusicFiles
{
    public class MockMusicFile : MusicFile
    {
        public MockMusicFile(MusicMetadata? metadata, string? fileName) : base(x => Task.FromResult(metadata), fileName)
        {
            GetMetadataAsync().GetAwaiter().GetResult();  // Ensure that the metadata is loaded.
        }

        public static MusicFile CreateEmpty(string fileName)
        {
            var musicFile = new MockMusicFile(new MusicMetadata(new TimeSpan(0, 3, 33), 320000)
            {
                Artists = Array.Empty<string>(),
                Title = ""
            }, fileName);
            return musicFile;
        }
    }
}
