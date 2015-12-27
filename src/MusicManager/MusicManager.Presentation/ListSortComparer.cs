using System;
using System.Collections;
using System.ComponentModel;

namespace Waf.MusicManager.Presentation
{
    public class ListSortComparer<T> : IComparer
    {
        private readonly Comparison<T> comparison;
        private readonly ListSortDirection sortDirection;


        public ListSortComparer(Comparison<T> comparison, ListSortDirection sortDirection)
        {
            this.comparison = comparison;
            this.sortDirection = sortDirection;
        }


        public int Compare(object x, object y)
        {
            var typedX = (T)x;
            var typedY = (T)y;
            return (sortDirection == ListSortDirection.Ascending) ? comparison(typedX, typedY) : comparison(typedY, typedX);
        }
    }
}
