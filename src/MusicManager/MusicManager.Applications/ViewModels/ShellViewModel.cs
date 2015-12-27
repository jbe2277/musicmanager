using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class ShellViewModel : ViewModel<IShellView>
    {
        private readonly IShellService shellService;
        private readonly IPlayerService playerService;
        private readonly AppSettings settings;
        private readonly ObservableCollection<Tuple<Exception, string>> errors;
        private readonly DelegateCommand exitCommand;
        private readonly DelegateCommand closeErrorCommand;
        private readonly DelegateCommand garbageCollectorCommand;
        private object detailsView;
        

        [ImportingConstructor]
        public ShellViewModel(IShellView view, IShellService shellService, IPlayerService playerService)
            : base(view)
        {
            this.shellService = shellService;
            this.playerService = playerService;
            this.settings = shellService.Settings;
            this.errors = new ObservableCollection<Tuple<Exception, string>>();
            this.exitCommand = new DelegateCommand(Close);
            this.closeErrorCommand = new DelegateCommand(CloseError);
            this.garbageCollectorCommand = new DelegateCommand(GC.Collect);

            errors.CollectionChanged += ErrorsCollectionChanged;
            view.Closed += ViewClosed;

            // Restore the window size when the values are valid.
            if (settings.Left >= 0 && settings.Top >= 0 && settings.Width > 0 && settings.Height > 0
                && settings.Left + settings.Width <= view.VirtualScreenWidth
                && settings.Top + settings.Height <= view.VirtualScreenHeight)
            {
                view.Left = settings.Left;
                view.Top = settings.Top;
                view.Height = settings.Height;
                view.Width = settings.Width;
            }
            view.IsMaximized = settings.IsMaximized;
        }


        public string Title { get { return ApplicationInfo.ProductName; } }

        public IShellService ShellService { get { return shellService; } }

        public IPlayerService PlayerService { get { return playerService; } }

        public IReadOnlyList<Tuple<Exception, string>> Errors { get { return errors; } }

        public Tuple<Exception, string> LastError { get { return errors.LastOrDefault(); } }

        public ICommand ExitCommand { get { return exitCommand; } }

        public ICommand CloseErrorCommand { get { return closeErrorCommand; } }

        public ICommand GarbageCollectorCommand { get { return garbageCollectorCommand; } }

        public object DetailsView
        {
            get { return detailsView; }
            private set { SetProperty(ref detailsView, value); }
        }

        public bool IsMusicPropertiesViewVisible
        {
            get { return DetailsView == ShellService.MusicPropertiesView; }
            set { if (value) { DetailsView = ShellService.MusicPropertiesView; } }
        }

        public bool IsPlaylistViewVisible
        {
            get { return DetailsView == ShellService.PlaylistView; }
            set { if (value) { DetailsView = ShellService.PlaylistView; } }
        }

        public bool IsTranscodingListViewVisible
        {
            get { return ShellService.TranscodingListView.IsValueCreated && DetailsView == ShellService.TranscodingListView.Value; }
            set { if (value) { DetailsView = ShellService.TranscodingListView.Value; } }
        }


        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        public void ShowError(Exception exception, string message)
        {
            errors.Add(new Tuple<Exception, string>(exception, message));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == "DetailsView")
            {
                RaisePropertyChanged("IsMusicPropertiesViewVisible");
                RaisePropertyChanged("IsPlaylistViewVisible");
                RaisePropertyChanged("IsTranscodingListViewVisible");
            }
        }

        private void CloseError()
        {
            if (errors.Any())
            {
                errors.RemoveAt(errors.Count - 1);
            }
        }

        private void ErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("LastError");
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            settings.Left = ViewCore.Left;
            settings.Top = ViewCore.Top;
            settings.Height = ViewCore.Height;
            settings.Width = ViewCore.Width;
            settings.IsMaximized = ViewCore.IsMaximized;
        }
    }
}
