using Newtonsoft.Json;
using SurfSync.Enums;
using System.IO;

namespace SurfSync.Config;

public sealed class ConfigReader
{
    private static List<Browser> Configs { get; set; }

    public static string GetBrowserPath(BrowserType browserType)
    {
        string relativePath = "Config\\config.json";
        string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        if (File.Exists(absolutePath))
        {
            string jsonContent = File.ReadAllText(absolutePath);
            Configs = JsonConvert.DeserializeObject<BrowsersConfig>(jsonContent).browsers;

            var config = Configs.FirstOrDefault(c => c.type == browserType);
            if (config != null && !string.IsNullOrWhiteSpace(config.path))
            {
                return config.path;
            }
            else
            {
                //TODO: show popup to chose selected browser type path and save to config
            }
        }

        //TODO: temporary hardcoded defaults
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