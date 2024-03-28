namespace FirefoxProfileLauncher.Browser;

public sealed class Profile()
{
    public string Name { get; set; }
    public bool IsRelative { get; set; }
    public string Path { get; set; }
    public bool Default { get; set; }
}