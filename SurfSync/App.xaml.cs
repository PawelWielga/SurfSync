using SurfSync.Browser;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using SurfSync.Config;
using SurfSync.Enums;
using SurfSync.Browsers;

namespace SurfSync;

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
        services.AddTransient<IBrowserService>(s => new FirefoxService(ConfigReader.GetBrowserPath(BrowserType.firefox)));
        services.AddTransient<IBrowserService>(s => new ChromeService(ConfigReader.GetBrowserPath(BrowserType.chrome)));

        services.AddTransient<BrowserResolver>(serviceProvider => key =>
        {
            switch (key)
            {
                case BrowserType.firefox:
                    return serviceProvider.GetService<FirefoxService>();
                case BrowserType.chrome:
                    return serviceProvider.GetService<ChromeService>();
                default:
                    throw new KeyNotFoundException();
            }
        });
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = new MainWindow(
            _serviceProvider.GetService<BrowserResolver>(), 
            _serviceProvider.GetServices<IBrowserService>()
            );

        mainWindow.Show();
    }
}