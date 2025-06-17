using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using Test.MusicManager.Applications;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Presentation.Services;

namespace Test.MusicManager.Presentation
{
    /// <summary>Integration tests (uses real service implementations instead of mock implementations)</summary>
    [TestClass]
    public class PresentationTest : ApplicationsTest
    {
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationsModule());
            builder.RegisterModule(new MockPresentationModule(useMock: false));

            builder.RegisterType<FileService>().As<IFileService>().AsSelf().SingleInstance();
            builder.RegisterType<FileSystemWatcherService>().As<IFileSystemWatcherService>().AsSelf().SingleInstance();
            builder.RegisterType<MusicFileContext>().As<IMusicFileContext>().AsSelf().SingleInstance();
            builder.RegisterType<Transcoder>().As<ITranscoder>().AsSelf().SingleInstance();


        }
    }
}
