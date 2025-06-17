using System.Windows;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.Services;

internal class ClipboardService : IClipboardService
{
    public void SetText(string text) => Clipboard.SetText(text);
}
