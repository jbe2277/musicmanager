using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Waf.UnitTesting;
using System.Waf.UnitTesting.Mocks;
using Test.MusicManager.Domain.UnitTesting;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Test.MusicManager.Applications.UnitTesting
{
    [TestClass]
    public abstract class ApplicationsTest : DomainTest
    {
        private AggregateCatalog? catalog;

        protected UnitTestSynchronizationContext Context { get; private set; } = null!;
        
        protected CompositionContainer Container { get; private set; } = null!;

        protected virtual UnitTestLevel UnitTestLevel => UnitTestLevel.UnitTest;

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
            AddApplicationCatalog(typeof(ShellViewModel));
            AddMockCatalog(typeof(MockMessageService));
            AddMockCatalog(typeof(ApplicationsTest));
        }

        protected void AddApplicationCatalog(Type typeInAssembly)
        {
            if (UnitTestLevel == UnitTestLevel.UnitTest)
            {
                AddCatalogCore(typeInAssembly, d => !UnitTestMetadata.IsContained(d));
            }
            else if (UnitTestLevel == UnitTestLevel.IntegrationTest)
            {
                AddCatalogCore(typeInAssembly, d => !UnitTestMetadata.IsContained(d) || UnitTestMetadata.IsContained(d, UnitTestMetadata.Data));
            }
        }

        protected void AddMockCatalog(Type typeInAssembly)
        {
            if (UnitTestLevel == UnitTestLevel.UnitTest)
            {
                AddCatalogCore(typeInAssembly);
            }
            else if (UnitTestLevel == UnitTestLevel.IntegrationTest)
            {
                AddCatalogCore(typeInAssembly, d => !UnitTestMetadata.IsContained(d) || !UnitTestMetadata.IsContained(d, UnitTestMetadata.Data));
            }
        }
        
        private void AddCatalogCore(Type typeInAssembly, Func<ComposablePartDefinition, bool>? filter = null)
        {
            catalog?.Catalogs.Add(filter == null ? new AssemblyCatalog(typeInAssembly.Assembly) : new FilteredCatalog(new AssemblyCatalog(typeInAssembly.Assembly), filter));
        }
    }
}
