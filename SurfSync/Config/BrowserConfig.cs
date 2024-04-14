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

    public static BrowsersConfig FromJson(string json)
    {
        return JsonConvert.DeserializeObject<BrowsersConfig>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}