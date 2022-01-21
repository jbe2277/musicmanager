using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.UnitTesting;
using System.Waf.UnitTesting.Mocks;
using Test.MusicManager.Domain;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications;

[TestClass]
public abstract class ApplicationsTest : DomainTest
{
    private AggregateCatalog? catalog;

    protected UnitTestSynchronizationContext Context { get; private set; } = null!;
        
    protected CompositionContainer Container { get; private set; } = null!;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Context = UnitTestSynchronizationContext.Create();

        catalog = new AggregateCatalog();
        OnCatalogInitialize(catalog);

        Container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
        var batch = new CompositionBatch();
        batch.AddExportedValue(Container);
        Container.Compose(batch);

        var shellService = Container.GetExportedValue<ShellService>();
        shellService.Settings = new AppSettings();

        ServiceLocator.RegisterInstance<IChangeTrackerService>(new ChangeTrackerService());
    }

    protected override void OnCleanup()
    {
        Container.Dispose();
        catalog?.Dispose();
        Context.Dispose();
        base.OnCleanup();
    }

    protected virtual void OnCatalogInitialize(AggregateCatalog catalog)
    {
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(MockMessageService).Assembly));
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(ApplicationsTest).Assembly));
    }
}
