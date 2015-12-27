using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.IO;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.Services
{
    [Export(typeof(IEnvironmentService))]
    internal class EnvironmentService : IEnvironmentService
    {
        private readonly Lazy<IReadOnlyList<string>> musicFilesToLoad;
        private readonly Lazy<string> profilePath;
        private readonly Lazy<string> appSettingsPath;


        public EnvironmentService()
        {
            musicFilesToLoad = new Lazy<IReadOnlyList<string>>(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
            profilePath = new Lazy<string>(() =>
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "ProfileOptimization"));
            appSettingsPath = new Lazy<string>(() => 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "Settings"));
            MusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            PublicMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
        }


        public IReadOnlyList<string> MusicFilesToLoad { get { return musicFilesToLoad.Value; } }

        public string ProfilePath { get { return profilePath.Value; } }

        public string AppSettingsPath { get { return appSettingsPath.Value; } }

        public string MusicPath { get; }

        public string PublicMusicPath { get; }
    }
}
