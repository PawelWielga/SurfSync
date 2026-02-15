using Newtonsoft.Json;
using SurfSync.Enums;

namespace SurfSync.Config;

public class Browser
{
    [JsonProperty("type")]
    public BrowserType type;

    [JsonProperty("path")]
    public string path;
}

public class BrowsersConfig
{
    [JsonProperty("browsers")]
    public List<Browser> browsers;

    [JsonProperty("visibleBrowsers")]
    public List<BrowserType> visibleBrowsers;

    [JsonProperty("hiddenFirefoxProfiles")]
    public List<string> hiddenFirefoxProfiles;

    [JsonProperty("profileVisualPreferences")]
    public List<ProfileVisualPreference> profileVisualPreferences;

    public static BrowsersConfig FromJson(string json)
    {
        return JsonConvert.DeserializeObject<BrowsersConfig>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class ProfileVisualPreference
{
    [JsonProperty("browserType")]
    public BrowserType browserType;

    [JsonProperty("profileName")]
    public string profileName;

    [JsonProperty("circleColor")]
    public string circleColor;

    [JsonProperty("textColor")]
    public string textColor;
}
