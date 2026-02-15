using System;
using System.IO;
using SurfSync.Config;
using SurfSync.Enums;

namespace SurfSync.Tests;

public class ConfigReaderTests
{
    [Fact]
    public void GetBrowserPath_ReturnsValueFromConfig()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configDir = Path.Combine(baseDir, "Config");
        Directory.CreateDirectory(configDir);
        var configPath = Path.Combine(configDir, "config.json");

        File.WriteAllText(configPath, "{\"browsers\":[{\"type\":\"firefox\",\"path\":\"C:/test/firefox.exe\"}]}");

        var path = ConfigReader.GetBrowserPath(BrowserType.firefox);

        Assert.Equal("C:/test/firefox.exe", path.Replace("\\", "/"));
    }

    [Fact]
    public void SetProfileVisualPreferences_SavesPreferencesAndReturnsByKey()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configDir = Path.Combine(baseDir, "Config");
        Directory.CreateDirectory(configDir);
        var configPath = Path.Combine(configDir, "config.json");

        File.WriteAllText(configPath, "{\"browsers\":[],\"visibleBrowsers\":[],\"hiddenFirefoxProfiles\":[],\"profileVisualPreferences\":[]}");

        ConfigReader.SetProfileVisualPreferences(new[]
        {
            new ProfileVisualPreference
            {
                browserType = BrowserType.firefox,
                profileName = "default",
                circleColor = "#FF3A8DFF",
                textColor = "#FFFFFFFF"
            }
        });

        var preferences = ConfigReader.GetProfileVisualPreferencesByKey();
        var key = ConfigReader.BuildProfileVisualKey(BrowserType.firefox, "DEFAULT");

        Assert.True(preferences.TryGetValue(key, out var visualPreference));
        Assert.Equal("#FF3A8DFF", visualPreference.circleColor);
        Assert.Equal("#FFFFFFFF", visualPreference.textColor);
    }
}
