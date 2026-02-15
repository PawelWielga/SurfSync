using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SurfSync.Config;
using SurfSync.Enums;
using SurfSync.Logging;
using SurfSync.Models;

namespace SurfSync.Browser;

public sealed class ChromeService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.chrome;
    public MainWindow MainWindow { get; set; }

    private readonly Lazy<string> _browserPath;
    private readonly string _localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _chromeLocalStatePath;

    public ChromeService()
    {
        _browserPath = new Lazy<string>(() => ConfigReader.GetBrowserPath(BrowserType));
        _chromeLocalStatePath = Path.Combine(_localAppDataPath, "Google", "Chrome", "User Data", "Local State");
    }

    private static List<Profile> DeserializeLocalStateFile(string localStateFilePath)
    {
        var profiles = new List<Profile>();

        if (!File.Exists(localStateFilePath))
        {
            // TODO: Can't find chrome profiles alert
            return profiles;
        }

        JObject localState;
        try
        {
            var json = File.ReadAllText(localStateFilePath);
            localState = JObject.Parse(json);
        }
        catch (IOException ex)
        {
            ErrorLogger.LogException(ex, "Chrome Local State read failed");
            return profiles;
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorLogger.LogException(ex, "Chrome Local State access denied");
            return profiles;
        }
        catch (JsonException ex)
        {
            ErrorLogger.LogException(ex, "Chrome Local State parse failed");
            return profiles;
        }

        var profileSection = localState["profile"];
        var infoCache = profileSection?["info_cache"] as JObject;
        var lastUsed = profileSection?["last_used"]?.ToString();

        if (infoCache == null)
            return profiles;

        foreach (var property in infoCache.Properties())
        {
            var data = property.Value;
            profiles.Add(new Profile
            {
                BrowserType = BrowserType.chrome,
                Name = data?["name"]?.ToString() ?? property.Name,
                IsRelative = true,
                Path = property.Name,
                Default = property.Name == lastUsed
            });
        }

        return profiles;
    }

    public List<Profile> GetProfiles() => DeserializeLocalStateFile(_chromeLocalStatePath);

    public void OpenBrowserProfileSettings()
    {
        Process.Start(_browserPath.Value, "chrome://settings/manageProfile");
    }

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath.Value, $"--profile-directory=\"{profile.Path}\"");
#if !DEBUG
        MainWindow?.Close();
#endif
    }
}
