using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class MockSelectionService : ISelectionService
    {
        private ObservableCollection<MusicFileDataModel> innerMusicFiles;
        private ObservableCollection<MusicFileDataModel> selectedMusicFiles;
        private ReadOnlyObservableList<MusicFileDataModel> musicFiles;


        public MockSelectionService()
        {
            this.innerMusicFiles = new ObservableCollection<MusicFileDataModel>();
            this.selectedMusicFiles = new ObservableCollection<MusicFileDataModel>();
            this.musicFiles = new ReadOnlyObservableList<MusicFileDataModel>(innerMusicFiles);
        }


        public IReadOnlyObservableList<MusicFileDataModel> MusicFiles { get { return musicFiles; } }

        public IList<MusicFileDataModel> SelectedMusicFiles { get { return selectedMusicFiles; } }
        

        public void SetMusicFiles(IEnumerable<MusicFileDataModel> musicFiles)
        {
            innerMusicFiles.Clear();
            musicFiles.ToList().ForEach(x => innerMusicFiles.Add(x));
        }

        public void SetSelectedMusicFiles(IEnumerable<MusicFileDataModel> selectedMusicFiles)
        {
            this.selectedMusicFiles.Clear();
            selectedMusicFiles.ToList().ForEach(x => this.selectedMusicFiles.Add(x));
        }
    }
}
