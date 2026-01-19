using SurfSync.Browser;
namespace SurfSync.Browsers;

public static class BrowserResolver
{
    public static List<IBrowserService> GetBrowsersInstances()
    {
        return new List<IBrowserService>
        {
            new ChromeService(),
            new FirefoxService(),
            new EdgeService(),
            new OperaService(),
            new BraveService()
        };
    }
}
