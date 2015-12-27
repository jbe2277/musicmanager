using System.ComponentModel;
using System.Runtime.Serialization;

namespace Waf.MusicManager.Applications.Properties
{
    [DataContract]
    public sealed class AppSettings : IExtensibleDataObject 
    {
        public AppSettings()
        {
            Initialize();
        }
        
        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public bool IsMaximized { get; set; }

        [DataMember]
        public string CurrentPath { get; set; }

        [DataMember]
        public double Volume { get; set; }

        [DataMember]
        public bool Shuffle { get; set; }

        [DataMember]
        public bool Repeat { get; set; }

        ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }

        private void Initialize()
        {
            Volume = 0.5;
        }
        
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }
    }
}
