using System.IO;
using System.Reflection;
using SurfSync.Browser;
using SurfSync.Models;
using System.Linq;

namespace SurfSync.Tests;

public class FirefoxServiceTests
{
    [Fact]
    public void DeserializeProfilesIniFile_ParsesProfiles()
    {
        var content = "[Profile0]\nName=Default\nIsRelative=1\nPath=path0\nDefault=1\n[Profile1]\nName=Other\nIsRelative=0\nPath=path1\nDefault=0";
        var file = Path.GetTempFileName();
        File.WriteAllText(file, content);

        var method = typeof(FirefoxService).GetMethod("DeserializeProfilesIniFile", BindingFlags.NonPublic | BindingFlags.Static);
        var profiles = (System.Collections.Generic.List<Profile>)method!.Invoke(null, new object[] { file });

        Assert.Equal(2, profiles.Count);
        var p0 = profiles.First(p => p.Path == "path0");
        Assert.True(p0.Default);
        Assert.Equal("Default", p0.Name);
        var p1 = profiles.First(p => p.Path == "path1");
        Assert.Equal("Other", p1.Name);
        Assert.False(p1.Default);
    }
}
