using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Waf.UnitTesting.Mocks;
using Test.MusicManager.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Presentation.Services;

namespace Test.MusicManager.Presentation
{
    /// <summary>Integration tests (uses real service implementations instead of mock implementations)</summary>
    [TestClass]
    public class PresentationTest : ApplicationsTest
    {
        // List of exports which must use the real implementation instead of the mock (integration test)
        private static readonly Type[] exportNames = [
            typeof(IFileService),
            typeof(IFileSystemWatcherService), typeof(FileSystemWatcherService),
            typeof(IMusicFileContext), typeof(MusicFileContext),
            typeof(ITranscoder), typeof(Transcoder),
        ];

        protected override void OnCatalogInitialize(AggregateCatalog catalog)
        {
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MockMessageService).Assembly));
            catalog.Catalogs.Add(new FilteredCatalog(new AssemblyCatalog(typeof(ApplicationsTest).Assembly), x => !IsOneOfContractNames(x)));
            catalog.Catalogs.Add(new FilteredCatalog(new AssemblyCatalog(typeof(FileSystemWatcherService).Assembly), IsOneOfContractNames));

            static bool IsOneOfContractNames(ComposablePartDefinition d) => d.ExportDefinitions.Any(x => exportNames.Any(y => y.FullName == x.ContractName));
        }
    }
}
