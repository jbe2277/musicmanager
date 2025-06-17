using Autofac;
using System.Waf.Applications.Services;
using System.Waf.Presentation.Services;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Presentation.Services;
using Waf.MusicManager.Presentation.Views;

namespace Waf.MusicManager.Presentation;

public class PresentationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FileDialogService>().As<IFileDialogService>().AsSelf().SingleInstance();
        builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance();
        builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();

        builder.RegisterType<ClipboardService>().As<IClipboardService>().AsSelf().SingleInstance();
        builder.RegisterType<EnvironmentService>().As<IEnvironmentService>().AsSelf().SingleInstance();
        builder.RegisterType<FileService>().As<IFileService>().AsSelf().SingleInstance();
        builder.RegisterType<FileSystemWatcherService>().As<IFileSystemWatcherService>().AsSelf().SingleInstance();
        builder.RegisterType<MusicFileContext>().As<IMusicFileContext>().AsSelf().SingleInstance();
        builder.RegisterType<Transcoder>().As<ITranscoder>().AsSelf().SingleInstance();

        builder.RegisterType<InfoWindow>().As<IInfoView>();
        builder.RegisterType<ManagerView>().As<IManagerView>().AsSelf().SingleInstance();
        builder.RegisterType<MusicPropertiesView>().As<IMusicPropertiesView>().AsSelf().SingleInstance();
        builder.RegisterType<PlayerView>().As<IPlayerView>().AsSelf().SingleInstance();
        builder.RegisterType<PlaylistView>().As<IPlaylistView>().AsSelf().SingleInstance();
        builder.RegisterType<ShellWindow>().As<IShellView>().AsSelf().SingleInstance();
        builder.RegisterType<TranscodingListView>().As<ITranscodingListView>().AsSelf().SingleInstance();
    }
}