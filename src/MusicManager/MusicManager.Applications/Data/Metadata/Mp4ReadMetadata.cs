using System.Collections.Generic;
using System.Linq;
using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Applications.Data.Metadata
{
    internal class Mp4ReadMetadata : ReadMetadata
    {
        protected override IEnumerable<string> ReadGenre(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return TryParseFromOneItem(base.ReadGenre(properties, customProperties));
        }

        private static IEnumerable<string> TryParseFromOneItem(IEnumerable<string> source)
        {
            // The WinRT API does not support some of the multiple tags for MP4 files.
            return source.Count() == 1 ? StringListConverter.FromString(source.First()) : source.ToArray();
        }
    }
}
