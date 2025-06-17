using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

public class MockEnvironmentService : IEnvironmentService
{
    public IReadOnlyList<string> MusicFilesToLoad { get; set; } = [];

    public string MusicPath { get; set; } = "";

    public string PublicMusicPath { get; set; } = "";
}
