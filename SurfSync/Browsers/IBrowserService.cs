using SurfSync.Enums;
using SurfSync.Models;

namespace SurfSync.Browser;

public interface IBrowserService
{
    public BrowserType BrowserType { get; }
    MainWindow MainWindow { get; set; }

    List<Profile> GetProfiles();
    void OpenBrowserWithProfile(Profile profile);
    void OpenBrowserProfileSettings();
}

