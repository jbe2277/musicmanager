using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;

namespace Test.MusicManager.Applications.Views
{
    [Export, Export(typeof(IShellView))]
    public class MockShellView : MockView, IShellView
    {
        public double VirtualScreenWidth { get; set; }
        
        public double VirtualScreenHeight { get; set; }

        public double Left { get; set; }
        
        public double Top { get; set; }
        
        public double Width { get; set; }
        
        public double Height { get; set; }
        
        public bool IsMaximized { get; set; }

        public bool IsVisible { get; private set; }


        public event CancelEventHandler Closing;
        
        public event EventHandler Closed;

        
        public void Show()
        {
            IsVisible = true;
        }

        public void Close()
        {
            OnClosed(EventArgs.Empty);
            IsVisible = false;
        }

        public void SetNAForLocationAndSize()
        {
            Top = double.NaN;
            Left = double.NaN;
            Width = double.NaN;
            Height = double.NaN;
        }

        public void RaiseClosing(CancelEventArgs e)
        {
            Closing?.Invoke(this, e);
        }

        protected void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }
    }
}
