using Newtonsoft.Json;

namespace SurfSync.Config;

public sealed class BrowserConfig
{
    [JsonProperty("browserPath")]
    public string BrowserPath { get; set; }

    public static BrowserConfig FromJson(string json)
    {
        return JsonConvert.DeserializeObject<BrowserConfig>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}