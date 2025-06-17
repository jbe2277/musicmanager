namespace Waf.MusicManager.Presentation.Services;

public class ListSortComparer<T>(Comparison<T?> comparison, ListSortDirection sortDirection) : IComparer<T>
{
    public int Compare(T? x, T? y) => sortDirection == ListSortDirection.Ascending ? comparison(x, y) : comparison(y, x);
}
