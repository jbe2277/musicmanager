using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

public class MockTranscoder : ITranscoder
{
    public Func<string, string, uint, CancellationToken, IProgress<double>, Task>? TranscodeAsyncAction { get; set; }
        
    public Task TranscodeAsync(string sourceFileName, string destinationFileName, uint bitrate, CancellationToken cancellationToken, IProgress<double> progress)
    {
        return TranscodeAsyncAction?.Invoke(sourceFileName, destinationFileName, bitrate, cancellationToken, progress) ?? Task.CompletedTask;
    }
}
