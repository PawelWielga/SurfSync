using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using SurfSync.Enums;
using SurfSync.Models;
using SurfSync.Config;

namespace SurfSync.Browser;

public sealed class ChromeService : IBrowserService
{
    public BrowserType BrowserType => BrowserType.chrome;
    public MainWindow MainWindow { get; set; }

    private readonly string _browserPath;
    private readonly string _localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _chromeLocalStatePath;

    private readonly List<Profile> _profiles;

    public ChromeService()
    {
        _browserPath = ConfigReader.GetBrowserPath(BrowserType);
        _chromeLocalStatePath = Path.Combine(_localAppDataPath, "Google", "Chrome", "User Data", "Local State");

        _profiles = DeserializeLocalStateFile(_chromeLocalStatePath);
    }

    private static List<Profile> DeserializeLocalStateFile(string localStateFilePath)
    {
        var profiles = new List<Profile>();

        if (!File.Exists(localStateFilePath))
        {
            // TODO: Can't find chrome profiles alert
            return profiles;
        }

        var json = File.ReadAllText(localStateFilePath);
        var localState = JObject.Parse(json);
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

    public List<Profile> GetProfiles() => _profiles;

    public void OpenBrowserProfileSettings()
    {
        Process.Start(_browserPath, "chrome://settings/manageProfile");
    }

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath, $"--profile-directory=\"{profile.Path}\"");
#if !DEBUG
        MainWindow?.Close();
#endif
    }
}