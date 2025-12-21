namespace Waf.MusicManager.Applications.Services;

internal class ManagerStatusService : Model, IManagerStatusService
{
    public bool UpdatingFilesList { get; private set => SetProperty(ref field, value); }

    public int TotalFilesCount { get; private set => SetProperty(ref field, value); } = -1;

    public void StartUpdatingFilesList()
    {
        UpdatingFilesList = true;
        TotalFilesCount = -1;
    }

    public void FinishUpdatingFilesList(int totalFilesCount)
    {
        UpdatingFilesList = false;
        TotalFilesCount = totalFilesCount;
    }
}
