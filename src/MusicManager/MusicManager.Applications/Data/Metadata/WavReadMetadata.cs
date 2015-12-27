namespace Waf.MusicManager.Applications.Data.Metadata
{
    internal class WavReadMetadata : ReadMetadata
    {
        protected override bool IsSupported { get { return false; } }
    }
}
