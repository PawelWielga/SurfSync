namespace SurfSync.Browser;

public interface IBrowserService
{
    MainWindow MainWindow { get; set; }

    List<Profile> GetProfiles();
    void OpenBrowserWithProfile(Profile profile);
    void OpenBrowserProfileSettings();
}
