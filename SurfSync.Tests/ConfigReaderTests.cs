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
}
