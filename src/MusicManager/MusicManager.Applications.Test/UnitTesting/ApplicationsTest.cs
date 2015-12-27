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

namespace Test.MusicManager.Applications.UnitTesting
{
    [TestClass]
    public abstract class ApplicationsTest : DomainTest
    {
        private UnitTestSynchronizationContext context;
        private AggregateCatalog catalog;
        private CompositionContainer container;
        

        protected UnitTestSynchronizationContext Context { get { return context; } } 
        
        protected CompositionContainer Container { get { return container; } }

        protected virtual UnitTestLevel UnitTestLevel { get { return UnitTestLevel.UnitTest; } }


        protected override void OnInitialize()
        {
            base.OnInitialize();

            context = UnitTestSynchronizationContext.Create();

            catalog = new AggregateCatalog();
            OnCatalogInitialize(catalog);

            container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            ShellService shellService = Container.GetExportedValue<ShellService>();
            shellService.Settings = new AppSettings();
        }

        protected override void OnCleanup()
        {
            container.Dispose();
            catalog.Dispose();
            context.Dispose();

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
        
        private void AddCatalogCore(Type typeInAssembly, Func<ComposablePartDefinition, bool> filter = null)
        {
            if (filter == null)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(typeInAssembly.Assembly));
            }
            else
            {
                catalog.Catalogs.Add(new FilteredCatalog(new AssemblyCatalog(typeInAssembly.Assembly), filter));
            }
        }
    }
}
