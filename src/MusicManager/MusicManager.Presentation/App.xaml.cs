using NLog;
using NLog.Config;
using NLog.Targets;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.Presentation;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Presentation.Services;

namespace Waf.MusicManager.Presentation;

public partial class App
{
    private static readonly Tuple<string, LogLevel>[] logSettings =
    {
        Tuple.Create("App", LogLevel.Info),
        Tuple.Create("MusicManager.*", LogLevel.Warn),
    };

    private AggregateCatalog? catalog;
    private CompositionContainer? container;
    private IEnumerable<IModuleController> moduleControllers = Array.Empty<IModuleController>();

    public App()
    {
        var fileTarget = new FileTarget("fileTarget")
        {
            FileName = Path.Combine(EnvironmentService.LogPath, "App.log"),
            Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.ff} ${level} ${processid} ${logger} ${message}  ${exception:format=tostring}",
            ArchiveAboveSize = 1024 * 1024 * 5,  // 5 MB
            MaxArchiveFiles = 2,
        };
        var logConfig = new LoggingConfiguration { DefaultCultureInfo = CultureInfo.InvariantCulture };
        logConfig.AddTarget(fileTarget);
        var maxLevel = LogLevel.AllLoggingLevels.Last();
        foreach (var x in logSettings) logConfig.AddRule(x.Item2, maxLevel, fileTarget, x.Item1);
        LogManager.Configuration = logConfig;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Log.App.Info("{0} {1} is starting; OS: {2}", ApplicationInfo.ProductName, ApplicationInfo.Version, Environment.OSVersion);

#if !DEBUG
        DispatcherUnhandledException += AppDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif
        catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(IMessageService).Assembly));
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
        container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
        var batch = new CompositionBatch();
        batch.AddExportedValue(container);
        container.Compose(batch);

        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        moduleControllers = container.GetExportedValues<IModuleController>();
        foreach (var x in moduleControllers) x.Initialize();
        foreach (var x in moduleControllers) x.Run();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        foreach (var x in moduleControllers.Reverse()) x.Shutdown();
        if (container is not null)
        {
            var shellService = container.GetExportedValue<IShellService>();
            var tasksToWait = shellService.TasksToCompleteBeforeShutdown.ToArray();
            while (tasksToWait.Any(t => !t.IsCompleted)) DispatcherHelper.DoEvents();  // Wait until all registered tasks are finished
        }
        container?.Dispose();
        catalog?.Dispose();
        base.OnExit(e);
    }

    private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) => HandleException(e.Exception, false);

    private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) => HandleException(e.ExceptionObject as Exception, e.IsTerminating);

    private static void HandleException(Exception? e, bool isTerminating)
    {
        if (e == null) return;
        Log.App.Error(e, "Unknown application error.");
        if (!isTerminating)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Presentation.Properties.Resources.UnknownError, e), ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
