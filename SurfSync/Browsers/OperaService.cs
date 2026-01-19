using System.Diagnostics;
using SurfSync.Enums;
using SurfSync.Models;
using SurfSync.Config;

namespace SurfSync.Browser;

public sealed class OperaService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.opera;
    public MainWindow MainWindow { get; set; }

    private readonly Lazy<string> _browserPath;

    public OperaService()
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
