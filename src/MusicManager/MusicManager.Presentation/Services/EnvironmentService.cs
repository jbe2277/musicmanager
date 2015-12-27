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
        private readonly string musicPath;
        private readonly string publicMusicPath;


        public EnvironmentService()
        {
            this.musicFilesToLoad = new Lazy<IReadOnlyList<string>>(() => Environment.GetCommandLineArgs().Skip(1).ToArray());
            this.profilePath = new Lazy<string>(() =>
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "ProfileOptimization"));
            this.appSettingsPath = new Lazy<string>(() => 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.Company, ApplicationInfo.ProductName, "Settings"));
            this.musicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            this.publicMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
        }


        public IReadOnlyList<string> MusicFilesToLoad { get { return musicFilesToLoad.Value; } }

        public string ProfilePath { get { return profilePath.Value; } }

        public string AppSettingsPath { get { return appSettingsPath.Value; } }

        public string MusicPath { get { return musicPath; } }

        public string PublicMusicPath { get { return publicMusicPath; } }
    }
}
