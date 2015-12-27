using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class TranscodingListViewModel : ViewModel<ITranscodingListView>
    {
        private readonly ITranscodingService transcodingService;
        private readonly ObservableCollection<TranscodeItem> selectedTranscodeItems;
        private TranscodingManager transcodingManager;

        
        [ImportingConstructor]
        public TranscodingListViewModel(ITranscodingListView view, ITranscodingService transcodingService) : base(view)
        {
            this.transcodingService = transcodingService;
            this.selectedTranscodeItems = new ObservableCollection<TranscodeItem>();
        }


        public ITranscodingService TranscodingService { get { return transcodingService; } }

        public IList<TranscodeItem> SelectedTranscodeItems { get { return selectedTranscodeItems; } }

        public TranscodingManager TranscodingManager
        {
            get { return transcodingManager; }
            set { SetProperty(ref transcodingManager, value); }
        }

        public Action<int, IEnumerable<string>> InsertFilesAction { get; set; }

        public Action<int, IEnumerable<MusicFile>> InsertMusicFilesAction { get; set; }
    }
}
