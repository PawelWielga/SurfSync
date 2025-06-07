using System.IO;
using System.Diagnostics;
using SurfSync.Models;
using SurfSync.Enums;
using SurfSync.Config;

namespace SurfSync.Browser;

public sealed class FirefoxService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.firefox;
    public MainWindow MainWindow { get; set; }

    private string _browserPath;

    private string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private string _firefoxProfilesPath;

    private List<Profile> _profiles;

    public FirefoxService()
    {
        _browserPath = ConfigReader.GetBrowserPath(BrowserType);

        _firefoxProfilesPath = Path.Combine(_appDataPath, "Mozilla", "Firefox", "profiles.ini");

        _profiles = DeserializeProfilesIniFile(_firefoxProfilesPath);
    }

    private static List<Profile> DeserializeProfilesIniFile(string profilesIniFilePath)
    {
        var profiles = new List<Profile>();

        if (!File.Exists(profilesIniFilePath))
        {
            // TODO: Can't find firefox profiles alert
            return profiles;
        }

        Profile? currentProfile = null;
        foreach (var line in File.ReadAllLines(profilesIniFilePath))
        {
            if (line.StartsWith("[Profile"))
            {
                currentProfile = new Profile
                {
                    BrowserType = BrowserType.firefox
                };
                profiles.Add(currentProfile);
                continue;
            }

            if (currentProfile is null)
                continue;

            if (line.StartsWith("Name"))
            {
                currentProfile.Name = line.Split('=')[1];
            }
            else if (line.StartsWith("IsRelative"))
            {
                currentProfile.IsRelative = int.Parse(line.Split('=')[1]) == 1;
            }
            else if (line.StartsWith("Path"))
            {
                currentProfile.Path = line.Split('=')[1];
            }
            else if (line.StartsWith("Default"))
            {
                currentProfile.Default = int.Parse(line.Split('=')[1]) == 1;
            }
        }

        return profiles;
    }

    public List<Profile> GetProfiles() => _profiles;

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath, $"-P {profile.Name}");
#if !DEBUG
        MainWindow?.Close();
#endif
    }

    public void OpenBrowserProfileSettings()
    {
        Process.Start(_browserPath, "-P");
    }


}
