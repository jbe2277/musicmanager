using System.IO;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.Services;

internal class EnvironmentService : IEnvironmentService
{
    private static readonly Lazy<string> appDataPath = new(() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.ProductName));
    private static readonly Lazy<string> profilePath = new(() => Path.Combine(AppDataPath, "ProfileOptimization"));
    private static readonly Lazy<string> logPath = new(() => Path.Combine(AppDataPath, "Log"));
    private readonly Lazy<IReadOnlyList<string>> musicFilesToLoad;
        
    public EnvironmentService()
    {
        musicFilesToLoad = new(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
        MusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        PublicMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
    }

    public static string AppDataPath => appDataPath.Value;

    public static string ProfilePath => profilePath.Value;

    public static string LogPath => logPath.Value;

    public IReadOnlyList<string> MusicFilesToLoad => musicFilesToLoad.Value;

    public string MusicPath { get; }

    public string PublicMusicPath { get; }
}
