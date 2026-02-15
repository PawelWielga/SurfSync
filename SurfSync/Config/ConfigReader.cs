using Newtonsoft.Json;
using SurfSync.Enums;
using System.IO;

namespace SurfSync.Config;

public sealed class ConfigReader
{
    private static readonly object ConfigLock = new();
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
    private static BrowsersConfig _config;
    private static DateTime _configWriteTimeUtc = DateTime.MinValue;

    public static void EnsureConfigExists()
    {
        if (File.Exists(ConfigPath))
            return;

        lock (ConfigLock)
        {
            if (File.Exists(ConfigPath))
                return;

            TryWriteDefaultConfig();
        }
    }

    public static string GetBrowserPath(BrowserType browserType)
    {
        var config = GetConfig();
        var browserConfig = config.browsers?.FirstOrDefault(c => c.type == browserType);
        if (browserConfig != null && !string.IsNullOrWhiteSpace(browserConfig.path))
        {
            return browserConfig.path;
        }

        // TODO: show popup to choose selected browser type path and save to config
        return GetDefaultBrowserPath(browserType);
    }

    public static IReadOnlyCollection<BrowserType> GetVisibleBrowsers()
    {
        var config = GetConfig();
        if (config.visibleBrowsers is null)
            return GetAllBrowserTypes().ToList();

        return config.visibleBrowsers.Distinct().ToList();
    }

    public static void SetVisibleBrowsers(IEnumerable<BrowserType> browsers)
    {
        var selectedBrowsers = (browsers ?? Enumerable.Empty<BrowserType>())
            .Distinct()
            .ToList();

        lock (ConfigLock)
        {
            var config = LoadConfigNoLock();
            config.visibleBrowsers = selectedBrowsers;
            SaveConfigNoLock(config);
        }
    }

    public static IReadOnlyCollection<string> GetHiddenFirefoxProfiles()
    {
        var config = GetConfig();
        return NormalizeHiddenProfiles(config.hiddenFirefoxProfiles);
    }

    public static void SetHiddenFirefoxProfiles(IEnumerable<string> profileNames)
    {
        var selectedProfiles = NormalizeHiddenProfiles(profileNames);

        lock (ConfigLock)
        {
            var config = LoadConfigNoLock();
            config.hiddenFirefoxProfiles = selectedProfiles;
            SaveConfigNoLock(config);
        }
    }

    public static IReadOnlyCollection<ProfileVisualPreference> GetProfileVisualPreferences()
    {
        var config = GetConfig();
        return NormalizeProfileVisualPreferences(config.profileVisualPreferences);
    }

    public static IReadOnlyDictionary<string, ProfileVisualPreference> GetProfileVisualPreferencesByKey()
    {
        return GetProfileVisualPreferences()
            .ToDictionary(
                preference => BuildProfileVisualKey(preference.browserType, preference.profileName),
                preference => preference,
                StringComparer.OrdinalIgnoreCase);
    }

    public static void SetProfileVisualPreferences(IEnumerable<ProfileVisualPreference> preferences)
    {
        var normalizedPreferences = NormalizeProfileVisualPreferences(preferences);

        lock (ConfigLock)
        {
            var config = LoadConfigNoLock();
            config.profileVisualPreferences = normalizedPreferences;
            SaveConfigNoLock(config);
        }
    }

    private static BrowsersConfig GetConfig()
    {
        lock (ConfigLock)
        {
            return LoadConfigNoLock();
        }
    }

    private static BrowsersConfig LoadConfigNoLock()
    {
        if (!File.Exists(ConfigPath))
        {
            TryWriteDefaultConfig();
            if (!File.Exists(ConfigPath))
            {
                _config = CreateDefaultConfig();
                _configWriteTimeUtc = DateTime.MinValue;
                return _config;
            }
        }

        var writeTimeUtc = File.GetLastWriteTimeUtc(ConfigPath);
        if (_config != null && _configWriteTimeUtc == writeTimeUtc)
            return _config;

        try
        {
            var jsonContent = File.ReadAllText(ConfigPath);
            _config = JsonConvert.DeserializeObject<BrowsersConfig>(jsonContent) ?? new BrowsersConfig();
        }
        catch (JsonException)
        {
            _config = new BrowsersConfig();
        }

        _config.browsers ??= new List<Browser>();
        _config.visibleBrowsers = _config.visibleBrowsers?.Distinct().ToList();
        _config.hiddenFirefoxProfiles = NormalizeHiddenProfiles(_config.hiddenFirefoxProfiles);
        _config.profileVisualPreferences = NormalizeProfileVisualPreferences(_config.profileVisualPreferences);
        _configWriteTimeUtc = writeTimeUtc;

        return _config;
    }

