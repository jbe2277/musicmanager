using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Waf.MusicManager.Applications
{
    [DataContract]
    public sealed class PlaylistSettings : IExtensibleDataObject 
    {
        [DataMember(Name = "FileNames")] 
        private readonly List<string> fileNames;


        public PlaylistSettings()
        {
            this.fileNames = new List<string>();
        }


        [DataMember]
        public string LastPlayedFileName { get; set; }

        [DataMember]
        public TimeSpan LastPlayedFilePosition { get; set; }
        
        public IReadOnlyList<string> FileNames { get { return fileNames; } }

        ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }


        public void ReplaceAll(IEnumerable<string> newFileNames)
        {
            fileNames.Clear();
            fileNames.AddRange(newFileNames);
        }
    }
}
