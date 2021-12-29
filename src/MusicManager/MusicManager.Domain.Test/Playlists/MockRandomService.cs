using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Domain.MusicFiles
{
    public class MockRandomService : IRandomService
    {
        public Func<int, int>? NextRandomNumberStub { get; set; }
        
        public int NextRandomNumber(int maxValue) => NextRandomNumberStub?.Invoke(maxValue) ?? throw new InvalidOperationException(nameof(NextRandomNumberStub) + " is not set");
    }
}
