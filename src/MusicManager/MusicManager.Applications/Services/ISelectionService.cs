using System.Collections.Generic;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.DataModels;

namespace Waf.MusicManager.Applications.Services
{
    public interface ISelectionService
    {
        IReadOnlyObservableList<MusicFileDataModel> MusicFiles { get; }

        IList<MusicFileDataModel> SelectedMusicFiles { get; }
    }
}
