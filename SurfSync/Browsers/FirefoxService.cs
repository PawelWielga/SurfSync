using System.IO;
using System.Diagnostics;
using SurfSync.Models;
using SurfSync.Enums;
using SurfSync.Config;
using SurfSync.Logging;

namespace SurfSync.Browser;

public sealed class FirefoxService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.firefox;
    public MainWindow MainWindow { get; set; }

    private readonly Lazy<string> _browserPath;

    private readonly string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private readonly string _firefoxProfilesPath;

    private readonly Lazy<List<Profile>> _profiles;

    public FirefoxService()
    {
        _browserPath = new Lazy<string>(() => ConfigReader.GetBrowserPath(BrowserType));

        _firefoxProfilesPath = Path.Combine(_appDataPath, "Mozilla", "Firefox", "profiles.ini");

        _profiles = new Lazy<List<Profile>>(() => DeserializeProfilesIniFile(_firefoxProfilesPath));
    }

    private static List<Profile> DeserializeProfilesIniFile(string profilesIniFilePath)
    {
        var profiles = new List<Profile>();

        if (!File.Exists(profilesIniFilePath))
        {
            // TODO: Can't find firefox profiles alert
            return profiles;
        }

        string[] lines;
        try
        {
            lines = File.ReadAllLines(profilesIniFilePath);
        }
        catch (IOException ex)
        {
            ErrorLogger.LogException(ex, "Firefox profiles.ini read failed");
            return profiles;
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorLogger.LogException(ex, "Firefox profiles.ini access denied");
            return profiles;
        }

        Profile currentProfile = null;
        foreach (var line in lines)
        {
            if (line.StartsWith("[Profile", StringComparison.Ordinal))
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

            if (line.StartsWith("Name=", StringComparison.Ordinal))
            {
                currentProfile.Name = line.Substring("Name=".Length);
            }
            else if (line.StartsWith("IsRelative=", StringComparison.Ordinal))
            {
                var value = line.Substring("IsRelative=".Length);
                if (int.TryParse(value, out var parsed))
                    currentProfile.IsRelative = parsed == 1;
            }
            else if (line.StartsWith("Path=", StringComparison.Ordinal))
            {
                currentProfile.Path = line.Substring("Path=".Length);
            }
            else if (line.StartsWith("Default=", StringComparison.Ordinal))
            {
                var value = line.Substring("Default=".Length);
                if (int.TryParse(value, out var parsed))
                    currentProfile.Default = parsed == 1;
            }
        }

        return profiles;
    }

    public List<Profile> GetProfiles() => _profiles.Value;

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath.Value, $"-P {profile.Name}");
#if !DEBUG
        MainWindow?.Close();
#endif
    }

    public void OpenBrowserProfileSettings()
    {
        Process.Start(_browserPath.Value, "-P");
    }


}
