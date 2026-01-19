using System.Diagnostics;
using SurfSync.Enums;
using SurfSync.Models;
using SurfSync.Config;

namespace SurfSync.Browser;

public sealed class EdgeService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.edge;
    public MainWindow MainWindow { get; set; }

    private readonly Lazy<string> _browserPath;

    public EdgeService()
    {
        _browserPath = new Lazy<string>(() => ConfigReader.GetBrowserPath(BrowserType));
    }

    public List<Profile> GetProfiles() => new List<Profile>();

    public void OpenBrowserProfileSettings()
    {
        // Not implemented yet
    }

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath.Value);
#if !DEBUG
        MainWindow?.Close();
#endif
    }
}
