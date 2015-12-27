using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export, Export(typeof(IMusicPropertiesService))]
    internal class MusicPropertiesController : IMusicPropertiesService
    {
        private readonly IShellService shellService;
        private readonly IMusicFileContext musicFileContext;
        private readonly ISelectionService selectionService;
        private readonly Lazy<MusicPropertiesViewModel> musicPropertiesViewModel;
        private readonly HashSet<MusicFile> musicFilesToSaveAfterPlaying;
        private TaskCompletionSource<object> allFilesSavedCompletion;
        
        
        [ImportingConstructor]
        public MusicPropertiesController(IShellService shellService, IMusicFileContext musicFileContext, ISelectionService selectionService, Lazy<MusicPropertiesViewModel> musicPropertiesViewModel)
        {
            this.shellService = shellService;
            this.musicFileContext = musicFileContext;
            this.selectionService = selectionService;
            this.musicPropertiesViewModel = musicPropertiesViewModel;
            this.musicFilesToSaveAfterPlaying = new HashSet<MusicFile>();
        }


        public PlaylistManager PlaylistManager { get; set; }

        private MusicPropertiesViewModel MusicPropertiesViewModel { get { return musicPropertiesViewModel.Value; } }


        public void Initialize()
        {
            PlaylistManager.PropertyChanged += PlaylistManagerPropertyChanged;
            ((INotifyCollectionChanged)selectionService.SelectedMusicFiles).CollectionChanged += SelectedMusicFilesCollectionChanged;
            shellService.MusicPropertiesView = MusicPropertiesViewModel.View;
        }

        public void Shutdown()
        {
            var task = SaveCurrentSelectedFileAsync();
            shellService.AddTaskToCompleteBeforeShutdown(task);

            if (musicFilesToSaveAfterPlaying.Any())
            {
                allFilesSavedCompletion = new TaskCompletionSource<object>();
                shellService.AddTaskToCompleteBeforeShutdown(allFilesSavedCompletion.Task);
            }
        }

        public void SelectMusicFiles(IReadOnlyList<MusicFile> musicFiles)
        {
            // Do not wait for the operation to complete. Continue immediately.
            SaveCurrentSelectedFileAsync().IgnoreResult();
            
            if (musicFiles.Count() <= 1)
            {
                MusicPropertiesViewModel.MusicFile = musicFiles.FirstOrDefault();
            }
            else
            {
                MusicPropertiesViewModel.MusicFile = musicFileContext.CreateFromMultiple(musicFiles);
            }
        }

        private Task SaveCurrentSelectedFileAsync()
        {
            musicFileContext.ApplyChanges(MusicPropertiesViewModel.MusicFile);
            return SaveChangesAsync(MusicPropertiesViewModel.MusicFile);
        }

        private async Task SaveMusicFilesToSaveAfterPlayingAsync()
        {
            foreach (var musicFile in musicFilesToSaveAfterPlaying.ToArray())
            {
                await SaveChangesAsync(musicFile);
            }
        }

        private async void PlaylistManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentItem")
            {
                await SaveMusicFilesToSaveAfterPlayingAsync();
            }
        }

        private async Task SaveChangesAsync(MusicFile musicFile)
        {
            if (musicFile == null)
            {
                return;
            }
            IReadOnlyCollection<MusicFile> allFilesToSave;
            if (musicFile.SharedMusicFiles.Any())
            {
                allFilesToSave = musicFile.SharedMusicFiles;
            }
            else
            {
                allFilesToSave = new[] { musicFile };
            }

            // Filter out the music file that is currently playing
            var playingMusicFile = PlaylistManager.CurrentItem != null ? PlaylistManager.CurrentItem.MusicFile : null;
            var filesToSave = allFilesToSave.Except(new[] { playingMusicFile }).ToArray();
            foreach (var x in allFilesToSave.Intersect(new[] { playingMusicFile })) { musicFilesToSaveAfterPlaying.Add(x); }
            
            if (!filesToSave.Any())
            {
                return;
            }
            var tasks = filesToSave.Select(x => musicFileContext.SaveChangesAsync(x)).ToArray();

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Logger.Error("SaveChangesAsync: {0}", ex);
                if (filesToSave.Count() == 1)
                {
                    shellService.ShowError(ex, Resources.CouldNotSaveFile, filesToSave.First().FileName);
                }
                else
                {
                    shellService.ShowError(ex, Resources.CouldNotSaveFiles);
                }
            }
            finally
            {
                RemoveMusicFilesToSaveAfterPlaying(filesToSave);
            }
        }

        private void RemoveMusicFilesToSaveAfterPlaying(IEnumerable<MusicFile> musicFiles)
        {
            foreach (var x in musicFiles) { musicFilesToSaveAfterPlaying.Remove(x); }
            
            if (allFilesSavedCompletion != null && !musicFilesToSaveAfterPlaying.Any())
            {
                allFilesSavedCompletion.SetResult(null);
            }
        }

        private void SelectedMusicFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectMusicFiles(selectionService.SelectedMusicFiles.Select(x => x.MusicFile).ToArray());
        }
    }
}
