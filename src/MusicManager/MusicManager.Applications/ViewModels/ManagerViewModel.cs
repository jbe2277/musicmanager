using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain;
using Windows.Storage;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class ManagerViewModel : ViewModel<IManagerView>
    {
        private readonly Lazy<ISelectionService> selectionService;
        private readonly IManagerStatusService managerStatusService;
        private readonly IPlayerService playerService;
        private readonly ITranscodingService transcodingService;
        private readonly FolderBrowserDataModel folderBrowser;
        private readonly SearchFilterDataModel searchFilter;
        private readonly DelegateCommand clearSearchCommand;
        private MusicFileDataModel selectedMusicFile;
        private ICommand updateSubDirectoriesCommand;
        private ICommand navigateDirectoryUpCommand;
        private ICommand navigateHomeCommand;
        private ICommand navigatePublicHomeCommand;
        private ICommand loadRecursiveCommand;
        private ICommand navigateToSelectedSubDirectoryCommand;
        private ICommand showMusicPropertiesCommand;
        
        
        [ImportingConstructor]
        public ManagerViewModel(IManagerView view, Lazy<ISelectionService> selectionService, IManagerStatusService managerStatusService, 
            IPlayerService playerService, ITranscodingService transcodingService)
            : base(view)
        {
            this.selectionService = selectionService;
            this.managerStatusService = managerStatusService;
            this.playerService = playerService;
            this.transcodingService = transcodingService;
            this.folderBrowser = new FolderBrowserDataModel();
            this.searchFilter = new SearchFilterDataModel();
            this.clearSearchCommand = new DelegateCommand(ClearSearch);
        }


        public ISelectionService SelectionService { get { return selectionService.Value; } }
        
        public IManagerStatusService ManagerStatusService { get { return managerStatusService; } }

        public IPlayerService PlayerService { get { return playerService; } }

        public ITranscodingService TranscodingService { get { return transcodingService; } }

        public FolderBrowserDataModel FolderBrowser { get { return folderBrowser; } }

        public SearchFilterDataModel SearchFilter { get { return searchFilter; } }

        public MusicFileDataModel SelectedMusicFile
        {
            get { return selectedMusicFile; }
            set { SetProperty(ref selectedMusicFile, value); }
        }

        public ICommand UpdateSubDirectoriesCommand
        {
            get { return updateSubDirectoriesCommand; }
            set { SetProperty(ref updateSubDirectoriesCommand, value); }
        }

        public ICommand NavigateDirectoryUpCommand
        { 
            get { return navigateDirectoryUpCommand; }
            set { SetProperty(ref navigateDirectoryUpCommand, value); }
        }

        public ICommand NavigateHomeCommand
        {
            get { return navigateHomeCommand; }
            set { SetProperty(ref navigateHomeCommand, value); }
        }

        public ICommand NavigatePublicHomeCommand
        {
            get { return navigatePublicHomeCommand; }
            set { SetProperty(ref navigatePublicHomeCommand, value); }
        }

        public ICommand LoadRecursiveCommand
        {
            get { return loadRecursiveCommand; }
            set { SetProperty(ref loadRecursiveCommand, value); }
        }

        public ICommand NavigateToSelectedSubDirectoryCommand
        {
            get { return navigateToSelectedSubDirectoryCommand; }
            set { SetProperty(ref navigateToSelectedSubDirectoryCommand, value); }
        }

        public ICommand ClearSearchCommand { get { return clearSearchCommand; } }

        public ICommand ShowMusicPropertiesCommand
        {
            get { return showMusicPropertiesCommand; }
            set { SetProperty(ref showMusicPropertiesCommand, value); }
        }


        private void ClearSearch()
        {
            SearchFilter.Clear();
        }
    }
}
