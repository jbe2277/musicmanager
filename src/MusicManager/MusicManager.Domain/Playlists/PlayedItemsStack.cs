namespace Waf.MusicManager.Domain.Playlists;

internal class PlayedItemsStack<T>(int capacity)
{
    private readonly int capacity = capacity;
    private readonly LinkedList<T> playlistItems = [];

    public int Count => playlistItems.Count;
        
    public T Pop()
    { 
        var result = (playlistItems.Last ?? throw new InvalidOperationException("PlayedItemsStack is empty.")).Value;
        playlistItems.RemoveLast();
        return result;
    }

    public bool Contains(T item) => playlistItems.Contains(item);

    public void Add(T item)
    {
        if (playlistItems.Count >= capacity) playlistItems.RemoveFirst();
        playlistItems.AddLast(item);
    }

    public void RemoveAll(T item)
    {
        bool removed;
        do
        {
            removed = playlistItems.Remove(item);
        }
        while (removed);
    }

    public void Clear() => playlistItems.Clear();
}
