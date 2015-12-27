using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IShellView))]
    public partial class ShellWindow : Window, IShellView
    {
        private readonly Lazy<ShellViewModel> viewModel;

        public ShellWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<ShellViewModel>(() => ViewHelper.GetViewModel<ShellViewModel>(this));
            Loaded += LoadedHandler;

            // Workaround: Need to load both DrawingImages now; otherwise the first one is not shown at the beginning.
            playPauseButton.ImageSource = (ImageSource)FindResource("PlayButtonImage");
            playPauseButton.ImageSource = (ImageSource)FindResource("PauseButtonImage");
        }


        public double VirtualScreenWidth { get { return SystemParameters.VirtualScreenWidth; } }

        public double VirtualScreenHeight { get { return SystemParameters.VirtualScreenHeight; } }

        public bool IsMaximized
        {
            get { return WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }

        private ShellViewModel ViewModel { get { return viewModel.Value; } }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.MediaPlayPause)
            {
                TryExecute(ViewModel.PlayerService.PlayPauseCommand);
                e.Handled = true;
            }
            else if (e.Key == Key.MediaPreviousTrack)
            {
                TryExecute(ViewModel.PlayerService.PreviousCommand);
                e.Handled = true;
            }
            else if (e.Key == Key.MediaNextTrack)
            {
                TryExecute(ViewModel.PlayerService.NextCommand);
                e.Handled = true;
            }
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            ViewModel.ShellService.PropertyChanged += ShellServicePropertyChanged;
            ViewModel.PlayerService.PropertyChanged += PlayerServicePropertyChanged;
            UpdatePlayPauseButton();
        }

        private void PlayerServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPlayerService.IsPlayCommand))
            {
                UpdatePlayPauseButton();
            }
        }

        private void UpdatePlayPauseButton()
        {
            if (ViewModel.PlayerService.IsPlayCommand)
            {
                playPauseButton.ImageSource = (ImageSource)FindResource("PlayButtonImage");
            }
            else
            {
                playPauseButton.ImageSource = (ImageSource)FindResource("PauseButtonImage");
            }
        }

        private void ShellServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IShellService.IsApplicationBusy))
            {
                if (ViewModel.ShellService.IsApplicationBusy)
                {        
                    Mouse.OverrideCursor = Cursors.Wait;    
                }
                else
                {
                    // Delay removing the wait cursor so that the UI has finished its work as well.
                    Dispatcher.InvokeAsync(() => Mouse.OverrideCursor = null, DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private static void TryExecute(ICommand command)
        {
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
