using System.ComponentModel.Composition;
using System.IO;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data
{
    [Export, Export(typeof(IFileSystemWatcherService)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
    public class MockFileSystemWatcherService : IFileSystemWatcherService
    {
        public NotifyFilters NotifyFilter { get; set; }

        public string Path { get; set; } = "";
        
        public bool EnableRaisingEvents { get; set; }
        
        public event FileSystemEventHandler? Created;

        public event RenamedEventHandler? Renamed;

        public event FileSystemEventHandler? Deleted;

        public void RaiseCreated(FileSystemEventArgs e) => OnCreated(e);

        public void RaiseRenamed(RenamedEventArgs e) => OnRenamed(e);

        public void RaiseDeleted(FileSystemEventArgs e) => OnDeleted(e);

        protected virtual void OnCreated(FileSystemEventArgs e) => Created?.Invoke(this, e);

        protected virtual void OnRenamed(RenamedEventArgs e) => Renamed?.Invoke(this, e);

        protected virtual void OnDeleted(FileSystemEventArgs e) => Deleted?.Invoke(this, e);
    }
}
