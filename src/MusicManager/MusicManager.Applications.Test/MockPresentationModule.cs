using Autofac;
using System.Waf.Applications.Services;
using System.Waf.UnitTesting.Mocks;
using Test.MusicManager.Applications.Services;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;

namespace Test.MusicManager.Applications;

public class MockPresentationModule(bool useMock = true) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MockFileDialogService>().As<IFileDialogService>().AsSelf().SingleInstance();
        builder.RegisterType<MockMessageService>().As<IMessageService>().AsSelf().SingleInstance();
        builder.RegisterType<MockSettingsService>().As<ISettingsService>().AsSelf().SingleInstance();

        builder.RegisterType<MockClipboardService>().As<IClipboardService>().AsSelf().SingleInstance();
        builder.RegisterType<MockEnvironmentService>().As<IEnvironmentService>().AsSelf().SingleInstance();

        if (useMock)
        {
            builder.RegisterType<MockFileService>().As<IFileService>().AsSelf().SingleInstance();
            builder.RegisterType<MockFileSystemWatcherService>().As<IFileSystemWatcherService>().AsSelf().SingleInstance();
            builder.RegisterType<MockMusicFileContext>().As<IMusicFileContext>().AsSelf().SingleInstance();
            builder.RegisterType<MockTranscoder>().As<ITranscoder>().AsSelf().SingleInstance();
        }

        builder.RegisterType<MockInfoView>().As<IInfoView>();
        builder.RegisterType<MockManagerView>().As<IManagerView>().AsSelf().SingleInstance();
        builder.RegisterType<MockMusicPropertiesView>().As<IMusicPropertiesView>().AsSelf().SingleInstance();
        builder.RegisterType<MockPlayerView>().As<IPlayerView>().AsSelf().SingleInstance();
        builder.RegisterType<MockPlaylistView>().As<IPlaylistView>().AsSelf().SingleInstance();
        builder.RegisterType<MockShellView>().As<IShellView>().AsSelf().SingleInstance();
        builder.RegisterType<MockTranscodingListView>().As<ITranscodingListView>().AsSelf().SingleInstance();
    }
}