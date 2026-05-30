using Autofac;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.Globalization;
using System.IO;
using System.Waf.Applications;
using System.Waf.Presentation;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Waf.MusicManager.Applications;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Presentation.Services;
using IContainer = Autofac.IContainer;

namespace Waf.MusicManager.Presentation;

public partial class App
{
    private static readonly IReadOnlyList<(string name, LogLevel level)> logSettings =
    [
        ("App", LogLevel.Info),
        ("MusicManager.*", LogLevel.Warn),
    ];

    private IContainer? container;
    private IReadOnlyList<IModuleController> moduleControllers = [];

    public App()
    {
        LogManager.Setup().LoadConfiguration(c =>
        {
            c.Configuration.DefaultCultureInfo = CultureInfo.InvariantCulture;
            var layout = "${date:universalTime=true:format=yyyy-MM-dd HH\\:mm\\:ss.ff} [${level:format=FirstCharacter}] ${processid} ${logger} ${message} ${exception}";
            var fileTarget = c.ForTarget("fileTarget").WriteTo(new AtomicFileTarget
            {
                FileName = Path.Combine(EnvironmentService.LogPath, "App.log"),
                Layout = layout,
                ArchiveAboveSize = 5_000_000,  // 5 MB
                MaxArchiveFiles = 2,
            }).WithAsync(AsyncTargetWrapperOverflowAction.Block);

            var traceTarget = c.ForTarget("traceTarget").WriteTo(new TraceTarget
            {
                Layout = layout,
                RawWrite = true
            }).WithAsync(AsyncTargetWrapperOverflowAction.Block);

            foreach (var (loggerNamePattern, minLevel) in logSettings)
            {
                c.ForLogger(loggerNamePattern).FilterMinLevel(minLevel).WriteTo(fileTarget).WriteTo(traceTarget);
            }
        });
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Log.App.Info("{0} {1} is starting; OS: {2}; .NET: {3}", ApplicationInfo.ProductName, ApplicationInfo.Version, Environment.OSVersion, Environment.Version);

#if !DEBUG
        DispatcherUnhandledException += AppDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif
        var builder = new ContainerBuilder();
        builder.RegisterModule(new ApplicationsModule());
        builder.RegisterModule(new PresentationModule());
        container = builder.Build();

        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        moduleControllers = container.Resolve<IReadOnlyList<IModuleController>>();
        foreach (var x in moduleControllers) x.Initialize();
        foreach (var x in moduleControllers) x.Run();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        foreach (var x in moduleControllers.Reverse()) x.Shutdown();
        if (container is not null)
        {
            var shellService = container.Resolve<IShellService>();
            var tasksToWait = shellService.TasksToCompleteBeforeShutdown.ToArray();
            while (tasksToWait.Any(t => !t.IsCompleted))  // Wait until all registered tasks are finished
            {
                DispatcherHelper.DoEvents();
                Thread.Sleep(25);
            }
        }
        container?.Dispose();
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
