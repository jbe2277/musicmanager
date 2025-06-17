using Autofac;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;

namespace Waf.MusicManager.Applications;

public class ApplicationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ManagerController>().AsSelf().SingleInstance();
        builder.RegisterType<ModuleController>().As<IModuleController>().AsSelf().SingleInstance();
        builder.RegisterType<MusicPropertiesController>().As<IMusicPropertiesService>().AsSelf().SingleInstance();
        builder.RegisterType<PlayerController>().AsSelf().SingleInstance();
        builder.RegisterType<PlaylistController>().As<IPlaylistService>().AsSelf().SingleInstance();
        builder.RegisterType<TranscodingController>().AsSelf().SingleInstance();

        builder.RegisterType<ManagerStatusService>().As<IManagerStatusService>().AsSelf().SingleInstance();
        builder.RegisterType<PlayerService>().As<IPlayerService>().AsSelf().SingleInstance();
        builder.RegisterType<SelectionService>().As<ISelectionService>().AsSelf().SingleInstance();
        builder.RegisterType<ShellService>().As<IShellService>().AsSelf().SingleInstance();
        builder.RegisterType<TranscodingService>().As<ITranscodingService>().AsSelf().SingleInstance();

        builder.RegisterType<InfoViewModel>().AsSelf();
        builder.RegisterType<ManagerViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<MusicPropertiesViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<PlayerViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<PlaylistViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<ShellViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<TranscodingListViewModel>().AsSelf().SingleInstance();
    }
}