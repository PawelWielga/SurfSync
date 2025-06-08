using System.Linq;
using SurfSync.Browsers;
using SurfSync.Browser;

namespace SurfSync.Tests;

public class BrowserResolverTests
{
    [Fact]
    public void GetBrowsersInstances_ReturnsAllBrowserServices()
    {
        var browsers = BrowserResolver.GetBrowsersInstances();

        Assert.NotEmpty(browsers);
        Assert.Equal(5, browsers.Count);
        Assert.Contains(browsers, b => b is ChromeService);
        Assert.Contains(browsers, b => b is FirefoxService);
        Assert.Contains(browsers, b => b is EdgeService);
        Assert.Contains(browsers, b => b is OperaService);
        Assert.Contains(browsers, b => b is BraveService);
    }
}
