using System.IO;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.Services;

internal class EnvironmentService : IEnvironmentService
{
    private readonly Lazy<IReadOnlyList<string>> musicFilesToLoad;
        
    public EnvironmentService()
    {
        musicFilesToLoad = new(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
        MusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        PublicMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
    }

    private static string AppDataPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.ProductName);

    public static string LogPath { get; } = Path.Combine(AppDataPath, "Log");

    public IReadOnlyList<string> MusicFilesToLoad => musicFilesToLoad.Value;

    public string MusicPath { get; }

    public string PublicMusicPath { get; }
}
