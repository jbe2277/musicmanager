﻿namespace Waf.MusicManager.Presentation
{
    public class ListSortComparer<T> : IComparer<T>
    {
        private readonly Comparison<T?> comparison;
        private readonly ListSortDirection sortDirection;

        public ListSortComparer(Comparison<T?> comparison, ListSortDirection sortDirection)
        {
            this.comparison = comparison;
            this.sortDirection = sortDirection;
        }

        public int Compare(T? x, T? y) => sortDirection == ListSortDirection.Ascending ? comparison(x, y) : comparison(y, x);
    }
}
