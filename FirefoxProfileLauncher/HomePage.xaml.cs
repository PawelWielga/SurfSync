using FirefoxProfileLauncher.Browser;
using FirefoxProfileLauncher.Components;
using System.Windows.Controls;
using System.Windows.Input;

namespace FirefoxProfileLauncher;

public partial class HomePage : Page
{
    private MainWindow MainWindow { get; init; }
    private IBrowserService _browserService { get; init; }

    public HomePage(MainWindow mainWindow, IBrowserService browserService)
    {
        MainWindow = mainWindow;
        _browserService = browserService;
        _browserService.MainWindow = MainWindow;

        InitializeComponent();

        PrepareProfiles();
    }

    private void PrepareProfiles()
    {
        var profiles = _browserService.GetProfiles();
        foreach (var profile in profiles)
        {
            ProfilesContainer.Children.Add(new UserProfileComponent(profile, _browserService.OpenBrowserWithProfile));
        }
    }

    private void Image_MouseDown(object sender, MouseButtonEventArgs e) => _browserService.OpenBrowserProfileSettings();
}