using System.Collections.Generic;
using System.Waf.Foundation;
using Waf.MusicManager.Domain;

namespace Waf.MusicManager.Applications.DataModels
{
    public class FolderBrowserDataModel : Model
    {
        private string userPath;
        private string currentPath;
        private IReadOnlyList<FolderItem> subDirectories;
        private FolderItem selectedSubDirectory;


        public string UserPath
        {
            get { return userPath; }
            set { SetProperty(ref userPath, value ?? ""); }
        }

        public string CurrentPath
        {
            get { return currentPath; }
            set { SetProperty(ref currentPath, value ?? ""); }
        }

        public IReadOnlyList<FolderItem> SubDirectories
        {
            get { return subDirectories; }
            set { SetProperty(ref subDirectories, value); }
        }

        public FolderItem SelectedSubDirectory
        {
            get { return selectedSubDirectory; }
            set { SetProperty(ref selectedSubDirectory, value); }
        }
    }
}
