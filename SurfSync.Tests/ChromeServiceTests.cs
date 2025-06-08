using System;
using System.IO;
using System.Reflection;
using SurfSync.Browser;
using SurfSync.Models;
using System.Linq;

namespace SurfSync.Tests;

public class ChromeServiceTests
{
    [Fact]
    public void DeserializeLocalStateFile_ParsesProfiles()
    {
        var json = "{\"profile\":{\"info_cache\":{\"Profile 1\":{\"name\":\"User1\"},\"Profile 2\":{\"name\":\"User2\"}},\"last_used\":\"Profile 2\"}}";
        var file = Path.GetTempFileName();
        File.WriteAllText(file, json);

        var method = typeof(ChromeService).GetMethod("DeserializeLocalStateFile", BindingFlags.NonPublic | BindingFlags.Static);
        var profiles = (System.Collections.Generic.List<Profile>)method!.Invoke(null, new object[] { file });

        Assert.Equal(2, profiles.Count);
        var p1 = profiles.First(p => p.Path == "Profile 1");
        Assert.Equal("User1", p1.Name);
        Assert.False(p1.Default);
        var p2 = profiles.First(p => p.Path == "Profile 2");
        Assert.True(p2.Default);
    }
}
