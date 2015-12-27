using System;
using System.ComponentModel;
using System.Windows.Controls;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SampleShellViewModel : ShellViewModel
    {
        public SampleShellViewModel() : base(new MockShellView(), new MockShellService(), null)
        {
            ShellService.PlaylistView = new Control();
            IsPlaylistViewVisible = true;
            ShowError(null, "A very long error message that does not fit in a default size window. Thus, the text block should trim the text and show ellipsis."
                + " And more text so that it is really too long.");
        }

        private class MockShellView : MockView, IShellView
        {
            public double VirtualScreenWidth { get { return 0; } }

            public double VirtualScreenHeight { get { return 0; } }
            
            public double Left { get; set; }
            
            public double Top { get; set; }
            
            public double Width { get; set; }
            
            public double Height { get; set; }
            
            public bool IsMaximized { get; set; }


            public event CancelEventHandler Closing;

            public event EventHandler Closed;

            
            public void Show()
            {
            }

            public void Close()
            {
            }

            protected virtual void OnClosing(CancelEventArgs e)
            {
                if (Closing != null) { Closing(this, e); }
            }

            protected virtual void OnClosed(EventArgs e)
            {
                if (Closed != null) { Closed(this, e); }
            }
        }
    }
}
