namespace Waf.MusicManager.Applications.Services;

internal class ManagerStatusService : Model, IManagerStatusService
{
    private bool updatingFilesList;
    private int totalFilesCount = -1;
        
    public bool UpdatingFilesList
    {
        get => updatingFilesList;
        private set => SetProperty(ref updatingFilesList, value);
    }

    public int TotalFilesCount
    {
        get => totalFilesCount;
        private set => SetProperty(ref totalFilesCount, value);
    }

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
