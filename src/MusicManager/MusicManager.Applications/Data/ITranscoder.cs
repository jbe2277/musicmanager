namespace Waf.MusicManager.Applications.Data;

public interface ITranscoder
{
    Task TranscodeAsync(string sourceFileName, string destinationFileName, uint bitrate, CancellationToken cancellationToken, IProgress<double> progress);
}
