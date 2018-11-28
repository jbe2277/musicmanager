using System.Collections.Generic;
using System.Linq;

namespace Waf.MusicManager.Domain.Playlists
{
    internal static class StatisticsHelper
    {
        public static double TruncatedMean(IEnumerable<double> values, double truncateRate)
        {
            if (!values.Any()) return 0;

            int truncateCount = (int)(values.Count() * truncateRate);
            var truncatedValues = values.OrderBy(x => x).Skip(truncateCount).Take(values.Count() - 2 * truncateCount);
            double truncatedMean = truncatedValues.Average();
            return truncatedMean;
        }
    }
}
