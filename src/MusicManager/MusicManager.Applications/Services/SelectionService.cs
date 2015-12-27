using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    [Export, Export(typeof(ISelectionService))]
    internal class SelectionService : ISelectionService
    {
        private readonly ObservableCollection<MusicFileDataModel> selectedMusicFileDataModels;
        private SynchronizingCollection<MusicFileDataModel, MusicFile> musicFileDataModels;


        [ImportingConstructor]
        public SelectionService()
        {
            this.selectedMusicFileDataModels = new ObservableCollection<MusicFileDataModel>();
        }

        
        public IReadOnlyObservableList<MusicFileDataModel> MusicFiles { get { return musicFileDataModels; } }

        public IList<MusicFileDataModel> SelectedMusicFiles { get { return selectedMusicFileDataModels; } }


        public void Initialize(IEnumerable<MusicFile> musicFiles)
        {
            musicFileDataModels = new SynchronizingCollection<MusicFileDataModel, MusicFile>(musicFiles, x => new MusicFileDataModel(x));
        }
    }
}
