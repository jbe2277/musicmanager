namespace Waf.MusicManager.Domain.Playlists;

internal class RandomService : IRandomService
{
    public int NextRandomNumber(int maxValue)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(maxValue, int.MaxValue);
        return new Random().Next(maxValue + 1);
    }
}
