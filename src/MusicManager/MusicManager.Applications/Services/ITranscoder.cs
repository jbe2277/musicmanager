namespace Waf.MusicManager.Applications.Services;

public interface ITranscoder
{
    Task TranscodeAsync(string sourceFileName, string destinationFileName, uint bitrate, CancellationToken cancellationToken, IProgress<double> progress);
}
