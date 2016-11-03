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
        private SynchronizingCollection<MusicFileDataModel, MusicFile> musicFileDataModels;


        [ImportingConstructor]
        public SelectionService()
        {
            SelectedMusicFiles = new ObservableCollection<MusicFileDataModel>();
        }

        
        public IReadOnlyObservableList<MusicFileDataModel> MusicFiles => musicFileDataModels;

        public IList<MusicFileDataModel> SelectedMusicFiles { get; }


        public void Initialize(IEnumerable<MusicFile> musicFiles)
        {
            musicFileDataModels = new SynchronizingCollection<MusicFileDataModel, MusicFile>(musicFiles, x => new MusicFileDataModel(x));
        }
    }
}
