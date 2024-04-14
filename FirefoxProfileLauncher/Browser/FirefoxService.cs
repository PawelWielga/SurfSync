using System.IO;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FirefoxProfileLauncher.Browser;

public sealed class FirefoxService : IBrowserService
{
    public MainWindow MainWindow { get; set; }

    private string _browserPath;
    private string _browserProcessName = "Firefox";
    private string _browserProfilesProcessName = "Firefox - ";

    private string _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private string _firefoxProfilesPath;
    private List<Profile> _profiles;

    public FirefoxService(string browserPath)
    {
        _browserPath = browserPath;

        _firefoxProfilesPath = Path.Combine(_appDataPath, "Mozilla", "Firefox", "profiles.ini");

        _profiles = DeserializrProfilesIniFile(_firefoxProfilesPath);
    }

    private List<Profile> DeserializrProfilesIniFile(string profilesIniFilePath)
    {
        var profiles = new List<Profile>();

        foreach (var line in File.ReadAllLines(profilesIniFilePath))
        {
            if (line.StartsWith("[Profile"))
            {
                var profile = new Profile();
                foreach (var innerLine in File.ReadAllLines(profilesIniFilePath).SkipWhile(l => l != line).Skip(1))
                {
                    if (innerLine.StartsWith("Name"))
                    {
                        profile.Name = innerLine.Split('=')[1];
                        continue;
                    }
                    if (innerLine.StartsWith("IsRelative"))
                    {
                        profile.IsRelative = int.Parse(innerLine.Split("=")[1]) == 1;
                        continue;
                    }
                    if (innerLine.StartsWith("Path"))
                    {
                        profile.Path = innerLine.Split('=')[1];
                        continue;
                    }
                    if (innerLine.StartsWith("Default"))
                    {
                        profile.Default = int.Parse(innerLine.Split("=")[1]) == 1;
                        continue;
                    }
                    break;
                }
                profiles.Add(profile);
            }
        }

        return profiles;
    }

    public List<Profile> GetProfiles() => _profiles;

    public void OpenBrowserWithProfile(Profile profile)
    {
        Process.Start(_browserPath, $"-P {profile.Name}");
#if !DEBUG
        MainWindow?.Close();
#endif
    }

    public void OpenBrowserProfileSettings()
    {
        var startInfo = new ProcessStartInfo(_browserPath, "-P");
        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        process.Exited += Process_Exited;
        process.Start();
    }

    private void Process_Exited(object sender, EventArgs e)
    {
        Thread.Sleep(100);
        var x = Process.GetProcessesByName(_browserProcessName);
        var asd = x.FirstOrDefault(a => 
        a.MainWindowTitle.Substring(0, Math.Min(10, a.MainWindowTitle.Length)) == _browserProfilesProcessName);
        asd.Exited += Process_Exited_2;
        Console.WriteLine("test");
    }

    private void Process_Exited_2(object sender, EventArgs e)
    {
        Console.WriteLine("test");
    }
}
