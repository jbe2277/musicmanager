using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Foundation;
using System.Windows.Input;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    [Export(typeof(IPlayerService)), Export]
    public class PlayerService : Model, IPlayerService
    {
        private ICommand playAllCommand;
        private ICommand playSelectedCommand;
        private ICommand enqueueAllCommand;
        private ICommand enqueueSelectedCommand;
        private ICommand previousCommand;
        private ICommand playPauseCommand;
        private ICommand nextCommand;
        private bool isPlayCommand;
        private MusicFile playingMusicFile;


        public ICommand PlayAllCommand
        {
            get { return playAllCommand; }
            set { SetProperty(ref playAllCommand, value); }
        }

        public ICommand PlaySelectedCommand
        {
            get { return playSelectedCommand; }
            set { SetProperty(ref playSelectedCommand, value); }
        }

        public ICommand EnqueueAllCommand
        {
            get { return enqueueAllCommand; }
            set { SetProperty(ref enqueueAllCommand, value); }
        }

        public ICommand EnqueueSelectedCommand
        {
            get { return enqueueSelectedCommand; }
            set { SetProperty(ref enqueueSelectedCommand, value); }
        }

        public ICommand PreviousCommand
        {
            get { return previousCommand; }
            set { SetProperty(ref previousCommand, value); }
        }

        public ICommand PlayPauseCommand
        {
            get { return playPauseCommand; }
            set { SetProperty(ref playPauseCommand, value); }
        }

        public ICommand NextCommand
        {
            get { return nextCommand; }
            set { SetProperty(ref nextCommand, value); }
        }

        public bool IsPlayCommand
        {
            get { return isPlayCommand; }
            set { SetProperty(ref isPlayCommand, value); }
        }

        public MusicFile PlayingMusicFile
        {
            get { return playingMusicFile; }
            set { SetProperty(ref playingMusicFile, value); }
        }
    }
}
