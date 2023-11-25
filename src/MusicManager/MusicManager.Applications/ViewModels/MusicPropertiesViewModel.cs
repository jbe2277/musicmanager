using System.ComponentModel.Composition;
using System.IO;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.ViewModels;

[Export]
public class MusicPropertiesViewModel : ViewModel<IMusicPropertiesView>
{
    private readonly IClipboardService clipboardService;
    private readonly DelegateCommand autoFillFromFileNameCommand;
    private MusicFile? musicFile;
    private IWeakEventProxy? musicFilePropertyChangedProxy;
    private IWeakEventProxy? metadataPropertyChangedProxy;

    [ImportingConstructor]
    public MusicPropertiesViewModel(IMusicPropertiesView view, IClipboardService clipboardService) : base(view)
    {
        this.clipboardService = clipboardService;
        CopyFileNameCommand = new DelegateCommand(CopyFileNameToClipboard);
        autoFillFromFileNameCommand = new(AutoFillFromFileName, CanAutoFillFromFileName);
    }

    public ICommand CopyFileNameCommand { get; }

    public ICommand AutoFillFromFileNameCommand => autoFillFromFileNameCommand;

    public MusicFile? MusicFile
    {
        get => musicFile;
        set
        {
            if (musicFile == value) return;
            if (musicFile != null)
            {
                WeakEvent.TryRemove(ref musicFilePropertyChangedProxy);
                if (musicFile.IsMetadataLoaded) WeakEvent.TryRemove(ref metadataPropertyChangedProxy);
            }
            musicFile = value;
            if (musicFile != null)
            {
                musicFilePropertyChangedProxy = WeakEvent.PropertyChanged.Add(musicFile, MusicFilePropertyChanged);
                MetadataLoaded();
            }
            RaisePropertyChanged();
            autoFillFromFileNameCommand.RaiseCanExecuteChanged();
        }
    }

    private void CopyFileNameToClipboard()
    {
        if (MusicFile?.FileName != null) clipboardService.SetText(Path.GetFileNameWithoutExtension(MusicFile.FileName));
    }

    private bool CanAutoFillFromFileName()
    {
        return MusicFile != null && MusicFile.IsMetadataLoaded && MusicFile.Metadata.IsSupported && string.IsNullOrEmpty(MusicFile.Metadata.Title) && !MusicFile.Metadata.Artists.Any();
    }

    private void AutoFillFromFileName()
    {
        var fileName = Path.GetFileNameWithoutExtension(MusicFile?.FileName) ?? throw new InvalidOperationException("MusicFile?.FileName must not be null");
        var metadata = fileName.Split(['-'], 2).Select(x => x.Trim()).ToArray();
        if (metadata.Length == 2)
        {
            MusicFile!.Metadata!.Artists = [ metadata[0] ];
            MusicFile.Metadata.Title = metadata[1];
        }
        else
        {
            MusicFile!.Metadata!.Title = fileName;
        }
    }

    private void MetadataLoaded()
    {
        if (MusicFile?.IsMetadataLoaded != true) return;
        metadataPropertyChangedProxy = WeakEvent.PropertyChanged.Add(MusicFile.Metadata!, MetadataPropertyChanged);
        autoFillFromFileNameCommand.RaiseCanExecuteChanged();
    }

    private void MusicFilePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MusicFile.IsMetadataLoaded)) MetadataLoaded();
    }

    private void MetadataPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MusicMetadata.Title) or nameof(MusicMetadata.Artists)) autoFillFromFileNameCommand.RaiseCanExecuteChanged();
    }
}
