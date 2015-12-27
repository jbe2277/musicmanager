using System.Linq;

namespace Waf.MusicManager.Applications.DataModels
{
    public class FolderItem
    {
        private readonly string path;
        private readonly string displayName;

        public FolderItem(string path, string displayName)
        {
            this.path = path;
            this.displayName = displayName;
        }

        public string Path { get { return path; } }

        public string DisplayName { get { return displayName; } }
    }
}
