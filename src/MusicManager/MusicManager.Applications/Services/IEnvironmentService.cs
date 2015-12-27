using System;
using System.Collections.Generic;

namespace Waf.MusicManager.Applications.Services
{
    public interface IEnvironmentService
    {
        IReadOnlyList<string> MusicFilesToLoad { get; }
        
        string AppSettingsPath { get; }

        string MusicPath { get; }

        string PublicMusicPath { get; }
    }
}
