using System.ComponentModel.Composition;
using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services
{
    [Export, Export(typeof(IClipboardService))]
    public class MockClipboardService : IClipboardService
    {
        public Action<string>? SetTextAction { get; set; }

        public void SetText(string text) => SetTextAction?.Invoke(text);
    }
}
