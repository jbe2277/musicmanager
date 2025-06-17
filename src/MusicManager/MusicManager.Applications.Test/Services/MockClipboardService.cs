using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

public class MockClipboardService : IClipboardService
{
    public Action<string>? SetTextAction { get; set; }

    public void SetText(string text) => SetTextAction?.Invoke(text);
}
