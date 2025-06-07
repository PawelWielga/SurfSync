using SurfSync.Enums;

namespace SurfSync.Models;

public sealed class Profile
{
    public string Name { get; set; }
    public BrowserType BrowserType { get; set; }
    public bool IsRelative { get; set; }
    public string Path { get; set; }
    public bool Default { get; set; }
}