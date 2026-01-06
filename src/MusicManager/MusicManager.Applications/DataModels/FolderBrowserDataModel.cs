using System.Waf.Foundation;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Applications.DataModels;

public class FolderBrowserDataModel : Model
{
    public string UserPath{ get; set => SetProperty(ref field, value ?? ""); } = "";

    public string CurrentPath { get; set => SetProperty(ref field, value ?? ""); } = null!;

    public IReadOnlyList<FolderItem> SubDirectories { get; set => SetProperty(ref field, value); } = [];

    public FolderItem? SelectedSubDirectory { get; set => SetProperty(ref field, value); }
}
