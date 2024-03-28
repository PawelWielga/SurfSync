using FirefoxProfileLauncher.Browser;
using FirefoxProfileLauncher.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FirefoxProfileLauncher;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IBrowserService>(new FirefoxService(ConfigReader.GetBrowserPath()));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = new MainWindow(_serviceProvider.GetRequiredService<IBrowserService>());
        mainWindow.Show();
    }
}