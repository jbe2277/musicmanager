using System.Collections.Generic;
using System.Linq;

namespace Waf.MusicManager.Applications.Data
{
    internal class SequenceEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private static readonly SequenceEqualityComparer<T> defaultInstance = new SequenceEqualityComparer<T>();

        public static SequenceEqualityComparer<T> Default { get { return defaultInstance; } }


        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.SequenceEqual(y);
        }

        public int GetHashCode(IEnumerable<T> sequence)
        {
            if (sequence == null || !sequence.Any()) { return 0; }
            return sequence.Select(x => x == null ? 0 : x.GetHashCode()).Aggregate((hash, next) => hash ^ next);
        }
    }
}
