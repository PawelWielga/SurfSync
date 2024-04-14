using Newtonsoft.Json;
using System.IO;

namespace SurfSync.Config;

public sealed class ConfigReader
{
    public static string GetBrowserPath()
    {
        string relativePath = "Config\\config.json";
        string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        if (File.Exists(absolutePath))
        {
            string jsonContent = File.ReadAllText(absolutePath);
            BrowserConfig config = JsonConvert.DeserializeObject<BrowserConfig>(jsonContent);

            if (config != null && !string.IsNullOrWhiteSpace(config.BrowserPath))
            {
                return config.BrowserPath;
            }
        }

        return ReadBrowserPath();
    }

    public static string ReadBrowserPath()
    {
        //TODO: SHOW POPUP TO CHOSE FIREFOX.EXE PATH

        var path = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";

        //TODO: SAVE PATH TO CONFIG.JSON FILE

        return path;
    }
}