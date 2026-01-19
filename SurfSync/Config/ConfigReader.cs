using Newtonsoft.Json;
using SurfSync.Enums;
using System.IO;

namespace SurfSync.Config;

public sealed class ConfigReader
{
    private static readonly object ConfigLock = new();
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json");
    private static List<Browser> _configs;
    private static DateTime _configsWriteTimeUtc = DateTime.MinValue;

    public static string GetBrowserPath(BrowserType browserType)
    {
        EnsureConfigLoaded();

        var config = _configs?.FirstOrDefault(c => c.type == browserType);
        if (config != null && !string.IsNullOrWhiteSpace(config.path))
        {
            return config.path;
        }
        else
        {
            //TODO: show popup to chose selected browser type path and save to config
        }

        return GetDefaultBrowserPath(browserType);
    }

    private static void EnsureConfigLoaded()
    {
        if (!File.Exists(ConfigPath))
        {
            TryWriteDefaultConfig();
            if (!File.Exists(ConfigPath))
            {
                _configs = null;
                _configsWriteTimeUtc = DateTime.MinValue;
                return;
            }
        }

        var writeTimeUtc = File.GetLastWriteTimeUtc(ConfigPath);
        if (_configs != null && _configsWriteTimeUtc == writeTimeUtc)
            return;

        lock (ConfigLock)
        {
            writeTimeUtc = File.GetLastWriteTimeUtc(ConfigPath);
            if (_configs != null && _configsWriteTimeUtc == writeTimeUtc)
                return;

            try
            {
                var jsonContent = File.ReadAllText(ConfigPath);
                var config = JsonConvert.DeserializeObject<BrowsersConfig>(jsonContent);
                _configs = config?.browsers ?? new List<Browser>();
            }
            catch (JsonException)
            {
                _configs = new List<Browser>();
            }

            _configsWriteTimeUtc = writeTimeUtc;
        }
    }

    private static void TryWriteDefaultConfig()
    {
        try
        {
            var configDirectory = Path.GetDirectoryName(ConfigPath);
            if (string.IsNullOrWhiteSpace(configDirectory))
                return;

            Directory.CreateDirectory(configDirectory);

            var config = new BrowsersConfig
            {
                browsers = new List<Browser>
                {
                    new Browser { type = BrowserType.firefox, path = GetDefaultBrowserPath(BrowserType.firefox) },
                    new Browser { type = BrowserType.chrome, path = GetDefaultBrowserPath(BrowserType.chrome) },
                    new Browser { type = BrowserType.edge, path = GetDefaultBrowserPath(BrowserType.edge) },
                    new Browser { type = BrowserType.opera, path = GetDefaultBrowserPath(BrowserType.opera) },
                    new Browser { type = BrowserType.brave, path = GetDefaultBrowserPath(BrowserType.brave) }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigPath, jsonContent);
        }
        catch
        {
            // Ignore config write failures.
        }
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
}
