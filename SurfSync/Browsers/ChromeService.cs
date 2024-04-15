using SurfSync.Enums;
using SurfSync.Models;

namespace SurfSync.Browser;

public sealed class ChromeService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.chrome;
    public MainWindow MainWindow { get; set; }


    public ChromeService()
    {
        
    }

    public List<Profile> GetProfiles()
    {
        return new List<Profile>();
    }

    public void OpenBrowserProfileSettings()
    {
        return;
    }

    public void OpenBrowserWithProfile(Profile profile)
    {
        return;
    }
}