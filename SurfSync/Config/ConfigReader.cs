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

        //TODO: temporary hardcoded
        return "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
    }
}