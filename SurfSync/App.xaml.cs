using SurfSync.Browsers;
using System.Windows;

using SurfSync.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SurfSync;

public partial class App : Application
{
    public App() { }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ErrorLogger.Initialize();
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        var mainWindow = new MainWindow(BrowserResolver.GetBrowsersInstances());

        mainWindow.Show();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            ErrorLogger.LogException(exception, "AppDomain.UnhandledException");
            return;
        }

        ErrorLogger.LogMessage("AppDomain.UnhandledException", e.ExceptionObject?.ToString() ?? "Unknown error object");
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        ErrorLogger.LogException(e.Exception, "TaskScheduler.UnobservedTaskException");
        e.SetObserved();
    }

    private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ErrorLogger.LogException(e.Exception, "Application.DispatcherUnhandledException");
    }
}
