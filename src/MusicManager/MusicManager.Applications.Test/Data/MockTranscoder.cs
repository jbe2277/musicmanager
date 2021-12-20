using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Data;

namespace Test.MusicManager.Applications.Data
{
    [Export, Export(typeof(ITranscoder)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
    public class MockTranscoder : ITranscoder
    {
        public Func<string, string, uint, CancellationToken, IProgress<double>, Task>? TranscodeAsyncAction { get; set; }
        
        public Task TranscodeAsync(string sourceFileName, string destinationFileName, uint bitrate, CancellationToken cancellationToken, IProgress<double> progress)
        {
            return TranscodeAsyncAction?.Invoke(sourceFileName, destinationFileName, bitrate, cancellationToken, progress) ?? Task.CompletedTask;
        }
    }
}
