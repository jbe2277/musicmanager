using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Waf.MusicManager.Applications.Services
{
    public static class MusicTitleHelper
    {
        public static string GetTitleText(string fileName, IEnumerable<string> artists, string title)
        {
            artists = artists ?? new string[0];
            var result = string.IsNullOrEmpty(title) && !artists.Any() ? Path.GetFileNameWithoutExtension(fileName) : title;
            return result ?? "";
        }
    }
}