    private static void TryWriteDefaultConfig()
    {
        try
        {
            SaveConfigNoLock(CreateDefaultConfig());
        }
        catch
        {
            // Ignore config write failures.
        }
    }

    private static void SaveConfigNoLock(BrowsersConfig config)
    {
        config.browsers ??= new List<Browser>();
        config.visibleBrowsers ??= new List<BrowserType>();
        config.hiddenFirefoxProfiles = NormalizeHiddenProfiles(config.hiddenFirefoxProfiles);
        config.profileVisualPreferences = NormalizeProfileVisualPreferences(config.profileVisualPreferences);

        var configDirectory = Path.GetDirectoryName(ConfigPath);
        if (string.IsNullOrWhiteSpace(configDirectory))
            return;

        Directory.CreateDirectory(configDirectory);

        var jsonContent = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(ConfigPath, jsonContent);

        _config = config;
        _configWriteTimeUtc = File.GetLastWriteTimeUtc(ConfigPath);
    }

    private static BrowsersConfig CreateDefaultConfig()
    {
        var allBrowsers = GetAllBrowserTypes();
        return new BrowsersConfig
        {
            browsers = allBrowsers
                .Select(type => new Browser
                {
                    type = type,
                    path = GetDefaultBrowserPath(type)
                })
                .ToList(),
            visibleBrowsers = allBrowsers.ToList(),
            hiddenFirefoxProfiles = new List<string>(),
            profileVisualPreferences = new List<ProfileVisualPreference>()
        };
    }

    private static IEnumerable<BrowserType> GetAllBrowserTypes()
    {
        return Enum.GetValues<BrowserType>();
    }

    private static string GetDefaultBrowserPath(BrowserType browserType)
    {
        return browserType switch
        {
            BrowserType.firefox => "C:\\Program Files\\Mozilla Firefox\\firefox.exe",
            BrowserType.chrome => "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
            BrowserType.edge => "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe",
            BrowserType.opera => "C:\\Program Files\\Opera\\opera.exe",
            BrowserType.brave => "C:\\Program Files\\BraveSoftware\\Brave-Browser\\Application\\brave.exe",
            _ => string.Empty
        };
    }

    private static List<string> NormalizeHiddenProfiles(IEnumerable<string> profileNames)
    {
        return (profileNames ?? Enumerable.Empty<string>())
            .Select(name => name?.Trim())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string BuildProfileVisualKey(BrowserType browserType, string profileName)
    {
        return $"{browserType}:{profileName?.Trim()}".ToLowerInvariant();
    }

    private static List<ProfileVisualPreference> NormalizeProfileVisualPreferences(IEnumerable<ProfileVisualPreference> preferences)
    {
        return (preferences ?? Enumerable.Empty<ProfileVisualPreference>())
            .Where(preference => preference is not null)
            .Select(preference => new ProfileVisualPreference
            {
                browserType = preference.browserType,
                profileName = preference.profileName?.Trim(),
                circleColor = NormalizeColor(preference.circleColor),
                textColor = NormalizeColor(preference.textColor)
            })
            .Where(preference => !string.IsNullOrWhiteSpace(preference.profileName))
            .GroupBy(preference => BuildProfileVisualKey(preference.browserType, preference.profileName), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Last())
            .Where(preference => !string.IsNullOrWhiteSpace(preference.circleColor) || !string.IsNullOrWhiteSpace(preference.textColor))
            .ToList();
    }

    private static string NormalizeColor(string color)
    {
        return string.IsNullOrWhiteSpace(color)
            ? string.Empty
            : color.Trim();
    }
}
