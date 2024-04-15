using SurfSync.Browsers;
using System.Windows;

namespace SurfSync;

public partial class App : Application
{
    public App() { }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = new MainWindow(BrowserResolver.GetBrowsersInstances());

        mainWindow.Show();
    }
}