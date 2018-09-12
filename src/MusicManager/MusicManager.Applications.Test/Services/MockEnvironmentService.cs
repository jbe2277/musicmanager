using System.Collections.Generic;
using System.ComponentModel.Composition;
using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services
{
    [Export, Export(typeof(IEnvironmentService))]
    public class MockEnvironmentService : IEnvironmentService
    {
        public MockEnvironmentService()
        {
            MusicFilesToLoad = new string[0];
        }
        

        public IReadOnlyList<string> MusicFilesToLoad { get; set; }
        
        public string MusicPath { get; set; }
        
        public string PublicMusicPath { get; set; }
    }
}
