using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Test.MusicManager.Domain;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Domain.MusicFiles;
using IContainer = Autofac.IContainer;

namespace Test.MusicManager.Applications;

[TestClass]
public abstract class ApplicationsTest : DomainTest
{
    protected UnitTestSynchronizationContext Context { get; private set; } = null!;
        
    protected IContainer Container { get; private set; } = null!;

    public T Get<T>() where T : notnull => Container.Resolve<T>();

    public Lazy<T> GetLazy<T>() where T : notnull => new(Get<T>);

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Context = UnitTestSynchronizationContext.Create();

        var builder = new ContainerBuilder();
        ConfigureContainer(builder);
        Container = builder.Build();

        var shellService = Container.Resolve<ShellService>();
        shellService.Settings = new AppSettings();

        ServiceLocator.RegisterInstance<IChangeTrackerService>(new ChangeTrackerService());
    }

    protected override void OnCleanup()
    {
        Container.Dispose();
        Context.Dispose();
        base.OnCleanup();
    }

    protected virtual void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new ApplicationsModule());
        builder.RegisterModule(new MockPresentationModule());
    }
}
