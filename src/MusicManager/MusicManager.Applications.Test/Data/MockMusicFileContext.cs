using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.Data
{
    [Export, Export(typeof(IMusicFileContext)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
    public class MockMusicFileContext : IMusicFileContext
    {
        private readonly Dictionary<string, MusicFile> musicFilesCache = new Dictionary<string,MusicFile>();

        public Action<MusicFile> ApplyChangesAction { get; set; }

        public Func<MusicFile, Task> SaveChangesAsyncAction { get; set; }


        public MusicFile Create(string fileName)
        {
            MusicFile musicFile;
            if (!musicFilesCache.TryGetValue(fileName, out musicFile))
            {
                musicFile = MockMusicFile.CreateEmpty(fileName);
                musicFilesCache.Add(fileName, musicFile);
            }
            return musicFile;
        }

        public MusicFile CreateFromMultiple(IEnumerable<MusicFile> musicFiles)
        {
            return new MockMusicFile(new MusicMetadata(new TimeSpan(0, 3, 33), 320000), null)
            {
                SharedMusicFiles = musicFiles.ToArray()
            };
        }

        public void ApplyChanges(MusicFile musicFile)
        {
            if (ApplyChangesAction != null)
            {
                ApplyChangesAction(musicFile);
            }
        }

        public Task SaveChangesAsync(MusicFile musicFile)
        {
            if (SaveChangesAsyncAction != null)
            {
                return SaveChangesAsyncAction(musicFile);
            }
            return Task.FromResult((object)null);
        }
    }
}
