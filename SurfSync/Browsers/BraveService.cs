using System.Diagnostics;
using SurfSync.Enums;
using SurfSync.Models;
using SurfSync.Config;

namespace SurfSync.Browser;

public sealed class BraveService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.brave;
    public MainWindow MainWindow { get; set; }

    private readonly string _browserPath;

    public BraveService()
    {
        _browserPath = ConfigReader.GetBrowserPath(BrowserType);
    }

    public List<Profile> GetProfiles() => new List<Profile>();

    public void OpenBrowserProfileSettings()
    {
        // Not implemented yet
    }

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath);
#if !DEBUG
        MainWindow?.Close();
#endif
    }
}
